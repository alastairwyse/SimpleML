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

namespace SimpleML.Containers
{
    /// <summary>
    /// Container class containing the cost of a neural network at a specified training epoch.
    /// </summary>
    public class CostHistoryItem
    {
        /// <summary>The epoch of training.</summary>
        private Int32 epoch;
        /// <summary>The cost at the specified epoch.</summary>
        private Double cost;

        /// <summary>
        /// The epoch of training.
        /// </summary>
        public Int32 Epoch
        {
            get
            {
                return epoch;
            }
        }

        /// <summary>
        /// The cost at the specified epoch.
        /// </summary>
        public Double Cost
        {
            get
            {
                return cost;
            }
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.Containers.CostHistoryItem class.
        /// </summary>
        /// <param name="epoch">The epoch of training.</param>
        /// <param name="cost">The cost at the specified epoch.</param>
        public CostHistoryItem(Int32 epoch, Double cost)
        {
            if (epoch < 1)
            {
                throw new ArgumentException("Parameter 'epoch' much be greater than 0.", "epoch");
            }

            this.epoch = epoch;
            this.cost = cost;
        }
    }
}
