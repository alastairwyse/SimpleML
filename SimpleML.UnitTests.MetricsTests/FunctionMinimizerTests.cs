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
using ApplicationMetrics;
using SimpleML;
using SimpleML.Containers;
using SimpleML.Containers.Persistence;
using SimpleML.Metrics;

namespace SimpleML.UnitTests.MetricsTests
{
    /// <summary>
    /// Tests for the metric logging functionality in class SimpleML.FunctionMinimizer.
    /// </summary>
    public class FunctionMinimizerTests
    {
        private Mockery mockery;
        private IMetricLogger mockMetricLogger;
        private FunctionMinimizer testFunctionMinimizer;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockMetricLogger = mockery.NewMock<IMetricLogger>();
            testFunctionMinimizer = new FunctionMinimizer(mockMetricLogger);
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
                Expect.Once.On(mockMetricLogger).Method("Begin").With(IsMetric.Equal(new FunctionMinimizationTime()));
                Expect.Once.On(mockMetricLogger).Method("End").With(IsMetric.Equal(new FunctionMinimizationTime()));
                Expect.Once.On(mockMetricLogger).Method("Increment").With(IsMetric.Equal(new FunctionMinimizationCompleted()));
                Expect.Once.On(mockMetricLogger).Method("Add").With(new AmountMetricMatcher(new FunctionMinimizationIterations(2)));
            }

            testFunctionMinimizer.Minimize(dataSeries, dataResults, initialThetaParameters, 0, new LogisticRegressionCostFunctionCalculator(), 2);

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
