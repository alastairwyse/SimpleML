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
 * 
 * (C) Copyright 1999, 2000 & 2001, Carl Edward Rasmussen
 * 
 * Permission is granted for anyone to copy, use, or modify these
 * programs and accompanying documents for purposes of research or
 * education, provided this copyright notice is retained, and note is
 * made of any changes that have been made.
 * 
 * These programs and documents are distributed without any warranty,
 * express or implied.  As the programs were written for research
 * purposes only, they have not been tested to the degree that would be
 * advisable in any important application.  All use of these programs is
 * entirely at the user's own risk.
 *
 * Changes:
 * 1) Wrapped in a class
 * 2) Method signature changed
 * 3) Converted to C# from Octave/Matlab
 * 4) Added statements for logging and metrics
 * 
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
    /// Minimizes a continuous differentiable multivariate function.
    /// </summary>
    public class FunctionMinimizer
    {
        // Code in this class is translated from Octave/Matlab function 'fmincg' written by Carl Edward Rasmussen, and provided as part of the Coursera Machine Learning course instructed by Andrew Ng (https://www.coursera.org/learn/machine-learning).

        /// <summary>Utility class used to write log events.</summary>
        private LoggingUtilities loggingUtilities;
        /// <summary>Utility class used to write metric events.</summary>
        private MetricsUtilities metricsUtilities;

        /// <summary>
        /// Initialises a new instance of the SimpleML.FunctionMinimizer class.
        /// </summary>
        public FunctionMinimizer()
        {
            loggingUtilities = new LoggingUtilities(new NullApplicationLogger());
            metricsUtilities = new MetricsUtilities(new NullMetricLogger());
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.FunctionMinimizer class.
        /// </summary>
        /// <param name="logger">The logger to write log events to.</param>
        public FunctionMinimizer(IApplicationLogger logger)
            : this()
        {
            loggingUtilities = new LoggingUtilities(logger);
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.FunctionMinimizer class.
        /// </summary>
        /// <param name="metricLogger">The metric logger to write metric and instrumentation events to.</param>
        public FunctionMinimizer(IMetricLogger metricLogger)
            : this()
        {
            metricsUtilities = new MetricsUtilities(metricLogger);
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.FunctionMinimizer class.
        /// </summary>
        /// <param name="logger">The logger to write log events to.</param>
        /// <param name="metricLogger">The metric logger to write metric and instrumentation events to.</param>
        public FunctionMinimizer(IApplicationLogger logger, IMetricLogger metricLogger)
            : this()
        {
            loggingUtilities = new LoggingUtilities(logger);
            metricsUtilities = new MetricsUtilities(metricLogger);
        }

        /// <summary>
        /// Minimizes a continuous differentiable multivariate function.
        /// </summary>
        /// <param name="dataSeries">The data series, stored column-wise in a matrix.</param>
        /// <param name="dataResults">The data results, stored as a single column matrix.</param>
        /// <param name="initialThetaParameters">The initial theta parameter values, stored as a single column matrix.</param>
        /// <param name="regularizationParameter">The regularization parameter (lambda).</param>
        /// <param name="costFunctionCalculator">The class to use to evaluate the cost during minimization.</param>
        /// <param name="maxIterations">The maximum number of iterations during minimization.</param>
        /// <returns>A single column matrix containing the optimized theta values.</returns>
        public Matrix Minimize(Matrix dataSeries, Matrix dataResults, Matrix initialThetaParameters, Double regularizationParameter, ICostFunctionGradientCalculator costFunctionCalculator, Int32 maxIterations)
        {
            // Line search constants
            const Double rho = 0.01;
            const Double sig = 0.5;
            const Double limitBoundary = 0.1;
            const Double extrapolateCount = 3.0;
            const Int32 maxFunctionEvaluations = 20;
            const Int32 maxSlopeRatio = 100;

            MatrixUtilities matrixUtilities = new MatrixUtilities();
            TrainingDataProcessorArgumentValidator argumentValidator = new TrainingDataProcessorArgumentValidator();

            argumentValidator.ValidateArguments(dataSeries, dataResults, initialThetaParameters, "dataSeries", "dataResults", "initialThetaParameters");
            if (regularizationParameter < 0)
            {
                throw new ArgumentOutOfRangeException("regularizationParameter", regularizationParameter, "The parameter 'regularizationParameter' must be greater than or equal to 0.");
            }
            if (maxIterations < 1)
            {
                throw new ArgumentOutOfRangeException("maxIterations", maxIterations, "The parameter 'maxIterations' must be greater than 0.");
            }

            metricsUtilities.Begin(new FunctionMinimizationTime());

            Int32 i = 0;
            Boolean lineSearchFailed = false;
            Tuple<Double, Matrix> costResult1 = costFunctionCalculator.Calculate(dataSeries, dataResults, initialThetaParameters, regularizationParameter);
            Double f1 = costResult1.Item1;
            Matrix df1 = costResult1.Item2;
            Matrix s = matrixUtilities.Copy(df1);
            s.ApplyElementWiseOperation(new Func<Double, Double>((x) => { return -x; }));
            Matrix d1Matrix = df1.Transpose() * s;
            Double d1 = d1Matrix.GetElement(1, 1);
            // TODO: Potential for divide by zero error here if d1 is 1.0??
            Double z1 = 1.0 / (1.0 - d1);
            Matrix X = matrixUtilities.Copy(initialThetaParameters);

            loggingUtilities.Log(this, LogLevel.Information, "Running function minimzation for " + initialThetaParameters.MDimension + " theta parameters, " + dataSeries.MDimension + " data points, " + maxIterations + " iterations.");

            while (i < maxIterations)
            {
                i++;
                Matrix X0 = matrixUtilities.Copy(X);
                Double f0 = f1;
                Matrix df0 = matrixUtilities.Copy(df1);
                Matrix sTemp = matrixUtilities.Copy(s);
                sTemp.ApplyElementWiseOperation(new Func<Double, Double>((x) => { return x * z1; }));
                X = X + sTemp;
                Tuple<Double, Matrix> costResult2 = costFunctionCalculator.Calculate(dataSeries, dataResults, X, regularizationParameter);
                Double f2 = costResult2.Item1;
                Matrix df2 = costResult2.Item2;
                Double d2 = (df2.Transpose() * s).GetElement(1, 1);
                Double f3 = f1;
                Double d3 = d1;
                Double z3 = -z1;
                Int32 M = maxFunctionEvaluations;
                Boolean success = false;
                Double limit = -1;

                while (true)
                {
                    Double z2 = 0;
                    Double A = 0;
                    Double B = 0;
                    
                    while ( ( (f2 > f1 + z1 * rho * d1) || (d2 > -sig*d1) ) && (M > 0) )
                    {
                        limit = z1;
                        if (f2 > f1)
                        {
                            z2 = z3 - (0.5 * d3 * z3 * z3) / (d3 * z3 + f2 - f3);
                        }
                        else
                        {
                            A = 6 * (f2 - f3) / z3 + 3 * (d2 + d3);
                            B = 3 * (f3 - f2) - z3 * (d3 + 2 * d2);
                            z2 = (Math.Sqrt(B * B - A * d2 * z3 * z3) - B) / A;
                        }
                        if (Double.IsNaN(z2) == true || Double.IsInfinity(z2) == true)
                        {
                            z2 = z3 / 2.0;     
                        }
                        z2 = Math.Max(Math.Min(z2, limitBoundary * z3), (1 - limitBoundary) * z3);
                        z1 = z1 + z2;
                        sTemp = matrixUtilities.Copy(s);
                        sTemp.ApplyElementWiseOperation(new Func<Double, Double>((x) => { return x * z2; }));
                        X = X + sTemp;
                        costResult2 = costFunctionCalculator.Calculate(dataSeries, dataResults, X, regularizationParameter);
                        f2 = costResult2.Item1;
                        df2 = costResult2.Item2;
                        M = M - 1;
                        d2 = (df2.Transpose() * s).GetElement(1, 1);
                        z3 = z3 - z2;
                    }

                    if ((f2 > f1 + z1 * rho * d1) || (d2 > -sig * d1))
                    {
                        break;
                    }
                    else if (d2 > sig * d1)
                    {
                        success = true;
                        break;
                    }
                    else if (M == 0)
                    {
                        break;
                    }

                    A = 6 * (f2 - f3) / z3 + 3 * (d2 + d3);
                    B = 3 * (f3 - f2) - z3 * (d3 + 2 * d2);
                    z2 = -d2 * z3 * z3 / (B + Math.Sqrt(B * B - A * d2 * z3 * z3));

                    if ((Double.IsNaN(z2) == true) || (Double.IsInfinity(z2) == true) || (z2 < 0))
                    {
                        if (limit < -0.5)
                        {
                            z2 = z1 * (extrapolateCount - 1);
                        }
                        else
                        {
                            z2 = (limit - z1) / 2.0;
                        }
                    }
                    else if ((limit > -0.5) && (z2 + z1 > limit))
                    {
                        z2 = (limit - z1) / 2;
                    }
                    else if ((limit < -0.5) && (z2 + z1 > z1 * extrapolateCount))
                    {
                        z2 = z1 * (extrapolateCount - 1);
                    }
                    else if (z2 < -z3 * limitBoundary)
                    {
                        z2 = -z3 * limitBoundary;
                    }
                    else if ((limit > -0.5) && (z2 < (limit - z1) * (1 - limitBoundary)))
                    {
                        z2 = (limit - z1) * (1 - limitBoundary);
                    }

                    f3 = f2;
                    d3 = d2;
                    z3 = -z2;
                    z1 = z1 + z2;
                    sTemp = matrixUtilities.Copy(s);
                    sTemp.ApplyElementWiseOperation(new Func<Double, Double>((x) => { return x * z2; }));
                    X = X + sTemp;
                    costResult2 = costFunctionCalculator.Calculate(dataSeries, dataResults, X, regularizationParameter);
                    f2 = costResult2.Item1;
                    df2 = costResult2.Item2;
                    M = M - 1;
                    d2 = (df2.Transpose() * s).GetElement(1, 1);
                }

                if (success == true)
                {
                    f1 = f2;
                    loggingUtilities.Log(this, LogLevel.Debug, "Function minimization iteration " + i + ", cost = " + f1 + ".");
                    // Omitting line 'fX = [fX' f1]';', as fX is not output
                    // Below section implements the line commented as 'Polack-Ribiere direction' in fmincg
                    Matrix numerator = (df2.Transpose() * df2) - (df1.Transpose() * df2);
                    Matrix denominator = df1.Transpose() * df1;
                    Double fractionValue = numerator.GetElement(1, 1) / denominator.GetElement(1, 1);
                    s.ApplyElementWiseOperation(new Func<Double,Double>((x) => { return x * fractionValue; }));
                    s = s - df2;
                    Matrix swapTemp = df1; 
                    df1 = df2;
                    df2 = swapTemp;
                    d2 = (df1.Transpose() * s).GetElement(1, 1);
                    if (d2 > 0)
                    {
                        s = matrixUtilities.Copy(df1);
                        s.ApplyElementWiseOperation(new Func<Double, Double>((x) => { return -x; }));
                        d2 = -(s.Transpose() * s).GetElement(1, 1);
                    }
                    z1 = z1 * Math.Min(maxSlopeRatio, d1 / (d2 - Double.Epsilon));
                    d1 = d2;
                    lineSearchFailed = false;
                }
                else
                {
                    X = matrixUtilities.Copy(X0); 
                    f1 = f0;
                    df1 = df0;
                    if (lineSearchFailed == true || i > maxIterations)
                    {
                        break;
                    }
                    Matrix swapTemp = df1;
                    df1 = df2;
                    df2 = swapTemp;
                    s = matrixUtilities.Copy(df1);
                    s.ApplyElementWiseOperation(new Func<Double, Double>((x) => { return -x; }));
                    d1 = -(s.Transpose() * s).GetElement(1, 1);
                    z1 = 1 / (1 - d1);
                    lineSearchFailed = true;
                }
            }

            metricsUtilities.End(new FunctionMinimizationTime());
            metricsUtilities.Increment(new FunctionMinimizationCompleted());
            metricsUtilities.Add(new FunctionMinimizationIterations(i));
            loggingUtilities.Log(this, LogLevel.Information, "Completed running function minimization.");

            return X;
        }
    }
}
