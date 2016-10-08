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
    /// Unit tests for class SimpleML.PolynomialFeatureGenerator.
    /// </summary>
    public class PolynomialFeatureGeneratorTests
    {
        private PolynomialFeatureGenerator testPolynomialFeatureGenerator;

        [SetUp]
        protected void SetUp()
        {
            testPolynomialFeatureGenerator = new PolynomialFeatureGenerator();
        }

        /// <summary>
        /// Tests that an exception is thrown when the GenerateFeatures() method is called with an invalid 'polynomialDegree' parameter.
        /// </summary>
        [Test]
        public void GenerateFeatures_InvalidDegreeParameter()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testPolynomialFeatureGenerator.GenerateFeatures(new Matrix(1, 1), 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'polynomialDegree' must be greater than 1."));
            Assert.AreEqual("polynomialDegree", e.ParamName);
        }

        /// <summary>
        /// Success tests for the GenerateFeatures() method.
        /// </summary>
        [Test]
        public void GenerateFeatures()
        {
            Double[] matrixValues = new Double[]
            { 
                2, 
                0, 
                -124.5, 
                0.457
            };
            Matrix data = new Matrix(4, 1, matrixValues);

            Matrix result = testPolynomialFeatureGenerator.GenerateFeatures(data, 2);

            Assert.AreEqual(2, result.GetElement(1, 1));
            Assert.AreEqual(0, result.GetElement(2, 1));
            Assert.That(result.GetElement(3, 1), Is.EqualTo(-124.5).Within(1e-14));
            Assert.That(result.GetElement(4, 1), Is.EqualTo(0.457).Within(1e-14));
            Assert.AreEqual(4, result.GetElement(1, 2));
            Assert.AreEqual(0, result.GetElement(2, 2));
            Assert.That(result.GetElement(3, 2), Is.EqualTo(15500.25).Within(1e-14));
            Assert.That(result.GetElement(4, 2), Is.EqualTo(0.208849).Within(1e-14));

            matrixValues = new Double[]
            { 
                2, -0.051,
                0, 7, 
                -124.5, 3.14, 
                0.457, 0
            };
            data = new Matrix(4, 2, matrixValues);
            
            result = testPolynomialFeatureGenerator.GenerateFeatures(data, 2);

            Assert.AreEqual(2, result.GetElement(1, 1));
            Assert.AreEqual(0, result.GetElement(2, 1));
            Assert.That(result.GetElement(3, 1), Is.EqualTo(-124.5).Within(1e-14));
            Assert.That(result.GetElement(4, 1), Is.EqualTo(0.457).Within(1e-14));
            Assert.That(result.GetElement(1, 2), Is.EqualTo(-0.051).Within(1e-14));
            Assert.AreEqual(7, result.GetElement(2, 2));
            Assert.That(result.GetElement(3, 2), Is.EqualTo(3.14).Within(1e-14));
            Assert.AreEqual(0, result.GetElement(4, 2));
            Assert.AreEqual(4, result.GetElement(1, 3));
            Assert.AreEqual(0, result.GetElement(2, 3));
            Assert.That(result.GetElement(3, 3), Is.EqualTo(15500.25).Within(1e-14));
            Assert.That(result.GetElement(4, 3), Is.EqualTo(0.208849).Within(1e-14));
            Assert.That(result.GetElement(1, 4), Is.EqualTo(-0.102).Within(1e-14));
            Assert.AreEqual(0, result.GetElement(2, 4));
            Assert.That(result.GetElement(3, 4), Is.EqualTo(-390.93).Within(1e-14));
            Assert.AreEqual(0, result.GetElement(4, 4));
            Assert.That(result.GetElement(1, 5), Is.EqualTo(0.002601).Within(1e-14));
            Assert.AreEqual(49, result.GetElement(2, 5));
            Assert.That(result.GetElement(3, 5), Is.EqualTo(9.8596).Within(1e-14));
            Assert.AreEqual(0, result.GetElement(4, 5));
        }
    }
}
