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
    /// Generates polynomial features of data series in a matrix.
    /// </summary>
    public class PolynomialFeatureGenerator
    {
        /// <summary>Utililty class to modify matrices.</summary>
        MatrixUtilities matrixUtilities;

        /// <summary>
        /// Initialises a new instance of the SimpleML.PolynomialFeatureGenerator class.
        /// </summary>
        public PolynomialFeatureGenerator()
        {
            matrixUtilities = new MatrixUtilities();
        }

        /// <summary>
        /// Generates polynomial features of the specified degree for the data series in the inputted matrix.
        /// </summary>
        /// <param name="data">A matrix containing the data series (stored column-wise) for which to generate the polynomial features.</param>
        /// <param name="polynomialDegree">The degree of features to generate.</param>
        /// <returns>A matrix containing the original data series and the generated polynomial features (stored column-wise).</returns>
        public Matrix GenerateFeatures(Matrix data, Int32 polynomialDegree)
        {
            if (polynomialDegree < 2)
            {
                throw new ArgumentException("Parameter 'polynomialDegree' must be greater than 1.", "polynomialDegree");
            }

            if (polynomialDegree > 2)
            {
                throw new NotSupportedException("Polynomial degrees greater than 2 are currently not supported.");
            }

            // Add a column of 1's to the inputted matrix
            Matrix biasedData = matrixUtilities.AddColumns(data, 1, true, 1);

            // Generate the horizontal size of the returned matrix
            Func<Int32, Int32> factorialIteration = null;
            factorialIteration = (Int32 factor) =>
            {   
                if ((factor) > 1) 
                {
                    return factor + factorialIteration(factor - 1);
                }
                else
                {
                    return factor;
                }
            };
            Matrix biasedReturnMatrix = new Matrix(biasedData.MDimension, factorialIteration.Invoke(biasedData.NDimension));

            // Generate the polynomial features
            Action<Int32> featureGeneratorIterator = null;
            for (int i = 1; i <= biasedData.MDimension; i++)
            {
                Int32 returnMatrixIndex = 1;
                featureGeneratorIterator = (Int32 startIndex) =>
                {
                    if (startIndex <= biasedData.NDimension)
                    {
                        for (int j = startIndex; j <= biasedData.NDimension; j++)
                        {
                            Double elementData = biasedData.GetElement(i, startIndex) * biasedData.GetElement(i, j);
                            biasedReturnMatrix.SetElement(i, returnMatrixIndex, elementData);
                            returnMatrixIndex++;
                        }
                        featureGeneratorIterator.Invoke(startIndex + 1);
                    }
                };
                featureGeneratorIterator.Invoke(1);
            }

            // Remove the bias column
            Matrix returnMatrix = new Matrix(biasedReturnMatrix.MDimension, biasedReturnMatrix.NDimension - 1);
            for (Int32 i = 1; i <= returnMatrix.MDimension; i++)
            {
                for (Int32 j = 1; j <= returnMatrix.NDimension; j++)
                {
                    returnMatrix.SetElement(i, j, biasedReturnMatrix.GetElement(i, j + 1));
                }
            }

            return returnMatrix;
        }
    }
}
