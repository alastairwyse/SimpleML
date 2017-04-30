﻿/*
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
using NMock2;
using ApplicationMetrics;

namespace SimpleML.UnitTests.MetricsTests
{
    /// <summary>
    /// Implementation of NMock2.Matcher class for metrics to allow different metric objects of the same type to be regarded as equal.
    /// </summary>
    /// <remarks>See http://nmock.sourceforge.net/advanced.html for reference.</remarks>
    class MetricMatcher : Matcher
    {
        // TODO: As this class will be used in every *.MetricsTests project, consider moving the class to ApplicationMetrics. 

        private MetricBase metric;

        public MetricMatcher(MetricBase metric)
        {
            this.metric = metric;
        }

        public override void DescribeTo(System.IO.TextWriter writer)
        {
            writer.Write(metric.ToString());
        }

        public override bool Matches(object o)
        {
            if (o.GetType() == metric.GetType())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
