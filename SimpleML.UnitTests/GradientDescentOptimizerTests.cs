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
using SimpleML;
using SimpleML.Containers;
using SimpleML.Containers.UnitTests;

namespace SimpleML.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.GradientDescentOptimizer.
    /// </summary>
    public class GradientDescentOptimizerTests
    {
        private Mockery mockery;

        IHypothesisCalculator mockHypothesisCalculator;
        private GradientDescentOptimizer testGradientDescentOptimizer;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockHypothesisCalculator = mockery.NewMock<IHypothesisCalculator>();
            testGradientDescentOptimizer = new GradientDescentOptimizer();
        }

        /// <summary>
        /// Tests that an exception is thrown if the Optimize() method is called with 'initialThetaParameters' and 'trainingDataSeries' parameters with incompatible dimensions.
        /// </summary>
        [Test]
        public void Optimize_TrainingDataAndInitialThetaParametersDimensionMismatch()
        {
            Matrix trainingDataSeries = new Matrix(100, 3);
            Matrix trainingDataResults = new Matrix(100, 1);
            Matrix initialThetaParameters = new Matrix(5, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, 1, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The 'm' dimension of parameter 'initialThetaParameters' must be 1 greater than the 'n' dimension of parameter 'trainingDataSeries'."));
            Assert.AreEqual("initialThetaParameters", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Optimize() method is called with 'trainingDataSeries' and 'trainingDataResults' parameters with incompatible dimensions.
        /// </summary>
        [Test]
        public void Optimize_TrainingDataAndTrainingResultsDimensionMismatch()
        {
            Matrix trainingDataSeries = new Matrix(100, 3);
            Matrix trainingDataResults = new Matrix(99, 1);
            Matrix initialThetaParameters = new Matrix(4, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, 1, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameters 'trainingDataSeries' and 'trainingDataResults' have differing 'm' dimensions."));
            Assert.AreEqual("trainingDataResults", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Optimize() method is called with parameter 'trainingDataResults' which is not a single column matrix.
        /// </summary>
        [Test]
        public void Optimize_InitialThetaParametersIsNotSingleColumnMatrix()
        {
            Matrix trainingDataSeries = new Matrix(100, 3);
            Matrix trainingDataResults = new Matrix(100, 2);
            Matrix initialThetaParameters = new Matrix(4, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, 1, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'trainingDataResults' must be a single column matrix (i.e. 'n' dimension equal to 1)."));
            Assert.AreEqual("trainingDataResults", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Optimize() method is called with parameter 'initialThetaParameters' which is not a single column matrix.
        /// </summary>
        [Test]
        public void Optimize_InitialTrainingResultsIsNotSingleColumnMatrix()
        {
            Matrix trainingDataSeries = new Matrix(100, 3);
            Matrix trainingDataResults = new Matrix(100, 1);
            Matrix initialThetaParameters = new Matrix(4, 2);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, 1, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'initialThetaParameters' must be a single column matrix (i.e. 'n' dimension equal to 1)."));
            Assert.AreEqual("initialThetaParameters", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Optimize() method is called with with parameter 'learningRate' equal to 0.
        /// </summary>
        [Test]
        public void Optimize_LearningRate0()
        {
            Matrix trainingDataSeries = new Matrix(100, 3);
            Matrix trainingDataResults = new Matrix(100, 1);
            Matrix initialThetaParameters = new Matrix(4, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, 0, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'learningRate' must be positive."));
            Assert.AreEqual("learningRate", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Optimize() method is called with parameter 'maxIterations' equal to 0.
        /// </summary>
        [Test]
        public void Optimize_MaxIterations0()
        {
            Matrix trainingDataSeries = new Matrix(100, 3);
            Matrix trainingDataResults = new Matrix(100, 1);
            Matrix initialThetaParameters = new Matrix(4, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, 1, 0);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'maxIterations' must be a positive integer."));
            Assert.AreEqual("maxIterations", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown when a matrix with incorrect 'm' dimension is returned by the IHypothesisCalculator when calling the Optimize() method.
        /// </summary>
        [Test]
        public void Optimize_HypothesisCalculatorReturnsIncorrectMDimension()
        {
            Matrix initialThetaParameters = new Matrix(2, 1, new Double[] { 0.5, 0.5 });
            Matrix trainingDataSeries = new Matrix(2, 1, new Double[] { 5, 2 });
            Matrix trainingDataResults = new Matrix(2, 1, new Double[] { 1, 6 });
            Matrix biasedTrainingDataSeries = new Matrix(2, 2, new Double[] { 1, 5, 1, 2 });

            using (mockery.Ordered)
            {
                Expect.Once.On(mockHypothesisCalculator).Method("Calculate").With(new MatrixMatcher(biasedTrainingDataSeries), new MatrixMatcher(new Matrix(2, 1, new Double[] { 0.5, 0.5 }))).Will(Return.Value(new Matrix(3, 1, new Double[] { 3, 1.5, 1 })));
            }

            Exception e = Assert.Throws<Exception>(delegate
            {
                testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, 1, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Hypothesis values returned by method hypothesisCalculator.Calculate() has 'm' dimension length 3.  Expected length 2."));
        }

        /// <summary>
        /// Tests that an exception is thrown when a matrix with incorrect 'n' dimension is returned by the IHypothesisCalculator when calling the Optimize() method.
        /// </summary>
        [Test]
        public void Optimize_HypothesisCalculatorReturnsIncorrectNDimension()
        {
            Matrix initialThetaParameters = new Matrix(2, 1, new Double[] { 0.5, 0.5 });
            Matrix trainingDataSeries = new Matrix(2, 1, new Double[] { 5, 2 });
            Matrix trainingDataResults = new Matrix(2, 1, new Double[] { 1, 6 });
            Matrix biasedTrainingDataSeries = new Matrix(2, 2, new Double[] { 1, 5, 1, 2 });

            using (mockery.Ordered)
            {
                Expect.Once.On(mockHypothesisCalculator).Method("Calculate").With(new MatrixMatcher(biasedTrainingDataSeries), new MatrixMatcher(new Matrix(2, 1, new Double[] { 0.5, 0.5 }))).Will(Return.Value(new Matrix(2, 2, new Double[] { 3, 1.5, 1, 2 })));
            }

            Exception e = Assert.Throws<Exception>(delegate
            {
                testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, 1, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Hypothesis values returned by method hypothesisCalculator.Calculate() has 'n' dimension length 2.  Expected length 1."));

        }

        /// <summary>
        /// Success tests for the Optimize() method.
        /// </summary>
        [Test]
        public void Optimize()
        {
            Matrix initialThetaParameters = new Matrix(2, 1, new Double[] { 0.5, 0.5 });
            Matrix trainingDataSeries = new Matrix(2, 1, new Double[] { 5, 2 });
            Matrix trainingDataResults = new Matrix(2, 1, new Double[] { 1, 6 });
            Matrix biasedTrainingDataSeries = new Matrix(2, 2, new Double[] { 1, 5, 1, 2 });

            using (mockery.Ordered)
            {
                Expect.Once.On(mockHypothesisCalculator).Method("Calculate").With(new MatrixMatcher(biasedTrainingDataSeries), new MatrixMatcher(new Matrix(2, 1, new Double[] { 0.5, 0.5 }))).Will(Return.Value(new Matrix(2, 1, new Double[] { 3, 1.5 })));
                Expect.Once.On(mockHypothesisCalculator).Method("Calculate").With(new MatrixMatcher(biasedTrainingDataSeries), new MatrixMatcher(new Matrix(2, 1, new Double[] { 0.625, 0.45 }))).Will(Return.Value(new Matrix(2, 1, new Double[] { 2.875, 1.525 })));
            }

            Matrix results = testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, 0.1, 2);

            Assert.That(results.GetElement(1, 1), NUnit.Framework.Is.EqualTo(0.755).Within(1e-3));
            Assert.That(results.GetElement(2, 1), NUnit.Framework.Is.EqualTo(0.42875).Within(1e-5));
        }
    }
}
