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
    /// An MMF module which runs gradient descent optimization for linear regression.
    /// </summary>
    public class LinearRegressionGradientDescentOptimizer : ModuleBase
    {
        private const String initialThetaParametersInputSlotName = "InitialThetaParameters";
        private const String trainingSeriesDataInputSlotName = "TrainingSeriesData";
        private const String trainingSeriesResultsInputSlotName = "TrainingSeriesResults";
        private const String learningRateInputSlotName = "LearningRate";
        private const String maxIterationsInputSlotName = "MaxIterations";
        private const String optimizedThetaParametersOutputSlotName = "OptimizedThetaParameters";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.LinearRegressionGradientDescentOptimizer class.
        /// </summary>
        public LinearRegressionGradientDescentOptimizer()
            : base()
        {
            Description = "Adds a bias term to the data series, and then runs gradient descent optimization for linear regression";
            AddInputSlot(initialThetaParametersInputSlotName, "The initial theta parameter values to use in the optimization, stored column-wise in a matrix", typeof(Matrix));
            AddInputSlot(trainingSeriesDataInputSlotName, "The training data series, stored column-wise in a matrix", typeof(Matrix));
            AddInputSlot(trainingSeriesResultsInputSlotName, "The training data results, stored as a single column matrix", typeof(Matrix));
            AddInputSlot(learningRateInputSlotName, "The learning rate (step size) to use in the optimization", typeof(Double));
            AddInputSlot(maxIterationsInputSlotName, "Maximum number of iterations to run the gradient descent for", typeof(Int32));
            AddOutputSlot(optimizedThetaParametersOutputSlotName, "A single column matrix containing the optimized theta parameter values", typeof(Matrix));
        }

        protected override void ImplementProcess()
        {
            Matrix initialThetaParameters = (Matrix)GetInputSlot(initialThetaParametersInputSlotName).DataValue;
            Matrix trainingSeriesData = (Matrix)GetInputSlot(trainingSeriesDataInputSlotName).DataValue;
            Matrix trainingSeriesResults = (Matrix)GetInputSlot(trainingSeriesResultsInputSlotName).DataValue;
            Double learningRate = (Double)GetInputSlot(learningRateInputSlotName).DataValue;
            Int32 maxIterations = (Int32)GetInputSlot(maxIterationsInputSlotName).DataValue;

            if (trainingSeriesData.NDimension != (initialThetaParameters.MDimension - 1))
            {
                String message = "The 'm' dimension of parameter '" + initialThetaParametersInputSlotName + "' must be 1 greater than the 'n' dimension of parameter '" + trainingSeriesDataInputSlotName + "'.";
                ArgumentException e = new ArgumentException(message, initialThetaParametersInputSlotName);
                logger.Log(this, LogLevel.Critical, message, e);
                throw e;
            }
              
            try
            {
                MatrixUtilities matrixUtilities = new MatrixUtilities();
                // Add a column of 1's to the training data
                Matrix biasedTrainingDataSeries = matrixUtilities.AddColumns(trainingSeriesData, 1, true, 1);
                GradientDescentOptimizer gradientDescentOptimizer = new GradientDescentOptimizer(logger, metricLogger, cancellationToken);
                MultivariateLinearRegressionHypothesisCalculator hypothesisCalculator = new MultivariateLinearRegressionHypothesisCalculator();
                MultivariateLinearRegressionCostFunctionCalculator costFunctionCalculator = new MultivariateLinearRegressionCostFunctionCalculator();
                Matrix optimizedThetaParameters = gradientDescentOptimizer.Optimize(initialThetaParameters, biasedTrainingDataSeries, trainingSeriesResults, hypothesisCalculator, costFunctionCalculator, learningRate, maxIterations);
                GetOutputSlot(optimizedThetaParametersOutputSlotName).DataValue = optimizedThetaParameters;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                logger.Log(this, LogLevel.Critical, "Error occurred whilst running gradient descent for linear regression.", e);
                throw;
            }
        }
    }
}
