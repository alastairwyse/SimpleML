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
using SimpleML;
using SimpleML.Containers;

namespace SimpleML.Samples.Modules
{
    /// <summary>
    /// An MMF module which adds columns to a matrix.
    /// </summary>
    public class MatrixColumnAdder : ModuleBase
    {
        private const String inputMatrixInputSlotName = "InputMatrix";
        private const String numberOfColumnsInputSlotName = "NumberOfColumns";
        private const String leftSideInputSlotName = "LeftSide";
        private const String defaultValueInputSlotName = "DefaultValue";
        private const String outputMatrixOutputSlotName = "OutputMatrix";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.MatrixColumnAdder class.
        /// </summary>
        public MatrixColumnAdder()
            : base()
        {
            Description = "Adds columns to a matrix";
            AddInputSlot(inputMatrixInputSlotName, "The matrix to add columns to", typeof(Matrix));
            AddInputSlot(numberOfColumnsInputSlotName, "The number of columns to add", typeof(Int32));
            AddInputSlot(leftSideInputSlotName, "Whether the columns should be added to the left side of the matrix (true = left, false = right)", typeof(Boolean));
            AddInputSlot(defaultValueInputSlotName, "The default value for the elements in the new columns", typeof(Double));
            AddOutputSlot(outputMatrixOutputSlotName, "The matrix with the additional columns", typeof(Matrix));
        }

        protected override void ImplementProcess()
        {
            Matrix inputMatrix = (Matrix)GetInputSlot(inputMatrixInputSlotName).DataValue;
            Int32 numberOfColumns = (Int32)GetInputSlot(numberOfColumnsInputSlotName).DataValue;
            Boolean leftSide = (Boolean)GetInputSlot(leftSideInputSlotName).DataValue;
            Double defaultValue = (Double)GetInputSlot(defaultValueInputSlotName).DataValue;
            MatrixUtilities matrixUtilities = new MatrixUtilities();

            if (numberOfColumns < 1)
            {
                String message = "Parameter '" + numberOfColumnsInputSlotName + "' must be greater than or equal to 1.";
                ArgumentException e = new ArgumentException(message, numberOfColumnsInputSlotName);
                logger.Log(this, LogLevel.Critical, message, e);
                throw e;
            }

            Matrix outputMatrix = matrixUtilities.AddColumns(inputMatrix, numberOfColumns, leftSide, defaultValue);
            GetOutputSlot(outputMatrixOutputSlotName).DataValue = outputMatrix;
            logger.Log(this, LogLevel.Information, "Added " + numberOfColumns + " " + (numberOfColumns == 1 ? "column" : "columns") + " to " + (leftSide == true ? "left" : "right") + " side of matrix, with default value " + defaultValue + ".");
        }
    }
}
