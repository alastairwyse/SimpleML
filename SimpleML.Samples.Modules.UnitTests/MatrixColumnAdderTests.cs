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
using SimpleML.Containers;
using SimpleML.Samples.Modules;

namespace SimpleML.Samples.Modules.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.Samples.Modules.MatrixColumnAdder.
    /// </summary>
    public class MatrixColumnAdderTests
    {
        private MatrixColumnAdder testMatrixColumnAdder;

        [SetUp]
        protected void SetUp()
        {
            testMatrixColumnAdder = new MatrixColumnAdder();
            testMatrixColumnAdder.Logger = new NullApplicationLogger();
        }

        /// <summary>
        /// Tests that an exception is thrown if the ImplementProcess() method is called with 'RightSideColumns' parameter less than 1.
        /// </summary>
        [Test]
        public void ImplementProcess_NumberOfColumnsParameterLessThan1()
        {
            testMatrixColumnAdder.GetInputSlot("InputMatrix").DataValue = new Matrix(2, 3);
            testMatrixColumnAdder.GetInputSlot("NumberOfColumns").DataValue = 0;
            testMatrixColumnAdder.GetInputSlot("LeftSide").DataValue = true;
            testMatrixColumnAdder.GetInputSlot("DefaultValue").DataValue = 1.0;

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrixColumnAdder.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'NumberOfColumns' must be greater than or equal to 1."));
            Assert.AreEqual("NumberOfColumns", e.ParamName);
        }

        /// <summary>
        /// Success tests for the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            testMatrixColumnAdder.GetInputSlot("InputMatrix").DataValue = new Matrix(2, 3, new Double[] { 1, 2, 3, 4, 5, 6 });
            testMatrixColumnAdder.GetInputSlot("NumberOfColumns").DataValue = 1;
            testMatrixColumnAdder.GetInputSlot("LeftSide").DataValue = true;
            testMatrixColumnAdder.GetInputSlot("DefaultValue").DataValue = 7.0;

            testMatrixColumnAdder.Process();
            Matrix outputMatrix = (Matrix)testMatrixColumnAdder.GetOutputSlot("OutputMatrix").DataValue;

            Assert.AreEqual(7.0, outputMatrix.GetElement(1, 1));
            Assert.AreEqual(1, outputMatrix.GetElement(1, 2));
            Assert.AreEqual(2, outputMatrix.GetElement(1, 3));
            Assert.AreEqual(3, outputMatrix.GetElement(1, 4));
            Assert.AreEqual(7.0, outputMatrix.GetElement(2, 1));
            Assert.AreEqual(4, outputMatrix.GetElement(2, 2));
            Assert.AreEqual(5, outputMatrix.GetElement(2, 3));
            Assert.AreEqual(6, outputMatrix.GetElement(2, 4));
        }
    }
}
