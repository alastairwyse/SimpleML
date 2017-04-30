/*
 * Copyright 2017 Alastair Wyse (http://www.oraclepermissiongenerator.net/simpleml/)
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
using SimpleML.Containers;
using SimpleML.Containers.UnitTests;
using NUnit.Framework;
using NMock2;

namespace SimpleML.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.LogisticRegressionErrorRateCalculatorTests.
    /// </summary>
    public class LogisticRegressionErrorRateCalculatorTests
    {
        private Mockery mockery;
        private IHypothesisCalculator mockHypothesisCalculator;
        private LogisticRegressionErrorRateCalculator testLogisticRegressionErrorRateCalculator;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockHypothesisCalculator = mockery.NewMock<IHypothesisCalculator>();
            testLogisticRegressionErrorRateCalculator = new LogisticRegressionErrorRateCalculator(mockHypothesisCalculator);
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
                testLogisticRegressionErrorRateCalculator.Calculate(dataSeries, dataResults, thetaParameters);
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
                testLogisticRegressionErrorRateCalculator.Calculate(dataSeries, dataResults, thetaParameters);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameters 'dataSeries' and 'dataResults' have differing 'm' dimensions."));
            Assert.AreEqual("dataResults", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Calculate() method is called where the 'dataResults' parameter is not a single column matrix.
        /// </summary>
        [Test]
        public void Calculate_DataResultsIsNotSingleColumnMatrix()
        {
            Matrix dataSeries = new Matrix(100, 3);
            Matrix dataResults = new Matrix(100, 2);
            Matrix thetaParameters = new Matrix(3, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testLogisticRegressionErrorRateCalculator.Calculate(dataSeries, dataResults, thetaParameters);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'dataResults' must be a single column matrix (i.e. 'n' dimension equal to 1)."));
            Assert.AreEqual("dataResults", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Calculate() method is called where the 'thetaParameters' parameter is not a single column matrix.
        /// </summary>
        [Test]
        public void Calculate_ThetaParametersIsNotSingleColumnMatrix()
        {
            Matrix dataSeries = new Matrix(100, 3);
            Matrix dataResults = new Matrix(100, 1);
            Matrix thetaParameters = new Matrix(3, 2);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testLogisticRegressionErrorRateCalculator.Calculate(dataSeries, dataResults, thetaParameters);
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
            Matrix dataSeries = new Matrix(5, 3, new Double[] { 1, 0.051267, 0.69956, 1, -0.092742, 0.68494, 1, -0.21371, 0.69225, 1, -0.375, 0.50219, 1, -0.51325, 0.46564 });
            Matrix dataResults = new Matrix(5, 1, new Double[] { 1.0, 0.0, 1.0, 1.0, 0.0 } );
            Matrix thetaParameters = new Matrix(3, 1, new Double[] { 1.0154, -2.1378, 1.0154 });
            Matrix hypothesisResults = new Matrix(5, 1, new Double[] { 0.0, 1.0, 1.0, 1.0, 0.0 });

            using (mockery.Ordered)
            {
                Expect.Once.On(mockHypothesisCalculator).Method("Calculate").With(new MatrixMatcher(dataSeries), new MatrixMatcher(thetaParameters)).Will(Return.Value(hypothesisResults));
            }

            Double errorRate = testLogisticRegressionErrorRateCalculator.Calculate(dataSeries, dataResults, thetaParameters);

            Assert.AreEqual(0.4, errorRate);
            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
