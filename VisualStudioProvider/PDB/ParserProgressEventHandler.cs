using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB
{
    public delegate void ParserProgressEventHandler(object sender, ParserProgressEventArgs e);

    public class ParserProgressEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public int Progress { get; private set; }
        public int Total { get; private set; }

        public ParserProgressEventArgs(string message, int progress, int total)
        {
            this.Message = message;
            this.Progress = progress;
            this.Total = total;
        }
    }
}
