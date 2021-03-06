﻿/*
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
    /// Tests for the logging functionality in class SimpleML.Samples.Modules.MatrixTrainTestSplitter.
    /// </summary>
    public class MatrixTrainTestSplitterTests
    {
        private Mockery mockery;
        private IApplicationLogger mockApplicationLogger;
        private MatrixTrainTestSplitter testMatrixTrainTestSplitter;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockApplicationLogger = mockery.NewMock<IApplicationLogger>();
            testMatrixTrainTestSplitter = new MatrixTrainTestSplitter();
            testMatrixTrainTestSplitter.Logger = mockApplicationLogger;
        }

        /// <summary>
        /// Tests the logging functionality when the ImplementProcess() method is called with 'TrainProportion' parameter greater than 99.
        /// </summary>
        [Test]
        public void ImplementProcess_RightSideColumnsParameterLessThan1()
        {
            testMatrixTrainTestSplitter.GetInputSlot("TrainProportion").DataValue = 100;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testMatrixTrainTestSplitter, LogLevel.Critical, "Parameter 'TrainProportion' must be between 1 and 99 (inclusive).", new TypeMatcher(typeof(ArgumentException)));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testMatrixTrainTestSplitter.Process();
            });

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests the logging functionality in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            Matrix inputMatrix = new Matrix(5, 2, new Double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0 });
            testMatrixTrainTestSplitter.GetInputSlot("InputData").DataValue = inputMatrix;
            testMatrixTrainTestSplitter.GetInputSlot("TrainProportion").DataValue = 60;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testMatrixTrainTestSplitter, LogLevel.Information, "Split matrix into train and test portions of 3 and 2 rows respectively.");
            }

            testMatrixTrainTestSplitter.Process();

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
