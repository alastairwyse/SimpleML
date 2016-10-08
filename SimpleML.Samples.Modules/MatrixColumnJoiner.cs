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
using ApplicationLogging;
using MathematicsModularFramework;
using SimpleML.Containers;

namespace SimpleML.Samples.Modules
{
    /// <summary>
    /// An MMF module which joins two matrices column-wise (i.e. horizontally) to produce a new matrix.
    /// </summary>
    public class MatrixColumnJoiner : ModuleBase
    {
        private const String leftMatrixInputSlotName = "LeftMatrix";
        private const String rightMatrixInputSlotName = "RightMatrix";
        private const String outputMatrixOutputSlotName = "OutputMatrix";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.MatrixColumnJoiner class.
        /// </summary>
        public MatrixColumnJoiner()
            : base()
        {
            Description = "Joins two SimpleML.Containers.Matrix classes column-wise (i.e. horizontally) to produce a new matrix";
            AddInputSlot(leftMatrixInputSlotName, "The matrix to join on the left side", typeof(Matrix));
            AddInputSlot(rightMatrixInputSlotName, "The matrix to join on the right side", typeof(Matrix));
            AddOutputSlot(outputMatrixOutputSlotName, "The joined matrix", typeof(Matrix));
        }

        protected override void ImplementProcess()
        {
            Matrix leftMatrix = (Matrix)GetInputSlot(leftMatrixInputSlotName).DataValue;
            Matrix rightMatrix = (Matrix)GetInputSlot(rightMatrixInputSlotName).DataValue;

            try
            {
                MatrixUtilities matrixUtilities = new MatrixUtilities();
                Matrix outputMatrix = matrixUtilities.JoinHorizontally(leftMatrix, rightMatrix);
                GetOutputSlot(outputMatrixOutputSlotName).DataValue = outputMatrix;
            }
            catch (Exception e)
            {
                logger.Log(this, LogLevel.Critical, "Error occurred whilst attempting to join matrices.", e);
                throw;
            }
            logger.Log(this, LogLevel.Information, "Joined matrices column-wise to produce a " + leftMatrix.MDimension + " x " + (leftMatrix.NDimension + rightMatrix.NDimension) + " matrix.");
        }
    }
}
