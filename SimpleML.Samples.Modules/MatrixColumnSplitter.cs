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
    /// An MMF module which removes a specified number of columns from the right side of the inputted matrix, producing 2 matrices.  Can be used to split a matrix containing training and results data, into separate matrices.
    /// </summary>
    public class MatrixColumnSplitter : ModuleBase
    {
        private const String inputMatrixInputSlotName = "InputMatrix";
        private const String rightSideColumnsInputSlotName = "RightSideColumns";
        private const String leftColumnsMatrixOutputSlotName = "LeftColumnsMatrix";
        private const String rightColumnsMatrixOutputSlotName = "RightColumnsMatrix";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.MatrixColumnSplitter class.
        /// </summary>
        public MatrixColumnSplitter()
            : base()
        {
            Description = "Removes a specified number of columns from the right side of the inputted SimpleML.Containers.Matrix class, producing 2 matrices.  Can be used to split a matrix containing training and results data, into separate matrices";
            AddInputSlot(inputMatrixInputSlotName, "The matrix to split", typeof(Matrix));
            AddInputSlot(rightSideColumnsInputSlotName, "The number of columns to remove from the right side of the matrix", typeof(Int32));
            AddOutputSlot(leftColumnsMatrixOutputSlotName, "The left columns of the matrix", typeof(Matrix));
            AddOutputSlot(rightColumnsMatrixOutputSlotName, "The right columns of the matrix", typeof(Matrix));
        }

        protected override void ImplementProcess()
        {
            Matrix inputMatrix = (Matrix)GetInputSlot(inputMatrixInputSlotName).DataValue;
            Int32 rightSideColumns = (Int32)GetInputSlot(rightSideColumnsInputSlotName).DataValue;

            if (rightSideColumns < 1)
            {
                throw new ArgumentException("Parameter '" + rightSideColumnsInputSlotName + "' must be greater than 0.", rightSideColumnsInputSlotName);
            }
            if (rightSideColumns > (inputMatrix.NDimension - 1))
            {
                throw new ArgumentException("Parameter '" + rightSideColumnsInputSlotName + "' must be less than the 'n' dimension of the input matrix.", rightSideColumnsInputSlotName);
            }

            Matrix leftColumnsMatrix = inputMatrix.GetColumnSubset(1, inputMatrix.NDimension - rightSideColumns);
            Matrix rightColumnsMatrix = inputMatrix.GetColumnSubset((inputMatrix.NDimension - rightSideColumns + 1), rightSideColumns);
            GetOutputSlot(leftColumnsMatrixOutputSlotName).DataValue = leftColumnsMatrix;
            GetOutputSlot(rightColumnsMatrixOutputSlotName).DataValue = rightColumnsMatrix;
            logger.Log(this, LogLevel.Information, "Split " + inputMatrix.NDimension + " column matrix into matrices of " + (inputMatrix.NDimension - rightSideColumns) + " and " + rightSideColumns + " columns.");
        }
    }
}
