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
using SimpleML.Containers;
using SimpleML.Samples.Modules;

namespace SimpleML.Samples.Modules.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.Samples.Modules.MultiParameterErrorRateCalculator.
    /// </summary>
    public class MultiParameterErrorRateCalculatorTests
    {
        private MultiParameterErrorRateCalculator testMultiParameterErrorRateCalculator;

        [SetUp]
        protected void SetUp()
        {
            testMultiParameterErrorRateCalculator = new MultiParameterErrorRateCalculator();
            testMultiParameterErrorRateCalculator.Logger = new NullApplicationLogger();
        }

        /// <summary>
        /// Success tests for the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            testMultiParameterErrorRateCalculator.GetInputSlot("DataSeries").DataValue = new Matrix(5, 3, new Double[] { 1, 0.051267, 0.69956, 1, -0.092742, 0.68494, 1, -0.21371, 0.69225, 1, -0.375, 0.50219, 1, -0.51325, 0.46564 });
            testMultiParameterErrorRateCalculator.GetInputSlot("DataResults").DataValue = new Matrix(5, 1, new Double[] { 1.0, 0.0, 1.0, 1.0, 0.0 });
            testMultiParameterErrorRateCalculator.GetInputSlot("ThetaParameterSet").DataValue = new List<Matrix>() { new Matrix(3, 1, new Double[] { 1.0154, -2.1378, 1.0154 }), new Matrix(3, 1, new Double[] { 1.0154, -2.1378, 1.0154 }) };

            testMultiParameterErrorRateCalculator.Process();

            List<Double> errorRateSet = (List<Double>)testMultiParameterErrorRateCalculator.GetOutputSlot("ErrorRateSet").DataValue;

            Assert.AreEqual(2, errorRateSet.Count);
        }
    }
}
