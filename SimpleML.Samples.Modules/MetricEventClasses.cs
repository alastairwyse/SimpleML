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
using ApplicationMetrics;

namespace SimpleML.Samples.Modules
{
    public class CsvFileRead : CountMetric
    {
        public CsvFileRead()
        {
            base.name = "CsvFileRead";
            base.description = "The number of CSV files read";
        }
    }

    public class CsvFileReadTime : IntervalMetric
    {
        public CsvFileReadTime()
        {
            base.name = "CsvFileReadTime";
            base.description = "The time taken to read a CSV file";
        }
    }

    public class MatrixRowOrderRandomized : CountMetric
    {
        public MatrixRowOrderRandomized()
        {
            base.name = "MatrixRowOrderRandomized";
            base.description = "The number of matrices randomized";
        }
    }

    public class MatrixFeaturesScaled : CountMetric
    {
        public MatrixFeaturesScaled()
        {
            base.name = "MatrixFeaturesScaled";
            base.description = "The number of matrices feature scaled";
        }
    }

    public class MatrixFeaturesRescaled : CountMetric
    {
        public MatrixFeaturesRescaled()
        {
            base.name = "MatrixFeaturesRescaled";
            base.description = "The number of matrices feature rescaled";
        }
    }

    public class MultiParameterTrainingIteration : CountMetric
    {
        public MultiParameterTrainingIteration()
        {
            base.name = "MultiParameterTrainingIteration";
            base.description = "The number of iterations of function minimization training run by the MultiParameterTrainer class";
        }
    }

    public class MultiParameterTrainingTime : IntervalMetric
    {
        public MultiParameterTrainingTime()
        {
            base.name = "MultiParameterTrainingTime";
            base.description = "The time taken to run multiple iterations of function minimization";
        }
    }
}
