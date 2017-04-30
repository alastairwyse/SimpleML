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
using ApplicationLogging;
using MathematicsModularFramework;
using SimpleML;
using SimpleML.Containers;

namespace SimpleML.Samples.Modules
{
    /// <summary>
    /// An MMF module which calculates the linear regression cost for a specified set of theta parameters.
    /// </summary>
    public class LinearRegressionCostSeriesCalculator : ModuleBase
    {
        // TODO: At some point, should probably rename this whole class to just 'LinearRegressionCostCalculator'.
        //   However will need to rename module in XML workflow templates, + change documentation / website, etc...

        private const String trainingSeriesDataInputSlotName = "TrainingSeriesData";
        private const String trainingSeriesResultsInputSlotName = "TrainingSeriesResults";
        private const String thetaParametersInputSlotName = "ThetaParameters";
        private const String costOutputSlotName = "Cost";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.LinearRegressionCostSeriesCalculator class.
        /// </summary>
        public LinearRegressionCostSeriesCalculator()
            : base()
        {
            Description = "Adds a bias term to the data series, and then calculates the linear regression cost for the specified set of theta parameters";
            AddInputSlot(trainingSeriesDataInputSlotName, "The training data series, stored column-wise in a matrix", typeof(Matrix));
            AddInputSlot(trainingSeriesResultsInputSlotName, "The training data results, stored as a single column matrix", typeof(Matrix));
            AddInputSlot(thetaParametersInputSlotName, "The theta parameter values to use in the cost calculation", typeof(Matrix));
            AddOutputSlot(costOutputSlotName, "The total cost", typeof(Double));
        }

        protected override void ImplementProcess()
        {
            Matrix trainingSeriesData = (Matrix)GetInputSlot(trainingSeriesDataInputSlotName).DataValue;
            Matrix trainingSeriesResults = (Matrix)GetInputSlot(trainingSeriesResultsInputSlotName).DataValue;
            Matrix thetaParameters = (Matrix)GetInputSlot(thetaParametersInputSlotName).DataValue;

            if (trainingSeriesData.NDimension != (thetaParameters.MDimension - 1))
            {
                String message = "The 'm' dimension of parameter '" + thetaParametersInputSlotName + "' must be 1 greater than the 'n' dimension of parameter '" + trainingSeriesDataInputSlotName + "'.";
                ArgumentException e = new ArgumentException(message, thetaParametersInputSlotName);
                logger.Log(this, LogLevel.Critical, message, e);
                throw e;
            }

            try
            {
                MatrixUtilities matrixUtilities = new MatrixUtilities();
                // Add a column of 1's to the training data
                Matrix biasedTrainingDataSeries = matrixUtilities.AddColumns(trainingSeriesData, 1, true, 1);
                MultivariateLinearRegressionCostFunctionCalculator costFunctionCalculator = new MultivariateLinearRegressionCostFunctionCalculator();
                Double cost = costFunctionCalculator.Calculate(biasedTrainingDataSeries, trainingSeriesResults, thetaParameters);
                GetOutputSlot(costOutputSlotName).DataValue = cost;
            }
            catch (Exception e)
            {
                logger.Log(this, LogLevel.Critical, "Error occurred whilst calculating linear regression cost.", e);
                throw;
            }
        }
    }
}
