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

namespace SimpleML.Containers.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.Containers.Matrix.
    /// </summary>
    public class MatrixTests
    {
        private Matrix testMatrix;

        [SetUp]
        protected void SetUp()
        {
            testMatrix = new Matrix(3, 2, new Double[] {1, 2, 3, 4, 5, 6});
        }

        /// <summary>
        /// Tests that an exception is thrown if an 'mDimension' parameter less than 1 is passed to the constructor.
        /// </summary>
        [Test]
        public void Constructor_MDimension0()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix = new Matrix(0, 2, new Double[] { 1, 2, 3, 4, 5, 6 });
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'mDimension' must be greater than 0."));
            Assert.AreEqual("mDimension", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if an 'nDimension' parameter less than 1 is passed to the constructor.
        /// </summary>
        [Test]
        public void Constructor_NDimension0()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix = new Matrix(3, 0, new Double[] { 1, 2, 3, 4, 5, 6 });
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'nDimension' must be greater than 0."));
            Assert.AreEqual("nDimension", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if a 'data' parameter of incorrect length is passed to the constructor.
        /// </summary>
        [Test]
        public void Constructor_DataIncorrectLength()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix = new Matrix(3, 2, new Double[] { 1, 2, 3, 4, 5 });
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Length of parameter 'data' must equal to the product of the 'm' and 'n' dimensions."));
            Assert.AreEqual("data", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the GetElement() method is called with an 'mIndex' parameter less than 1.
        /// </summary>
        [Test]
        public void GetElement_MParameter0()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix.GetElement(0, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'mIndex' must be greater than 0."));
            Assert.AreEqual("mIndex", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the GetElement() method is called with an 'mIndex' parameter larger than the dimension of the matrix.
        /// </summary>
        [Test]
        public void GetElement_MParameterLargerThanDimension()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix.GetElement(4, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'mIndex' is larger than the 'm' dimension of the matrix."));
            Assert.AreEqual("mIndex", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the GetElement() method is called with an 'nIndex' parameter less than 1.
        /// </summary>
        [Test]
        public void GetElement_NParameter0()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix.GetElement(3, 0);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'nIndex' must be greater than 0."));
            Assert.AreEqual("nIndex", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the GetElement() method is called with an 'nIndex' parameter larger than the dimension of the matrix.
        /// </summary>
        [Test]
        public void GetElement_NParameterLargerThanDimension()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix.GetElement(3, 3);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'nIndex' is larger than the 'n' dimension of the matrix."));
            Assert.AreEqual("nIndex", e.ParamName);
        }

        /// <summary>
        /// Success tests for the GetElement() method.
        /// </summary>
        [Test]
        public void GetElement()
        {
            Assert.AreEqual(1, testMatrix.GetElement(1, 1));
            Assert.AreEqual(2, testMatrix.GetElement(1, 2));
            Assert.AreEqual(3, testMatrix.GetElement(2, 1));
            Assert.AreEqual(4, testMatrix.GetElement(2, 2));
            Assert.AreEqual(5, testMatrix.GetElement(3, 1));
            Assert.AreEqual(6, testMatrix.GetElement(3, 2));
        }

        /// <summary>
        /// Tests that an exception is thrown if the SetElement() method is called with an 'mIndex' parameter less than 1.
        /// </summary>
        [Test]
        public void SetElement_MParameter0()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix.SetElement(0, 1, 99);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'mIndex' must be greater than 0."));
            Assert.AreEqual("mIndex", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the SetElement() method is called with an 'mIndex' parameter larger than the dimension of the matrix.
        /// </summary>
        [Test]
        public void SetElement_MParameterLargerThanDimension()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix.SetElement(4, 1, 99);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'mIndex' is larger than the 'm' dimension of the matrix."));
            Assert.AreEqual("mIndex", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the SetElement() method is called with an 'nIndex' parameter less than 1.
        /// </summary>
        [Test]
        public void SetElement_NParameter0()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix.SetElement(3, 0, 99);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'nIndex' must be greater than 0."));
            Assert.AreEqual("nIndex", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the SetElement() method is called with an 'nIndex' parameter larger than the dimension of the matrix.
        /// </summary>
        [Test]
        public void SetElement_NParameterLargerThanDimension()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix.SetElement(3, 3, 99);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'nIndex' is larger than the 'n' dimension of the matrix."));
            Assert.AreEqual("nIndex", e.ParamName);
        }

        /// <summary>
        /// Success tests for the SetElement() method.
        /// </summary>
        [Test]
        public void SetElement()
        {
            testMatrix.SetElement(1, 1, 6);
            testMatrix.SetElement(1, 2, 5);
            testMatrix.SetElement(2, 1, 4);
            testMatrix.SetElement(2, 2, 3);
            testMatrix.SetElement(3, 1, 2);
            testMatrix.SetElement(3, 2, 1);

            Assert.AreEqual(6, testMatrix.GetElement(1, 1));
            Assert.AreEqual(5, testMatrix.GetElement(1, 2));
            Assert.AreEqual(4, testMatrix.GetElement(2, 1));
            Assert.AreEqual(3, testMatrix.GetElement(2, 2));
            Assert.AreEqual(2, testMatrix.GetElement(3, 1));
            Assert.AreEqual(1, testMatrix.GetElement(3, 2));
        }

        /// <summary>
        /// Tests that an exception is thrown if the * operator is used on matrices with incompatible dimensions
        /// </summary>
        [Test]
        public void MultiplyOperator_IncompatibleDimensions()
        {
            Matrix testMatrix2 = new Matrix(3, 4, new Double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                Matrix result = testMatrix * testMatrix2;
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Attempt to multiply matrices with incompatible dimensions '3 x 2' and '3 x 4'."));
            Assert.AreEqual("multiplierMatrix", e.ParamName);
        }

        /// <summary>
        /// Success tests for the * operator.
        /// </summary>
        [Test]
        public void MultiplyOperator()
        {
            testMatrix = new Matrix(3, 2, new Double[] { 2, 3, 5, 7, 11, 13 });
            Matrix testMatrix2 = new Matrix(2, 4, new Double[] { 17, 19, 23, 41, 29, 31, 37, 43 });

            Matrix result = testMatrix * testMatrix2;

            Assert.AreEqual(121, result.GetElement(1, 1));
            Assert.AreEqual(131, result.GetElement(1, 2));
            Assert.AreEqual(157, result.GetElement(1, 3));
            Assert.AreEqual(211, result.GetElement(1, 4));
            Assert.AreEqual(288, result.GetElement(2, 1));
            Assert.AreEqual(312, result.GetElement(2, 2));
            Assert.AreEqual(374, result.GetElement(2, 3));
            Assert.AreEqual(506, result.GetElement(2, 4));
            Assert.AreEqual(564, result.GetElement(3, 1));
            Assert.AreEqual(612, result.GetElement(3, 2));
            Assert.AreEqual(734, result.GetElement(3, 3));
            Assert.AreEqual(1010, result.GetElement(3, 4));
        }

        /// <summary>
        /// Tests that an exception is thrown if the + operator is used on matrices with different 'm' dimensions
        /// </summary>
        [Test]
        public void AdditionOperator_DifferentMDimensions()
        {
            Matrix testMatrix2 = new Matrix(2, 2);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                Matrix result = testMatrix + testMatrix2;
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Attempt to add matricies with differing 'm' dimensions 3 and 2."));
            Assert.AreEqual("firstTerm", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the + operator is used on matrices with different 'n' dimensions
        /// </summary>
        [Test]
        public void AdditionOperator_DifferentNDimensions()
        {
            Matrix testMatrix2 = new Matrix(3, 3);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                Matrix result = testMatrix + testMatrix2;
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Attempt to add matricies with differing 'n' dimensions 2 and 3."));
            Assert.AreEqual("firstTerm", e.ParamName);
        }

        /// <summary>
        /// Success tests for the + operator.
        /// </summary>
        [Test]
        public void AdditionOperator()
        {
            testMatrix = new Matrix(3, 2, new Double[] { -2, 1, 0.5, 7, -1.5, 12 });
            Matrix testMatrix2 = new Matrix(3, 2, new Double[] { 7, -0.6, -3, 23.4, 0, -1 });

            Matrix result = testMatrix + testMatrix2;

            Assert.AreEqual(5, result.GetElement(1, 1));
            Assert.AreEqual(0.4, result.GetElement(1, 2));
            Assert.AreEqual(-2.5, result.GetElement(2, 1));
            Assert.AreEqual(30.4, result.GetElement(2, 2));
            Assert.AreEqual(-1.5, result.GetElement(3, 1));
            Assert.AreEqual(11, result.GetElement(3, 2));
        }

        /// <summary>
        /// Tests that an exception is thrown if the - operator is used on matrices with different 'm' dimensions
        /// </summary>
        [Test]
        public void SubtractionOperator_DifferentMDimensions()
        {
            Matrix testMatrix2 = new Matrix(2, 2);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                Matrix result = testMatrix - testMatrix2;
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Attempt to subtract matricies with differing 'm' dimensions 3 and 2."));
            Assert.AreEqual("firstTerm", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the - operator is used on matrices with different 'n' dimensions
        /// </summary>
        [Test]
        public void SubtractionOperator_DifferentNDimensions()
        {
            Matrix testMatrix2 = new Matrix(3, 3);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                Matrix result = testMatrix - testMatrix2;
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Attempt to subtract matricies with differing 'n' dimensions 2 and 3."));
            Assert.AreEqual("firstTerm", e.ParamName);
        }

        /// <summary>
        /// Success tests for the - operator.
        /// </summary>
        [Test]
        public void SubtractionOperator()
        {
            testMatrix = new Matrix(3, 2, new Double[] { -2, 1, 0.5, 7, -1.5, 12 });
            Matrix testMatrix2 = new Matrix(3, 2, new Double[] { 7, -0.6, -3, 23.4, 0, -1 });

            Matrix result = testMatrix - testMatrix2;

            Assert.AreEqual(-9, result.GetElement(1, 1));
            Assert.AreEqual(1.6, result.GetElement(1, 2));
            Assert.AreEqual(3.5, result.GetElement(2, 1));
            Assert.AreEqual(-16.4, result.GetElement(2, 2));
            Assert.AreEqual(-1.5, result.GetElement(3, 1));
            Assert.AreEqual(13, result.GetElement(3, 2));
        }

        /// <summary>
        /// Success tests for the ApplyElementWiseOperation() method.
        /// </summary>
        [Test]
        public void ApplyElementWiseOperation()
        {
            testMatrix.ApplyElementWiseOperation(new Func<Double, Double>( (x) => { return x * 2; } ) );

            Assert.AreEqual(2, testMatrix.GetElement(1, 1));
            Assert.AreEqual(4, testMatrix.GetElement(1, 2));
            Assert.AreEqual(6, testMatrix.GetElement(2, 1));
            Assert.AreEqual(8, testMatrix.GetElement(2, 2));
            Assert.AreEqual(10, testMatrix.GetElement(3, 1));
            Assert.AreEqual(12, testMatrix.GetElement(3, 2));
        }

        /// <summary>
        /// Success tests for the MDimension property.
        /// </summary>
        [Test]
        public void MDimension()
        {
            Assert.AreEqual(3, testMatrix.MDimension);
        }

        /// <summary>
        /// Success tests for the NDimension property.
        /// </summary>
        [Test]
        public void NDimension()
        {
            Assert.AreEqual(2, testMatrix.NDimension);
        }

        /// <summary>
        /// Success tests for the Transpose() method.
        /// </summary>
        [Test]
        public void Transpose()
        {
            Matrix result = testMatrix.Transpose();

            Assert.AreEqual(2, result.MDimension);
            Assert.AreEqual(3, result.NDimension);
            Assert.AreEqual(1, result.GetElement(1, 1));
            Assert.AreEqual(3, result.GetElement(1, 2));
            Assert.AreEqual(5, result.GetElement(1, 3));
            Assert.AreEqual(2, result.GetElement(2, 1));
            Assert.AreEqual(4, result.GetElement(2, 2));
            Assert.AreEqual(6, result.GetElement(2, 3));
        }

        /// <summary>
        /// Success tests for the SumHorizontally() method.
        /// </summary>
        [Test]
        public void SumHorizontally()
        {
            testMatrix = new Matrix(4, 3, new Double[] { 1, 2, -2, -27, 5, 6, 7, 0.19, 9, -5.21, 11, 12 });

            Matrix result = testMatrix.SumHorizontally();

            Assert.AreEqual(1, result.MDimension);
            Assert.AreEqual(3, result.NDimension);
            Assert.AreEqual(-24.21, result.GetElement(1, 1));
            Assert.AreEqual(18.19, result.GetElement(1, 2));
            Assert.AreEqual(25, result.GetElement(1, 3));
        }

        /// <summary>
        /// Tests that an exception is thrown if the GetColumnSubset() method is called with a 'startColumn' parameter less than 1.
        /// </summary>
        [Test]
        public void GetColumnSubset_StartColumnParameterLessThan1()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix.GetColumnSubset(0, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'startColumn' must be greater than or equal to 1."));
            Assert.AreEqual("startColumn", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the GetColumnSubset() method is called with a 'startColumn' parameter greater than than n dimension of the source matrix.
        /// </summary>
        [Test]
        public void GetColumnSubset_StartColumnParameterGreaterThanMatrixNDimension()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix.GetColumnSubset(3, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'startColumn' is greater than the 'n' dimension of the matrix."));
            Assert.AreEqual("startColumn", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the GetColumnSubset() method is called with a 'numberOfColumns' parameter less than 1.
        /// </summary>
        [Test]
        public void GetColumnSubset_NumberOfColumnsParameterLessThan1()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix.GetColumnSubset(1, 0);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'numberOfColumns' must be greater than or equal to 1."));
            Assert.AreEqual("numberOfColumns", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the GetColumnSubset() method is called with a 'numberOfColumns' parameter that exceeds the n dimension of the source matrix..
        /// </summary>
        [Test]
        public void GetColumnSubset_NumberOfColumnsParameterExceedsMatrixDimension()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix.GetColumnSubset(1, 3);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'numberOfColumns' exceeds the 'n' dimension of the matrix."));
            Assert.AreEqual("numberOfColumns", e.ParamName);
        }

        /// <summary>
        /// Success tests for the GetColumnSubset() method.
        /// </summary>
        [Test]
        public void GetColumnSubset()
        {
            testMatrix = new Matrix(2, 4, new Double[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            Matrix result = testMatrix.GetColumnSubset(3, 2);

            Assert.AreEqual(3, result.GetElement(1, 1));
            Assert.AreEqual(4, result.GetElement(1, 2));
            Assert.AreEqual(7, result.GetElement(2, 1));
            Assert.AreEqual(8, result.GetElement(2, 2));
        }

        /// <summary>
        /// Tests that an exception is thrown if the GetRowSubset() method is called with a 'startRow' parameter less than 1.
        /// </summary>
        [Test]
        public void GetRowSubset_StartRowParameterLessThan1()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix.GetRowSubset(0, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'startRow' must be greater than or equal to 1."));
            Assert.AreEqual("startRow", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the GetRowSubset() method is called with a 'startRow' parameter greater than than m dimension of the source matrix.
        /// </summary>
        [Test]
        public void GetRowSubset_StartRowParameterGreaterThanMatrixMDimension()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix.GetRowSubset(4, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'startRow' is greater than the 'm' dimension of the matrix."));
            Assert.AreEqual("startRow", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the GetRowSubset() method is called with a 'numberOfRows' parameter less than 1.
        /// </summary>
        [Test]
        public void GetRowSubset_NumberOfRowsParameterLessThan1()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix.GetRowSubset(1, 0);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'numberOfRows' must be greater than or equal to 1."));
            Assert.AreEqual("numberOfRows", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the GetRowSubset() method is called with a 'numberOfRows' parameter that exceeds the m dimension of the source matrix..
        /// </summary>
        [Test]
        public void GetRowSubset_NumberOfRowsParameterExceedsMatrixDimension()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrix.GetRowSubset(2, 3);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'numberOfRows' exceeds the 'm' dimension of the matrix."));
            Assert.AreEqual("numberOfRows", e.ParamName);
        }

        /// <summary>
        /// Success tests for the GetRowSubset() method.
        /// </summary>
        [Test]
        public void GetRowSubset()
        {
            testMatrix = new Matrix(4, 2, new Double[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            Matrix result = testMatrix.GetRowSubset(3, 2);

            Assert.AreEqual(5, result.GetElement(1, 1));
            Assert.AreEqual(6, result.GetElement(1, 2));
            Assert.AreEqual(7, result.GetElement(2, 1));
            Assert.AreEqual(8, result.GetElement(2, 2));
        }
    }
}
