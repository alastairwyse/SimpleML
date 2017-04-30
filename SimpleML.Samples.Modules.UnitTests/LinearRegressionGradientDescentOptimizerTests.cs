/*
 * Copyright 2016 Alastair Wyse (http://www.oraclepermissiongenerator.net/simpleml/)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NMock2;
using ApplicationLogging;
using ApplicationMetrics;
using SimpleML;
using SimpleML.Containers;

namespace SimpleML.Samples.Modules.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.Samples.Modules.LinearRegressionGradientDescentOptimizer.
    /// </summary>
    public class LinearRegressionGradientDescentOptimizerTests
    {
        private Mockery mockery;
        private LinearRegressionGradientDescentOptimizer testLinearRegressionGradientDescentOptimizer;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            testLinearRegressionGradientDescentOptimizer = new LinearRegressionGradientDescentOptimizer();
            testLinearRegressionGradientDescentOptimizer.Logger = new NullApplicationLogger();
            testLinearRegressionGradientDescentOptimizer.MetricLogger = new NullMetricLogger();
        }

        /// <summary>
        /// Tests that an exception is thrown if the ImplementProcess() method is called with 'InitialThetaParameters' and 'TrainingSeriesData' input data with incompatible dimensions.
        /// </summary>
        [Test]
        public void ImplementProcess_TrainingSeriesDataAndInitialThetaParametersDimensionMismatch()
        {
            Matrix trainingSeriesData = new Matrix(4, 3, new Double[] { 1, 2, 3, 1, 3, 4, 1, 4, 5, 1, 5, 6 });
            Matrix trainingSeriesResults = new Matrix(4, 1, new Double[] { 7, 6, 5, 4 });
            Matrix thetaParameters = new Matrix(3, 1, new Double[] { 0.1, 0.2, 0.3 });
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("TrainingSeriesData").DataValue = trainingSeriesData;
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("TrainingSeriesResults").DataValue = trainingSeriesResults;
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("InitialThetaParameters").DataValue = thetaParameters;
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("LearningRate").DataValue = 0.1;
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("MaxIterations").DataValue = 5;

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testLinearRegressionGradientDescentOptimizer.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The 'm' dimension of parameter 'InitialThetaParameters' must be 1 greater than the 'n' dimension of parameter 'TrainingSeriesData'."));
            Assert.AreEqual("InitialThetaParameters", e.ParamName);
        }

        /// <summary>
        /// Success tests for the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            // Tests that a column of 1's is added to the left side of parameter 'TrainingSeriesData'

            Matrix initialThetaParameters = new Matrix(2, 1, new Double[] { 0.5, 0.5 });
            Matrix trainingSeriesData = new Matrix(2, 1, new Double[] { 5, 2 });
            Matrix trainingSeriesResults = new Matrix(2, 1, new Double[] { 1, 6 });
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("TrainingSeriesData").DataValue = trainingSeriesData;
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("TrainingSeriesResults").DataValue = trainingSeriesResults;
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("InitialThetaParameters").DataValue = initialThetaParameters;
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("LearningRate").DataValue = 0.1;
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("MaxIterations").DataValue = 5;

            testLinearRegressionGradientDescentOptimizer.Process();

            Matrix optimizedThetaParameters = (Matrix)testLinearRegressionGradientDescentOptimizer.GetOutputSlot("OptimizedThetaParameters").DataValue;

            Assert.AreEqual(2, optimizedThetaParameters.MDimension);
            Assert.AreEqual(1, optimizedThetaParameters.NDimension);
            Assert.That(optimizedThetaParameters.GetElement(1, 1), NUnit.Framework.Is.EqualTo(1.1257075).Within(1e-7));
            Assert.That(optimizedThetaParameters.GetElement(2, 1), NUnit.Framework.Is.EqualTo(0.33415265625).Within(1e-12));
        }
    }
}
