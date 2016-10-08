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
        /// <summary>Utililty class to modify matrices.</summary>
        private MatrixUtilities matrixUtilities;
        /// <summary>The logger to write log events to.</summary>
        private IApplicationLogger logger;
        /// <summary>Utility class used to write log events.</summary>
        private LoggingUtilities loggingUtilities;
        /// <summary>Utility class used to write metric events.</summary>
        private MetricsUtilities metricsUtilities;

        /// <summary>
        /// Initialises a new instance of the SimpleML.GradientDescentOptimizer class.
        /// </summary>
        public GradientDescentOptimizer()
        {
            matrixUtilities = new MatrixUtilities();
            logger = new NullApplicationLogger();
            loggingUtilities = new LoggingUtilities(logger);
            metricsUtilities = new MetricsUtilities(new NullMetricLogger());
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.GradientDescentOptimizer class.
        /// </summary>
        /// <param name="logger">The logger to write log events to.</param>
        public GradientDescentOptimizer(IApplicationLogger logger)
            : this()
        {
            this.logger = logger;
            loggingUtilities = new LoggingUtilities(logger);
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.GradientDescentOptimizer class.
        /// </summary>
        /// <param name="metricLogger">The metric logger to write metric and instrumentation events to.</param>
        public GradientDescentOptimizer(IMetricLogger metricLogger)
            : this()
        {
            metricsUtilities = new MetricsUtilities(metricLogger);
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.GradientDescentOptimizer class.
        /// </summary>
        /// <param name="logger">The logger to write log events to.</param>
        /// <param name="metricLogger">The metric logger to write metric and instrumentation events to.</param>
        public GradientDescentOptimizer(IApplicationLogger logger, IMetricLogger metricLogger)
            : this()
        {
            this.logger = logger;
            loggingUtilities = new LoggingUtilities(logger);
            metricsUtilities = new MetricsUtilities(metricLogger);
        }

        /// <summary>
        /// Performs gradient descent optimization.
        /// </summary>
        /// <param name="initialThetaParameters">The initial theta parameter values to use in the optimization, stored column-wise in a matrix.</param>
        /// <param name="trainingDataSeries">The training data series, stored column-wise in a matrix.</param>
        /// <param name="trainingDataResults">The training data results, stored as a single column matrix.</param>
        /// <param name="hypothesisCalculator">Class used to calculate the value of the hypothesis function.</param>
        /// <param name="learningRate">The learning rate (step size) to use in the optimization.</param>
        /// <param name="maxIterations">Maximum number of iterations to run the gradient descent for.</param>
        /// <returns>A single column matrix containing the optimized theta parameter values.</returns>
        public Matrix Optimize(Matrix initialThetaParameters, Matrix trainingDataSeries, Matrix trainingDataResults, IHypothesisCalculator hypothesisCalculator, Double learningRate, Int32 maxIterations)
        {
            if ( trainingDataSeries.NDimension != (initialThetaParameters.MDimension - 1) )
            {
                throw new ArgumentException("The 'm' dimension of parameter 'initialThetaParameters' must be 1 greater than the 'n' dimension of parameter 'trainingDataSeries'.", "initialThetaParameters");
            }
            if (initialThetaParameters.NDimension != 1)
            {
                throw new ArgumentException("The parameter 'initialThetaParameters' must be a single column matrix (i.e. 'n' dimension equal to 1).", "initialThetaParameters");
            }
            if (trainingDataSeries.MDimension != trainingDataResults.MDimension)
            {
                throw new ArgumentException("Parameters 'trainingDataSeries' and 'trainingDataResults' have differing 'm' dimensions.", "trainingDataResults");
            }
            if (trainingDataResults.NDimension != 1)
            {
                throw new ArgumentException("The parameter 'trainingDataResults' must be a single column matrix (i.e. 'n' dimension equal to 1).", "trainingDataResults");
            }
            if (learningRate <= 0)
            {
                throw new ArgumentException("The parameter 'learningRate' must be positive.", "learningRate");
            }
            if (maxIterations <= 0)
            {
                throw new ArgumentException("The parameter 'maxIterations' must be a positive integer.", "maxIterations");
            }

            metricsUtilities.Begin(new GradientDescentOptimizationTime());

            Matrix currentThetaParameters;
            try
            {
                currentThetaParameters = CopyMatrix(initialThetaParameters);

                loggingUtilities.Log(this, LogLevel.Information, "Running gradient descent optimization for " + initialThetaParameters.MDimension + " theta parameters, " + trainingDataSeries.MDimension + " training data points, " + maxIterations + " iterations.");

                // Add a column of 1's to the training data
                Matrix biasedTrainingDataSeries = matrixUtilities.AddColumns(trainingDataSeries, 1, true, 1);

                for (int i = 1; i <= maxIterations; i++)
                {
                    // Calculate the new theta values
                    Matrix newThetaParameters = new Matrix(initialThetaParameters.MDimension, initialThetaParameters.NDimension);

                    Matrix hypothesisValues = hypothesisCalculator.Calculate(biasedTrainingDataSeries, currentThetaParameters);
                    if (hypothesisValues.MDimension != biasedTrainingDataSeries.MDimension)
                    {
                        throw new Exception("Hypothesis values returned by method hypothesisCalculator.Calculate() has 'm' dimension length " + hypothesisValues.MDimension + ".  Expected length " + +biasedTrainingDataSeries.MDimension + ".");
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
                        Matrix currentThetaDataSeries = new Matrix(biasedTrainingDataSeries.MDimension, 1);
                        for (int k = 1; k <= currentThetaDataSeries.MDimension; k++)
                        {
                            currentThetaDataSeries.SetElement(k, 1, biasedTrainingDataSeries.GetElement(k, j));
                        }

                        // Transpose the current theta data series and multiply by the cost to effectively multiply element-wise and then sum the series
                        Double summedCost = (costSeries.Transpose() * currentThetaDataSeries).GetElement(1, 1);
                        newThetaValue = currentThetaParameters.GetElement(j, 1) - (learningRate * (1.0 / biasedTrainingDataSeries.MDimension)) * summedCost;

                        newThetaParameters.SetElement(j, 1, newThetaValue);
                    }

                    Matrix resultDifferences = hypothesisValues - trainingDataResults;
                    Func<Double, Double> absoluteValue = new Func<Double, Double>((x) => { return Math.Abs(x); });
                    resultDifferences.ApplyElementWiseOperation(absoluteValue);
                    Double totalCost = (resultDifferences.SumHorizontally()).GetElement(1, 1);
                    loggingUtilities.Log(this, LogLevel.Debug, "Gradient descent iteration " + i + ", cost = " + totalCost + ".");

                    currentThetaParameters = newThetaParameters;
                }

            }
            catch (Exception e)
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
        
        /// <summary>
        /// Makes a copy of the specified matrix.
        /// </summary>
        /// <param name="inputMatrix">The matrix to copy.</param>
        /// <returns>The copy of the matrix.</returns>
        private Matrix CopyMatrix(Matrix inputMatrix)
        {
            // TODO: Consider moving this to the Matrix class

            Matrix returnMatrix = new Matrix(inputMatrix.MDimension, inputMatrix.NDimension);

            for (int i = 1; i <= inputMatrix.NDimension; i++)
            {
                for (int j = 1; j <= inputMatrix.MDimension; j++)
                {
                    returnMatrix.SetElement(j, i, inputMatrix.GetElement(j, i));
                }
            }

            return returnMatrix;
        }
    }
}
