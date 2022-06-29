using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using System.Windows.Forms;
using EnvDTE;
using System.Runtime.InteropServices;

namespace BuildTasks
{
    public class ModifyConnection : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        public bool Execute()
        {
            return true;
        }
    }
}
