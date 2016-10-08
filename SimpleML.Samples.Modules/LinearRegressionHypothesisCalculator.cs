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
    /// An MMF module which adds a bias term to a series of data, and then applies a multi-variate linear regression hypothesis (effectively predicts the result of linear regression hypothesis for a series of data points).
    /// </summary>
    public class LinearRegressionHypothesisCalculator : ModuleBase
    {
        private const String dataSeriesInputSlotName = "DataSeries";
        private const String thetaParametersInputSlotName = "ThetaParameters";
        private const String resultsOutputSlotName = "Results";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.LinearRegressionHypothesisCalculator class.
        /// </summary>
        public LinearRegressionHypothesisCalculator()
            : base()
        {
            Description = "Adds a bias term to a series of data, and then applies a multi-variate linear regression hypothesis (effectively predicts the result of linear regression hypothesis for a series of data points)";
            AddInputSlot(dataSeriesInputSlotName, "The data series to apply the hypothesis to, stored column-wise in a matrix", typeof(Matrix));
            AddInputSlot(thetaParametersInputSlotName, "The parameter values of the hypothesis, stored column-wise in a matrix", typeof(Matrix));
            AddOutputSlot(resultsOutputSlotName, "The result values, stored column-wise in a matrix", typeof(Matrix));
        }

        protected override void ImplementProcess()
        {
            Matrix dataSeries = (Matrix)GetInputSlot(dataSeriesInputSlotName).DataValue;
            Matrix thetaParameters = (Matrix)GetInputSlot(thetaParametersInputSlotName).DataValue;

            if (dataSeries.NDimension != (thetaParameters.MDimension - 1))
            {
                throw new ArgumentException("The 'm' dimension of parameter '" + thetaParametersInputSlotName + "' must be 1 greater than the 'n' dimension of parameter '" + dataSeriesInputSlotName + "'.", thetaParametersInputSlotName);
            }

            try
            {
                MatrixUtilities matrixUtilities = new MatrixUtilities();
                MultivariateLinearRegressionHypothesisCalculator hypothesisCalculator = new MultivariateLinearRegressionHypothesisCalculator();
                Matrix biasedDataSeries = matrixUtilities.AddColumns(dataSeries, 1, true, 1.0);
                Matrix results = hypothesisCalculator.Calculate(biasedDataSeries, thetaParameters);
                GetOutputSlot(resultsOutputSlotName).DataValue = results;
            }
            catch (Exception e)
            {
                logger.Log(this, LogLevel.Critical, "Error occurred whilst calculating linear regression hypothesis.", e);
                throw;
            }
            logger.Log(this, LogLevel.Information, "Applied multi-variate linear regression hypothesis to matrix data series of " + dataSeries.MDimension + " items.");
        }
    }
}
