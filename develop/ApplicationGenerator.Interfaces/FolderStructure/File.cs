// file:	FolderStructure\File.cs
//
// summary:	Implements the file class

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using Utils;
using System.Web.Script.Serialization;

namespace AbstraX.FolderStructure
{
    /// <summary>   A file. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/3/2022. </remarks>

    public class File : FileSystemObject
    {
        /// <summary>   Gets a value indicating whether the exists. </summary>
        ///
        /// <value> True if exists, false if not. </value>

        public override bool Exists { get; }

        /// <summary>   Gets or sets the pathname of the folder. </summary>
        ///
        /// <value> The pathname of the folder. </value>

        public Folder Folder { get; set; }

        /// <summary>   Gets or sets the length. </summary>
        ///
        /// <value> The length. </value>

        public long Length { get; internal set; }

        /// <summary>   Gets or sets the hash. </summary>
        ///
        /// <value> The hash. </value>

        public long Hash { get; internal set; }

        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>

        public override string Name { get; }

        /// <summary>   Gets the information. </summary>
        ///
        /// <value> The information. </value>

        public StringBuilder Info { get; }

        /// <summary>   Gets the icon. </summary>
        ///
        /// <value> The icon. </value>

        public Icon Icon { get; }

        /// <summary>   Gets the kind. </summary>
        ///
        /// <value> The kind. </value>

        public override FileSystemObjectKind Kind => FileSystemObjectKind.File;

        /// <summary>   Gets the system local file. </summary>
        ///
        /// <value> The system local file. </value>

        [ScriptIgnore]
        public FileInfo SystemLocalFile { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/3/2022. </remarks>
        ///
        /// <param name="folderHierarchy">  The folder hierarchy. </param>
        /// <param name="systemLocalFile">  The system local file. </param>
        /// <param name="info">             (Optional) The information. </param>

        public File(IFolderHierarchy folderHierarchy, FileInfo systemLocalFile, StringBuilder info = null) : base(folderHierarchy)
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
                    this.Icon = tempFile.GetLargeIcon<Icon>();
                }

                tempFile.Delete();
            }
            else
            {
                this.Icon = systemLocalFile.GetLargeIcon<Icon>();
            }
        }

        /// <summary>   Sets non assembly modules default file. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/3/2022. </remarks>
        ///
        /// <param name="nonAssemblyModules">   The non assembly modules. </param>

        public void SetNonAssemblyModulesDefaultFile(IEnumerable<Module> nonAssemblyModules)
        {
            foreach (var module in nonAssemblyModules)
            {
                module.File = this;
            }
        }

        /// <summary>   Gets the pathname of the folder. </summary>
        ///
        /// <value> The pathname of the folder. </value>

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
