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

namespace SimpleML.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.LogisticRegressionHypothesisCalculator.
    /// </summary>
    public class LogisticRegressionHypothesisCalculatorTests
    {
        private LogisticRegressionHypothesisCalculator testLogisticRegressionHypothesisCalculator;

        [SetUp]
        protected void SetUp()
        {
            testLogisticRegressionHypothesisCalculator = new LogisticRegressionHypothesisCalculator();
        }

        // ** NOTE ** - Tests for incorrect parameters are covered in the unit tests for class HypothesisCalculatorUtilities

        /// <summary>
        /// Success tests for the Calculate() method.
        /// </summary>
        [Test]
        public void Calculate()
        {
            Matrix dataSeries = new Matrix(4, 3, new Double[] { 1, 0.46025, 0.012427, 1, -0.092742, 0.68494, 1, -0.21371, 0.69225, 1, -0.375, 0.50219 });
            Matrix thetaParameters = new Matrix(3, 1, new Double[] { 1.01546, 0.39892, -2.13786 });

            Matrix results = testLogisticRegressionHypothesisCalculator.Calculate(dataSeries, thetaParameters);

            Assert.That(results.GetElement(1, 1), Is.EqualTo(0.763595836839424).Within(1e-15));
            Assert.That(results.GetElement(2, 1), Is.EqualTo(0.380873464072489).Within(1e-15));
            Assert.That(results.GetElement(3, 1), Is.EqualTo(0.365927800857934).Within(1e-15));
            Assert.That(results.GetElement(4, 1), Is.EqualTo(0.448249262795645).Within(1e-15));
        }
    }
}
