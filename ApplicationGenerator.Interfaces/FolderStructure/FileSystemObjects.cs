using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;

namespace AbstraX.FolderStructure
{
    public class FileSystemObjects<T> : BaseList<T> where T : FileSystemObject
    {
        private ApplicationFolderHierarchy folderHierarchy;
        private FileSystemObject fileSystemObject;

        public FileSystemObjects(FileSystemObject fileSystemObject, ApplicationFolderHierarchy folderHierarchy)
        {
            this.folderHierarchy = folderHierarchy;
            this.fileSystemObject = fileSystemObject;
        }

        public override void Add(T item)
        {
            if (item is Folder)
            {
                var folder = (Folder)(FileSystemObject) item;

                folder.Parent = (Folder) fileSystemObject;
            }
            else if (item is File)
            {
                var file = (File)(FileSystemObject) item;

                file.Folder = (Folder)fileSystemObject;
                file.fullPath = Path.Combine(file.Folder.FullName, file.Name).ForwardSlashes();
            }
            else
            {
                DebugUtils.Break();
            }

            folderHierarchy.IndexAdd(item.RelativeFullName, item);
            base.Add(item);
        }

        public override void Clear()
        {
            base.Clear();
        }

        public override void AddRange(IEnumerable<T> items)
        {
            base.AddRange(items);

            foreach (var item in items)
            {
                folderHierarchy.IndexAdd(item.RelativeFullName, item);
            }
        }

        public override bool Remove(T item)
        {
            folderHierarchy.IndexRemove(item.RelativeFullName);

            return base.Remove(item);
        }

        public override void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }
    }
}
