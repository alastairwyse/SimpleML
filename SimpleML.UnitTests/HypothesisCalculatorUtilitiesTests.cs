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
using SimpleML;
using SimpleML.Containers;

namespace SimpleML.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.HypothesisCalculatorUtilities.
    /// </summary>
    public class HypothesisCalculatorUtilitiesTests
    {
        private HypothesisCalculatorUtilities testHypothesisCalculatorUtilities;

        [SetUp]
        protected void SetUp()
        {
            testHypothesisCalculatorUtilities = new HypothesisCalculatorUtilities();
        }

        /// <summary>
        /// Success tests for the VerifyParameters() method.
        /// </summary>
        [Test]
        public void VerifyParameters()
        {
            Matrix dataSeries = new Matrix(100, 3);
            Matrix thetaParameters = new Matrix(4, 1);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testHypothesisCalculatorUtilities.VerifyParameters(dataSeries, thetaParameters);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The 'm' dimension of parameter 'thetaParameters' must match the 'n' dimension of parameter 'dataSeries'."));
            Assert.AreEqual("thetaParameters", e.ParamName);

            thetaParameters = new Matrix(3, 2);

            e = Assert.Throws<ArgumentException>(delegate
            {
                testHypothesisCalculatorUtilities.VerifyParameters(dataSeries, thetaParameters);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The parameter 'thetaParameters' must be a single column matrix (i.e. 'n' dimension equal to 1)."));
            Assert.AreEqual("thetaParameters", e.ParamName);
        }
    }
}
