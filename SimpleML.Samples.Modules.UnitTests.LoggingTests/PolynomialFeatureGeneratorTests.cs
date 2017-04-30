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
    /// Tests for the logging functionality in class SimpleML.Samples.Modules.PolynomialFeatureGenerator.
    /// </summary>
    public class PolynomialFeatureGeneratorTests
    {
        private Mockery mockery;
        private IApplicationLogger mockApplicationLogger;
        private PolynomialFeatureGenerator testPolynomialFeatureGenerator;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockApplicationLogger = mockery.NewMock<IApplicationLogger>();
            testPolynomialFeatureGenerator = new PolynomialFeatureGenerator();
            testPolynomialFeatureGenerator.Logger = mockApplicationLogger;
        }

        /// <summary>
        /// Tests the logging functionality when an exception occurs in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess_Exception()
        {
            Matrix dataSeries = new Matrix(2, 3, new Double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0 });
            testPolynomialFeatureGenerator.GetInputSlot("DataSeries").DataValue = dataSeries;
            testPolynomialFeatureGenerator.GetInputSlot("PolynomialDegree").DataValue = 1;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testPolynomialFeatureGenerator, LogLevel.Critical, "Error occurred whilst generating polynomial features for data series.", new TypeMatcher(typeof(ArgumentException)));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testPolynomialFeatureGenerator.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'polynomialDegree' must be greater than 1."));
            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests the logging functionality in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            Matrix dataSeries = new Matrix(2, 3, new Double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0 });
            testPolynomialFeatureGenerator.GetInputSlot("DataSeries").DataValue = dataSeries;
            testPolynomialFeatureGenerator.GetInputSlot("PolynomialDegree").DataValue = 2;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testPolynomialFeatureGenerator, LogLevel.Information, "Generated polynomial features for data series, producing a matrix with 9 columns.");
            }

            testPolynomialFeatureGenerator.Process();

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
