using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Build.Framework;
using System.Diagnostics;
using System.Threading;

namespace BuildTasks
{
    public class KillCassiniDelayed : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }
        private static Timer timer;
        private static int count;

        public bool Execute()
        {
            try
            {
                var projectFile = new FileInfo(this.BuildEngine.ProjectFileOfTaskNode);
                var killCasiniPath = Path.Combine(projectFile.Directory.Parent.FullName, @"Binaries\SolutionLibraries\KillCasini.exe");

                var process = new Process();
                var startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    FileName = killCasiniPath
                };

                process.StartInfo = startInfo;
                process.Start();
            }
            catch (Exception ex)
            {
                var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, string.Format("Error launching KillCasini. {0}", ex.Message) + ". You can try running Visual Studio as Administrator to uninstall this component.", "", "", DateTime.Now);
                this.BuildEngine.LogErrorEvent(message);
                return false;
            }

            return true;
        }
    }
}
