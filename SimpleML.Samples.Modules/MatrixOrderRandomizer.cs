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
using MathematicsModularFramework;
using SimpleML.Containers;

namespace SimpleML.Samples.Modules
{
    /// <summary>
    /// An MMF module which randomizes the order of rows in a matrix.
    /// </summary>
    public class MatrixOrderRandomizer : ModuleBase
    {
        private const String inputMatrixInputSlotName = "InputMatrix";
        private const String randomSeedInputSlotName = "RandomSeed";
        private const String outputMatrixOutputSlotName = "OutputMatrix";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.MatrixOrderRandomizer class.
        /// </summary>
        public MatrixOrderRandomizer()
            : base()
        {
            Description = "Randomizes the order of rows in a SimpleML.Containers.Matrix class";
            AddInputSlot(inputMatrixInputSlotName, "The matrix to randomize the rows of", typeof(Matrix));
            AddInputSlot(randomSeedInputSlotName, "The seed value for the random number generator", typeof(Int32));
            AddOutputSlot(outputMatrixOutputSlotName, "The matrix with row order randomized", typeof(Matrix));
        }

        protected override void ImplementProcess()
        {
            Matrix inputMatrix = (Matrix)GetInputSlot(inputMatrixInputSlotName).DataValue;
            Int32 randomSeed = (Int32)GetInputSlot(randomSeedInputSlotName).DataValue;

            MatrixRandomizer maxtrixRandomizer = new MatrixRandomizer();
            Matrix outputMatrix = maxtrixRandomizer.Randomize(inputMatrix, randomSeed);
            metricLogger.Increment(new MatrixRowOrderRandomized());
            GetOutputSlot(outputMatrixOutputSlotName).DataValue = outputMatrix;
        }
    }
}
