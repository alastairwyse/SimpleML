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
using System.IO;
using NUnit.Framework;
using SimpleML;
using SimpleML.Containers;
using SimpleML.Containers.Persistence;

namespace SimpleML.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.FunctionMinimizer.
    /// </summary>
    public class FunctionMinimizerTests
    {
        private FunctionMinimizer testFunctionMinimizer;

        [SetUp]
        protected void SetUp()
        {
            testFunctionMinimizer = new FunctionMinimizer();
        }

        /// <summary>
        /// Tests that an exception is thrown if the Minimize() method is called with 'initialThetaParameters' and 'dataSeries' parameters with incompatible dimensions.
        /// </summary>
        [Test]
        public void Minimize_DataSeriesAndInitialThetaParametersDimensionMismatch()
        {
            Matrix dataSeries = new Matrix(100, 3);
            Matrix dataResults = new Matrix(100, 1);
            Matrix initialThetaParameters = new Matrix(4, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testFunctionMinimizer.Minimize(dataSeries, dataResults, initialThetaParameters, 0, new LogisticRegressionCostFunctionCalculator(), 100);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The 'm' dimension of parameter 'initialThetaParameters' must match the 'n' dimension of parameter 'dataSeries'."));
            Assert.AreEqual("initialThetaParameters", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Minimize() method is called with 'dataSeries' and 'dataResults' parameters with incompatible dimensions.
        /// </summary>
        [Test]
        public void Minimize_TrainingDataAndTrainingResultsDimensionMismatch()
        {
            Matrix dataSeries = new Matrix(100, 3);
            Matrix dataResults = new Matrix(99, 1);
            Matrix initialThetaParameters = new Matrix(3, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testFunctionMinimizer.Minimize(dataSeries, dataResults, initialThetaParameters, 0, new LogisticRegressionCostFunctionCalculator(), 100);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameters 'dataSeries' and 'dataResults' have differing 'm' dimensions."));
            Assert.AreEqual("dataResults", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Minimize() method is called where the 'dataResults' parameter is not a single column matrix.
        /// </summary>
        [Test]
        public void Minimize_DataResultsIsNotSingleColumnMatrix()
        {
            Matrix dataSeries = new Matrix(100, 3);
            Matrix dataResults = new Matrix(100, 2);
            Matrix initialThetaParameters = new Matrix(3, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testFunctionMinimizer.Minimize(dataSeries, dataResults, initialThetaParameters, 0, new LogisticRegressionCostFunctionCalculator(), 100);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'dataResults' must be a single column matrix (i.e. 'n' dimension equal to 1)."));
            Assert.AreEqual("dataResults", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Minimize() method is called where the 'initialThetaParameters' parameter is not a single column matrix.
        /// </summary>
        [Test]
        public void Minimize_InitialThetaParametersIsNotSingleColumnMatrix()
        {
            Matrix dataSeries = new Matrix(100, 3);
            Matrix dataResults = new Matrix(100, 1);
            Matrix initialThetaParameters = new Matrix(3, 2);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testFunctionMinimizer.Minimize(dataSeries, dataResults, initialThetaParameters, 0, new LogisticRegressionCostFunctionCalculator(), 100);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'initialThetaParameters' must be a single column matrix (i.e. 'n' dimension equal to 1)."));
            Assert.AreEqual("initialThetaParameters", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Minimize() method is called where the 'regularizationParameter' parameter is less than 0.
        /// </summary>
        [Test]
        public void Minimize_RegularizationParameterLessThan0()
        {
            Matrix dataSeries = new Matrix(1, 6, new Double[] { 1.0, 0.0512670, 0.699560, 0.0026283, 0.0358643, 0.4893842 });
            Matrix dataResults = new Matrix(1, 1, new Double[] { 1.0 });
            Matrix initialThetaParameters = new Matrix(6, 1, new Double[] { 0.1, -0.1, 0.1, -0.1, 0.1, -0.1 });

            ArgumentOutOfRangeException e = Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                testFunctionMinimizer.Minimize(dataSeries, dataResults, initialThetaParameters, -0.1, new LogisticRegressionCostFunctionCalculator(), 100);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'regularizationParameter' must be greater than or equal to 0."));
            Assert.AreEqual("regularizationParameter", e.ParamName);
            Assert.AreEqual(-0.1, ((Double)e.ActualValue));
        }

        /// <summary>
        /// Tests that an exception is thrown if the Minimize() method is called where the 'maxIterations' parameter is less than 1.
        /// </summary>
        [Test]
        public void Minimize_MaxIterationsParameterLessThan1()
        {
            Matrix dataSeries = new Matrix(1, 6, new Double[] { 1.0, 0.0512670, 0.699560, 0.0026283, 0.0358643, 0.4893842 });
            Matrix dataResults = new Matrix(1, 1, new Double[] { 1.0 });
            Matrix initialThetaParameters = new Matrix(6, 1, new Double[] { 0.1, -0.1, 0.1, -0.1, 0.1, -0.1 });

            ArgumentOutOfRangeException e = Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                testFunctionMinimizer.Minimize(dataSeries, dataResults, initialThetaParameters, 0, new LogisticRegressionCostFunctionCalculator(), 0);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'maxIterations' must be greater than 0."));
            Assert.AreEqual("maxIterations", e.ParamName);
            Assert.AreEqual(0, ((Int32)e.ActualValue));
        }

        /// <summary>
        /// Success tests for the Minimize() method.
        /// </summary>
        [Test]
        public void Minimize()
        {
            // TODO: Remove dependency on external test data file.  Need a test case with fewer inputs and iterations.

            MatrixUtilities matrixUtilities = new MatrixUtilities();
            MatrixCsvReader matrixCsvReader = new MatrixCsvReader();
            String testDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\FunctionMinimizer Test Data.csv");
            Matrix testData = matrixCsvReader.Read(testDataPath, 1, 3);
            Matrix dataSeries = testData.GetColumnSubset(1, 2);
            dataSeries = matrixUtilities.AddColumns(dataSeries, 1, true, 1.0);
            Matrix dataResults = testData.GetColumnSubset(3, 1);
            Matrix initialThetaParameters = new Matrix(3, 1);

            Matrix thetaParameters = testFunctionMinimizer.Minimize(dataSeries, dataResults, initialThetaParameters, 0, new LogisticRegressionCostFunctionCalculator(), 400);

            Assert.That(thetaParameters.GetElement(1, 1), NUnit.Framework.Is.EqualTo(-25.1612394416319).Within(1e-13));
            Assert.That(thetaParameters.GetElement(2, 1), NUnit.Framework.Is.EqualTo(0.206230959830457).Within(1e-15));
            Assert.That(thetaParameters.GetElement(3, 1), NUnit.Framework.Is.EqualTo(0.201470839491568).Within(1e-15));
        }
    }
}
