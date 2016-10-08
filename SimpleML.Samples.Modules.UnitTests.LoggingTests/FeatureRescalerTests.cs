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
using NMock2.Matchers;
using ApplicationLogging;
using SimpleML;
using SimpleML.Containers;
using SimpleML.Samples.Modules;

namespace SimpleML.Samples.Modules.UnitTests.LoggingTests
{
    /// <summary>
    /// Tests for the logging functionality in class SimpleML.Samples.Modules.FeatureRescaler.
    /// </summary>
    public class FeatureRescalerTests
    {
        private Mockery mockery;
        private IApplicationLogger mockApplicationLogger;
        private FeatureRescaler testFeatureRescaler;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockApplicationLogger = mockery.NewMock<IApplicationLogger>();
            testFeatureRescaler = new FeatureRescaler();
            testFeatureRescaler.Logger = mockApplicationLogger;
        }

        /// <summary>
        /// Tests the logging functionality when an exception occurs in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess_Exception()
        {
            Matrix inputMatrix = new Matrix(2, 3, new Double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0 });
            List<FeatureScalingParameters> featureScalingParameters = new List<FeatureScalingParameters>();
            testFeatureRescaler.GetInputSlot("InputMatrix").DataValue = inputMatrix;
            testFeatureRescaler.GetInputSlot("FeatureScalingParameters").DataValue = featureScalingParameters;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testFeatureRescaler, LogLevel.Critical, "Error occurred whilst attempting to rescale matrix.", new TypeMatcher(typeof(ArgumentException)));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testFeatureRescaler.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Size of parameter 'featureScalingParameters' '0' does not match the size of the 'n' dimension of the matrix '3'."));
            Assert.AreEqual("featureScalingParameters", e.ParamName);
        }
    }
}
