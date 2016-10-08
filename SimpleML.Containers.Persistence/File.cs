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

namespace SimpleML.Containers.Persistence
{
    /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="T:SimpleML.Containers.Persistence.IFile"]/*'/>
    public class File : IFile
    {
        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:SimpleML.Containers.Persistence.IFile.ReadAllLines(System.String)"]/*'/>
        public string[] ReadAllLines(string path)
        {
            return System.IO.File.ReadAllLines(path);
        }
    }
}
