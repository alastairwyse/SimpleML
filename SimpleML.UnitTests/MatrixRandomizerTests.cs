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
    /// Unit tests for class SimpleML.MatrixRandomizer.
    /// </summary>
    public class MatrixRandomizerTests
    {
        private MatrixRandomizer testMatrixRandomizer;

        [SetUp]
        protected void SetUp()
        {
            testMatrixRandomizer = new MatrixRandomizer();
        }

        /// <summary>
        /// Success tests for the Randomize() method.
        /// </summary>
        [Test]
        public void Randomize()
        {
            Matrix inputMatrix = new Matrix(3, 4, new Double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
            
            Matrix resultMatrix = testMatrixRandomizer.Randomize(inputMatrix, 13);

            Assert.AreEqual(5, resultMatrix.GetElement(1, 1));
            Assert.AreEqual(6, resultMatrix.GetElement(1, 2));
            Assert.AreEqual(7, resultMatrix.GetElement(1, 3));
            Assert.AreEqual(8, resultMatrix.GetElement(1, 4));
            Assert.AreEqual(9, resultMatrix.GetElement(2, 1));
            Assert.AreEqual(10, resultMatrix.GetElement(2, 2));
            Assert.AreEqual(11, resultMatrix.GetElement(2, 3));
            Assert.AreEqual(12, resultMatrix.GetElement(2, 4));
            Assert.AreEqual(1, resultMatrix.GetElement(3, 1));
            Assert.AreEqual(2, resultMatrix.GetElement(3, 2));
            Assert.AreEqual(3, resultMatrix.GetElement(3, 3));
            Assert.AreEqual(4, resultMatrix.GetElement(3, 4));
        }
    }
}
