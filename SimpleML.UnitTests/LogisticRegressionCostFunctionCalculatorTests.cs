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
    /// Unit tests for class SimpleML.LogisticRegressionCostFunctionCalculator.
    /// </summary>
    public class LogisticRegressionCostFunctionCalculatorTests
    {
        private LogisticRegressionCostFunctionCalculator testLogisticRegressionCostFunctionCalculator;

        [SetUp]
        protected void SetUp()
        {
            testLogisticRegressionCostFunctionCalculator = new LogisticRegressionCostFunctionCalculator();
        }

        /// <summary>
        /// Tests that an exception is thrown if the Calculate() method is called with 'thetaParameters' and 'dataSeries' parameters with incompatible dimensions.
        /// </summary>
        [Test]
        public void Calculate_DataSeriesAndThetaParametersDimensionMismatch()
        {
            Matrix dataSeries = new Matrix(100, 3);
            Matrix dataResults = new Matrix(100, 1);
            Matrix thetaParameters = new Matrix(4, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testLogisticRegressionCostFunctionCalculator.Calculate(dataSeries, dataResults, thetaParameters, 0);
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
                testLogisticRegressionCostFunctionCalculator.Calculate(dataSeries, dataResults, thetaParameters, 0);
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
                testLogisticRegressionCostFunctionCalculator.Calculate(dataSeries, dataResults, thetaParameters, 0);
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
                testLogisticRegressionCostFunctionCalculator.Calculate(dataSeries, dataResults, thetaParameters, 0);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'thetaParameters' must be a single column matrix (i.e. 'n' dimension equal to 1)."));
            Assert.AreEqual("thetaParameters", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Calculate() method is called where the 'regularizationParameter' parameter is less than 0.
        /// </summary>
        [Test]
        public void Calculate_RegularizationParameterLessThan0()
        {
            Matrix dataSeries = new Matrix(1, 6, new Double[] { 1.0, 0.0512670, 0.699560, 0.0026283, 0.0358643, 0.4893842 });
            Matrix dataResults = new Matrix(1, 1, new Double[] { 1.0 });
            Matrix thetaParameters = new Matrix(6, 1, new Double[] { 0.1, -0.1, 0.1, -0.1, 0.1, -0.1 });

            ArgumentOutOfRangeException e = Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                testLogisticRegressionCostFunctionCalculator.Calculate(dataSeries, dataResults, thetaParameters, -0.1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'regularizationParameter' must be greater than or equal to 0."));
            Assert.AreEqual("regularizationParameter", e.ParamName);
            Assert.AreEqual(-0.1, ((Double)e.ActualValue));
        }

        /// <summary>
        /// Success tests for the Calculate() method.
        /// </summary>
        [Test]
        public void Calculate()
        {
            // Test without regularization
            Matrix dataSeries = new Matrix(3, 4, new Double[] { 1, 8, 1, 6, 1, 3, 5, 7, 1, 4, 9, 2 });
            Matrix dataResults = new Matrix(3, 1, new Double[] { 1, 0, 1 });
            Matrix thetaParameters = new Matrix(4, 1, new Double[] { -2, -1, 1, 2 });

            Double cost = testLogisticRegressionCostFunctionCalculator.Calculate(dataSeries, dataResults, thetaParameters);

            Assert.That(cost, NUnit.Framework.Is.EqualTo(4.68316654981069).Within(1e-15));

            // Test with regularization
            dataSeries = new Matrix(1, 6, new Double[] { 1.0, 0.0512670, 0.699560, 0.0026283, 0.0358643, 0.4893842 });
            dataResults = new Matrix(1, 1, new Double[] { 1.0 });
            thetaParameters = new Matrix(6, 1, new Double[] { 0.1, -0.1, 0.1, -0.1, 0.1, -0.1 });

            Tuple<Double, Matrix> result = testLogisticRegressionCostFunctionCalculator.Calculate(dataSeries, dataResults, thetaParameters, 0.1);
            
            Assert.That(result.Item1, NUnit.Framework.Is.EqualTo(0.637815401088081).Within(1e-15));
            Assert.That(result.Item2.GetElement(1, 1), NUnit.Framework.Is.EqualTo(-0.47023162755641).Within(1e-14));
            Assert.That(result.Item2.GetElement(2, 1), NUnit.Framework.Is.EqualTo(-0.0341073648499345).Within(1e-16));
            Assert.That(result.Item2.GetElement(3, 1), NUnit.Framework.Is.EqualTo(-0.318955237373362).Within(1e-15));
            Assert.That(result.Item2.GetElement(4, 1), NUnit.Framework.Is.EqualTo(-0.0112359097867065).Within(1e-16));
            Assert.That(result.Item2.GetElement(5, 1), NUnit.Framework.Is.EqualTo(-0.00686452816017135).Within(1e-17));
            Assert.That(result.Item2.GetElement(6, 1), NUnit.Framework.Is.EqualTo(-0.240123928866392).Within(1e-15));
        }
    }
}
