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
    /// Contains utlity methods for implementations of interface IHypothesisCalculator.
    /// </summary>
    public class HypothesisCalculatorUtilities
    {
        /// <summary>
        /// Verifies parameters passed to the Calculate() method.
        /// </summary>
        /// <param name="dataSeries">The data series used in evaluating the hypothesis.</param>
        /// <param name="thetaParameters">The parameter values of the hypothesis.</param>
        public void VerifyParameters(Matrix dataSeries, Matrix thetaParameters)
        {
            if (dataSeries.NDimension != thetaParameters.MDimension)
            {
                throw new ArgumentException("The 'm' dimension of parameter 'thetaParameters' must match the 'n' dimension of parameter 'dataSeries'.", "thetaParameters");
            }

            if (thetaParameters.NDimension != 1)
            {
                throw new ArgumentException("The parameter 'thetaParameters' must be a single column matrix (i.e. 'n' dimension equal to 1).", "thetaParameters");
            }
        }
    }
}
