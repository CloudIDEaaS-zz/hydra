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
using EnvDTE;
using Process = System.Diagnostics.Process;
using ServiceProvider = Microsoft.VisualStudio.Shell.ServiceProvider;
using Debugger = System.Diagnostics.Debugger;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using System.Text.RegularExpressions;
using System.IO.Packaging;
using System.Windows.Forms;
using Utils;

namespace BuildTasks
{
    public class RegisterPackages : ITask
    {
        private string installerPath;
        private IVsExtensionManager extensionManager;
        private string solutionPath;

        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        public RegisterPackages()
        {
            var guidService = typeof(SVsExtensionManager).GUID;

            solutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));

            extensionManager = (IVsExtensionManager)ServiceProvider.GlobalProvider.GetService(guidService);

            installerPath = Path.Combine(solutionPath, @"Binaries\VSIXInstaller\VSIXInstaller.exe");

            Debug.Assert(new FileInfo(installerPath).Exists);

            var fileInfo = new FileInfo(installerPath);

            Debug.Assert(fileInfo.Exists);
        }

        public bool Execute()
        {
            try
            {
                if (Regex.IsMatch(Environment.CommandLine, ".*?msbuild.exe\" "))
                {
                    return true;
                }

                var packages = new Dictionary<string, string>()
                {
                    {"HydraPackage.e4f2acc8-08f0-4b27-8421-e2b85a04956b", @"CodeGenerationPipeline\VSIX\HydraPackageTemplate\HydraPackageTemplate.vsix"},
                    {"HydraSolution.8e12e46c-4214-42a0-b0a2-3afd02d36ec3", @"CodeGenerationPipeline\VSIX\HydraSolutionTemplate\HydraSolutionTemplate.vsix"},
                };

                foreach (var package in packages)
                {
                    var projectFile = new FileInfo(this.BuildEngine.ProjectFileOfTaskNode);
                    var packagePath = Path.Combine(projectFile.Directory.Parent.FullName, package.Value);

                    RegisterPackage(package.Key, packagePath);
                }
            }
            catch (Exception ex)
            {
                var error = string.Format("Error registering packages. You can run Visual Studio as Administrator to install this component.", ex);

                var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, error, "", "", DateTime.Now);
                this.BuildEngine.LogErrorEvent(message);

                Console.WriteLine(error);
                return false;
            }

            return true;
        }

        private void RestartExplorer()
        {
            var command = "/C taskkill /f /im explorer.exe";
            var commandProcess = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = command;
            commandProcess.StartInfo = startInfo;

            commandProcess.Start();
            commandProcess.WaitForExit();

            var windows = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

            command = string.Format("/C start {0}", Path.Combine(windows, "explorer.exe"));
            commandProcess = new Process();
            startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = command;
            commandProcess.StartInfo = startInfo;

            commandProcess.Start();
            commandProcess.WaitForExit();

            System.Threading.Thread.Sleep(1000);
        }

        public void RegisterPackage(string packageIdentifier, string packagePath)
        {
            var fileInfo = new FileInfo(packagePath);
            var outdated = false;
            var installedExtension = (IInstalledExtension)null;

            if (!fileInfo.Exists)
            {
                var openFileDialog = new OpenFileDialog();
                var ext = Path.GetExtension(packagePath).Remove(0, 1);
                var window = new WindowWrapper(Process.GetCurrentProcess().MainWindowHandle);

                openFileDialog.CheckFileExists = true;
                openFileDialog.Filter = string.Format("Package files (*.{0})|*.{0}", ext);
                openFileDialog.DefaultExt = ext;
                openFileDialog.InitialDirectory = Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%");
                openFileDialog.Title = string.Format("Select file for '{0}'", packagePath);

                if (openFileDialog.ShowDialog(window) != DialogResult.OK)
                {
                    var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, string.Format("Error finding package '{0}' at '{1}'", packageIdentifier, packagePath) + ". Please select file from bin folder of Package Project or get latest from TFS.", "", "", DateTime.Now);
                    this.BuildEngine.LogErrorEvent(message);

                    return;
                }
                else
                {
                    File.Copy(openFileDialog.FileName, packagePath);
                }
            }

            fileInfo = new FileInfo(packagePath);

            if (!fileInfo.Exists)
            {
                var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, string.Format("Error finding package '{0}' at '{1}'", packageIdentifier, packagePath) + ". Please select file from bin folder of Package Project or get latest from TFS.", "", "", DateTime.Now);
                this.BuildEngine.LogErrorEvent(message);

                return;
            }

            if (fileInfo.Extension == ".dll")
            {
                // is a dll, use RegPkg

                var projectFile = new FileInfo(this.BuildEngine.ProjectFileOfTaskNode);
                var regPkgPath = Path.Combine(solutionPath, @"Binaries\SolutionLibraries\RegPkg.exe");

                var process = new Process();
                var startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = regPkgPath,
                    Arguments = "/unregister" + " \"" + fileInfo.FullName + "\""
                };

                process.StartInfo = startInfo;
                process.Start();

                var result = process.StandardOutput.ReadToEnd();
                result += "\r\n" + process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, string.Format("Error unregistering package '{0}'. ", packageIdentifier) + result + " Exit code = " + process.ExitCode.ToString() + ". You can try running Visual Studio as Administrator to uninstall this component.", "", "", DateTime.Now);
                    this.BuildEngine.LogErrorEvent(message);
                }
                else
                {
                    var message = new BuildMessageEventArgs(string.Format("'{0}' package unregistered successfully.  ", packageIdentifier) + result, "", "", MessageImportance.High);   
                    this.BuildEngine.LogMessageEvent(message);
                }

                process = new Process();
                startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    FileName = regPkgPath,
                    Arguments = "/root:Software\\Microsoft\\VisualStudio\\10.0 \"" + packagePath + "\""
                };

                process.StartInfo = startInfo;
                process.Start();

                result = process.StandardOutput.ReadToEnd();
                result += "\r\n" + process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, string.Format("Error registering package '{0}'. ", packageIdentifier) + result + " Exit code = " + process.ExitCode.ToString() + ". You can try running Visual Studio as Administrator to install this component.", "", "", DateTime.Now);
                    this.BuildEngine.LogErrorEvent(message);

                    return;
                }
                else
                {
                    var message = new BuildMessageEventArgs(string.Format("'{0}' package registered successfully.  ", packageIdentifier) + result, "", "", MessageImportance.High);
                    this.BuildEngine.LogMessageEvent(message);
                }
            }
            else
            {
                // is vsx

                Action<DateTime, DirectoryInfo> checkOutdated = null;

                checkOutdated = new Action<DateTime, DirectoryInfo>((date, directory) =>
                {
                    foreach (var file in directory.GetFiles())
                    {
                        if (file.Extension != ".vsix" && file.Extension != ".zip")
                        {
                            if (DateTime.Compare(file.LastWriteTime, date) > 0)
                            {
                                outdated = true;
                                return;
                            }
                        }
                    }

                    foreach (var dir in directory.GetDirectories())
                    {
                        checkOutdated(date, dir);
                    }
                });

                // lets do another workaround due to Microsoft stupidity 

                //var extensionPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\VisualStudio\10.0\Extensions\Cloud IDEaaS");
                //var folder = new DirectoryInfo(extensionPath);

                //Action<DirectoryInfo> deleteDeleteMeFiles = null;

                //deleteDeleteMeFiles = new Action<DirectoryInfo>(directory =>
                //{
                //    foreach (var file in directory.GetFiles("*.deleteme"))
                //    {
                //        try
                //        {
                //            file.Delete();
                //        }
                //        catch (Exception ex)
                //        {
                //            try
                //            {
                //                RestartExplorer();
                //                file.Delete();
                //            }
                //            catch
                //            {
                //                try
                //                {
                //                    var hydraCopyPath = Path.Combine(solutionPath, @"Binaries\SolutionLibraries\HydraCopy.exe");

                //                    var process = new Process();
                //                    var startInfo = new ProcessStartInfo
                //                    {
                //                        UseShellExecute = false,
                //                        RedirectStandardError = true,
                //                        RedirectStandardOutput = true,
                //                        CreateNoWindow = true,
                //                        FileName = hydraCopyPath,
                //                        Arguments = "\"" + file.FullName + "\" /delete"
                //                    };

                //                    process.StartInfo = startInfo;
                //                    process.Start();

                //                    process.WaitForExit();

                //                    if (process.ExitCode != 0)
                //                    {
                //                        throw new Exception(string.Format("HydraCopy.exe exited with error code = '{0}'", process.ExitCode));
                //                    }
                //                }
                //                catch (Exception ex2)
                //                {
                //                    var message2 = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, string.Format("Error launching HydraCopy.exe to remove leftover files for package '{0}'. Error={1}. You can try rebuilding or restarting Visual Studio to remove these files.", packageIdentifier, ex2.ToString()), "", "", DateTime.Now);
                //                    this.BuildEngine.LogErrorEvent(message2);
                //                }

                //                var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, string.Format("Error removing leftover files for package '{0}'. Error={1}. You can try rebuilding or restarting Visual Studio to remove these files.", packageIdentifier, ex.ToString()), "", "", DateTime.Now);
                //                this.BuildEngine.LogErrorEvent(message);
                //            }
                //        }
                //    }

                //    foreach (var dir in directory.GetDirectories())
                //    {
                //        deleteDeleteMeFiles(dir);
                //    }
                //});

                //if (folder.Exists)
                //{
                //    deleteDeleteMeFiles(folder);
                //}

                //if (Environment.Is64BitOperatingSystem)
                //{
                //    extensionPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Microsoft Visual Studio 10.0\Common7\IDE\Extensions\Cloud IDEaaS");
                //}
                //else
                //{
                //    extensionPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Microsoft Visual Studio 10.0\Common7\IDE\Extensions\Cloud IDEaaS");
                //}

                //folder = new DirectoryInfo(extensionPath);

                //if (folder.Exists)
                //{
                //    deleteDeleteMeFiles(folder);
                //}

                if (extensionManager.TryGetInstalledExtension(packageIdentifier, out installedExtension))
                {
                    checkOutdated(installedExtension.InstalledOn.DateTime, fileInfo.Directory);

                    if (!outdated)
                    {
                        return;
                    }
                }

                if (outdated)
                {
                    // see if there are templates in folder the VSIX installer will balk

                    fileInfo = new FileInfo(Path.Combine(fileInfo.DirectoryName, "extension.vsixmanifest"));

                    if (fileInfo.Exists)
                    {
                        var stream = File.OpenRead(fileInfo.FullName);
                        var installLocation = Path.Combine(Environment.ExpandEnvironmentVariables("%LocalAppData%"), @"Microsoft\VisualStudio\10.0\Extensions");
                        var _namespace = "vs";

                        stream.Seek(0, SeekOrigin.Begin);

                        var xPathDocument = new XPathDocument(stream);

                        stream.Close();

                        var nsmgr = new XmlNamespaceManager(new NameTable());
                        nsmgr.AddNamespace(_namespace, "http://schemas.microsoft.com/developer/vsx-schema/2010");

                        var navigator = xPathDocument.CreateNavigator();

                        var iter = navigator.Select(string.Format("{0}:Vsix/{0}:Identifier/{0}:Author", _namespace), nsmgr);
                        iter.MoveNext();

                        var author = iter.Current.Value;

                        iter = navigator.Select(string.Format("{0}:Vsix/{0}:Identifier/{0}:Name", _namespace), nsmgr);
                        iter.MoveNext();

                        var name = iter.Current.Value;

                        iter = navigator.Select(string.Format("{0}:Vsix/{0}:Identifier/{0}:Version", _namespace), nsmgr);
                        iter.MoveNext();

                        var version = iter.Current.Value;

                        installLocation = Path.Combine(installLocation, string.Format(@"{0}\{1}\{2}", author, name, version));

                        var dirInstall = new DirectoryInfo(installLocation);

                        if (dirInstall.Exists)
                        {
                            try
                            {
                                dirInstall.Delete(true);
                            }
                            catch
                            {
                                try
                                {
                                    RestartExplorer();
                                    dirInstall.Delete(true);
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                    }

                    try
                    {
                        if (extensionManager.IsInstalled(installedExtension))
                        {
                            extensionManager.Uninstall(installedExtension);
                        }

                        var message = new BuildMessageEventArgs(string.Format("'{0}' package unregistered successfully.  ", packageIdentifier), "", "", MessageImportance.High);
                        this.BuildEngine.LogMessageEvent(message);
                    }
                    catch (Exception ex)
                    {
                        var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, string.Format("Error unregistering package '{0}'. Error={1}", packageIdentifier, ex.ToString()), "", "", DateTime.Now);
                        this.BuildEngine.LogErrorEvent(message);
                    }
                }

                if (installedExtension == null || outdated)
                {
                    try
                    {
                        var debug = false;

                        if (debug)
                        {
                            var package = ZipPackage.Open(@"C:\Users\u164225\Documents\Visual Studio 2010\My Exported Templates\DummyProject.vsix");
                            var parts = package.GetParts();

                            package.Close();

                            package = ZipPackage.Open(packagePath);
                            parts = package.GetParts();

                            package.Close();

                            var installExtension2 = extensionManager.CreateInstallableExtension(@"C:\Users\u164225\Documents\Visual Studio 2010\My Exported Templates\DummyProject.vsix");
                            var reason2 = extensionManager.Install(installExtension2, false);
                            var message2 = new BuildMessageEventArgs(string.Format("'{0}' package registered successfully with restart reason={1}.  ", "DummyProject.fc1c3820-6d48-428d-963d-1f4bfa392f6b", reason2), "", "", MessageImportance.High);

                            Debugger.Break();
                        }

                        var installExtension = extensionManager.CreateInstallableExtension(packagePath);
                        var reason = extensionManager.Install(installExtension, false);
                        var message = new BuildMessageEventArgs(string.Format("'{0}' package registered successfully with restart reason={1}.  ", packageIdentifier, reason), "", "", MessageImportance.High);

                        this.BuildEngine.LogMessageEvent(message);
                    }
                    catch (Exception ex2)
                    {
                        try
                        {
                            RestartExplorer();

                            var installExtension = extensionManager.CreateInstallableExtension(packagePath);
                            var reason = extensionManager.Install(installExtension, false);
                            var message = new BuildMessageEventArgs(string.Format("'{0}' package registered successfully with restart reason={1}.  ", packageIdentifier, reason), "", "", MessageImportance.High);

                            this.BuildEngine.LogMessageEvent(message);
                        }
                        catch (Exception ex)
                        {
                            var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, string.Format("Error registering package '{0}'. Error={1}", packageIdentifier, ex.ToString()), "", "", DateTime.Now);
                            this.BuildEngine.LogErrorEvent(message);
                        }
                    }
                }
            }
        }
    }
}
