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
using System.IO;
using System.Xml;
using System.Threading;
using MathematicsModularFramework;
using MathematicsModularFramework.Serialization;
using ApplicationLogging;
using ApplicationMetrics;
using SimpleML.Containers;
using SimpleML.Containers.Serialization;
using SimpleML.Samples.Modules;

namespace SimpleML.Samples
{
    class Program
    {
        private const String logFilePath = @"C:\Temp";

        static void Main(string[] args)
        {
            // ---------------------------------------------------------------------------------------------
            // First create a very simple module graph to demonstrate the basic concepts and functionlity...
            // ---------------------------------------------------------------------------------------------
            
            // Create the modules
            RandomIntegerGenerator randomIntegerGenerator = new RandomIntegerGenerator();
            IntegerDivider integerDivider = new IntegerDivider();
            DoubleDivider doubleDivider = new DoubleDivider();
            // Add the modules to the module graph
            ModuleGraph moduleGraph = new ModuleGraph();
            moduleGraph.AddModule(randomIntegerGenerator);
            moduleGraph.AddModule(integerDivider);
            moduleGraph.AddModule(doubleDivider);
            // Create the slot links
            moduleGraph.CreateSlotLink(randomIntegerGenerator.GetOutputSlot("LargerInteger"), integerDivider.GetInputSlot("Dividend"));
            moduleGraph.CreateSlotLink(randomIntegerGenerator.GetOutputSlot("SmallerInteger"), integerDivider.GetInputSlot("Divisor"));
            moduleGraph.CreateSlotLink(randomIntegerGenerator.GetOutputSlot("LargerInteger"), doubleDivider.GetInputSlot("Dividend"));
            moduleGraph.CreateSlotLink(randomIntegerGenerator.GetOutputSlot("SmallerInteger"), doubleDivider.GetInputSlot("Divisor"));
            // Create the module graph processor and process the graph
            using (ModuleGraphProcessor processor = new ModuleGraphProcessor())
            {
                processor.Process(moduleGraph, false);
            }
            // Output the results
            Console.WriteLine("LargerInteger: " + randomIntegerGenerator.GetOutputSlot("LargerInteger").DataValue);
            Console.WriteLine("SmallerInteger: " + randomIntegerGenerator.GetOutputSlot("SmallerInteger").DataValue);
            Console.WriteLine("Quotient: " + integerDivider.GetOutputSlot("Quotient").DataValue);
            Console.WriteLine("Remainder: " + integerDivider.GetOutputSlot("Remainder").DataValue);
            Console.WriteLine("Double Result: " + doubleDivider.GetOutputSlot("Result").DataValue);
            Console.WriteLine();
            Console.WriteLine();


            // ------------------------------------------------------
            // Load some more complex workflows from XML documents...
            // ------------------------------------------------------
            
            // Setup custom serialization for matrices and lists
            ContainerObjectXmlSerializer containerObjectXmlSerializer = new ContainerObjectXmlSerializer();
            XmlDataSerializer xmlDataSerializer = new XmlDataSerializer();
            xmlDataSerializer.AddDataTypeSupport
            (
                typeof(Matrix),
                (Object inputObject, XmlWriter xmlWriter) => { containerObjectXmlSerializer.SerializeMatrix((Matrix)inputObject, xmlWriter); },
                (XmlReader xmlReader) => { return containerObjectXmlSerializer.DeserializeMatrix(xmlReader); }
            );
            xmlDataSerializer.AddDataTypeSupport
            (
                typeof(List<Double>),
                (Object inputObject, XmlWriter xmlWriter) => 
                {
                    List<Double> inputList = (List<Double>)inputObject;
                    foreach(Double currentDouble in inputList)
                    {
                        xmlWriter.WriteStartElement("Element");
                        // Use the XmlDataSerializer to serialize each element
                        xmlDataSerializer.SerializeDataValue(typeof(Double), currentDouble, xmlWriter);
                        xmlWriter.WriteEndElement();
                    }
                },
                (XmlReader xmlReader) => 
                {
                    List<Double> returnList = new List<Double>();
                    while (xmlReader.IsStartElement("Element") == true)
                    {
                        xmlReader.ReadStartElement("Element");
                        // Use the XmlDataSerializer to deserialize each element
                        Double currentElement = (Double)xmlDataSerializer.DeserializeDataValue(typeof(Double), xmlReader);
                        returnList.Add(currentElement);
                        xmlReader.ReadEndElement();
                    }
                    return returnList;
                }
            );
            xmlDataSerializer.AddDataTypeSupport
            (
                typeof(List<Int32>),
                (Object inputObject, XmlWriter xmlWriter) =>
                {
                    List<Int32> inputList = (List<Int32>)inputObject;
                    foreach (Int32 currentInt in inputList)
                    {
                        xmlWriter.WriteStartElement("Element");
                        // Use the XmlDataSerializer to serialize each element
                        xmlDataSerializer.SerializeDataValue(typeof(Int32), currentInt, xmlWriter);
                        xmlWriter.WriteEndElement();
                    }
                },
                (XmlReader xmlReader) =>
                {
                    List<Int32> returnList = new List<Int32>();
                    while (xmlReader.IsStartElement("Element") == true)
                    {
                        xmlReader.ReadStartElement("Element");
                        // Use the XmlDataSerializer to deserialize each element
                        Int32 currentElement = (Int32)xmlDataSerializer.DeserializeDataValue(typeof(Int32), xmlReader);
                        returnList.Add(currentElement);
                        xmlReader.ReadEndElement();
                    }
                    return returnList;
                }
            );
            
            // Following code gives an example of how to serialize a module graph...
            /*
            String graphFilePath = Path.Combine(logFilePath, "SimpleMLSampleGraph.xml");
            using (FileStream serializedGraphStream = new FileStream(graphFilePath, FileMode.Create))
            using (StreamWriter serializedGraphStreamWriter = new StreamWriter(serializedGraphStream))
            {

                ModuleGraphXmlSerializer moduleGraphXmlSerializer = new ModuleGraphXmlSerializer(xmlDataSerializer);
                moduleGraphXmlSerializer.Serialize(BuildLinearRegressionModuleGraph(), serializedGraphStreamWriter);
            }
            */

            // Uncomment lines below to process the various module graphs...
            //ProcessGraph("Basic Linear Regression.xml", "Basic Linear Regression without Feature Scaling", xmlDataSerializer, "Results");
            //ProcessGraph("Linear Regression with Feature Scaling.xml", "Linear Regression with Feature Scaling", xmlDataSerializer, "RescaledMatrix");
            //ProcessGraph("Linear Regression with Feature Scaling and Test Data Cost.xml", "Linear Regression with Feature Scaling, Cost Calculated against Test Data", xmlDataSerializer, "RescaledMatrix");
            //ProcessGraph("Linear Regression with Polynomial Features.xml", "Linear Regression with Polynomial Features", xmlDataSerializer, "RescaledMatrix");
            //ProcessGraph("Linear Regression with Polynomial Features and Test Data Cost.xml", "Linear Regression with Polynomial Features, Cost Calculated against Test Data", xmlDataSerializer, "RescaledMatrix");
        }

        /// <summary>
        /// Opens and processes a module graph which is serialized in a file at the specified path.
        /// </summary>
        /// <remarks>Assumes the two end point modules of the graph are a LinearRegressionCostSeriesCalculator, and either a FeatureRescaler or LinearRegressionHypothesisCalculator module.</remarks>
        /// <param name="graphFileName">The name of the file containing the serialized module graph.</param>
        /// <param name="consoleTitle">A title to write to the console before processing the graph.</param>
        /// <param name="xmlDataSerializer">The XML data serializer to use when deserializing the module graph.</param>
        /// <param name="resultOutputSlotName">The name of the end point module output slot which contains </param>
        private static void ProcessGraph(String graphFileName, String consoleTitle, XmlDataSerializer xmlDataSerializer, String resultOutputSlotName)
        {
            ModuleGraph moduleGraph;

            // Deserialize the module graph from the specified XML file
            String resourcesPath = Path.Combine(Environment.CurrentDirectory, @"..\..\", "Resources");
            using (FileStream serializedGraphStream = new FileStream(Path.Combine(resourcesPath, graphFileName), FileMode.Open))
            using (StreamReader serializedGraphStreamReader = new StreamReader(serializedGraphStream))
            {
                ModuleGraphXmlSerializer moduleGraphXmlSerializer = new ModuleGraphXmlSerializer(xmlDataSerializer);
                moduleGraph = moduleGraphXmlSerializer.Deserialize(serializedGraphStreamReader);
            }

            // Get references to the two end point modules
            LinearRegressionCostSeriesCalculator costSeriesCalculator = (LinearRegressionCostSeriesCalculator)moduleGraph.EndPoints.ElementAt<IModule>(0);
            IModule resultModule = (IModule)moduleGraph.EndPoints.ElementAt<IModule>(1);

            // Process the module graph
            Console.WriteLine("-- " + consoleTitle + " --");
            using (FileApplicationLogger logger = new FileApplicationLogger(LogLevel.Information, '|', "  ", Path.Combine(logFilePath, "SimpleMLSampleLog.txt")))
            using (SizeLimitedBufferProcessor bufferProcessor = new SizeLimitedBufferProcessor(10))
            using (FileMetricLogger metricLogger = new FileMetricLogger('|', Path.Combine(logFilePath, "SimpleMLSampleMetrics.txt"), bufferProcessor, true))
            {
                bufferProcessor.Start();
                try
                {
            using (ModuleGraphProcessor processor = new ModuleGraphProcessor(logger, metricLogger))
                    processor.Process(moduleGraph, false);
                }
                finally
                {
                    bufferProcessor.Stop();
                }
            }

            Console.WriteLine("Cost calculated as " + costSeriesCalculator.GetOutputSlot("Cost").DataValue);

            Console.WriteLine("-- Rescaled hypothesis results --");
            Matrix results = (Matrix)resultModule.GetOutputSlot(resultOutputSlotName).DataValue;
            for (Int32 i = 1; i <= results.MDimension; i++)
            {
                for (Int32 j = 1; j <= results.NDimension; j++)
                {
                    Console.Write(results.GetElement(i, j) + "\t");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// Opens and processes the logistic regression module graph.
        /// </summary>
        /// <param name="xmlDataSerializer">The XML data serializer to use when deserializing the module graph.</param>
        private static void ProcessLogisticRegressionGraph(XmlDataSerializer xmlDataSerializer)
        {
            ModuleGraph moduleGraph;

            // Deserialize the module graph
            String resourcesPath = Path.Combine(Environment.CurrentDirectory, @"..\..\", "Resources");
            using (FileStream serializedGraphStream = new FileStream(Path.Combine(resourcesPath, "Logistic Regression Regularization Comparison.xml"), FileMode.Open))
            using (StreamReader serializedGraphStreamReader = new StreamReader(serializedGraphStream))
            {
                ModuleGraphXmlSerializer moduleGraphXmlSerializer = new ModuleGraphXmlSerializer(xmlDataSerializer);
                moduleGraph = moduleGraphXmlSerializer.Deserialize(serializedGraphStreamReader);
            }

            // Get the error rate calculator and trainer modules from the graph
            MultiParameterErrorRateCalculator errorRateCalculator = (MultiParameterErrorRateCalculator)moduleGraph.EndPoints.ElementAt<IModule>(0);
            OutputSlot multiTrainerOutputSlot = moduleGraph.GetOutputSlotLinkedToInputSlot(errorRateCalculator.GetInputSlot("ThetaParameterSet"));
            MultiParameterTrainer trainer = (MultiParameterTrainer)multiTrainerOutputSlot.Module;

            // Process the module graph
            using (FileApplicationLogger logger = new FileApplicationLogger(LogLevel.Information, '|', "  ", Path.Combine(logFilePath, "SimpleMLSampleLog.txt")))
            using (SizeLimitedBufferProcessor bufferProcessor = new SizeLimitedBufferProcessor(10))
            using (FileMetricLogger metricLogger = new FileMetricLogger('|', Path.Combine(logFilePath, "SimpleMLSampleMetrics.txt"), bufferProcessor, true))
            using (ModuleGraphProcessor processor = new ModuleGraphProcessor(logger, metricLogger))
            {
                bufferProcessor.Start();
                try
                {
                    processor.Process(moduleGraph, false);
                }
                finally
                {
                    bufferProcessor.Stop();
                }
            }

            // Output the results
            List<Double> errorRates = (List<Double>)errorRateCalculator.GetOutputSlot("ErrorRateSet").DataValue;
            List<Matrix> thetaParameterSet = (List<Matrix>)trainer.GetOutputSlot("OptimizedThetaParameterSet").DataValue;
            for (Int32 i = 0; i < errorRates.Count; i++)
            {
                Matrix currentThetaParameters = thetaParameterSet[i];
                Console.WriteLine("For theta parameters...");
                for (Int32 j = 1; j <= currentThetaParameters.MDimension; j++)
                {
                    for (Int32 k = 1; k <= currentThetaParameters.NDimension; k++)
                    {
                        Console.WriteLine("  " + currentThetaParameters.GetElement(j, k));
                    }
                }
                Double currentErrorRate = errorRates[i] * 100;
                Console.WriteLine("Error rate = " + currentErrorRate.ToString("F2") + "%");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Processes and subsequently cancels processing of a module graph
        /// </summary>
        /// <param name="graphFileName">The name of the file containing the serialized module graph.</param>
        /// <param name="consoleTitle">A title to write to the console before processing the graph.</param>
        /// <param name="xmlDataSerializer">The XML data serializer to use when deserializing the module graph.</param>
        private static void ProcessAndCancelGraph(String graphFileName, String consoleTitle, XmlDataSerializer xmlDataSerializer)
        {
            ModuleGraph moduleGraph;

            // Deserialize the module graph from the specified XML file
            String resourcesPath = Path.Combine(Environment.CurrentDirectory, @"..\..\", "Resources");
            using (FileStream serializedGraphStream = new FileStream(Path.Combine(resourcesPath, graphFileName), FileMode.Open))
            using (StreamReader serializedGraphStreamReader = new StreamReader(serializedGraphStream))
            {
                ModuleGraphXmlSerializer moduleGraphXmlSerializer = new ModuleGraphXmlSerializer(xmlDataSerializer);
                moduleGraph = moduleGraphXmlSerializer.Deserialize(serializedGraphStreamReader);
            }

            // Process and cancel the module graph
            Console.WriteLine("-- " + consoleTitle + " --");
            using (FileApplicationLogger logger = new FileApplicationLogger(LogLevel.Information, '|', "  ", Path.Combine(logFilePath, "SimpleMLSampleLog.txt")))
            using (SizeLimitedBufferProcessor bufferProcessor = new SizeLimitedBufferProcessor(10))
            using (FileMetricLogger metricLogger = new FileMetricLogger('|', Path.Combine(logFilePath, "SimpleMLSampleMetrics.txt"), bufferProcessor, true))
            using (ModuleGraphProcessor processor = new ModuleGraphProcessor(logger, metricLogger))
            {
                // Create a thread to call CancelProcessing() after 8 seconds
                Thread cancellationThread = new Thread
                (() =>
                {
                    Thread.Sleep(8000);
                    processor.CancelProcessing();
                }
                );
                cancellationThread.Start();

                // Process the module graph
                bufferProcessor.Start();
                try
                {
                    processor.Process(moduleGraph, false);
                }
                finally
                {
                    bufferProcessor.Stop();
                }
            }
        }

        /// <summary>
        /// Builds a module graph performing linear regression without feature scaling.
        /// </summary>
        /// <remarks>Equivalent to the module graph in file Resources\Linear Regression No Feature Scaling.xml</remarks>
        /// <returns>The module graph.</returns>
        private static ModuleGraph BuildLinearRegressionNoFeatureScalingModuleGraph()
        {
            // Create the modules
            MatrixCsvReader matrixCsvReader = new MatrixCsvReader();
            MatrixOrderRandomizer matrixOrderRandomizer = new MatrixOrderRandomizer();
            MatrixTrainTestSplitter matrixTrainTestSplitter = new MatrixTrainTestSplitter();
            MatrixColumnSplitter matrixColumnSplitter = new MatrixColumnSplitter();
            LinearRegressionGradientDescentOptimizer gradientDescentOptimizer = new LinearRegressionGradientDescentOptimizer();
            LinearRegressionCostSeriesCalculator costSeriesCalculator = new LinearRegressionCostSeriesCalculator();
            LinearRegressionHypothesisCalculator hypothesisCalculator = new LinearRegressionHypothesisCalculator();
            ModuleGraph moduleGraph = new ModuleGraph();
            // Add the modules to the module graph
            moduleGraph.AddModule(matrixCsvReader);
            moduleGraph.AddModule(matrixOrderRandomizer);
            moduleGraph.AddModule(matrixTrainTestSplitter);
            moduleGraph.AddModule(matrixColumnSplitter);
            moduleGraph.AddModule(gradientDescentOptimizer);
            moduleGraph.AddModule(costSeriesCalculator);
            moduleGraph.AddModule(hypothesisCalculator);
            // Create the slot links
            moduleGraph.CreateSlotLink(matrixCsvReader.GetOutputSlot("Matrix"), matrixOrderRandomizer.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixOrderRandomizer.GetOutputSlot("OutputMatrix"), matrixTrainTestSplitter.GetInputSlot("InputData"));
            moduleGraph.CreateSlotLink(matrixTrainTestSplitter.GetOutputSlot("TrainData"), matrixColumnSplitter.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("LeftColumnsMatrix"), gradientDescentOptimizer.GetInputSlot("TrainingSeriesData"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("RightColumnsMatrix"), gradientDescentOptimizer.GetInputSlot("TrainingSeriesResults"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("LeftColumnsMatrix"), costSeriesCalculator.GetInputSlot("TrainingSeriesData"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("RightColumnsMatrix"), costSeriesCalculator.GetInputSlot("TrainingSeriesResults"));
            moduleGraph.CreateSlotLink(gradientDescentOptimizer.GetOutputSlot("OptimizedThetaParameters"), costSeriesCalculator.GetInputSlot("ThetaParameters"));
            moduleGraph.CreateSlotLink(gradientDescentOptimizer.GetOutputSlot("OptimizedThetaParameters"), hypothesisCalculator.GetInputSlot("ThetaParameters"));
            // Set the input slot data values
            matrixCsvReader.GetInputSlot("CsvFilePath").DataValue = @"..\..\Resources\MarketData.csv";
            matrixCsvReader.GetInputSlot("CsvStartingColumn").DataValue = 1;
            matrixCsvReader.GetInputSlot("CsvNumberOfColumns").DataValue = 3;
            matrixOrderRandomizer.GetInputSlot("RandomSeed").DataValue = 1000;
            matrixTrainTestSplitter.GetInputSlot("TrainProportion").DataValue = 60;
            matrixColumnSplitter.GetInputSlot("RightSideColumns").DataValue = 1;
            gradientDescentOptimizer.GetInputSlot("InitialThetaParameters").DataValue = new Matrix(3, 1);
            gradientDescentOptimizer.GetInputSlot("LearningRate").DataValue = 1.0;
            gradientDescentOptimizer.GetInputSlot("MaxIterations").DataValue = 1500;
            Matrix predictValuesSeries = new Matrix(3, 2, new Double[] { 5103.27, 15323.14, 5411.61, 16385.89, 3510.40, 8235.87 });
            hypothesisCalculator.GetInputSlot("DataSeries").DataValue = predictValuesSeries;

            return moduleGraph;
        }

        /// <summary>
        /// Builds a module graph performing linear regression.
        /// </summary>
        /// <remarks>Equivalent to the module graph in file Resources\Linear Regression.xml</remarks>
        /// <returns>The module graph.</returns>
        private static ModuleGraph BuildLinearRegressionModuleGraph()
        {
            // Create the modules
            MatrixCsvReader matrixCsvReader = new MatrixCsvReader();
            MatrixOrderRandomizer matrixOrderRandomizer = new MatrixOrderRandomizer();
            AutomaticFeatureScaler automaticfeatureScaler = new Modules.AutomaticFeatureScaler();
            FeatureScalingParameterListSplitter scalingParameterSplitter = new FeatureScalingParameterListSplitter();
            MatrixTrainTestSplitter matrixTrainTestSplitter = new MatrixTrainTestSplitter();
            MatrixColumnSplitter matrixColumnSplitter = new MatrixColumnSplitter();
            LinearRegressionGradientDescentOptimizer gradientDescentOptimizer = new LinearRegressionGradientDescentOptimizer();
            LinearRegressionCostSeriesCalculator costSeriesCalculator = new LinearRegressionCostSeriesCalculator();
            Modules.FeatureScaler featureScaler = new Modules.FeatureScaler();
            LinearRegressionHypothesisCalculator hypothesisCalculator = new LinearRegressionHypothesisCalculator();
            FeatureRescaler featureRescaler = new FeatureRescaler();
            ModuleGraph moduleGraph = new ModuleGraph();
            // Add the modules to the module graph
            moduleGraph.AddModule(matrixCsvReader);
            moduleGraph.AddModule(matrixOrderRandomizer);
            moduleGraph.AddModule(automaticfeatureScaler);
            moduleGraph.AddModule(scalingParameterSplitter);
            moduleGraph.AddModule(matrixTrainTestSplitter);
            moduleGraph.AddModule(matrixColumnSplitter);
            moduleGraph.AddModule(gradientDescentOptimizer);
            moduleGraph.AddModule(costSeriesCalculator);
            moduleGraph.AddModule(featureScaler);
            moduleGraph.AddModule(hypothesisCalculator);
            moduleGraph.AddModule(featureRescaler);
            // Create the slot links
            moduleGraph.CreateSlotLink(matrixCsvReader.GetOutputSlot("Matrix"), matrixOrderRandomizer.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixOrderRandomizer.GetOutputSlot("OutputMatrix"), automaticfeatureScaler.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(automaticfeatureScaler.GetOutputSlot("ScaledMatrix"), matrixTrainTestSplitter.GetInputSlot("InputData"));
            moduleGraph.CreateSlotLink(automaticfeatureScaler.GetOutputSlot("FeatureScalingParameters"), scalingParameterSplitter.GetInputSlot("InputScalingParameters"));
            moduleGraph.CreateSlotLink(matrixTrainTestSplitter.GetOutputSlot("TrainData"), matrixColumnSplitter.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("LeftColumnsMatrix"), gradientDescentOptimizer.GetInputSlot("TrainingSeriesData"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("RightColumnsMatrix"), gradientDescentOptimizer.GetInputSlot("TrainingSeriesResults"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("LeftColumnsMatrix"), costSeriesCalculator.GetInputSlot("TrainingSeriesData"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("RightColumnsMatrix"), costSeriesCalculator.GetInputSlot("TrainingSeriesResults"));
            moduleGraph.CreateSlotLink(gradientDescentOptimizer.GetOutputSlot("OptimizedThetaParameters"), costSeriesCalculator.GetInputSlot("ThetaParameters"));
            moduleGraph.CreateSlotLink(scalingParameterSplitter.GetOutputSlot("StartScalingParameters"), featureScaler.GetInputSlot("FeatureScalingParameters"));
            moduleGraph.CreateSlotLink(gradientDescentOptimizer.GetOutputSlot("OptimizedThetaParameters"), hypothesisCalculator.GetInputSlot("ThetaParameters"));
            moduleGraph.CreateSlotLink(featureScaler.GetOutputSlot("ScaledMatrix"), hypothesisCalculator.GetInputSlot("DataSeries"));
            moduleGraph.CreateSlotLink(hypothesisCalculator.GetOutputSlot("Results"), featureRescaler.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(scalingParameterSplitter.GetOutputSlot("EndScalingParameters"), featureRescaler.GetInputSlot("FeatureScalingParameters"));
            // Set the input slot data values
            matrixCsvReader.GetInputSlot("CsvFilePath").DataValue = @"..\..\Resources\MarketData.csv";
            matrixCsvReader.GetInputSlot("CsvStartingColumn").DataValue = 1;
            matrixCsvReader.GetInputSlot("CsvNumberOfColumns").DataValue = 3;
            matrixOrderRandomizer.GetInputSlot("RandomSeed").DataValue = 1000;
            scalingParameterSplitter.GetInputSlot("RightSideItems").DataValue = 1;
            matrixTrainTestSplitter.GetInputSlot("TrainProportion").DataValue = 60;
            matrixColumnSplitter.GetInputSlot("RightSideColumns").DataValue = 1;
            gradientDescentOptimizer.GetInputSlot("InitialThetaParameters").DataValue = new Matrix(3, 1);
            gradientDescentOptimizer.GetInputSlot("LearningRate").DataValue = 1.0;
            gradientDescentOptimizer.GetInputSlot("MaxIterations").DataValue = 1500;
            Matrix predictValuesSeries = new Matrix(3, 2, new Double[] { 5103.27, 15323.14, 5411.61, 16385.89, 3510.40, 8235.87 });
            featureScaler.GetInputSlot("InputMatrix").DataValue = predictValuesSeries;

            return moduleGraph;
        }

        /// <summary>
        /// Builds a module graph performing linear regression and outputting the cost calculated against the test data.
        /// </summary>
        /// <remarks>Equivalent to the module graph in file Resources\Linear Regression with Test Data Cost.xml</remarks>
        /// <returns>The module graph.</returns>
        private static ModuleGraph BuildLinearRegressionTestDataCostModuleGraph()
        {
            // Create the modules
            MatrixCsvReader matrixCsvReader = new MatrixCsvReader();
            MatrixOrderRandomizer matrixOrderRandomizer = new MatrixOrderRandomizer();
            AutomaticFeatureScaler automaticfeatureScaler = new Modules.AutomaticFeatureScaler();
            FeatureScalingParameterListSplitter scalingParameterSplitter = new FeatureScalingParameterListSplitter();
            MatrixTrainTestSplitter matrixTrainTestSplitter = new MatrixTrainTestSplitter();
            MatrixColumnSplitter matrixColumnSplitter = new MatrixColumnSplitter();
            MatrixColumnSplitter matrixColumnSplitter2 = new MatrixColumnSplitter();
            LinearRegressionGradientDescentOptimizer gradientDescentOptimizer = new LinearRegressionGradientDescentOptimizer();
            LinearRegressionCostSeriesCalculator costSeriesCalculator = new LinearRegressionCostSeriesCalculator();
            Modules.FeatureScaler featureScaler = new Modules.FeatureScaler();
            LinearRegressionHypothesisCalculator hypothesisCalculator = new LinearRegressionHypothesisCalculator();
            FeatureRescaler featureRescaler = new FeatureRescaler();
            // Add the modules to the module graph
            ModuleGraph moduleGraph = new ModuleGraph();
            moduleGraph.AddModule(matrixCsvReader);
            moduleGraph.AddModule(matrixOrderRandomizer);
            moduleGraph.AddModule(automaticfeatureScaler);
            moduleGraph.AddModule(scalingParameterSplitter);
            moduleGraph.AddModule(matrixTrainTestSplitter);
            moduleGraph.AddModule(matrixColumnSplitter);
            moduleGraph.AddModule(matrixColumnSplitter2);
            moduleGraph.AddModule(gradientDescentOptimizer);
            moduleGraph.AddModule(costSeriesCalculator);
            moduleGraph.AddModule(featureScaler);
            moduleGraph.AddModule(hypothesisCalculator);
            moduleGraph.AddModule(featureRescaler);
            // Create the slot links
            moduleGraph.CreateSlotLink(matrixCsvReader.GetOutputSlot("Matrix"), matrixOrderRandomizer.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixOrderRandomizer.GetOutputSlot("OutputMatrix"), automaticfeatureScaler.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(automaticfeatureScaler.GetOutputSlot("ScaledMatrix"), matrixTrainTestSplitter.GetInputSlot("InputData"));
            moduleGraph.CreateSlotLink(automaticfeatureScaler.GetOutputSlot("FeatureScalingParameters"), scalingParameterSplitter.GetInputSlot("InputScalingParameters"));
            moduleGraph.CreateSlotLink(matrixTrainTestSplitter.GetOutputSlot("TrainData"), matrixColumnSplitter.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixTrainTestSplitter.GetOutputSlot("TestData"), matrixColumnSplitter2.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("LeftColumnsMatrix"), gradientDescentOptimizer.GetInputSlot("TrainingSeriesData"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("RightColumnsMatrix"), gradientDescentOptimizer.GetInputSlot("TrainingSeriesResults"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter2.GetOutputSlot("LeftColumnsMatrix"), costSeriesCalculator.GetInputSlot("TrainingSeriesData"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter2.GetOutputSlot("RightColumnsMatrix"), costSeriesCalculator.GetInputSlot("TrainingSeriesResults"));
            moduleGraph.CreateSlotLink(gradientDescentOptimizer.GetOutputSlot("OptimizedThetaParameters"), costSeriesCalculator.GetInputSlot("ThetaParameters"));
            moduleGraph.CreateSlotLink(scalingParameterSplitter.GetOutputSlot("StartScalingParameters"), featureScaler.GetInputSlot("FeatureScalingParameters"));
            moduleGraph.CreateSlotLink(gradientDescentOptimizer.GetOutputSlot("OptimizedThetaParameters"), hypothesisCalculator.GetInputSlot("ThetaParameters"));
            moduleGraph.CreateSlotLink(featureScaler.GetOutputSlot("ScaledMatrix"), hypothesisCalculator.GetInputSlot("DataSeries"));
            moduleGraph.CreateSlotLink(hypothesisCalculator.GetOutputSlot("Results"), featureRescaler.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(scalingParameterSplitter.GetOutputSlot("EndScalingParameters"), featureRescaler.GetInputSlot("FeatureScalingParameters"));
            // Set the input slot data values
            matrixCsvReader.GetInputSlot("CsvFilePath").DataValue = @"..\..\Resources\MarketData.csv";
            matrixCsvReader.GetInputSlot("CsvStartingColumn").DataValue = 1;
            matrixCsvReader.GetInputSlot("CsvNumberOfColumns").DataValue = 3;
            matrixOrderRandomizer.GetInputSlot("RandomSeed").DataValue = 1000;
            scalingParameterSplitter.GetInputSlot("RightSideItems").DataValue = 1;
            matrixTrainTestSplitter.GetInputSlot("TrainProportion").DataValue = 60;
            matrixColumnSplitter.GetInputSlot("RightSideColumns").DataValue = 1;
            matrixColumnSplitter2.GetInputSlot("RightSideColumns").DataValue = 1;
            gradientDescentOptimizer.GetInputSlot("InitialThetaParameters").DataValue = new Matrix(3, 1);
            gradientDescentOptimizer.GetInputSlot("LearningRate").DataValue = 1.0;
            gradientDescentOptimizer.GetInputSlot("MaxIterations").DataValue = 1500;
            Matrix predictValuesSeries = new Matrix(3, 2, new Double[] { 5103.27, 15323.14, 5411.61, 16385.89, 3510.40, 8235.87 });
            featureScaler.GetInputSlot("InputMatrix").DataValue = predictValuesSeries;

            return moduleGraph;
        }

        /// <summary>
        /// Builds a module graph performing linear regression and generating polynomial features on the training data.
        /// </summary>
        /// <remarks>Equivalent to the module graph in file Resources\Linear Regression with Polynomial Features.xml</remarks>
        /// <returns>The module graph.</returns>
        private static ModuleGraph BuildLinearRegressionWithPolynomialFeaturesModuleGraph()
        {
            // Create the modules
            MatrixCsvReader matrixCsvReader = new MatrixCsvReader();
            MatrixColumnSplitter matrixColumnSplitter2 = new MatrixColumnSplitter();
            MatrixColumnJoiner matrixColumnJoiner = new MatrixColumnJoiner();
            Modules.PolynomialFeatureGenerator polynomialFeatureGenerator1 = new Modules.PolynomialFeatureGenerator();
            MatrixOrderRandomizer matrixOrderRandomizer = new MatrixOrderRandomizer();
            AutomaticFeatureScaler automaticfeatureScaler = new Modules.AutomaticFeatureScaler();
            FeatureScalingParameterListSplitter scalingParameterSplitter = new FeatureScalingParameterListSplitter();
            MatrixTrainTestSplitter matrixTrainTestSplitter = new MatrixTrainTestSplitter();
            MatrixColumnSplitter matrixColumnSplitter = new MatrixColumnSplitter();
            LinearRegressionGradientDescentOptimizer gradientDescentOptimizer = new LinearRegressionGradientDescentOptimizer();
            LinearRegressionCostSeriesCalculator costSeriesCalculator = new LinearRegressionCostSeriesCalculator();
            Modules.PolynomialFeatureGenerator polynomialFeatureGenerator2 = new Modules.PolynomialFeatureGenerator();
            Modules.FeatureScaler featureScaler = new Modules.FeatureScaler();
            LinearRegressionHypothesisCalculator hypothesisCalculator = new LinearRegressionHypothesisCalculator();
            FeatureRescaler featureRescaler = new FeatureRescaler();
            // Add the modules to the module graph
            ModuleGraph moduleGraph = new ModuleGraph();
            moduleGraph.AddModule(matrixCsvReader);
            moduleGraph.AddModule(matrixColumnSplitter2);
            moduleGraph.AddModule(matrixColumnJoiner);
            moduleGraph.AddModule(polynomialFeatureGenerator1);
            moduleGraph.AddModule(matrixOrderRandomizer);
            moduleGraph.AddModule(automaticfeatureScaler);
            moduleGraph.AddModule(scalingParameterSplitter);
            moduleGraph.AddModule(matrixTrainTestSplitter);
            moduleGraph.AddModule(matrixColumnSplitter);
            moduleGraph.AddModule(gradientDescentOptimizer);
            moduleGraph.AddModule(costSeriesCalculator);
            moduleGraph.AddModule(polynomialFeatureGenerator2);
            moduleGraph.AddModule(featureScaler);
            moduleGraph.AddModule(hypothesisCalculator);
            moduleGraph.AddModule(featureRescaler);
            // Create the slot links
            moduleGraph.CreateSlotLink(matrixCsvReader.GetOutputSlot("Matrix"), matrixColumnSplitter2.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter2.GetOutputSlot("LeftColumnsMatrix"), polynomialFeatureGenerator1.GetInputSlot("DataSeries"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter2.GetOutputSlot("RightColumnsMatrix"), matrixColumnJoiner.GetInputSlot("RightMatrix"));
            moduleGraph.CreateSlotLink(polynomialFeatureGenerator1.GetOutputSlot("OutputMatrix"), matrixColumnJoiner.GetInputSlot("LeftMatrix"));
            moduleGraph.CreateSlotLink(matrixColumnJoiner.GetOutputSlot("OutputMatrix"), matrixOrderRandomizer.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixOrderRandomizer.GetOutputSlot("OutputMatrix"), automaticfeatureScaler.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(automaticfeatureScaler.GetOutputSlot("ScaledMatrix"), matrixTrainTestSplitter.GetInputSlot("InputData"));
            moduleGraph.CreateSlotLink(automaticfeatureScaler.GetOutputSlot("FeatureScalingParameters"), scalingParameterSplitter.GetInputSlot("InputScalingParameters"));
            moduleGraph.CreateSlotLink(matrixTrainTestSplitter.GetOutputSlot("TrainData"), matrixColumnSplitter.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("LeftColumnsMatrix"), gradientDescentOptimizer.GetInputSlot("TrainingSeriesData"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("RightColumnsMatrix"), gradientDescentOptimizer.GetInputSlot("TrainingSeriesResults"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("LeftColumnsMatrix"), costSeriesCalculator.GetInputSlot("TrainingSeriesData"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("RightColumnsMatrix"), costSeriesCalculator.GetInputSlot("TrainingSeriesResults"));
            moduleGraph.CreateSlotLink(gradientDescentOptimizer.GetOutputSlot("OptimizedThetaParameters"), costSeriesCalculator.GetInputSlot("ThetaParameters"));
            moduleGraph.CreateSlotLink(scalingParameterSplitter.GetOutputSlot("StartScalingParameters"), featureScaler.GetInputSlot("FeatureScalingParameters"));
            moduleGraph.CreateSlotLink(gradientDescentOptimizer.GetOutputSlot("OptimizedThetaParameters"), hypothesisCalculator.GetInputSlot("ThetaParameters"));
            moduleGraph.CreateSlotLink(polynomialFeatureGenerator2.GetOutputSlot("OutputMatrix"), featureScaler.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(featureScaler.GetOutputSlot("ScaledMatrix"), hypothesisCalculator.GetInputSlot("DataSeries"));
            moduleGraph.CreateSlotLink(hypothesisCalculator.GetOutputSlot("Results"), featureRescaler.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(scalingParameterSplitter.GetOutputSlot("EndScalingParameters"), featureRescaler.GetInputSlot("FeatureScalingParameters"));
            // Set the input slot data values
            matrixCsvReader.GetInputSlot("CsvFilePath").DataValue = @"..\..\Resources\MarketData.csv";
            matrixCsvReader.GetInputSlot("CsvStartingColumn").DataValue = 1;
            matrixCsvReader.GetInputSlot("CsvNumberOfColumns").DataValue = 3;
            matrixColumnSplitter2.GetInputSlot("RightSideColumns").DataValue = 1;
            polynomialFeatureGenerator1.GetInputSlot("PolynomialDegree").DataValue = 2;
            matrixOrderRandomizer.GetInputSlot("RandomSeed").DataValue = 1000;
            scalingParameterSplitter.GetInputSlot("RightSideItems").DataValue = 1;
            matrixTrainTestSplitter.GetInputSlot("TrainProportion").DataValue = 60;
            matrixColumnSplitter.GetInputSlot("RightSideColumns").DataValue = 1;
            gradientDescentOptimizer.GetInputSlot("InitialThetaParameters").DataValue = new Matrix(6, 1);
            gradientDescentOptimizer.GetInputSlot("LearningRate").DataValue = 1.95;
            gradientDescentOptimizer.GetInputSlot("MaxIterations").DataValue = 80000;
            Matrix predictValuesSeries = new Matrix(3, 2, new Double[] { 5103.27, 15323.14, 5411.61, 16385.89, 3510.40, 8235.87 });
            polynomialFeatureGenerator2.GetInputSlot("DataSeries").DataValue = predictValuesSeries;
            polynomialFeatureGenerator2.GetInputSlot("PolynomialDegree").DataValue = 2;

            return moduleGraph;
        }

        /// <summary>
        /// Builds a module graph performing linear regression generating polynomial features on the training data, and outputting the cost calculated against the test data..
        /// </summary>
        /// <remarks>Equivalent to the module graph in file Resources\Linear Regression with Polynomial Features.xml</remarks>
        /// <returns>The module graph.</returns>
        private static ModuleGraph BuildLinearRegressionWithPolynomialFeaturesTestDataCostModuleGraph()
        {
            // Create the modules
            MatrixCsvReader matrixCsvReader = new MatrixCsvReader();
            MatrixColumnSplitter matrixColumnSplitter2 = new MatrixColumnSplitter();
            MatrixColumnJoiner matrixColumnJoiner = new MatrixColumnJoiner();
            Modules.PolynomialFeatureGenerator polynomialFeatureGenerator1 = new Modules.PolynomialFeatureGenerator();
            MatrixOrderRandomizer matrixOrderRandomizer = new MatrixOrderRandomizer();
            AutomaticFeatureScaler automaticfeatureScaler = new Modules.AutomaticFeatureScaler();
            FeatureScalingParameterListSplitter scalingParameterSplitter = new FeatureScalingParameterListSplitter();
            MatrixTrainTestSplitter matrixTrainTestSplitter = new MatrixTrainTestSplitter();
            MatrixColumnSplitter matrixColumnSplitter3 = new MatrixColumnSplitter();
            MatrixColumnSplitter matrixColumnSplitter = new MatrixColumnSplitter();
            LinearRegressionGradientDescentOptimizer gradientDescentOptimizer = new LinearRegressionGradientDescentOptimizer();
            LinearRegressionCostSeriesCalculator costSeriesCalculator = new LinearRegressionCostSeriesCalculator();
            Modules.PolynomialFeatureGenerator polynomialFeatureGenerator2 = new Modules.PolynomialFeatureGenerator();
            Modules.FeatureScaler featureScaler = new Modules.FeatureScaler();
            LinearRegressionHypothesisCalculator hypothesisCalculator = new LinearRegressionHypothesisCalculator();
            FeatureRescaler featureRescaler = new FeatureRescaler();
            // Add the modules to the module graph
            ModuleGraph moduleGraph = new ModuleGraph();
            moduleGraph.AddModule(matrixCsvReader);
            moduleGraph.AddModule(matrixColumnSplitter2);
            moduleGraph.AddModule(matrixColumnJoiner);
            moduleGraph.AddModule(polynomialFeatureGenerator1);
            moduleGraph.AddModule(matrixOrderRandomizer);
            moduleGraph.AddModule(automaticfeatureScaler);
            moduleGraph.AddModule(scalingParameterSplitter);
            moduleGraph.AddModule(matrixTrainTestSplitter);
            moduleGraph.AddModule(matrixColumnSplitter3);
            moduleGraph.AddModule(matrixColumnSplitter);
            moduleGraph.AddModule(gradientDescentOptimizer);
            moduleGraph.AddModule(costSeriesCalculator);
            moduleGraph.AddModule(polynomialFeatureGenerator2);
            moduleGraph.AddModule(featureScaler);
            moduleGraph.AddModule(hypothesisCalculator);
            moduleGraph.AddModule(featureRescaler);
            // Create the slot links
            moduleGraph.CreateSlotLink(matrixCsvReader.GetOutputSlot("Matrix"), matrixColumnSplitter2.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter2.GetOutputSlot("LeftColumnsMatrix"), polynomialFeatureGenerator1.GetInputSlot("DataSeries"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter2.GetOutputSlot("RightColumnsMatrix"), matrixColumnJoiner.GetInputSlot("RightMatrix"));
            moduleGraph.CreateSlotLink(polynomialFeatureGenerator1.GetOutputSlot("OutputMatrix"), matrixColumnJoiner.GetInputSlot("LeftMatrix"));
            moduleGraph.CreateSlotLink(matrixColumnJoiner.GetOutputSlot("OutputMatrix"), matrixOrderRandomizer.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixOrderRandomizer.GetOutputSlot("OutputMatrix"), automaticfeatureScaler.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(automaticfeatureScaler.GetOutputSlot("ScaledMatrix"), matrixTrainTestSplitter.GetInputSlot("InputData"));
            moduleGraph.CreateSlotLink(automaticfeatureScaler.GetOutputSlot("FeatureScalingParameters"), scalingParameterSplitter.GetInputSlot("InputScalingParameters"));
            moduleGraph.CreateSlotLink(matrixTrainTestSplitter.GetOutputSlot("TrainData"), matrixColumnSplitter.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixTrainTestSplitter.GetOutputSlot("TestData"), matrixColumnSplitter3.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("LeftColumnsMatrix"), gradientDescentOptimizer.GetInputSlot("TrainingSeriesData"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("RightColumnsMatrix"), gradientDescentOptimizer.GetInputSlot("TrainingSeriesResults"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter3.GetOutputSlot("LeftColumnsMatrix"), costSeriesCalculator.GetInputSlot("TrainingSeriesData"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter3.GetOutputSlot("RightColumnsMatrix"), costSeriesCalculator.GetInputSlot("TrainingSeriesResults"));
            moduleGraph.CreateSlotLink(gradientDescentOptimizer.GetOutputSlot("OptimizedThetaParameters"), costSeriesCalculator.GetInputSlot("ThetaParameters"));
            moduleGraph.CreateSlotLink(scalingParameterSplitter.GetOutputSlot("StartScalingParameters"), featureScaler.GetInputSlot("FeatureScalingParameters"));
            moduleGraph.CreateSlotLink(gradientDescentOptimizer.GetOutputSlot("OptimizedThetaParameters"), hypothesisCalculator.GetInputSlot("ThetaParameters"));
            moduleGraph.CreateSlotLink(polynomialFeatureGenerator2.GetOutputSlot("OutputMatrix"), featureScaler.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(featureScaler.GetOutputSlot("ScaledMatrix"), hypothesisCalculator.GetInputSlot("DataSeries"));
            moduleGraph.CreateSlotLink(hypothesisCalculator.GetOutputSlot("Results"), featureRescaler.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(scalingParameterSplitter.GetOutputSlot("EndScalingParameters"), featureRescaler.GetInputSlot("FeatureScalingParameters"));
            // Set the input slot data values
            matrixCsvReader.GetInputSlot("CsvFilePath").DataValue = @"..\..\Resources\MarketData.csv";
            matrixCsvReader.GetInputSlot("CsvStartingColumn").DataValue = 1;
            matrixCsvReader.GetInputSlot("CsvNumberOfColumns").DataValue = 3;
            matrixColumnSplitter2.GetInputSlot("RightSideColumns").DataValue = 1;
            polynomialFeatureGenerator1.GetInputSlot("PolynomialDegree").DataValue = 2;
            matrixOrderRandomizer.GetInputSlot("RandomSeed").DataValue = 1000;
            scalingParameterSplitter.GetInputSlot("RightSideItems").DataValue = 1;
            matrixTrainTestSplitter.GetInputSlot("TrainProportion").DataValue = 60;
            matrixColumnSplitter.GetInputSlot("RightSideColumns").DataValue = 1;
            matrixColumnSplitter3.GetInputSlot("RightSideColumns").DataValue = 1;
            gradientDescentOptimizer.GetInputSlot("InitialThetaParameters").DataValue = new Matrix(6, 1);
            gradientDescentOptimizer.GetInputSlot("LearningRate").DataValue = 1.95;
            gradientDescentOptimizer.GetInputSlot("MaxIterations").DataValue = 80000;
            Matrix predictValuesSeries = new Matrix(3, 2, new Double[] { 5103.27, 15323.14, 5411.61, 16385.89, 3510.40, 8235.87 });
            polynomialFeatureGenerator2.GetInputSlot("DataSeries").DataValue = predictValuesSeries;
            polynomialFeatureGenerator2.GetInputSlot("PolynomialDegree").DataValue = 2;

            return moduleGraph;
        }

        /// <summary>
        /// Builds a module graph performing logistic regression.
        /// </summary>
        /// <remarks>Equivalent to the module graph in file Resources\Logistic Regression Regularization Comparison.xml</remarks>
        /// <returns>The module graph.</returns>
        private static ModuleGraph BuildLogisticRegressionModuleGraph()
        {
            // Create the modules
            MatrixCsvReader matrixCsvReader = new MatrixCsvReader();
            MatrixOrderRandomizer matrixOrderRandomizer = new MatrixOrderRandomizer();
            MatrixColumnSplitter matrixColumnSplitter = new MatrixColumnSplitter();
            Modules.PolynomialFeatureGenerator polynomialFeatureGenerator = new Modules.PolynomialFeatureGenerator();
            MatrixColumnAdder matrixColumnAdder = new MatrixColumnAdder();
            MatrixColumnJoiner matrixColumnJoiner = new MatrixColumnJoiner();
            MatrixTrainTestSplitter matrixTrainTestSplitter = new MatrixTrainTestSplitter();
            MatrixColumnSplitter matrixColumnSplitter2 = new MatrixColumnSplitter();
            MatrixColumnSplitter matrixColumnSplitter3 = new MatrixColumnSplitter();
            MultiParameterTrainer multiParameterTrainer = new MultiParameterTrainer();
            Modules.LogisticRegressionCostFunctionCalculator costFunctionCalculator = new Modules.LogisticRegressionCostFunctionCalculator();
            MultiParameterErrorRateCalculator multiParameterErrorRateCalculator = new MultiParameterErrorRateCalculator();
            // Add the modules to the module graph
            ModuleGraph moduleGraph = new ModuleGraph();
            moduleGraph.AddModule(matrixCsvReader);
            moduleGraph.AddModule(matrixOrderRandomizer);
            moduleGraph.AddModule(matrixColumnSplitter);
            moduleGraph.AddModule(polynomialFeatureGenerator);
            moduleGraph.AddModule(matrixColumnAdder);
            moduleGraph.AddModule(matrixColumnJoiner);
            moduleGraph.AddModule(matrixTrainTestSplitter);
            moduleGraph.AddModule(matrixColumnSplitter2);
            moduleGraph.AddModule(matrixColumnSplitter3);
            moduleGraph.AddModule(costFunctionCalculator);
            moduleGraph.AddModule(multiParameterTrainer);
            moduleGraph.AddModule(multiParameterErrorRateCalculator);
            // Create the slot links
            moduleGraph.CreateSlotLink(matrixCsvReader.GetOutputSlot("Matrix"), matrixOrderRandomizer.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixOrderRandomizer.GetOutputSlot("OutputMatrix"), matrixColumnSplitter.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("LeftColumnsMatrix"), polynomialFeatureGenerator.GetInputSlot("DataSeries"));
            moduleGraph.CreateSlotLink(polynomialFeatureGenerator.GetOutputSlot("OutputMatrix"), matrixColumnAdder.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixColumnAdder.GetOutputSlot("OutputMatrix"), matrixColumnJoiner.GetInputSlot("LeftMatrix"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter.GetOutputSlot("RightColumnsMatrix"), matrixColumnJoiner.GetInputSlot("RightMatrix"));
            moduleGraph.CreateSlotLink(matrixColumnJoiner.GetOutputSlot("OutputMatrix"), matrixTrainTestSplitter.GetInputSlot("InputData"));
            moduleGraph.CreateSlotLink(matrixTrainTestSplitter.GetOutputSlot("TrainData"), matrixColumnSplitter2.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter2.GetOutputSlot("LeftColumnsMatrix"), multiParameterTrainer.GetInputSlot("DataSeries"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter2.GetOutputSlot("RightColumnsMatrix"), multiParameterTrainer.GetInputSlot("DataResults"));
            moduleGraph.CreateSlotLink(costFunctionCalculator.GetOutputSlot("CostFunctionCalculator"), multiParameterTrainer.GetInputSlot("CostFunctionCalculator"));
            moduleGraph.CreateSlotLink(multiParameterTrainer.GetOutputSlot("OptimizedThetaParameterSet"), multiParameterErrorRateCalculator.GetInputSlot("ThetaParameterSet"));
            moduleGraph.CreateSlotLink(matrixTrainTestSplitter.GetOutputSlot("TestData"), matrixColumnSplitter3.GetInputSlot("InputMatrix"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter3.GetOutputSlot("LeftColumnsMatrix"), multiParameterErrorRateCalculator.GetInputSlot("DataSeries"));
            moduleGraph.CreateSlotLink(matrixColumnSplitter3.GetOutputSlot("RightColumnsMatrix"), multiParameterErrorRateCalculator.GetInputSlot("DataResults"));

            // Set the input slot data values
            matrixCsvReader.GetInputSlot("CsvFilePath").DataValue = @"..\..\Resources\LogisticRegressionData.csv";
            matrixCsvReader.GetInputSlot("CsvStartingColumn").DataValue = 1;
            matrixCsvReader.GetInputSlot("CsvNumberOfColumns").DataValue = 3;
            matrixOrderRandomizer.GetInputSlot("RandomSeed").DataValue = 1000;
            matrixColumnSplitter.GetInputSlot("RightSideColumns").DataValue = 1;
            polynomialFeatureGenerator.GetInputSlot("PolynomialDegree").DataValue = 2;
            matrixColumnAdder.GetInputSlot("NumberOfColumns").DataValue = 1;
            matrixColumnAdder.GetInputSlot("LeftSide").DataValue = true;
            matrixColumnAdder.GetInputSlot("DefaultValue").DataValue = 1.0;
            matrixTrainTestSplitter.GetInputSlot("TrainProportion").DataValue = 70;
            matrixColumnSplitter2.GetInputSlot("RightSideColumns").DataValue = 1;
            matrixColumnSplitter3.GetInputSlot("RightSideColumns").DataValue = 1;
            multiParameterTrainer.GetInputSlot("InitialThetaParameters").DataValue = new Matrix(6, 1);
            List<Double> regularizationParameterSet = new List<Double>() { 0, 0.5, 1, 1.5, 100 };
            List<Int32> maxIterationParameterSet = new List<Int32>() { 400, 400, 400, 400, 400 };
            multiParameterTrainer.GetInputSlot("RegularizationParameterSet").DataValue = regularizationParameterSet;
            multiParameterTrainer.GetInputSlot("MaxIterationParameterSet").DataValue = maxIterationParameterSet;

            return moduleGraph;
        }
    }
}
