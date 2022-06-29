// file:	ApplicationFolderHierarchy.cs
//
// summary:	Implements the application folder hierarchy class

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
    /// <summary>   An application folder hierarchy. </summary>
    ///
    /// <remarks>   Ken, 10/11/2020. </remarks>

    public abstract class ApplicationFolderHierarchy : IFolderHierarchy
    {
        /// <summary>   Gets or sets the root. </summary>
        ///
        /// <value> The root. </value>

        public string Root { get; private set; }

        /// <summary>   Gets or sets the project root. </summary>
        ///
        /// <value> The project root. </value>

        public string ProjectRoot { get; private set; }

        /// <summary>   Gets or sets the services root. </summary>
        ///
        /// <value> The services root. </value>

        public string ServicesRoot { get; private set; }

        /// <summary>   Gets or sets the entities root. </summary>
        ///
        /// <value> The entities root. </value>

        public string EntitiesRoot { get; private set; }

        /// <summary>   Gets or sets the pathname of the project root folder. </summary>
        ///
        /// <value> The pathname of the project root folder. </value>

        public Folder ProjectRootFolder { get; private set; }

        /// <summary>   Gets or sets the pathname of the root folder. </summary>
        ///
        /// <value> The pathname of the root folder. </value>

        public Folder RootFolder { get; private set; }

        /// <summary>   Gets or sets the pathname of the services root folder. </summary>
        ///
        /// <value> The pathname of the services root folder. </value>

        public Folder ServicesRootFolder { get; private set; }

        /// <summary>   Gets or sets the pathname of the entities root folder. </summary>
        ///
        /// <value> The pathname of the entities root folder. </value>

        public Folder EntitiesRootFolder { get; private set; }

        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <param name="name"> The name. </param>
        ///
        /// <returns>   The indexed item. </returns>

        public abstract string this[string name] { get; }

        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <param name="type"> An enum constant representing the type option. </param>
        ///
        /// <returns>   The indexed item. </returns>

        public abstract string this[Enum type] { get; }

        /// <summary>   Gets the zero-based index of this.  </summary>
        ///
        /// <value> The index. </value>

        public BTreeDictionary<string, FileSystemObject> Index { get; }

        /// <summary>   Gets the file system. </summary>
        ///
        /// <value> The file system. </value>

        public FileSystem FileSystem { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="folderRoot">           The folder root. </param>
        /// <param name="projectFolderRoot">    The project folder root. </param>
        /// <param name="servicesFolderRoot">   The services folder root. </param>
        /// <param name="entitiesFolderRoot">   The entities folder root. </param>

        public ApplicationFolderHierarchy(string folderRoot, string projectFolderRoot, string servicesFolderRoot, string entitiesFolderRoot)
        {
            this.Root = folderRoot.ForwardSlashes();
            this.ProjectRoot = projectFolderRoot.ForwardSlashes();
            this.ServicesRoot = servicesFolderRoot.ForwardSlashes();
            this.EntitiesRoot = entitiesFolderRoot.ForwardSlashes();
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
            this.EntitiesRootFolder = new Folder(this, this.EntitiesRoot);

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

        /// <summary>   Index add. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="rootedFullName">   Name of the rooted full. </param>
        /// <param name="fileSystemObject"> The file system object. </param>

        public void IndexAdd(string rootedFullName, FileSystemObject fileSystemObject)
        {
            DebugUtils.ThrowIf(this.Index.ContainsKey(rootedFullName), () => new IOException(string.Format("Path '{0}' already exists in virtual file system.", rootedFullName)));

            this.Index.Add(rootedFullName, fileSystemObject);
        }

        /// <summary>   Index remove. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="rootedFullName">   Name of the rooted full. </param>

        public void IndexRemove(string rootedFullName)
        {
            this.Index.Remove(rootedFullName);
        }

        /// <summary>   Gets all folders. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <returns>   all folders. </returns>

        public List<Folder> GetAllFolders()
        {
            var folders = new List<Folder>();

            this.ProjectRootFolder.GetDescendantsAndSelf(f => f.Folders, (f) =>
            {
                folders.Add(f);
            });

            return folders;
        }

        /// <summary>   Gets all files. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <returns>   all files. </returns>

        public List<AbstraX.FolderStructure.File> GetAllFiles()
        {
            var files = new List<AbstraX.FolderStructure.File>();

            this.ProjectRootFolder.GetDescendantsAndSelf(f => f.Folders, (f) =>
            {
                files.AddRange(f.Files);
            });

            return files;
        }

        /// <summary>   Gets all file system objects. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <returns>   all file system objects. </returns>

        public List<FileSystemObject> GetAllFileSystemObjects()
        {
            var fileSystemObjects = new List<FileSystemObject>();

            this.ProjectRootFolder.GetDescendantsAndSelf(f => f.Folders, (f) =>
            {
                fileSystemObjects.Add(f);
                fileSystemObjects.AddRange(f.Files);
            });

            return fileSystemObjects;
        }

        /// <summary>   Creates a path. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="pathCreate">   The path create. </param>
        /// <param name="root">         The root. </param>
        /// <param name="returnFull">   (Optional) True to return full. </param>
        ///
        /// <returns>   The new path. </returns>

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
                    else if (path.StartsWith(this.EntitiesRoot))
                    {
                        parts = FileSystemObject.BluntStart(path, this.EntitiesRoot).SplitCombine('/');
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
                    else if (path.StartsWith(this.EntitiesRoot))
                    {
                        parts = FileSystemObject.BluntStart(path, this.EntitiesRoot).SplitCombine('/');
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

        /// <summary>   Creates project path. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="pathCreate">   The path create. </param>
        /// <param name="returnFull">   (Optional) True to return full. </param>
        ///
        /// <returns>   The new project path. </returns>

        public string CreateProjectPath(string pathCreate, bool returnFull = false)
        {
            return CreatePath(pathCreate, this.ProjectRoot, returnFull);
        }

        /// <summary>   Creates services path. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="pathCreate">   The path create. </param>
        /// <param name="returnFull">   (Optional) True to return full. </param>
        ///
        /// <returns>   The new services path. </returns>

        protected internal string CreateServicesPath(string pathCreate, bool returnFull = false)
        {
            return CreatePath(pathCreate, this.ServicesRoot, returnFull);
        }

        /// <summary>   Creat entities path. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="pathCreate">   The path create. </param>
        /// <param name="returnFull">   (Optional) True to return full. </param>
        ///
        /// <returns>   A string. </returns>

        protected internal string CreateEntitiesPath(string pathCreate, bool returnFull = false)
        {
            return CreatePath(pathCreate, this.EntitiesRoot, returnFull);
        }

        /// <summary>   Gets property value. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="type"> An enum constant representing the type option. </param>
        ///
        /// <returns>   The property value. </returns>

        protected string GetPropertyValue(Enum type)
        {
            var property = this.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public).Where(p => p.HasCustomAttribute<FileSystemTypeAttribute>()).Single(p => p.GetCustomAttribute<FileSystemTypeAttribute>().Type.Equals(type));

            return (string)property.GetValue(this);
        }

        /// <summary>   Blunt start. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/3/2022. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   A string. </returns>

        public string BluntStart(string path)
        {
            if (path.StartsWith(this.ProjectRoot))
            {
                path = FileSystemObject.BluntStart(path, this.Root);
            }
            else if (path.StartsWith(this.ServicesRoot))
            {
                path = FileSystemObject.BluntStart(path, this.Root);

                if (FileSystemObject.IsPathSystemRooted(path))
                {
                    path = FileSystemObject.BluntStart(path, this.ServicesRoot);
                }
            }
            else if (path.StartsWith(this.EntitiesRoot))
            {
                path = FileSystemObject.BluntStart(path, this.Root);

                if (FileSystemObject.IsPathSystemRooted(path))
                {
                    path = FileSystemObject.BluntStart(path, this.EntitiesRoot);
                }
            }

            return path;
        }
    }
}
