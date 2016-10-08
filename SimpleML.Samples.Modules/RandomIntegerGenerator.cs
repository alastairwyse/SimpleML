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
    /// An MMF module which generates two random integers, one between 10 and 50, and the other between 2 and 10.
    /// </summary>
    public class RandomIntegerGenerator : ModuleBase
    {
        private const String smallerIntegerOutputSlotName = "SmallerInteger";
        private const String largerIntegerOutputSlotName = "LargerInteger";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.RandomIntegerGenerator class.
        /// </summary>
        public RandomIntegerGenerator()
            : base()
        {
            Description = "Generates two random integers, one between 10 and 50, and the other between 2 and 10";
            AddOutputSlot(largerIntegerOutputSlotName, "A random number between 10 and 50", typeof(Int32));
            AddOutputSlot(smallerIntegerOutputSlotName, "A random number between 2 and 10", typeof(Int32));
        }

        protected override void ImplementProcess()
        {
            Random randomGenerator = new Random();

            GetOutputSlot(largerIntegerOutputSlotName).DataValue = randomGenerator.Next(10, 51);
            GetOutputSlot(smallerIntegerOutputSlotName).DataValue = randomGenerator.Next(2, 11);
        }
    }
}
