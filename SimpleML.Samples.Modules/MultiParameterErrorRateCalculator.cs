/*
 * Copyright 2017 Alastair Wyse (http://www.oraclepermissiongenerator.net/simpleml/)
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
    /// An MMF module which finds the logistic regression error rate based on multiple sets of theta parameters.
    /// </summary>
    public class MultiParameterErrorRateCalculator : ModuleBase
    {
        private const String dataSeriesInputSlotName = "DataSeries";
        private const String dataResultsInputSlotName = "DataResults";
        private const String thetaParameterSetInputSlotName = "ThetaParameterSet";
        private const String errorRateSetOutputSlotName = "ErrorRateSet";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.MultiParameterErrorRateCalculator class.
        /// </summary>
        public MultiParameterErrorRateCalculator()
            : base()
        {
            Description = "Finds the logistic regression error rate based on multiple sets of theta parameters";
            AddInputSlot(dataSeriesInputSlotName, "The data series, stored column-wise in a matrix", typeof(Matrix));
            AddInputSlot(dataResultsInputSlotName, "The data results, stored as a single column matrix", typeof(Matrix));
            AddInputSlot(thetaParameterSetInputSlotName, "A set of single column matrices containing the theta values for each error rate calculation", typeof(List<Matrix>));
            AddOutputSlot(errorRateSetOutputSlotName, "A set of error rates", typeof(List<Double>));
        }

        protected override void ImplementProcess()
        {
            Matrix dataSeries = (Matrix)GetInputSlot(dataSeriesInputSlotName).DataValue;
            Matrix dataResults = (Matrix)GetInputSlot(dataResultsInputSlotName).DataValue;
            List<Matrix> thetaParameterSet = (List<Matrix>)GetInputSlot(thetaParameterSetInputSlotName).DataValue;

            List<Double> errorRateSet = new List<Double>();
            LogisticRegressionErrorRateCalculator errorRateCalculator = new LogisticRegressionErrorRateCalculator();
            try
            {
                foreach (Matrix currentThetaParameters in thetaParameterSet)
                {
                    Double currentErrorRate = errorRateCalculator.Calculate(dataSeries, dataResults, currentThetaParameters);
                    errorRateSet.Add(currentErrorRate);
                }
                GetOutputSlot(errorRateSetOutputSlotName).DataValue = errorRateSet;
            }
            catch (Exception e)
            {
                logger.Log(this, LogLevel.Critical, "Error occurred whilst calculating logistic regression error rate.", e);
                throw;
            }
            logger.Log(this, LogLevel.Information, "Calculated logistic regression error rate for " + errorRateSet.Count + " sets of theta parameters.");
        }
    }
}
