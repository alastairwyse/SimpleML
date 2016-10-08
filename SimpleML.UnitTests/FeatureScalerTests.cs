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
using SimpleML;
using SimpleML.Containers;

namespace SimpleML.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.FeatureScaler.
    /// </summary>
    public class FeatureScalerTests
    {
        private FeatureScaler testFeatureScaler;

        [SetUp]
        protected void SetUp()
        {
            testFeatureScaler = new FeatureScaler();
        }

        /// <summary>
        /// Tests that an exception is thrown if an empty array is passed to the array version of the Scale() method which accepts scaling parameters.
        /// </summary>
        [Test]
        public void ScaleArrayParametersIn_ArrayIs0Length()
        {
            FeatureScalingParameters featureScalingParameters = new FeatureScalingParameters(0.5, 0.25);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testFeatureScaler.Scale(new Double[0], featureScalingParameters);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'features' is an empty array."));
            Assert.AreEqual("features", e.ParamName);
        }

        /// <summary>
        /// Success tests for the array version of the Scale() method which accepts scaling parameters.
        /// </summary>
        [Test]
        public void ScaleArrayParametersIn()
        {
            Double[] featureValues = new Double[] { -2.4, 1.0, 3.56, 6.56 };
            FeatureScalingParameters featureScalingParameters = new FeatureScalingParameters(2.18, 8.96);

            Double[] scaledFeatureValues = testFeatureScaler.Scale(featureValues, featureScalingParameters);

            Assert.That(scaledFeatureValues[0], Is.EqualTo(-0.51116071428571428571428571428571).Within(1e-14));
            Assert.That(scaledFeatureValues[1], Is.EqualTo(-0.13169642857142857142857142857143).Within(1e-14));
            Assert.That(scaledFeatureValues[2], Is.EqualTo(0.15401785714285714285714285714286).Within(1e-14));
            Assert.That(scaledFeatureValues[3], Is.EqualTo(0.48883928571428571428571428571429).Within(1e-14));
        }

        /// <summary>
        /// Tests that an exception is thrown if an empty array is passed to the array version of the Scale() method which generates the scaling parameters.
        /// </summary>
        [Test]
        public void ScaleArrayParametersOut_ArrayIs0Length()
        {
            FeatureScalingParameters featureScalingParameters;

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testFeatureScaler.Scale(new Double[0], out featureScalingParameters);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'features' is an empty array."));
            Assert.AreEqual("features", e.ParamName);
        }

        /// <summary>
        /// Success tests for the array version of the Scale() method which generates the scaling parameters.
        /// </summary>
        [Test]
        public void ScaleArrayParametersOut()
        {
            Double[] featureValues = new Double[] { -2.4, 1.0, 3.56, 6.56 };
            FeatureScalingParameters featureScalingParameters;

            Double[] scaledFeatureValues = testFeatureScaler.Scale(featureValues, out featureScalingParameters);

            Assert.That(scaledFeatureValues[0], Is.EqualTo(-0.51116071428571428571428571428571).Within(1e-14));
            Assert.That(scaledFeatureValues[1], Is.EqualTo(-0.13169642857142857142857142857143).Within(1e-14));
            Assert.That(scaledFeatureValues[2], Is.EqualTo(0.15401785714285714285714285714286).Within(1e-14));
            Assert.That(scaledFeatureValues[3], Is.EqualTo(0.48883928571428571428571428571429).Within(1e-14));
            Assert.That(featureScalingParameters.Mean, Is.EqualTo(2.18).Within(1e-14));
            Assert.That(featureScalingParameters.Span, Is.EqualTo(8.96).Within(1e-14));
        }

        /// <summary>
        /// Tests that an exception is thrown if the matrix version of the Scale() method which accepts scaling parameters is called with a list of scaling parameters whose dimension doesn't match the 'n' dimension of the feature matrix.
        /// </summary>
        [Test]
        public void ScaleMatrixParametersIn_ScalingParametersDoNotMatchArrayNDimension()
        {
            Matrix featureValues = new Matrix(2, 3, new Double[] { 1, 2, 3, 4, 5, 6 });
            List<FeatureScalingParameters> featureScalingParameters = new List<FeatureScalingParameters> 
            {
                new FeatureScalingParameters(1, 2), 
                new FeatureScalingParameters(3, 4) 
            };

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testFeatureScaler.Scale(featureValues, featureScalingParameters);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The number of feature scaling parameters provided (2), does not match the 'n' dimension of the inputted matrix of features (3)."));
            Assert.AreEqual("featureScalingParameters", e.ParamName);
        }

        /// <summary>
        /// Success tests for the matrix version of the Scale() method which accepts scaling parameters.
        /// </summary>
        [Test]
        public void ScaleMatrixParametersIn()
        {
            Matrix featureValues = new Matrix(4, 2, new Double[] { -2.4, 4993.39, 1.0, 4140.96, 3.56, 2375.32, 6.56, 8322.94 });
            List<FeatureScalingParameters> featureScalingParameters = new List<FeatureScalingParameters> 
            {
                new FeatureScalingParameters(2.18, 8.96), 
                new FeatureScalingParameters(4958.1525, 5947.62) 
            };

            Matrix scaledFeatureValues = testFeatureScaler.Scale(featureValues, featureScalingParameters);

            Assert.That(scaledFeatureValues.GetElement(1, 1), Is.EqualTo(-0.51116071428571428571428571428571).Within(1e-14));
            Assert.That(scaledFeatureValues.GetElement(2, 1), Is.EqualTo(-0.13169642857142857142857142857143).Within(1e-14));
            Assert.That(scaledFeatureValues.GetElement(3, 1), Is.EqualTo(0.15401785714285714285714285714286).Within(1e-14));
            Assert.That(scaledFeatureValues.GetElement(4, 1), Is.EqualTo(0.48883928571428571428571428571429).Within(1e-14));
            Assert.That(scaledFeatureValues.GetElement(1, 2), Is.EqualTo(0.0059246387630683).Within(1e-14));
            Assert.That(scaledFeatureValues.GetElement(2, 2), Is.EqualTo(-0.1373982366055666).Within(1e-14));
            Assert.That(scaledFeatureValues.GetElement(3, 2), Is.EqualTo(-0.4342632010787508).Within(1e-14));
            Assert.That(scaledFeatureValues.GetElement(4, 2), Is.EqualTo(0.5657367989212492).Within(1e-14));
        }

        /// <summary>
        /// Success tests for the matrix version of the Scale() method which generates the scaling parameters.
        /// </summary>
        [Test]
        public void ScaleMatrixParametersOut()
        {
            Matrix featureValues = new Matrix(4, 2, new Double[] { -2.4, 4993.39, 1.0, 4140.96, 3.56, 2375.32, 6.56, 8322.94 });
            List<FeatureScalingParameters> featureScalingParameters;

            Matrix scaledFeatureValues = testFeatureScaler.Scale(featureValues, out featureScalingParameters);

            Assert.That(scaledFeatureValues.GetElement(1, 1), Is.EqualTo(-0.51116071428571428571428571428571).Within(1e-14));
            Assert.That(scaledFeatureValues.GetElement(2, 1), Is.EqualTo(-0.13169642857142857142857142857143).Within(1e-14));
            Assert.That(scaledFeatureValues.GetElement(3, 1), Is.EqualTo(0.15401785714285714285714285714286).Within(1e-14));
            Assert.That(scaledFeatureValues.GetElement(4, 1), Is.EqualTo(0.48883928571428571428571428571429).Within(1e-14));
            Assert.That(featureScalingParameters[0].Mean, Is.EqualTo(2.18).Within(1e-2));
            Assert.That(featureScalingParameters[0].Span, Is.EqualTo(8.96).Within(1e-2));
            Assert.That(scaledFeatureValues.GetElement(1, 2), Is.EqualTo(0.0059246387630683).Within(1e-14));
            Assert.That(scaledFeatureValues.GetElement(2, 2), Is.EqualTo(-0.1373982366055666).Within(1e-14));
            Assert.That(scaledFeatureValues.GetElement(3, 2), Is.EqualTo(-0.4342632010787508).Within(1e-14));
            Assert.That(scaledFeatureValues.GetElement(4, 2), Is.EqualTo(0.5657367989212492).Within(1e-14));
            Assert.That(featureScalingParameters[1].Mean, Is.EqualTo(4958.1525).Within(1e-4));
            Assert.That(featureScalingParameters[1].Span, Is.EqualTo(5947.62).Within(1e-2));
        }

        /// <summary>
        /// Success tests for the double version of the Rescale() method.
        /// </summary>
        [Test]
        public void RescaleDouble()
        {
            FeatureScalingParameters featureScalingParameters = new FeatureScalingParameters(2.18, 8.96);

            Double featureValue = testFeatureScaler.Rescale(-0.51116071428571428571428571428571, featureScalingParameters);

            Assert.That(featureValue, Is.EqualTo(-2.4).Within(1e-14));
        }

        /// <summary>
        /// Tests that an exception is thrown if an empty array is passed to the Rescale() method.
        /// </summary>
        [Test]
        public void RescaleArray_ArrayIs0Length()
        {
            FeatureScalingParameters featureScalingParameters = new FeatureScalingParameters(1, 2);

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testFeatureScaler.Rescale(new Double[0], featureScalingParameters);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'scaledFeatures' is an empty array."));
            Assert.AreEqual("scaledFeatures", e.ParamName);
        }

        /// <summary>
        /// Success tests for the array version of the Rescale() method.
        /// </summary>
        [Test]
        public void RescaleArray()
        {
            Double[] scaledFeatureValues = new Double[]
            { 
                -0.51116071428571428571428571428571, 
                -0.13169642857142857142857142857143, 
                0.15401785714285714285714285714286, 
                0.48883928571428571428571428571429
            };
            FeatureScalingParameters featureScalingParameters = new FeatureScalingParameters(2.18, 8.96);

            Double[] featureValues = testFeatureScaler.Rescale(scaledFeatureValues, featureScalingParameters);

            Assert.That(featureValues[0], Is.EqualTo(-2.4).Within(1e-14));
            Assert.That(featureValues[1], Is.EqualTo(1.0).Within(1e-14));
            Assert.That(featureValues[2], Is.EqualTo(3.56).Within(1e-14));
            Assert.That(featureValues[3], Is.EqualTo(6.56).Within(1e-14));
        }

        /// <summary>
        /// Tests that an exception is thrown when the matrix version of the Rescale() method is called with set of feature scaling paramters whose size does not match the 'n' dimension of the matrix.
        /// </summary>
        [Test]
        public void RescaleMatrix_ScalingParametersDontMatchMatrixDimension()
        {
            Matrix scaledFeatureValues = new Matrix(4, 2);

            List<FeatureScalingParameters> featureScalingParameters = new List<FeatureScalingParameters>();
            featureScalingParameters.Add(new FeatureScalingParameters(2.18, 8.96));

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testFeatureScaler.Rescale(scaledFeatureValues, featureScalingParameters);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Size of parameter 'featureScalingParameters' '1' does not match the size of the 'n' dimension of the matrix '2'."));
            Assert.AreEqual("featureScalingParameters", e.ParamName);
        }

        /// <summary>
        /// Success tests for the matrix version of the Rescale() method.
        /// </summary>
        [Test]
        public void RescaleMatrix()
        {
            Double[] scaledFeatureValuesArray = new Double[]
            { 
                -0.51116071428571428571428571428571, 
                0.0059246387630683, 
                -0.13169642857142857142857142857143, 
                -0.1373982366055666, 
                0.15401785714285714285714285714286, 
                -0.4342632010787508, 
                0.48883928571428571428571428571429, 
                0.5657367989212492
            };
            Matrix scaledFeatureValues = new Matrix(4, 2, scaledFeatureValuesArray);

            List<FeatureScalingParameters> featureScalingParameters = new List<FeatureScalingParameters>();
            featureScalingParameters.Add(new FeatureScalingParameters(2.18, 8.96));
            featureScalingParameters.Add(new FeatureScalingParameters(4958.1525, 5947.62));
            
            Matrix featureValues = testFeatureScaler.Rescale(scaledFeatureValues, featureScalingParameters);

            Assert.That(featureValues.GetElement(1, 1), Is.EqualTo(-2.4).Within(1e-14));
            Assert.That(featureValues.GetElement(2, 1), Is.EqualTo(1.0).Within(1e-14));
            Assert.That(featureValues.GetElement(3, 1), Is.EqualTo(3.56).Within(1e-14));
            Assert.That(featureValues.GetElement(4, 1), Is.EqualTo(6.56).Within(1e-14));
            Assert.That(featureValues.GetElement(1, 2), Is.EqualTo(4993.39).Within(1e-14));
            Assert.That(featureValues.GetElement(2, 2), Is.EqualTo(4140.96).Within(1e-14));
            Assert.That(featureValues.GetElement(3, 2), Is.EqualTo(2375.32).Within(1e-14));
            Assert.That(featureValues.GetElement(4, 2), Is.EqualTo(8322.94).Within(1e-14));
        }
    }
}
