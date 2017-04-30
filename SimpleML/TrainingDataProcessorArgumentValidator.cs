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

namespace SimpleML
{
    /// <summary>
    /// Provides methods to validate training data and theta parameter arguments.
    /// </summary>
    /// <remarks>Many classes that process training data and theta parameters have the same argument validation checks.  This class allows the code for those argument validation checks to be centrlized.</remarks>
    class TrainingDataProcessorArgumentValidator
    {
        /// <summary>
        /// Initialises a new instance of the SimpleML.TrainingDataProcessorArgumentValidator class.
        /// </summary>
        public TrainingDataProcessorArgumentValidator()
        {
        }

        /// <summary>
        /// Validate the inputted training data and theta parameter arguments.
        /// </summary>
        /// <param name="dataSeries">The data series.</param>
        /// <param name="dataResults">The data results.</param>
        /// <param name="thetaParameters">The theta parameter values.</param>
        /// <param name="dataSeriesArgumentName">The name of the data series argument in the client classes' method (for use in an Exception message if required).</param>
        /// <param name="dataResultsArgumentName">>The name of the data results argument in the client classes' method (for use in an Exception message if required).</param>
        /// <param name="thetaParametersArgumentName">>The name of the theta parameters argument in the client classes' method (for use in an Exception message if required).</param>
        public void ValidateArguments(Matrix dataSeries, Matrix dataResults, Matrix thetaParameters, String dataSeriesArgumentName, String dataResultsArgumentName, String thetaParametersArgumentName)
        {
            if (dataSeries.NDimension != thetaParameters.MDimension)
            {
                throw new ArgumentException("The 'm' dimension of parameter '" + thetaParametersArgumentName + "' must match the 'n' dimension of parameter '" + dataSeriesArgumentName + "'.", thetaParametersArgumentName);
            }
            if (thetaParameters.NDimension != 1)
            {
                throw new ArgumentException("The parameter '" + thetaParametersArgumentName + "' must be a single column matrix (i.e. 'n' dimension equal to 1).", thetaParametersArgumentName);
            }
            if (dataSeries.MDimension != dataResults.MDimension)
            {
                throw new ArgumentException("Parameters '" + dataSeriesArgumentName + "' and '" + dataResultsArgumentName + "' have differing 'm' dimensions.", dataResultsArgumentName);
            }
            if (dataResults.NDimension != 1)
            {
                throw new ArgumentException("The parameter '" + dataResultsArgumentName + "' must be a single column matrix (i.e. 'n' dimension equal to 1).", dataResultsArgumentName);
            }
        }
    }
}
