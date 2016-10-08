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

namespace SimpleML.Containers
{
    /// <summary>
    /// Represents a mathematical matrix.
    /// </summary>
    public class Matrix
    {
        /// <summary>The data underlying the matrix.</summary>
        private Double[] data;
        /// <summary>The size of the 'm' or vertical dimension of the matrix.</summary>
        private Int32 mDimension;
        /// <summary>The size of the 'n' or horizontal dimension of the matrix.</summary>
        private Int32 nDimension;

        /// <summary>
        /// The size of the 'm' or vertical dimension of the matrix.
        /// </summary>
        public Int32 MDimension
        {
            get
            {
                return mDimension;
            }
        }

        /// <summary>
        /// The size of the 'n' or horizontal dimension of the matrix.
        /// </summary>
        public Int32 NDimension
        {
            get
            {
                return nDimension;
            }
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.Matrix class.
        /// </summary>
        /// <param name="mDimension">The size of the 'm' or vertical dimension of the matrix.</param>
        /// <param name="nDimension">The size of the 'n' or horizontal dimension of the matrix.</param>
        /// <param name="data">The complete data for the matrix arranged in rows and then columns (i.e. in English reading format).</param>
        public Matrix(Int32 mDimension, Int32 nDimension, Double[] data)
        {
            if (mDimension < 1)
            {
                throw new ArgumentException("Parameter 'mDimension' must be greater than 0.", "mDimension");
            }
            if (nDimension < 1)
            {
                throw new ArgumentException("Parameter 'nDimension' must be greater than 0.", "nDimension");
            }
            if (mDimension * nDimension != (data.Length))
            {
                throw new ArgumentException("Length of parameter 'data' must equal to the product of the 'm' and 'n' dimensions.", "data");
            }

            this.data = data;
            this.mDimension = mDimension;
            this.nDimension = nDimension;
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.Matrix class with all elements 0.
        /// </summary>
        /// <param name="mDimension">The size of the 'm' or vertical dimension of the matrix.</param>
        /// <param name="nDimension">The size of the 'n' or horizontal dimension of the matrix.</param>
        public Matrix(Int32 mDimension, Int32 nDimension)
            : this(mDimension, nDimension, new Double[mDimension * nDimension])
        {
        }

        /// <summary>
        /// Returns the value of the element of the matrix at the specified position.
        /// </summary>
        /// <param name="mIndex">The position of the element along the 'm' (vertical) axis.</param>
        /// <param name="nIndex">The position of the element along the 'n' (horizontal) axis.</param>
        /// <returns>The value of the element.</returns>
        public Double GetElement(Int32 mIndex, Int32 nIndex)
        {
            VerifyIndexParameters(mIndex, nIndex);
            
            Int32 dataIndex = ((mIndex - 1) * nDimension + nIndex) - 1;
            return data[dataIndex];
        }

        /// <summary>
        /// Sets the value of the element of the matrix at the specified position.
        /// </summary>
        /// <param name="mIndex">The position of the element along the 'm' (vertical) axis.</param>
        /// <param name="nIndex">The position of the element along the 'n' (horizontal) axis.</param>
        /// <param name="value">The value of the element.</param>
        public void SetElement(Int32 mIndex, Int32 nIndex, Double value)
        {
            VerifyIndexParameters(mIndex, nIndex);

            Int32 dataIndex = ((mIndex - 1) * nDimension + nIndex) - 1;
            data[dataIndex] = value;
        }

        /// <summary>
        /// Multiplies a matrix by another matrix.
        /// </summary>
        /// <param name="multiplicandMatrix">The matrix to multiply.</param>
        /// <param name="multiplierMatrix">The matrix to multiply by.</param>
        /// <returns>The product matrix.</returns>
        public static Matrix operator * (Matrix multiplicandMatrix, Matrix multiplierMatrix)
        {
            if (multiplicandMatrix.nDimension != multiplierMatrix.mDimension)
            {
                throw new ArgumentException("Attempt to multiply matrices with incompatible dimensions '" + multiplicandMatrix.mDimension + " x " + multiplicandMatrix.nDimension + "' and '" + multiplierMatrix.mDimension + " x " + multiplierMatrix.nDimension + "'.", "multiplierMatrix");
            }

            // TODO: Could be made more efficient by building the data array of the return matrix before construction (hence avoid multiple parameter checks called by GetElement())

            Matrix returnMatrix = new Matrix(multiplicandMatrix.mDimension, multiplierMatrix.nDimension, new Double[multiplicandMatrix.mDimension * multiplierMatrix.nDimension]);
            for (Int32 i = 1; i <= multiplicandMatrix.mDimension; i++)
            {
                for (Int32 j = 1; j <= multiplierMatrix.nDimension; j++)
                {
                    Double elementTotal = 0;

                    for (Int32 k = 1; k <= multiplicandMatrix.nDimension; k++)
                    {
                        elementTotal += multiplicandMatrix.GetElement(i, k) * multiplierMatrix.GetElement(k, j);
                    }

                    returnMatrix.SetElement(i, j, elementTotal);
                }
            }

            return returnMatrix;
        }

        /// <summary>
        /// Adds two matrices.
        /// </summary>
        /// <param name="firstTerm">The first matrix.</param>
        /// <param name="secondTerm">The second matrix.</param>
        /// <returns>The result of the addition.</returns>
        public static Matrix operator + (Matrix firstTerm, Matrix secondTerm)
        {
            if (firstTerm.MDimension != secondTerm.MDimension)
            {
                throw new ArgumentException("Attempt to add matricies with differing 'm' dimensions " + firstTerm.mDimension + " and " + secondTerm.mDimension + ".", "firstTerm");
            }
            if (firstTerm.NDimension != secondTerm.NDimension)
            {
                throw new ArgumentException("Attempt to add matricies with differing 'n' dimensions " + firstTerm.nDimension + " and " + secondTerm.nDimension + ".", "firstTerm");
            }

            Matrix returnMatrix = new Matrix(firstTerm.mDimension, firstTerm.nDimension);
            for (Int32 i = 1; i <= firstTerm.mDimension; i++)
            {
                for (Int32 j = 1; j <= firstTerm.nDimension; j++)
                {
                    Double element = firstTerm.GetElement(i, j) + secondTerm.GetElement(i, j);
                    returnMatrix.SetElement(i, j, element);
                }
            }

            return returnMatrix;
        }

        /// <summary>
        /// Subtracts one matrix from another.
        /// </summary>
        /// <param name="firstTerm">The matrix to subtract from.</param>
        /// <param name="secondTerm">The matrix to subtract.</param>
        /// <returns>The result of the subtraction.</returns>
        public static Matrix operator - (Matrix firstTerm, Matrix secondTerm)
        {
            if (firstTerm.MDimension != secondTerm.MDimension)
            {
                throw new ArgumentException("Attempt to subtract matricies with differing 'm' dimensions " + firstTerm.mDimension + " and " + secondTerm.mDimension + ".", "firstTerm");
            }
            if (firstTerm.NDimension != secondTerm.NDimension)
            {
                throw new ArgumentException("Attempt to subtract matricies with differing 'n' dimensions " + firstTerm.nDimension + " and " + secondTerm.nDimension + ".", "firstTerm");
            }

            Matrix returnMatrix = new Matrix(firstTerm.mDimension, firstTerm.nDimension);
            for (Int32 i = 1; i <= firstTerm.mDimension; i++)
            {
                for (Int32 j = 1; j <= firstTerm.nDimension; j++)
                {
                    Double element = firstTerm.GetElement(i, j) - secondTerm.GetElement(i, j);
                    returnMatrix.SetElement(i, j, element);
                }
            }

            return returnMatrix;
        }

        /// <summary>
        /// Returns the tranpose of this matrix.
        /// </summary>
        /// <returns>The tranpose of this matrix.</returns>
        public Matrix Transpose()
        {
            // TODO: Could be made more efficient by building the data array of the return matrix before construction (hence avoid multiple parameter checks called by GetElement() and SetElement())

            Matrix returnMatrix = new Matrix(nDimension, mDimension, new Double[data.Length]);
            for (Int32 i = 1; i <= mDimension; i++)
            {
                for (Int32 j = 1; j <= nDimension; j++)
                {
                    Double currentElementValue = GetElement(i, j);
                    returnMatrix.SetElement(j, i, currentElementValue);
                }
            }

            return returnMatrix;
        }

        /// <summary>
        /// Applies the specified element-wise operation to the matrix.
        /// </summary>
        /// <param name="operation">The operation to apply.</param>
        public void ApplyElementWiseOperation(Func<Double, Double> operation)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = operation.Invoke(data[i]);
            }
        }

        /// <summary>
        /// Sums the columns of the matrix, producing a matrix with 'n' dimension matching the original matrix, and 'm' dimension of 1.
        /// </summary>
        /// <returns>A matrix containing the sums of the columns.</returns>
        public Matrix SumHorizontally()
        {
            Matrix returnMatrix = new Matrix(1, nDimension);

            for (int i = 0; i <= ((mDimension - 1) * nDimension); i = i + nDimension)
            {
                for (int j = 0; j < nDimension; j++)
                {
                    returnMatrix.SetElement(1, j + 1, returnMatrix.GetElement(1, j + 1) + data[i + j]);
                }
            }

            return returnMatrix;
        }

        /// <summary>
        /// Returns a matrix defined by the specified subset of columns of this matrix.
        /// </summary>
        /// <param name="startColumn">The first column of this matrix to include in the subset.</param>
        /// <param name="numberOfColumns">The number of columns of this matrix to include in the subset.</param>
        /// <returns>The subset matrix.</returns>
        public Matrix GetColumnSubset(Int32 startColumn, Int32 numberOfColumns)
        {
            if (startColumn < 1)
            {
                throw new ArgumentException("Parameter 'startColumn' must be greater than or equal to 1.", "startColumn");
            }
            if (startColumn > nDimension)
            {
                throw new ArgumentException("Parameter 'startColumn' is greater than the 'n' dimension of the matrix.", "startColumn");
            }
            if (numberOfColumns < 1)
            {
                throw new ArgumentException("Parameter 'numberOfColumns' must be greater than or equal to 1.", "numberOfColumns");
            }
            if ( (startColumn + numberOfColumns - 1) > nDimension )
            {
                throw new ArgumentException("Parameter 'numberOfColumns' exceeds the 'n' dimension of the matrix.", "numberOfColumns");
            }

            Matrix returnMatrix = new Matrix(mDimension, numberOfColumns);
            for (int i = 1; i <= mDimension; i++)
            {
                for (int j = 1; j <= numberOfColumns; j++)
                {
                    returnMatrix.SetElement(i, j, GetElement( i, (startColumn + j - 1) ));
                }
            }

            return returnMatrix;
        }

        /// <summary>
        /// Returns a matrix defined by the specified subset of rows of this matrix.
        /// </summary>
        /// <param name="startRow">The first row of this matrix to include in the subset.</param>
        /// <param name="numberOfRows">The number of rows of this matrix to include in the subset.</param>
        /// <returns>The subset matrix.</returns>
        public Matrix GetRowSubset(Int32 startRow, Int32 numberOfRows)
        {
            if (startRow < 1)
            {
                throw new ArgumentException("Parameter 'startRow' must be greater than or equal to 1.", "startRow");
            }
            if (startRow > mDimension)
            {
                throw new ArgumentException("Parameter 'startRow' is greater than the 'm' dimension of the matrix.", "startRow");
            }
            if (numberOfRows < 1)
            {
                throw new ArgumentException("Parameter 'numberOfRows' must be greater than or equal to 1.", "numberOfRows");
            }
            if ((startRow + numberOfRows - 1) > mDimension)
            {
                throw new ArgumentException("Parameter 'numberOfRows' exceeds the 'm' dimension of the matrix.", "numberOfRows");
            }

            Matrix returnMatrix = new Matrix(numberOfRows, nDimension);
            for (int i = 1; i <= numberOfRows; i++)
            {
                for (int j = 1; j <= nDimension; j++)
                {
                    returnMatrix.SetElement(i, j, GetElement( (startRow + i -1), j));
                }
            }

            return returnMatrix;
        }

        #region Private Methods

        /// <summary>
        /// Verifies parameters containing the 'm' and 'n' indices of the matrix and throws exceptions on any errors.  Used by public methods like GetElement() and SetElement().
        /// </summary>
        /// <param name="mIndex">The 'm' index parameter.</param>
        /// <param name="nIndex">The 'n' index parameter.</param>
        private void VerifyIndexParameters(Int32 mIndex, Int32 nIndex)
        {
            if (mIndex < 1)
            {
                throw new ArgumentException("Parameter 'mIndex' must be greater than 0.", "mIndex");
            }
            if (mIndex > mDimension)
            {
                throw new ArgumentException("Parameter 'mIndex' is larger than the 'm' dimension of the matrix.", "mIndex");
            }
            if (nIndex < 1)
            {
                throw new ArgumentException("Parameter 'nIndex' must be greater than 0.", "nIndex");
            }
            if (nIndex > nDimension)
            {
                throw new ArgumentException("Parameter 'nIndex' is larger than the 'n' dimension of the matrix.", "nIndex");
            }
        }

        #endregion
    }
}
