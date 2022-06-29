using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Build.Framework;
using System.Diagnostics;
using System.Xml.XPath;
using System.Xml;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using EnvDTE;
using Process = System.Diagnostics.Process;
using Debugger = System.Diagnostics.Debugger;
using Microsoft.VisualStudio.ExtensionManager;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO.Packaging;
using Utils;
using Utils.Hierarchies;
using System.Xml.Linq;
using System.Net.Http;
using HtmlAgilityPack;

namespace BuildTasks
{
    public class UpdateExportedProjectTemplates : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        public bool Execute()
        {
            try
            {
                var solutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
                var projectPath = Path.Combine(solutionPath, @"ApplicationGenerator.IonicAngular");
                var templatesFilePath = Path.Combine(projectPath, @"ExportedProjectTemplates");
                var directory = new DirectoryInfo(templatesFilePath);
                var parentProcess = Process.GetCurrentProcess().GetParent();
                var packages = new[] { "Hydra.Interfaces", "Utils.Core" };
                var latestVersions = new Dictionary<string, string>();
                var environmentVariable = "LastHydraUpdateExportedProjectTemplatesDate";
                var updatedDate = Environment.GetEnvironmentVariable(environmentVariable, EnvironmentVariableTarget.User);

                if (updatedDate == null || DateTime.Parse(updatedDate) < DateTime.Today)
                {
                    updatedDate = DateTime.Now.ToString();
                    Environment.SetEnvironmentVariable(environmentVariable, updatedDate, EnvironmentVariableTarget.User);
                }
                else
                {
                    return true;
                }

                if (!parentProcess.ProcessName.IsOneOf("devenv", "msvsmon"))
                {
                    return true;
                }

                foreach (var package in packages)
                {
                    var client = new HttpClient();
                    var html = client.GetStringAsync($"https://www.nuget.org/packages/{ package }/").Result;
                    var document = new HtmlAgilityPack.HtmlDocument();
                    HtmlNode body;
                    HtmlNode versionDiv;
                    HtmlNode versionTable;
                    HtmlNode topVersionRow;
                    string latestVersion;

                    document.LoadHtml(html);

                    body = document.DocumentNode.Descendants("body").Single();
                    versionDiv = body.SelectSingleNode("//div[@id='version-history']");
                    versionTable = versionDiv.SelectSingleNode("table");
                    topVersionRow = versionTable.SelectSingleNode("tbody/tr[1]");
                    latestVersion = topVersionRow.SelectSingleNode("td/a").Attributes["title"].Value;

                    latestVersions.Add(package, latestVersion);
                }

                foreach (var projectFile in directory.GetFiles("*.csproj", SearchOption.AllDirectories))
                {
                    var projectDocumentContents = File.ReadAllText(projectFile.FullName);

                    foreach (var pair in latestVersions)
                    {
                        var package = pair.Key;
                        var latestVersion = pair.Value;
                        var regex = new Regex($"\\<PackageReference Include=\"{ package }\" Version=\"(?<version>.+?)\" /\\>", RegexOptions.Multiline | RegexOptions.Singleline);

                        if (regex.IsMatch(projectDocumentContents))
                        {
                            var match = regex.Match(projectDocumentContents);
                            var group = match.Groups["version"];
                            var version = group.Value;

                            if (version != latestVersion)
                            {
                                projectDocumentContents = group.Replace(projectDocumentContents, latestVersion);
                            }
                        }
                    }

                    using (var writer = new StreamWriter(projectFile.FullName))
                    {
                        writer.Write(projectDocumentContents);
                        writer.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                var error = string.Format("Error updating exported project templates. \r\nError: {0}", ex.ToString());

                var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, error, "", "", DateTime.Now);
                this.BuildEngine.LogErrorEvent(message);

                Console.WriteLine(error);

                return false;
            }

            return true;
        }
    }
}
