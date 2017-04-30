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
    /// Tests for the logging functionality in class SimpleML.Samples.Modules.LinearRegressionCostSeriesCalculator.
    /// </summary>
    public class LinearRegressionCostSeriesCalculatorTests
    {
        private Mockery mockery;
        private IApplicationLogger mockApplicationLogger;
        private LinearRegressionCostSeriesCalculator testLinearRegressionCostSeriesCalculator;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockApplicationLogger = mockery.NewMock<IApplicationLogger>();
            testLinearRegressionCostSeriesCalculator = new LinearRegressionCostSeriesCalculator();
            testLinearRegressionCostSeriesCalculator.Logger = mockApplicationLogger;
        }

        /// <summary>
        /// Tests the logging functionality when the ImplementProcess() method is called with 'ThetaParameters' and 'TrainingSeriesData' input data with incompatible dimensions.
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

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testLinearRegressionCostSeriesCalculator, LogLevel.Critical, "The 'm' dimension of parameter 'ThetaParameters' must be 1 greater than the 'n' dimension of parameter 'TrainingSeriesData'.", new TypeMatcher(typeof(ArgumentException)));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testLinearRegressionCostSeriesCalculator.Process();
            });

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests the logging functionality when an exception occurs in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess_Exception()
        {
            Matrix trainingSeriesData = new Matrix(2, 1, new Double[] { 1.0, 2.0 });
            Matrix trainingSeriesResults = new Matrix(2, 2, new Double[] { 3.0, 4.0, 7.0, 8.0 });
            Matrix thetaParameters = new Matrix(2, 1, new Double[] { 9.0, 10.0 });
            testLinearRegressionCostSeriesCalculator.GetInputSlot("TrainingSeriesData").DataValue = trainingSeriesData;
            testLinearRegressionCostSeriesCalculator.GetInputSlot("TrainingSeriesResults").DataValue = trainingSeriesResults;
            testLinearRegressionCostSeriesCalculator.GetInputSlot("ThetaParameters").DataValue = thetaParameters;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testLinearRegressionCostSeriesCalculator, LogLevel.Critical, "Error occurred whilst calculating linear regression cost.", new TypeMatcher(typeof(ArgumentException)));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testLinearRegressionCostSeriesCalculator.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'dataResults' must be a single column matrix (i.e. 'n' dimension equal to 1)."));
            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
