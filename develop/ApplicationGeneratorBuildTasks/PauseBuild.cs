using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using System.Runtime.InteropServices;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Process = System.Diagnostics.Process;
using Thread = System.Threading.Thread;
using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio.Shell;

namespace BuildTasks
{
    public class PauseBuild : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        public bool Execute()
        {
            MessageBox.Show("Now!");
            return true;
        }
    }
}
