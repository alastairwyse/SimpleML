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
using SimpleML;
using SimpleML.Containers;

namespace SimpleML.Containers.UnitTests
{
    /// <summary>
    /// Implementation of NMock2.Matcher class for equality of instances of the SimpleML.Matrix class.
    /// </summary>
    /// <remarks>See http://nmock.sourceforge.net/advanced.html for reference.</remarks>
    public class MatrixMatcher : Matcher
    {
        private Matrix matrix;

        public MatrixMatcher(Matrix matrix)
        {
            this.matrix = matrix;
        }

        public override void DescribeTo(System.IO.TextWriter writer)
        {
            writer.Write("SimpleML.Matrix (" + matrix.MDimension + " x " + matrix.NDimension + " dimensional with elements { ");
            for (int i = 1; i <= matrix.NDimension; i++)
            {
                for (int j = 1; j <= matrix.MDimension; j++)
                {
                    writer.Write(matrix.GetElement(j, i) + " ");
                }
            }
            writer.Write("})");
        }

        public override bool Matches(object o)
        {
            if (o.GetType() != matrix.GetType())
            {
                return false;
            }
            Matrix comparisonMatrix = (Matrix)o;
            if (comparisonMatrix.MDimension != matrix.MDimension)
            {
                return false;
            }
            if (comparisonMatrix.NDimension != matrix.NDimension)
            {
                return false;
            }
            for (int i = 1; i <= matrix.NDimension; i++)
            {
                for (int j = 1; j <= matrix.MDimension; j++)
                {
                    if (matrix.GetElement(j, i) != comparisonMatrix.GetElement(j, i))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
