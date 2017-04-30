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
using SimpleML.Containers;

namespace SimpleML
{
    /// <summary>
    /// Calculates the logistic regression error rate for a set of theta parameters against a set of input and result data.
    /// </summary>
    public class LogisticRegressionErrorRateCalculator
    {
        TrainingDataProcessorArgumentValidator argumentValidator;
        IHypothesisCalculator hypothesisCalculator;

        /// <summary>
        /// Initialises a new instance of the SimpleML.LogisticRegressionErrorRateCalculator class.
        /// </summary>
        public LogisticRegressionErrorRateCalculator()
        {
            argumentValidator = new TrainingDataProcessorArgumentValidator();
            hypothesisCalculator = new LogisticRegressionHypothesisCalculator();
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.LogisticRegressionErrorRateCalculator class.  Note this is an additional constructor to facilitate unit tests, and should not be used to instantiate the class under normal conditions.
        /// </summary>
        /// <param name="hypothesisCalculator">A test (mock) IHypothesisCalculator object.</param>
        public LogisticRegressionErrorRateCalculator(IHypothesisCalculator hypothesisCalculator)
            : this()
        {
            this.hypothesisCalculator = hypothesisCalculator;
        }

        /// <summary>
        /// Calculates the logistic regression error rate.
        /// </summary>
        /// <param name="dataSeries">The data series used to calculate the error rate, stored column-wise in a matrix.</param>
        /// <param name="dataResults">The data results, stored as a single column matrix.</param>
        /// <param name="thetaParameters">The theta parameter values to use in the error rate calculation, stored as a single column matrix.</param>
        /// <returns>The error rate.</returns>
        public Double Calculate(Matrix dataSeries, Matrix dataResults, Matrix thetaParameters)
        {
            argumentValidator.ValidateArguments(dataSeries, dataResults, thetaParameters, "dataSeries", "dataResults", "thetaParameters");

            Int32 errorCount = 0;

            Matrix hypothesisResults = hypothesisCalculator.Calculate(dataSeries, thetaParameters);
            // Iterate through the hypothesis results and compare to the actual results.
            for (Int32 i = 1; i <= hypothesisResults.MDimension; i++)
            {
                Double currentHypothesisResult = hypothesisResults.GetElement(i, 1);
                Double roundedHypothesisResult = Math.Round(currentHypothesisResult);
                // Increment the error count if the rounded result differs from the actual result
                if (roundedHypothesisResult != dataResults.GetElement(i, 1))
                {
                    errorCount++;
                }
            }

            // Convert the error count to a percentage of the total 
            return Convert.ToDouble(errorCount) / Convert.ToDouble(hypothesisResults.MDimension);
        }
    }
}
