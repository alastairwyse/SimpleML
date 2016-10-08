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
using SimpleML.Containers;

namespace SimpleML.Containers.Persistence
{
    /// <summary>
    /// Converts a file in CSV format to a matrix.
    /// </summary>
    public class MatrixCsvReader
    {
        IFile file;

        /// <summary>
        /// Initialises a new instance of the SimpleML.Containers.Persistence.MatrixCsvReader class.
        /// </summary>
        public MatrixCsvReader()
        {
            file = new File();
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.Containers.Persistence.MatrixCsvReader class.  Note this is an additional constructor to facilitate unit tests, and should not be used to instantiate the class under normal conditions.
        /// </summary>
        /// <param name="file">A test (mock) File object.</param>
        public MatrixCsvReader(IFile file)
        {
            this.file = file;
        }

        /// <summary>
        /// Reads specified columns from a CSV file, and returns them as a matrix.
        /// </summary>
        /// <param name="filePath">The full path to the CSV file.</param>
        /// <param name="startColumn">The column within the file to start reading at.</param>
        /// <param name="numberOfColumns">The number of columns to read.</param>
        /// <returns>The CSV file data as a matrix.</returns>
        public Matrix Read(String filePath, Int32 startColumn, Int32 numberOfColumns)
        {
            if (startColumn < 1)
            {
                throw new ArgumentException("Parameter 'startColumn' must be greater than or equal to 1.", "startColumn");
            }
            if (numberOfColumns < 1)
            {
                throw new ArgumentException("Parameter 'numberOfColumns' must be greater than or equal to 1.", "numberOfColumns");
            }

            String[] lines;

            try
            {
                lines = file.ReadAllLines(filePath);
            }
            catch (Exception e)
            {
                throw new Exception("Unable to read from file '" + filePath + "'.", e);
            }
            Matrix returnMatrix = new Matrix(lines.Length, numberOfColumns);

            for (int i = 0; i < lines.Length; i++)
            {
                String[] items = lines[i].Split(',');
                if (items.Length < (startColumn + numberOfColumns - 1))
                {
                    throw new Exception("Row " + (i + 1) + " of the file does not contain enough columns.  Expected " + (startColumn + numberOfColumns - 1) + " but found " + items.Length + ".");
                }
                for (int j = (startColumn - 1); j < (startColumn + numberOfColumns - 1); j++)
                {
                    Double currentDouble = new Double();
                    Boolean result = Double.TryParse(items[j], out currentDouble);
                    if (result == true)
                    {
                        returnMatrix.SetElement(i + 1, (j - startColumn + 2), currentDouble);
                    }
                    else
                    {
                        throw new Exception("CSV element at row " + (i + 1) + ", column " + (j + 1) + " '" + items[j] + "' could not be converted to a number.");
                    }
                }
            }
            return returnMatrix;
        }
    }
}
