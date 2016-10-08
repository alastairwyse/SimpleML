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
    /// An MMF module which generates polynomial features for data series stored column-wise in a matrix.
    /// </summary>
    public class PolynomialFeatureGenerator : ModuleBase
    {
        private const String dataSeriesInputSlotName = "DataSeries";
        private const String polynomialDegreeInputSlotName = "PolynomialDegree";
        private const String outputMatrixOutputSlotName = "OutputMatrix";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Samples.Modules.PolynomialFeatureGenerator class.
        /// </summary>
        public PolynomialFeatureGenerator()
            : base()
        {
            Description = "Generates polynomial features for data series stored column-wise in a SimpleML.Containers.Matrix class";
            AddInputSlot(dataSeriesInputSlotName, "The data series to generate the polynomial features for, stored column-wise in a matrix", typeof(Matrix));
            AddInputSlot(polynomialDegreeInputSlotName, "The degree of polynomial features to generate", typeof(Int32));
            AddOutputSlot(outputMatrixOutputSlotName, "The data series with polynomial features added (stored column-wise)", typeof(Matrix));
        }

        protected override void ImplementProcess()
        {
            Matrix dataSeries = (Matrix)GetInputSlot(dataSeriesInputSlotName).DataValue;
            Int32 polynomialDegree = (Int32)GetInputSlot(polynomialDegreeInputSlotName).DataValue;
            Matrix outputMatrix = null;

            try
            {
                SimpleML.PolynomialFeatureGenerator polynomialFeatureGenerator = new SimpleML.PolynomialFeatureGenerator();
                outputMatrix = polynomialFeatureGenerator.GenerateFeatures(dataSeries, polynomialDegree);
                GetOutputSlot(outputMatrixOutputSlotName).DataValue = outputMatrix;
            }
            catch (Exception e)
            {
                logger.Log(this, LogLevel.Critical, "Error occurred whilst generating polynomial features for data series.", e);
                throw;
            }
            logger.Log(this, LogLevel.Information, "Generated polynomial features for data series, producing a matrix with " + outputMatrix.NDimension + " columns.");
        }
    }
}
