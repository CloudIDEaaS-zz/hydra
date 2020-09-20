using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using Utils;
using System.Web.Script.Serialization;

namespace AbstraX.FolderStructure
{
    public class File : FileSystemObject
    {
        public override bool Exists { get; }
        public Folder Folder { get; set; }
        public long Length { get; internal set; }
        public long Hash { get; internal set; }
        public override string Name { get; }
        public StringBuilder Info { get; }
        public Icon Icon { get; }
        public override FileSystemObjectKind Kind => FileSystemObjectKind.File;
        [ScriptIgnore]
        public FileInfo SystemLocalFile { get; }

        public File(ApplicationFolderHierarchy folderHierarchy, FileInfo systemLocalFile, StringBuilder info = null) : base(folderHierarchy)
        {
            FileInfo tempFile = null;

            this.Name = systemLocalFile.Name;
            this.SystemLocalFile = systemLocalFile;
            this.Info = info;

            if (!systemLocalFile.Exists)
            {
                tempFile = new FileInfo(Path.Combine(Path.GetTempPath(), "Temp" + systemLocalFile.Extension));

                using (tempFile.Create())
                {
                    this.Icon = tempFile.GetIcon<Icon>();
                }

                tempFile.Delete();
            }
            else
            {
                this.Icon = systemLocalFile.GetIcon<Icon>();
            }
        }

        public void SetNonAssemblyModulesDefaultFile(IEnumerable<Module> nonAssemblyModules)
        {
            foreach (var module in nonAssemblyModules)
            {
                module.File = this;
            }
        }

        public string FolderName
        {
            get
            {
                if (this.Folder != null)
                {
                    return this.Folder.FullName;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
