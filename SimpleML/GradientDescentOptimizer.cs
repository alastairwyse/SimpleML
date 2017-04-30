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
using System.Threading;
using SimpleML.Containers;
using SimpleML.Metrics;
using ApplicationLogging;
using ApplicationMetrics;

namespace SimpleML
{
    /// <summary>
    /// An implementation of the gradient descent optimizer algorithm.
    /// </summary>
    public class GradientDescentOptimizer
    {
        /// <summary>The number of iterations between each check for cancellation.</summary>
        private const Int32 iterationsBetweenCancellationChecks = 1000;
        /// <summary>Utililty class to modify matrices.</summary>
        private MatrixUtilities matrixUtilities;
        /// <summary>The logger to write log events to.</summary>
        private IApplicationLogger logger;
        /// <summary>Utility class used to write log events.</summary>
        private LoggingUtilities loggingUtilities;
        /// <summary>Utility class used to write metric events.</summary>
        private MetricsUtilities metricsUtilities;
        /// <summary>Used to check for cancellation.</summary>
        private CancellationToken cancellationToken;
        /// <summary>Used to indicate whether a CancellationToken was set via the constructor.  As CancellationToken is a struct, it can't be set to null.</summary>
        private Boolean cancellationTokenIsSet;

        /// <summary>
        /// Initialises a new instance of the SimpleML.GradientDescentOptimizer class.
        /// </summary>
        public GradientDescentOptimizer()
        {
            matrixUtilities = new MatrixUtilities();
            logger = new NullApplicationLogger();
            loggingUtilities = new LoggingUtilities(logger);
            metricsUtilities = new MetricsUtilities(new NullMetricLogger());
            cancellationTokenIsSet = false;
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.GradientDescentOptimizer class.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the optimization process.</param>
        public GradientDescentOptimizer(CancellationToken cancellationToken)
            : this()
        {
            this.cancellationToken = cancellationToken;
            cancellationTokenIsSet = true;
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.GradientDescentOptimizer class.
        /// </summary>
        /// <param name="logger">The logger to write log events to.</param>
        /// <param name="cancellationToken">Token used to cancel the optimization process.</param>
        public GradientDescentOptimizer(IApplicationLogger logger, CancellationToken cancellationToken)
            : this()
        {
            this.logger = logger;
            loggingUtilities = new LoggingUtilities(logger);
            this.cancellationToken = cancellationToken;
            cancellationTokenIsSet = true;
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.GradientDescentOptimizer class.
        /// </summary>
        /// <param name="metricLogger">The metric logger to write metric and instrumentation events to.</param>
        /// <param name="cancellationToken">Token used to cancel the optimization process.</param>
        public GradientDescentOptimizer(IMetricLogger metricLogger, CancellationToken cancellationToken)
            : this()
        {
            metricsUtilities = new MetricsUtilities(metricLogger);
            this.cancellationToken = cancellationToken;
            cancellationTokenIsSet = true;
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.GradientDescentOptimizer class.
        /// </summary>
        /// <param name="logger">The logger to write log events to.</param>
        /// <param name="metricLogger">The metric logger to write metric and instrumentation events to.</param>
        /// <param name="cancellationToken">Token used to cancel the optimization process.</param>
        public GradientDescentOptimizer(IApplicationLogger logger, IMetricLogger metricLogger, CancellationToken cancellationToken)
            : this(logger, cancellationToken)
        {
            metricsUtilities = new MetricsUtilities(metricLogger);
        }

        /// <summary>
        /// Performs gradient descent optimization.
        /// </summary>
        /// <param name="initialThetaParameters">The initial theta parameter values to use in the optimization, stored column-wise in a matrix.</param>
        /// <param name="trainingDataSeries">The training data series, stored column-wise in a matrix.</param>
        /// <param name="trainingDataResults">The training data results, stored as a single column matrix.</param>
        /// <param name="hypothesisCalculator">Class used to calculate the value of the hypothesis function.</param>
        /// <param name="costFunctionCalculator">Class used to calculate the cost of the theta parameters at each iteration.</param>
        /// <param name="learningRate">The learning rate (step size) to use in the optimization.</param>
        /// <param name="maxIterations">Maximum number of iterations to run the gradient descent for.</param>
        /// <returns>A single column matrix containing the optimized theta parameter values.</returns>
        public Matrix Optimize(Matrix initialThetaParameters, Matrix trainingDataSeries, Matrix trainingDataResults, IHypothesisCalculator hypothesisCalculator, ICostFunctionCalculator costFunctionCalculator, Double learningRate, Int32 maxIterations)
        {
            TrainingDataProcessorArgumentValidator argumentValidator = new TrainingDataProcessorArgumentValidator();

            argumentValidator.ValidateArguments(trainingDataSeries, trainingDataResults, initialThetaParameters, "trainingDataSeries", "trainingDataResults", "initialThetaParameters");
            if (learningRate <= 0)
            {
                throw new ArgumentException("The parameter 'learningRate' must be positive.", "learningRate");
            }
            if (maxIterations <= 0)
            {
                throw new ArgumentException("The parameter 'maxIterations' must be a positive integer.", "maxIterations");
            }

            metricsUtilities.Begin(new GradientDescentOptimizationTime());

            Int32 totalIterationsCounter = 0;
            Matrix currentThetaParameters;
            try
            {
                currentThetaParameters = matrixUtilities.Copy(initialThetaParameters);

                loggingUtilities.Log(this, LogLevel.Information, "Running gradient descent optimization for " + initialThetaParameters.MDimension + " theta parameters, " + trainingDataSeries.MDimension + " training data points, " + maxIterations + " iterations.");

                Int32 remainingIterations = maxIterations;
                while (remainingIterations > 0)
                {
                    Int32 performedIterations = Math.Min(iterationsBetweenCancellationChecks, remainingIterations);

                    for (int i = 1; i <= performedIterations; i++)
                    {
                        // Calculate the new theta values
                        Matrix newThetaParameters = new Matrix(initialThetaParameters.MDimension, initialThetaParameters.NDimension);

                        Matrix hypothesisValues = hypothesisCalculator.Calculate(trainingDataSeries, currentThetaParameters);
                        if (hypothesisValues.MDimension != trainingDataSeries.MDimension)
                        {
                            throw new Exception("Hypothesis values returned by method hypothesisCalculator.Calculate() has 'm' dimension length " + hypothesisValues.MDimension + ".  Expected length " + +trainingDataSeries.MDimension + ".");
                        }
                        if (hypothesisValues.NDimension != 1)
                        {
                            throw new Exception("Hypothesis values returned by method hypothesisCalculator.Calculate() has 'n' dimension length " + hypothesisValues.NDimension + ".  Expected length 1.");
                        }

                        for (int j = 1; j <= newThetaParameters.MDimension; j++)
                        {
                            Double newThetaValue = 0;
                            Matrix costSeries = hypothesisValues - trainingDataResults;

                            // Retrieve the data series corresponding to the current theta value
                            Matrix currentThetaDataSeries = new Matrix(trainingDataSeries.MDimension, 1);
                            for (int k = 1; k <= currentThetaDataSeries.MDimension; k++)
                            {
                                currentThetaDataSeries.SetElement(k, 1, trainingDataSeries.GetElement(k, j));
                            }

                            // Transpose the current theta data series and multiply by the cost to effectively multiply element-wise and then sum the series
                            Double summedCost = (costSeries.Transpose() * currentThetaDataSeries).GetElement(1, 1);
                            newThetaValue = currentThetaParameters.GetElement(j, 1) - (learningRate * (1.0 / trainingDataSeries.MDimension)) * summedCost;

                            newThetaParameters.SetElement(j, 1, newThetaValue);
                        }

                        // TODO: Some inefficiency could be removed here, as hypothesis for training data series is potentially recalculated inside the costFunctionCalculator class (when it has already been calculated above).
                        Double cost = costFunctionCalculator.Calculate(trainingDataSeries, trainingDataResults, currentThetaParameters);
                        totalIterationsCounter++;
                        loggingUtilities.Log(this, LogLevel.Debug, "Gradient descent iteration " + totalIterationsCounter + ", cost = " + cost + ".");

                        currentThetaParameters = newThetaParameters;
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                    remainingIterations = remainingIterations - performedIterations;
                }
            }
            catch(OperationCanceledException)
            {
                logger.Log(this, LogLevel.Information, "Gradient descent optimization cancelled after "  + totalIterationsCounter + " iterations.");
                metricsUtilities.CancelBegin(new GradientDescentOptimizationTime());
                throw;
            }
            catch (Exception)
            {
                metricsUtilities.CancelBegin(new GradientDescentOptimizationTime());
                throw;
            }

            metricsUtilities.End(new GradientDescentOptimizationTime());
            metricsUtilities.Increment(new GradientDescentOptimizationCompleted());
            metricsUtilities.Add(new GradientDescentIterations(maxIterations));
            loggingUtilities.Log(this, LogLevel.Information, "Completed running gradient descent optimization.");

            return currentThetaParameters;
        }
    }
}
