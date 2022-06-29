using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Xml;
using CodeInterfaces;
using System.Collections;
using Metaspec;
using System.Xml.XPath;
using Microsoft.Build.Construction;
using Microsoft.Build.CommandLine;
using Microsoft.Build.Framework;
using System.Text.RegularExpressions;
using VisualStudioProvider.Silverlight;
using System.Text;
using Utils;

namespace VisualStudioProvider
{
    [DebuggerDisplay("{Name}, {RelativePath}, {ProjectType}")]
    public class VSProject : IVSProject, ILogger, IEventSource
    {
        static readonly Type s_ProjectInSolution;
        static readonly Type s_ProjectRootElement;
        static readonly Type s_ProjectRootElementCache;
        static readonly PropertyInfo s_ProjectInSolution_ProjectName;
        static readonly PropertyInfo s_ProjectInSolution_ProjectType;
        static readonly PropertyInfo s_ProjectInSolution_RelativePath;
        static readonly PropertyInfo s_ProjectInSolution_ProjectGuid;
        static readonly PropertyInfo s_ProjectInSolution_ParentProjectGuid;
        static readonly PropertyInfo s_ProjectRootElement_Items;
        static readonly PropertyInfo s_ProjectRootElement_Properties;

        private VSSolution solution;
        private VSProject parentProject;
        private List<IVSProject> childProjects;
        private string projectFileName;
        private object internalSolutionProject;
        private XPathDocument xPathDocument;
        private XmlNamespaceManager nsmgr;
        private IProject iProject;
        private List<VSProjectItem> items;
        private List<VSProjectProperty> properties;
        private ProjectElementContainer rootElementContainer;
        private ProjectRootElement rootElement;
        private LoggerVerbosity loggerVerbosity = LoggerVerbosity.Minimal;
        private string parameters = string.Empty;
        private string name;

        public event AnyEventHandler AnyEventRaised;
        public event BuildFinishedEventHandler BuildFinished;
        public event BuildStartedEventHandler BuildStarted;
        public event CustomBuildEventHandler CustomEventRaised;
        public event BuildErrorEventHandler ErrorRaised;
        public event BuildMessageEventHandler MessageRaised;
        public event ProjectFinishedEventHandler ProjectFinished;
        public event ProjectStartedEventHandler ProjectStarted;
        public event BuildStatusEventHandler StatusEventRaised;
        public event TargetFinishedEventHandler TargetFinished;
        public event TargetStartedEventHandler TargetStarted;
        public event TaskFinishedEventHandler TaskFinished;
        public event TaskStartedEventHandler TaskStarted;
        public event BuildWarningEventHandler WarningRaised;
        private ICollection itemCollection;
        private bool loadOnDemand;
        private bool loadOnDemandProperties;
        private bool includeHiddenItems;
        private bool allItemsLoaded;
        public string OutputFile { get; private set; }
        public string OutputPath { get; private set; }
        public string ProjectType { get; private set; }
        public string RelativePath { get; private set; }
        public string ProjectGuid { get; private set; }
        public string ParentProjectGuid { get; private set; }
        public IProjectRoot ProjectRoot { get; set; }
        public IArchitectureLayer ArchitectureLayer { get; set; }
        public bool IsSilverlightApplication { get; private set; }
        public string XAPFilename { get; private set; }
        public string Hash { get; private set; }
        public Guid InstanceId { get; private set; }

        public string GetOutputFile(string configuration)
        {
            string outputFile = null;
            var platform = rootElement.Properties.SingleOrDefault(p2 => p2.Name == "Platform");
            string platformValue = null;

            if (platform == null)
            {
                platformValue = rootElement.Properties.First(p2 => p2.Name == "PlatformTarget").Value;
            }
            else
            {
                platformValue = platform.Value;
            }

            var outputPathElement = rootElement.Properties.SingleOrDefault(p =>
            {
                if (p.Name == "OutputPath")
                {
                    var condition = Regex.Replace(p.Parent.Condition, @"\s", "");

                    if (condition == string.Format("'$(Configuration)|$(Platform)'=='{0}|{1}'", configuration, platformValue))
                    {
                        return true;
                    }
                    else if (platformValue == "AnyCPU")
                    {
                        var pattern = string.Format(@"'\$\(Configuration\)\|\$\(Platform\)'=='{0}\|\w+?'", configuration);

                        return Regex.IsMatch(condition, pattern);
                    }

                    return false;
                }
                else
                {
                    return false;
                }

            });

            if (outputPathElement != null)
            {
                var pathValue = outputPathElement.Value;

                if (pathValue.Contains("$(Configuration"))
                {
                    pathValue = pathValue.Replace("$(Configuration)", configuration);
                }

                outputFile = Path.GetFullPath(Path.Combine(Path.Combine(Path.GetDirectoryName(projectFileName), pathValue, Path.GetFileName(this.OutputFile))));
            }

            return outputFile;
        }

        public string GetOutputPath(string configuration)
        {
            string outputPath = null;
            var platform = rootElement.Properties.SingleOrDefault(p2 => p2.Name == "Platform");
            string platformValue = null;

            if (platform == null)
            {
                platformValue = rootElement.Properties.First(p2 => p2.Name == "PlatformTarget").Value;
            }
            else
            {
                platformValue = platform.Value;
            }

            var outputPathElement = rootElement.Properties.SingleOrDefault(p =>
            {
                if (p.Name == "OutputPath")
                {
                    var condition = Regex.Replace(p.Parent.Condition, @"\s", "");

                    if (condition == string.Format("'$(Configuration)|$(Platform)'=='{0}|{1}'", configuration, platformValue))
                    {
                        return true;
                    }
                    else if (platformValue == "AnyCPU")
                    {
                        var pattern = string.Format(@"'\$\(Configuration\)\|\$\(Platform\)'=='{0}\|\w+?'", configuration);

                        return Regex.IsMatch(condition, pattern);
                    }

                    return false;
                }
                else
                {
                    return false;
                }

            });

            if (outputPathElement != null)
            {
                var pathValue = outputPathElement.Value;

                if (pathValue.Contains("$(Configuration"))
                {
                    pathValue = pathValue.Replace("$(Configuration)", configuration);
                }

                outputPath = pathValue;
            }

            return outputPath;
        }

        public string Name 
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public bool IncludeHiddenItems
        {
            get
            {
                return includeHiddenItems;
            }

            set
            {
                var include = includeHiddenItems;

                includeHiddenItems = value;

                if (!include && includeHiddenItems)
                {

                    if (!loadOnDemand)
                    {
                        // will force the iteration

                        GetItems().ToList();
                    }
                }
            }
        }

        public IVSSolution ParentSolution
        {
            get
            {
                return solution;
            }
        }

        public IVSProject ParentProject
        {
            get
            {
                return parentProject;
            }
            
            internal set
            {
                parentProject = (VSProject) value;

                parentProject.childProjects.Add(this);
            }
        }

        public string ParentHierarchy
        {
            get
            {
                var builder = new StringBuilder();
                var parent = this.ParentProject;
                var list = new List<IVSProject>();

                while (parent != null)
                {
                    list.Add(parent);
                    parent = parent.ParentProject;
                }

                foreach (var parent2 in list.Reverse<IVSProject>())
                {
                    builder.Append(parent2.Name + @"\");
                }

                if (builder.Length > 0)
                {
                    builder.Remove(builder.Length - 1, 1);
                }

                return builder.ToString();
            }
        }

        public string Hierarchy
        {
            get
            {
                var builder = new StringBuilder();
                var parent = this.ParentProject;
                var list = new List<IVSProject>();

                list.Add(this);

                while (parent != null)
                {
                    list.Add(parent);
                    parent = parent.ParentProject;
                }

                foreach (var parent2 in list.Reverse<IVSProject>())
                {
                    builder.Append(parent2.Name + @"\");
                }

                if (builder.Length > 0)
                {
                    builder.Remove(builder.Length - 1, 1);
                }

                return builder.ToString();
            }
        }

        public List<IVSProject> ChildProjects
        {
            get 
            { 
                return childProjects; 
            }
        }

        public string FileName
        {
            get
            {
                return projectFileName;
            }
        }

        public IProject IProject
        {
            get 
            {
                if (iProject == null)
                {
                    iProject = solution.ISolution.getProjects().SingleOrDefault(p => p.getPath() == this.projectFileName);
                }

                return iProject; 
            }
        }

        public string XPathNamespacePrefix
        {
            get
            {
                return "vs";
            }
        }

        public string DefaultNamespace 
        {
            get
            {
                if (iProject != null && iProject is ICsProject && ((ICsProject)iProject).getNamespace() != null)
                {
                    return ((ICsProject)iProject).getNamespace().name;
                }
                else if (iProject is ICsProject)
                {
                    var iter = this.Select(string.Format("{0}:Project/{0}:PropertyGroup/{0}:RootNamespace", this.XPathNamespacePrefix));

                    iter.MoveNext();

                    return iter.Current.Value;
                }
                else
                {
                    return null;
                }
            }
        }

        public string OutputType
        {
            get
            {
                var iter = this.Select(string.Format("{0}:Project/{0}:PropertyGroup/{0}:OutputType", this.XPathNamespacePrefix));

                if (iter == null)
                {
                    return string.Empty;
                }
                else
                {
                    iter.MoveNext();

                    return iter.Current.Value;
                }
            }
        }

        public IEnumerable<IVSProject> SilverlightWebTargets
        {
            get
            {
                return this.ParentSolution.Projects.Where(p => p.SilverlightApplicationList.Any(s => s.SilverlightProject == this));
            }
        }

        public IEnumerable<ISilverlightApp> SilverlightApplicationList
        {
            get
            {
                var list = new List<ISilverlightApp>();
                var listProperty = this.properties.SingleOrDefault(p => p.Name == "SilverlightApplicationList");

                if (listProperty != null)
                {
                    if (!string.IsNullOrWhiteSpace(listProperty.Value))
                    {
                        var apps = listProperty.Value.Split(',');

                        foreach (var appText in apps)
                        {
                            var appParts = appText.Split('|');
                            var guid = appParts[0];
                            var path = appParts[2];
                            var configSpecificFolders = appParts[3];
                            var project = solution.Projects.SingleOrDefault(p => p.ProjectGuid == guid);

                            list.Add(new SilverlightApp(Guid.Parse(guid), project, path, bool.Parse(configSpecificFolders)));
                        }
                    }
                }
                    
                return list;
            }
        }

        static VSProject()
        {
            s_ProjectInSolution = Type.GetType("Microsoft.Build.Construction.ProjectInSolution, Microsoft.Build, Version=15.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);

            s_ProjectInSolution_ProjectName = s_ProjectInSolution.GetProperty("ProjectName", BindingFlags.Public | BindingFlags.Instance);
            s_ProjectInSolution_ProjectType = s_ProjectInSolution.GetProperty("ProjectType", BindingFlags.Public | BindingFlags.Instance);
            s_ProjectInSolution_RelativePath = s_ProjectInSolution.GetProperty("RelativePath", BindingFlags.Public | BindingFlags.Instance);
            s_ProjectInSolution_ProjectGuid = s_ProjectInSolution.GetProperty("ProjectGuid", BindingFlags.Public | BindingFlags.Instance);
            s_ProjectInSolution_ParentProjectGuid = s_ProjectInSolution.GetProperty("ProjectGuid", BindingFlags.Public | BindingFlags.Instance);

            s_ProjectRootElement = Type.GetType("Microsoft.Build.Construction.ProjectRootElement, Microsoft.Build, Version=15.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            s_ProjectRootElementCache = Type.GetType("Microsoft.Build.Evaluation.ProjectRootElementCache, Microsoft.Build, Version=15.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);

            s_ProjectRootElement_Items = s_ProjectRootElement.GetProperty("Items", BindingFlags.Public | BindingFlags.Instance);
            s_ProjectRootElement_Properties = s_ProjectRootElement.GetProperty("Properties", BindingFlags.Public | BindingFlags.Instance);
        }

        private IEnumerable<IVSProjectItem> GetItems()
        {
            ICollection propertyCollection;
            List<VSProjectItem> references;
            int count;
            var dependencyItemsToResolve = new List<VSProjectItem>();
            List<VSProjectItem> unsavedItems;

            if (this.InPotentiallyPartialIterator())
            {
                var fullItems = this.GetItems();

                foreach (var item in fullItems.ToList())
                {
                    yield return item;
                }

                yield break;
            }

            itemCollection = (ICollection)s_ProjectRootElement_Items.GetValue(rootElement, null);
            count = itemCollection.Count;

            unsavedItems = items.Where(i => i.IsUnsaved).ToList();

            items.Clear();

            foreach (var unsavedItem in unsavedItems)
            {
                items.Add(unsavedItem);

                if (unsavedItem.DependentUponMetadata == null)
                {
                    yield return unsavedItem;
                }
            }

            foreach (var item in itemCollection)
            {
                var projectItem = new VSProjectItem(this, item);

                if (!unsavedItems.Any(i => i.FilePath == projectItem.FilePath))
                {
                    items.Add(projectItem);

                    if (projectItem.DependentUponMetadata == null)
                    {
                        yield return projectItem;
                    }
                }
            }

            references = items.Where(i => i.ItemType.IsOneOf("Reference", "ProjectReference")).ToList();

            if (references.Count > 0)
            {
                var referenceFolder = new VSProjectItem(this, "References");

                items.Add(referenceFolder);

                yield return referenceFolder;

                foreach (var reference in references)
                {
                    reference.ParentItem = referenceFolder;
                    referenceFolder.AddChild(reference);
                }
            }

            foreach (var item in items.Where(i => i.ItemType.NotOneOf("Reference", "ProjectReference")).ToList())
            {
                if (item.ProjectPath != null && !item.HasDependentUpon)
                {
                    VSProjectItem parentItem = null;
                    var parts = item.ProjectPath.Split('\\', StringSplitOptions.RemoveEmptyEntries);
                    string path = null;

                    foreach (var part in parts)
                    {
                        if (path == null)
                        {
                            path = part;
                        }
                        else
                        {
                            path += @"\" + part;
                        }

                        if (part == item.Name)
                        {
                            if (parentItem != null)
                            {
                                item.ParentItem = parentItem;
                                parentItem.AddChild(item);
                            }
                        }
                        else
                        {
                            if (items.Any(i => i.RelativePath == path))
                            {
                                parentItem = items.Single(i => i.RelativePath == path);
                            }
                            else
                            {
                                var newItem = new VSProjectItem(this, path, parentItem);

                                items.Add(newItem);

                                yield return newItem;

                                parentItem = newItem;
                            }
                        }
                    }
                }
                else
                {
                    if (item.HasDependentUpon)
                    {
                        dependencyItemsToResolve.Add(item);
                    }
                }
            }

            if (includeHiddenItems)
            {
                var directoryName = Path.GetDirectoryName(this.FileName);
                var directory = new DirectoryInfo(directoryName);
                Action<DirectoryInfo> recurseItems = null;
                Action<string> addParts = null;
                var hiddenItems = new List<VSProjectItem>();

                addParts = (p) =>
                {
                    var parts = p.Split('\\', StringSplitOptions.RemoveEmptyEntries);
                    string path = null;
                    VSProjectItem parentItem = null;

                    foreach (var part in parts)
                    {
                        if (path == null)
                        {
                            path = part;
                        }
                        else
                        {
                            path += @"\" + part;
                        }

                        if (items.Any(i => i.RelativePath == path))
                        {
                            VSProjectItem item;

                            if (unsavedItems.Any(i => i.RelativePath == path))
                            {
                                item = unsavedItems.Single(i => i.RelativePath == path);
                            }
                            else
                            {
                                item = items.Single(i => i.RelativePath == path);

                                if (parentItem != null)
                                {
                                    if (item.ParentItem != (IVSProjectItem)parentItem)
                                    {
                                        item.ParentItem = parentItem;
                                        parentItem.AddChild(item);
                                    }
                                }
                            }

                            parentItem = item;
                        }
                        else
                        {
                            if (unsavedItems.Any(i => i.RelativePath == path))
                            {
                                var item = unsavedItems.Single(i => i.RelativePath == path);

                                parentItem = item;
                            }
                            else
                            {
                                var newItem = new VSProjectItem(this, path, parentItem, true);

                                hiddenItems.Add(newItem);
                                items.Add(newItem);

                                parentItem = newItem;
                            }
                        }
                    }
                };

                recurseItems = (d) =>
                {
                    foreach (var subDirectory in d.GetDirectories())
                    {
                        var relativePath = subDirectory.FullName.RemoveStart(directory.FullName + "\\");

                        addParts(relativePath);

                        recurseItems(subDirectory);
                    }

                    foreach (var file in d.GetFiles())
                    {
                        var relativePath = file.FullName.RemoveStart(directory.FullName + "\\");

                        addParts(relativePath);
                    }
                };

                recurseItems(directory);

                foreach (var hiddenItem in hiddenItems)
                {
                    yield return hiddenItem;
                }
            }

            propertyCollection = (ICollection)s_ProjectRootElement_Properties.GetValue(rootElement, null);
            properties.Clear();

            foreach (var item in propertyCollection)
            {
                properties.Add(new VSProjectProperty(this, (ProjectPropertyElement) item));
            }

            foreach (var dependencyItem in dependencyItemsToResolve)
            {
                try
                {
                    var dependentUponItem = items.Single(i => i.RelativePath == dependencyItem.RelativePath.RemoveEnd(dependencyItem.Name) + dependencyItem.DependentUponMetadata.Value);

                    if (!dependentUponItem.ChildItems.Any(i => i.FilePath == dependencyItem.FilePath))
                    {
                        dependencyItem.DependentUpon = dependencyItem;

                        if (dependencyItem.ParentItem != null && dependencyItem.ParentItem.ChildItems.Any(i => i.FilePath == dependencyItem.FilePath))
                        {
                            ((VSProjectItem)dependencyItem.ParentItem).Remove(dependencyItem);
                        }

                        dependentUponItem.AddChild(dependencyItem);
                        dependencyItem.ParentItem = dependentUponItem;

                        if (items.Any(i => i.FilePath == dependencyItem.FilePath))
                        {
                            var existingItem = items.Single(i => i.FilePath == dependencyItem.FilePath);

                            existingItem.IsSubItem = true;
                        }
                    }
                }
                catch
                {
                }
            }

            loadOnDemand = false;
        }

        private IEnumerable<IVSProjectProperty> GetProperties()
        {
            var propertyCollection = (ICollection)s_ProjectRootElement_Properties.GetValue(rootElement, null);
            properties.Clear();

            foreach (var item in propertyCollection)
            {
                properties.Add(new VSProjectProperty(this, (ProjectPropertyElement) item));
            }

            return properties;
        }

        public string this[string propertyName]
        {
            get
            {
                return properties.Single(p => p.Name == propertyName).Value;
            }
        }

        public IEnumerable<IVSProjectItem> Items
        {
            get
            {
                if (loadOnDemand)
                {
                    return this.GetItems();
                }
                else
                {
                    return items;
                }
            }
        }

        public IEnumerable<IVSProjectItem> CompileItems
        {
            get
            {
                return this.Items.Where(i => i.ItemType == "Compile");
            }
        }

        public IEnumerable<IVSProjectProperty> Properties
        {
            get
            {
                if (loadOnDemandProperties)
                {
                    loadOnDemandProperties = false;

                    return this.GetProperties();
                }
                else
                {
                    return properties;
                }
            }
        }

        public void AddCompileFile(string fileNameSource, string relativeTargetPath = null)
        {
            int count;
            var fileInfo = new FileInfo(projectFileName);
            string fileRelativePath;
            ProjectItemElement itemElement;
            var projectDirectory = fileInfo.DirectoryName;
            var itemGroup = rootElementContainer.Children.OfType<ProjectItemGroupElement>().Where(p => p.Children.Cast<ProjectItemElement>().Any(i => i.ItemType == "Compile")).FirstOrDefault();

            if (itemGroup == null)
            {
                itemGroup = rootElement.CreateItemGroupElement();
                rootElementContainer.AppendChild(itemGroup);
            }

            if (relativeTargetPath != null)
            {
                fileInfo = new FileInfo(Path.Combine(projectDirectory, relativeTargetPath));

                if (!fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }

                File.Copy(fileNameSource, fileInfo.FullName);
            }
            else
            {
                fileInfo = new FileInfo(fileNameSource);
            }

            itemCollection = (ICollection)s_ProjectRootElement_Items.GetValue(rootElement, null);
            count = itemCollection.Count;

            fileRelativePath = fileInfo.GetRelativePath(projectDirectory);

            itemElement = itemGroup.AddItem("Compile", fileRelativePath);

            itemCollection = (ICollection)s_ProjectRootElement_Items.GetValue(rootElement, null);
            count = itemCollection.Count;

            AddToItems(fileNameSource, itemElement);
        }

        public void RemoveItem(string relativePath)
        {
            int count;
            ProjectItemElement itemElement;
            var fileInfo = new FileInfo(projectFileName);
            var itemGroup = rootElementContainer.Children.OfType<ProjectItemGroupElement>()
                .Where(p => p.Children.Cast<ProjectItemElement>()
                .Any(i => i.Include == relativePath)).Single();

            itemCollection = (ICollection)s_ProjectRootElement_Items.GetValue(rootElement, null);
            count = itemCollection.Count;

            itemElement = itemGroup.Children.OfType<ProjectItemElement>().Single(i => i.Include == relativePath);
            itemGroup.RemoveChild(itemElement);

            itemCollection = (ICollection)s_ProjectRootElement_Items.GetValue(rootElement, null);
            count = itemCollection.Count;
        }

        public bool HasItem(string relativePath)
        {
            var fileInfo = new FileInfo(projectFileName);
            var hasItem = rootElementContainer.Children.OfType<ProjectItemGroupElement>()
                .Where(p => p.Children.Cast<ProjectItemElement>()
                .Any(i => i.Include == relativePath)).Any();

            return hasItem;
        }

        public void AddItem(string fileNameSource, string itemType, string relativeTargetPath = null)
        {
            int count;
            var fileInfo = new FileInfo(projectFileName);
            string fileRelativePath;
            ProjectItemElement itemElement;
            var projectDirectory = fileInfo.DirectoryName;
            var itemGroup = rootElementContainer.Children.OfType<ProjectItemGroupElement>()
                .Where(p => p.Children.Cast<ProjectItemElement>()
                .Any(i => i.ItemType == "Compile")).Single();

            if (relativeTargetPath != null)
            {
                fileInfo = new FileInfo(Path.Combine(projectDirectory, relativeTargetPath));

                if (!fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }

                File.Copy(fileNameSource, fileInfo.FullName);
            }
            else
            {
                fileInfo = new FileInfo(fileNameSource);
            }

            itemCollection = (ICollection)s_ProjectRootElement_Items.GetValue(rootElement, null);
            count = itemCollection.Count;

            fileRelativePath = fileInfo.GetRelativePath(projectDirectory);

            itemElement = itemGroup.AddItem(itemType, fileRelativePath);

            itemCollection = (ICollection)s_ProjectRootElement_Items.GetValue(rootElement, null);
            count = itemCollection.Count;

            AddToItems(fileNameSource, itemElement);
        }

        public void AddItem(string fileNameSource, string itemType, IVSProjectItem dependentUpon)
        {
            int count;
            ProjectMetadataElement metadataElement;
            var fileInfo = new FileInfo(projectFileName);
            string fileRelativePath;
            var projectDirectory = fileInfo.DirectoryName;
            ProjectItemElement itemElement;
            var itemGroup = rootElementContainer.Children.OfType<ProjectItemGroupElement>()
                .Where(p => p.Children.Cast<ProjectItemElement>()
                .Any(i => i.ItemType == "Compile")).Single();

            fileInfo = new FileInfo(fileNameSource);

            itemCollection = (ICollection)s_ProjectRootElement_Items.GetValue(rootElement, null);
            count = itemCollection.Count;

            fileRelativePath = fileInfo.GetRelativePath(projectDirectory);

            itemElement = itemGroup.AddItem(itemType, fileRelativePath);

            itemCollection = (ICollection)s_ProjectRootElement_Items.GetValue(rootElement, null);
            count = itemCollection.Count;

            metadataElement = (ProjectMetadataElement) VSProjectItem.s_ProjectItemElement_AddMetadata.Invoke(itemElement, new object[] { "DependentUpon", Path.GetFileName(dependentUpon.FileName) });

            AddToItems(fileNameSource, itemElement);
        }

        private void AddToItems(string fileNameSource, ProjectItemElement element, bool isUnsaved = true)
        {
            var relativePath = fileNameSource.RemoveStart(Path.GetDirectoryName(this.FileName) + @"\");
            var item = new VSProjectItem(this, element);
            var existingItem = items.SingleOrDefault(i => i.FilePath == item.FilePath);

            if (existingItem != null) 
            {
                if (existingItem.IsHidden)
                {
                    items.Remove(existingItem);
                }
                else
                {
                    DebugUtils.Break();
                }
            }

            items.Add(item);

            if (item.ProjectPath == null)
            {
                DebugUtils.Break();
            }
            else
            {
                if (item.ProjectPath != null && !item.HasDependentUpon)
                {
                    VSProjectItem parentItem = null;
                    var parts = item.ProjectPath.Split('\\', StringSplitOptions.RemoveEmptyEntries);
                    string path = null;

                    foreach (var part in parts)
                    {
                        if (path == null)
                        {
                            path = part;
                        }
                        else
                        {
                            path += @"\" + part;
                        }

                        if (part == item.Name)
                        {
                            if (parentItem != null)
                            {
                                if (item.ParentItem != (IVSProjectItem)parentItem)
                                {
                                    if (parentItem.ChildItems.Any(i => i.FilePath == item.FilePath))
                                    {
                                        existingItem = (VSProjectItem) parentItem.ChildItems.Single(i => i.FilePath == item.FilePath);

                                        if (existingItem.IsHidden)
                                        {
                                            parentItem.Remove(existingItem);
                                        }
                                    }

                                    item.ParentItem = parentItem;
                                    parentItem.AddChild(item);
                                }
                            }
                        }
                        else
                        {
                            if (items.Any(i => i.RelativePath == path))
                            {
                                parentItem = items.Single(i => i.RelativePath == path);

                                if (parentItem.IsHidden)
                                {
                                    parentItem.IsHidden = false;
                                }
                            }
                            else
                            {
                                var newItem = new VSProjectItem(this, path, parentItem);

                                items.Add(newItem);
                                newItem.IsUnsaved = isUnsaved;

                                parentItem = newItem;
                            }
                        }
                    }
                }
                else
                {
                    var dependencyItem = item;
                    var dependentUponItem = items.Single(i => i.RelativePath == dependencyItem.RelativePath.RemoveEnd(dependencyItem.Name) + dependencyItem.DependentUponMetadata.Value);

                    dependencyItem.IsUnsaved = isUnsaved;

                    dependencyItem.DependentUpon = dependentUponItem;
                    dependentUponItem.AddChild(dependencyItem);

                    if (dependencyItem.ParentItem != null && dependencyItem.ParentItem.ChildItems.Any(i => i.FilePath == dependencyItem.FilePath))
                    {
                        ((VSProjectItem)dependencyItem.ParentItem).Remove(dependencyItem);
                    }

                    dependencyItem.ParentItem = dependentUponItem;
                }
            }
        }

        public void RemoveItem(VSProjectItem item)
        {
            ProjectElementContainer parent = ((ProjectElement)item.InternalProjectItem).Parent;

            parent.RemoveChild((ProjectElement) item.InternalProjectItem);
        }

        public void Save()
        {
            rootElement.Save(projectFileName);
        }

        public void Compile()
        {
            this.Save();

            MSBuildApp.Execute(this.FileName, null, null, new Dictionary<string, string>() 
            {
                {"Configuration", "Debug"},
                {"Platform", "Any CPU"},
                {"OutputPath", @"bin\Debug"}
            },  new ILogger[] { this }, loggerVerbosity);
        }

        public IEnumerable<IVSProject> ReferencedProjects
        {
            get
            {
                foreach (var item in this.Items)
                {
                    if (item.ItemType == "ProjectReference")
                    {
                        yield return new VSProject(item.FilePath);
                    }
                }
            }
        }

        public IEnumerable<string> ReferencedAssemblies
        {
            get
            {
                foreach (var item in this.Items)
                {
                    if (item.ItemType == "Reference")
                    {
                        yield return item.Name;
                    }
                }
            }
        }

        public bool IsWebApplication
        {
            get
            {
                return this.Properties.Any(p => p.Name == "ProjectTypeGuids" && p.Value.Split(';').Any(v => v.AsCaseless() == "{349C5851-65DF-11DA-9384-00065B846F21}"));
            }   
        }

        public XPathNodeIterator Select(string xPath)
        {
            if (this.ProjectType == "SolutionFolder")
            {
                return null;
            }

            if (xPathDocument == null)
            {
                var stream = File.OpenRead(projectFileName);

                stream.Seek(0, SeekOrigin.Begin);

                xPathDocument = new XPathDocument(stream);

                stream.Close();

                nsmgr = new XmlNamespaceManager(new NameTable());
                nsmgr.AddNamespace("vs", "http://schemas.microsoft.com/developer/msbuild/2003");
            }

            return xPathDocument.CreateNavigator().Select(xPath, nsmgr);
        }

        public XPathNodeIterator SelectFormat(string xPathFormatString)
        {
            var xPath = string.Format(xPathFormatString, this.XPathNamespacePrefix);

            return this.Select(xPath);
        }

        public VSProject(VSSolution solution, object internalSolutionProject, bool loadOnDemand = false)
        {
            this.loadOnDemand = loadOnDemand;
            this.loadOnDemandProperties = loadOnDemand;
            this.Name = s_ProjectInSolution_ProjectName.GetValue(internalSolutionProject, null) as string;
            this.ProjectType = s_ProjectInSolution_ProjectType.GetValue(internalSolutionProject, null).ToString();
            this.RelativePath = s_ProjectInSolution_RelativePath.GetValue(internalSolutionProject, null) as string;
            this.ProjectGuid = s_ProjectInSolution_ProjectGuid.GetValue(internalSolutionProject, null) as string;
            this.ParentProjectGuid = s_ProjectInSolution_ParentProjectGuid.GetValue(internalSolutionProject, null) as string;
            InstanceId = Guid.NewGuid();

            this.solution = solution;
            this.internalSolutionProject = internalSolutionProject;

            if (this.ProjectType != "SolutionFolder")
            {
                this.projectFileName = Path.GetFullPath(Path.Combine(solution.SolutionPath, this.RelativePath));
            }

            childProjects = new List<IVSProject>();
            items = new List<VSProjectItem>();
            properties = new List<VSProjectProperty>();

            if (this.ProjectType == "KnownToBeMSBuildFormat" || this.RelativePath.EndsWith(".wixproj"))
            {
                this.Parse();
            }
        }

        public VSProject(string fileName, bool loadOnDemand = false)
        {
            this.loadOnDemand = loadOnDemand;
            projectFileName = fileName;
            items = new List<VSProjectItem>();
            properties = new List<VSProjectProperty>();
            childProjects = new List<IVSProject>();
            InstanceId = Guid.NewGuid();

            this.Parse();
        }

        public void Reparse(bool includeHiddenItems = false)
        {
            this.Parse(includeHiddenItems);
        }

        private void Parse(bool includeHiddenItems = false)
        {
            var stream = File.OpenRead(projectFileName);
            var reader = XmlReader.Create(stream);
            var cache = s_ProjectRootElementCache.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(new object[] { true });
            var fileInfo = new FileInfo(projectFileName);
            var addPropertyElements = new List<VSProjectProperty>();

            this.includeHiddenItems = includeHiddenItems;

            this.Hash = fileInfo.GetHash();

            if (s_ProjectRootElement.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Any(c => c.GetParameters().Count() == 4 && c.GetParameters().First().ParameterType.Name == "XmlReader" && c.GetParameters().ElementAt(1).ParameterType.Name == "ProjectRootElementCache"))
            {
                rootElement = (ProjectRootElement)s_ProjectRootElement.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Where(c => c.GetParameters().Count() >= 2 && c.GetParameters().First().ParameterType.Name == "XmlReader" && c.GetParameters().ElementAt(1).ParameterType.Name == "ProjectRootElementCache").First().Invoke(new object[] { reader, cache, true, true });
            }
            else if (s_ProjectRootElement.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Any(c => c.GetParameters().Count() == 2 && c.GetParameters().First().ParameterType.Name == "XmlReader" && c.GetParameters().ElementAt(1).ParameterType.Name == "ProjectRootElementCache"))
            {
                rootElement = (ProjectRootElement)s_ProjectRootElement.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Where(c => c.GetParameters().Count() == 2 && c.GetParameters().First().ParameterType.Name == "XmlReader" && c.GetParameters().ElementAt(1).ParameterType.Name == "ProjectRootElementCache").First().Invoke(new object[] { reader, cache });
            }
            else
            {
                throw new Exception("Unable to find constructor in ProjectRootElementCache for VSProject parser");
            }

            if (rootElement.Properties.Count(p => p.Name == "AssemblyName") == 1)
            {
                var assemblyName = rootElement.Properties.SingleOrDefault(p => p.Name == "AssemblyName");
                ProjectPropertyElement outputPathElement;

                if (assemblyName != null)
                {
                    var configurationPair = rootElement.Properties.SingleOrDefault(p2 => p2.Name == "Configuration");

                    if (configurationPair == null)
                    {
                        var rawXml = rootElement.RawXml;
                    }
                    else
                    { 
                        var configuration = configurationPair.Value;
                        var platform = rootElement.Properties.SingleOrDefault(p2 => p2.Name == "Platform");
                        var isSilverlightAppProperty = rootElement.Properties.SingleOrDefault(p2 => p2.Name == "SilverlightApplication");
                        string platformValue = string.Empty;

                        if (this.name == null)
                        {
                            this.name = assemblyName.Value;
                        }

                        if (isSilverlightAppProperty != null)
                        {
                            IsSilverlightApplication = bool.Parse(isSilverlightAppProperty.Value);

                            if (IsSilverlightApplication)
                            {
                                var xapFileNameProperty = rootElement.Properties.SingleOrDefault(p2 => p2.Name == "XapFilename");

                                if (xapFileNameProperty != null)
                                {
                                    XAPFilename = xapFileNameProperty.Value;
                                }
                            }
                        }

                        if (platform == null)
                        {
                            platformValue = rootElement.Properties.First(p2 => p2.Name == "PlatformTarget").Value;
                        }
                        else
                        {
                            platformValue = platform.Value;
                        }

                        outputPathElement = rootElement.Properties.SingleOrDefault(p =>
                        {
                            if (p.Name == "OutputPath")
                            {
                                var condition = Regex.Replace(p.Parent.Condition, @"\s", "");

                                if (condition == string.Format("'$(Configuration)|$(Platform)'=='{0}|{1}'", configuration, platformValue))
                                {
                                    return true;
                                }
                                else if (platformValue == "AnyCPU")
                                {
                                    var pattern = string.Format(@"'\$\(Configuration\)\|\$\(Platform\)'=='{0}\|\w+?'", configuration);

                                    return Regex.IsMatch(condition, pattern);
                                }

                                return false;
                            }
                            else
                            {
                                return false;
                            }

                        });

                        if (outputPathElement != null)
                        {
                            OutputPath = outputPathElement.Value;
                        }

                        var outputType = rootElement.Properties.SingleOrDefault(p => p.Name == "OutputType").Value;
                        string outputExt = string.Empty;

                        switch (outputType)
                        {
                            case "Library":
                                outputExt = ".dll";
                                break;
                            case "Exe":
                                outputExt = ".exe";
                                break;
                            case "WinExe":
                                outputExt = ".exe";
                                break;
                            default:
                                Debugger.Break();
                                break;
                        }

                        if (OutputPath != null)
                        {
                            var directory = new FileInfo(this.projectFileName).DirectoryName;

                            this.OutputFile = Path.GetFullPath(Path.Combine(directory, OutputPath, assemblyName.Value + outputExt));
                        }
                    }
                }
            }
            else if (rootElement.Properties.Count(p => p.Name == "OutputName") == 1)
            {
                var outputName = rootElement.Properties.SingleOrDefault(p => p.Name == "OutputName");
                ProjectPropertyElement outputPathElement;

                if (outputName != null)
                {
                    var configuration = rootElement.Properties.Single(p2 => p2.Name == "Configuration").Value;
                    var platform = rootElement.Properties.SingleOrDefault(p2 => p2.Name == "Platform");
                    var isSilverlightAppProperty = rootElement.Properties.SingleOrDefault(p2 => p2.Name == "SilverlightApplication");
                    string platformValue = string.Empty;

                    if (this.name == null)
                    {
                        this.name = outputName.Value;
                    }

                    if (isSilverlightAppProperty != null)
                    {
                        IsSilverlightApplication = bool.Parse(isSilverlightAppProperty.Value);

                        if (IsSilverlightApplication)
                        {
                            var xapFileNameProperty = rootElement.Properties.SingleOrDefault(p2 => p2.Name == "XapFilename");

                            if (xapFileNameProperty != null)
                            {
                                XAPFilename = xapFileNameProperty.Value;
                            }
                        }
                    }

                    if (platform == null)
                    {
                        platformValue = rootElement.Properties.First(p2 => p2.Name == "PlatformTarget").Value;
                    }
                    else
                    {
                        platformValue = platform.Value;
                    }

                    outputPathElement = rootElement.Properties.SingleOrDefault(p =>
                    {
                        if (p.Name == "OutputPath")
                        {
                            var condition = Regex.Replace(p.Parent.Condition, @"\s", "");

                            if (condition == string.Format("'$(Configuration)|$(Platform)'=='{0}|{1}'", configuration, platformValue))
                            {
                                return true;
                            }
                            else if (platformValue == "AnyCPU")
                            {
                                var pattern = string.Format(@"'\$\(Configuration\)\|\$\(Platform\)'=='{0}\|\w+?'", configuration);

                                return Regex.IsMatch(condition, pattern);
                            }

                            return false;
                        }
                        else
                        {
                            return false;
                        }

                    });

                    if (outputPathElement != null)
                    {
                        OutputPath = outputPathElement.Value;

                        if (OutputPath.Contains("$(Configuration"))
                        {
                            OutputPath = OutputPath.Replace("$(Configuration)", configuration);
                        }
                    }

                    var outputType = rootElement.Properties.SingleOrDefault(p => p.Name == "OutputType").Value;
                    string outputExt = string.Empty;

                    switch (outputType)
                    {
                        case "Library":
                            outputExt = ".dll";
                            break;
                        case "Exe":
                            outputExt = ".exe";
                            break;
                        case "WinExe":
                        case "Bundle":
                            outputExt = ".exe";
                            break;
                        case "Package":
                            outputExt = ".msi";
                            break;
                        default:
                            Debugger.Break();
                            break;
                    }

                    if (OutputPath != null)
                    {
                        var directory = new FileInfo(this.projectFileName).DirectoryName;

                        this.OutputFile = Path.GetFullPath(Path.Combine(directory, OutputPath, outputName.Value + outputExt));
                    }
                }
            }
            else
            {
                var targetFramework = rootElement.Properties.SingleOrDefault(p => p.Name == "TargetFramework");

                this.name = Path.GetFileNameWithoutExtension(projectFileName);

                if (targetFramework != null)
                {
                    var directory = new FileInfo(this.projectFileName).DirectoryName;
                    var outputDirectory = new DirectoryInfo(Path.Combine(directory, $"bin\\Debug\\{ targetFramework.Value }"));
                    ProjectPropertyElement propertyElement;

                    // kn todo - assuming debug and dll

                    this.OutputPath = outputDirectory.FullName;
                    this.OutputFile = Path.Combine(outputDirectory.FullName, this.name + ".dll");

                    propertyElement = rootElement.CreatePropertyElement("RootNamespace");

                    propertyElement.Value = this.name;

                    addPropertyElements.Add(new VSProjectProperty(this, propertyElement));
                }

                //Debug.WriteLine("Project '{0}' has either zero or more than one assembly output section.  Assembly related properties will be indeterminant and set to null.", this.Name);
            }

            rootElementContainer = (ProjectElementContainer)rootElement;

            stream.Close();

            itemCollection = (ICollection)s_ProjectRootElement_Items.GetValue(rootElement, null);

            if (!loadOnDemand)
            {
                // will force the iteration

                GetProperties().ToList();
                GetItems().ToList();

                foreach (var properyElement in addPropertyElements)
                {
                    properties.Add(properyElement);
                }
            }
        }

        public IEnumerable<IVSProjectItem> EDMXModels
        {
            get 
            {
                return items.Where(i => i.ItemType == "EntityDeploy");
            }
        }

        public IEnumerable<IVSProjectItem> RestModels
        {
            get
            {
                return items.Where(i => i.ItemType == "None" && i.FilePath.EndsWith(".Model.json"));
            }
        }

        public IEnumerable<IVSProjectItem> Schemas
        {
            get
            {
                var schemas = items.Where(i => i.Name.EndsWith(".xsd") && !i.Include.StartsWith(@"Web References\") && !i.Include.StartsWith(@"Service References\"));

                return schemas;
            }
        }

        public void Dispose()
        {
        }

        public void Initialize(IEventSource eventSource)
        {
            eventSource.AnyEventRaised += (sender, e) =>
            {
                AnyEventRaised(sender, e);
            };

            eventSource.BuildFinished += (sender, e) =>
            {
                BuildFinished(sender, e);
            };

            eventSource.BuildStarted += (sender, e) =>
            {
                BuildStarted(sender, e);
            };

            eventSource.CustomEventRaised += (sender, e) =>
            {
                CustomEventRaised(sender, e);
            };

            eventSource.ErrorRaised += (sender, e) =>
            {
                ErrorRaised(sender, e);
            };

            eventSource.MessageRaised += (sender, e) =>
            {
                MessageRaised(sender, e);
            };

            eventSource.ProjectFinished += (sender, e) =>
            {
                ProjectFinished(sender, e);
            };

            eventSource.ProjectStarted += (sender, e) =>
            {
                ProjectStarted(sender, e);
            };

            eventSource.StatusEventRaised += (sender, e) =>
            {
                StatusEventRaised(sender, e);
            };

            eventSource.TargetFinished += (sender, e) =>
            {
                TargetFinished(sender, e);
            };

            eventSource.TargetStarted += (sender, e) =>
            {
                TargetStarted(sender, e);
            };

            eventSource.TaskFinished += (sender, e) =>
            {
                TaskFinished(sender, e);
            };

            eventSource.TaskStarted += (sender, e) =>
            {
                TaskStarted(sender, e);
            };

            eventSource.WarningRaised += (sender, e) =>
            {
                WarningRaised(sender, e);
            };
        }

        public string Parameters
        {
            get
            {
                return parameters;
            }
            set
            {
                parameters = value;
            }
        }

        public void Shutdown()
        {
        }

        public LoggerVerbosity Verbosity
        {
            get
            {
                return loggerVerbosity;
            }
            set
            {
                loggerVerbosity = value;
            }
        }

        public override int GetHashCode()
        {
            return this.Hash.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is VSProject)
            {
                return Equals((VSProject)obj);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(VSProject project)
        {
            return project.Hash == this.Hash;
        }

        public static bool operator ==(VSProject a, VSProject b)
        {
            bool equal;

            if (CompareExtensions.CheckNullEquality(a, b, out equal))
            {
                return equal;
            }

            return a.Equals(b);
        }

        public static bool operator !=(VSProject a, VSProject b)
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
