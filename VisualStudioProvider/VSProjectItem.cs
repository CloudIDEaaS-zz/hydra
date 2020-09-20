using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Xml;
using CodeInterfaces;
using System.Text;
using Metaspec;
using System.Text.RegularExpressions;
using System.Collections;
using Microsoft.Build.Construction;
using Utils;

namespace VisualStudioProvider
{
    [DebuggerDisplay("{Name}, {Include}, {ItemType}")]
    public class VSProjectItem : IVSProjectItem
    {
        internal static readonly Type s_ProjectItemElement;
        internal static readonly PropertyInfo s_ProjectItemElement_ItemType;
        internal static readonly PropertyInfo s_ProjectItemElement_Include;
        internal static readonly PropertyInfo s_ProjectItemElement_HasMetadata;
        internal static readonly PropertyInfo s_ProjectItemElement_Metadata;
        internal static readonly MethodInfo s_ProjectItemElement_AddMetadata;
        private VSProject project;
        private object internalProjectItem;
        private string itemType;
        private ICsFile iCSFile;
        private IEnumerable<ProjectMetadataElement> metadata;
        public string FilePath { get; private set; }
        public IProjectRoot ProjectRoot { get; set; }
        public bool IsHidden { get; internal set; }
        public IArchitectureLayer ArchitectureLayer { get; set; }
        public string Hash { get; private set; }
        public string ItemInfo { get; private set; }
        public Guid InstanceId { get; private set; }
        public string Include { get; private set; }
        public string RelativePath { get; private set; }
        public bool HasMetadata { get; private set; }
        public bool IsUnsaved { get; internal set; }
        public IVSProjectItem ParentItem { get; set; }
        private List<IVSProjectItem> childItems;
        private string name;
        private IVSProjectItem dependentUponItem;
        public bool IsSubItem { get; set; }

        static VSProjectItem()
        {
            s_ProjectItemElement = Type.GetType("Microsoft.Build.Construction.ProjectItemElement, Microsoft.Build, Version=15.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);

            s_ProjectItemElement_ItemType = s_ProjectItemElement.GetProperty("ItemType", BindingFlags.Public | BindingFlags.Instance);
            s_ProjectItemElement_Include = s_ProjectItemElement.GetProperty("Include", BindingFlags.Public | BindingFlags.Instance);
            s_ProjectItemElement_HasMetadata = s_ProjectItemElement.GetProperty("HasMetadata", BindingFlags.Public | BindingFlags.Instance);
            s_ProjectItemElement_Metadata = s_ProjectItemElement.GetProperty("Metadata", BindingFlags.Public | BindingFlags.Instance);
            s_ProjectItemElement_AddMetadata = s_ProjectItemElement.GetMethod("AddMetadata", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string), typeof(string) }, null);
        }

        string ICodeFile.FileName
        {
            get 
            {
                return this.FilePath;
            }
        }

        public int HierarchySort
        {
            get
            {
                switch (itemType)
                {
                    case "ProjectConfiguration":
                        return 1;
                    case "References":
                        return 2;
                    case "Folder":

                        if (this.Name == "Properties" && this.ParentItem == null)
                        {
                            return 1;
                        }
                        else
                        {
                            return 3;
                        }

                    case "Compile":
                    case "Content":
                    case "File":
                        return 4;
                    case "Reference":
                    case "ProjectReference":
                        return 5;
                    default:
                        return 4;
                }
            }
        }

        public IVSProject ParentProject
        {
            get
            {
                return project;
            }
        }

        public string ProjectPath
        {
            get
            {
                if (this.RelativePath != null && (this.RelativePath.StartsWith(@"..") || this.RelativePath.StartsWithAny(DriveInfo.GetDrives().Select(d => d.Name + ":").ToArray())))
                {
                    var metaData = this.Metadata.SingleOrDefault(m => m.Name == "Link");

                    if (metaData != null)
                    {
                        return metaData.Value;
                    }
                    else
                    {
                        return this.RelativePath;
                    }
                }
                else
                {
                    return this.RelativePath;
                }
            }
        }

        public bool IsLink
        {
            get 
            {
                var metaData = this.Metadata.SingleOrDefault(m => m.Name == "Link");

                if (metaData != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public object InternalProjectItem
        {
            get 
            { 
                return internalProjectItem; 
            }
        }

        public ICsFile ICsFile
        {
            get
            {
                if (this.ItemType == "Compile" || this.ItemType == "EntityDeploy")
                {
                    if (project.IProject != null)
                    {
                        iCSFile = ((ICsProject)project.IProject).getFiles().FirstOrDefault(f => f.getPath() == FilePath);
                    }
                }

                return iCSFile;
            }
        }

        public bool IsFile
        {
            get
            {
                var list = new List<string> 
                {
                    "ApplicationDefinition",
                    "PropertiesFile",
                    "Compile",
                    "ProjectReference",
                    "EmbeddedResource",
                    "Content",
                    "Page",
                    "Resource"
                };

                return list.Contains(itemType);
            }
        }

        public string ItemType
        { 
            get
            {
                return itemType;
            }
        }

        public IEnumerable<IVSProjectMetadataElement> Metadata
        {
            get
            {
                foreach (var element in metadata)
                {
                    yield return new VSProjectMetadataElement(element);
                }
            }
        }

        public IEnumerable<IVSProjectItem> ChildItems
        {
            get
            {
                return childItems;
            }
        }

        public VSProjectItem(VSProject project, object internalProjectItem, VSProjectItem parentItem = null, bool isHidden = false)
        {
            FileInfo file;

            if (internalProjectItem is string)
            {
                var path = (string) internalProjectItem;

                if (path == "References")
                {
                    RelativePath = path;
                    this.itemType = "References";
                }
                else
                {
                    RelativePath = path;
                    FilePath = Path.Combine(Path.GetDirectoryName(project.FileName), path);

                    if (File.Exists(FilePath))
                    {
                        this.itemType = "File";
                    }
                    else
                    {
                        this.itemType = "Folder";
                    }
                }

                this.project = project;
                this.HasMetadata = false;
                this.Include = null;
                this.Hash = this.RelativePath.GetHash();
            }
            else
            {
                this.itemType = s_ProjectItemElement_ItemType.GetValue(internalProjectItem, null) as string;
                this.Include = s_ProjectItemElement_Include.GetValue(internalProjectItem, null) as string;
                this.HasMetadata = (bool)s_ProjectItemElement_HasMetadata.GetValue(internalProjectItem, null);
                this.project = project;
                this.internalProjectItem = internalProjectItem;

                file = new FileInfo(project.FileName);

                metadata = s_ProjectItemElement_Metadata.GetValue(internalProjectItem, null) as ICollection<ProjectMetadataElement>;

                if (this.itemType != "ProjectConfiguration")
                {
                    if (this.itemType == "WebReferenceUrl" || this.itemType == "Reference")
                    {
                        var hintPath = metadata.SingleOrDefault(m => m.Name == "HintPath");

                        if (hintPath != null)
                        {
                            RelativePath = hintPath.Value;
                        }

                        FilePath = this.Include.RemoveSurroundingSlashes();
                    }
                    else
                    {
                        if (this.itemType != "SchemaFiles" && this.itemType != "WebReferenceUrl")
                        {
                            try
                            {
                                var include = this.Include.RemoveSurroundingSlashes();
                                var pattern = string.Format(@"(?<variable>\$\((?<value>{0})\))", StringExtensions.REGEX_IDENTIFIER_MIDSTRING);
                                var hasErrors = true;
                                var errorCount = 0;

                                while (hasErrors)
                                {
                                    hasErrors = false;

                                    if (include.RegexIsMatch(pattern))
                                    {
                                        var regex = new Regex(pattern);
                                        var match = regex.Match(include);
                                        var variable = match.Groups["variable"].Value;
                                        var value = match.Groups["value"].Value;

                                        if (this.ParentProject.Properties.Any(p => p.Name == value))
                                        {
                                            var property = this.ParentProject.Properties.Single(p => p.Name == value);
                                            var propertyValue = property.Value;

                                            propertyValue = propertyValue.Replace("$(MSBuildThisFileDirectory)", Path.GetDirectoryName(this.ParentProject.FileName) + @"\");

                                            include = include.RegexReplace(pattern, propertyValue);
                                        }
                                        else
                                        {
                                            include = include.Replace("$(MSBuildThisFileDirectory)", Path.GetDirectoryName(this.ParentProject.FileName) + @"\");
                                        }
                                    }

                                    errorCount++;

                                    if (errorCount > 10)
                                    {
                                        DebugUtils.Break();
                                    }
                                }

                                RelativePath = include;
                                FilePath = Path.GetFullPath(Path.Combine(file.DirectoryName, RelativePath));
                            }
                            catch (Exception ex)
                            {
                                FilePath = ex.Message;
                            }
                        }
                    }
                }
            }

            this.IsHidden = isHidden;
            this.InstanceId = Guid.NewGuid();

            try
            {
                if (this.itemType != "SchemaFiles" && this.itemType != "WebReferenceUrl")
                {
                    UpdateInfo();
                }
            }
            catch
            {
            }

            childItems = new List<IVSProjectItem>();

            if (parentItem != null)
            {
                this.ParentItem = parentItem;
                parentItem.AddChild(this);
            }
        }

        private void UpdateInfo()
        {
            var builder = new StringBuilder();

            using (builder.AppendTag("Item", new { Type = itemType }))
            {
                if (this.FilePath != null)
                {
                    var file = new FileInfo(this.FilePath);

                    if (file.Exists)
                    {
                        string fileHash;

                        try
                        {
                            fileHash = file.GetHash();
                        }
                        catch (Exception ex)
                        {
                            // if it's open, likely gonna change

                            fileHash = string.Format("{0}, Error: {1}", file.FullName, ex.Message).GetHash();  
                        }

                        using (builder.AppendTag("FileHash", fileHash))
                        {
                        }
                    }

                    using (builder.AppendTag("Value", this.FilePath))
                    {
                    }
                }
                else
                {
                    using (builder.AppendTag("Value", this.RelativePath))
                    {
                    }
                }

                using (builder.AppendTag("Hidden", this.IsHidden ? "True" : "False"))
                {
                }

                if (this.HasMetadata)
                {
                    using (builder.AppendTag("Metadata"))
                    {
                        foreach (var metaData in this.Metadata)
                        {
                            using (builder.AppendTag("MetadataItem"))
                            {
                                using (builder.AppendTag("Name", metaData.Name))
                                {
                                }

                                using (builder.AppendTag("Value", metaData.Value))
                                {
                                }
                            }
                        }
                    }
                }
            }

            this.ItemInfo = builder.ToString();
            this.Hash = this.ItemInfo.GetHash();
        }

        internal void AddChild(VSProjectItem item)
        {
            this.childItems.Add(item);
        }

        public byte[] FileContents
        {
            get 
            {
                return File.ReadAllBytes(FilePath);
            }
        }

        public Stream FileStream
        {
            get
            {
                return new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            }
        }

        public string Name
        {
            get 
            {
                if (name == null)
                {
                    if (this.itemType.IsOneOf("File", "Folder", "References"))
                    {
                        name = this.RelativePath.Split('\\', StringSplitOptions.RemoveEmptyEntries).Last();
                    }
                    else if (FilePath != null)
                    {
                        if (Uri.IsWellFormedUriString(FilePath, UriKind.RelativeOrAbsolute))
                        {
                            FilePath = this.Include;

                            name = FilePath;
                        }
                        else
                        {
                            FileInfo file = null;

                            try
                            {
                                file = new FileInfo(FilePath);
                            }
                            catch (PathTooLongException ex)
                            {
                                name = this.Include;
                            }

                            if (file != null && file.Exists)
                            {
                                name = file.Name;
                            }
                            else
                            {
                                var directory = new DirectoryInfo(FilePath);

                                if (directory.Exists)
                                {
                                    name = directory.Name;
                                }
                                else
                                {
                                    name = this.Include;
                                }
                            }
                        }
                    }
                    else
                    {
                        name = this.Include;
                    }
                }

                return name;
            }
        }

        public T GetFileContents<T>()
        {
            switch (typeof(T).Name)
            {
                case "string":
                case "String":
                    {
                        var fileContents = this.FileContents;
                        var bom = CodePoint.GetBOM(fileContents);
                        var contents = ASCIIEncoding.ASCII.GetString(fileContents, bom.Length, fileContents.Length - bom.Length);

                        return (T)(object)contents;
                    }
                case "char[]":
                case "Char[]":
                    {
                        var fileContents = this.FileContents;
                        var bom = CodePoint.GetBOM(fileContents);
                        var contents = ASCIIEncoding.ASCII.GetString(fileContents, bom.Length, fileContents.Length - bom.Length).ToCharArray();

                        return (T)(object)contents;
                    }

                default:
                    throw new Exception("Unsupported content type");
            }
        }

        public IVSProjectItem DependentUpon
        {
            get
            {
                return dependentUponItem;
            }

            internal set
            {
                dependentUponItem = value;
            }
        }

        internal IVSProjectMetadataElement DependentUponMetadata
        {
            get
            {
                if (this.HasMetadata)
                {
                    if (this.Metadata.Any(m => m.Name == "DependentUpon"))
                    {
                        var metaData = this.Metadata.Single(m => m.Name == "DependentUpon");
                        return (IVSProjectMetadataElement)metaData;
                    }
                }

                return null;
            }
        }

        public bool HasDependentUpon
        {
            get
            {
                if (this.HasMetadata)
                {
                    return this.Metadata.Any(m => m.Name == "DependentUpon");
                }
                else
                {
                    return false;
                }
            }
        }

        public override int GetHashCode()
        {
            return this.Hash.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is VSProjectItem)
            {
                return Equals((VSProjectItem)obj);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(VSProjectItem project)
        {
            return project.Hash == this.Hash;
        }

        public static bool operator ==(VSProjectItem a, VSProjectItem b)
        {
            bool equal;

            if (CompareExtensions.CheckNullEquality(a, b, out equal))
            {
                return equal;
            }

            return a.Equals(b);
        }

        public static bool operator !=(VSProjectItem a, VSProjectItem b)
        {
            bool equal;

            if (CompareExtensions.CheckNullEquality(a, b, out equal))
            {
                return !equal;
            }

            return !a.Equals(b);
        }

        public void AddFromFile(string fileName)
        {
            this.ParentProject.AddItem(fileName, "Content", this);
        }

        internal void Remove(IVSProjectItem item)
        {
            childItems.Remove(item);
        }
    }
}
