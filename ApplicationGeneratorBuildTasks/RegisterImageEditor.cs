using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Build.Framework;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BuildTasks
{
    public class RegisterImageEditor : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        public bool Execute()
        {
            try
            {
                if (Regex.IsMatch(Environment.CommandLine, ".*?msbuild.exe\" "))
                {
                    return true;
                }

                var projectFile = new FileInfo(this.BuildEngine.ProjectFileOfTaskNode);
                var targetPath = Path.Combine(projectFile.Directory.FullName, @"bin\Debug\ImageEditor.dll");
                var dotNetFxFolder = string.Empty;

                if (Environment.Is64BitOperatingSystem)
                {
                    dotNetFxFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework64\v4.0.30319");
                }
                else
                {
                    dotNetFxFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework\v4.0.30319");
                }

                var process = new Process();
                var startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    FileName = Path.Combine(dotNetFxFolder, @"RegAsm.exe"),
                    Arguments = "\"" + targetPath + "\" /codebase /tlb"
                };

                process.StartInfo = startInfo;
                process.Start();

                var result = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, "Error registering Image Editor.  " + result + ".  You can also run Visual Studio as Administrator to install this component.", "", "", DateTime.Now);
                    this.BuildEngine.LogErrorEvent(message);
                }

            }
            catch (Exception ex)
            {
                var error = string.Format("Error registering Image Editor: {0}. You can also run Visual Studio as Administrator to install this component.", ex);

                var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, error, "", "", DateTime.Now);
                this.BuildEngine.LogErrorEvent(message);

                Console.WriteLine(error);
                return false;
            }

            return true;
        }
    }
}
