// file:	ApplicationFolderHierarchy.cs
//
// summary:	Implements the application folder hierarchy class

using AbstraX.FolderStructure;
using BTreeIndex.Collections.Generic.BTree;
using System;
using System.Collections.Generic;

namespace AbstraX
{
    /// <summary>   Interface for folder hierarchy. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/3/2022. </remarks>

    public interface IFolderHierarchy
    {
        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <param name="type"> An enum constant representing the type option. </param>
        ///
        /// <returns>   The indexed item. </returns>

        string this[Enum type] { get; }

        /// <summary>   Indexer to get items within this collection using array index syntax. </summary>
        ///
        /// <param name="name"> The name. </param>
        ///
        /// <returns>   The indexed item. </returns>

        string this[string name] { get; }

        /// <summary>   Gets the file system. </summary>
        ///
        /// <value> The file system. </value>

        FileSystem FileSystem { get; }

        /// <summary>   Gets the zero-based index of this. </summary>
        ///
        /// <value> The index. </value>

        BTreeDictionary<string, FileSystemObject> Index { get; }

        /// <summary>   Gets the root. </summary>
        ///
        /// <value> The root. </value>

        string Root { get; }

        /// <summary>   Gets the pathname of the root folder. </summary>
        ///
        /// <value> The pathname of the root folder. </value>

        Folder RootFolder { get; }

        /// <summary>   Index add. </summary>
        ///
        /// <param name="rootedFullName">   Name of the rooted full. </param>
        /// <param name="fileSystemObject"> The file system object. </param>

        void IndexAdd(string rootedFullName, FileSystemObject fileSystemObject);

        /// <summary>   Index remove. </summary>
        ///
        /// <param name="rootedFullName">   Name of the rooted full. </param>

        void IndexRemove(string rootedFullName);

        /// <summary>   Blunt start. </summary>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   A string. </returns>

        string BluntStart(string path);

        /// <summary>   Creates project path. </summary>
        ///
        /// <param name="pathCreate">   The path create. </param>
        /// <param name="returnFull">   (Optional) True to return full. </param>
        ///
        /// <returns>   The new project path. </returns>

        string CreateProjectPath(string pathCreate, bool returnFull = false);

        /// <summary>   Gets all files. </summary>
        ///
        /// <returns>   all files. </returns>

        List<AbstraX.FolderStructure.File> GetAllFiles();

        /// <summary>   Gets all folders. </summary>
        ///
        /// <returns>   all folders. </returns>

        List<Folder> GetAllFolders();

        /// <summary>   Gets all file system objects. </summary>
        ///
        /// <returns>   all file system objects. </returns>

        List<FileSystemObject> GetAllFileSystemObjects();
    }
}