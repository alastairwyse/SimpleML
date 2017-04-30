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
using ApplicationMetrics;
using SimpleML;
using SimpleML.UnitTests;
using SimpleML.Containers;
using SimpleML.Metrics;

namespace SimpleML.UnitTests.MetricsTests
{
    /// <summary>
    /// Tests for the metric logging functionality in class SimpleML.GradientDescentOptimizer.
    /// </summary>
    public class GradientDescentOptimizerTests
    {
        private Mockery mockery;
        private IMetricLogger mockMetricLogger;
        private IHypothesisCalculator mockHypothesisCalculator;
        private ICostFunctionCalculator costFunctionCalculator;
        private CancellationToken cancellationToken;
        private GradientDescentOptimizer testGradientDescentOptimizer;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockMetricLogger = mockery.NewMock<IMetricLogger>();
            mockHypothesisCalculator = mockery.NewMock<IHypothesisCalculator>();
            costFunctionCalculator = new MultivariateLinearRegressionCostFunctionCalculator();
            cancellationToken = new CancellationToken();
            testGradientDescentOptimizer = new GradientDescentOptimizer(mockMetricLogger, cancellationToken);
        }

        /// <summary>
        /// Tests the metric logging functionality when an exception occurs in the Optimize() method.
        /// </summary>
        [Test]
        public void Optimize_Exception()
        {
            Matrix initialThetaParameters = new Matrix(2, 1, new Double[] { 0.5, 0.5 });
            Matrix trainingDataSeries = new Matrix(2, 2, new Double[] { 1, 5, 1, 2 });
            Matrix trainingDataResults = new Matrix(2, 1, new Double[] { 1, 6 });

            using (mockery.Ordered)
            {
                Expect.Once.On(mockMetricLogger).Method("Begin").With(IsMetric.Equal(new GradientDescentOptimizationTime()));
                Expect.Once.On(mockHypothesisCalculator).Method("Calculate").WithAnyArguments().Will(Return.Value(new Matrix(3, 1, new Double[] { 3, 1.5, 1 })));
                Expect.Once.On(mockMetricLogger).Method("CancelBegin").With(IsMetric.Equal(new GradientDescentOptimizationTime()));
            }

            Exception e = Assert.Throws<Exception>(delegate
            {
                testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, costFunctionCalculator, 0.1, 5);
            });

            mockery.VerifyAllExpectationsHaveBeenMet();
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
                Expect.Once.On(mockMetricLogger).Method("Begin").With(IsMetric.Equal(new GradientDescentOptimizationTime()));
                Expect.Once.On(mockMetricLogger).Method("End").With(IsMetric.Equal(new GradientDescentOptimizationTime()));
                Expect.Once.On(mockMetricLogger).Method("Increment").With(IsMetric.Equal(new GradientDescentOptimizationCompleted()));
                Expect.Once.On(mockMetricLogger).Method("Add").With(new AmountMetricMatcher(new GradientDescentIterations(5)));
            }

            testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, new MultivariateLinearRegressionHypothesisCalculator(), costFunctionCalculator, 0.1, 5);

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests the metric logging functionality when the optimization process is cancelled.
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
                testGradientDescentOptimizer = new GradientDescentOptimizer(mockMetricLogger, cancellationTokenSource.Token);

                // This NMock IAction is used to signal cancellation to occur after 100 gradient descent iterations
                //   Is triggered each time the below Expect on the mockCostFunctionCalculator is called, and signals the AutoResetEvent after 100 triggers
                SignalAfterIterationsAction signalAfterIterationsAction = new SignalAfterIterationsAction(autoResetEvent, 100);

                Expect.Once.On(mockMetricLogger).Method("Begin").With(IsMetric.Equal(new GradientDescentOptimizationTime()));
                Expect.On(mockHypothesisCalculator).Method("Calculate").WithAnyArguments().Will(Return.Value(new Matrix(2, 1, new Double[] { 3, 1.5 })), signalAfterIterationsAction);
                Expect.Once.On(mockMetricLogger).Method("CancelBegin").With(IsMetric.Equal(new GradientDescentOptimizationTime()));

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
                    testGradientDescentOptimizer.Optimize(initialThetaParameters, trainingDataSeries, trainingDataResults, mockHypothesisCalculator, costFunctionCalculator, 0.1, 5000);
                });
            }

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
