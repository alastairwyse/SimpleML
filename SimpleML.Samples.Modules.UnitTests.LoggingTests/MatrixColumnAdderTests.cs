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
using NMock2.Matchers;
using ApplicationLogging;
using SimpleML.Containers;
using SimpleML.Samples.Modules;

namespace SimpleML.Samples.Modules.UnitTests.LoggingTests
{
    /// <summary>
    /// Tests for the logging functionality in class SimpleML.Samples.Modules.MatrixColumnAdder.
    /// </summary>
    public class MatrixColumnAdderTests
    {
        private Mockery mockery;
        private IApplicationLogger mockApplicationLogger;
        private MatrixColumnAdder testMatrixColumnAdder;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockApplicationLogger = mockery.NewMock<IApplicationLogger>();
            testMatrixColumnAdder = new MatrixColumnAdder();
            testMatrixColumnAdder.Logger = mockApplicationLogger;
        }

        /// <summary>
        /// Tests the logging functionality when an exception occurs in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess_Exception()
        {
            testMatrixColumnAdder.GetInputSlot("InputMatrix").DataValue = new Matrix(2, 3);
            testMatrixColumnAdder.GetInputSlot("NumberOfColumns").DataValue = 0;
            testMatrixColumnAdder.GetInputSlot("LeftSide").DataValue = true;
            testMatrixColumnAdder.GetInputSlot("DefaultValue").DataValue = 1.0;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testMatrixColumnAdder, LogLevel.Critical, "Parameter 'NumberOfColumns' must be greater than or equal to 1.", new TypeMatcher(typeof(ArgumentException)));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrixColumnAdder.Process();
            });

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests the logging functionality in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            testMatrixColumnAdder.GetInputSlot("InputMatrix").DataValue = new Matrix(2, 3);
            testMatrixColumnAdder.GetInputSlot("NumberOfColumns").DataValue = 1;
            testMatrixColumnAdder.GetInputSlot("LeftSide").DataValue = true;
            testMatrixColumnAdder.GetInputSlot("DefaultValue").DataValue = 7.0;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testMatrixColumnAdder, LogLevel.Information, "Added 1 column to left side of matrix, with default value 7.");
            }

            testMatrixColumnAdder.Process();

            mockery.VerifyAllExpectationsHaveBeenMet();

            testMatrixColumnAdder = new MatrixColumnAdder();
            testMatrixColumnAdder.Logger = mockApplicationLogger;
            testMatrixColumnAdder.GetInputSlot("InputMatrix").DataValue = new Matrix(2, 3);
            testMatrixColumnAdder.GetInputSlot("NumberOfColumns").DataValue = 2;
            testMatrixColumnAdder.GetInputSlot("LeftSide").DataValue = false;
            testMatrixColumnAdder.GetInputSlot("DefaultValue").DataValue = 8.0;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testMatrixColumnAdder, LogLevel.Information, "Added 2 columns to right side of matrix, with default value 8.");
            }

            testMatrixColumnAdder.Process();

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
