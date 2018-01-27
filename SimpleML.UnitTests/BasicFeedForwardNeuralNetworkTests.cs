/*
 * Copyright 2017 Alastair Wyse (http://www.oraclepermissiongenerator.net/simpleml/)
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

namespace SimpleML.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.BasicFeedForwardNeuralNetwork.
    /// </summary>
    public class BasicFeedForwardNeuralNetworkTests
    {
        private BasicFeedForwardNeuralNetwork testBasicFeedForwardNeuralNetwork;

        [SetUp]
        protected void SetUp()
        {
            testBasicFeedForwardNeuralNetwork = new BasicFeedForwardNeuralNetwork(2, 5, 1);
        }

        /// <summary>
        /// Tests that an exception is thrown if the constructor is called with a 'numberOfInputUnits' parameter of 0.
        /// </summary>
        [Test]
        public void Constructor_0InputUnits()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testBasicFeedForwardNeuralNetwork = new BasicFeedForwardNeuralNetwork(0, 5, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'numberOfInputUnits' must be greater than or equal to 1."));
            Assert.AreEqual("numberOfInputUnits", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the constructor is called with a 'numberOfHiddenUnits' parameter of 0.
        /// </summary>
        [Test]
        public void Constructor_0HiddenUnits()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testBasicFeedForwardNeuralNetwork = new BasicFeedForwardNeuralNetwork(2, 0, 1);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'numberOfHiddenUnits' must be greater than or equal to 1."));
            Assert.AreEqual("numberOfHiddenUnits", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the constructor is called with a 'numberOfOutputUnits' parameter of 0.
        /// </summary>
        [Test]
        public void Constructor_0OutputUnits()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testBasicFeedForwardNeuralNetwork = new BasicFeedForwardNeuralNetwork(2, 5, 0);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'numberOfOutputUnits' must be greater than or equal to 1."));
            Assert.AreEqual("numberOfOutputUnits", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Train() method is called with a 'trainingData' parameter with incorrect second dimension size.
        /// </summary>
        [Test]
        public void Train_TrainingDataSecondDimensionMismatch()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testBasicFeedForwardNeuralNetwork.Train(new Double[4, 3], new Double[4, 1], 1.0, 2, 200, false);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The second dimension of parameter 'trainingData' must be equal to the number of input units in the network (2)."));
            Assert.AreEqual("trainingData", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Train() method is called with a 'trainingData' parameter with 0 first dimension size.
        /// </summary>
        [Test]
        public void Train_TrainingDataFirstDimension0()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testBasicFeedForwardNeuralNetwork.Train(new Double[0, 2], new Double[4, 1], 1.0, 2, 200, false);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The first dimension of parameter 'trainingData' must be greater than 0."));
            Assert.AreEqual("trainingData", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Train() method is called with a 'targetValues' parameter with incorrect second dimension size.
        /// </summary>
        [Test]
        public void Train_TargetValuesSecondDimensionMismatch()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testBasicFeedForwardNeuralNetwork.Train(new Double[4, 2], new Double[4, 2], 1.0, 2, 200, false);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The second dimension of parameter 'targetValues' must be equal to the number of output units in the network (1)."));
            Assert.AreEqual("targetValues", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Train() method is called with a 'targetValues' parameter with 0 first dimension size.
        /// </summary>
        [Test]
        public void Train_TargetValuesFirstDimension0()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testBasicFeedForwardNeuralNetwork.Train(new Double[4, 2], new Double[0, 1], 1.0, 2, 200, false);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The first dimension of parameter 'targetValues' must be greater than 0."));
            Assert.AreEqual("targetValues", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Train() method is called with 'trainingData' and 'targetValues' parameters with differing second dimensions.
        /// </summary>
        [Test]
        public void Train_TrainingDataAndTargetValuesFirstDimensionMismatch()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testBasicFeedForwardNeuralNetwork.Train(new Double[4, 2], new Double[5, 1], 1.0, 2, 200, false);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The length of the first dimension of parameters 'trainingData' and 'targetValues' must be equal."));
            Assert.AreEqual("targetValues", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Train() method is called with 'batchSize' parameter of 0.
        /// </summary>
        [Test]
        public void Train_BatchSize0()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testBasicFeedForwardNeuralNetwork.Train(new Double[4, 2], new Double[4, 1], 1.0, 0, 200, false);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'batchSize' must be greater than 0."));
            Assert.AreEqual("batchSize", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Train() method is called with 'batchSize' parameter greater than the number of training cases.
        /// </summary>
        [Test]
        public void Train_BatchSizeGreaterThanTrainingCases()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testBasicFeedForwardNeuralNetwork.Train(new Double[4, 2], new Double[4, 1], 1.0, 5, 200, false);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'batchSize' must be greater less than or equal to the number of training cases in parameter 'trainingData'."));
            Assert.AreEqual("batchSize", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Train() method is called with 'numberOfEpochs' parameter of 0.
        /// </summary>
        [Test]
        public void Train_NumberOfEpochs0()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testBasicFeedForwardNeuralNetwork.Train(new Double[4, 2], new Double[4, 1], 1.0, 4, 0, false);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("Parameter 'numberOfEpochs' must be greater than or equal to 1."));
            Assert.AreEqual("numberOfEpochs", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Predict() method is called with an 'inputData' parameter with incorrect second dimension size.
        /// </summary>
        [Test]
        public void Predict_InputDataSecondDimensionMismatch()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testBasicFeedForwardNeuralNetwork.Predict(new Double[4, 3]);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The second dimension of parameter 'inputData' must be equal to the number of input units in the network (2)."));
            Assert.AreEqual("inputData", e.ParamName);
        }

        /// <summary>
        /// Tests that an exception is thrown if the Predict() method is called with an 'inputData' parameter with 0 first dimension size.
        /// </summary>
        [Test]
        public void Predict_InputDataFirstDimension0()
        {
            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testBasicFeedForwardNeuralNetwork.Predict(new Double[0, 2]);
            });

            Assert.That(e.Message, NUnit.Framework.Does.StartWith("The first dimension of parameter 'inputData' must be greater than 0."));
            Assert.AreEqual("inputData", e.ParamName);
        }
    }
}
