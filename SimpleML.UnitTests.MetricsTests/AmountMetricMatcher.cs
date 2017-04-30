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
using NMock2;
using ApplicationMetrics;

namespace SimpleML.UnitTests.MetricsTests
{
    /// <summary>
    /// NMock2.Matcher class which allows AmountMetric classes to be compared when used as parameters in NMock2 Expect() method calls.
    /// </summary>
    class AmountMetricMatcher : Matcher
    {
        private AmountMetric amountMetricToMatch;

        /// <summary>
        /// Initialises a new instance of the SimpleML.UnitTests.MetricsTests.AmountMetricMatcher class.
        /// </summary>
        /// <param name="amountMetricToMatch">The 'expected' AmountMetric (as opposed to the 'actual' one provided in the method call.</param>
        public AmountMetricMatcher(AmountMetric amountMetricToMatch)
        {
            this.amountMetricToMatch = amountMetricToMatch;
        }

        public override bool Matches(object o)
        {
            if (amountMetricToMatch.GetType() != o.GetType())
            {
                return false;
            }

            AmountMetric comparisonAmountMetric = (AmountMetric)o;
            if (amountMetricToMatch.Amount != comparisonAmountMetric.Amount)
            {
                return false;
            }

            return true;
        }

        public override void DescribeTo(System.IO.TextWriter writer)
        {
            writer.Write(amountMetricToMatch.GetType().Name + "(" + amountMetricToMatch.Amount + ")");
        }
    }
}
