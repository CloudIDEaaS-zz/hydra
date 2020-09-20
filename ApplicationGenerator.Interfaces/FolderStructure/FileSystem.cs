using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;

namespace AbstraX.FolderStructure
{
    public class FileSystem
    {
        private ApplicationFolderHierarchy folderHierarchy;

        public FileSystem(ApplicationFolderHierarchy folderHierarchy)
        {
            this.folderHierarchy = folderHierarchy;
        }

        public bool Contains(string path)
        {
            if (path.Contains(@"\"))
            {
                path = path.ForwardSlashes();
            }

            if (FileSystemObject.IsPathSystemRooted(path))
            {
                if (path.StartsWith(folderHierarchy.ProjectRoot))
                {
                    path = FileSystemObject.BluntStart(path, folderHierarchy.Root);
                }
                else if (path.StartsWith(folderHierarchy.ServicesRoot))
                {
                    path = FileSystemObject.BluntStart(path, folderHierarchy.Root);

                    if (FileSystemObject.IsPathSystemRooted(path))
                    {
                        path = FileSystemObject.BluntStart(path, folderHierarchy.ServicesRoot);
                    }
                }
            }

            return folderHierarchy.Index.ContainsKey(path);
        }

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
                    if (path.StartsWith(folderHierarchy.ProjectRoot))
                    {
                        path = FileSystemObject.BluntStart(path, folderHierarchy.Root);
                    }
                    else if (path.StartsWith(folderHierarchy.ServicesRoot))
                    {
                        path = FileSystemObject.BluntStart(path, folderHierarchy.Root);

                        if (FileSystemObject.IsPathSystemRooted(path))
                        {
                            path = FileSystemObject.BluntStart(path, folderHierarchy.ServicesRoot);
                        }
                    }
                }

                obj = folderHierarchy.Index[path];

                DebugUtils.ThrowIf(obj == null, () => new DirectoryNotFoundException(string.Format("Path '{0}' does not exist in virtual file system.", path)));

                return obj;
            }
        }

        public bool Exists(string path)
        {
            if (FileSystemObject.IsPathSystemRooted(path))
            {
                if (path.StartsWith(folderHierarchy.ProjectRoot))
                {
                    path = FileSystemObject.BluntStart(path, folderHierarchy.Root);
                }
                else if (path.StartsWith(folderHierarchy.ServicesRoot))
                {
                    path = FileSystemObject.BluntStart(path, folderHierarchy.Root);
                }
            }

            return folderHierarchy.Index.ContainsKey(path);
        }

        public string CreatePath(string path)
        {
            return folderHierarchy.CreateProjectPath(path);
        }

        public File AddSystemLocalProjectFile(FileInfo fileInfo, string info)
        {
            var builder = new StringBuilder(info);

            return AddSystemLocalProjectFile(fileInfo, builder);
        }

        public void DeleteFile(string filePath)
        {
            string folderPath;
            string fileName;

            if (FileSystemObject.IsPathSystemRooted(filePath))
            {
                filePath = filePath.ForwardSlashes();

                if (filePath.StartsWith(folderHierarchy.ProjectRoot))
                {
                    filePath = FileSystemObject.BluntStart(filePath, folderHierarchy.Root);
                }
                else if (filePath.StartsWith(folderHierarchy.ServicesRoot))
                {
                    filePath = FileSystemObject.BluntStart(filePath, folderHierarchy.Root);

                    if (FileSystemObject.IsPathSystemRooted(filePath))
                    {
                        filePath = FileSystemObject.BluntStart(filePath, folderHierarchy.ServicesRoot);
                    }
                }
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

        public File AddSystemLocalServicesFile(FileInfo fileInfo, string info)
        {
            var builder = new StringBuilder(info);

            return AddSystemLocalServicesFile(fileInfo, builder);
        }

        public File AddSystemLocalServicesFile(FileInfo fileInfo, StringBuilder info = null)
        {
            var filePath = fileInfo.FullName;
            var folderPath = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            File file;
            Folder folder;

            if (!folderHierarchy.FileSystem.Exists(folderPath))
            {
                folderPath = folderHierarchy.CreateServicesPath(folderPath);
            }

            folder = (Folder)folderHierarchy.FileSystem[folderPath];
            file = new File(folderHierarchy, fileInfo, info);

            folder.Files.Add(file);

            return file;
        }
    }
}
