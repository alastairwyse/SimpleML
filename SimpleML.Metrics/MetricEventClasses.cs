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
using ApplicationMetrics;

namespace SimpleML.Metrics
{
    public class GradientDescentOptimizationCompleted : CountMetric
    {
        public GradientDescentOptimizationCompleted()
        {
            base.name = "GradientDescentOptimizationCompleted";
            base.description = "The number gradient descent optimizations completed";
        }
    }

    public class GradientDescentIterations : AmountMetric
    {
        public GradientDescentIterations(long iterations)
        {
            base.name = "GradientDescentIterations";
            base.description = "The number of iterations of gradient descent";
            base.amount = iterations;
        }
    }

    public class GradientDescentOptimizationTime : IntervalMetric
    {
        public GradientDescentOptimizationTime()
        {
            base.name = "GradientDescentOptimizationTime";
            base.description = "The time taken to perform gradient descent optimization";
        }
    }
}
