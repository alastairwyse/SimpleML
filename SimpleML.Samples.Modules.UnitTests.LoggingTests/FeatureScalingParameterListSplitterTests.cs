using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NMock2;
using NMock2.Matchers;
using ApplicationLogging;
using SimpleML;
using SimpleML.Containers;
using SimpleML.Samples.Modules;

namespace SimpleML.Samples.Modules.UnitTests.LoggingTests
{
    /// <summary>
    /// Tests the logging functionality in class SimpleML.Samples.Modules.FeatureScalingParameterListSplitter.
    /// </summary>
    public class FeatureScalingParameterListSplitterTests
    {
        private Mockery mockery;
        private IApplicationLogger mockApplicationLogger;
        private FeatureScalingParameterListSplitter testFeatureScalingParameterListSplitter;

        [SetUp]
        protected void SetUp()
        {
            mockery = new Mockery();
            mockApplicationLogger = mockery.NewMock<IApplicationLogger>();
            testFeatureScalingParameterListSplitter = new FeatureScalingParameterListSplitter();
            testFeatureScalingParameterListSplitter.Logger = mockApplicationLogger;
        }

        /// <summary>
        /// Tests the logging functionality when the ImplementProcess() method is called with 'RightSideItems' parameter less than 1.
        /// </summary>
        [Test]
        public void ImplementProcess_RightSideItemsParameterLessThan1()
        {
            testFeatureScalingParameterListSplitter.GetInputSlot("RightSideItems").DataValue = 0;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testFeatureScalingParameterListSplitter, LogLevel.Critical, "Parameter 'RightSideItems' must be greater than 0.", new TypeMatcher(typeof(ArgumentException)));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testFeatureScalingParameterListSplitter.Process();
            });

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests the logging functionality whenthe ImplementProcess() method is called with 'RightSideItems' parameter which is the same as the size of parameter 'InputScalingParameters'.
        /// </summary>
        [Test]
        public void ImplementProcess_RightSideItemsParameterSameAsListSize()
        {
            List<FeatureScalingParameters> featureScalingParameters = new List<FeatureScalingParameters>()
            {
                new FeatureScalingParameters(0.5, 1.0), 
                new FeatureScalingParameters(0.4, 0.9), 
                new FeatureScalingParameters(0.6, 1.1)
            };
            testFeatureScalingParameterListSplitter.GetInputSlot("InputScalingParameters").DataValue = featureScalingParameters;
            testFeatureScalingParameterListSplitter.GetInputSlot("RightSideItems").DataValue = 3;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testFeatureScalingParameterListSplitter, LogLevel.Critical, "Parameter 'RightSideItems' must be less than the size of the inputted list.", new TypeMatcher(typeof(ArgumentException)));
            }

            ArgumentException e = Assert.Throws<ArgumentException>(delegate
            {
                testFeatureScalingParameterListSplitter.Process();
            });

            mockery.VerifyAllExpectationsHaveBeenMet();
        }

        /// <summary>
        /// Tests the logging functionality in the ImplementProcess() method.
        /// </summary>
        [Test]
        public void ImplementProcess()
        {
            List<FeatureScalingParameters> featureScalingParameters = new List<FeatureScalingParameters>()
            {
                new FeatureScalingParameters(0.5, 1.0), 
                new FeatureScalingParameters(0.4, 0.9), 
                new FeatureScalingParameters(0.6, 1.1)
            };
            testFeatureScalingParameterListSplitter.GetInputSlot("InputScalingParameters").DataValue = featureScalingParameters;
            testFeatureScalingParameterListSplitter.GetInputSlot("RightSideItems").DataValue = 1;

            using (mockery.Ordered)
            {
                Expect.Once.On(mockApplicationLogger).Method("Log").With(testFeatureScalingParameterListSplitter, LogLevel.Information, "Split List of FeatureScalingParameters of size 3 into Lists of 2 and 1 items.");
            }

            testFeatureScalingParameterListSplitter.Process();

            mockery.VerifyAllExpectationsHaveBeenMet();
        }
    }
}
