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
    /// Calculates the cost and gradients of theta parameters for logistic regression.
    /// </summary>
    public class LogisticRegressionCostFunctionCalculator : ICostFunctionGradientCalculator
    {
        /// <summary>
        /// Initialises a new instance of the SimpleML.LogisticRegressionCostFunctionCalculator class.
        /// </summary>
        public LogisticRegressionCostFunctionCalculator()
        {
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:SimpleML.ICostFunctionCalculator.Calculate(SimpleML.Containers.Matrix,SimpleML.Containers.Matrix,SimpleML.Containers.Matrix)"]/*'/>
        public Double Calculate(Matrix dataSeries, Matrix dataResults, Matrix thetaParameters)
        {
            return Calculate(dataSeries, dataResults, thetaParameters, 0).Item1;
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:SimpleML.ICostFunctionGradientCalculator.Calculate(SimpleML.Containers.Matrix,SimpleML.Containers.Matrix,SimpleML.Containers.Matrix,System.Double)"]/*'/>
        public Tuple<Double, Matrix> Calculate(Matrix dataSeries, Matrix dataResults, Matrix thetaParameters, Double regularizationParameter)
        {
            TrainingDataProcessorArgumentValidator argumentValidator = new TrainingDataProcessorArgumentValidator();

            argumentValidator.ValidateArguments(dataSeries, dataResults, thetaParameters, "dataSeries", "dataResults", "thetaParameters");
            if (regularizationParameter < 0)
            {
                throw new ArgumentOutOfRangeException("regularizationParameter", regularizationParameter, "The parameter 'regularizationParameter' must be greater than or equal to 0.");
            }

            Double cost = 0;
            Matrix gradients = new Matrix(thetaParameters.MDimension, thetaParameters.NDimension);
            LogisticRegressionHypothesisCalculator hypothesisCalculator = new LogisticRegressionHypothesisCalculator();

            for (Int32 i = 1; i <= dataSeries.MDimension; i++)
            {
                Matrix currentRow = dataSeries.GetRowSubset(i, 1);
                Matrix currentHypothesisMatrix = hypothesisCalculator.Calculate(currentRow, thetaParameters);
                Double currentHypothesis = currentHypothesisMatrix.GetElement(1, 1);
                // Calculate the cost for the current element of the data series and add to the running total
                cost = cost + ( -dataResults.GetElement(i, 1) * Math.Log(currentHypothesis) - (1 - dataResults.GetElement(i, 1)) * Math.Log(1 - currentHypothesis) );
                // Calculate the gradients
                for (Int32 j = 1; j <= thetaParameters.MDimension; j++)
                {
                    Double currentGradient = ( currentHypothesis - dataResults.GetElement(i, 1) ) * currentRow.GetElement(1, j);
                    currentGradient = currentGradient + gradients.GetElement(j, 1);
                    gradients.SetElement(j, 1, currentGradient);
                }
            }

            // TODO: Could get some efficiency gain by not doing regularization calculation when regularizationParameter is 0

            // Calculate the regularization for the cost
            Double regularization = 0;
            for (Int32 i = 2; i <= thetaParameters.MDimension; i++)
            {
                regularization = regularization + Math.Pow(thetaParameters.GetElement(i, 1), 2.0);
            }
            regularization = regularization * (regularizationParameter / (Double)(2 * dataSeries.MDimension));
            // Divide the cost by the number of elements in the data series, and add regularization
            cost = cost / (Double)dataSeries.MDimension + regularization;
            // Divide the gradients by the number of elements in the data series
            gradients.ApplyElementWiseOperation(new Func<Double, Double>((x) => { return x / (Double)dataSeries.MDimension; }));

            // Add regularization to the gradients
            for (Int32 i = 2; i <= thetaParameters.MDimension; i++)
            {
                Double currentGradient = gradients.GetElement(i, 1);
                currentGradient = currentGradient + ((regularizationParameter / (Double)dataSeries.MDimension) * thetaParameters.GetElement(i, 1));
                gradients.SetElement(i, 1, currentGradient);
            }

            return new Tuple<double, Matrix>(cost, gradients);
        }
    }
}
