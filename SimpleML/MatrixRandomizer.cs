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
    /// Randomly reorders the rows in a Matrix.
    /// </summary>
    public class MatrixRandomizer
    {
        // TODO: Possibly need to create unit tests for this class

        /// <summary>
        /// Randomly reorders the rows in the specified Matrix, returning the reordered rows in a new Matrix.
        /// </summary>
        /// <param name="inputMatrix">The Matrix in which to randomize the rows.</param>
        /// <param name="randomSeed">The seed for the random number generator.</param>
        /// <returns>A new Matrix with the same rows in random order.</returns>
        public Matrix Randomize(Matrix inputMatrix, Int32 randomSeed)
        {
            Matrix returnMatrix = new Matrix(inputMatrix.MDimension, inputMatrix.NDimension);
            List<Int32> matrixRows = new List<Int32>();
            Random randomGenerator = new Random(randomSeed);

            // Populate the list with an entry for each row in the source matrix
            for (int i = 1; i <= inputMatrix.MDimension; i++)
            {
                matrixRows.Add(i);
            }

            // Randomize the rows into the return matrix
            for (int i = 1; i <= inputMatrix.MDimension; i++)
            {
                Int32 matrixRowsIndex = randomGenerator.Next(1, matrixRows.Count + 1);
                Int32 rowToCopy = matrixRows[matrixRowsIndex - 1];

                for (int j = 1; j <= inputMatrix.NDimension; j++)
                {
                    returnMatrix.SetElement(i, j, inputMatrix.GetElement(rowToCopy, j));
                }

                matrixRows.Remove(rowToCopy);
            }

            return returnMatrix;
        }
    }
}
