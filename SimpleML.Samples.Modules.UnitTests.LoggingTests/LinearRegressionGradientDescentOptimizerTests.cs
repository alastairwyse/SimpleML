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
using NMock2.Matchers;
using ApplicationLogging;
using SimpleML.Containers;
using SimpleML.Samples.Modules;

namespace SimpleML.Samples.Modules.UnitTests.LoggingTests
{
    /// <summary>
    /// Tests for the logging functionality in class SimpleML.Samples.Modules.LinearRegressionGradientDescentOptimizer.
    /// </summary>
    public class LinearRegressionGradientDescentOptimizerTests
    {
        private Mockery mockery;
        private IApplicationLogger mockApplicationLogger;
        private LinearRegressionGradientDescentOptimizer testLinearRegressionGradientDescentOptimizer;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockApplicationLogger = mockery.NewMock<IApplicationLogger>();
            testLinearRegressionGradientDescentOptimizer = new LinearRegressionGradientDescentOptimizer();
            testLinearRegressionGradientDescentOptimizer.Logger = mockApplicationLogger;
        }

        /// <summary>
        /// Tests the logging functionality when the ImplementProcess() method is called with 'InitialThetaParameters' and 'TrainingSeriesData' input data with incompatible dimensions.
        /// </summary>
        [Test]
        public void ImplementProcess_TrainingSeriesDataAndInitialThetaParametersDimensionMismatch()
        {
            Matrix trainingSeriesData = new Matrix(4, 3, new Double[] { 1, 2, 3, 1, 3, 4, 1, 4, 5, 1, 5, 6 });
            Matrix trainingSeriesResults = new Matrix(4, 1, new Double[] { 7, 6, 5, 4 });
            Matrix initialthetaParameters = new Matrix(3, 1, new Double[] { 0.1, 0.2, 0.3 });
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("TrainingSeriesData").DataValue = trainingSeriesData;
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("TrainingSeriesResults").DataValue = trainingSeriesResults;
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("InitialThetaParameters").DataValue = initialthetaParameters;
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("LearningRate").DataValue = 0.1;
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("MaxIterations").DataValue = 200;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testLinearRegressionGradientDescentOptimizer, LogLevel.Critical, "The 'm' dimension of parameter 'InitialThetaParameters' must be 1 greater than the 'n' dimension of parameter 'TrainingSeriesData'.", new TypeMatcher(typeof(ArgumentException)));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testLinearRegressionGradientDescentOptimizer.Process();
            });

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests the logging functionality when an exception occurs in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess_Exception()
        {
            Matrix initialThetaParameters = new Matrix(2, 1, new Double[] { 9.0, 10.0 });
            Matrix trainingSeriesData = new Matrix(2, 1, new Double[] { 1.0, 2.0 });
            Matrix trainingSeriesResults = new Matrix(2, 2, new Double[] { 5.0, 6.0, 7.0, 8.0 });

            testLinearRegressionGradientDescentOptimizer.GetInputSlot("InitialThetaParameters").DataValue = initialThetaParameters;
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("TrainingSeriesData").DataValue = trainingSeriesData;
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("TrainingSeriesResults").DataValue = trainingSeriesResults;
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("LearningRate").DataValue = 0.1;
            testLinearRegressionGradientDescentOptimizer.GetInputSlot("MaxIterations").DataValue = 200;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testLinearRegressionGradientDescentOptimizer, LogLevel.Critical, "Error occurred whilst running gradient descent for linear regression.", new TypeMatcher(typeof(ArgumentException)));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testLinearRegressionGradientDescentOptimizer.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'trainingDataResults' must be a single column matrix (i.e. 'n' dimension equal to 1)."));
            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
