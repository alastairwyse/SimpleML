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
using FrameworkAbstraction;

namespace SimpleML.Containers.Persistence
{
    /// <summary>
    /// Reads digit image label data in MNIST format from a file, and returns the data in a byte array with each element a label value (expected to be 0-9 inclusive).
    /// </summary>
    /// <remarks>See http://yann.lecun.com/exdb/mnist/ for details on the expected file format.</remarks>
    public class MnistLabelFileReader
    {
        private IFile file;

        /// <summary>
        /// Initialises a new instance of the SimpleML.Containers.Persistence.MnistLabelFileReader class.
        /// </summary>
        public MnistLabelFileReader()
        {
            file = new File();
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.Containers.Persistence.MnistLabelFileReader class.  Note this is an additional constructor to facilitate unit tests, and should not be used to instantiate the class under normal conditions.
        /// </summary>
        /// <param name="file">A test (mock) File object.</param>
        public MnistLabelFileReader(IFile file)
        {
            this.file = file;
        }

        /// <summary>
        /// Reads the MNIST labels from the specified file.
        /// </summary>
        /// <param name="filePath">The full path to the MNIST label file.</param>
        /// <param name="startItem">The (1 based) index of the first label to read from the file.</param>
        /// <param name="numberOfItems">The number of labels to read from the file.</param>
        /// <returns>The labels values in a byte array.</returns>
        public Byte[] Read(String filePath, Int32 startItem, Int32 numberOfItems)
        {
            if (String.IsNullOrEmpty(filePath) == true)
            {
                throw new ArgumentNullException("filePath", "Parameter 'filePath' is empty or null.");
            }
            if (startItem < 1)
            {
                throw new ArgumentException("Parameter 'startItem' must be greater than equal to 1.", "startItem");
            }
            if (numberOfItems < 1)
            {
                throw new ArgumentException("Parameter 'numberOfItems' must be greater than equal to 1.", "numberOfItems");
            }

            using (IFileStream fileStream = file.OpenRead(filePath))
            {
                // Move to the position holding the number of items
                fileStream.Seek(4, System.IO.SeekOrigin.Begin);
                // Read the actual number of items
                Byte[] actualItemsBytes = new Byte[4];
                fileStream.Read(ref actualItemsBytes, 0, 4);
                if (BitConverter.IsLittleEndian == true)
                {
                    Array.Reverse(actualItemsBytes);
                }
                Int32 actualItems = BitConverter.ToInt32(actualItemsBytes, 0);

                // Check that the startImage and numberOfImages parameters don't exceed the actual number of items
                if ((startItem + numberOfItems - 1) > actualItems)
                {
                    throw new ArgumentException("Parameters 'startItem' and 'numberOfItems' specify a range that is greater than the actual number of labels in the file " + actualItems + ".", "numberOfItems");
                }

                // Read the labels
                Byte[] labelBytes = new Byte[numberOfItems];
                Int32 readByteCount = fileStream.Read(ref labelBytes, 0, numberOfItems);
                if (readByteCount < numberOfItems)
                {
                    throw new Exception("Attempted to read " + (startItem + numberOfItems - 1) + " labels from file '" + filePath + "', but was only able to read " + readByteCount + ".  The file may be incomplete.");
                }

                return labelBytes;
            }
        }
    }
}
