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
using ApplicationLogging;
using SimpleML;
using SimpleML.Containers;

namespace SimpleML.Samples.Modules.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.Samples.Modules.LinearRegressionCostSeriesCalculator.
    /// </summary>
    public class LinearRegressionCostSeriesCalculatorTests
    {
        private LinearRegressionCostSeriesCalculator testLinearRegressionCostSeriesCalculator;

        [SetUp]
        protected void SetUp()
        {
            testLinearRegressionCostSeriesCalculator = new LinearRegressionCostSeriesCalculator();
            testLinearRegressionCostSeriesCalculator.Logger = new NullApplicationLogger();
        }

        /// <summary>
        /// Tests that an exception is thrown if the ImplementProcess() method is called with 'ThetaParameters' and 'TrainingSeriesData' input data with incompatible dimensions.
        /// </summary>
        [Test]
        public void ImplementProcess_TrainingSeriesDataAndThetaParametersDimensionMismatch()
        {
            Matrix trainingSeriesData = new Matrix(4, 3, new Double[] { 1, 2, 3, 1, 3, 4, 1, 4, 5, 1, 5, 6 });
            Matrix trainingSeriesResults = new Matrix(4, 1, new Double[] { 7, 6, 5, 4 });
            Matrix thetaParameters = new Matrix(3, 1, new Double[] { 0.1, 0.2, 0.3 });
            testLinearRegressionCostSeriesCalculator.GetInputSlot("TrainingSeriesData").DataValue = trainingSeriesData;
            testLinearRegressionCostSeriesCalculator.GetInputSlot("TrainingSeriesResults").DataValue = trainingSeriesResults;
            testLinearRegressionCostSeriesCalculator.GetInputSlot("ThetaParameters").DataValue = thetaParameters;

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testLinearRegressionCostSeriesCalculator.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The 'm' dimension of parameter 'ThetaParameters' must be 1 greater than the 'n' dimension of parameter 'TrainingSeriesData'."));
            Assert.AreEqual("ThetaParameters", e.ParamName);
        }

        /// <summary>
        /// Success tests for the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            // Tests that a column of 1's is added to the left side of parameter 'TrainingSeriesData'

            Matrix trainingSeriesData = new Matrix(4, 2, new Double[] { 2, 3, 3, 4, 4, 5, 5, 6 });
            Matrix trainingSeriesResults = new Matrix(4, 1, new Double[] { 7, 6, 5, 4 });
            Matrix thetaParameters = new Matrix(3, 1, new Double[] { 0.1, 0.2, 0.3 });
            testLinearRegressionCostSeriesCalculator.GetInputSlot("TrainingSeriesData").DataValue = trainingSeriesData;
            testLinearRegressionCostSeriesCalculator.GetInputSlot("TrainingSeriesResults").DataValue = trainingSeriesResults;
            testLinearRegressionCostSeriesCalculator.GetInputSlot("ThetaParameters").DataValue = thetaParameters;

            testLinearRegressionCostSeriesCalculator.Process();

            Double cost = (Double)testLinearRegressionCostSeriesCalculator.GetOutputSlot("Cost").DataValue;

            Assert.That(cost, NUnit.Framework.Is.EqualTo(7.0175).Within(1e-4));
        }
    }
}
