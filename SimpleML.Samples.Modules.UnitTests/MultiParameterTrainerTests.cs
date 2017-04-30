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
using SimpleML;
using SimpleML.Containers;
using SimpleML.Samples.Modules;

namespace SimpleML.Samples.Modules.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.Samples.Modules.MultiParameterTrainer.
    /// </summary>
    public class MultiParameterTrainerTests
    {
        private MultiParameterTrainer testMultiParameterTrainer;

        [SetUp]
        protected void SetUp()
        {
            testMultiParameterTrainer = new MultiParameterTrainer();
            testMultiParameterTrainer.Logger = new NullApplicationLogger();
        }

        /// <summary>
        /// Tests that an exception is thrown if the ImplementProcess() method is called with 'RegularizationParameterSet' and 'MaxIterationParameterSet' parameters of different sizes.
        /// </summary>
        [Test]
        public void ImplementProcess_RegularizationParameterSetAndMaxIterationParameterSetParameterSizeMismatch()
        {
            List<Double> regularizationParameterSet = new List<double>();
            regularizationParameterSet.Add(0.1);
            regularizationParameterSet.Add(1.0);
            regularizationParameterSet.Add(10.0);
            List<Int32> maxIterationParameterSet = new List<Int32>();
            maxIterationParameterSet.Add(400);
            maxIterationParameterSet.Add(400);

            testMultiParameterTrainer.GetInputSlot("DataSeries").DataValue = new Matrix(3, 2);
            testMultiParameterTrainer.GetInputSlot("DataResults").DataValue = new Matrix(3, 1);
            testMultiParameterTrainer.GetInputSlot("InitialThetaParameters").DataValue = new Matrix(2, 1);
            testMultiParameterTrainer.GetInputSlot("RegularizationParameterSet").DataValue = regularizationParameterSet;
            testMultiParameterTrainer.GetInputSlot("CostFunctionCalculator").DataValue = new SimpleML.LogisticRegressionCostFunctionCalculator();
            testMultiParameterTrainer.GetInputSlot("MaxIterationParameterSet").DataValue = maxIterationParameterSet;

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMultiParameterTrainer.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameters 'RegularizationParameterSet' and 'MaxIterationParameterSet' must be lists of equal size."));
            Assert.AreEqual("RegularizationParameterSet", e.ParamName);
        }
    }
}
