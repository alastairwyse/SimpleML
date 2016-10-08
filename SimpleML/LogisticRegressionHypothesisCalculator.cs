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
    /// Calculates the result of the hypothesis function for logistic regression.
    /// </summary>
    public class LogisticRegressionHypothesisCalculator : IHypothesisCalculator
    {
        // TODO: Need to create unit tests for this class

        HypothesisCalculatorUtilities hypothesisCalculatorutilities;

        /// <summary>
        /// Initialises a new instance of the SimpleML.MultivariateLinearRegressionHypothesisCalculator class.
        /// </summary>
        public LogisticRegressionHypothesisCalculator()
        {
            hypothesisCalculatorutilities = new HypothesisCalculatorUtilities();
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:SimpleML.IHypothesisCalculator.Calculate(SimpleML.Matrix,SimpleML.Matrix)"]/*'/>
        public Matrix Calculate(Matrix dataSeries, Matrix thetaParameters)
        {
            hypothesisCalculatorutilities.VerifyParameters(dataSeries, thetaParameters);

            Matrix returnMatrix = dataSeries * thetaParameters;
            Func<Double, Double> sigmoidFunction = new Func<Double, Double>( (x) => { return 1 / ( 1 +  Math.Exp(-x) ); } );
            returnMatrix.ApplyElementWiseOperation(sigmoidFunction);

            return returnMatrix;
        }
    }
}
