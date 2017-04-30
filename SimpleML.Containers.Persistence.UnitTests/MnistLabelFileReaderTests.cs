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
    /// Unit tests for class SimpleML.Containers.Persistence.MnistLabelFileReader.
    /// </summary>
    public class MnistLabelFileReaderTests
    {
        private const String testFilePath = @"C:\Temp\TestMnistLabelFile.csv";
        private Mockery mockery;
        private IFile mockFile;
        private IFileStream mockFileStream;
        private MnistLabelFileReader testMnistLabelFileReader;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockFile = mockery.NewMock<IFile>();
            mockFileStream = mockery.NewMock<IFileStream>();
            testMnistLabelFileReader = new MnistLabelFileReader(mockFile);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Read() method is called with a blank filePath parameter.
        /// </summary>
        [Test]
        public void Read_FilePathParameterBlank()
        {
            ArgumentNullException e = Assert.Throws<ArgumentNullException>(delegate
            {
                testMnistLabelFileReader.Read("", 1, 10);
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
                testMnistLabelFileReader.Read(null, 1, 10);
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
                testMnistLabelFileReader.Read(testFilePath, 0, 10);
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
                testMnistLabelFileReader.Read(testFilePath, 1, 0);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'numberOfItems' must be greater than equal to 1."));
            Assert.AreEqual("numberOfItems", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Read() method is called where the 'startItem' and 'numberOfItems' parameters exceed the number of labels in the file.
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
                testMnistLabelFileReader.Read(testFilePath, 60000, 2);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameters 'startItem' and 'numberOfItems' specify a range that is greater than the actual number of labels in the file 60000."));
            Assert.AreEqual("numberOfItems", e.ParamName);

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests that an exception is thrown if the Read() method is called where file being read contains less labels than stated in the file header.
        /// </summary>
        [Test]
        public void Read_FileIncomplete()
        {
            Byte[] numberOfItemsBytes = BitConverter.GetBytes(20);
            Byte[] labelBytes = new Byte[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 0 };
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(numberOfItemsBytes);
            }

            using (mockery.Ordered)
            {
                Expect.Once.On(mockFile).Method("OpenRead").With(testFilePath).Will(Return.Value(mockFileStream));
                Expect.Once.On(mockFileStream).Method("Seek").With(4L, System.IO.SeekOrigin.Begin).Will(Return.Value(4L));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", numberOfItemsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[10], 0, 10).Will(new NMock2.Actions.SetNamedParameterAction("array", labelBytes), Return.Value(9));
                Expect.Once.On(mockFileStream).Method("Dispose");
            }

            Exception e = Assert.Throws<Exception>(delegate
            {
                testMnistLabelFileReader.Read(testFilePath, 1, 10);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Attempted to read 10 labels from file '" + testFilePath + "', but was only able to read 9.  The file may be incomplete."));

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Success tests for the Read() method.
        /// </summary>
        [Test]
        public void Read()
        {
            Byte[] numberOfItemsBytes = BitConverter.GetBytes(20);
            Byte[] labelBytes = new Byte[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            if (BitConverter.IsLittleEndian == true)
            {
                Array.Reverse(numberOfItemsBytes);
            }

            using (mockery.Ordered)
            {
                Expect.Once.On(mockFile).Method("OpenRead").With(testFilePath).Will(Return.Value(mockFileStream));
                Expect.Once.On(mockFileStream).Method("Seek").With(4L, System.IO.SeekOrigin.Begin).Will(Return.Value(4L));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[4], 0, 4).Will(new NMock2.Actions.SetNamedParameterAction("array", numberOfItemsBytes), Return.Value(4));
                Expect.Once.On(mockFileStream).Method("Read").With(new Byte[10], 0, 10).Will(new NMock2.Actions.SetNamedParameterAction("array", labelBytes), Return.Value(10));
                Expect.Once.On(mockFileStream).Method("Dispose");
            }

            Byte[] result = testMnistLabelFileReader.Read(testFilePath, 1, 10);

            Assert.AreEqual(10, result.Length);
            for (Int32 i = 0; i < result.Length; i++)
            {
                Assert.AreEqual(labelBytes[i], result[i]);
            }
            
            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
