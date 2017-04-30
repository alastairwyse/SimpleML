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
using SimpleML.Containers;

namespace SimpleML
{
    /// <summary>
    /// Contains common utility methods used to modify SimpleML.Containers.Matrix classes.
    /// </summary>
    public class MatrixUtilities
    {
        /// <summary>
        /// Initialises a new instance of the SimpleML.MatrixUtilities class.
        /// </summary>
        public MatrixUtilities()
        {
        }

        /// <summary>
        /// Adds additional columns to the left or right side of the specified matrix, returning a new matrix.
        /// </summary>
        /// <param name="inputMatrix">The matrix to add columns to.</param>
        /// <param name="numberOfColumns">The number of columns to add.</param>
        /// <param name="leftSide">Whether the columns should be added to the left side of the matrix (true = left, false = right).</param>
        /// <param name="defaultValue">The default value for the elements in the new columns.</param>
        /// <returns>The matrix with the additional columns.</returns>
        public Matrix AddColumns(Matrix inputMatrix, Int32 numberOfColumns, Boolean leftSide, Double defaultValue)
        {
            if (numberOfColumns < 1)
            {
                throw new ArgumentException("Parameter 'numberOfColumns' must be greater than or equal to 1.", "numberOfColumns");
            }

            Matrix returnMatrix = new Matrix(inputMatrix.MDimension, inputMatrix.NDimension + numberOfColumns);
            for (int i = 1; i <= returnMatrix.MDimension; i++)
            {
                for (int j = 1; j <= returnMatrix.NDimension; j++)
                {
                    if (leftSide == true)
                    {
                        if (j <= numberOfColumns)
                        {
                            returnMatrix.SetElement(i, j, defaultValue);
                        }
                        else
                        {
                            returnMatrix.SetElement(i, j, inputMatrix.GetElement(i, j - numberOfColumns));
                        }
                    }
                    else
                    {
                        if (j <= inputMatrix.NDimension)
                        {
                            returnMatrix.SetElement(i, j, inputMatrix.GetElement(i, j));
                        }
                        else
                        {
                            returnMatrix.SetElement(i, j, defaultValue);
                        }
                    }
                }
            }

            return returnMatrix;
        }

        /// <summary>
        /// Joins two matrices horizontally, returning a new matrix.
        /// </summary>
        /// <param name="leftMatrix">The matrix to join on the left side.</param>
        /// <param name="rightMatrix">The matrix to join on the right side.</param>
        /// <returns>The joined matrices.</returns>
        public Matrix JoinHorizontally(Matrix leftMatrix, Matrix rightMatrix)
        {
            if (leftMatrix.MDimension != rightMatrix.MDimension)
            {
                throw new ArgumentException("The 'm' dimension of parameter 'rightMatrix' '" + rightMatrix.MDimension + "' does not match the 'm' dimension of parameter 'leftMatrix' '" + leftMatrix.MDimension + "'.", "rightMatrix");
            }

            Matrix returnMatrix = new Matrix(leftMatrix.MDimension, leftMatrix.NDimension + rightMatrix.NDimension);
            for (Int32 i = 1; i <= leftMatrix.MDimension; i++)
            {
                for (Int32 j = 1; j <= leftMatrix.NDimension + rightMatrix.NDimension; j++)
                {
                    if (j <= leftMatrix.NDimension)
                    {
                        returnMatrix.SetElement(i, j, leftMatrix.GetElement(i, j));
                    }
                    else
                    {
                        returnMatrix.SetElement(i, j, rightMatrix.GetElement(i, j - leftMatrix.NDimension));
                    }
                }
            }

            return returnMatrix;
        }

        /// <summary>
        /// Makes a copy of the inputted matrix.
        /// </summary>
        /// <param name="inputMatrix">The matrix to copy.</param>
        /// <returns>The copy of the matrix.</returns>
        public Matrix Copy(Matrix inputMatrix)
        {
            Matrix returnMatrix = new Matrix(inputMatrix.MDimension, inputMatrix.NDimension);

            for (int i = 1; i <= inputMatrix.NDimension; i++)
            {
                for (int j = 1; j <= inputMatrix.MDimension; j++)
                {
                    returnMatrix.SetElement(j, i, inputMatrix.GetElement(j, i));
                }
            }

            return returnMatrix;
        }

        /// <summary>
        /// Writes the contents of the inputted matrix to the console.
        /// </summary>
        /// <param name="inputMatrix">The matrix to write to the console.</param>
        /// <remarks>The method could be made more generic (e.g. write to a stream), but found myself writing and copying this many times for test code, so thought it better to put in here.  ** NOTE ** this method may be removed from this class at some point, so do not create permanent dependencies on it.</remarks>
        public void WriteToConsole(Matrix inputMatrix)
        {
            for (Int32 i = 1; i <= inputMatrix.MDimension; i++)
            {
                for (Int32 j = 1; j <= inputMatrix.NDimension; j++)
                {
                    Console.Write(inputMatrix.GetElement(i, j) + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
