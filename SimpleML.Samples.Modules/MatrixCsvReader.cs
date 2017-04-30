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
using SimpleML.Containers.Persistence;

namespace SimpleML.Samples.Modules
{
    /// <summary>
    /// An MMF module which converts a CSV file into a matrix.
    /// </summary>
    public class MatrixCsvReader : ModuleBase
    {
        private const String csvFilePathInputSlotName = "CsvFilePath";
        private const String startColumnInputSlotName = "CsvStartingColumn";
        private const String numberOfColumnsInputSlotName = "CsvNumberOfColumns";
        private const String matrixOutputSlotName = "Matrix";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.MatrixCsvReader class.
        /// </summary>
        public MatrixCsvReader()
            : base()
        {
            Description = "Reads the contents of a CSV file into a SimpleML.Containers.Matrix class";
            AddInputSlot(csvFilePathInputSlotName, "The full path to the CSV file", typeof(String));
            AddInputSlot(startColumnInputSlotName, "The index of the first column to read from the CSV file (1-based)", typeof(Int32));
            AddInputSlot(numberOfColumnsInputSlotName, "The number of columns to read from the CSV file", typeof(Int32));
            AddOutputSlot(matrixOutputSlotName, "A matrix containing the data from the CSV file", typeof(Matrix));
        }

        protected override void ImplementProcess()
        {
            // TODO: Look at possibility of storing slot types as local constants, and having a utility method on ModuleBase to cast based on type in a variable
            //       e.g. public T ConvertType<T>(object input)
            String csvFilePath = (String)GetInputSlot(csvFilePathInputSlotName).DataValue;
            Int32 startColumn = (Int32)GetInputSlot(startColumnInputSlotName).DataValue;
            Int32 numberOfColumns = (Int32)GetInputSlot(numberOfColumnsInputSlotName).DataValue;

            SimpleML.Containers.Persistence.MatrixCsvReader matrixCsvReader = new Containers.Persistence.MatrixCsvReader();
            metricLogger.Begin(new CsvFileReadTime());
            Matrix result = null;
            try
            {
                result = matrixCsvReader.Read(csvFilePath, startColumn, numberOfColumns);
            }
            catch (Exception e)
            {
                metricLogger.CancelBegin(new CsvFileReadTime());
                logger.Log(this, LogLevel.Critical, "Error occurred whilst attempting to read matrix from CSV file at path \"" + csvFilePath + "\".", e);
                throw;
            }
            metricLogger.End(new CsvFileReadTime());
            metricLogger.Increment(new CsvFileRead());
            logger.Log(this, LogLevel.Information, "Read CSV data from file at path \"" + csvFilePath + "\" into a matrix.");
            GetOutputSlot(matrixOutputSlotName).DataValue = result;
        }
    }
}
