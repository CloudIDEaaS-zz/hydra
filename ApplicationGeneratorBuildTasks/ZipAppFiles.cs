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
using Utils.Hierarchies;

namespace BuildTasks
{
    public class ZipAppFiles : ITask
    {
        private string installerPath;
        private IVsExtensionManager extensionManager;
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        public bool Execute()
        {
            try
            {
                var solutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
                var projectPath = Path.Combine(solutionPath, @"ApplicationGenerator");
                var appFilePath = Path.Combine(projectPath, @"app");
                var directory = new DirectoryInfo(appFilePath);
                var parentProcess = Process.GetCurrentProcess().GetParent();

                if (!parentProcess.ProcessName.IsOneOf("devenv", "msvsmon"))
                {
                    return true;
                }

                using (var package = ZipPackage.Open(Path.Combine(projectPath, "app.zip"), FileMode.Create))
                {
                    directory.GetDescendantsAndSelf(d => d.GetDirectories(), d =>
                    {
                        foreach (var file in d.GetFiles())
                        {
                            var path = d.FullName.RemoveStartIfMatches(appFilePath).ForwardSlashes();
                            var part = package.CreatePart(new Uri(path + "/" + file.Name, UriKind.Relative), "");

                            using (var stream = new MemoryStream(File.ReadAllBytes(file.FullName)))
                            {
                                part.GetStream().WriteAll(stream);
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                var error = string.Format("Error zipping up Hydra applicaton files. \r\nError: {0}", ex.ToString());

                var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, error, "", "", DateTime.Now);
                this.BuildEngine.LogErrorEvent(message);

                Console.WriteLine(error);

                return false;
            }

            return true;
        }
    }
}
