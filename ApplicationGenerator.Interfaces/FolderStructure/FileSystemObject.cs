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
    [DebuggerDisplay(" { DebugInfo } ")]
    public abstract class FileSystemObject
    {
        [ScriptIgnore]
        protected internal string fullPath;
        [ScriptIgnore]
        protected readonly ApplicationFolderHierarchy folderHierarchy;

        public abstract FileSystemObjectKind Kind { get; }
        public abstract string Name { get; }
        public string Extension { get; }
        public abstract bool Exists { get; }

        public FileSystemObject(ApplicationFolderHierarchy folderHierarchy)
        {
            this.folderHierarchy = folderHierarchy;
        }

        public virtual string FullName
        {
            get
            {
                return fullPath;
            }
        }

        internal static bool IsPathSystemRooted(string path)
        {
            var rooted = path.AsCaseless().StartsWithAny(DriveInfo.GetDrives().Select(d => d.Name).ToArray());

            if (!rooted)
            {
                rooted = path.BackSlashes().AsCaseless().StartsWithAny(DriveInfo.GetDrives().Select(d => d.Name).ToArray());
            }

            return rooted;
        }

        [ScriptIgnore]
        public string DebugInfo
        {
            get
            {
                return this.Kind.ToString() + ": " + this.RelativeFullName;
            }
        }

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

        internal static string BluntStart(string name, string root)
        {
            var fullName = name.RemoveStartIfMatches(root);

            return fullName.AppendIf("/", fullName.IsNullOrEmpty());
        }

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
