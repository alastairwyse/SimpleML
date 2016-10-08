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
    /// An MMF module which scales the columns of a matrix based on inputted scaling parameters.
    /// </summary>
    public class FeatureScaler : ModuleBase
    {
        private const String inputMatrixInputSlotName = "InputMatrix";
        private const String featureScalingParametersInputSlotName = "FeatureScalingParameters";
        private const String scaledMatrixOutputSlotName = "ScaledMatrix";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.FeatureScaler class.
        /// </summary>
        public FeatureScaler()
            : base()
        {
            Description = "Scales the columns of a matrix based on inputted scaling parameters";
            AddInputSlot(inputMatrixInputSlotName, "The matrix to scale the columns of", typeof(Matrix));
            AddInputSlot(featureScalingParametersInputSlotName, "The parameter values to use for feature scaling", typeof(List<FeatureScalingParameters>));
            AddOutputSlot(scaledMatrixOutputSlotName, "The matrix with columns scaled", typeof(Matrix));
        }

        protected override void ImplementProcess()
        {
            Matrix inputMatrix = (Matrix)GetInputSlot(inputMatrixInputSlotName).DataValue;
            List<FeatureScalingParameters> featureScalingParameters = (List<FeatureScalingParameters>)GetInputSlot(featureScalingParametersInputSlotName).DataValue;

            try
            {
                SimpleML.FeatureScaler featureScaler = new SimpleML.FeatureScaler();
                Matrix scaledMatrix = featureScaler.Scale(inputMatrix, featureScalingParameters);
                GetOutputSlot(scaledMatrixOutputSlotName).DataValue = scaledMatrix;
            }
            catch (Exception e)
            {
                logger.Log(this, LogLevel.Critical, "Error occurred whilst attempting to scale matrix.", e);
                throw;
            }
            metricLogger.Increment(new MatrixFeaturesScaled());
        }
    }
}
