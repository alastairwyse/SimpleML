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
using System.Threading;
using NMock2;

namespace SimpleML.UnitTests
{
    /// <summary>
    /// An NMock action which signals (i.e. calls Set() on) a provided EventWaitHandle class, after a specified number of calls to the Invoke() method.
    /// </summary>
    public class SignalAfterIterationsAction : IAction
    {
        private Int32 iterations;
        private Int32 count;
        private EventWaitHandle eventWaitHandle;

        /// <summary>
        /// Initialises a new instance of the SimpleML.UnitTests.SignalAfterIterationsAction class.
        /// </summary>
        /// <param name="eventWaitHandle">The EventWaitHandle to signal.</param>
        /// <param name="iterations">The number of calls to the Invoke() method to signal after.</param>
        public SignalAfterIterationsAction(EventWaitHandle eventWaitHandle, Int32 iterations)
        {
            if (iterations < 1)
            {
                throw new ArgumentException("Parameter 'iterations' must be greater than 0.", "iterations");
            }
            this.iterations = iterations;
            count = 0;
            this.eventWaitHandle = eventWaitHandle;
        }

        public void Invoke(NMock2.Monitoring.Invocation invocation)
        {
            count++;
            if (iterations == count)
            {
                eventWaitHandle.Set();
            }
        }

        public void DescribeTo(System.IO.TextWriter writer)
        {
            writer.Write("(SignalAfterIterationsAction): Invoke() method called " + count + " times");
        }
    }
}
