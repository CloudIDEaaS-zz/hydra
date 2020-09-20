using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB
{
    public delegate void LibraryInfoEventHandler(object sender, LibraryInfoEventArgs e);

    public class LibraryInfoEventArgs : EventArgs
    {
        public string LibraryFile { get; private set; }

        public LibraryInfoEventArgs(string libraryFile)
        {
            this.LibraryFile = libraryFile;
        }
    }
}
