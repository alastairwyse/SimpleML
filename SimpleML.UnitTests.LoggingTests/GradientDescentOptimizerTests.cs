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
using ApplicationLogging;
using SimpleML;
using SimpleML.UnitTests;
using SimpleML.Containers;

namespace SimpleML.UnitTests.LoggingTests
{
    /// <summary>
    /// Tests for the logging functionality in class SimpleML.GradientDescentOptimizer.
    /// </summary>
    public class GradientDescentOptimizerTests
    {
        private Mockery mockery;
        private ICostFunctionCalculator mockCostFunctionCalculator;
        private IApplicationLogger mockApplicationLogger;
        private IHypothesisCalculator mockHypothesisCalculator;
        private IHypothesisCalculator hypothesisCalculator;
        private CancellationToken cancellationToken;
        private GradientDescentOptimizer testGradientDescentOptimizer;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockHypothesisCalculator = mockery.NewMock<IHypothesisCalculator>();
            mockCostFunctionCalculator = mockery.NewMock<ICostFunctionCalculator>();
            mockApplicationLogger = mockery.NewMock<IApplicationLogger>();
            hypothesisCalculator = new MultivariateLinearRegressionHypothesisCalculator();
            cancellationToken = new CancellationToken();
            testGradientDescentOptimizer = new GradientDescentOptimizer(mockApplicationLogger, cancellationToken);
        }

        /// <summary>
        /// Success tests for the Optimize() method.
        /// </summary>
        [Test]
        public void Optimize()
        {
            Matrix initialThetaParameters = new Matrix(2, 1, new Double[] { 0.5, 0.5 });
            Matrix trainingDataSeries = new Matrix(2, 2, new Double[] { 1, 5, 1, 2 });
            Matrix trainingDataResults = new Matrix(2, 1, new Double[] { 1, 6 });

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testGradientDescentOptimizer, LogLevel.Information, "Running gradient descent optimization for 2 theta parameters, 2 training data points, 5 iterations.");
                Expect.Once.On(mockCostFunctionCalculator).Method("Calculate").WithAnyArguments().Will(Return.Value(6.0625));
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testGradientDescentOptimizer, LogLevel.Debug, "Gradient descent iteration 1, cost = 6.0625.");
                Expect.Once.On(mockCostFunctionCalculator).Method("Calculate").WithAnyArguments().Will(Return.Value(5.8853125));
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testGradientDescentOptimizer, LogLevel.Debug, "Gradient descent iteration 2, cost = 5.8853125.");
                Expect.Once.On(mockCostFunctionCalculator).Method("Calculate").WithAnyArguments().Will(Return.Value(5.713851953125));
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testGradientDescentOptimizer, LogLevel.Debug, "Gradient descent iteration 3, cost = 5.713851953125.");
                Expect.Once.On(mockCostFunctionCalculator).Method("Calculate").WithAnyArguments().Will(Return.Value(5.54754384472656));
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testGradientDescentOptimizer, LogLevel.Debug, "Gradient descent iteration 4, cost = 5.54754384472656.");
                Expect.Once.On(mockCostFunctionCalculator).Method("Calculate").WithAnyArguments().Will(Return.Value(5.38612136721192));
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testGradientDescentOptimizer, LogLevel.Debug, "Gradient descent iteration 5, cost = 5.38612136721192.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testGradientDescentOptimizer, LogLevel.Information, "Completed running gradient descent optimization.");
            }

            testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, hypothesisCalculator, mockCostFunctionCalculator, 0.1, 5);

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests the logging functionality when the optimization process is cancelled.
        /// </summary>
        [Test]
        public void Optimize_Cancelled()
        {
            // TODO: Improve the test so that it actually checks the number of iterations stated in the Log() call

            Matrix initialThetaParameters = new Matrix(2, 1, new Double[] { 0.5, 0.5 });
            Matrix trainingDataSeries = new Matrix(2, 2, new Double[] { 1, 5, 1, 2 });
            Matrix trainingDataResults = new Matrix(2, 1, new Double[] { 1, 6 });

            using (AutoResetEvent autoResetEvent = new AutoResetEvent(false))
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                testGradientDescentOptimizer = new GradientDescentOptimizer(mockApplicationLogger, cancellationTokenSource.Token);

                // This NMock IAction is used to signal cancellation to occur after 100 gradient descent iterations
                //   Is triggered each time the below Expect on the mockCostFunctionCalculator is called, and signals the AutoResetEvent after 100 triggers
                //   However, the cancellation will not occur until at least 1000 iterations have run through (this is the value of GradientDescentOptimizer const 'iterationsBetweenCancellationChecks' as at 2017-04-16)
                //   Hence the numeric value in the cancellation log statement (i.e. "cancelled after x iterations") is not checked, as it will depend on thread scheduling
                SignalAfterIterationsAction signalAfterIterationsAction = new SignalAfterIterationsAction(autoResetEvent, 100);
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testGradientDescentOptimizer, LogLevel.Information, "Running gradient descent optimization for 2 theta parameters, 2 training data points, 5000 iterations.");
                Expect.On(mockHypothesisCalculator).Method("Calculate").WithAnyArguments().Will(Return.Value(new Matrix(2, 1, new Double[] { 3, 1.5 })));
                Expect.On(mockCostFunctionCalculator).Method("Calculate").WithAnyArguments().Will(Return.Value(6.0625), signalAfterIterationsAction);
                Expect.On(mockApplicationLogger).Method("Log").With(testGradientDescentOptimizer, LogLevel.Debug, new NMock2.Matchers.TypeMatcher(typeof(String)));
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testGradientDescentOptimizer, LogLevel.Information, new NMock2.Matchers.StringContainsMatcher("Gradient descent optimization cancelled after "));

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

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
