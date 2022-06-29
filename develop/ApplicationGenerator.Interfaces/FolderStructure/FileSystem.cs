// file:	FolderStructure\FileSystem.cs
//
// summary:	Implements the file system class

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utils;

namespace AbstraX.FolderStructure
{
    /// <summary>   A file system. </summary>
    ///
    /// <remarks>   Ken, 10/11/2020. </remarks>

    public class FileSystem
    {
        /// <summary>   The folder hierarchy. </summary>
        private IFolderHierarchy folderHierarchy;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="folderHierarchy">  The folder hierarchy. </param>

        public FileSystem(IFolderHierarchy folderHierarchy)
        {
            this.folderHierarchy = folderHierarchy;
        }

        /// <summary>   Query if this  contains the given path. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   True if the object is in this collection, false if not. </returns>

        public bool Contains(string path)
        {
            if (path.Contains(@"\"))
            {
                path = path.ForwardSlashes();
            }

            if (FileSystemObject.IsPathSystemRooted(path))
            {
            }

            return folderHierarchy.Index.ContainsKey(path);
        }

        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   The indexed item. </returns>

        public FileSystemObject this[string path]
        {
            get
            {
                FileSystemObject obj;

                if (path.Contains(@"\"))
                {
                    path = path.ForwardSlashes();
                }

                if (FileSystemObject.IsPathSystemRooted(path))
                {
                    path = folderHierarchy.BluntStart(path);
                }

                obj = folderHierarchy.Index[path];

                DebugUtils.ThrowIf(obj == null, () => new DirectoryNotFoundException(string.Format("Path '{0}' does not exist in virtual file system.", path)));

                return obj;
            }
        }

        /// <summary>   Determine if 'path' exists. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool Exists(string path)
        {
            if (FileSystemObject.IsPathSystemRooted(path))
            {
                path = folderHierarchy.BluntStart(path);
            }

            return folderHierarchy.Index.ContainsKey(path);
        }

        /// <summary>   Creates a path. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   The new path. </returns>

        public string CreatePath(string path)
        {
            return folderHierarchy.CreateProjectPath(path);
        }

        /// <summary>   Adds a system local project file to 'info'. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="fileInfo"> Information describing the file. </param>
        /// <param name="info">     The information. </param>
        ///
        /// <returns>   A File. </returns>

        public File AddSystemLocalProjectFile(FileInfo fileInfo, string info)
        {
            var builder = new StringBuilder(info);

            return AddSystemLocalProjectFile(fileInfo, builder);
        }

        /// <summary>   Deletes the file described by filePath. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="filePath"> Full pathname of the file. </param>

        public void DeleteFile(string filePath)
        {
            string folderPath;
            string fileName;

            if (FileSystemObject.IsPathSystemRooted(filePath))
            {
                filePath = filePath.ForwardSlashes();

                filePath = folderHierarchy.BluntStart(filePath);
            }

            folderPath = Path.GetDirectoryName(filePath);
            folderPath = folderPath.ForwardSlashes();

            fileName = Path.GetFileName(filePath);

            if (folderHierarchy.FileSystem.Exists(filePath))
            {
                var folder = (Folder)folderHierarchy.FileSystem[folderPath];
                var file = (File)folderHierarchy.FileSystem[filePath];

                folder.Files.Remove(file);
            }
        }

        /// <summary>   Clears the empty files. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/31/2020. </remarks>

        public void ClearEmptyFiles()
        {
            var files = this.folderHierarchy.GetAllFiles();

            foreach (var file in files.Where(f => f.Length == 0))
            {
                this.DeleteFile(file.FullName);
            }
        }

        /// <summary>   Adds a system local project file to 'info'. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="fileInfo"> Information describing the file. </param>
        /// <param name="info">     (Optional) The information. </param>
        ///
        /// <returns>   A File. </returns>

        public File AddSystemLocalProjectFile(FileInfo fileInfo, StringBuilder info = null)
        {
            var filePath = fileInfo.FullName;
            var folderPath = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            File file;
            Folder folder;

            if (!folderHierarchy.FileSystem.Exists(folderPath))
            {
                folderPath = folderHierarchy.CreateProjectPath(folderPath);
            }

            folder = (Folder) folderHierarchy.FileSystem[folderPath];
            file = new File(folderHierarchy, fileInfo, info);

            folder.Files.Add(file);

            return file;
        }

        /// <summary>   Adds a system local services file to 'info'. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="fileInfo"> Information describing the file. </param>
        /// <param name="info">     The information. </param>
        ///
        /// <returns>   A File. </returns>

        public File AddSystemLocalServicesFile(FileInfo fileInfo, string info)
        {
            var builder = new StringBuilder(info);

            return AddSystemLocalServicesFile(fileInfo, builder);
        }

        /// <summary>   Adds a system local services file to 'info'. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="fileInfo"> Information describing the file. </param>
        /// <param name="info">     (Optional) The information. </param>
        ///
        /// <returns>   A File. </returns>

        public File AddSystemLocalServicesFile(FileInfo fileInfo, StringBuilder info = null)
        {
            var filePath = fileInfo.FullName;
            var folderPath = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            File file;
            Folder folder;

            if (!folderHierarchy.FileSystem.Exists(folderPath))
            {
                folderPath = ((ApplicationFolderHierarchy) folderHierarchy).CreateServicesPath(folderPath);
            }

            folder = (Folder)folderHierarchy.FileSystem[folderPath];
            file = new File(folderHierarchy, fileInfo, info);

            folder.Files.Add(file);

            return file;
        }

        /// <summary>   Adds a system local Entities file to 'info'. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="fileInfo"> Information describing the file. </param>
        /// <param name="info">     The information. </param>
        ///
        /// <returns>   A File. </returns>

        public File AddSystemLocalEntitiesFile(FileInfo fileInfo, string info)
        {
            var builder = new StringBuilder(info);

            return AddSystemLocalEntitiesFile(fileInfo, builder);
        }

        /// <summary>   Adds a system local Entities file to 'info'. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="fileInfo"> Information describing the file. </param>
        /// <param name="info">     (Optional) The information. </param>
        ///
        /// <returns>   A File. </returns>

        public File AddSystemLocalEntitiesFile(FileInfo fileInfo, StringBuilder info = null)
        {
            var filePath = fileInfo.FullName;
            var folderPath = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            File file;
            Folder folder;

            if (!folderHierarchy.FileSystem.Exists(folderPath))
            {
                folderPath = ((ApplicationFolderHierarchy)folderHierarchy).CreateEntitiesPath(folderPath);
            }

            folder = (Folder)folderHierarchy.FileSystem[folderPath];
            file = new File(folderHierarchy, fileInfo, info);

            folder.Files.Add(file);

            return file;
        }
    }
}
