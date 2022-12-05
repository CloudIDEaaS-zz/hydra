using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Utilities;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Utils;

namespace HydraDebugAssistant.Info
{
    public class TreeNodeInfo
    {
        public FileSystemInfo FileSystemInfo { get; }
        public bool BreakpointSet { get; set; }
        public bool PausedOnBreakpoint { get; set; }

        public TreeNodeInfo(FileSystemInfo fileSystemInfo)
        {
            FileSystemInfo = fileSystemInfo;
        }
    }
}
