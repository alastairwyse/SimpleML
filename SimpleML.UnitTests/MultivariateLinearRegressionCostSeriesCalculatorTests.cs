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
using SimpleML;
using SimpleML.Containers;

namespace SimpleML.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.MultivariateLinearRegressionCostFunctionCalculator.
    /// </summary>
    public class MultivariateLinearRegressionCostFunctionCalculatorTests
    {
        private MultivariateLinearRegressionCostFunctionCalculator testMultivariateLinearRegressionCostFunctionCalculator;

        [SetUp]
        protected void SetUp()
        {
            testMultivariateLinearRegressionCostFunctionCalculator = new MultivariateLinearRegressionCostFunctionCalculator();
        }

        /// <summary>
        /// Tests that an exception is thrown if the Calculate() method is called with 'thetaParameters' and 'dataSeries' parameters with incompatible dimensions.
        /// </summary>
        [Test]
        public void Calculate_TrainingDataAndInitialThetaParametersDimensionMismatch()
        {
            Matrix dataSeries = new Matrix(100, 3);
            Matrix dataResults = new Matrix(100, 1);
            Matrix thetaParameters = new Matrix(4, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMultivariateLinearRegressionCostFunctionCalculator.Calculate(dataSeries, dataResults, thetaParameters);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The 'm' dimension of parameter 'thetaParameters' must match the 'n' dimension of parameter 'dataSeries'."));
            Assert.AreEqual("thetaParameters", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Calculate() method is called with 'dataSeries' and 'dataResults' parameters with incompatible dimensions.
        /// </summary>
        [Test]
        public void Calculate_TrainingDataAndTrainingResultsDimensionMismatch()
        {
            Matrix dataSeries = new Matrix(100, 3);
            Matrix dataResults = new Matrix(99, 1);
            Matrix thetaParameters = new Matrix(3, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMultivariateLinearRegressionCostFunctionCalculator.Calculate(dataSeries, dataResults, thetaParameters);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameters 'dataSeries' and 'dataResults' have differing 'm' dimensions."));
            Assert.AreEqual("dataResults", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Calculate() method is called where the 'dataResults' parameter is not a single column matrix.
        /// </summary>
        [Test]
        public void Calculate_InitialThetaParametersIsNotSingleColumnMatrix()
        {
            Matrix dataSeries = new Matrix(100, 3);
            Matrix dataResults = new Matrix(100, 2);
            Matrix thetaParameters = new Matrix(3, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMultivariateLinearRegressionCostFunctionCalculator.Calculate(dataSeries, dataResults, thetaParameters);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'dataResults' must be a single column matrix (i.e. 'n' dimension equal to 1)."));
            Assert.AreEqual("dataResults", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Calculate() method is called where the 'thetaParameters' parameter is not a single column matrix.
        /// </summary>
        [Test]
        public void Calculate_InitialTrainingResultsIsNotSingleColumnMatrix()
        {
            Matrix dataSeries = new Matrix(100, 3);
            Matrix dataResults = new Matrix(100, 1);
            Matrix thetaParameters = new Matrix(3, 2);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMultivariateLinearRegressionCostFunctionCalculator.Calculate(dataSeries, dataResults, thetaParameters);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'thetaParameters' must be a single column matrix (i.e. 'n' dimension equal to 1)."));
            Assert.AreEqual("thetaParameters", e.ParamName);
        }

        /// <summary>
        /// Success tests for the Calculate() method.
        /// </summary>
        [Test]
        public void Calculate()
        {
            Matrix dataSeries = new Matrix(4, 3, new Double[] { 1, 2, 3, 1, 3, 4, 1, 4, 5, 1, 5, 6 });
            Matrix dataResults = new Matrix(4, 1, new Double[] { 7, 6, 5, 4 });
            Matrix thetaParameters = new Matrix(3, 1, new Double[] { 0.1, 0.2, 0.3 });

            Double cost = testMultivariateLinearRegressionCostFunctionCalculator.Calculate(dataSeries, dataResults, thetaParameters);

            Assert.That(cost, NUnit.Framework.Is.EqualTo(7.0175).Within(1e-4));
        }
    }
}
