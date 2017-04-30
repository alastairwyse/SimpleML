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
    /// An MMF module which executes multiple iterations of function minimization using varying parameters.
    /// </summary>
    public class MultiParameterTrainer : ModuleBase
    {
        private const String dataSeriesInputSlotName = "DataSeries";
        private const String dataResultsInputSlotName = "DataResults";
        private const String initialThetaParametersInputSlotName = "InitialThetaParameters";
        private const String regularizationParameterSetInputSlotName = "RegularizationParameterSet";
        private const String maxIterationParameterSetInputSlotName = "MaxIterationParameterSet";
        private const String costFunctionCalculatorInputSlotName = "CostFunctionCalculator";
        private const String optimizedThetaParameterSetOutputSlotName = "OptimizedThetaParameterSet";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.MultiParameterTrainer class.
        /// </summary>
        public MultiParameterTrainer()
            : base()
        {
            Description = "Executes multiple iterations of function minimization using varying parameters";
            AddInputSlot(dataSeriesInputSlotName, "The data series, stored column-wise in a matrix", typeof(Matrix));
            AddInputSlot(dataResultsInputSlotName, "The data results, stored as a single column matrix", typeof(Matrix));
            AddInputSlot(initialThetaParametersInputSlotName, "The initial theta parameter values, stored as a single column matrix", typeof(Matrix));
            AddInputSlot(regularizationParameterSetInputSlotName, "The set of regularization parameters (lambda)", typeof(List<Double>));
            AddInputSlot(maxIterationParameterSetInputSlotName, "The set of maximum iteration parameters (the number of iterations during each minimization iterations)", typeof(List<Int32>));
            AddInputSlot(costFunctionCalculatorInputSlotName, "The class to use to evaluate the cost during minimization", typeof(ICostFunctionGradientCalculator));
            AddOutputSlot(optimizedThetaParameterSetOutputSlotName, "A set of single column matrices containing the optimized theta values for each minimization iterations", typeof(List<Matrix>));
        }

        protected override void ImplementProcess()
        {
            Matrix dataSeries = (Matrix)GetInputSlot(dataSeriesInputSlotName).DataValue;
            Matrix dataResults = (Matrix)GetInputSlot(dataResultsInputSlotName).DataValue;
            Matrix initialThetaParameters = (Matrix)GetInputSlot(initialThetaParametersInputSlotName).DataValue;
            List<Double> regularizationParameterSet = (List<Double>)GetInputSlot(regularizationParameterSetInputSlotName).DataValue;
            List<Int32> maxIterationParameterSet = (List<Int32>)GetInputSlot(maxIterationParameterSetInputSlotName).DataValue;
            ICostFunctionGradientCalculator costFunctionCalculator = (ICostFunctionGradientCalculator)GetInputSlot(costFunctionCalculatorInputSlotName).DataValue;

            if (regularizationParameterSet.Count != maxIterationParameterSet.Count)
            {
                String message = "Parameters '" + regularizationParameterSetInputSlotName + "' and '" + maxIterationParameterSetInputSlotName + "' must be lists of equal size.";
                ArgumentException e = new ArgumentException(message, regularizationParameterSetInputSlotName);
                logger.Log(this, LogLevel.Critical, message, e);
                throw e;
            }

            List<Matrix> optimizedThetaParameterSet = new List<Matrix>();
            SimpleML.FunctionMinimizer functionMinimizer = new SimpleML.FunctionMinimizer(logger, metricLogger);
            metricLogger.Begin(new MultiParameterTrainingTime());
            try
            {
                for (Int32 i = 0; i < regularizationParameterSet.Count; i++)
                {
                    Double regularizationParameter = regularizationParameterSet[i];
                    Int32 maxIterations = maxIterationParameterSet[i];
                    Matrix optimizedThetaParameters = functionMinimizer.Minimize(dataSeries, dataResults, initialThetaParameters, regularizationParameter, costFunctionCalculator, maxIterations);
                    optimizedThetaParameterSet.Add(optimizedThetaParameters);
                    metricLogger.Increment(new MultiParameterTrainingIteration());
                }
            }
            catch (Exception)
            {
                metricLogger.CancelBegin(new MultiParameterTrainingTime());
                throw;
            }
            GetOutputSlot(optimizedThetaParameterSetOutputSlotName).DataValue = optimizedThetaParameterSet;
            metricLogger.End(new MultiParameterTrainingTime());
        }
    }
}
