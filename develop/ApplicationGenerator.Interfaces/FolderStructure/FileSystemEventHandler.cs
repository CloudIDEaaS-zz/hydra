// file:	FolderStructure\FileSystemEventHandler.cs
//
// summary:	Implements the file system event handler class

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.FolderStructure
{
    /// <summary>   Delegate for handling FileSystem events. </summary>
    ///
    /// <remarks>   Ken, 10/12/2020. </remarks>
    ///
    /// <param name="sender">   Source of the event. </param>
    /// <param name="e">        File system event information. </param>

    public delegate void FileSystemEventHandler(object sender, FileSystemEventArgs e);

    /// <summary>   Additional information for file system events. </summary>
    ///
    /// <remarks>   Ken, 10/12/2020. </remarks>

    public class FileSystemEventArgs : EventArgs
    {
        /// <summary>   Gets information describing the file. </summary>
        ///
        /// <value> Information describing the file. </value>

        public FileInfo FileInfo { get; }

        /// <summary>   Gets the virtual file. </summary>
        ///
        /// <value> The virtual file. </value>

        public File VirtualFile { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/12/2020. </remarks>
        ///
        /// <param name="fileInfo"> Information describing the file. </param>
        /// <param name="file">     The file. </param>

        public FileSystemEventArgs(FileInfo fileInfo, File file)
        {
            this.FileInfo = fileInfo;
            this.VirtualFile = file;
        }
    }
}
