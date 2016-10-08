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
using ApplicationLogging;
using MathematicsModularFramework;
using SimpleML.Containers;

namespace SimpleML.Samples.Modules
{
    /// <summary>
    /// An MMF module which splits a matrix horizontally into two new matrices according to the specified proportions.  Can be used to split a set of training data into separate portions for training and test.
    /// </summary>
    public class MatrixTrainTestSplitter : ModuleBase
    {
        private const String inputDataInputSlotName = "InputData";
        private const String trainProportionInputSlotName = "TrainProportion";
        private const String trainDataOutputSlotName = "TrainData";
        private const String testDataOutputSlotName = "TestData";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.MatrixTrainTestSplitter class.
        /// </summary>
        public MatrixTrainTestSplitter()
            : base()
        {
            Description = "Splits the data in SimpleML.Containers.Matrix into training and testing data";
            AddInputSlot(inputDataInputSlotName, "The data to split", typeof(Matrix));
            AddInputSlot(trainProportionInputSlotName, "The percentage of the data to use as training data (remaining data will be test data)", typeof(Int32));
            AddOutputSlot(trainDataOutputSlotName, "The train portion of the data", typeof(Matrix));
            AddOutputSlot(testDataOutputSlotName, "The test portion of the data", typeof(Matrix));
        }

        protected override void ImplementProcess()
        {
            Matrix inputData = (Matrix)GetInputSlot(inputDataInputSlotName).DataValue;
            Int32 trainProportion = (Int32)GetInputSlot(trainProportionInputSlotName).DataValue;

            // Check arguments/parameters
            if ((trainProportion < 1) || (trainProportion > 99))
            {
                throw new ArgumentException("Parameter '" + trainProportionInputSlotName + "' must be between 1 and 99 (inclusive).", trainProportionInputSlotName);
            }

            // Decide how many rows of the matrix should form the train and test portions
            Int32 trainNumRows = (Int32)Math.Round((inputData.MDimension * trainProportion / 100.0), 0);
            Int32 testNumRows = inputData.MDimension - trainNumRows;

            Matrix trainData = inputData.GetRowSubset(1, trainNumRows);
            Matrix testData = inputData.GetRowSubset(trainNumRows + 1, testNumRows);
            GetOutputSlot(trainDataOutputSlotName).DataValue = trainData;
            GetOutputSlot(testDataOutputSlotName).DataValue = testData;
            logger.Log(this, LogLevel.Information, "Split matrix into train and test portions of " + trainNumRows + " and " + testNumRows + " rows respectively.");
        }
    }
}
