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
    /// Calculates the total cost for a series of training data using specified theta parameters, for multivariate linear regression.
    /// </summary>
    public class MultivariateLinearRegressionCostSeriesCalculator
    {
        /// <summary>Utililty class to modify matrices.</summary>
        MatrixUtilities matrixUtilities;

        /// <summary>
        /// Initialises a new instance of the SimpleML.MultivariateLinearRegressionCostSeriesCalculator class.
        /// </summary>
        public MultivariateLinearRegressionCostSeriesCalculator()
        {
            matrixUtilities = new MatrixUtilities();
        }

        /// <summary>
        /// Calculates the cost using the specified data series and theta parameters.
        /// </summary>
        /// <param name="trainingDataSeries">The training data series, stored column-wise in a matrix.</param>
        /// <param name="trainingDataResults">The training data results, stored as a single column matrix.</param>
        /// <param name="thetaParameters">The theta parameter values to use in the cost calculation.</param>
        /// <returns>The total cost.</returns>
        public Double CalculateCost(Matrix trainingDataSeries, Matrix trainingDataResults, Matrix thetaParameters)
        {
            // TODO: Most of this code is duplicated in the GradientDescentOptimizer.Optimize() method.
            //    Find a way to refactor (put into common utility class or similar)

            if (trainingDataSeries.NDimension != (thetaParameters.MDimension - 1))
            {
                throw new ArgumentException("The 'm' dimension of parameter 'thetaParameters' must be 1 greater than the 'n' dimension of parameter 'trainingDataSeries'.", "thetaParameters");
            }
            if (thetaParameters.NDimension != 1)
            {
                throw new ArgumentException("The parameter 'thetaParameters' must be a single column matrix (i.e. 'n' dimension equal to 1).", "thetaParameters");
            }
            if (trainingDataSeries.MDimension != trainingDataResults.MDimension)
            {
                throw new ArgumentException("Parameters 'trainingDataSeries' and 'trainingDataResults' have differing 'm' dimensions.", "trainingDataResults");
            }
            if (trainingDataResults.NDimension != 1)
            {
                throw new ArgumentException("The parameter 'trainingDataResults' must be a single column matrix (i.e. 'n' dimension equal to 1).", "trainingDataResults");
            }

            // Add a column of 1's to the training data
            Matrix biasedTrainingDataSeries = matrixUtilities.AddColumns(trainingDataSeries, 1, true, 1);

            MultivariateLinearRegressionHypothesisCalculator hypothesisCalculator = new MultivariateLinearRegressionHypothesisCalculator();
            Matrix hypothesisValues = hypothesisCalculator.Calculate(biasedTrainingDataSeries, thetaParameters);
            Matrix resultDifferences = hypothesisValues - trainingDataResults;
            Func<Double, Double> absoluteValue = new Func<Double, Double>( (x) => { return Math.Abs(x); } );
            resultDifferences.ApplyElementWiseOperation(absoluteValue);
            Double totalCost = (resultDifferences.SumHorizontally()).GetElement(1, 1);

            return totalCost;
        }
    }
}
