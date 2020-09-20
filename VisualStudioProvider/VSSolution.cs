using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using CodeInterfaces;
using Metaspec;
using Utils;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Collections;

namespace VisualStudioProvider
{
    public delegate void OnParserErrorHandler(object sender, EventArgs<Exception> args);
    public delegate void OnParserErrorRawHandler(object sender, Exception ex);

    public class VSSolution : IVSSolution
    {
        //internal class SolutionParser 
        //Name: Microsoft.Build.Construction.SolutionParser 
        //Assembly: Microsoft.Build, Version=4.0.0.0 

        static readonly Type s_SolutionParser;
        static readonly PropertyInfo s_SolutionParser_solutionReader;
        static readonly MethodInfo s_SolutionParser_parseSolution;
        static readonly PropertyInfo s_SolutionParser_projects;
        private List<VSProject> projects;
        private ISolution iSolution;
        public static event OnParserErrorHandler OnParserError;
        public static event OnParserErrorRawHandler OnParserErrorRaw;
        private bool loadOnDemand;
        private object solutionParser;
        private List<KeyValuePair<string, string>> parentChildren;
        private IList projectArray;
        public IProjectRoot ProjectRoot { get; set; }
        public IArchitectureLayer ArchitectureLayer { get; set; }
        public string FileName { get; private set; }
        public string Hash { get; private set; }
        public Guid InstanceId { get; private set; }

        public ISolution ISolution
        {
            get 
            {
                if (iSolution == null)
                {
                    var iSolutionParser = new SolutionParser();

                    iSolution = iSolutionParser.Parse(FileName);
                }

                return iSolution; 
            }
        }

        public IProjectStructure FindLayer(string layerName)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get
            {
                return Path.GetFileNameWithoutExtension(FileName);
            }
        }

        public int ProjectCount
        {
            get
            {
                if (s_SolutionParser == null)
                {
                    throw new InvalidOperationException("Can not find type 'Microsoft.Build.Construction.SolutionParser' are you missing a assembly reference to 'Microsoft.Build.dll'?");
                }

                if (projectArray == null)
                {
                    Reparse();
                    projectArray = (Array)s_SolutionParser_projects.GetValue(solutionParser, null);
                }

                return projectArray.Count;
            }
        }

        public IEnumerable<IVSProject> Projects
        {
            get
            {
                if (loadOnDemand)
                {
                    var parentChildren = new List<KeyValuePair<string, string>>();
                    var currentProjectIndex = 0;

                    if (this.InPotentiallyPartialIterator())
                    {
                        var fullProjects = this.Projects;

                        foreach (var project in fullProjects.ToList())
                        {
                            yield return project;
                        }

                        yield break;
                    }

                    if (s_SolutionParser == null)
                    {
                        throw new InvalidOperationException("Can not find type 'Microsoft.Build.Construction.SolutionParser' are you missing a assembly reference to 'Microsoft.Build.dll'?");
                    }

                    if (this.ProjectCount != 0 && projects == null)
                    {
                        Reparse();

                        projects = new List<VSProject>();

                        if (projectArray == null)
                        {
                            projectArray = (Array)s_SolutionParser_projects.GetValue(solutionParser, null);
                        }
                    }
                    else
                    {
                        foreach (var project in projects)
                        {
                            currentProjectIndex++;
                            yield return project;
                        }
                    }

                    for (int i = currentProjectIndex; i < projectArray.Count; i++)
                    {
                       VSProject project = null;

                        try
                        {
                            project = new VSProject(this, projectArray[i], loadOnDemand);

                            projects.Add(project);
                        }
                        catch (Exception ex)
                        {
                            if (OnParserError != null)
                            {
                                OnParserError(projectArray[i], new EventArgs<Exception>(ex));
                            }

                            if (OnParserErrorRaw != null)
                            {
                                OnParserErrorRaw(projectArray[i], ex);
                            }
                        }

                        if (project != null)
                        {
                            currentProjectIndex = i + 1;

                            yield return project;
                        }
                    }

                    foreach (var pair in parentChildren)
                    {
                        var child = projects.Single(p => p.ProjectGuid == pair.Key);
                        var parent = projects.Single(p => p.ProjectGuid == pair.Value);

                        child.ParentProject = parent;
                    }

                    currentProjectIndex = 0;
                    loadOnDemand = false;
                }
                else
                {
                    foreach (var project in projects)
                    {
                        yield return project;
                    }
                }
            }
        }

        static VSSolution()
        { 
            s_SolutionParser = Type.GetType("Microsoft.Build.Construction.SolutionFile, Microsoft.Build, Version=15.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            s_SolutionParser_solutionReader = s_SolutionParser.GetProperty("SolutionReader", BindingFlags.NonPublic | BindingFlags.Instance);
            s_SolutionParser_projects = s_SolutionParser.GetProperty("ProjectsInOrder", BindingFlags.Public | BindingFlags.Instance);
            s_SolutionParser_parseSolution = s_SolutionParser.GetMethod("ParseSolution", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public string SolutionPath
        {
            get
            {
                var file = new FileInfo(FileName);

                return file.DirectoryName;
            }
        }

        public void Reparse()
        {
            var fileInfo = new FileInfo(FileName);

            if (s_SolutionParser == null)
            {
                throw new InvalidOperationException("Can not find type 'Microsoft.Build.Construction.SolutionParser' are you missing a assembly reference to 'Microsoft.Build.dll'?");
            }

            parentChildren = new List<KeyValuePair<string, string>>();
            solutionParser = s_SolutionParser.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);

            this.Hash = fileInfo.GetHash();

            using (var streamReader = new StreamReader(FileName))
            {
                using (streamReader.MarkForReset())
                {
                    var contents = streamReader.ReadToEnd();
                    var regex = new Regex(@"GlobalSection\(NestedProjects\) = preSolution(?<nestedprojects>.*?)EndGlobalSection", RegexOptions.Singleline);

                    if (regex.IsMatch(contents))
                    {
                        var matchContents = regex.Match(contents);
                        var nestedProjects = matchContents.Groups["nestedprojects"].Value;

                        regex = new Regex(@"\s*(?<child>\{[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}\})\s*=\s*(?<parent>\{[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}\})", RegexOptions.Singleline);

                        if (regex.IsMatch(nestedProjects))
                        {
                            foreach (var match in regex.Matches(nestedProjects).Cast<Match>())
                            {
                                var child = match.Groups["child"].Value;
                                var parent = match.Groups["parent"].Value;

                                parentChildren.Add(new KeyValuePair<string, string>(child, parent));
                            }
                        }
                    }
                }

                s_SolutionParser_solutionReader.SetValue(solutionParser, streamReader, null);
                s_SolutionParser_parseSolution.Invoke(solutionParser, null);
            }

            if (projectArray == null)
            {
                projectArray = (IList)s_SolutionParser_projects.GetValue(solutionParser, null);
            }

            if (!loadOnDemand)
            {
                projects = new List<VSProject>();

                for (int i = 0; i < projectArray.Count; i++)
                {
                    try
                    {
                        projects.Add(new VSProject(this, projectArray[i]));
                    }
                    catch (Exception ex)
                    {
                        if (OnParserError != null)
                        {
                            OnParserError(projectArray[i], new EventArgs<Exception>(ex));
                        }

                        if (OnParserErrorRaw != null)
                        {
                            OnParserErrorRaw(projectArray[i], ex);
                        }
                    }
                }

                foreach (var pair in parentChildren)
                {
                    var child = projects.Single(p => p.ProjectGuid == pair.Key);
                    var parent = projects.Single(p => p.ProjectGuid == pair.Value);

                    child.ParentProject = parent;
                }
            }
        }

        public VSSolution(string solutionFileName, bool loadOnDemand = true)
        {
            this.FileName = solutionFileName;
            this.loadOnDemand = loadOnDemand;
            this.InstanceId = Guid.NewGuid();

            if (!loadOnDemand)
            {
                Reparse();
            }
        }

        public IEnumerable<IVSProjectItem> EDMXModels
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {
        }

        public IEnumerable<IVSProjectItem> Schemas
        {
            get { throw new NotImplementedException(); }
        }

        public override int GetHashCode()
        {
            return this.Hash.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is VSSolution)
            {
                return Equals((VSSolution)obj);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(VSSolution project)
        {
            return project.Hash == this.Hash;
        }

        public static bool operator ==(VSSolution a, VSSolution b)
        {
            bool equal;

            if (CompareExtensions.CheckNullEquality(a, b, out equal))
            {
                return equal;
            }

            return a.Equals(b);
        }

        public static bool operator !=(VSSolution a, VSSolution b)
        {
            bool equal;

            if (CompareExtensions.CheckNullEquality(a, b, out equal))
            {
                return !equal;
            }

            return !a.Equals(b);
        }
    }
}