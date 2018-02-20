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
using SimpleML.Containers;

namespace SimpleML
{
    /// <summary>
    /// A feed-forward neural network using logistic units, a single hidden layer, and cross-entropy cost function.
    /// </summary>
    /// <remarks>Uses arrays and for loops rather than matrix operations for the forward and backward passes in training, to simplify understanding of the back-propogation process.</remarks>
    public class BasicFeedForwardNeuralNetwork
    {
        /// <summary>The number of input units in the network.</summary>
        private Int32 numberOfInputUnits;
        /// <summary>The number of hidden units in the network.</summary>
        private Int32 numberOfHiddenUnits;
        /// <summary>The number of output units in the network.</summary>
        private Int32 numberOfOutputUnits;
        /// <summary>The values of the weights between the input and hidden layers of the network.  The first dimension represents the input units, and the second dimension the hidden units.</summary>
        private Double[,] inputToHiddenLayerWeights;
        /// <summary>The values of the weights between the input and hidden layers of the network.  The first dimension represents the input units, and the second dimension the hidden units.</summary>
        private Double[,] hiddenToOutputLayerWeights;
        /// <summary>The activation values of the hidden layer units.</summary>
        private Double[] hiddenLayerActivationValues;
        /// <summary>The activation values of the output layer units.</summary>
        private Double[] outputLayerActivationValues;
        /// <summary>The cross entropy cost using the current weight values.</summary>
        private Double cost;
        /// <summary>The history of cost values as a result of training.</summary>
        private List<CostHistoryItem> costHistory;

        /// <summary>
        /// The values of the weights between the input and hidden layers of the network.  The first dimension represents the input units, and the second dimension the hidden units.
        /// </summary>
        public Double[,] InputToHiddenLayerWeights
        {
            get
            {
                return inputToHiddenLayerWeights;
            }
        }

        /// <summary>
        /// The values of the weights between the hidden and output layers of the network.  The first dimension represents the hidden units, and the second dimension the output units.
        /// </summary>
        public Double[,] HiddenToOutputLayerWeights
        {
            get
            {
                return hiddenToOutputLayerWeights;
            }
        }

        /// <summary>
        /// The activation values of the hidden layer units.
        /// </summary>
        public Double[] HiddenLayerActivationValues
        {
            get
            {
                return hiddenLayerActivationValues;
            }
        }

        /// <summary>
        /// The activation values of the output layer units.
        /// </summary>
        public Double[] OutputLayerActivationValues
        {
            get
            {
                return outputLayerActivationValues;
            }
        }

        /// <summary>
        /// The cross entropy cost using the current weight values.
        /// </summary>
        public Double Cost
        {
            get
            {
                return cost;
            }
        }

        /// <summary>
        /// The history of cost values as a result of training.
        /// </summary>
        public List<CostHistoryItem> CostHistory
        {
            get
            {
                return costHistory;
            }
        }

        /// <summary>
        /// Initialises a new instance of the SimpleML.BasicFeedForwardNeuralNetwork class.
        /// </summary>
        /// <param name="numberOfInputUnits">The number of input units in the network.</param>
        /// <param name="numberOfHiddenUnits">The number of hidden units in the network.</param>
        /// <param name="numberOfOutputUnits">The number of output units in the network.</param>
        public BasicFeedForwardNeuralNetwork(Int32 numberOfInputUnits, Int32 numberOfHiddenUnits, Int32 numberOfOutputUnits)
        {
            if (numberOfInputUnits < 1)
            {
                throw new ArgumentException("Parameter 'numberOfInputUnits' must be greater than or equal to 1.", "numberOfInputUnits");
            }
            if (numberOfHiddenUnits < 1)
            {
                throw new ArgumentException("Parameter 'numberOfHiddenUnits' must be greater than or equal to 1.", "numberOfHiddenUnits");
            }
            if (numberOfOutputUnits < 1)
            {
                throw new ArgumentException("Parameter 'numberOfOutputUnits' must be greater than or equal to 1.", "numberOfOutputUnits");
            }
            this.numberOfInputUnits = numberOfInputUnits;
            this.numberOfHiddenUnits = numberOfHiddenUnits;
            this.numberOfOutputUnits = numberOfOutputUnits;
            inputToHiddenLayerWeights = new Double[numberOfHiddenUnits, numberOfInputUnits];
            hiddenToOutputLayerWeights = new Double[numberOfOutputUnits, numberOfHiddenUnits];
            hiddenLayerActivationValues = new Double[numberOfHiddenUnits];
            outputLayerActivationValues = new Double[numberOfOutputUnits];
            cost = 0.0;
            costHistory = new List<CostHistoryItem>();
            InitialiseWeights();
        }

        /// <summary>
        /// Sets the weights in the network to small, random values.
        /// </summary>
        public void InitialiseWeights()
        {
            Random randomNumberGenerator = new Random();

            // Initialise the input unit to hidden unit weights
            for (Int32 i = 0; i < numberOfHiddenUnits; i++)
            {
                for (Int32 j = 0; j < numberOfInputUnits; j++)
                {
                    // This will set each weight to a small, random value
                    inputToHiddenLayerWeights[i, j] = (randomNumberGenerator.NextDouble() * 0.24) - 0.12;
                }
            }

            // Initialise the hidden to output unit weights
            for (Int32 i = 0; i < numberOfOutputUnits; i++)
            {
                for (Int32 j = 0; j < numberOfHiddenUnits; j++)
                {
                    hiddenToOutputLayerWeights[i, j] = (randomNumberGenerator.NextDouble() * 0.24) - 0.12;
                }
            }
        }

        /// <summary>
        /// Trains the network using the specified training data for the specified number of epochs.
        /// </summary>
        /// <param name="trainingData">An array containing the training data.  The first dimension represents each individual training case, and the second dimension represents the input layer values.</param>
        /// <param name="targetValues">An array containing the training target values (labels).  The first dimension represents each individual training case, and the second dimension represents the output layer target values.</param>
        /// <param name="learningRate">The learning rate to use during training.</param>
        /// <param name="batchSize">The number of training cases to evaluate before updating the weights during each iteration through the training set.</param>
        /// <param name="numberOfEpochs">The number of iterations through the complete training set to perform during training.</param>
        /// <param name="keepCostHistory">Whether to record the history of cost values.</param>
        public void Train(Double[,] trainingData, Double[,] targetValues, Double learningRate, Int32 batchSize, Int32 numberOfEpochs, Boolean keepCostHistory)
        {
            // Check the input parameters
            if (trainingData.GetLength(1) != numberOfInputUnits)
            {
                throw new ArgumentException("The second dimension of parameter 'trainingData' must be equal to the number of input units in the network (" + numberOfInputUnits + ").", "trainingData");
            }
            if (trainingData.GetLength(0) == 0)
            {
                throw new ArgumentException("The first dimension of parameter 'trainingData' must be greater than 0.", "trainingData");
            }
            if (targetValues.GetLength(1) != numberOfOutputUnits)
            {
                throw new ArgumentException("The second dimension of parameter 'targetValues' must be equal to the number of output units in the network (" + numberOfOutputUnits + ").", "targetValues");
            }
            if (targetValues.GetLength(0) == 0)
            {
                throw new ArgumentException("The first dimension of parameter 'targetValues' must be greater than 0.", "targetValues");
            }
            if (trainingData.GetLength(0) != targetValues.GetLength(0))
            {
                throw new ArgumentException("The length of the first dimension of parameters 'trainingData' and 'targetValues' must be equal.", "targetValues");
            }
            if (batchSize < 1)
            {
                throw new ArgumentException("Parameter 'batchSize' must be greater than 0.", "batchSize");
            }
            if (batchSize > trainingData.GetLength(0))
            {
                throw new ArgumentException("Parameter 'batchSize' must be greater less than or equal to the number of training cases in parameter 'trainingData'.", "batchSize");
            }
            if (numberOfEpochs < 1)
            {
                throw new ArgumentException("Parameter 'numberOfEpochs' must be greater than or equal to 1.", "numberOfEpochs");
            }

            costHistory.Clear();
            Int32 numberOfTrainingCases = trainingData.GetLength(0);
            Int32 currentTrainingCaseIndex = 0;
            Int32 completedEpochCount = 0;
            Int32 completedBatchCount = 0;

            // Iterate through the training set for the specified number of epochs
            while (completedEpochCount < numberOfEpochs)
            {
                Int32 completedTrainingCasesInCurrentBatchCount = 0;
                Double currentBatchCost = 0.0;

                // Create arrays to hold the gradients for each of the weights
                Double[,] inputToHiddenLayerGradients = new Double[numberOfHiddenUnits, numberOfInputUnits];
                Double[,] hiddenToOutputLayerGradients = new Double[numberOfOutputUnits, numberOfHiddenUnits];

                // Iterate through one batch
                while ((completedEpochCount < numberOfEpochs) && (completedTrainingCasesInCurrentBatchCount < batchSize))
                {
                    // Take the current training and result cases into separate variables
                    Double[] currentTrainingCase = GetArraySlice(currentTrainingCaseIndex, trainingData);
                    Double[] currentTarget = GetArraySlice(currentTrainingCaseIndex, targetValues);

                    // -------------------
                    // Do the forward pass
                    // -------------------
                    // Calculate the activation values for the hidden layer
                    for (Int32 k = 0; k < numberOfHiddenUnits; k++)
                    {
                        Double currentHiddenUnitLogitValue = 0.0;

                        // Sum up the products of the input unit and the corresponding weight which feeds into the current hidden unit
                        for (Int32 m = 0; m < numberOfInputUnits; m++)
                        {
                            currentHiddenUnitLogitValue += currentTrainingCase[m] * inputToHiddenLayerWeights[k, m];
                        }
                        // Apply the activation function, and store the value
                        hiddenLayerActivationValues[k] = ApplySigmoidFunction(currentHiddenUnitLogitValue);
                    }

                    // Calculate the activation value for the output layer
                    for (Int32 k = 0; k < numberOfOutputUnits; k++)
                    {
                        Double currentOutputUnitLogitValue = 0.0;

                        for (Int32 m = 0; m < numberOfHiddenUnits; m++)
                        {
                            currentOutputUnitLogitValue += hiddenLayerActivationValues[m] * hiddenToOutputLayerWeights[k, m];
                        }
                        outputLayerActivationValues[k] = ApplySigmoidFunction(currentOutputUnitLogitValue);
                    }

                    // Calculate the cost for this training case
                    Double currentCost = CalculateCost(currentTarget, outputLayerActivationValues);
                    currentBatchCost += currentCost;

                    // --------------------
                    // Do the backward pass
                    // --------------------

                    // Calculate the hidden to output layer gradients
                    for (Int32 k = 0; k < numberOfOutputUnits; k++)
                    {
                        for (Int32 m = 0; m < numberOfHiddenUnits; m++)
                        {
                            hiddenToOutputLayerGradients[k, m] += (outputLayerActivationValues[k] - currentTarget[k]) * hiddenLayerActivationValues[m];
                        }
                    }

                    // Calculate the input to hidden layer gradients
                    for (Int32 k = 0; k < numberOfHiddenUnits; k++)
                    {
                        for (Int32 m = 0; m < numberOfInputUnits; m++)
                        {
                            Double currentHiddenToOutputLayerGradient = 0.0;

                            for (Int32 n = 0; n < numberOfOutputUnits; n++)
                            {
                                currentHiddenToOutputLayerGradient += ((outputLayerActivationValues[n] - currentTarget[n]) * hiddenToOutputLayerWeights[n, k] * (hiddenLayerActivationValues[k] * (1 - hiddenLayerActivationValues[k])) * currentTrainingCase[m]);
                            }

                            inputToHiddenLayerGradients[k, m] += currentHiddenToOutputLayerGradient;
                        }
                    }

                    completedTrainingCasesInCurrentBatchCount++;
                    if (currentTrainingCaseIndex == (numberOfTrainingCases - 1))
                    {
                        currentTrainingCaseIndex = 0;
                        completedEpochCount++;
                    }
                    else
                    {
                        currentTrainingCaseIndex++;
                    }
                }

                completedBatchCount++;

                // Average the cost by the number of training cases in the current batch
                cost = currentBatchCost / Convert.ToDouble(completedTrainingCasesInCurrentBatchCount);
                if (keepCostHistory == true)
                {
                    costHistory.Add(new CostHistoryItem(completedBatchCount, cost));
                }

                // Average the gradients by the number of training cases in the current batch
                DivideArrayElements(inputToHiddenLayerGradients, Convert.ToDouble(completedTrainingCasesInCurrentBatchCount));
                DivideArrayElements(hiddenToOutputLayerGradients, Convert.ToDouble(completedTrainingCasesInCurrentBatchCount));

                // Update the input to hidden layer weights
                for (Int32 j = 0; j < inputToHiddenLayerWeights.GetLength(0); j++)
                {
                    for (Int32 k = 0; k < inputToHiddenLayerWeights.GetLength(1); k++)
                    {
                        inputToHiddenLayerWeights[j, k] -= inputToHiddenLayerGradients[j, k] * learningRate;
                    }
                }

                // Update the hidden to output layer weights
                for (Int32 j = 0; j < hiddenToOutputLayerWeights.GetLength(0); j++)
                {
                    for (Int32 k = 0; k < hiddenToOutputLayerWeights.GetLength(1); k++)
                    {
                        hiddenToOutputLayerWeights[j, k] -= hiddenToOutputLayerGradients[j, k] * learningRate;
                    }
                }
            }
        }

        /// <summary>
        /// Predicts output values for the specified input data based on the current weights in the network.
        /// </summary>
        /// <param name="inputData">An array containing the input data.  The first dimension represents each individual data item, and the second dimension represents the individual input layer values for the data item.</param>
        /// <returns>A 2 dimensional array containing the predicted values.  The first dimension contains an entry for each individual item in the input data.  The second dimension represents each output layer unit.</returns>
        public Double[,] Predict(Double[,] inputData)
        {
            if (inputData.GetLength(1) != numberOfInputUnits)
            {
                throw new ArgumentException("The second dimension of parameter 'inputData' must be equal to the number of input units in the network (" + numberOfInputUnits + ").", "inputData");
            }
            if (inputData.GetLength(0) == 0)
            {
                throw new ArgumentException("The first dimension of parameter 'inputData' must be greater than 0.", "inputData");
            }

            Double[,] predictedValues = new Double[inputData.GetLength(0), numberOfOutputUnits];

            for (Int32 i = 0; i < inputData.GetLength(0); i++)
            {
                // Calculate the activation values for the hidden layer
                for (Int32 k = 0; k < numberOfHiddenUnits; k++)
                {
                    Double currentHiddenUnitLogitValue = 0.0;

                    // Sum up the products of the input unit and the corresponding weight which feeds into the current hidden unit
                    for (Int32 m = 0; m < numberOfInputUnits; m++)
                    {
                        currentHiddenUnitLogitValue += inputData[i, m] * inputToHiddenLayerWeights[k, m];
                    }
                    // Apply the activation function, and store the value
                    hiddenLayerActivationValues[k] = ApplySigmoidFunction(currentHiddenUnitLogitValue);
                }

                // Calculate the activation value for the output layer
                for (Int32 k = 0; k < numberOfOutputUnits; k++)
                {
                    Double currentOutputUnitLogitValue = 0.0;

                    for (Int32 m = 0; m < numberOfHiddenUnits; m++)
                    {
                        currentOutputUnitLogitValue += hiddenLayerActivationValues[m] * hiddenToOutputLayerWeights[k, m];
                    }
                    outputLayerActivationValues[k] = ApplySigmoidFunction(currentOutputUnitLogitValue);
                    predictedValues[i, k] = ApplySigmoidFunction(currentOutputUnitLogitValue);
                }
            }

            return predictedValues;
        }

        /// <summary>
        /// Retrieves a 'slice' from the first dimension of a 2 dimensional array.
        /// </summary>
        /// <remarks>Used to retrieve a single training or target case from a set of cases.</remarks>
        /// <param name="index">The index of the first dimension of the array to retrieve.</param>
        /// <param name="inputArray">The array to retrive the slice from.</param>
        /// <returns>The array slice.</returns>
        private Double[] GetArraySlice(Int32 index, Double[,] inputArray)
        {
            Double[] returnArray = new Double[inputArray.GetLength(1)];
            for (Int32 i = 0; i < returnArray.Length; i++)
            {
                returnArray[i] = inputArray[index, i];
            }
            return returnArray;
        }

        /// <summary>
        /// Applies the sigmoid activation function to the inputted number.
        /// </summary>
        /// <param name="inputValue">The number to apply the sigmoid function to.</param>
        /// <returns>The number with the sigmoid function applied.</returns>
        private Double ApplySigmoidFunction(Double inputValue)
        {
            return 1.0 / (1.0 + Math.Exp(-inputValue));
        }

        /// <summary>
        /// Calculates the cost of the specified output layer values with regard to the specified target values, using the cross entropy cost function.
        /// </summary>
        /// <param name="targetValues">The target values for the output layer.</param>
        /// <param name="outputLayerActivationValues">The actual activation values of the output layer.</param>
        /// <returns>The cross entropy cost.</returns>
        private Double CalculateCost(Double[] targetValues, Double[] outputLayerActivationValues)
        {
            Double cost = 0.0;
            for (Int32 i = 0; i < targetValues.Length; i++)
            {
                cost = cost + (targetValues[i] * Math.Log(outputLayerActivationValues[i])) + ((1 - targetValues[i]) * Math.Log(1 - outputLayerActivationValues[i]));
            }
            return -cost;
        }

        /// <summary>
        /// Divides all elements of the specified 2 dimensional array of doubles by the specified number.
        /// </summary>
        /// <param name="inputArray">The array to divide the elements of.</param>
        /// <param name="divisor">The number to divide by.</param>
        private void DivideArrayElements(Double[,] inputArray, Double divisor)
        {
            for (Int32 i = 0; i < inputArray.GetLength(0); i++)
            {
                for (Int32 j = 0; j < inputArray.GetLength(1); j++)
                {
                    inputArray[i, j] = inputArray[i, j] / divisor;
                }
            }
        }
    }
}
