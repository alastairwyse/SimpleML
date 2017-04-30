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
using System.IO;
using NUnit.Framework;
using NMock2;
using NMock2.Matchers;
using ApplicationLogging;
using ApplicationMetrics;
using SimpleML.Containers;
using SimpleML.Samples.Modules;

namespace SimpleML.Samples.Modules.UnitTests.LoggingTests
{
    /// <summary>
    /// Tests for the logging functionality in class SimpleML.Samples.Modules.MatrixCsvReader.
    /// </summary>
    public class MatrixCsvReaderTests
    {
        private Mockery mockery;
        private IApplicationLogger mockApplicationLogger;
        private MatrixCsvReader testMatrixCsvReader;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockApplicationLogger = mockery.NewMock<IApplicationLogger>();
            testMatrixCsvReader = new MatrixCsvReader();
            testMatrixCsvReader.Logger = mockApplicationLogger;
            testMatrixCsvReader.MetricLogger = new NullMetricLogger();
        }

        /// <summary>
        /// Tests the logging functionality when an exception occurs in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess_Exception()
        {
            String testFilePath = @"C:\Temp\MatrixData.csv";
            testMatrixCsvReader.GetInputSlot("CsvFilePath").DataValue = testFilePath;
            testMatrixCsvReader.GetInputSlot("CsvStartingColumn").DataValue = 0;
            testMatrixCsvReader.GetInputSlot("CsvNumberOfColumns").DataValue = 5;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testMatrixCsvReader, LogLevel.Critical, "Error occurred whilst attempting to read matrix from CSV file at path \"" + testFilePath + "\".", new TypeMatcher(typeof(ArgumentException)));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrixCsvReader.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'startColumn' must be greater than or equal to 1."));
            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests the logging functionality in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            // TODO: Remove dependency on external file.  Will need to somehow be able to inject mock IFile through the module into the underlying CSV reader.

            String testFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\FunctionMinimizer Test Data.csv");
            testMatrixCsvReader.GetInputSlot("CsvFilePath").DataValue = testFilePath;
            testMatrixCsvReader.GetInputSlot("CsvStartingColumn").DataValue = 1;
            testMatrixCsvReader.GetInputSlot("CsvNumberOfColumns").DataValue = 2;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testMatrixCsvReader, LogLevel.Information, "Read CSV data from file at path \"" + testFilePath + "\" into a matrix.");
            }

            testMatrixCsvReader.Process();

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
