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
using Debugger = System.Diagnostics.Debugger;
using Microsoft.VisualStudio.ExtensionManager;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO.Packaging;
using Utils;

namespace BuildTasks
{
    public class InstallTemplates : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }
        [Required]
        public string Configuration { get; set; }

        public bool Execute()
        {
            try
            {
                if (Keys.ControlKey.IsPressed())
                {
                    if (MessageBox.Show("Debug?", "Debug InstallTemplates", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Debugger.Launch();
                    }
                }

                var solutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
                var itemTemplatesOutputPath = Path.Combine(solutionPath, @"ApplicationGenerator.IonicAngular\ExportedItemTemplates");
                var projectTemplatesOutputPath = Path.Combine(solutionPath, @"ApplicationGenerator.IonicAngular\ExportedProjectTemplates");
                var vsTemplatesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Visual Studio 2019\Templates\ItemTemplates\Hydra");
                var projectDirectory = Path.GetDirectoryName(this.BuildEngine.ProjectFileOfTaskNode);
                var localTemplatesPath = Path.Combine(projectDirectory, @"bin\" + this.Configuration + @"\Templates");
                var directory = new DirectoryInfo(itemTemplatesOutputPath);
                var parentProcess = Process.GetCurrentProcess().GetParent();

                if (parentProcess.ProcessName.IsOneOf("devenv", "msvsmon"))
                {
                    if (!Directory.Exists(vsTemplatesPath))
                    {
                        Directory.CreateDirectory(vsTemplatesPath);
                    }

                    // Visual Studio templates

                    foreach (var folder in directory.GetDirectories())
                    {
                        using (var package = ZipPackage.Open(Path.Combine(vsTemplatesPath, folder.Name + ".zip"), FileMode.Create))
                        {
                            foreach (var file in folder.GetFiles())
                            {
                                var part = package.CreatePart(new Uri("/" + file.Name, UriKind.Relative), "");

                                using (var stream = new MemoryStream(File.ReadAllBytes(file.FullName)))
                                {
                                    part.GetStream().WriteAll(stream);
                                }
                            }
                        }
                    }
                }


                // Locally distributed templates

                directory = new DirectoryInfo(projectTemplatesOutputPath);

                if (!Directory.Exists(localTemplatesPath))
                {
                    Directory.CreateDirectory(localTemplatesPath);
                }

                foreach (var folder in directory.GetDirectories())
                {
                    using (var package = ZipPackage.Open(Path.Combine(localTemplatesPath, folder.Name + ".zip"), FileMode.Create))
                    {
                        var subFolders = folder.GetDirectories("*", SearchOption.AllDirectories);

                        foreach (var file in folder.GetFiles())
                        {
                            var part = package.CreatePart(new Uri("/" + file.Name, UriKind.Relative), "");

                            using (var stream = new MemoryStream(File.ReadAllBytes(file.FullName)))
                            {
                                part.GetStream().WriteAll(stream);
                            }
                        }

                        foreach (var subFolder in subFolders)
                        {
                            foreach (var file in subFolder.GetFiles())
                            {
                                var relativePath = file.FullName.RemoveStart(folder.FullName).ReverseSlashes();
                                var part = package.CreatePart(new Uri(relativePath, UriKind.Relative), "");

                                using (var stream = new MemoryStream(File.ReadAllBytes(file.FullName)))
                                {
                                    part.GetStream().WriteAll(stream);
                                }
                            }
                        }
                    }
                }

                foreach (var file in directory.GetFiles())
                {
                    File.Copy(file.FullName, Path.Combine(localTemplatesPath, file.Name), true);
                }

            }
            catch (Exception ex)
            {
                var error = string.Format("Error installing Hydra Visual Studio templates. \r\nError: {0}", ex.ToString());

                var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, error, "", "", DateTime.Now);
                this.BuildEngine.LogErrorEvent(message);

                Console.WriteLine(error);

                return false;
            }

            return true;
        }
    }
}
