using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Linq;
using Utils;

namespace AbstraX.FolderStructure
{
    public class Folder : FileSystemObject
    {
        public override string Name { get; }
        public override bool Exists { get; }
        [ScriptIgnore]
        public Folder Parent { get; set; }
        [ScriptIgnore]
        public FileSystemObjects<File> Files { get; }
        [ScriptIgnore]
        public FileSystemObjects<Folder> Folders { get; }
        public override FileSystemObjectKind Kind => FileSystemObjectKind.Folder;
        private List<IModuleAssembly> moduleAssemblies;
        private List<Module> modules;

        public Folder(ApplicationFolderHierarchy folderHierarchy, string path) : base(folderHierarchy)
        {
            this.Folders = new FileSystemObjects<Folder>(this, folderHierarchy);
            this.Files = new FileSystemObjects<File>(this, folderHierarchy);
            this.moduleAssemblies = new List<IModuleAssembly>();
            this.modules = new List<Module>();
            this.Name = Path.GetFileName(path);

            fullPath = path;
        }

        public int FileCount
        {
            get
            {
                return this.Files.Count;
            }
        }

        public int FolderCount
        {
            get
            {
                return this.Folders.Count;
            }
        }

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

        [ScriptIgnore]
        public IEnumerable<Module> Modules
        {
            get
            {
                return this.modules;
            }
        }

        [ScriptIgnore]
        public IEnumerable<IModuleAssembly> ModuleAssemblies
        {
            get
            {
                return this.moduleAssemblies;
            }
        }

        public bool HasFolders
        {
            get
            {
                return this.Folders.Count > 0;
            }
        }

        public bool HasFiles
        {
            get
            {
                return this.Files.Count > 0;
            }
        }

        public override string FullName
        {
            get
            {
                return fullPath;
            }
        }

        [ScriptIgnore]
        public Folder Root
        {
            get
            {
                return this.folderHierarchy.ProjectRootFolder;
            }
        }
    }
}
