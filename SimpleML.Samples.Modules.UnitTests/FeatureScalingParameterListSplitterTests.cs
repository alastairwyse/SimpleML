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
using ApplicationLogging;
using SimpleML.Samples.Modules;

namespace SimpleML.Samples.Modules.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.Samples.Modules.FeatureScalingParameterListSplitter.
    /// </summary>
    public class FeatureScalingParameterListSplitterTests
    {
        private FeatureScalingParameterListSplitter testFeatureScalingParameterListSplitter;

        [SetUp]
        protected void SetUp()
        {
            testFeatureScalingParameterListSplitter = new FeatureScalingParameterListSplitter();
            testFeatureScalingParameterListSplitter.Logger = new NullApplicationLogger();
        }

        /// <summary>
        /// Tests that an exception is thrown if the ImplementProcess() method is called with 'RightSideItems' parameter less than 1.
        /// </summary>
        [Test]
        public void ImplementProcess_RightSideItemsParameterLessThan1()
        {
            testFeatureScalingParameterListSplitter.GetInputSlot("RightSideItems").DataValue = 0;

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testFeatureScalingParameterListSplitter.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'RightSideItems' must be greater than 0."));
            Assert.AreEqual("RightSideItems", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the ImplementProcess() method is called with 'RightSideItems' parameter which is the same as the size of parameter 'InputScalingParameters'.
        /// </summary>
        [Test]
        public void ImplementProcess_RightSideItemsParameterSameAsListSize()
        {
            List<FeatureScalingParameters> featureScalingParameters = new List<FeatureScalingParameters>()
            {
                new FeatureScalingParameters(0.5, 1.0), 
                new FeatureScalingParameters(0.4, 0.9), 
                new FeatureScalingParameters(0.6, 1.1)
            };
            testFeatureScalingParameterListSplitter.GetInputSlot("InputScalingParameters").DataValue = featureScalingParameters;
            testFeatureScalingParameterListSplitter.GetInputSlot("RightSideItems").DataValue = 3;

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testFeatureScalingParameterListSplitter.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'RightSideItems' must be less than the size of the inputted list."));
            Assert.AreEqual("RightSideItems", e.ParamName);
        }

        /// <summary>
        /// Success tests for the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            List<FeatureScalingParameters> featureScalingParameters = new List<FeatureScalingParameters>()
            {
                new FeatureScalingParameters(0.5, 1.0), 
                new FeatureScalingParameters(0.4, 0.9), 
                new FeatureScalingParameters(0.6, 1.1)
            };
            testFeatureScalingParameterListSplitter.GetInputSlot("InputScalingParameters").DataValue = featureScalingParameters;
            testFeatureScalingParameterListSplitter.GetInputSlot("RightSideItems").DataValue = 1;

            testFeatureScalingParameterListSplitter.Process();

            List<FeatureScalingParameters> startScalingParameters = (List<FeatureScalingParameters>)testFeatureScalingParameterListSplitter.GetOutputSlot("StartScalingParameters").DataValue;
            List<FeatureScalingParameters> endScalingParameters = (List<FeatureScalingParameters>)testFeatureScalingParameterListSplitter.GetOutputSlot("EndScalingParameters").DataValue;
            Assert.AreEqual(2, startScalingParameters.Count);
            Assert.AreEqual(1, endScalingParameters.Count);
            Assert.AreEqual(0.5, startScalingParameters[0].Mean);
            Assert.AreEqual(1.0, startScalingParameters[0].Span);
            Assert.AreEqual(0.4, startScalingParameters[1].Mean);
            Assert.AreEqual(0.9, startScalingParameters[1].Span);
            Assert.AreEqual(0.6, endScalingParameters[0].Mean);
            Assert.AreEqual(1.1, endScalingParameters[0].Span);
        }
    }
}
