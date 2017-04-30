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
    /// An MMF module which minimizes a continuous differentiable multivariate function.
    /// </summary>
    public class FunctionMinimizer : ModuleBase
    {
        private const String dataSeriesInputSlotName = "DataSeries";
        private const String dataResultsInputSlotName = "DataResults";
        private const String initialThetaParametersInputSlotName = "InitialThetaParameters";
        private const String regularizationParameterInputSlotName = "RegularizationParameter";
        private const String costFunctionCalculatorInputSlotName = "CostFunctionCalculator";
        private const String maxIterationsInputSlotName = "MaxIterations";
        private const String optimizedThetaParametersOutputSlotName = "OptimizedThetaParameters";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.FunctionMinimizer class.
        /// </summary>
        public FunctionMinimizer()
            : base()
        {
            Description = "Minimizes a continuous differentiable multivariate function";
            AddInputSlot(dataSeriesInputSlotName, "The data series, stored column-wise in a matrix", typeof(Matrix));
            AddInputSlot(dataResultsInputSlotName, "The data results, stored as a single column matrix", typeof(Matrix));
            AddInputSlot(initialThetaParametersInputSlotName, "The initial theta parameter values, stored as a single column matrix", typeof(Matrix));
            AddInputSlot(regularizationParameterInputSlotName, "The regularization parameter (lambda)", typeof(Double));
            AddInputSlot(costFunctionCalculatorInputSlotName, "The class to use to evaluate the cost during minimization", typeof(ICostFunctionGradientCalculator));
            AddInputSlot(maxIterationsInputSlotName, "The maximum number of iterations during minimization", typeof(Int32));
            AddOutputSlot(optimizedThetaParametersOutputSlotName, "A single column matrix containing the optimized theta values", typeof(Matrix));
        }

        protected override void ImplementProcess()
        {
            Matrix dataSeries = (Matrix)GetInputSlot(dataSeriesInputSlotName).DataValue;
            Matrix dataResults = (Matrix)GetInputSlot(dataResultsInputSlotName).DataValue;
            Matrix initialThetaParameters = (Matrix)GetInputSlot(initialThetaParametersInputSlotName).DataValue;
            Double regularizationParameter = (Double)GetInputSlot(regularizationParameterInputSlotName).DataValue;
            ICostFunctionGradientCalculator costFunctionCalculator = (ICostFunctionGradientCalculator)GetInputSlot(costFunctionCalculatorInputSlotName).DataValue;
            Int32 maxIterations = (Int32)GetInputSlot(maxIterationsInputSlotName).DataValue;

            try
            {
                SimpleML.FunctionMinimizer functionMinimizer = new SimpleML.FunctionMinimizer(logger, metricLogger);
                Matrix optimizedThetaParameters = functionMinimizer.Minimize(dataSeries, dataResults, initialThetaParameters, regularizationParameter, costFunctionCalculator, maxIterations);
                GetOutputSlot(optimizedThetaParametersOutputSlotName).DataValue = optimizedThetaParameters;
            }
            catch (Exception e)
            {
                logger.Log(this, LogLevel.Critical, "Error occurred whilst running function minimization.", e);
                throw;
            }
        }
    }
}
