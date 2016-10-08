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
using System.Xml;

namespace SimpleML.Containers.Serialization
{
    /// <summary>
    /// Serializes classes in the SimpleML.Containers namespace to and from XML.
    /// </summary>
    public class ContainerObjectXmlSerializer
    {
        /// <summary>The name of the element storing the dimensions of a matrix.</summary>
        protected String matrixDimensionsElementName = "Dimensions";
        /// <summary>The name of the element storing the 'm' dimension of a matrix.</summary>
        protected String matrixMDimensionElementName = "MDimension";
        /// <summary>The name of the element storing the 'n' dimension of a matrix.</summary>
        protected String matrixNDimensionElementName = "NDimension";
        /// <summary>The name of the element storing the rows of a matrix.</summary>
        protected String matrixRowsElementName = "Rows";
        /// <summary>The name of the element storing an individual row of a matrix.</summary>
        protected String matrixRowElementName = "Row";

        /// <summary>
        /// Initialises a new instance of the SimpleML.Containers.Serialization.ContainerObjectXmlSerializer class.
        /// </summary>
        public ContainerObjectXmlSerializer()
        {
        }

        /// <summary>
        /// Serializes a matrix to the specified XML writer.
        /// </summary>
        /// <param name="inputMatrix">The matrix to serialize.</param>
        /// <param name="xmlWriter">The XML writer to serialize to.</param>
        public void SerializeMatrix(Matrix inputMatrix, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(matrixDimensionsElementName);
            xmlWriter.WriteElementString(matrixMDimensionElementName, inputMatrix.MDimension.ToString());
            xmlWriter.WriteElementString(matrixNDimensionElementName, inputMatrix.NDimension.ToString());
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement(matrixRowsElementName);
            for (Int32 i = 1; i <= inputMatrix.MDimension; i = i + 1)
            {
                StringBuilder rowData = new StringBuilder();
                for (Int32 j = 1; j < inputMatrix.NDimension; j = j + 1)
                {
                    rowData.Append(inputMatrix.GetElement(i, j) + ", ");
                }
                rowData.Append(inputMatrix.GetElement(i, inputMatrix.NDimension));
                xmlWriter.WriteElementString(matrixRowElementName, rowData.ToString());
            }
            xmlWriter.WriteEndElement();
        }

        /// <summary>
        /// Deserializes a matrix from an XML reader.
        /// </summary>
        /// <param name="xmlReader">The XML reader to deserialize from.</param>
        /// <returns>The deserialized matrix.</returns>
        public Matrix DeserializeMatrix(XmlReader xmlReader)
        {
            // TODO: Could probably put more explicit exception handling here...
            xmlReader.ReadStartElement(matrixDimensionsElementName);
            String mMDimensionString = xmlReader.ReadElementString(matrixMDimensionElementName);
            String nMDimensionString = xmlReader.ReadElementString(matrixNDimensionElementName);
            xmlReader.ReadEndElement();
            Int32 mMDimension = Int32.Parse(mMDimensionString);
            Int32 nMDimension = Int32.Parse(nMDimensionString);
            Matrix returnMatrix = new Matrix(mMDimension, nMDimension);
            
            xmlReader.ReadStartElement(matrixRowsElementName);
            Int32 rowIndex = 1;
            while (xmlReader.IsStartElement(matrixRowElementName) == true)
            {
                // TODO: ...e.g. checks here that number of elements matches 'n' dimension of matrix.
                String rowString = xmlReader.ReadElementString(matrixRowElementName);
                String[] rowStringElements = rowString.Split(',');
                for (Int32 i = 1; i <= returnMatrix.NDimension; i = i + 1)
                {
                    returnMatrix.SetElement(rowIndex, i, Double.Parse(rowStringElements[i - 1]));
                }
                rowIndex++;
            }
            xmlReader.ReadEndElement();

            return returnMatrix;
        }
    }
}
