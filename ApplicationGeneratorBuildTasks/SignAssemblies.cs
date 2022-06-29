using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Build.Framework;
using System.Diagnostics;
using System.Xml.XPath;
using System.Xml;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Process = System.Diagnostics.Process;
using Debugger = System.Diagnostics.Debugger;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO.Packaging;
using Utils;
using System.Xml.Linq;
using ApplicationGeneratorBuildTasks;
using Utils.Hierarchies;
using Brutal.Dev.StrongNameSigner;
using System.Reflection;
using BuildTasks.Utility;
using RunDllExport;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace BuildTasks
{
    public class SignAssemblies : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }
        [Required] 
        public string Configuration { get; set; }
        [Required]
        public string TargetFramework { get; set; }
        [Required]
        public string TargetAssembly { get; set; }
        [Required]
        public string SolutionDir { get; set; }
        [Required]
        public string ProductRelativePath { get; set; }
        [Required]
        public string TargetDir { get; set; }
        [Required]
        public string ProjectPath { get; set; }

        public bool Execute()
        {
            try
            {
                if (Keys.ControlKey.IsPressed())
                {
                    if (MessageBox.Show("Debug?", "Debug SignAssemblies", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Debugger.Launch();
                    }
                }

                if (this.Configuration != "Release")
                {
                    return true;
                }

                var solutionDir = this.SolutionDir;
                var productFilePath = Path.Combine(solutionDir, this.ProductRelativePath);
                var targetBinaries = TargetDir;
                var projectPath = this.ProjectPath;
                var projectFileNameNoExt = Path.GetFileNameWithoutExtension(projectPath);
                var (components, packageFiles) = ComponentFinder.GetComponents(targetBinaries, projectPath, this.TargetFramework, string.Empty, productFilePath);
                var directories = ComponentFinder.GetDirectories(targetBinaries, projectPath, this.TargetFramework, this.TargetAssembly, productFilePath);
                var outputPath = Path.Combine(targetBinaries, "Signed");
                var outputDirectory = new DirectoryInfo(outputPath);
                var componentFiles = components.Select(c => new FileInfo(Path.Combine(solutionDir, c.File.Source.RemoveStartIfMatches(@"..\"))));
                var newFilesToFix = new List<FileInfo>();
                var paths = componentFiles.Select(f => f.DirectoryName).Distinct().ToArray();
                var keyPath = Path.Combine(solutionDir, @"Hydra.ProdReleaseBuildTarget\HydraCodeCert\CloudIDEaaS.pfx");
                var snkKeyPath = Path.Combine(solutionDir, @"Hydra.ProdReleaseBuildTarget\HydraCodeCert\Key.snk");
                var password = "Ca1hm%%3na8s#sirJnGa7BGJhN2OQ7Fy584OoxEuD!2FYSb6NjKXNSCSYSCwOfEX#2j3BX8CM$5GC%y!IwUN6bMP83NwM0MLApa!";
                var certificate = new X509Certificate2(keyPath, password, X509KeyStorageFlags.Exportable);
                ApplicationGeneratorBuildTasks.Directory topLeveDirectory;
                FileInfo existingScannerFile;
                FileInfo newScannerFile = null;

                if (outputDirectory.Exists)
                {
                    outputDirectory.ForceDeleteAllFilesAndSubFolders();
                }

                foreach (var componentFile in componentFiles.Where(f => f.Extension.IsOneOf(".dll", ".exe")))
                {
                    var path = componentFile.FullName;

                    try
                    {
                        if (componentFile.Exists)
                        {
                            AssemblyInfo oldInfo;
                            AssemblyInfo newInfo;

                            Console.WriteLine("Strong-name signing {0}...", componentFile.Name);

                            oldInfo = SigningHelper.GetAssemblyInfo(path);

                            if (oldInfo.IsSigned)
                            {
                                var copyToFileInfo = new FileInfo(Path.Combine(outputPath, componentFile.Name));

                                Console.WriteLine("Already signed");

                                componentFile.CopyTo(copyToFileInfo.FullName);
                            }
                            else
                            {
                                FileInfo newFile;

                                if (componentFile.Name.AsCaseless() == "Hydra.Scanner.dll")
                                {
                                    var existinComponentFile = componentFile;
                                    var originalComponentFile = Path.Combine(solutionDir, @"Hydra.Scanner\bin\Release\Hydra.Scanner.dll");

                                    existingScannerFile = componentFile;
                                    newScannerFile = new FileInfo(Path.Combine(outputPath, "Hydra.Scanner.dll"));

                                    path = originalComponentFile;
                                    newInfo = SigningHelper.SignAssembly(path, snkKeyPath, outputPath, null, paths);
                                }
                                else
                                {
                                    newInfo = SigningHelper.SignAssembly(path, keyPath, outputPath, password, paths);
                                }

                                newFile = new FileInfo(newInfo.FilePath);

                                newFilesToFix.Add(newFile);

                                if (newInfo.IsSigned)
                                {
                                    Console.WriteLine("Strong-name signed successfully");
                                }
                                else
                                {
                                    Console.WriteLine("Strong-name signed successfully");
                                }
                            }
                        }
                        else
                        {
                            throw new FileNotFoundException($"File does not exist: '{ path }'");
                        }
                    }
                    catch (Exception ex)
                    {
                        var error = string.Format("Warning, Error signing assembly, \r\nError: {0}\r\nFile: {1}", ex.ToString(), path);

                        var message = new BuildWarningEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, error, "", "", DateTime.Now);
                        this.BuildEngine.LogWarningEvent(message);

                        Console.WriteLine(error);
                    }
                }

                foreach (var newFile in newFilesToFix)
                {
                    try
                    {
                        var refFiles = newFilesToFix.Where(f => f.FullName != newFile.FullName);

                        foreach (var refFile in refFiles)
                        {
                            Console.WriteLine("Fixing references to {1} in {0}...", newFile.Name, refFile.Name);

                            if (!SigningHelper.FixAssemblyReference(newFile.FullName, refFile.FullName, keyPath, password, paths))
                            {
                                Console.WriteLine("Nothing to fix");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var error = string.Format("Warning, Error fixing assembly, \r\nError: {0}\r\nFile: {1}", ex.ToString(), newFile.FullName);

                        var message = new BuildWarningEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, error, "", "", DateTime.Now);
                        this.BuildEngine.LogWarningEvent(message);

                        Console.WriteLine(error);
                    }
                }    

                foreach (var newSigned in newFilesToFix)
                {
                    try
                    {
                        Console.WriteLine("Removing invalid friend references from '{0}'...", newSigned.Name);

                        if (!SigningHelper.RemoveInvalidFriendAssemblies(newSigned.FullName, keyPath, password, paths))
                        {
                            Console.WriteLine("Nothing to fix");
                        }
                        else
                        {
                            Console.WriteLine("Invalid friend assemblies removed.");
                        }
                    }
                    catch (Exception ex)
                    {
                        var error = string.Format("Warning, Error fixing assembly, \r\nError: {0}\r\nFile: {1}", ex.ToString(), newSigned.FullName);

                        var message = new BuildWarningEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, error, "", "", DateTime.Now);
                        this.BuildEngine.LogWarningEvent(message);

                        Console.WriteLine(error);
                    }
                }

                topLeveDirectory = new ApplicationGeneratorBuildTasks.Directory();
                topLeveDirectory.Directories.AddRange(directories);

                Console.WriteLine("Copying non-binary directories and files");

                CopyDirectories(Path.GetDirectoryName(projectPath), targetBinaries, outputPath, topLeveDirectory);

                if (newScannerFile != null && newScannerFile.Exists)
                {
                    Console.WriteLine("Handling scanner file");

                    HandleScannerFile(newScannerFile, solutionDir, snkKeyPath);
                }

                foreach (var newSigned in newFilesToFix)
                {
                    LookForDialog();

                    try
                    {
                        Console.WriteLine("Certificate signing '{0}'...", newSigned.Name);

                        AppSigner.Sign(newSigned.FullName, certificate, snkKeyPath);
                    }
                    catch (Exception ex)
                    {
                        var error = string.Format("Warning, Error certificated signing assembly, \r\nError: {0}\r\nFile: {1}", ex.ToString(), newSigned.FullName);

                        var message = new BuildWarningEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, error, "", "", DateTime.Now);
                        this.BuildEngine.LogWarningEvent(message);

                        Console.WriteLine(error);
                    }
                }
            }
            catch (Exception ex)
            {
                var error = string.Format("Error signing assemblies. \r\nError: {0}", ex.ToString());

                var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, error, "", "", DateTime.Now);
                this.BuildEngine.LogErrorEvent(message);

                Console.WriteLine(error);

                return false;
            }

            return true;
        }

        private void LookForDialog()
        {
            Console.WriteLine("Looking for popup dialog");

            Task.Run(() =>
            {
                var foundWindow = false;
                var atttemptedDismissal = false;
                var dismissedWindow = false;

                while (!dismissedWindow)
                {
                    foundWindow = false;

                    while (!foundWindow)
                    {
                        var hwnd = ControlExtensions.GetWindows().SingleOrDefault(h => ControlExtensions.GetClassName(h) == "#32770" && ControlExtensions.GetWindowText(h) == "Signing data with your private exchange key");

                        if (hwnd != IntPtr.Zero)
                        {
                            var okayButton = ControlExtensions.GetChildWindows(hwnd).SingleOrDefault(h => ControlExtensions.GetClassName(h) == "Button" && ControlExtensions.GetWindowText(h) == "OK");

                            if (okayButton != IntPtr.Zero)
                            {
                                Console.WriteLine("Found popup dialog. Attempting to dismiss");

                                ControlExtensions.SetFocus(okayButton);
                                ControlExtensions.ClickWindow(okayButton);

                                ControlExtensions.TypeKey(okayButton, Keys.Enter);

                                foundWindow = true;
                                atttemptedDismissal = true;
                            }
                        }
                        else if (atttemptedDismissal)
                        {
                            foundWindow = true;
                            dismissedWindow = true;

                            Console.WriteLine("Popup dialog dismissed");

                            Thread.Sleep(1000);
                        }

                        Thread.Sleep(100);
                    }
                }
            });
        }

        public void HandleScannerFile(FileInfo newScannerFile, string solutionDir, string keyPath)
        {
            var handlerExe = Path.Combine(solutionDir, @"Binaries\SolutionLibraries\RunDllExport\RunDllExport.exe");
            var handlerExeFile = new FileInfo(handlerExe);
            var dllExportCommandHandler = new DllExportCommandHandler(handlerExe);
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            var stopwatch = new Stopwatch();
            var signedDirectory = newScannerFile.Directory;
            var tempDirectory = new DirectoryInfo(Path.Combine(Path.GetPathRoot(solutionDir), "DllExport"));
            var x86Directory = new DirectoryInfo(Path.Combine(signedDirectory.FullName, "x86"));
            StreamWriter writer;
            StreamReader reader;
            CommandPacket commandPacketReturn;
            CriticalBuildMessageEventArgs messageEventArgs;
            string json;
            string json64;
            object jsonObject;
            string error;
            string message;
            string connectResponse;

            if (tempDirectory.Exists)
            {
                tempDirectory.ForceDeleteAllFilesAndSubFolders();
            }
            else
            {
                tempDirectory.Create();
            }

            if (!x86Directory.Exists)
            {
                var securityRules = new DirectorySecurity();

                securityRules.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.Modify, AccessControlType.Allow));
                securityRules.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.ExecuteFile, AccessControlType.Allow));

                x86Directory.Create(securityRules);
            }

            DebugUtils.AssertThrow(handlerExeFile.Exists, $"DllExportHandler does not exist, path: '{ handlerExe }'");

            dllExportCommandHandler.OutputWriteLine = (format, args) =>
            {
                outputBuilder.AppendLineFormat(format, args);
                Console.WriteLine("\t" + string.Format(format, args));
            };

            dllExportCommandHandler.ErrorWriteLine = (format, args) =>
            {
                errorBuilder.AppendLineFormat(format, args);
                Console.WriteLine("\t" + string.Format(format, args));
            };

            stopwatch.Start();

            dllExportCommandHandler.RunForStdio(handlerExeFile.DirectoryName, "-waitForInput");

            DebugUtils.AssertThrow(errorBuilder.ToString() == string.Empty, errorBuilder.ToString());
            DebugUtils.AssertThrow(!dllExportCommandHandler.Process.HasExited, () => $"Unexpected exit code for DllExport: { dllExportCommandHandler.Process.ExitCode.ToHexString(true) }");

            writer = dllExportCommandHandler.Writer;
            reader = dllExportCommandHandler.Reader;

            writer.WriteJsonCommand(new CommandPacket
            {
                Command = ServerCommands.CONNECT
            });

            commandPacketReturn = reader.ReadJsonCommand();
            connectResponse = commandPacketReturn.Response.ToString().Trim();

            Console.WriteLine(connectResponse);

            DebugUtils.AssertThrow(connectResponse.GetLastLine().Trim() == "Connected successfully", $"Connection to DllExport assembly failed with { commandPacketReturn.Response }");

            jsonObject = new
            {
                Platform = "AnyCPU",
                EmitDebugSymbols = true,
                LeaveIntermediateFiles = false,
                Timeout = 1000,
                ProjectPath =  Path.Combine(solutionDir, @"Hydra.Scanner\Hydra.Scanner.csproj"),
                InputFileName = newScannerFile.FullName,
                TestTempDirectory = tempDirectory.FullName,
                Files = "VkYp3s6v",
                KeyFile = keyPath,
                PrivateKey = "KaPdSgUkXp2s5v8y/B?E(H+MbQeThWmY"
            };

            json = JsonExtensions.ToJsonText(jsonObject);
            json64 = json.ToBase64();

            writer.WriteJsonCommand(new CommandPacket
            {
                Command = ServerCommands.EXECUTE,
                Arguments = new KeyValuePair<string, object>[]
                {
                    new KeyValuePair<string, object>("PropertiesJson", json64)
                }
            });

            commandPacketReturn = reader.ReadJsonCommand();
            DebugUtils.AssertThrow(commandPacketReturn.Response.ToString() == "Complete", $"Response from DllExport assembly failed with { commandPacketReturn.Response }");

            message = ((string)commandPacketReturn.Arguments.Single(a => a.Key == "Output").Value).FromBase64ToString();
            message += "\r\n" + ((string)commandPacketReturn.Arguments.Single(a => a.Key == "Warning").Value).FromBase64ToString();

            messageEventArgs = new CriticalBuildMessageEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, message, "", "", DateTime.Now);
            this.BuildEngine.LogMessageEvent(messageEventArgs);

            error = (string)commandPacketReturn.Arguments.Single(a => a.Key == "Error").Value;

            DebugUtils.AssertThrow(error.IsNullOrEmpty(), error.FromBase64ToString());

            writer.WriteJsonCommand(new CommandPacket
            {
                Command = ServerCommands.TERMINATE
            });

            while (!dllExportCommandHandler.Process.WaitForExit(1))
            {
                Thread.Sleep(100);

                if (stopwatch.ElapsedMilliseconds > 10000)
                {
                    throw new TimeoutException("Timeout waiting for DllExport process to complete");
                }
            }

            DebugUtils.AssertThrow(dllExportCommandHandler.Process.ExitCode == 0, $"Unexpected exit code for DllExport: { dllExportCommandHandler.Process.ExitCode.ToHexString(true) }");

            stopwatch.Stop();

            x86Directory.Refresh();

            DebugUtils.AssertThrow(x86Directory.Exists, $"DllExport x86 output directory not created");

            x86Directory.CopyTo(signedDirectory.FullName, true);
        }

        private void CopyDirectories(string projectLocation, string targetBinariesPath, string outputPath, ApplicationGeneratorBuildTasks.Directory topLeveDirectory)
        {
            var files = new List<FileInfo>();
            var directory = new DirectoryInfo(targetBinariesPath);

            files.AddRange(directory.GetFiles());

            topLeveDirectory.GetDescendants<ApplicationGeneratorBuildTasks.Directory>(d => d.Directories, (d) =>
            {
                if (d.Name == "Signed")
                {
                    return false;
                }

                foreach (var component in d.Files)
                {
                    var relativePath = component.File.Source;
                    var fullPath = Path.GetFullPath(Path.Combine(projectLocation, relativePath));
                    var copyFrom = new FileInfo(fullPath);

                    files.Add(copyFrom);
                }

                return true;
            });

            files = files.Where(f => !f.Extension.IsOneOf(".dll", ".exe", ".pdb")).ToList();

            foreach (var file in files)
            {
                var relativePath = file.FullName.RemoveStart(targetBinariesPath);
                var copyTo = new FileInfo(Path.Combine(outputPath, relativePath.RemoveStartIfMatches(@"\")));

                if (!copyTo.Directory.Exists)
                {
                    copyTo.Directory.Create();
                }

                file.CopyTo(copyTo.FullName);
            }
        }
    }
}
