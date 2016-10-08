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
using MathematicsModularFramework;
using SimpleML.Containers;

namespace SimpleML.Samples.Modules
{
    /// <summary>
    /// An MMF module which scales the columns of a matrix to values between -1 and 1.
    /// </summary>
    public class AutomaticFeatureScaler : ModuleBase
    {
        private const String inputMatrixInputSlotName = "InputMatrix";
        private const String scaledMatrixOutputSlotName = "ScaledMatrix";
        private const String featureScalingParametersOutputSlotName = "FeatureScalingParameters";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.AutomaticFeatureScaler class.
        /// </summary>
        public AutomaticFeatureScaler()
            : base()
        {
            Description = "Scales the columns of a SimpleML.Containers.Matrix class to values between -1 and 1";
            AddInputSlot(inputMatrixInputSlotName, "The matrix to scale the columns of", typeof(Matrix));
            AddOutputSlot(scaledMatrixOutputSlotName, "The matrix with columns scaled", typeof(Matrix));
            AddOutputSlot(featureScalingParametersOutputSlotName, "The parameter values used for feature scaling", typeof(List<FeatureScalingParameters>));
        }

        protected override void ImplementProcess()
        {
            Matrix inputMatrix = (Matrix)GetInputSlot(inputMatrixInputSlotName).DataValue;

            SimpleML.FeatureScaler featureScaler = new SimpleML.FeatureScaler();
            List<FeatureScalingParameters> featureScalingParameters = new List<FeatureScalingParameters>();
            Matrix scaledMatrix = featureScaler.Scale(inputMatrix, out featureScalingParameters);
            metricLogger.Increment(new MatrixFeaturesScaled());

            GetOutputSlot(scaledMatrixOutputSlotName).DataValue = scaledMatrix;
            GetOutputSlot(featureScalingParametersOutputSlotName).DataValue = featureScalingParameters;
        }
    }
}
