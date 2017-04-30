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
using System.Threading;
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
        private IHypothesisCalculator mockHypothesisCalculator;
        private ICostFunctionCalculator mockCostFunctionCalculator;
        private GradientDescentOptimizer testGradientDescentOptimizer;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockHypothesisCalculator = mockery.NewMock<IHypothesisCalculator>();
            mockCostFunctionCalculator = mockery.NewMock<ICostFunctionCalculator>();
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
            Matrix initialThetaParameters = new Matrix(4, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, mockCostFunctionCalculator, 1, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The 'm' dimension of parameter 'initialThetaParameters' must match the 'n' dimension of parameter 'trainingDataSeries'."));
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
            Matrix initialThetaParameters = new Matrix(3, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, mockCostFunctionCalculator, 1, 1);
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
            Matrix initialThetaParameters = new Matrix(3, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, mockCostFunctionCalculator, 1, 1);
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
            Matrix initialThetaParameters = new Matrix(3, 2);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, mockCostFunctionCalculator, 1, 1);
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
            Matrix initialThetaParameters = new Matrix(3, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, mockCostFunctionCalculator, 0, 1);
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
            Matrix initialThetaParameters = new Matrix(3, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, mockCostFunctionCalculator, 1, 0);
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
            Matrix trainingDataSeries = new Matrix(2, 2, new Double[] { 1, 5, 1, 2 });
            Matrix trainingDataResults = new Matrix(2, 1, new Double[] { 1, 6 });

            using (mockery.Ordered)
            {
                Expect.Once.On(mockHypothesisCalculator).Method("Calculate").With(new MatrixMatcher(trainingDataSeries), new MatrixMatcher(new Matrix(2, 1, new Double[] { 0.5, 0.5 }))).Will(Return.Value(new Matrix(3, 1, new Double[] { 3, 1.5, 1 })));
            }

            Exception e = Assert.Throws<Exception>(delegate
            {
                testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, mockCostFunctionCalculator, 1, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Hypothesis values returned by method hypothesisCalculator.Calculate() has 'm' dimension length 3.  Expected length 2."));
            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests that an exception is thrown when a matrix with incorrect 'n' dimension is returned by the IHypothesisCalculator when calling the Optimize() method.
        /// </summary>
        [Test]
        public void Optimize_HypothesisCalculatorReturnsIncorrectNDimension()
        {
            Matrix initialThetaParameters = new Matrix(2, 1, new Double[] { 0.5, 0.5 });
            Matrix trainingDataSeries = new Matrix(2, 2, new Double[] { 1, 5, 1, 2 });
            Matrix trainingDataResults = new Matrix(2, 1, new Double[] { 1, 6 });

            using (mockery.Ordered)
            {
                Expect.Once.On(mockHypothesisCalculator).Method("Calculate").With(new MatrixMatcher(trainingDataSeries), new MatrixMatcher(new Matrix(2, 1, new Double[] { 0.5, 0.5 }))).Will(Return.Value(new Matrix(2, 2, new Double[] { 3, 1.5, 1, 2 })));
            }

            Exception e = Assert.Throws<Exception>(delegate
            {
                testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, mockCostFunctionCalculator, 1, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Hypothesis values returned by method hypothesisCalculator.Calculate() has 'n' dimension length 2.  Expected length 1."));
            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Success tests for the Optimize() method.
        /// </summary>
        [Test]
        public void Optimize()
        {
            // Have to use the below double values inside matricies which are compared using NMock2 Expect statements.
            //   e.g. for the second theta parameter in iteration 3, both the expected and actual double values ToString() to 0.42875, but are not considered equal.
            //   If you calculate the expected values below by the same method as the actual values, they are considered equal, and the Expect statements then work.
            Double iteration3ThetaParameter2 = 0.45 - (0.1 * (1.0 / 2.0) * 0.425000000000001);
            Double iteration4ThetaParameter2 = iteration3ThetaParameter2 - (0.1 * (1.0 / 2.0) * 0.71875);
            Double iteration5ThetaParameter2 = iteration4ThetaParameter2 - (0.1 * (1.0 / 2.0) * 0.547625);

            Matrix initialThetaParameters = new Matrix(2, 1, new Double[] { 0.5, 0.5 });
            Matrix trainingDataSeries = new Matrix(2, 2, new Double[] { 1, 5, 1, 2 });
            Matrix trainingDataResults = new Matrix(2, 1, new Double[] { 1, 6 });

            using (mockery.Ordered)
            {
                Expect.Once.On(mockHypothesisCalculator).Method("Calculate").With(new MatrixMatcher(trainingDataSeries), new MatrixMatcher(new Matrix(2, 1, new Double[] { 0.5, 0.5 }))).Will(Return.Value(new Matrix(2, 1, new Double[] { 3, 1.5 })));
                Expect.Once.On(mockCostFunctionCalculator).Method("Calculate").With(new MatrixMatcher(trainingDataSeries), new MatrixMatcher(trainingDataResults), new MatrixMatcher(new Matrix(2, 1, new Double[] { 0.5, 0.5 }))).Will(Return.Value(6.0625));
                Expect.Once.On(mockHypothesisCalculator).Method("Calculate").With(new MatrixMatcher(trainingDataSeries), new MatrixMatcher(new Matrix(2, 1, new Double[] { 0.625, 0.45 }))).Will(Return.Value(new Matrix(2, 1, new Double[] { 2.875, 1.525 })));
                Expect.Once.On(mockCostFunctionCalculator).Method("Calculate").With(new MatrixMatcher(trainingDataSeries), new MatrixMatcher(trainingDataResults), new MatrixMatcher(new Matrix(2, 1, new Double[] { 0.625, 0.45 }))).Will(Return.Value(5.8853125));
                Expect.Once.On(mockHypothesisCalculator).Method("Calculate").With(new MatrixMatcher(trainingDataSeries), new MatrixMatcher(new Matrix(2, 1, new Double[] { 0.755, iteration3ThetaParameter2 }))).Will(Return.Value(new Matrix(2, 1, new Double[] { 2.89875, 1.6125 })));
                Expect.Once.On(mockCostFunctionCalculator).Method("Calculate").With(new MatrixMatcher(trainingDataSeries), new MatrixMatcher(trainingDataResults), new MatrixMatcher(new Matrix(2, 1, new Double[] { 0.755, iteration3ThetaParameter2 }))).Will(Return.Value(5.713851953125));
                Expect.Once.On(mockHypothesisCalculator).Method("Calculate").With(new MatrixMatcher(trainingDataSeries), new MatrixMatcher(new Matrix(2, 1, new Double[] { 0.8794375, iteration4ThetaParameter2 }))).Will(Return.Value(new Matrix(2, 1, new Double[] { 2.8435, 1.6650625 })));
                Expect.Once.On(mockCostFunctionCalculator).Method("Calculate").With(new MatrixMatcher(trainingDataSeries), new MatrixMatcher(trainingDataResults), new MatrixMatcher(new Matrix(2, 1, new Double[] { 0.8794375, iteration4ThetaParameter2 }))).Will(Return.Value(5.54754384472656));
                Expect.Once.On(mockHypothesisCalculator).Method("Calculate").With(new MatrixMatcher(trainingDataSeries), new MatrixMatcher(new Matrix(2, 1, new Double[] { 1.004009375, iteration5ThetaParameter2 }))).Will(Return.Value(new Matrix(2, 1, new Double[] { 2.831165625, 1.734871875 })));
                Expect.Once.On(mockCostFunctionCalculator).Method("Calculate").With(new MatrixMatcher(trainingDataSeries), new MatrixMatcher(trainingDataResults), new MatrixMatcher(new Matrix(2, 1, new Double[] { 1.004009375, iteration5ThetaParameter2 }))).Will(Return.Value(5.38612136721192));
            }

            Matrix results = testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, mockCostFunctionCalculator, 0.1, 5);

            Assert.That(results.GetElement(1, 1), NUnit.Framework.Is.EqualTo(1.1257075).Within(1e-7));
            Assert.That(results.GetElement(2, 1), NUnit.Framework.Is.EqualTo(0.33415265625).Within(1e-12));
            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests that Optimize() method throws an OperationCanceledException is the CancellationToken is set as cancelled.
        /// </summary>
        [Test]
        public void Optimize_Cancelled()
        {
            Matrix initialThetaParameters = new Matrix(2, 1, new Double[] { 0.5, 0.5 });
            Matrix trainingDataSeries = new Matrix(2, 2, new Double[] { 1, 5, 1, 2 });
            Matrix trainingDataResults = new Matrix(2, 1, new Double[] { 1, 6 });

            using (AutoResetEvent autoResetEvent = new AutoResetEvent(false))
            {
                // This NMock IAction is used to signal cancellation to occur after 100 gradient descent iterations
                //   Is triggered each time the below Expect on the mockCostFunctionCalculator is called, and signals the AutoResetEvent after 100 triggers
                SignalAfterIterationsAction signalAfterIterationsAction = new SignalAfterIterationsAction(autoResetEvent, 100);
                Expect.On(mockHypothesisCalculator).Method("Calculate").WithAnyArguments().Will(Return.Value(new Matrix(2, 1, new Double[] { 3, 1.5 })));
                Expect.On(mockCostFunctionCalculator).Method("Calculate").WithAnyArguments().Will(Return.Value(6.0625), signalAfterIterationsAction);

                using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
                {
                    testGradientDescentOptimizer = new GradientDescentOptimizer(cancellationTokenSource.Token);
                    Thread cancellationThread = new Thread
                        (() =>
                        {
                            autoResetEvent.WaitOne();
                            cancellationTokenSource.Cancel();
                        }
                        );
                    cancellationThread.Start();
                    OperationCanceledException e = Assert.Throws<OperationCanceledException>(delegate
                    {
                        testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, mockCostFunctionCalculator, 0.1, 5000);
                    });
                }
            }

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
