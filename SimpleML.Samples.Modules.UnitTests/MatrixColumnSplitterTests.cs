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
using SimpleML.Containers;
using SimpleML.Samples.Modules;

namespace SimpleML.Samples.Modules.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.Samples.Modules.MatrixColumnSplitter.
    /// </summary>
    public class MatrixColumnSplitterTests
    {
        private MatrixColumnSplitter testMatrixColumnSplitter;

        [SetUp]
        protected void SetUp()
        {
            testMatrixColumnSplitter = new MatrixColumnSplitter();
            testMatrixColumnSplitter.Logger = new NullApplicationLogger();
        }

        /// <summary>
        /// Tests that an exception is thrown if the ImplementProcess() method is called with 'RightSideColumns' parameter less than 1.
        /// </summary>
        [Test]
        public void ImplementProcess_RightSideColumnsParameterLessThan1()
        {
            testMatrixColumnSplitter.GetInputSlot("RightSideColumns").DataValue = 0;

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrixColumnSplitter.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'RightSideColumns' must be greater than 0."));
            Assert.AreEqual("RightSideColumns", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the ImplementProcess() method is called with 'RightSideColumns' parameter which is the same as the 'n' dimension of the matrix in parameter 'InputMatrix'.
        /// </summary>
        [Test]
        public void ImplementProcess_RightSideItemsParameterSameAsListSize()
        {
            Matrix inputMatrix = new Matrix(4, 3);
            testMatrixColumnSplitter.GetInputSlot("InputMatrix").DataValue = inputMatrix;
            testMatrixColumnSplitter.GetInputSlot("RightSideColumns").DataValue = 3;

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrixColumnSplitter.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'RightSideColumns' must be less than the 'n' dimension of the input matrix."));
            Assert.AreEqual("RightSideColumns", e.ParamName);
        }

        /// <summary>
        /// Success tests for the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            Matrix inputMatrix = new Matrix(2, 4, new Double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0 });
            testMatrixColumnSplitter.GetInputSlot("InputMatrix").DataValue = inputMatrix;
            testMatrixColumnSplitter.GetInputSlot("RightSideColumns").DataValue = 1;

            testMatrixColumnSplitter.Process();

            Matrix leftColumnsMatrix = (Matrix)testMatrixColumnSplitter.GetOutputSlot("LeftColumnsMatrix").DataValue;
            Matrix rightColumnsMatrix = (Matrix)testMatrixColumnSplitter.GetOutputSlot("RightColumnsMatrix").DataValue;

            Assert.AreEqual(2, leftColumnsMatrix.MDimension);
            Assert.AreEqual(3, leftColumnsMatrix.NDimension);
            Assert.That(leftColumnsMatrix.GetElement(1, 1), NUnit.Framework.Is.EqualTo(1.0).Within(1e-1));
            Assert.That(leftColumnsMatrix.GetElement(1, 2), NUnit.Framework.Is.EqualTo(2.0).Within(1e-1));
            Assert.That(leftColumnsMatrix.GetElement(1, 3), NUnit.Framework.Is.EqualTo(3.0).Within(1e-1));
            Assert.That(leftColumnsMatrix.GetElement(2, 1), NUnit.Framework.Is.EqualTo(5.0).Within(1e-1));
            Assert.That(leftColumnsMatrix.GetElement(2, 2), NUnit.Framework.Is.EqualTo(6.0).Within(1e-1));
            Assert.That(leftColumnsMatrix.GetElement(2, 3), NUnit.Framework.Is.EqualTo(7.0).Within(1e-1));
            Assert.AreEqual(2, rightColumnsMatrix.MDimension);
            Assert.AreEqual(1, rightColumnsMatrix.NDimension);
            Assert.That(rightColumnsMatrix.GetElement(1, 1), NUnit.Framework.Is.EqualTo(4.0).Within(1e-1));
            Assert.That(rightColumnsMatrix.GetElement(2, 1), NUnit.Framework.Is.EqualTo(8.0).Within(1e-1));
        }
    }
}
