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
    public class RegisterPackageTool : ITask
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
                var targetPath = Path.Combine(projectFile.Directory.FullName, @"bin\Debug\HydraPackageTool.dll");
                var regPath = Path.Combine(projectFile.Directory.FullName, @"RegisterCustomTool.reg");
                var dotNetFxFolder = string.Empty;
                var netFXToolsFolder = string.Empty;

                if (Environment.Is64BitOperatingSystem)
                {
                    dotNetFxFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework64\v4.0.30319");
                    netFXToolsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\x64");
                }
                else
                {
                    dotNetFxFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework\v4.0.30319");
                    netFXToolsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Microsoft SDKs\Windows\v7.0A\Bin\NETFX 4.0 Tools\x86");
                }

                var process = new Process();
                var startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = Path.Combine(dotNetFxFolder, @"RegAsm.exe"),
                    Arguments = "\"" + targetPath + "\" /codebase"
                };

                process.StartInfo = startInfo;
                process.Start();

                var result = process.StandardOutput.ReadToEnd();
                result += "\r\n" + process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, "Error registering Package Tool.  " + result + ".  You can also run Visual Studio as Administrator to install this component.", "", "", DateTime.Now);
                    this.BuildEngine.LogErrorEvent(message);
                }
                else
                {
                    var message = new BuildMessageEventArgs("Package Tool registered successfully.  " + result, "", "", MessageImportance.High);
                    this.BuildEngine.LogMessageEvent(message);
                }

                targetPath = Path.GetFullPath(Path.Combine(projectFile.Directory.FullName, @".."));

                targetPath = Path.Combine(targetPath, @"HydraPackageToolInterfaces\bin\Debug\HydraPackageToolInterfaces.dll");

                process = new Process();
                startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = Path.Combine(netFXToolsFolder, @"gacutil.exe"),
                    Arguments = "/u \"HydraPackageToolInterfaces\""
                };

                process.StartInfo = startInfo;
                process.Start();

                result = process.StandardError.ReadToEnd();
                result += "\r\n" + process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, "Error unregistering Package Interfaces.  " + result + ".  You can also run Visual Studio as Administrator to install this component.", "", "", DateTime.Now);
                    this.BuildEngine.LogErrorEvent(message);
                }
                else
                {
                    var message = new BuildMessageEventArgs("Package Interfaces unregistered successfully.  " + result, "", "", MessageImportance.High);
                    this.BuildEngine.LogMessageEvent(message);
                }

                process = new Process();
                startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    FileName = Path.Combine(netFXToolsFolder, @"gacutil.exe"),
                    Arguments = "/i \"" + targetPath + "\""
                };

                process.StartInfo = startInfo;
                process.Start();

                result = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, "Error registering Package Interfaces.  " + result + ".  You can also run Visual Studio as Administrator to install this component.", "", "", DateTime.Now);
                    this.BuildEngine.LogErrorEvent(message);
                }
                else
                {
                    var message = new BuildMessageEventArgs("Package Interfaces registered successfully.  " + result, "", "", MessageImportance.High);
                    this.BuildEngine.LogMessageEvent(message);
                }

                process = new Process();
                startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    FileName = "regedit.exe",
                    Arguments = "/s \"" + regPath + "\""
                };

                process.StartInfo = startInfo;
                process.Start();

                result = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, "Error registering Package Tool.  " + result + ".  You can also run Visual Studio as Administrator to install this component.", "", "", DateTime.Now);
                    this.BuildEngine.LogErrorEvent(message);
                }
                else
                {
                    var message = new BuildMessageEventArgs("Package Tool registered successfully.  " + result, "", "", MessageImportance.High);
                    this.BuildEngine.LogMessageEvent(message);
                }
            }
            catch (Exception ex)
            {
                var error = string.Format("Error registering Package Tool: {0}. You can also run Visual Studio as Administrator to install this component.", ex);

                var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, error, "", "", DateTime.Now);
                this.BuildEngine.LogErrorEvent(message);

                Console.WriteLine(error);
                return false;
            }

            return true;
        }
    }
}
