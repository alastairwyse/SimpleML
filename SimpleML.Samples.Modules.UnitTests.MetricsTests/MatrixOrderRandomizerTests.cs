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
using ApplicationLogging;
using ApplicationMetrics;
using SimpleML.Containers;
using SimpleML.Samples.Modules;

namespace SimpleML.Samples.Modules.UnitTests.MetricsTests
{
    /// <summary>
    /// Tests for the metric logging functionality in class SimpleML.Samples.Modules.MatrixOrderRandomizer.
    /// </summary>
    public class MatrixOrderRandomizerTests
    {
        private Mockery mockery;
        private IMetricLogger mockMetricLogger;
        private MatrixOrderRandomizer testMatrixOrderRandomizer;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockMetricLogger = mockery.NewMock<IMetricLogger>();
            testMatrixOrderRandomizer = new MatrixOrderRandomizer();
            testMatrixOrderRandomizer.Logger = new NullApplicationLogger();
            testMatrixOrderRandomizer.MetricLogger = mockMetricLogger;
        }

        /// <summary>
        /// Tests the metric logging functionality in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            Matrix inputMatrix = new Matrix(3, 4, new Double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
            testMatrixOrderRandomizer.GetInputSlot("InputMatrix").DataValue = inputMatrix;
            testMatrixOrderRandomizer.GetInputSlot("RandomSeed").DataValue = 13;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockMetricLogger).Method("Increment").With(IsMetric.Equal(new MatrixRowOrderRandomized()));
            }

            testMatrixOrderRandomizer.Process();

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
