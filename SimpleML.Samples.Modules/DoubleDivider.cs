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

namespace SimpleML.Samples.Modules
{
    /// <summary>
    ///  An MMF module which divides one integer by another, returning the result as a double.
    /// </summary>
    public class DoubleDivider : ModuleBase
    {
        private const String dividendInputSlotName = "Dividend";
        private const String divisorInputSlotName = "Divisor";
        private const String resultOutputSlotName = "Result";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.DoubleDivider class.
        /// </summary>
        public DoubleDivider()
            : base()
        {
            Description = "Divides one integer by another, returning the result as a double";
            AddInputSlot(dividendInputSlotName, "The dividend", typeof(Int32));
            AddInputSlot(divisorInputSlotName, "The divisor", typeof(Int32));
            AddOutputSlot(resultOutputSlotName, "The result of the division", typeof(Double));
        }

        protected override void ImplementProcess()
        {
            Int32 dividend = (Int32)GetInputSlot(dividendInputSlotName).DataValue;
            Int32 divisor = (Int32)GetInputSlot(divisorInputSlotName).DataValue;

            GetOutputSlot(resultOutputSlotName).DataValue = Convert.ToDouble(dividend) / Convert.ToDouble(divisor);
        }
    }
}
