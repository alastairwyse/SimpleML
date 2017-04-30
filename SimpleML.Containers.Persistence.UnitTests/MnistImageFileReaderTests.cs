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
using NUnit.Framework;
using NMock2;
using NMock2.Actions;

namespace SimpleML.Containers.Persistence.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.Containers.Persistence.MnistImageFileReader.
    /// </summary>
    public class MnistImageFileReaderTests
    {
        private const String testFilePath = @"C:\Temp\TestMnistImageFile.csv";
        private Mockery mockery;
        private IFile mockFile;
        private IFileStream mockFileStream;
        private MnistImageFileReader testMnistImageFileReader;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockFile = mockery.NewMock<IFile>();
            mockFileStream = mockery.NewMock<IFileStream>();
            testMnistImageFileReader = new MnistImageFileReader(mockFile);
        }

        /// <summary>
        /// Tests that an exception is thrown the 'ImageRows' property is interrogated before the Read() method is called.
        /// </summary>
        [Test]
        public void ImageRows_GotBeforeReadMethodCalled()
        {
            InvalidOperationException e = Assert.Throws<InvalidOperationException>(delegate
            {
                Int32 result = testMnistImageFileReader.ImageRows;
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Property cannot be retrieved until the Read() method has been called."));
        }

        /// <summary>
        /// Tests that an exception is thrown the 'ImageColumns' property is interrogated before the Read() method is called.
        /// </summary>
        [Test]
        public void ImageColumns_GotBeforeReadMethodCalled()
        {
            InvalidOperationException e = Assert.Throws<InvalidOperationException>(delegate
            {
                Int32 result = testMnistImageFileReader.ImageColumns;
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Property cannot be retrieved until the Read() method has been called."));
        }

        /// <summary>
        /// Tests that an exception is thrown if the Read() method is called with a blank filePath parameter.
        /// </summary>
        [Test]
        public void Read_FilePathParameterBlank()
        {
            ArgumentNullException e = Assert.Throws<ArgumentNullException>(delegate
            {
                testMnistImageFileReader.Read("", 1, 10);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'filePath' is empty or null."));
            Assert.AreEqual("filePath", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Read() method is called with a null filePath parameter.
        /// </summary>
        [Test]
        public void Read_FilePathParameterNull()
        {
            ArgumentException e = Assert.Throws<ArgumentNullException>(delegate
            {
                testMnistImageFileReader.Read(null, 1, 10);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'filePath' is empty or null."));
            Assert.AreEqual("filePath", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Read() method is called with a 'startItem' parameter value less than 1.
        /// </summary>
        [Test]
        public void Read_StartItemParameterLessThan1()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMnistImageFileReader.Read(testFilePath, 0, 10);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'startItem' must be greater than equal to 1."));
            Assert.AreEqual("startItem", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Read() method is called with a 'numberOfItems' parameter value less than 1.
        /// </summary>
        [Test]
        public void Read_NumberOfItemsParameterLessThan1()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMnistImageFileReader.Read(testFilePath, 1, 0);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'numberOfItems' must be greater than equal to 1."));
            Assert.AreEqual("numberOfItems", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Read() method is called where the 'startItem' and 'numberOfItems' parameters exceed the number of images in the file.
        /// </summary>
        [Test]
        public void Read_StartItemAndNumberOfItemsParametersExceedNumberOfLabels()
        {
            Byte[] numberOfItemsBytes = BitConverter.GetBytes(60000);
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(numberOfItemsBytes);
            }

            using (mockery.Ordered)
            {
                Expect.Once.On(mockFile).Method("OpenRead").With(testFilePath).Will(Return.Value(mockFileStream));
                Expect.Once.On(mockFileStream).Method("Seek").With(4L, System.IO.SeekOrigin.Begin).Will(Return.Value(4L));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", numberOfItemsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Dispose");
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMnistImageFileReader.Read(testFilePath, 60000, 2);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameters 'startItem' and 'numberOfItems' specify a range that is greater than the actual number of images in the file 60000."));
            Assert.AreEqual("numberOfItems", e.ParamName);

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests that an exception is thrown if the Read() method is called where file being read contains less images than stated in the file header.
        /// </summary>
        [Test]
        public void Read_FileIncomplete()
        {
            Byte[] numberOfItemsBytes = BitConverter.GetBytes(60000);
            Byte[] rowsBytes = BitConverter.GetBytes(2);
            Byte[] columnsBytes = BitConverter.GetBytes(3);
            Byte[] image = new Byte[] { 0, 0, 127, 255, 127, 0 };
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(numberOfItemsBytes);
                Array.Reverse(rowsBytes);
                Array.Reverse(columnsBytes);
            }

            using (mockery.Ordered)
            {
                Expect.Once.On(mockFile).Method("OpenRead").With(testFilePath).Will(Return.Value(mockFileStream));
                Expect.Once.On(mockFileStream).Method("Seek").With(4L, System.IO.SeekOrigin.Begin).Will(Return.Value(4L));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", numberOfItemsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", rowsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", columnsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[6], 0, 6).Will(new NMock2.Actions.SetNamedParameterAction("array", image), Return.Value(6));
                Expect.Once.On(mockFileStream).Method("Read").With(image, 0, 6).Will(Return.Value(0));
                Expect.Once.On(mockFileStream).Method("Dispose");
            }

            Exception e = Assert.Throws<Exception>(delegate
            {
                testMnistImageFileReader.Read(testFilePath, 1, 60000);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Attempted to read 60000 images from file '" + testFilePath + "', but was only able to read 1.  The file may be incomplete."));

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests that an exception is thrown if the Read() method is called where file being read contains less images than stated in the file header, and the final image is only partially returned (i.e. less than the total number of expected pixels).
        /// </summary>
        [Test]
        public void Read_FileIncompletePartialRead()
        {
            Byte[] numberOfItemsBytes = BitConverter.GetBytes(60000);
            Byte[] rowsBytes = BitConverter.GetBytes(2);
            Byte[] columnsBytes = BitConverter.GetBytes(3);
            Byte[] image = new Byte[] { 0, 0, 127, 255, 127, 0 };
            Byte[] image2 = new Byte[] { 0, 0, 126, 254, 126, 0 };
            Byte[] partialImage = new Byte[] { 0, 0, 127, 0, 0, 0 };
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(numberOfItemsBytes);
                Array.Reverse(rowsBytes);
                Array.Reverse(columnsBytes);
            }

            using (mockery.Ordered)
            {
                Expect.Once.On(mockFile).Method("OpenRead").With(testFilePath).Will(Return.Value(mockFileStream));
                Expect.Once.On(mockFileStream).Method("Seek").With(4L, System.IO.SeekOrigin.Begin).Will(Return.Value(4L));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", numberOfItemsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", rowsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", columnsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[6], 0, 6).Will(new NMock2.Actions.SetNamedParameterAction("array", image), Return.Value(6));
                Expect.Once.On(mockFileStream).Method("Read").With(image, 0, 6).Will(new NMock2.Actions.SetNamedParameterAction("array", image2), Return.Value(6));
                Expect.Once.On(mockFileStream).Method("Read").With(image2, 0, 6).Will(new NMock2.Actions.SetNamedParameterAction("array", partialImage), Return.Value(3));
                Expect.Once.On(mockFileStream).Method("Dispose");
            }

            Exception e = Assert.Throws<Exception>(delegate
            {
                testMnistImageFileReader.Read(testFilePath, 1, 60000);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Attempted to read 60000 images from file '" + testFilePath + "', but was only able to read 2.  The file may be incomplete."));

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Success tests for the Read() method.
        /// </summary>
        [Test]
        public void Read()
        {
            Byte[] numberOfItemsBytes = BitConverter.GetBytes(60000);
            Byte[] rowsBytes = BitConverter.GetBytes(2);
            Byte[] columnsBytes = BitConverter.GetBytes(3);
            Byte[] image1 = new Byte[] { 0, 0, 127, 255, 127, 0 };
            Byte[] image2 = new Byte[] { 0, 0, 126, 254, 126, 0 };
            Byte[] image3 = new Byte[] { 0, 5, 120, 190, 65, 3 };
            Byte[] image4 = new Byte[] { 0, 15, 123, 189, 66, 0 };
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(numberOfItemsBytes);
                Array.Reverse(rowsBytes);
                Array.Reverse(columnsBytes);
            }

            using (mockery.Ordered)
            {
                Expect.Once.On(mockFile).Method("OpenRead").With(testFilePath).Will(Return.Value(mockFileStream));
                Expect.Once.On(mockFileStream).Method("Seek").With(4L, System.IO.SeekOrigin.Begin).Will(Return.Value(4L));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", numberOfItemsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", rowsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", columnsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[6], 0, 6).Will(new NMock2.Actions.SetNamedParameterAction("array", image1), Return.Value(6));
                Expect.Once.On(mockFileStream).Method("Read").With(image1, 0, 6).Will(new NMock2.Actions.SetNamedParameterAction("array", image2), Return.Value(6));
                Expect.Once.On(mockFileStream).Method("Read").With(image2, 0, 6).Will(new NMock2.Actions.SetNamedParameterAction("array", image3), Return.Value(6));
                Expect.Once.On(mockFileStream).Method("Read").With(image3, 0, 6).Will(new NMock2.Actions.SetNamedParameterAction("array", image4), Return.Value(6));
                Expect.Once.On(mockFileStream).Method("Dispose");
            }

            Byte[,] result = testMnistImageFileReader.Read(testFilePath, 1, 4);

            Assert.AreEqual(4, result.GetLength(0));
            Assert.AreEqual(6, result.GetLength(1));
            for (Int32 i = 0; i < 6; i++)
            {
                Assert.AreEqual(image1[i], result[0, i]);
                Assert.AreEqual(image2[i], result[1, i]);
                Assert.AreEqual(image3[i], result[2, i]);
                Assert.AreEqual(image4[i], result[3, i]);
            }

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Success tests for the 'ImageColumns' property.
        /// </summary>
        [Test]
        public void ImageColumns()
        {
            Byte[] numberOfItemsBytes = BitConverter.GetBytes(60000);
            Byte[] rowsBytes = BitConverter.GetBytes(2);
            Byte[] columnsBytes = BitConverter.GetBytes(3);
            Byte[] image = new Byte[] { 0, 0, 127, 255, 127, 0 };
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(numberOfItemsBytes);
                Array.Reverse(rowsBytes);
                Array.Reverse(columnsBytes);
            }

            using (mockery.Ordered)
            {
                Expect.Once.On(mockFile).Method("OpenRead").With(testFilePath).Will(Return.Value(mockFileStream));
                Expect.Once.On(mockFileStream).Method("Seek").With(4L, System.IO.SeekOrigin.Begin).Will(Return.Value(4L));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", numberOfItemsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", rowsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", columnsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[6], 0, 6).Will(new NMock2.Actions.SetNamedParameterAction("array", image), Return.Value(6));
                Expect.Once.On(mockFileStream).Method("Dispose");
            }

            testMnistImageFileReader.Read(testFilePath, 1, 1);

            Assert.AreEqual(3, testMnistImageFileReader.ImageColumns);

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Success tests for the 'ImageRows' property.
        /// </summary>
        [Test]
        public void ImageRows()
        {
            Byte[] numberOfItemsBytes = BitConverter.GetBytes(60000);
            Byte[] rowsBytes = BitConverter.GetBytes(2);
            Byte[] columnsBytes = BitConverter.GetBytes(3);
            Byte[] image = new Byte[] { 0, 0, 127, 255, 127, 0 };
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(numberOfItemsBytes);
                Array.Reverse(rowsBytes);
                Array.Reverse(columnsBytes);
            }

            using (mockery.Ordered)
            {
                Expect.Once.On(mockFile).Method("OpenRead").With(testFilePath).Will(Return.Value(mockFileStream));
                Expect.Once.On(mockFileStream).Method("Seek").With(4L, System.IO.SeekOrigin.Begin).Will(Return.Value(4L));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", numberOfItemsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", rowsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", columnsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[6], 0, 6).Will(new NMock2.Actions.SetNamedParameterAction("array", image), Return.Value(6));
                Expect.Once.On(mockFileStream).Method("Dispose");
            }

            testMnistImageFileReader.Read(testFilePath, 1, 1);

            Assert.AreEqual(2, testMnistImageFileReader.ImageRows);

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
