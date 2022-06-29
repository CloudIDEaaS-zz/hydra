// file:	FolderStructure\Folder.cs
//
// summary:	Implements the folder class

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Linq;
using Utils;
using Newtonsoft.Json;

namespace AbstraX.FolderStructure
{
    /// <summary>   A folder. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/21/2020. </remarks>

    public class Folder : FileSystemObject
    {
        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>

        public override string Name { get; }

        /// <summary>   Gets a value indicating whether the exists. </summary>
        ///
        /// <value> True if exists, false if not. </value>

        public override bool Exists { get; }

        /// <summary>   Gets or sets the parent. </summary>
        ///
        /// <value> The parent. </value>

        [ScriptIgnore]
        public Folder Parent { get; set; }

        /// <summary>   Gets the files. </summary>
        ///
        /// <value> The files. </value>

        [ScriptIgnore]
        public FileSystemObjects<File> Files { get; }

        /// <summary>   Gets the folders. </summary>
        ///
        /// <value> The folders. </value>

        [ScriptIgnore]
        public FileSystemObjects<Folder> Folders { get; }

        /// <summary>   Gets the kind. </summary>
        ///
        /// <value> The kind. </value>

        public override FileSystemObjectKind Kind => FileSystemObjectKind.Folder;
        /// <summary>   The module assemblies. </summary>
        private List<IModuleAssembly> moduleAssemblies;
        /// <summary>   The modules. </summary>
        private List<Module> modules;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/21/2020. </remarks>
        ///
        /// <param name="folderHierarchy">  The folder hierarchy. </param>
        /// <param name="path">             Full pathname of the file. </param>

        public Folder(IFolderHierarchy folderHierarchy, string path) : base(folderHierarchy)
        {
            this.Folders = new FileSystemObjects<Folder>(this, folderHierarchy);
            this.Files = new FileSystemObjects<File>(this, folderHierarchy);
            this.moduleAssemblies = new List<IModuleAssembly>();
            this.modules = new List<Module>();
            this.Name = Path.GetFileName(path);

            fullPath = path;
        }

        /// <summary>   Gets the file system objects. </summary>
        ///
        /// <value> The file system objects. </value>

        public IEnumerable<FileSystemObject> FileSystemObjects
        {
            get
            {
                return this.Folders.Cast<FileSystemObject>().Concat(this.Files);
            }
        }

        /// <summary>   Gets the number of files. </summary>
        ///
        /// <value> The number of files. </value>

        public int FileCount
        {
            get
            {
                return this.Files.Count;
            }
        }

        /// <summary>   Gets the number of folders. </summary>
        ///
        /// <value> The number of folders. </value>

        public int FolderCount
        {
            get
            {
                return this.Folders.Count;
            }
        }

        /// <summary>   Adds an assembly. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/21/2020. </remarks>
        ///
        /// <param name="moduleAssembly">   The module assembly. </param>

        public void AddAssembly(IModuleAssembly moduleAssembly)
        {
            if (!this.moduleAssemblies.Any(a => a.Name == moduleAssembly.Name))
            {
                var folders = folderHierarchy.GetAllFolders().Where(f => f.ModuleAssemblies.Any(a => a == moduleAssembly));

                if (folders.Count() > 0)
                {
                    var folder = folders.First();

                    DebugUtils.Break();
                }

                this.moduleAssemblies.Add(moduleAssembly);
            }
        }

        /// <summary>   Adds a module. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/21/2020. </remarks>
        ///
        /// <param name="module">   The module. </param>

        public void AddModule(Module module)
        {
            // kn - may not need this

            if (!this.modules.Any(a => a.Name == module.Name))
            {
                var folders = folderHierarchy.GetAllFolders().Where(f => f.ModuleAssemblies.Any(a => a == module));

                if (folders.Count() > 0)
                {
                    var folder = folders.First();

                    DebugUtils.Break();
                }

                this.modules.Add(module);
            }
        }

        /// <summary>   Gets the modules. </summary>
        ///
        /// <value> The modules. </value>

        [ScriptIgnore]
        public IEnumerable<Module> Modules
        {
            get
            {
                return this.modules;
            }
        }

        /// <summary>   Gets the module assemblies. </summary>
        ///
        /// <value> The module assemblies. </value>

        [ScriptIgnore]
        public IEnumerable<IModuleAssembly> ModuleAssemblies
        {
            get
            {
                return this.moduleAssemblies;
            }
        }

        /// <summary>   Gets a value indicating whether this  has folders. </summary>
        ///
        /// <value> True if this  has folders, false if not. </value>

        public bool HasFolders
        {
            get
            {
                return this.Folders.Count > 0;
            }
        }

        /// <summary>   Gets a value indicating whether this  has files. </summary>
        ///
        /// <value> True if this  has files, false if not. </value>

        public bool HasFiles
        {
            get
            {
                return this.Files.Count > 0;
            }
        }

        /// <summary>   Gets the name of the full. </summary>
        ///
        /// <value> The name of the full. </value>

        public override string FullName
        {
            get
            {
                return fullPath;
            }
        }

        /// <summary>   Gets the root. </summary>
        ///
        /// <value> The root. </value>

        [ScriptIgnore, JsonIgnore]
        public Folder Root
        {
            get
            {
                return ((ApplicationFolderHierarchy)folderHierarchy).RootFolder;
            }
        }
    }
}
