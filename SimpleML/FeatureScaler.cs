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
    /// Applies feature scaling between -1 and 1 to an array, or column-wise to a matrix.
    /// </summary>
    public class FeatureScaler
    {
        /// <summary>
        /// Initialises a new instance of the SimpleML.FeatureScaler class.
        /// </summary>
        public FeatureScaler()
        {
        }

        /// <summary>
        /// Scales the feature values contained in the specified array, using the specified scaling parameters. 
        /// </summary>
        /// <param name="features">The feature values to scale.</param>
        /// <param name="featureScalingParameters">The parameters to use to scale the features.</param>
        /// <returns>The scaled feature values.</returns>
        public Double[] Scale(Double[] features, FeatureScalingParameters featureScalingParameters)
        {
            Double[] scaledFeatures = ImplementScale(features, ref featureScalingParameters, false);
            return scaledFeatures;
        }

        /// <summary>
        /// Scales the feature values contained in the specified array.
        /// </summary>
        /// <param name="features">The feature values to scale.</param>
        /// <param name="featureScalingParameters">The parameters used to scale the features.</param>
        /// <returns>The scaled feature values.</returns>
        public Double[] Scale(Double[] features, out FeatureScalingParameters featureScalingParameters)
        {
            featureScalingParameters = new FeatureScalingParameters(0, 0);
            Double[] scaledFeatures = ImplementScale(features, ref featureScalingParameters, true);
            return scaledFeatures;
        }

        /// <summary>
        /// Scales the feature values contained in the specified matrix, treating each column of the matrix as a separate set of features, and using the specified scaling parameters.
        /// </summary>
        /// <param name="features">The matrix containing the columns to apply feature scaling to.</param>
        /// <param name="featureScalingParameters">The parameters to use to scale the features, one item for each column in the matrix.</param>
        /// <returns>The scaled feature values.</returns>
        public Matrix Scale(Matrix features, List<FeatureScalingParameters> featureScalingParameters)
        {
            if (features.NDimension != featureScalingParameters.Count)
            {
                throw new ArgumentException("The number of feature scaling parameters provided (" + featureScalingParameters.Count + "), does not match the 'n' dimension of the inputted matrix of features (" + features.NDimension + ").", "featureScalingParameters");
            }

            Matrix scaledFeatures = ImplementScale(features, ref featureScalingParameters, false);
            return scaledFeatures;
        }

        /// <summary>
        /// Scales the feature values contained in the specified matrix, treating each column of the matrix as a separate set of features.
        /// </summary>
        /// <param name="features">The matrix containing the columns to apply feature scaling to.</param>
        /// <param name="featureScalingParameters">A collection of the parameters used to scale the features, with one item for each column in the matrix.</param>
        /// <returns>The scaled feature values.</returns>
        public Matrix Scale(Matrix features, out List<FeatureScalingParameters> featureScalingParameters)
        {
            featureScalingParameters = new List<FeatureScalingParameters>();
            Matrix scaledFeatures = ImplementScale(features, ref featureScalingParameters, true);
            return scaledFeatures;
        }

        /// <summary>
        /// Rescales the specified scaled feature value (effectively the reverse process of the Scale() method).
        /// </summary>
        /// <param name="scaledFeature">The value to rescale.</param>
        /// <param name="featureScalingParameters">The parameters to use to rescale.</param>
        /// <returns>The rescaled feature value.</returns>
        public Double Rescale(Double scaledFeature, FeatureScalingParameters featureScalingParameters)
        {
            return scaledFeature * featureScalingParameters.Span + featureScalingParameters.Mean;
        }

        /// <summary>
        /// Rescales the scaled feature values contained in the specified array (effectively the reverse process of the Scale() method).
        /// </summary>
        /// <param name="scaledFeatures">The values to rescale.</param>
        /// <param name="featureScalingParameters">The parameters to use to rescale.</param>
        /// <returns>The rescaled feature values.</returns>
        public Double[] Rescale(Double[] scaledFeatures, FeatureScalingParameters featureScalingParameters)
        {
            if (scaledFeatures.Length == 0)
            {
                throw new ArgumentException("Parameter 'scaledFeatures' is an empty array.", "scaledFeatures");
            }

            Double[] returnFeatures = new Double[scaledFeatures.Length];

            for (int i = 0; i < scaledFeatures.Length; i++)
            {
                returnFeatures[i] = Rescale(scaledFeatures[i], featureScalingParameters);
            }

            return returnFeatures;
        }

        /// <summary>
        /// Rescales the scaled feature values contained in the specified matrix (effectively the reverse process of the Scale() method), treating each column of the matrix as a separate set of features.
        /// </summary>
        /// <param name="scaledFeatures">The matrix containing columns of values to rescale.</param>
        /// <param name="featureScalingParameters">A collection of the parameters to use to rescale, with one item for each column in the matrix.</param>
        /// <returns>The rescaled feature values.</returns>
        public Matrix Rescale(Matrix scaledFeatures, List<FeatureScalingParameters> featureScalingParameters)
        {
            // TODO: This method could be made a lot more efficient by refactoring the Matrix class to store underlying values column-wise instead of row-wise, and by exposing a method which allowed traversing a column of the matrix and updating values (potentially in a similar way to how the ApplyElementWiseOperation() method is implemented... by allowing passing of a Func)
            //   Accessing the Metric via GetElement() and SetElement() incurs additional penalty of dimension parameter checking on every access

            if (scaledFeatures.NDimension != featureScalingParameters.Count)
            {
                throw new ArgumentException("Size of parameter 'featureScalingParameters' '" + featureScalingParameters.Count + "' does not match the size of the 'n' dimension of the matrix '" + scaledFeatures.NDimension + "'.", "featureScalingParameters");
            }

            Matrix returnFeatures = new Matrix(scaledFeatures.MDimension, scaledFeatures.NDimension);

            for (int i = 1; i <= scaledFeatures.NDimension; i++)
            {
                for (int j = 1; j <= scaledFeatures.MDimension; j++)
                {
                    returnFeatures.SetElement(j, i, Rescale(scaledFeatures.GetElement(j, i), featureScalingParameters[i - 1]));
                }
            }

            return returnFeatures;
        }

        #region Private Methods

        /// <summary>
        /// Internal implementation of the Scale() method for an array of Doubles.
        /// </summary>
        /// <param name="features">The feature values to scale.</param>
        /// <param name="featureScalingParameters">The feature scaling parameters (used in the case 'calculateScalingParameters' is set false).</param>
        /// <param name="calculateScalingParameters">Whether to calculate the feature scaling parameteres (and return in parameter 'featureScalingParameters'), or use the feature scaling parameters provided in parameter 'featureScalingParameters' (true = calculate).</param>
        /// <returns>The scaled feature values.</returns>
        private Double[] ImplementScale(Double[] features, ref FeatureScalingParameters featureScalingParameters, Boolean calculateScalingParameters)
        {
            if (features.Length == 0)
            {
                throw new ArgumentException("Parameter 'features' is an empty array.", "features");
            }

            if (calculateScalingParameters == true)
            {
                featureScalingParameters = CalculateScalingParameters(features);
            }

            // Create the scaled array
            Double[] returnFeatures = new Double[features.Length];

            for (int i = 0; i < returnFeatures.Length; i++)
            {
                returnFeatures[i] = (features[i] - featureScalingParameters.Mean) / featureScalingParameters.Span;
            }

            return returnFeatures;
        }

        /// <summary>
        /// Internal implementation of the Scale() method for a matrix.
        /// </summary>
        /// <param name="features">The matrix containing the columns to apply feature scaling to.</param>
        /// <param name="featureScalingParameters">A collection of the parameters to use to scale the features, with one item for each column in the matrix (used in the case 'calculateScalingParameters' is set false).</param>
        /// <param name="calculateScalingParameters">Whether to calculate the feature scaling parameteres (and return in parameter 'featureScalingParameters'), or use the feature scaling parameters provided in parameter 'featureScalingParameters' (true = calculate).</param>
        /// <returns>The scaled feature values.</returns>
        private Matrix ImplementScale(Matrix features, ref List<FeatureScalingParameters> featureScalingParameters, Boolean calculateScalingParameters)
        {
            // TODO: This method could be made a lot more efficient by refactoring the Matrix class to store underlying values column-wise instead of row-wise, and by exposing a method which allowed traversing a column of the matrix and updating values (potentially in a similar way to how the ApplyElementWiseOperation() method is implemented... by allowing passing of a Func)
            //   Accessing the Metric via GetElement() and SetElement() incurs additional penalty of dimension parameter checking on every access

            Matrix returnFeatures = new Matrix(features.MDimension, features.NDimension);

            if (calculateScalingParameters == true)
            {
                featureScalingParameters = new List<FeatureScalingParameters>();
            }
            
            for (int i = 1; i <= features.NDimension; i++)
            {
                FeatureScalingParameters currentColumnScalingParameters;
                if (calculateScalingParameters == true)
                {
                    Double[] currentFeatures = new Double[features.MDimension];
                    for (Int32 j = 1; j <= features.MDimension; j++)
                    {
                        currentFeatures[j - 1] = features.GetElement(j, i);
                    }
                    currentColumnScalingParameters = CalculateScalingParameters(currentFeatures);
                    featureScalingParameters.Add(currentColumnScalingParameters);
                }
                else
                {
                    currentColumnScalingParameters = featureScalingParameters[i - 1];
                }

                for (int j = 1; j <= features.MDimension; j++)
                {
                    returnFeatures.SetElement(j, i, (features.GetElement(j, i) - currentColumnScalingParameters.Mean) / currentColumnScalingParameters.Span);
                }
            }

            return returnFeatures;
        }

        /// <summary>
        /// Calculates the scaling parameters for an array of features.
        /// </summary>
        /// <param name="features">The features to calculate the scaling parameters for.</param>
        /// <returns>The scaling parameters.</returns>
        private FeatureScalingParameters CalculateScalingParameters(Double[] features)
        {
            Double min;
            Double max;
            Double mean = 0;

            min = features[0];
            max = features[0];

            // Find the maximum, minimum, and average values and setup the scaling parameters
            foreach (Double currentItem in features)
            {
                if (currentItem < min)
                {
                    min = currentItem;
                }
                if (currentItem > max)
                {
                    max = currentItem;
                }
                mean += currentItem;
            }
            mean = mean / features.Length;

            return new FeatureScalingParameters(mean, max - min);
        }

        #endregion
    }
}
