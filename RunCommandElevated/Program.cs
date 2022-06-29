using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
using Utils.GPO;

namespace RunCommandElevated
{
    public class Program
    {
        public static readonly Guid HYDRA_ADMIN_TOOL_GUID = Guid.NewGuid();
        private const int S_OK = 0;

        public static void Main(string[] args)
        {
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            FileSystemAccessRule rule;
            FileInfo file;
            FileSecurity fileSecurity;
            string filePath;
            int exitCode;

            if (args[0] == "addPathExclusions")
            {
                var globalNpmModules = Environment.ExpandEnvironmentVariables(@"%appdata%\npm\node_modules");
                bool enabled;
                int hResult;

                //hResult = GPOExclusionsHelper.GetPathExclusionsEnabled(HYDRA_ADMIN_TOOL_GUID, out enabled);

                //if (hResult != S_OK)
                //{
                //    DebugUtils.Break();
                //}

                hResult = GPOExclusionsHelper.AddLocation(HYDRA_ADMIN_TOOL_GUID, globalNpmModules);

                if (hResult != S_OK)
                {
                    DebugUtils.Break();
                }

                //hResult = GPOExclusionsHelper.GetPathExclusionsEnabled(HYDRA_ADMIN_TOOL_GUID, out enabled);

                //if (hResult != S_OK)
                //{
                //    DebugUtils.Break();
                //}

                //if (enabled)
                //{
                //    Console.WriteLine("Setting to {0}", !enabled);
                //}

                hResult = GPOExclusionsHelper.SetPathExclusionsEnabled(HYDRA_ADMIN_TOOL_GUID, true);

                if (hResult != S_OK)
                {
                    DebugUtils.Break();
                }

                //hResult = GPOExclusionsHelper.RemoveLocation(HYDRA_ADMIN_TOOL_GUID, globalNpmModules);

                //if (hResult != S_OK)
                //{
                //    DebugUtils.Break();
                //}

                //if (!enabled)
                //{
                //    hResult = GPOExclusionsHelper.SetPathExclusionsEnabled(HYDRA_ADMIN_TOOL_GUID, enabled);

                //    if (hResult != S_OK)
                //    {
                //        DebugUtils.Break();
                //    }
                //}

                return;
            }

            if (args[0] == "manageRunningProcesses")
            {
                var frmRunningProcesses = new frmRunningProcesses();

                frmRunningProcesses.Processes = args[1].Split("-").Select(id => Process.GetProcesses().SingleOrDefault(p => p.Id == int.Parse(id))).Where(p => p != null).ToList();
                frmRunningProcesses.Path = args[2].RemoveQuotes();

                try
                {
                    Environment.ExitCode = (int)frmRunningProcesses.ShowDialog();
                }
                catch (Exception ex)
                {
                    Environment.ExitCode = Marshal.GetExceptionCode();
                }

                return;
            }

            filePath = args[1].RemoveQuotes();
            file = new FileInfo(filePath);
            rule = new FileSystemAccessRule("Authenticated Users", FileSystemRights.FullControl, AccessControlType.Allow);

            fileSecurity = file.GetAccessControl();
            fileSecurity.SetAccessRule(rule);
            file.SetAccessControl(fileSecurity);

            if (args[0] == "manageLockingProcesses")
            {
                var frmLockingProcesses = new frmLockingProcesses();

                frmLockingProcesses.LockedPath = args[1].RemoveQuotes();

                try
                {
                    Environment.ExitCode = (int)frmLockingProcesses.ShowDialog();
                }
                catch (Exception ex)
                {
                    Environment.ExitCode = Marshal.GetExceptionCode();
                }

                return;
            }
            else
            {
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;

                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = $"/C { args.ToDelimitedList(" ")} ";
                startInfo.UseShellExecute = false;

                process.StartInfo = startInfo;

                process.OutputDataReceived += (s, e2) =>
                {
                    var data = e2.Data;

                    if (data != null)
                    {
                        Console.WriteLine(data);
                    }
                };

                process.ErrorDataReceived += (s, e2) =>
                {
                    var data = e2.Data;

                    if (data != null)
                    {
                        using (var colorizer = new ConsoleColorizer(ConsoleColor.Red))
                        {
                            Console.WriteLine(data);
                        }
                    }
                };

                process.Exited += (s, e2) =>
                {
                    exitCode = process.ExitCode;

                    if (exitCode != 0)
                    {
                        using (var colorizer = new ConsoleColorizer(ConsoleColor.Red))
                        {
                            Console.WriteLine("{0} {1} failed with ExitCode={2}", startInfo.FileName, startInfo.Arguments, exitCode);
                        }
                    }
                };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();
            }

            Environment.ExitCode = process.ExitCode;
        }
    }
}
