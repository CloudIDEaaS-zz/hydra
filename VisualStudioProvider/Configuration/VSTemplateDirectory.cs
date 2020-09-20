using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Microsoft.VisualStudio.Shell;
using Utils;
using System.Diagnostics;
using System.IO;
using CodeInterfaces;
using System.Threading;

namespace VisualStudioProvider.Configuration
{
    public delegate void LoadParentDirectoryHandler(Guid parentGuid, out VSTemplateDirectory parent);

    [DebuggerDisplay("{ DebugInfo }")]
    public class VSTemplateDirectory
    {
        private VSTemplateDirectory parentDirectory;
        public static event LoadParentDirectoryHandler LoadParentDirectory;
        public string DeveloperActivity { get; private set; }
        public int SortPriority { get; private set; }
        public string TemplatesDir { get; private set; }
        public List<DirectoryInfo> AllTemplateDirs { get; private set; }
        public List<ICodeTemplate> AllTemplates { get; private set; }
        public List<ICodeTemplate> Templates { get; private set; }
        public string Name { get; private set; }
        public VSPackage Package { get; protected set; }
        public Guid Guid { get; protected set; }
        public List<VSTemplateDirectory> SubDirectories { get; protected set; }

        public VSTemplateDirectory()
        {
            this.SubDirectories = new List<VSTemplateDirectory>();
            this.AllTemplates = new List<ICodeTemplate>();
            this.Templates = new List<ICodeTemplate>();
            this.SortPriority = 3000;
        }

        public VSTemplateDirectory(string name) : this()
        {
            this.Name = name;
        }

        public VSTemplateDirectory(RegistryKey key, VSPackage package, bool isPseudoFolder = false) : this()
        {
            var directoryKey = key.ToIndexable();

            this.Package = package;
            this.SubDirectories = new List<VSTemplateDirectory>();
            this.Guid = Guid.Parse(directoryKey.SubName);

            if (isPseudoFolder)
            {
                var displayName = (string)directoryKey["DisplayName"];
                var name = string.Empty;

                if (displayName.StartsWith("#"))
                {
                    name = ResourceLoader.LoadStringFrom(Package.SatelliteDll.FullName, displayName);
                }
                else
                {
                    name = displayName;
                }

                this.Name = name;
            }
            else
            {
                var keys = key.Enumerate();

                if (keys.Count() == 1)
                {
                    var subKey = key.Enumerate().First();

                    ProcessSubKey(subKey, false);
                }
                else
                {
                    foreach (var subKey in keys)
                    {
                        ProcessSubKey(subKey, true);
                    }
                }
            }
        }

        public string ProjectType
        {
            get
            {
                if (ProjectTypeMappings.Mappings.ContainsKey(this.Guid))
                {
                    return ProjectTypeMappings.Mappings[this.Guid];
                }
                else
                {
                    return null;
                }
            }
        }

        public Guid PackageGuid
        {
            get
            {
                if (this.Package != null)
                {
                    return this.Package.PackageGuid;
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }

        public string DebugInfo
        {
            get
            {
                return string.Format("{{{0}}} {1}, path: {2}, children: {3}, templates: {4}", this.Guid.ToString(), this.Name, this.Path, this.SubDirectories.Count, this.AllTemplates.Count);
            }
        }

        public string Path
        {
            get
            {
                var path = string.Empty;
                var parentDirectory = this.ParentDirectory;

                while (parentDirectory != null)
                {
                    path = (parentDirectory.Name ?? "{Null}") + @"\" + path;

                    parentDirectory = parentDirectory.ParentDirectory;
                }

                path += (this.Name ?? "{Null}");

                return path;
            }
        }

        public VSTemplateDirectory ParentDirectory
        {
            get
            {
                return parentDirectory;
            }

            private set
            {
                parentDirectory = value;

                if (parentDirectory != null)
                {
                    parentDirectory.SubDirectories.Add(this);
                }
            }
        }

        protected void ProcessSubKey(RegistryKeyIndexable subKey, bool isSubDirectory)
        {
            var nameKey = (string)subKey.Default;
            var name = string.Empty;
            var folder = (string) subKey["Folder"];
            var developerActivity = (string)subKey["DeveloperActivity"];
            var templatesDir = (string)subKey["TemplatesDir"];
            var sortPriority = (int) ((int?) subKey["SortPriority"] ?? 3000);
            var parentDirectory = (VSTemplateDirectory) null;

            if (Package.SatelliteDll != null && nameKey.StartsWith("#"))
            {
                name = ResourceLoader.LoadStringFrom(Package.SatelliteDll.FullName, nameKey);
            }
            else
            {
                name = nameKey;
            }

            if (folder != null)
            {
                var guid = Guid.Parse(folder);

                LoadParentDirectory(guid, out parentDirectory);
            }

            var directory = (VSTemplateDirectory)null;

            if (isSubDirectory)
            {
                var subDirectory = new VSTemplateDirectory
                {
                    ParentDirectory = parentDirectory,
                    Name = name,
                    DeveloperActivity = developerActivity,
                    TemplatesDir = templatesDir,
                    SortPriority = sortPriority
                };

                directory = subDirectory;

                SubDirectories.Add(subDirectory);
            }
            else
            {
                this.Name = name;
                this.ParentDirectory = parentDirectory;
                this.DeveloperActivity = developerActivity;
                this.TemplatesDir = templatesDir;
                this.SortPriority = sortPriority;

                directory = this;
            }

            directory.AllTemplateDirs = new List<DirectoryInfo>();

            templatesDir = templatesDir.Expand();

            if (templatesDir.Contains(@"\.\") || templatesDir.Contains(@"\..\"))
            {
                var index = templatesDir.IndexOf(@"\.\");
                var subIndex = index + @"\.\".Length;

                if (index == -1)
                {
                    index = templatesDir.IndexOf(@"\..\");
                    subIndex = index + @"\..\".Length;
                }

                var templatesRoot = templatesDir.Substring(0, index);
                var subDirFind = templatesDir.Substring(subIndex);
                Action<DirectoryInfo> recurseFind = null;

                recurseFind = (d) =>
                {
                    if (d.FullName.EndsWith(subDirFind))
                    {
                        directory.AllTemplateDirs.Add(d);
                    }

                    d.GetDirectories().ToList().ForEach(d2 => recurseFind(d2));
                };

                if (templatesRoot.Length > 0)
                {
                    var root = new DirectoryInfo(templatesRoot);

                    if (root.Exists)
                    {
                        recurseFind(root);
                    }
                }
            }
            else
            {
                if (templatesDir == @"\\")
                {

                }
                else
                {
                    directory.AllTemplateDirs.Add(new DirectoryInfo(templatesDir));
                }
            }
        }

        public void BuildOut()
        {
            this.AllTemplates.ForEach(t => 
            {
                var subDirectoryPath = string.Empty;
                var languageID = Thread.CurrentThread.CurrentCulture.LCID.ToString();

                if (t.TemplateLocation.StartsWith(VSConfig.ProjectTemplateDirectory))
                {
                    subDirectoryPath = t.TemplateLocation.RemoveStart(VSConfig.ProjectTemplateDirectory.Length);
                }
                else if (t.TemplateLocation.StartsWith(VSConfig.ItemTemplateDirectory))
                {
                    subDirectoryPath = t.TemplateLocation.RemoveStart(VSConfig.ItemTemplateDirectory.Length);
                }
                else
                {
                    return;
                }

                if (subDirectoryPath.EndsWith(languageID))
                {
                    subDirectoryPath = subDirectoryPath.RemoveEnd(languageID.Length);
                }

                if (this.ProjectType != null && subDirectoryPath.StartsWith(this.ProjectType))
                {
                    subDirectoryPath = subDirectoryPath.RemoveStart(this.ProjectType.Length);
                }

                var subDir = FindSubDirectory(subDirectoryPath);

                if (subDir == null)
                {
                    subDir = CreateSubDirectory(subDirectoryPath);

                    if (subDir == null)
                    {
                        Debugger.Break();
                    }
                }

                subDir.Templates.Add(t);
            });
        }

        public VSTemplateDirectory FindSubDirectory(string subPath)
        {
            Action<VSTemplateDirectory> recurse = null;
            var foundItems = new List<VSTemplateDirectory>();

            recurse = (dir) =>
            {
                if (dir.Path.RemoveStart(this.Path).RemoveSurroundingSlashes() == subPath.RemoveSurroundingSlashes())
                {
                    foundItems.Add(dir);
                }

                dir.SubDirectories.ForEach(d => recurse(d));
            };

            this.SubDirectories.ForEach(d => recurse(d));

            if (foundItems.Count > 0)
            {
                return foundItems.Single();
            }
            else
            {
                return null;
            }
        }

        public VSTemplateDirectory CreateSubDirectory(string subPath)
        {
            var subPathStripped = subPath.RemoveSurroundingSlashes();
            var parts = subPathStripped.Split('\\').Where(s => s.Length > 0);
            var path = parts.First();
            var subDir = FindSubDirectory(path);

            if (subDir != null)
            {
                subDir.CreateSubDirectory(subPathStripped.RemoveStart(path).RemoveSurroundingSlashes());
            }
            else
            {
                var subPathSub = string.Empty;

                subPathSub = subPathStripped.RemoveStart(path).RemoveSurroundingSlashes();
                subDir = new VSTemplateDirectory(path);

                if (subPathSub.Split('\\').Where(s => s.Length > 0).Count() > 0)
                {
                    subDir.CreateSubDirectory(subPathSub.RemoveSurroundingSlashes());
                }

                subDir.ParentDirectory = this;
            }

            return FindSubDirectory(subPath);
        }
    }
}
