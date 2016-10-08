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
using NUnit.Framework;
using NMock2;

namespace SimpleML.Containers.Persistence.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.Containers.Persistence.MatrixCsvReader.
    /// </summary>
    public class MatrixCsvReaderTests
    {
        private const String testFilePath = @"C:\Temp\TestCsvFile.csv";
        private Mockery mockery;
        private IFile mockFile;
        private MatrixCsvReader testMatrixCsvReader;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockFile = mockery.NewMock<IFile>();
            testMatrixCsvReader = new MatrixCsvReader(mockFile);
        }

        /// <summary>
        /// Test that an exception is thrown if the Read() method is called with 'startColumn' parameter value less than 1.
        /// </summary>
        [Test]
        public void Read_StartColumnParameterLessThan1()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrixCsvReader.Read(testFilePath, 0, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'startColumn' must be greater than or equal to 1."));
            Assert.AreEqual("startColumn", e.ParamName);
        }

        /// <summary>
        /// Test that an exception is thrown if the Read() method is called with 'numberOfColumns' parameter value less than 1.
        /// </summary>
        [Test]
        public void Read_NumberOfColumnsParameterLessThan1()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrixCsvReader.Read(testFilePath, 1, 0);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'numberOfColumns' must be greater than or equal to 1."));
            Assert.AreEqual("numberOfColumns", e.ParamName);
        }

        /// <summary>
        /// Test that an exception is thrown if the Read() method is called on a file which cannot be opened.
        /// </summary>
        [Test]
        public void Read_UnableToOpenFile()
        {
            using (mockery.Ordered)
            {
                Expect.Once.On(mockFile).Method("ReadAllLines").With(testFilePath).Will(Throw.Exception(new System.IO.FileNotFoundException("Could not find file '" + testFilePath + "'.")));
            }

            Exception e = Assert.Throws<Exception>(delegate
            {
                testMatrixCsvReader.Read(testFilePath, 1, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Unable to read from file '" + testFilePath + "'."));
        }

        /// <summary>
        /// Test that an exception is thrown if the Read() method is called on a file which has insufficient columns in one of its rows.
        /// </summary>
        [Test]
        public void Read_RowContainsInsufficientColumns()
        {
            String[] fileContents = new String[3];
            fileContents[0] = "0.1,0.2,0.3";
            fileContents[1] = "0.4,0.5";
            fileContents[2] = "0.6,0.7,0.8";

            using (mockery.Ordered)
            {
                Expect.Once.On(mockFile).Method("ReadAllLines").With(testFilePath).Will(Return.Value(fileContents));
            }

            Exception e = Assert.Throws<Exception>(delegate
            {
                testMatrixCsvReader.Read(testFilePath, 2, 2);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Row 2 of the file does not contain enough columns.  Expected 3 but found 2."));
        }

        /// <summary>
        /// Test that an exception is thrown if the Read() method is called on a file which contains non-numeric data.
        /// </summary>
        [Test]
        public void Read_CellContainsNonNumericData()
        {
            String[] fileContents = new String[3];
            fileContents[0] = "0.1,0.2,0.3";
            fileContents[1] = "0.4,0.5,0.6";
            fileContents[2] = "0.7,eight,0.9";

            using (mockery.Ordered)
            {
                Expect.Once.On(mockFile).Method("ReadAllLines").With(testFilePath).Will(Return.Value(fileContents));
            }

            Exception e = Assert.Throws<Exception>(delegate
            {
                testMatrixCsvReader.Read(testFilePath, 2, 2);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("CSV element at row 3, column 2 'eight' could not be converted to a number."));
        }

        /// <summary>
        /// Success tests for the Read() method.
        /// </summary>
        [Test]
        public void Read()
        {
            String[] fileContents = new String[2];
            fileContents[0] = "0.1,0.2,0.3,0.4";
            fileContents[1] = "0.5,0.6,0.7,0.8";

            using (mockery.Ordered)
            {
                Expect.Exactly(2).On(mockFile).Method("ReadAllLines").With(testFilePath).Will(Return.Value(fileContents));
            }

            Matrix result = testMatrixCsvReader.Read(testFilePath, 1, 4);

            Assert.AreEqual(2, result.MDimension);
            Assert.AreEqual(4, result.NDimension);
            Assert.AreEqual(0.1, result.GetElement(1, 1));
            Assert.AreEqual(0.2, result.GetElement(1, 2));
            Assert.AreEqual(0.3, result.GetElement(1, 3));
            Assert.AreEqual(0.4, result.GetElement(1, 4));
            Assert.AreEqual(0.5, result.GetElement(2, 1));
            Assert.AreEqual(0.6, result.GetElement(2, 2));
            Assert.AreEqual(0.7, result.GetElement(2, 3));
            Assert.AreEqual(0.8, result.GetElement(2, 4));

            result = testMatrixCsvReader.Read(testFilePath, 3, 1);

            Assert.AreEqual(2, result.MDimension);
            Assert.AreEqual(1, result.NDimension);
            Assert.AreEqual(0.3, result.GetElement(1, 1));
            Assert.AreEqual(0.7, result.GetElement(2, 1));
        }
    }
}
