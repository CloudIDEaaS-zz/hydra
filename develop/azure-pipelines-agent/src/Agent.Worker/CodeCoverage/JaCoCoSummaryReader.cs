// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Agent.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Agent.Worker.CodeCoverage
{
    public sealed class JaCoCoSummaryReader : AgentService, ICodeCoverageSummaryReader
    {
        public Type ExtensionType => typeof(ICodeCoverageSummaryReader);
        public string Name => "JaCoCo";

        private const string _covered = "covered";
        private const string _missed = "missed";

        public IEnumerable<CodeCoverageStatistics> GetCodeCoverageSummary(IExecutionContext context, string summaryXmlLocation)
        {
            var doc = CodeCoverageUtilities.ReadSummaryFile(context, summaryXmlLocation);
            return ReadDataFromNodes(doc, summaryXmlLocation);
        }

        private IEnumerable<CodeCoverageStatistics> ReadDataFromNodes(XmlDocument doc, string summaryXmlLocation)
        {
            var listCoverageStats = new List<CodeCoverageStatistics>();

            if (doc == null)
            {
                return null;
            }

            //read data from report nodes
            XmlNode reportNode = doc.SelectSingleNode("report");
            if (reportNode != null)
            {
                XmlNodeList counterNodeList = doc.SelectNodes("/report/counter");
                if (counterNodeList != null)
                {

                    foreach (XmlNode counterNode in counterNodeList)
                    {
                        var coverageStatistics = new CodeCoverageStatistics();

                        if (counterNode.Attributes != null)
                        {
                            if (counterNode.Attributes["type"] != null)
                            {
                                coverageStatistics.Label = ToTitleCase(counterNode.Attributes["type"].Value);
                                coverageStatistics.Position = CodeCoverageUtilities.GetPriorityOrder(coverageStatistics.Label);
                            }

                            if (counterNode.Attributes[_covered] != null)
                            {
                                float covered;
                                if (!float.TryParse(counterNode.Attributes[_covered].Value, out covered))
                                {
                                    throw new InvalidDataException(StringUtil.Loc("InvalidValueInXml", _covered, summaryXmlLocation));
                                }
                                coverageStatistics.Covered = (int)covered;
                                if (counterNode.Attributes[_missed] != null)
                                {
                                    float missed;
                                    if (!float.TryParse(counterNode.Attributes[_missed].Value, out missed))
                                    {
                                        throw new InvalidDataException(StringUtil.Loc("InvalidValueInXml", _missed, summaryXmlLocation));
                                    }
                                    coverageStatistics.Total = (int)missed + (int)covered;
                                }
                            }
                        }

                        listCoverageStats.Add(coverageStatistics);
                    }
                }
            }
            return listCoverageStats.AsEnumerable();
        }

        private string ToTitleCase(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString))
            {
                return inputString;
            }

            string outputString = string.Empty;
            var newWord = true;
            for (int i = 0; i < inputString.Length; i++)
            {
                if (inputString[i] == ' ')
                {
                    newWord = true;
                    outputString += inputString[i];
                    continue;
                }
                if (newWord)
                {
                    outputString += char.ToUpper(inputString[i]);
                }
                else
                {
                    outputString += char.ToLower(inputString[i]);
                }

                newWord = false;
            }
            return outputString;
        }
    }
}
