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
using MathematicsModularFramework;
using SimpleML;

namespace SimpleML.Samples.Modules
{
    /// <summary>
    /// An MMF module which returns a cost function calculator for logistic regression, for injection into input slots of other modules.
    /// </summary>
    public class LogisticRegressionCostFunctionCalculator : ModuleBase
    {
        private const String costFunctionCalculatorOutputSlotName = "CostFunctionCalculator";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.LogisticRegressionCostFunctionCalculator class.
        /// </summary>
        public LogisticRegressionCostFunctionCalculator()
            : base()
        {
            Description = "Returns a cost function calculator for logistic regression, for injection into input slots of other modules";
            AddOutputSlot(costFunctionCalculatorOutputSlotName, "A cost function calculator for logistic regression", typeof(SimpleML.LogisticRegressionCostFunctionCalculator));
        }

        protected override void ImplementProcess()
        {
            GetOutputSlot(costFunctionCalculatorOutputSlotName).DataValue = new SimpleML.LogisticRegressionCostFunctionCalculator();
        }
    }
}
