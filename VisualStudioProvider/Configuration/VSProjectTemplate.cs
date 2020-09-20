using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeInterfaces;
using System.IO;
using System.Text.RegularExpressions;
using Utils;

namespace VisualStudioProvider.Configuration
{
    public class VSProjectTemplate : VSTemplate, ICodeProjectTemplate
    {
        public List<ICodeTemplateProject> Projects { get; private set; }

        public VSProjectTemplate()
        {
            Projects = new List<ICodeTemplateProject>();
        }

        /// TODO:
        /// - truly process $if$ $then$'s
        /// - framework version should come from target solution

        public override void CopyAndProcess(string copyToPath, ICodeTemplateParameters parameters, bool overwriteExisting = true, List<string> skip = null)
        {
            var templateFile = VSConfigProvider.Decompress(this.ZippedTemplate, copyToPath, overwriteExisting, skip);

            File.Delete(templateFile);

            this.parameters = parameters;

            foreach (var project in this.Projects)
            {
                if (project.ReplaceParameters)
                {
                    var fileName = Path.Combine(copyToPath, project.FileName);

                    using (var stream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite))
                    {
                        ReplaceParameters(stream, parameters);
                    }
                }

                foreach (var projectItem in project.ProjectItems)
                {
                    var fileName = Path.Combine(copyToPath, projectItem.FileName);
                    var oldFileName = fileName;
                    var fileInfo = new FileInfo(fileName);

                    if (!fileInfo.Exists)
                    {
                        fileInfo = CommonLocation.GetFiles().Where(f => f.Name == fileInfo.Name).Single();
                        fileName = fileInfo.FullName;
                    }

                    if (!string.IsNullOrEmpty(projectItem.TargetFileName))
                    {
                        var path = Path.Combine(projectItem.Folder, projectItem.TargetFileName);
                        var targetFileName = Path.Combine(copyToPath, path);
                        var targetFileInfo = new FileInfo(targetFileName);

                        if (fileName != targetFileName)
                        {
                            if (!targetFileInfo.Directory.Exists)
                            {
                                targetFileInfo.Directory.Create();
                            }

                            if (overwriteExisting)
                            {
                                if (targetFileInfo.Exists)
                                {
                                    if (targetFileInfo.IsReadOnly)
                                    {
                                        targetFileInfo.MakeWritable();
                                    }

                                    targetFileInfo.Delete();
                                }
                            }

                            File.Copy(fileName, targetFileName);

                            fileName = targetFileName;
                        }
                    }
                    else if (oldFileName != fileName)
                    {
                        File.Copy(oldFileName, fileName);
                    }

                    if (projectItem.ReplaceParameters)
                    {
                        if (skip == null || !skip.Any(s => s.ToLower() == projectItem.FileName.ToLower()))
                        {
                            fileInfo = new FileInfo(fileName);

                            fileInfo.MakeWritable();

                            using (var stream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite))
                            {
                                ReplaceParameters(stream, parameters);
                            }
                        }
                    }
                }
            }
        }

        public override void ReplaceParameters(Stream stream, ICodeTemplateParameters parameters)
        {
            var reader = new StreamReader(stream);
            var writer = new StreamWriter(stream);
            var text = reader.ReadToEnd();
            text = ReplaceParameters(text, parameters);

            stream.SetLength(0);

            writer.Write(text);
            writer.Flush();

            writer.Close();
        }

        public override string ReplaceParameters(string text, ICodeTemplateParameters parameters)
        {
            var regex = new Regex(@"(?<ifblock>(?<ifstatement>\$if\$\s*?\((?<predicate>[\w\s\$\>\<\=\.]*?)\)[\s\r\n\t]*)(?<consequent>.*?)(?<endif>\$endif\$[\s\r\n\t]*))", RegexOptions.Multiline | RegexOptions.Singleline);

            while (regex.IsMatch(text))
            {
                foreach (var match in regex.Matches(text).Cast<Match>())
                {
                    var ifBlockGroup = match.Groups["ifblock"];
                    var ifStatementGroup = match.Groups["ifstatement"];
                    var predicateGroup = match.Groups["predicate"];
                    var endIfGroup = match.Groups["endif"];

                    var ifBlock = ifBlockGroup.Value;
                    var predicate = predicateGroup.Value;
                    var consequent = match.Groups["consequent"].Value;

                    // this is as far as we are parsing for now, TODO - handle better by parsing predicate

                    if (predicate == "$targetframeworkversion$ >= 3.5" || predicate == "$targetframeworkversion$ >= 4.0")  // of course this is a temp hack
                    {
                        var capture = ifStatementGroup.Captures.Cast<Capture>().Single();
                        var length = capture.Length;

                        text = text.Remove(capture.Index, length);

                        capture = endIfGroup.Captures.Cast<Capture>().Single();

                        text = text.Remove(capture.Index - length, capture.Length);
                    }

                    break;
                }
            }

            text = ReplaceParameterText(text, parameters);
            return text;
        }
    }
}
