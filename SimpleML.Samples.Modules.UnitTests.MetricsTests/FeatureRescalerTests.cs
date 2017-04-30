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
    /// Tests for the metric logging functionality in class SimpleML.Samples.Modules.FeatureRescaler.
    /// </summary>
    public class FeatureRescalerTests
    {
        private Mockery mockery;
        private IMetricLogger mockMetricLogger;
        private FeatureRescaler testFeatureRescaler;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockMetricLogger = mockery.NewMock<IMetricLogger>();
            testFeatureRescaler = new FeatureRescaler();
            testFeatureRescaler.Logger = new NullApplicationLogger();
            testFeatureRescaler.MetricLogger = mockMetricLogger;
        }

        /// <summary>
        /// Tests the metric logging functionality in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            Double[] scaledFeatureValuesArray = new Double[]
            { 
                -0.51116071428571428571428571428571, 
                0.0059246387630683, 
                -0.13169642857142857142857142857143, 
                -0.1373982366055666, 
                0.15401785714285714285714285714286, 
                -0.4342632010787508, 
                0.48883928571428571428571428571429, 
                0.5657367989212492
            };
            Matrix scaledFeatureValues = new Matrix(4, 2, scaledFeatureValuesArray);
            List<FeatureScalingParameters> featureScalingParameters = new List<FeatureScalingParameters>();
            featureScalingParameters.Add(new FeatureScalingParameters(2.18, 8.96));
            featureScalingParameters.Add(new FeatureScalingParameters(4958.1525, 5947.62));
            testFeatureRescaler.GetInputSlot("InputMatrix").DataValue = scaledFeatureValues;
            testFeatureRescaler.GetInputSlot("FeatureScalingParameters").DataValue = featureScalingParameters;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockMetricLogger).Method("Increment").With(IsMetric.Equal(new MatrixFeaturesRescaled()));
            }

            testFeatureRescaler.Process();

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
