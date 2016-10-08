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
using SimpleML.Containers;
using SimpleML.Samples.Modules;

namespace SimpleML.Samples.Modules.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.Samples.Modules.LinearRegressionHypothesisCalculator.
    /// </summary>
    public class LinearRegressionHypothesisCalculatorTests
    {
        private LinearRegressionHypothesisCalculator testLinearRegressionHypothesisCalculator;

        [SetUp]
        protected void SetUp()
        {
            testLinearRegressionHypothesisCalculator = new LinearRegressionHypothesisCalculator();
            testLinearRegressionHypothesisCalculator.Logger = new NullApplicationLogger();
        }

        /// <summary>
        /// Tests that an exception is thrown if the ImplementProcess() method is called with dimension mismatch between parameters 'DataSeries' and 'ThetaParameters'.
        /// </summary>
        [Test]
        public void ImplementProcess_DataSeriesAndThetaParametersDimensionMismatch()
        {
            Matrix dataSeries = new Matrix(2, 2, new Double[] { -1.1, 0.5, 2.3, -0.2 });
            Matrix thetaParameters = new Matrix(2, 1, new Double[] { 4.3, 0.6 });
            testLinearRegressionHypothesisCalculator.GetInputSlot("DataSeries").DataValue = dataSeries;
            testLinearRegressionHypothesisCalculator.GetInputSlot("ThetaParameters").DataValue = thetaParameters;

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testLinearRegressionHypothesisCalculator.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The 'm' dimension of parameter 'ThetaParameters' must be 1 greater than the 'n' dimension of parameter 'DataSeries'."));
            Assert.AreEqual("ThetaParameters", e.ParamName);
        }

        /// <summary>
        /// Success tests for the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            Matrix dataSeries = new Matrix(2, 2, new Double[] { -1.1, 0.5, 2.3, -0.2 });
            Matrix thetaParameters = new Matrix(3, 1, new Double[] { 1.1, 4.3, 0.6 });
            testLinearRegressionHypothesisCalculator.GetInputSlot("DataSeries").DataValue = dataSeries;
            testLinearRegressionHypothesisCalculator.GetInputSlot("ThetaParameters").DataValue = thetaParameters;

            testLinearRegressionHypothesisCalculator.Process();

            Matrix results = (Matrix)testLinearRegressionHypothesisCalculator.GetOutputSlot("Results").DataValue;

            Assert.AreEqual(2, results.MDimension);
            Assert.AreEqual(1, results.NDimension);
            Assert.That(results.GetElement(1, 1), NUnit.Framework.Is.EqualTo(-3.33).Within(1e-2));
            Assert.That(results.GetElement(2, 1), NUnit.Framework.Is.EqualTo(10.87).Within(1e-2));
        }
    }
}
