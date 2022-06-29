// file:	FolderStructure\FileSystemObject.cs
//
// summary:	Implements the file system object class

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Utils;
using System.Linq;
using System.Web.Script.Serialization;

namespace AbstraX.FolderStructure
{
    /// <summary>   A file system object. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 3/4/2022. </remarks>

    [DebuggerDisplay(" { DebugInfo } ")]
    public abstract class FileSystemObject
    {
        /// <summary>   Full pathname of the full file. </summary>
        [ScriptIgnore]
        public string fullPath;
        /// <summary>   The folder hierarchy. </summary>
        [ScriptIgnore]
        protected readonly IFolderHierarchy folderHierarchy;

        /// <summary>   Gets the kind. </summary>
        ///
        /// <value> The kind. </value>

        public abstract FileSystemObjectKind Kind { get; }

        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>

        public abstract string Name { get; }

        /// <summary>   Gets the extension. </summary>
        ///
        /// <value> The extension. </value>

        public string Extension { get; }

        /// <summary>   Gets a value indicating whether the exists. </summary>
        ///
        /// <value> True if exists, false if not. </value>

        public abstract bool Exists { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2022. </remarks>
        ///
        /// <param name="folderHierarchy">  The folder hierarchy. </param>

        public FileSystemObject(IFolderHierarchy folderHierarchy)
        {
            this.folderHierarchy = folderHierarchy;
        }

        /// <summary>   Gets the name of the full. </summary>
        ///
        /// <value> The name of the full. </value>

        public virtual string FullName
        {
            get
            {
                return fullPath;
            }
        }

        /// <summary>   Query if 'path' is path system rooted. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2022. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        ///
        /// <returns>   True if path system rooted, false if not. </returns>

        internal static bool IsPathSystemRooted(string path)
        {
            var rooted = path.AsCaseless().StartsWithAny(DriveInfo.GetDrives().Select(d => d.Name).ToArray());

            if (!rooted)
            {
                rooted = path.BackSlashes().AsCaseless().StartsWithAny(DriveInfo.GetDrives().Select(d => d.Name).ToArray());
            }

            return rooted;
        }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        [ScriptIgnore]
        public string DebugInfo
        {
            get
            {
                return this.Kind.ToString() + ": " + this.RelativeFullName;
            }
        }

        /// <summary>   Gets the name of the relative full. </summary>
        ///
        /// <value> The name of the relative full. </value>

        public virtual string RelativeFullName
        {
            get
            {
                if (this.FullName == null)
                {
                    return this.Name;
                }
                else if (folderHierarchy != null)
                {
                    return BluntStart(this.FullName, folderHierarchy.Root);
                }
                else
                {
                    return this.FullName;
                }
            }
        }

        /// <summary>   Blunt start. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2022. </remarks>
        ///
        /// <param name="name"> The name. </param>
        /// <param name="root"> The root. </param>
        ///
        /// <returns>   A string. </returns>

        internal static string BluntStart(string name, string root)
        {
            var fullName = name.RemoveStartIfMatches(root);

            return fullName.AppendIf("/", fullName.IsNullOrEmpty());
        }

        /// <summary>   Path combine. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2022. </remarks>
        ///
        /// <param name="path1">            The first path. </param>
        /// <param name="path2">            The second path. </param>
        /// <param name="path3">            The third path. </param>
        /// <param name="forwardSlashes">   (Optional) True to forward slashes. </param>
        ///
        /// <returns>   A string. </returns>

        public static string PathCombine(string path1, string path2, string path3, bool forwardSlashes = false)
        {
            path1 = path1.BackSlashes();
            path2 = path2.BackSlashes().RemoveStartIfMatches(@"\");

            if (forwardSlashes)
            {
                return Path.Combine(path1, path2, path3).ForwardSlashes();
            }
            else
            {
                return Path.Combine(path1, path2, path3);
            }
        }

        /// <summary>   Path combine. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/4/2022. </remarks>
        ///
        /// <param name="path1">            The first path. </param>
        /// <param name="path2">            The second path. </param>
        /// <param name="forwardSlashes">   (Optional) True to forward slashes. </param>
        ///
        /// <returns>   A string. </returns>

        public static string PathCombine(string path1, string path2, bool forwardSlashes = false)
        {
            path1 = path1.BackSlashes();
            path2 = path2.BackSlashes().RemoveStartIfMatches(@"\");

            if (forwardSlashes)
            {
                return Path.Combine(path1, path2).ForwardSlashes();
            }
            else
            {
                return Path.Combine(path1, path2);
            }
        }
    }
}
