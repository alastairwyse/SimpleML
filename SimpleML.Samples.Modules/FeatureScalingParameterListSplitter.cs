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
using SimpleML;

namespace SimpleML.Samples.Modules
{
    /// <summary>
    /// An MMF module which removes a specified number of SimpleML.FeatureScalingParameters from the end of the inputted List, producing 2 Lists.  Can be used to split a List containing scaling parameters for training and results data, into separate Lists.
    /// </summary>
    public class FeatureScalingParameterListSplitter : ModuleBase
    {
        private const String inputScalingParametersInputSlotName = "InputScalingParameters";
        private const String rightSideItemsInputSlotName = "RightSideItems";
        private const String startScalingParametersOutputSlotName = "StartScalingParameters";
        private const String endScalingParametersOutputSlotName = "EndScalingParameters";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.FeatureScalingParameterListSplitter class.
        /// </summary>
        public FeatureScalingParameterListSplitter()
            : base()
        {
            Description = "Removes a specified number of SimpleML.FeatureScalingParameters from the end of the inputted List, producing 2 Lists.  Can be used to split a List containing scaling parameters for training and results data, into separate Lists";
            AddInputSlot(inputScalingParametersInputSlotName, "The matrix to split", typeof(List<FeatureScalingParameters>));
            AddInputSlot(rightSideItemsInputSlotName, "The number of items to remove from the right side of the List", typeof(Int32));
            AddOutputSlot(startScalingParametersOutputSlotName, "The beginning items from the original List", typeof(List<FeatureScalingParameters>));
            AddOutputSlot(endScalingParametersOutputSlotName, "The end items from the original List", typeof(List<FeatureScalingParameters>));
        }

        protected override void ImplementProcess()
        {
            List<FeatureScalingParameters> inputScalingParameters = (List<FeatureScalingParameters>)GetInputSlot(inputScalingParametersInputSlotName).DataValue;
            Int32 rightSideItems = (Int32)GetInputSlot(rightSideItemsInputSlotName).DataValue;

            if (rightSideItems < 1)
            {
                String message = "Parameter '" + rightSideItemsInputSlotName + "' must be greater than 0.";
                ArgumentException e = new ArgumentException(message, rightSideItemsInputSlotName);
                logger.Log(this, LogLevel.Critical, message, e);
                throw e;
            }
            if (rightSideItems > (inputScalingParameters.Count - 1))
            {
                String message = "Parameter '" + rightSideItemsInputSlotName + "' must be less than the size of the inputted list.";
                ArgumentException e = new ArgumentException(message, rightSideItemsInputSlotName);
                logger.Log(this, LogLevel.Critical, message, e);
                throw e;
            }

            List<FeatureScalingParameters> startScalingParameters = inputScalingParameters.GetRange(0, inputScalingParameters.Count - rightSideItems);
            List<FeatureScalingParameters> endScalingParameters = inputScalingParameters.GetRange((inputScalingParameters.Count - rightSideItems), rightSideItems);
            GetOutputSlot(startScalingParametersOutputSlotName).DataValue = startScalingParameters;
            GetOutputSlot(endScalingParametersOutputSlotName).DataValue = endScalingParameters;
            logger.Log(this, LogLevel.Information, "Split List of FeatureScalingParameters of size " + inputScalingParameters.Count + " into Lists of " + (inputScalingParameters.Count - rightSideItems) + " and " + rightSideItems + " items.");
        }
    }
}
