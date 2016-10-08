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
    /// Unit tests for class SimpleML.MultivariateLinearRegressionCostSeriesCalculator.
    /// </summary>
    public class MultivariateLinearRegressionCostSeriesCalculatorTests
    {
        private MultivariateLinearRegressionCostSeriesCalculator testMultivariateLinearRegressionCostSeriesCalculator;

        [SetUp]
        protected void SetUp()
        {
            testMultivariateLinearRegressionCostSeriesCalculator = new MultivariateLinearRegressionCostSeriesCalculator();
        }

        /// <summary>
        /// Tests that an exception is thrown if the CalculateCost() method is called with 'thetaParameters' and 'trainingDataSeries' parameters with incompatible dimensions.
        /// </summary>
        [Test]
        public void CalculateCost_TrainingDataAndInitialThetaParametersDimensionMismatch()
        {
            Matrix trainingDataSeries = new Matrix(100, 3);
            Matrix trainingDataResults = new Matrix(100, 1);
            Matrix thetaParameters = new Matrix(5, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMultivariateLinearRegressionCostSeriesCalculator.CalculateCost(trainingDataSeries, trainingDataResults, thetaParameters);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The 'm' dimension of parameter 'thetaParameters' must be 1 greater than the 'n' dimension of parameter 'trainingDataSeries'."));
            Assert.AreEqual("thetaParameters", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the CalculateCost() method is called with 'trainingDataSeries' and 'trainingDataResults' parameters with incompatible dimensions.
        /// </summary>
        [Test]
        public void CalculateCost_TrainingDataAndTrainingResultsDimensionMismatch()
        {
            Matrix trainingDataSeries = new Matrix(100, 3);
            Matrix trainingDataResults = new Matrix(99, 1);
            Matrix thetaParameters = new Matrix(4, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMultivariateLinearRegressionCostSeriesCalculator.CalculateCost(trainingDataSeries, trainingDataResults, thetaParameters);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameters 'trainingDataSeries' and 'trainingDataResults' have differing 'm' dimensions."));
            Assert.AreEqual("trainingDataResults", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the CalculateCost() method is called where the 'trainingDataResults' parameter is not a single column matrix.
        /// </summary>
        [Test]
        public void CalculateCost_InitialThetaParametersIsNotSingleColumnMatrix()
        {
            Matrix trainingDataSeries = new Matrix(100, 3);
            Matrix trainingDataResults = new Matrix(100, 2);
            Matrix thetaParameters = new Matrix(4, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMultivariateLinearRegressionCostSeriesCalculator.CalculateCost(trainingDataSeries, trainingDataResults, thetaParameters);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'trainingDataResults' must be a single column matrix (i.e. 'n' dimension equal to 1)."));
            Assert.AreEqual("trainingDataResults", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the CalculateCost() method is called where the 'thetaParameters' parameter is not a single column matrix.
        /// </summary>
        [Test]
        public void CalculateCost_InitialTrainingResultsIsNotSingleColumnMatrix()
        {
            Matrix trainingDataSeries = new Matrix(100, 3);
            Matrix trainingDataResults = new Matrix(100, 1);
            Matrix thetaParameters = new Matrix(4, 2);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMultivariateLinearRegressionCostSeriesCalculator.CalculateCost(trainingDataSeries, trainingDataResults, thetaParameters);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'thetaParameters' must be a single column matrix (i.e. 'n' dimension equal to 1)."));
            Assert.AreEqual("thetaParameters", e.ParamName);
        }

        /// <summary>
        /// Success tests for the CalculateCost() method.
        /// </summary>
        [Test]
        public void CalculateCost()
        {
            Matrix trainingDataSeries = new Matrix(3, 2, new Double[] { 1.1, 20.6, -3.9, 4.0, 0.6, -0.7 });
            Matrix trainingDataResults = new Matrix(3, 1, new Double[] { 65.0, 24.5, -2.81 });
            Matrix thetaParameters = new Matrix(3, 1, new Double[] { 1.1, -2.2, 3.3 });

            Double cost = testMultivariateLinearRegressionCostSeriesCalculator.CalculateCost(trainingDataSeries, trainingDataResults, thetaParameters);

            Assert.That(cost, NUnit.Framework.Is.EqualTo(3.56).Within(1e-2));
        }
    }
}
