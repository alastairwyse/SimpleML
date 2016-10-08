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
using ApplicationLogging;
using SimpleML.Containers;
using SimpleML.Samples.Modules;

namespace SimpleML.Samples.Modules.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.Samples.Modules.MatrixTrainTestSplitter.
    /// </summary>
    public class MatrixTrainTestSplitterTests
    {
        private MatrixTrainTestSplitter testMatrixTrainTestSplitter;

        [SetUp]
        protected void SetUp()
        {
            testMatrixTrainTestSplitter = new MatrixTrainTestSplitter();
            testMatrixTrainTestSplitter.Logger = new NullApplicationLogger();
        }

        /// <summary>
        /// Tests that an exception is thrown if the ImplementProcess() method is called with 'TrainProportion' parameter greater than 99.
        /// </summary>
        [Test]
        public void ImplementProcess_RightSideColumnsParameterLessThan1()
        {
            testMatrixTrainTestSplitter.GetInputSlot("TrainProportion").DataValue = 100;

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrixTrainTestSplitter.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'TrainProportion' must be between 1 and 99 (inclusive)."));
            Assert.AreEqual("TrainProportion", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the ImplementProcess() method is called with 'TrainProportion' parameter less than 1.
        /// </summary>
        [Test]
        public void ImplementProcess_RightSideColumnsParameterGreaterThan99()
        {
            testMatrixTrainTestSplitter.GetInputSlot("TrainProportion").DataValue = 0;

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrixTrainTestSplitter.Process();
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'TrainProportion' must be between 1 and 99 (inclusive)."));
            Assert.AreEqual("TrainProportion", e.ParamName);
        }

        /// <summary>
        /// Success tests for the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            Matrix inputMatrix = new Matrix(5, 2, new Double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0 });
            testMatrixTrainTestSplitter.GetInputSlot("InputData").DataValue = inputMatrix;
            testMatrixTrainTestSplitter.GetInputSlot("TrainProportion").DataValue = 60;

            testMatrixTrainTestSplitter.Process();

            Matrix trainData = (Matrix)testMatrixTrainTestSplitter.GetOutputSlot("TrainData").DataValue;
            Matrix testData = (Matrix)testMatrixTrainTestSplitter.GetOutputSlot("TestData").DataValue;

            Assert.AreEqual(3, trainData.MDimension);
            Assert.AreEqual(2, trainData.NDimension);
            Assert.That(trainData.GetElement(1, 1), NUnit.Framework.Is.EqualTo(1.0).Within(1e-1));
            Assert.That(trainData.GetElement(1, 2), NUnit.Framework.Is.EqualTo(2.0).Within(1e-1));
            Assert.That(trainData.GetElement(2, 1), NUnit.Framework.Is.EqualTo(3.0).Within(1e-1));
            Assert.That(trainData.GetElement(2, 2), NUnit.Framework.Is.EqualTo(4.0).Within(1e-1));
            Assert.That(trainData.GetElement(3, 1), NUnit.Framework.Is.EqualTo(5.0).Within(1e-1));
            Assert.That(trainData.GetElement(3, 2), NUnit.Framework.Is.EqualTo(6.0).Within(1e-1));
            Assert.AreEqual(2, testData.MDimension);
            Assert.AreEqual(2, testData.NDimension);
            Assert.That(testData.GetElement(1, 1), NUnit.Framework.Is.EqualTo(7.0).Within(1e-1));
            Assert.That(testData.GetElement(1, 2), NUnit.Framework.Is.EqualTo(8.0).Within(1e-1));
            Assert.That(testData.GetElement(2, 1), NUnit.Framework.Is.EqualTo(9.0).Within(1e-1));
            Assert.That(testData.GetElement(2, 2), NUnit.Framework.Is.EqualTo(10.0).Within(1e-1));
        }
    }
}
