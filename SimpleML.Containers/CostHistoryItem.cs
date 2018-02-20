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
    /// Container class containing the cost of a neural network at a specified training batch.
    /// </summary>
    public class CostHistoryItem
    {
        /// <summary>The batch of training.</summary>
        private Int32 batch;
        /// <summary>The cost at the end of the specified batch.</summary>
        private Double cost;

        /// <summary>
        /// The batch of training.
        /// </summary>
        public Int32 Batch
        {
            get
            {
                return batch;
            }
        }

        /// <summary>
        /// The cost at the end of the specified batch.
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
        /// <param name="batch">The batch of training.</param>
        /// <param name="cost">The cost at the end of the specified batch.</param>
        public CostHistoryItem(Int32 batch, Double cost)
        {
            if (batch < 1)
            {
                throw new ArgumentException("Parameter 'batch' much be greater than 0.", "batch");
            }

            this.batch = batch;
            this.cost = cost;
        }
    }
}
