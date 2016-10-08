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
using ApplicationLogging;

namespace SimpleML
{
    /// <summary>
    /// Implementation of the IApplicationLogger interface which does not perform any logging.
    /// </summary>
    public class NullApplicationLogger : IApplicationLogger
    {
        public void Log(object source, int eventIdentifier, LogLevel level, string text, Exception sourceException)
        {
        }

        public void Log(int eventIdentifier, LogLevel level, string text, Exception sourceException)
        {
        }

        public void Log(object source, LogLevel level, string text, Exception sourceException)
        {
        }

        public void Log(LogLevel level, string text, Exception sourceException)
        {
        }

        public void Log(object source, int eventIdentifier, LogLevel level, string text)
        {
        }

        public void Log(int eventIdentifier, LogLevel level, string text)
        {
        }

        public void Log(object source, LogLevel level, string text)
        {
        }

        public void Log(LogLevel level, string text)
        {
        }
    }
}
