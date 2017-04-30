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
using ApplicationLogging;
using SimpleML;
using SimpleML.Containers;
using SimpleML.Containers.Persistence;

namespace SimpleML.UnitTests.LoggingTests
{
    /// <summary>
    /// Tests for the logging functionality in class SimpleML.FunctionMinimizer.
    /// </summary>
    public class FunctionMinimizerTests
    {
        private Mockery mockery;
        private IApplicationLogger mockApplicationLogger;
        private FunctionMinimizer testFunctionMinimizer;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockApplicationLogger = mockery.NewMock<IApplicationLogger>();
            testFunctionMinimizer = new FunctionMinimizer(mockApplicationLogger);
        }

        /// <summary>
        /// Success tests for the Minimize() method.
        /// </summary>
        [Test]
        public void Minimize()
        {
            // TODO: Remove dependency on external test data file.  Need a test case with fewer inputs and iterations.

            MatrixUtilities matrixUtilities = new MatrixUtilities();
            MatrixCsvReader matrixCsvReader = new MatrixCsvReader();
            String testDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\FunctionMinimizer Test Data.csv");
            Matrix testData = matrixCsvReader.Read(testDataPath, 1, 3);
            Matrix dataSeries = testData.GetColumnSubset(1, 2);
            dataSeries = matrixUtilities.AddColumns(dataSeries, 1, true, 1.0);
            Matrix dataResults = testData.GetColumnSubset(3, 1);
            Matrix initialThetaParameters = new Matrix(3, 1);

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testFunctionMinimizer, LogLevel.Information, "Running function minimzation for 3 theta parameters, 100 data points, 2 iterations.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testFunctionMinimizer, LogLevel.Debug, "Function minimization iteration 1, cost = 0.631683744307642.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testFunctionMinimizer, LogLevel.Debug, "Function minimization iteration 2, cost = 0.629758258000487.");
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testFunctionMinimizer, LogLevel.Information, "Completed running function minimization.");
            }

            testFunctionMinimizer.Minimize(dataSeries, dataResults, initialThetaParameters, 0, new LogisticRegressionCostFunctionCalculator(), 2);

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
