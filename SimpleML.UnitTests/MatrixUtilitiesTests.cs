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
    /// Unit tests for class SimpleML.MatrixUtilitiesTests.
    /// </summary>
    public class MatrixUtilitiesTests
    {
        private MatrixUtilities testMatrixUtilities;

        [SetUp]
        protected void SetUp()
        {
            testMatrixUtilities = new MatrixUtilities();
        }

        /// <summary>
        /// Tests that an exception is thrown if the AddColumns() method is called with 'numberOfColumns' parameters equal to 0.
        /// </summary>
        [Test]
        public void AddColumns_NumberOfColumnsParameter0()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrixUtilities.AddColumns(new Matrix(1, 1), 0, true, 1.0);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'numberOfColumns' must be greater than or equal to 1."));
            Assert.AreEqual("numberOfColumns", e.ParamName);
        }

        /// <summary>
        /// Success tests for the AddColumns() method.
        /// </summary>
        [Test]
        public void AddColumns()
        {
            // First test adds 2 columns to the left of the test matrix.
            // This matrix...
            //
            //  1   2   3
            //  4   5   6
            //  7   8   9
            // 10  11  12
            //
            // ...should become...
            //
            //  1.1  1.1   1   2   3
            //  1.1  1.1   4   5   6
            //  1.1  1.1   7   8   9
            //  1.1  1.1  10  11  12

            Matrix testMatrix = new Matrix(4, 3, new Double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });

            Matrix resultMatrix = testMatrixUtilities.AddColumns(testMatrix, 2, true, 1.1);

            Assert.AreEqual(4, resultMatrix.MDimension);
            Assert.AreEqual(5, resultMatrix.NDimension);
            Assert.That(resultMatrix.GetElement(1, 1), NUnit.Framework.Is.EqualTo(1.1).Within(1e-2));
            Assert.That(resultMatrix.GetElement(1, 2), NUnit.Framework.Is.EqualTo(1.1).Within(1e-2));
            Assert.That(resultMatrix.GetElement(1, 3), NUnit.Framework.Is.EqualTo(1.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(1, 4), NUnit.Framework.Is.EqualTo(2.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(1, 5), NUnit.Framework.Is.EqualTo(3.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(2, 1), NUnit.Framework.Is.EqualTo(1.1).Within(1e-2));
            Assert.That(resultMatrix.GetElement(2, 2), NUnit.Framework.Is.EqualTo(1.1).Within(1e-2));
            Assert.That(resultMatrix.GetElement(2, 3), NUnit.Framework.Is.EqualTo(4.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(2, 4), NUnit.Framework.Is.EqualTo(5.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(2, 5), NUnit.Framework.Is.EqualTo(6.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(3, 1), NUnit.Framework.Is.EqualTo(1.1).Within(1e-2));
            Assert.That(resultMatrix.GetElement(3, 2), NUnit.Framework.Is.EqualTo(1.1).Within(1e-2));
            Assert.That(resultMatrix.GetElement(3, 3), NUnit.Framework.Is.EqualTo(7.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(3, 4), NUnit.Framework.Is.EqualTo(8.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(3, 5), NUnit.Framework.Is.EqualTo(9.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(4, 1), NUnit.Framework.Is.EqualTo(1.1).Within(1e-2));
            Assert.That(resultMatrix.GetElement(4, 2), NUnit.Framework.Is.EqualTo(1.1).Within(1e-2));
            Assert.That(resultMatrix.GetElement(4, 3), NUnit.Framework.Is.EqualTo(10.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(4, 4), NUnit.Framework.Is.EqualTo(11.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(4, 5), NUnit.Framework.Is.EqualTo(12.0).Within(1e-1));

            // Second test adds 2 columns to the right of the test matrix.
            // This matrix...
            //
            //  1   2   3
            //  4   5   6
            //  7   8   9
            // 10  11  12
            //
            // ...should become...
            //
            //  1   2   3  1.1  1.1
            //  4   5   6  1.1  1.1
            //  7   8   9  1.1  1.1
            // 10  11  12  1.1  1.1

            testMatrix = new Matrix(4, 3, new Double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });

            resultMatrix = testMatrixUtilities.AddColumns(testMatrix, 2, false, 1.2);

            Assert.AreEqual(4, resultMatrix.MDimension);
            Assert.AreEqual(5, resultMatrix.NDimension);
            Assert.That(resultMatrix.GetElement(1, 1), NUnit.Framework.Is.EqualTo(1.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(1, 2), NUnit.Framework.Is.EqualTo(2.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(1, 3), NUnit.Framework.Is.EqualTo(3.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(1, 4), NUnit.Framework.Is.EqualTo(1.2).Within(1e-2));
            Assert.That(resultMatrix.GetElement(1, 5), NUnit.Framework.Is.EqualTo(1.2).Within(1e-2));
            Assert.That(resultMatrix.GetElement(2, 1), NUnit.Framework.Is.EqualTo(4.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(2, 2), NUnit.Framework.Is.EqualTo(5.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(2, 3), NUnit.Framework.Is.EqualTo(6.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(2, 4), NUnit.Framework.Is.EqualTo(1.2).Within(1e-2));
            Assert.That(resultMatrix.GetElement(2, 5), NUnit.Framework.Is.EqualTo(1.2).Within(1e-2));
            Assert.That(resultMatrix.GetElement(3, 1), NUnit.Framework.Is.EqualTo(7.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(3, 2), NUnit.Framework.Is.EqualTo(8.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(3, 3), NUnit.Framework.Is.EqualTo(9.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(3, 4), NUnit.Framework.Is.EqualTo(1.2).Within(1e-2));
            Assert.That(resultMatrix.GetElement(3, 5), NUnit.Framework.Is.EqualTo(1.2).Within(1e-2));
            Assert.That(resultMatrix.GetElement(4, 1), NUnit.Framework.Is.EqualTo(10.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(4, 2), NUnit.Framework.Is.EqualTo(11.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(4, 3), NUnit.Framework.Is.EqualTo(12.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(4, 4), NUnit.Framework.Is.EqualTo(1.2).Within(1e-2));
            Assert.That(resultMatrix.GetElement(4, 5), NUnit.Framework.Is.EqualTo(1.2).Within(1e-2));
        }

        /// <summary>
        /// Tests that an exception is thrown if the JoinHorizontally() method is called with 'leftMatrix' and 'rightMatrix' parameters whose 'm' dimensions don't match.
        /// </summary>
        [Test]
        public void JoinHorizontally_ParameterMDimensionMismatch()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrixUtilities.JoinHorizontally(new Matrix(3, 4), new Matrix(2, 1));
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The 'm' dimension of parameter 'rightMatrix' '2' does not match the 'm' dimension of parameter 'leftMatrix' '3'."));
            Assert.AreEqual("rightMatrix", e.ParamName);
        }

        /// <summary>
        /// Success tests for the JoinHorizontally() method.
        /// </summary>
        [Test]
        public void JoinHorizontally()
        {
            // Test joins matrices...
            //
            //  1   2   3        4   5
            //  6   7   8        9  10
            //
            // ...to become...
            //
            //  1   2   3   4   5
            //  6   7   8   9  10

            Matrix leftMatrix = new Matrix(2, 3, new Double[] { 1.0, 2.0, 3.0, 6.0, 7.0, 8.0 });
            Matrix rightMatrix = new Matrix(2, 2, new Double[] { 4.0, 5.0, 9.0, 10.0 });

            Matrix resultMatrix = testMatrixUtilities.JoinHorizontally(leftMatrix, rightMatrix);

            Assert.AreEqual(2, resultMatrix.MDimension);
            Assert.AreEqual(5, resultMatrix.NDimension);
            Assert.That(resultMatrix.GetElement(1, 1), NUnit.Framework.Is.EqualTo(1.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(1, 2), NUnit.Framework.Is.EqualTo(2.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(1, 3), NUnit.Framework.Is.EqualTo(3.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(1, 4), NUnit.Framework.Is.EqualTo(4.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(1, 5), NUnit.Framework.Is.EqualTo(5.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(2, 1), NUnit.Framework.Is.EqualTo(6.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(2, 2), NUnit.Framework.Is.EqualTo(7.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(2, 3), NUnit.Framework.Is.EqualTo(8.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(2, 4), NUnit.Framework.Is.EqualTo(9.0).Within(1e-1));
            Assert.That(resultMatrix.GetElement(2, 5), NUnit.Framework.Is.EqualTo(10.0).Within(1e-1));
        }
    }
}
