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
    /// Tests for the metric logging functionality in class SimpleML.Samples.Modules.FeatureScaler.
    /// </summary>
    public class FeatureScalerTests
    {
        private Mockery mockery;
        private IMetricLogger mockMetricLogger;
        private FeatureScaler testFeatureScaler;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockMetricLogger = mockery.NewMock<IMetricLogger>();
            testFeatureScaler = new FeatureScaler();
            testFeatureScaler.Logger = new NullApplicationLogger();
            testFeatureScaler.MetricLogger = mockMetricLogger;
        }

        /// <summary>
        /// Tests the metric logging functionality in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            Matrix inputMatrix = new Matrix(4, 2, new Double[] { -2.4, 4993.39, 1.0, 4140.96, 3.56, 2375.32, 6.56, 8322.94 });
            List<FeatureScalingParameters> featureScalingParameters = new List<FeatureScalingParameters> 
            {
                new FeatureScalingParameters(2.18, 8.96), 
                new FeatureScalingParameters(4958.1525, 5947.62) 
            };
            testFeatureScaler.GetInputSlot("InputMatrix").DataValue = inputMatrix;
            testFeatureScaler.GetInputSlot("FeatureScalingParameters").DataValue = featureScalingParameters;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockMetricLogger).Method("Increment").With(IsMetric.Equal(new MatrixFeaturesScaled()));
            }

            testFeatureScaler.Process();

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
