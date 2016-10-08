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

namespace SimpleML
{
    /// <summary>
    /// Container class holding parameters used in feature scaling.
    /// </summary>
    public class FeatureScalingParameters
    {
        private Double mean;
        private Double span;

        /// <summary>
        /// The average of all feature values before scaling.
        /// </summary>
        public Double Mean
        {
            get
            {
                return mean;
            }
        }

        /// <summary>
        /// The span (i.e. difference between highest and lowest values) of all feature values before scaling.
        /// </summary>
        public Double Span
        {
            get
            {
                return span;
            }
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.FeatureScalingParameters class.
        /// </summary>
        /// <param name="mean">The average of all feature values before scaling.</param>
        /// <param name="span">The span (i.e. difference between highest and lowest values) of all feature values before scaling.</param>
        public FeatureScalingParameters(Double mean, Double span)
        {
            this.mean = mean;
            this.span = span;
        }
    }
}
