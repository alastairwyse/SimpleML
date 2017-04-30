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
    /// Tests for the logging functionality in class SimpleML.Samples.Modules.MatrixColumnSplitter.
    /// </summary>
    public class MatrixColumnSplitterTests
    {
        private Mockery mockery;
        private IApplicationLogger mockApplicationLogger;
        private MatrixColumnSplitter testMatrixColumnSplitter;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockApplicationLogger = mockery.NewMock<IApplicationLogger>();
            testMatrixColumnSplitter = new MatrixColumnSplitter();
            testMatrixColumnSplitter.Logger = mockApplicationLogger;
        }

        /// <summary>
        /// Tests the logging functionality when the ImplementProcess() method is called with 'RightSideColumns' parameter less than 1.
        /// </summary>
        [Test]
        public void ImplementProcess_RightSideColumnsParameterLessThan1()
        {
            testMatrixColumnSplitter.GetInputSlot("RightSideColumns").DataValue = 0;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testMatrixColumnSplitter, LogLevel.Critical, "Parameter 'RightSideColumns' must be greater than 0.", new TypeMatcher(typeof(ArgumentException)));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrixColumnSplitter.Process();
            });

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests the logging functionality when the ImplementProcess() method is called with 'RightSideColumns' parameter which is the same as the 'n' dimension of the matrix in parameter 'InputMatrix'.
        /// </summary>
        [Test]
        public void ImplementProcess_RightSideItemsParameterSameAsListSize()
        {
            Matrix inputMatrix = new Matrix(4, 3);
            testMatrixColumnSplitter.GetInputSlot("InputMatrix").DataValue = inputMatrix;
            testMatrixColumnSplitter.GetInputSlot("RightSideColumns").DataValue = 3;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testMatrixColumnSplitter, LogLevel.Critical, "Parameter 'RightSideColumns' must be less than the 'n' dimension of the input matrix.", new TypeMatcher(typeof(ArgumentException)));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrixColumnSplitter.Process();
            });

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests the logging functionality in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            Matrix inputMatrix = new Matrix(2, 4, new Double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0 });
            testMatrixColumnSplitter.GetInputSlot("InputMatrix").DataValue = inputMatrix;
            testMatrixColumnSplitter.GetInputSlot("RightSideColumns").DataValue = 1;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testMatrixColumnSplitter, LogLevel.Information, "Split 4 column matrix into matrices of 3 and 1 columns.");
            }

            testMatrixColumnSplitter.Process();

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
