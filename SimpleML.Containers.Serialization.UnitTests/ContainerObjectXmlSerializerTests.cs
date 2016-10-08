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
using System.IO;
using System.Xml;
using NUnit.Framework;
using SimpleML.Containers;
using SimpleML.Containers.Serialization;

namespace SimpleML.Containers.Serialization.UnitTests
{
    /// <summary>
    /// Unit tests for class SimpleML.Containers.Serialization.ContainerObjectXmlSerializer
    /// </summary>
    public class ContainerObjectXmlSerializerTests
    {
        private const String xmlWrappingTag = "SerializedData";

        private MemoryStream memoryStream;
        private XmlWriter xmlWriter;
        private ContainerObjectXmlSerializer testContainerObjectXmlSerializer;

        [SetUp]
        protected void SetUp()
        {
            memoryStream = new MemoryStream();
            xmlWriter = XmlWriter.Create(memoryStream);
            testContainerObjectXmlSerializer = new ContainerObjectXmlSerializer();

            // Write XML document headers and tag to the XML writer, so that test methods can write partial XML which is still valid
            xmlWriter.WriteStartElement(xmlWrappingTag);
        }

        [TearDown]
        protected void TearDown()
        {
            ((IDisposable)xmlWriter).Dispose();
            memoryStream.Dispose();
        }

        /// <summary>
        /// Success tests for the SerializeMatrix() and DeserializeMatrix() methods.
        /// </summary>
        [Test]
        public void SerializeMatrix()
        {
            Matrix testMatrix = new Matrix(4, 3, new Double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.1 });

            testContainerObjectXmlSerializer.SerializeMatrix(testMatrix, xmlWriter);

            WriteClosingElementAndResetXmlStream(xmlWriter, memoryStream);

            Matrix resultMatrix;
            using (XmlReader xmlReader = XmlReader.Create(memoryStream))
            {
                AdvanceXmlReaderPosition(xmlReader);
                resultMatrix = testContainerObjectXmlSerializer.DeserializeMatrix(xmlReader);
            }

            Assert.AreEqual(4, resultMatrix.MDimension);
            Assert.AreEqual(3, resultMatrix.NDimension);
            Assert.That(resultMatrix.GetElement(1, 1), Is.EqualTo(1).Within(1e-0));
            Assert.That(resultMatrix.GetElement(1, 2), Is.EqualTo(2).Within(1e-0));
            Assert.That(resultMatrix.GetElement(1, 3), Is.EqualTo(3).Within(1e-0));
            Assert.That(resultMatrix.GetElement(2, 1), Is.EqualTo(4).Within(1e-0));
            Assert.That(resultMatrix.GetElement(2, 2), Is.EqualTo(5).Within(1e-0));
            Assert.That(resultMatrix.GetElement(2, 3), Is.EqualTo(6).Within(1e-0));
            Assert.That(resultMatrix.GetElement(3, 1), Is.EqualTo(7).Within(1e-0));
            Assert.That(resultMatrix.GetElement(3, 2), Is.EqualTo(8).Within(1e-0));
            Assert.That(resultMatrix.GetElement(3, 3), Is.EqualTo(9).Within(1e-0));
            Assert.That(resultMatrix.GetElement(4, 1), Is.EqualTo(10).Within(1e-0));
            Assert.That(resultMatrix.GetElement(4, 2), Is.EqualTo(11).Within(1e-0));
            Assert.That(resultMatrix.GetElement(4, 3), Is.EqualTo(12.1).Within(1e-1));
        }

        # region Private Methods

        /// <summary>
        /// Writes closing 'wrapping' XML element to the specified XML writer, and resets the position of the specified memory stream to 0.
        /// </summary>
        /// <param name="xmlWriter">The XML writer to write the closing element to.</param>
        /// <param name="memoryStream">The memory stream to reset the position of.</param>
        private void WriteClosingElementAndResetXmlStream(XmlWriter xmlWriter, MemoryStream memoryStream)
        {
            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            memoryStream.Position = 0;
        }

        /// <summary>
        /// Advances the position in the specified XML reader, to after the wrapping element tag.
        /// </summary>
        /// <param name="xmlReader">The XML reader to advance the position of.</param>
        private void AdvanceXmlReaderPosition(XmlReader xmlReader)
        {
            xmlReader.ReadStartElement(xmlWrappingTag);
        }

        #endregion
    }
}
