// file:	FolderStructure\FileSystemObjects.cs
//
// summary:	Implements the file system objects class

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;

namespace AbstraX.FolderStructure
{
    /// <summary>   A file system objects. </summary>
    ///
    /// <remarks>   Ken, 10/11/2020. </remarks>
    ///
    /// <typeparam name="T">    Generic type parameter. </typeparam>

    public class FileSystemObjects<T> : BaseList<T> where T : FileSystemObject
    {
        /// <summary>   The folder hierarchy. </summary>
        private IFolderHierarchy folderHierarchy;
        /// <summary>   The file system object. </summary>
        private FileSystemObject fileSystemObject;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="fileSystemObject"> The file system object. </param>
        /// <param name="folderHierarchy">  The folder hierarchy. </param>

        public FileSystemObjects(FileSystemObject fileSystemObject, IFolderHierarchy folderHierarchy)
        {
            this.folderHierarchy = folderHierarchy;
            this.fileSystemObject = fileSystemObject;
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="item"> The object to add to the
        ///                     <see cref="T:System.Collections.Generic.ICollection`1" />. </param>

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

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>

        public override void Clear()
        {
            base.Clear();
        }

        /// <summary>   Adds a range. </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="items">    An IEnumerable&lt;T&gt; of items to append to this. </param>

        public override void AddRange(IEnumerable<T> items)
        {
            base.AddRange(items);

            foreach (var item in items)
            {
                folderHierarchy.IndexAdd(item.RelativeFullName, item);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the
        /// <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="item"> The object to remove from the
        ///                     <see cref="T:System.Collections.Generic.ICollection`1" />. </param>
        ///
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the
        /// <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also
        /// returns false if <paramref name="item" /> is not found in the original
        /// <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>

        public override bool Remove(T item)
        {
            folderHierarchy.IndexRemove(item.RelativeFullName);

            return base.Remove(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified
        /// index.
        /// </summary>
        ///
        /// <remarks>   Ken, 10/11/2020. </remarks>
        ///
        /// <param name="index">    The zero-based index at which <paramref name="item" /> should be
        ///                         inserted. </param>
        /// <param name="item">     The object to insert into the
        ///                         <see cref="T:System.Collections.Generic.IList`1" />. </param>

        public override void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }
    }
}
