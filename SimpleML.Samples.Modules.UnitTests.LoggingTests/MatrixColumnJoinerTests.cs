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
    /// Tests for the logging functionality in class SimpleML.Samples.Modules.MatrixColumnJoiner.
    /// </summary>
    public class MatrixColumnJoinerTests
    {
        private Mockery mockery;
        private IApplicationLogger mockApplicationLogger;
        private MatrixColumnJoiner testMatrixColumnJoiner;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockApplicationLogger = mockery.NewMock<IApplicationLogger>();
            testMatrixColumnJoiner = new MatrixColumnJoiner();
            testMatrixColumnJoiner.Logger = mockApplicationLogger;
        }

        /// <summary>
        /// Tests the logging functionality when an exception occurs in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess_Exception()
        {
            Matrix leftMatrix = new Matrix(2, 3, new Double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0 });
            Matrix rightMatrix = new Matrix(1, 1, new Double[] { 7.0 });
            testMatrixColumnJoiner.GetInputSlot("LeftMatrix").DataValue = leftMatrix;
            testMatrixColumnJoiner.GetInputSlot("RightMatrix").DataValue = rightMatrix;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testMatrixColumnJoiner, LogLevel.Critical, "Error occurred whilst attempting to join matrices.", new TypeMatcher(typeof(ArgumentException)));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrixColumnJoiner.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The 'm' dimension of parameter 'rightMatrix' '1' does not match the 'm' dimension of parameter 'leftMatrix' '2'."));
            Assert.AreEqual("rightMatrix", e.ParamName);
            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests the logging functionality in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            Matrix leftMatrix = new Matrix(2, 3, new Double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0 });
            Matrix rightMatrix = new Matrix(2, 1, new Double[] { 7.0, 8.0 });
            testMatrixColumnJoiner.GetInputSlot("LeftMatrix").DataValue = leftMatrix;
            testMatrixColumnJoiner.GetInputSlot("RightMatrix").DataValue = rightMatrix;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testMatrixColumnJoiner, LogLevel.Information, "Joined matrices column-wise to produce a 2 x 4 matrix.");
            }

            testMatrixColumnJoiner.Process();

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
