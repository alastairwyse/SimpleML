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
            Description = "Calculates the linear regression cost for a specified set of theta parameters";
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

            try
            {
                MultivariateLinearRegressionCostSeriesCalculator costSeriesCalculator = new MultivariateLinearRegressionCostSeriesCalculator();
                Double cost = costSeriesCalculator.CalculateCost(trainingSeriesData, trainingSeriesResults, thetaParameters);
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
