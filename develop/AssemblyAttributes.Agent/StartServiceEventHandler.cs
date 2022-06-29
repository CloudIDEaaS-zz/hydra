using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyAttributesShim.Agent
{
    public delegate void StartServiceEventHandler(object sender, StartServiceEventArgs e);

    public class StartServiceEventArgs : EventArgs
    {
        public TextWriter Writer { get; set; }
        public TextReader Reader { get; set; }

        public StartServiceEventArgs()
        {
        }
    }
}
