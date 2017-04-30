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
    /// Calculates the cost for a series of training data using specified theta parameters, for multivariate linear regression.
    /// </summary>
    public class MultivariateLinearRegressionCostFunctionCalculator : ICostFunctionCalculator
    {
        /// <summary>Utililty class to modify matrices.</summary>
        MatrixUtilities matrixUtilities;

        /// <summary>
        /// Initialises a new instance of the SimpleML.MultivariateLinearRegressionCostFunctionCalculator class.
        /// </summary>
        public MultivariateLinearRegressionCostFunctionCalculator()
        {
            matrixUtilities = new MatrixUtilities();
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:SimpleML.ICostFunctionCalculator.Calculate(SimpleML.Containers.Matrix,SimpleML.Containers.Matrix,SimpleML.Containers.Matrix)"]/*'/>
        public Double Calculate(Matrix dataSeries, Matrix dataResults, Matrix thetaParameters)
        {
            TrainingDataProcessorArgumentValidator argumentValidator = new TrainingDataProcessorArgumentValidator();

            argumentValidator.ValidateArguments(dataSeries, dataResults, thetaParameters, "dataSeries", "dataResults", "thetaParameters");

            MultivariateLinearRegressionHypothesisCalculator hypothesisCalculator = new MultivariateLinearRegressionHypothesisCalculator();
            Matrix hypothesisValues = hypothesisCalculator.Calculate(dataSeries, thetaParameters);
            Matrix resultDifferences = hypothesisValues - dataResults;
            Func<Double, Double> squareFunction = new Func<Double, Double>( (x) => { return Math.Pow(x, 2); } );
            resultDifferences.ApplyElementWiseOperation(squareFunction);
            Double totalCost = (resultDifferences.SumHorizontally()).GetElement(1, 1) / dataSeries.MDimension / 2;

            return totalCost;
        }
    }
}
