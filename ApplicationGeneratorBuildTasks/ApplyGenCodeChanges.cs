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
using System.Text.RegularExpressions;
using VisualStudioProvider;
using System.Reflection;

namespace BuildTasks
{
    public class ApplyGenCodeChanges : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }
        private Task task;

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);

        public ApplyGenCodeChanges()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);
        }

        private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var projectFile = new FileInfo(this.BuildEngine.ProjectFileOfTaskNode);
            var name = args.Name.Substring(0, args.Name.IndexOf(','));
            var assemblyPath = Path.Combine(projectFile.Directory.Parent.FullName, string.Format(@"Binaries\SolutionLibraries\{0}.dll", name));
            var assemblyFile = new FileInfo(assemblyPath);

            if (assemblyFile.Exists)
            {
                var assembly = Assembly.LoadFrom(assemblyPath);

                return assembly;
            }

            return null;
        }

        public bool Execute()
        {
            try
            {
                if (Regex.IsMatch(Environment.CommandLine, ".*?msbuild.exe\" "))
                {
                    return true;
                }

                var dte = GetDTE();
                var dte2 = (EnvDTE80.DTE2)dte;
                var projectFile = new FileInfo(this.BuildEngine.ProjectFileOfTaskNode);
                var solutionPath = projectFile.Directory.Parent.FullName;
                var warnings = new List<string>();

                //if (task == null)
                //{
                //    task = new Task(() =>
                //    {
                //        StartWatch(solutionPath);
                //    });

                //    task.Start();
                //}
                //else
                //{
                //    task.Dispose();
                //}

                VSSolution.OnParserErrorRaw += (sender, ex) =>
                {
                    warnings.Add(ex.Message);
                };

                var solution = new VSSolution(Path.Combine(solutionPath, "Hydra.sln"));

                foreach (Document document in dte.Documents)
                {
                    var documentName = document.Name;

                    if (documentName.EndsWith(".cs"))
                    {
                        var fileInfo = new FileInfo(Path.Combine(document.Path, documentName));
                        var parts = fileInfo.DirectoryName.Split('\\');

                        if (parts.Contains("Generated_Code"))
                        {
                            if ((fileInfo.Attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly)
                            {
                                var selection = document.Selection;

                                if (selection != null)
                                {
                                    var windowText = string.Empty;
                                    var generatedFileText = string.Empty;
                                    var realFileText = string.Empty;

                                    selection.SelectAll();
                                    windowText = selection.Text;

                                    using (var reader = fileInfo.OpenText())
                                    {
                                        generatedFileText = reader.ReadToEnd();
                                    }

                                    if (windowText == generatedFileText && (DateTime.Now - fileInfo.LastWriteTime < new TimeSpan(0, 0, 20)))
                                    {
                                        var projectDocuments = solution.Projects.SelectMany(p => p.Items).Cast<VSProjectItem>().Where(i => i.Name == documentName);

                                        if (projectDocuments.Count() == 1)
                                        {
                                            var projectDocument = projectDocuments.Single();
                                            var documentFile = new FileInfo(projectDocument.FilePath);

                                            using (var reader = documentFile.OpenText())
                                            {
                                                realFileText = reader.ReadToEnd();
                                            }

                                            if (generatedFileText != realFileText)
                                            {
                                                var message = string.Format("The auto-generated file '{0}' does not match the file in project '{1}'.  Would you like to update?", documentName, projectDocument.ParentProject.Name);

                                                if (MessageBox.Show(message, "Auto-Generated Code Detection", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                                {
                                                    //try
                                                    //{
                                                    //    //var workspaceInfo = Workstation.Current.GetLocalWorkspaceInfo(documentFile.FullName);
                                                    //    //var server = TfsConfigurationServerFactory.GetConfigurationServer(workspaceInfo.ServerUri, new UICredentialsProvider());

                                                    //    //server.EnsureAuthenticated();

                                                    //    //var workspace = workspaceInfo.GetWorkspace(server.GetTeamProjectCollection(workspaceInfo.ServerGuid));

                                                    //    //workspace.PendEdit(documentFile.FullName);                 
                                                    //}
                                                    //catch (Exception ex)
                                                    //{
                                                    //    Console.WriteLine(string.Format("Error checking out file: '{0}'", ex));
                                                    //    MessageBox.Show(string.Format("Error checking out file: '{0}'", ex));

                                                    //    BuildEngine.LogWarningEvent(new BuildWarningEventArgs("", "", "", 0, 0, 0, 0, string.Format("Error checking out file: '{0}'", ex), "", ""));
                                                    //}

                                                    if ((documentFile.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                                                    {
                                                        documentFile.Attributes &= (~FileAttributes.ReadOnly);
                                                    }

                                                    using (var stream = documentFile.OpenWrite())
                                                    {
                                                        var textWriter = new StreamWriter(stream);

                                                        stream.SetLength(0);

                                                        textWriter.Write(generatedFileText);
                                                        textWriter.Flush();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (var warning in warnings)
                {
                    this.BuildEngine.LogWarningEvent(new BuildWarningEventArgs("", "", "", 0, 0, 0, 0, warning, "", ""));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error applying generated code runtime changes: '{0}'", ex));
                MessageBox.Show(string.Format("Error applying generated code runtime changes: '{0}'", ex));

                BuildEngine.LogWarningEvent(new BuildWarningEventArgs("", "", "", 0, 0, 0, 0, string.Format("Error applying generated code runtime changes: '{0}'", ex), "", ""));

                return false;
            }

            return true;
        }

        private void StartWatch(string solutionPath)
        {
            var watcher = new FileSystemWatcher(solutionPath);
            var webGenFileName = "Hydra.Web.g.cs";
            var webGenFile = new FileInfo(Path.Combine(solutionPath, @"AutoGenBackup\" + webGenFileName));
            var stopWatch = new Stopwatch();
            var error = true;

            stopWatch.Start();

            Debug.WriteLine("Watching");

            Debug.Assert(webGenFile.Exists);

            watcher.IncludeSubdirectories = true;
            watcher.Filter = webGenFileName;
            watcher.Path = solutionPath;

            watcher.Created += (sender, e) =>
            {
                if (e.FullPath.EndsWith(webGenFileName))
                {
                    var fileInfo = new FileInfo(e.FullPath);

                    Debug.WriteLine(e.FullPath + " created!");

                    if (fileInfo.Length < (webGenFile.Length / 2))
                    {
                        error = true;

                        Debug.WriteLine(e.FullPath + " detected to be truncated!");

                        while (error)
                        {
                            try
                            {
                                File.Copy(webGenFile.FullName, fileInfo.FullName + ".cs", true);
                                Debug.WriteLine("File released.. copied");

                                error = false;
                            }
                            catch
                            {
                                error = true;
                                Thread.Sleep(1);
                            }
                        }
                    }
                }
            };

            watcher.Changed += (sender, e) =>
            {
                if (e.FullPath.EndsWith(webGenFileName))
                {
                    var fileInfo = new FileInfo(e.FullPath);

                    Debug.WriteLine(e.FullPath + " changed!");

                    if (fileInfo.Length < (webGenFile.Length / 2))
                    {
                        error = true;

                        Debug.WriteLine(e.FullPath + " detected to be truncated!");

                        while (error)
                        {
                            try
                            {
                                File.Copy(webGenFile.FullName, fileInfo.FullName + ".cs", true);
                                Debug.WriteLine("File released.. copied");

                                error = false;
                            }
                            catch
                            {
                                error = true;
                                Thread.Sleep(1);
                            }
                        }
                    }
                }
            };

            watcher.EnableRaisingEvents = true;

            while (true)
            {
                Thread.Sleep(1);
            }
        }

        public DTE GetDTE()
        {
            int processId = 0;

            foreach (var process in System.Diagnostics.Process.GetProcesses())
            {
                if (process.ProcessName.ToLower() == "devenv")
                {
                    if (process.MainWindowTitle.StartsWith("Hydra"))
                    {
                        processId = process.Id;
                    }
                }
            }

            if (processId != 0)
            {
                string progId = "!VisualStudio.DTE.10.0:" + processId.ToString();
                object runningObject = null;

                IBindCtx bindCtx = null;
                IRunningObjectTable rot = null;
                IEnumMoniker enumMonikers = null;

                try
                {
                    Marshal.ThrowExceptionForHR(CreateBindCtx(reserved: 0, ppbc: out bindCtx));
                    bindCtx.GetRunningObjectTable(out rot);
                    rot.EnumRunning(out enumMonikers);

                    IMoniker[] moniker = new IMoniker[1];
                    IntPtr numberFetched = IntPtr.Zero;
                    while (enumMonikers.Next(1, moniker, numberFetched) == 0)
                    {
                        IMoniker runningObjectMoniker = moniker[0];

                        string name = null;

                        try
                        {
                            if (runningObjectMoniker != null)
                            {
                                runningObjectMoniker.GetDisplayName(bindCtx, null, out name);
                            }
                        }
                        catch (UnauthorizedAccessException)
                        {
                            // Do nothing, there is something in the ROT that we do not have access to.
                        }

                        if (!string.IsNullOrEmpty(name) && string.Equals(name, progId, StringComparison.Ordinal))
                        {
                            Marshal.ThrowExceptionForHR(rot.GetObject(runningObjectMoniker, out runningObject));
                            break;
                        }
                    }
                }
                finally
                {
                    if (enumMonikers != null)
                    {
                        Marshal.ReleaseComObject(enumMonikers);
                    }

                    if (rot != null)
                    {
                        Marshal.ReleaseComObject(rot);
                    }

                    if (bindCtx != null)
                    {
                        Marshal.ReleaseComObject(bindCtx);
                    }
                }

                return (DTE)runningObject;
            }
            else
            {
                return null;
            }
        }
    }
}
