﻿/*
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
    /// Tests for the logging functionality in class SimpleML.Samples.Modules.FeatureScaler.
    /// </summary>
    public class FeatureScalerTests
    {
        private Mockery mockery;
        private IApplicationLogger mockApplicationLogger;
        private FeatureScaler testFeatureScaler;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockApplicationLogger = mockery.NewMock<IApplicationLogger>();
            testFeatureScaler = new FeatureScaler();
            testFeatureScaler.Logger = mockApplicationLogger;
        }

        /// <summary>
        /// Tests the logging functionality when an exception occurs in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess_Exception()
        {
            Matrix inputMatrix = new Matrix(2, 3, new Double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0 });
            List<FeatureScalingParameters> featureScalingParameters = new List<FeatureScalingParameters>();
            testFeatureScaler.GetInputSlot("InputMatrix").DataValue = inputMatrix;
            testFeatureScaler.GetInputSlot("FeatureScalingParameters").DataValue = featureScalingParameters;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testFeatureScaler, LogLevel.Critical, "Error occurred whilst attempting to scale matrix.", new TypeMatcher(typeof(ArgumentException)));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testFeatureScaler.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The number of feature scaling parameters provided (0), does not match the 'n' dimension of the inputted matrix of features (3)."));
            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
