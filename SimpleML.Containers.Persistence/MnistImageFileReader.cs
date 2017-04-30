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
    /// Reads digit image data in MNIST format from a file, and returns the data in a 2 dimensional byte array.
    /// </summary>
    /// <remarks>See http://yann.lecun.com/exdb/mnist/ for details on the expected file format.</remarks>
    public class MnistImageFileReader
    {
        private IFile file;
        /// <summary>The number of rows of pixels in each image.</summary>
        private Nullable<Int32> imageRows;
        /// <summary>The number of columns of pixels in each image.</summary>
        private Nullable<Int32> imageColumns;

        /// <summary>
        /// The number of rows of pixels in each image.
        /// </summary>
        public Int32 ImageRows
        {
            get
            {
                if (imageRows == null)
                {
                    throw new InvalidOperationException("Property cannot be retrieved until the Read() method has been called.");
                }
                else
                {
                    return (Int32)imageRows;
                }
            }
        }

        /// <summary>
        /// The number of columns of pixels in each image.
        /// </summary>
        public Int32 ImageColumns
        {
            get
            {
                if (imageColumns == null)
                {
                    throw new InvalidOperationException("Property cannot be retrieved until the Read() method has been called.");
                }
                else
                {
                    return (Int32)imageColumns;
                }
            }
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.Containers.Persistence.MnistImageFileReader class.
        /// </summary>
        public MnistImageFileReader()
        {
            file = new File();
            imageRows = null;
            imageColumns = null;
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.Containers.Persistence.MnistImageFileReader class.  Note this is an additional constructor to facilitate unit tests, and should not be used to instantiate the class under normal conditions.
        /// </summary>
        /// <param name="file">A test (mock) File object.</param>
        public MnistImageFileReader(IFile file)
            : this()
        {
            this.file = file;
        }

        /// <summary>
        /// Reads the MNIST images from the specified file.
        /// </summary>
        /// <param name="filePath">The full path to the MNIST image file.</param>
        /// <param name="startItem">The (1 based) index of the first image to read from the file.</param>
        /// <param name="numberOfItems">The number of images to read from the file.</param>
        /// <returns>A 2 dimensional byte array representing the image data.  The first dimension represents each image.  The second dimension represents the intensity of each pixel in an individual image (read left to right, top to bottom), ranging from 0 (white) to 255 (black).</returns>
        public Byte[,] Read(String filePath, Int32 startItem, Int32 numberOfItems)
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

            using(IFileStream fileStream = file.OpenRead(filePath))
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
                    throw new ArgumentException("Parameters 'startItem' and 'numberOfItems' specify a range that is greater than the actual number of images in the file " + actualItems + ".", "numberOfItems");
                }
                
                // Read the rows and columns
                Byte[] rowsBytes = new Byte[4], columnsBytes = new Byte[4];
                fileStream.Read(ref rowsBytes, 0, 4);
                fileStream.Read(ref columnsBytes, 0, 4);
                if (BitConverter.IsLittleEndian == true)
                {
                    Array.Reverse(rowsBytes);
                    Array.Reverse(columnsBytes);
                }
                imageRows = BitConverter.ToInt32(rowsBytes, 0);
                imageColumns = BitConverter.ToInt32(columnsBytes, 0);
                Int32 numberOfImagesToRead = startItem + numberOfItems - 1;
                Int32 numberOfPixelsPerImage = (Int32)imageRows * (Int32)imageColumns;
                
                // Read the images
                Byte[] singleImageBytes = new Byte[numberOfPixelsPerImage];
                Byte[,] returnImages = new Byte[numberOfImagesToRead, numberOfPixelsPerImage];
                for (Int32 i = 0; i < numberOfImagesToRead; i++)
                {
                    Int32 readByteCount = fileStream.Read(ref singleImageBytes, 0, numberOfPixelsPerImage);
                    if (readByteCount < numberOfPixelsPerImage)
                    {
                        throw new Exception("Attempted to read " + numberOfImagesToRead + " images from file '" + filePath + "', but was only able to read " + i + ".  The file may be incomplete.");
                    }

                    // Copy to the array to return
                    for (Int32 k = 0; k < numberOfPixelsPerImage; k++)
                    {
                        returnImages[i, k] = singleImageBytes[k];
                    }
                }

                return returnImages;
            }
        }
    }
}
