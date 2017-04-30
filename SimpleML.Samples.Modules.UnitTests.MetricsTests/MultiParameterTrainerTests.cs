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
using NUnit.Framework;
using NMock2;
using ApplicationMetrics;
using SimpleML.Metrics;
using SimpleML.Containers;
using SimpleML.Samples.Modules;

namespace SimpleML.Samples.Modules.UnitTests.MetricsTests
{
    /// <summary>
    /// Tests for the metric logging functionality in class SimpleML.Samples.Modules.MultiParameterTrainer.
    /// </summary>
    public class MultiParameterTrainerTests
    {
        private Mockery mockery;
        private IMetricLogger mockMetricLogger;
        private MultiParameterTrainer testMultiParameterTrainer;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockMetricLogger = mockery.NewMock<IMetricLogger>();
            testMultiParameterTrainer = new MultiParameterTrainer();
            testMultiParameterTrainer.MetricLogger = mockMetricLogger;
        }

        /// <summary>
        /// Tests the metric logging functionality when an exception occurs in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess_Exception()
        {
            List<Double> regularizationParameterSet = new List<double>();
            regularizationParameterSet.Add(0.1);
            regularizationParameterSet.Add(1.0);
            regularizationParameterSet.Add(10.0);
            List<Int32> maxIterationParameterSet = new List<Int32>();
            maxIterationParameterSet.Add(400);
            maxIterationParameterSet.Add(400);
            maxIterationParameterSet.Add(400);

            testMultiParameterTrainer.GetInputSlot("DataSeries").DataValue = new Matrix(3, 2);
            testMultiParameterTrainer.GetInputSlot("DataResults").DataValue = new Matrix(3, 2);
            testMultiParameterTrainer.GetInputSlot("InitialThetaParameters").DataValue = new Matrix(2, 1);
            testMultiParameterTrainer.GetInputSlot("RegularizationParameterSet").DataValue = regularizationParameterSet;
            testMultiParameterTrainer.GetInputSlot("CostFunctionCalculator").DataValue = new SimpleML.LogisticRegressionCostFunctionCalculator();
            testMultiParameterTrainer.GetInputSlot("MaxIterationParameterSet").DataValue = maxIterationParameterSet;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockMetricLogger).Method("Begin").With(IsMetric.Equal(new MultiParameterTrainingTime()));
                Expect.Once.On(mockMetricLogger).Method("CancelBegin").With(IsMetric.Equal(new MultiParameterTrainingTime()));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMultiParameterTrainer.Process();
            });

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests the metric logging functionality in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            // TODO: Need to implement this test (preferrably verify metric logging only in MultiParameterTrainer, and not underlying FunctionMinimizer object)
        }
    }
}
