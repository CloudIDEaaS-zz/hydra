using AbstraX.FolderStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;
using System.Linq;
using BindingFlags = System.Reflection.BindingFlags;
using BTreeIndex.Collections.Generic.BTree;
using Utils.Hierarchies;
using static AbstraX.FolderStructure.FileSystemObject;
using System.Reflection;

namespace AbstraX
{
    public abstract class ApplicationFolderHierarchy
    {
        public string Root { get; private set; }
        public string ProjectRoot { get; private set; }
        public string ServicesRoot { get; private set; }
        public Folder ProjectRootFolder { get; private set; }
        public Folder RootFolder { get; private set; }
        public Folder ServicesRootFolder { get; private set; }

        public abstract string this[string name] { get; }
        public abstract string this[Enum type] { get; }
        internal BTreeDictionary<string, FileSystemObject> Index { get; }
        public FileSystem FileSystem { get; }

        public ApplicationFolderHierarchy(string folderRoot, string projectFolderRoot, string servicesFolderRoot)
        {
            this.Root = folderRoot.ForwardSlashes();
            this.ProjectRoot = projectFolderRoot.ForwardSlashes();
            this.ServicesRoot = servicesFolderRoot.ForwardSlashes();
            this.RootFolder = new Folder(this, this.Root);

            if (projectFolderRoot == folderRoot)
            {
                this.ProjectRootFolder = this.RootFolder;
            }
            else
            {
                this.ProjectRootFolder = new Folder(this, this.ProjectRoot);
            }

            this.ServicesRootFolder = new Folder(this, this.ServicesRoot);

            this.FileSystem = new FileSystem(this);
            this.Index = new BTreeDictionary<string, FileSystemObject>();

            IndexAdd(this.RootFolder.RelativeFullName, this.RootFolder);

            if (projectFolderRoot.StartsWith(folderRoot) && projectFolderRoot != folderRoot)
            {
                this.RootFolder.Folders.Add(this.ProjectRootFolder);
            }

            if (servicesFolderRoot.StartsWith(folderRoot) && projectFolderRoot != folderRoot)
            {
                this.RootFolder.Folders.Add(this.ServicesRootFolder);
            }
        }

        internal void IndexAdd(string rootedFullName, FileSystemObject fileSystemObject)
        {
            DebugUtils.ThrowIf(this.Index.ContainsKey(rootedFullName), () => new IOException(string.Format("Path '{0}' already exists in virtual file system.", rootedFullName)));

            this.Index.Add(rootedFullName, fileSystemObject);
        }

        internal void IndexRemove(string rootedFullName)
        {
            this.Index.Remove(rootedFullName);
        }

        internal List<Folder> GetAllFolders()
        {
            var folders = new List<Folder>();

            this.ProjectRootFolder.GetDescendantsAndSelf(f => f.Folders, (f) =>
            {
                folders.Add(f);
            });

            return folders;
        }

        internal List<AbstraX.FolderStructure.File> GetAllFiles()
        {
            var files = new List<AbstraX.FolderStructure.File>();

            this.ProjectRootFolder.GetDescendantsAndSelf(f => f.Folders, (f) =>
            {
                files.AddRange(f.Files);
            });

            return files;
        }

        internal List<FileSystemObject> GetAllFileSystemObjects()
        {
            var fileSystemObjects = new List<FileSystemObject>();

            this.ProjectRootFolder.GetDescendantsAndSelf(f => f.Folders, (f) =>
            {
                fileSystemObjects.Add(f);
                fileSystemObjects.AddRange(f.Files);
            });

            return fileSystemObjects;
        }

        private string CreatePath(string pathCreate, string root, bool returnFull = false)
        {
            if (FileSystemObject.IsPathSystemRooted(pathCreate))
            {
                var path = FileSystemObject.BluntStart(pathCreate.ForwardSlashes(), root);

                return this.CreatePath(path, root, returnFull);
            }
            else
            {
                string path;
                string[] parts;
                Folder parentFolder = null;

                if (returnFull)
                {
                    path = PathCombine(root, pathCreate, true);

                    if (path.StartsWith(this.Root))
                    {
                        parts = FileSystemObject.BluntStart(path, this.Root).SplitCombine('/');
                    }
                    else if (path.StartsWith(this.ServicesRoot))
                    {
                        parts = FileSystemObject.BluntStart(path, this.ServicesRoot).SplitCombine('/');
                    }
                    else if (path.StartsWith(this.ProjectRoot))
                    {
                        parts = FileSystemObject.BluntStart(path, this.ProjectRoot).SplitCombine('/');
                    }
                    else
                    {
                        e.Throw<InvalidOperationException>("Unrecognized path prefix");
                        parts = null;
                    }
                }
                else
                {
                    path = PathCombine(root, pathCreate, true);

                    if (path.StartsWith(this.Root))
                    {
                        path = FileSystemObject.BluntStart(path, this.Root);
                        parts = path.SplitCombine('/');
                    }
                    else if (path.StartsWith(this.ServicesRoot))
                    {
                        path = FileSystemObject.BluntStart(path, this.ServicesRoot);
                        parts = path.SplitCombine('/');
                    }
                    else if (path.StartsWith(this.ProjectRoot))
                    {
                        path = FileSystemObject.BluntStart(path, this.ProjectRoot);
                        parts = path.SplitCombine('/');
                    }
                    else
                    {
                        e.Throw<InvalidOperationException>("Unrecognized path prefix");
                        parts = null;
                    }
                }

                foreach (var part in parts)
                {
                    if (this.Index.ContainsKey(part))
                    {
                        parentFolder = (Folder)this.Index[part];
                    }
                    else
                    {
                        var newFolder = new Folder(this, PathCombine(this.Root, part, true));

                        parentFolder.Folders.Add(newFolder);
                        parentFolder = newFolder;
                    }
                }

                var debug = false;

                if (debug)
                {
                    var folders = this.GetAllFolders();
                }

                return path;
            }
        }

        protected internal string CreateProjectPath(string pathCreate, bool returnFull = false)
        {
            return CreatePath(pathCreate, this.ProjectRoot, returnFull);
        }

        protected internal string CreateServicesPath(string pathCreate, bool returnFull = false)
        {
            return CreatePath(pathCreate, this.ServicesRoot, returnFull);
        }

        protected string GetPropertyValue(Enum type)
        {
            var property = this.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public).Where(p => p.HasCustomAttribute<FileSystemTypeAttribute>()).Single(p => p.GetCustomAttribute<FileSystemTypeAttribute>().Type.Equals(type));

            return (string)property.GetValue(this);
        }
    }
}
