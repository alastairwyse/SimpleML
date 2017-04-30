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
    /// Tests for the logging functionality in class SimpleML.Samples.Modules.LinearRegressionHypothesisCalculator.
    /// </summary>
    public class LinearRegressionHypothesisCalculatorTests
    {
        private Mockery mockery;
        private IApplicationLogger mockApplicationLogger;
        private LinearRegressionHypothesisCalculator testLinearRegressionHypothesisCalculator;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockApplicationLogger = mockery.NewMock<IApplicationLogger>();
            testLinearRegressionHypothesisCalculator = new LinearRegressionHypothesisCalculator();
            testLinearRegressionHypothesisCalculator.Logger = mockApplicationLogger;
        }

        /// <summary>
        /// Tests the logging functionality when the ImplementProcess() method is called with dimension mismatch between parameters 'DataSeries' and 'ThetaParameters'.
        /// </summary>
        [Test]
        public void ImplementProcess_DataSeriesAndThetaParametersDimensionMismatch()
        {
            Matrix dataSeries = new Matrix(2, 2, new Double[] { -1.1, 0.5, 2.3, -0.2 });
            Matrix thetaParameters = new Matrix(2, 1, new Double[] { 4.3, 0.6 });
            testLinearRegressionHypothesisCalculator.GetInputSlot("DataSeries").DataValue = dataSeries;
            testLinearRegressionHypothesisCalculator.GetInputSlot("ThetaParameters").DataValue = thetaParameters;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testLinearRegressionHypothesisCalculator, LogLevel.Critical, "The 'm' dimension of parameter 'ThetaParameters' must be 1 greater than the 'n' dimension of parameter 'DataSeries'.", new TypeMatcher(typeof(ArgumentException)));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testLinearRegressionHypothesisCalculator.Process();
            });

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests the logging functionality when an exception occurs in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess_Exception()
        {
            Matrix dataSeries = new Matrix(2, 3, new Double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0 });
            Matrix thetaParameters = new Matrix(4, 2, new Double[] { 9.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0 });

            testLinearRegressionHypothesisCalculator.GetInputSlot("DataSeries").DataValue = dataSeries;
            testLinearRegressionHypothesisCalculator.GetInputSlot("ThetaParameters").DataValue = thetaParameters;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testLinearRegressionHypothesisCalculator, LogLevel.Critical, "Error occurred whilst calculating linear regression hypothesis.", new TypeMatcher(typeof(ArgumentException)));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testLinearRegressionHypothesisCalculator.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'thetaParameters' must be a single column matrix (i.e. 'n' dimension equal to 1)."));
            Assert.AreEqual("thetaParameters", e.ParamName);
            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests the logging functionality in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            Matrix dataSeries = new Matrix(2, 2, new Double[] { -1.1, 0.5, 2.3, -0.2 });
            Matrix thetaParameters = new Matrix(3, 1, new Double[] { 1.1, 4.3, 0.6 });
            testLinearRegressionHypothesisCalculator.GetInputSlot("DataSeries").DataValue = dataSeries;
            testLinearRegressionHypothesisCalculator.GetInputSlot("ThetaParameters").DataValue = thetaParameters;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testLinearRegressionHypothesisCalculator, LogLevel.Information, "Applied multi-variate linear regression hypothesis to matrix data series of 2 items.");
            }

            testLinearRegressionHypothesisCalculator.Process();

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
