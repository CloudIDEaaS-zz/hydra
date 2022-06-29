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
using System.Xml.Linq;
using ApplicationGeneratorBuildTasks;
using Utils.Hierarchies;
using AbstraX.Handlers.CommandHandlers;

namespace BuildTasks
{
    public class UpdateHydraNuget : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }
        [Required] 
        public string Configuration { get; set; }
        [Required]
        public string TargetFramework { get; set; }

        public bool Execute()
        {
            try
            {
                if (Keys.ControlKey.IsPressed())
                {
                    if (MessageBox.Show("Debug?", "Debug HydraNuget", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Debugger.Launch();
                    }
                }

                var solutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
                var hydraInterfacesProjectFile = Path.Combine(solutionPath, @"Hydra.Interfaces\Hydra.Interfaces.csproj");
                var hydraInterfacesProjectFolder = Path.Combine(solutionPath, @"Hydra.Interfaces");
                var hydraInterfacesBinaries = Path.Combine(solutionPath, @"Hydra.Interfaces\bin", this.Configuration);
                var utilsBinaries = Path.Combine(solutionPath, @"Utils.Core\bin", this.Configuration);
                var nuspecFile = Path.Combine(solutionPath, @"Hydra.Interfaces\Hydra.Interfaces.nuspec");
                var files = ComponentFinder.GetFiles(hydraInterfacesBinaries, this.TargetFramework);
                var utilsFiles = ComponentFinder.GetFiles(utilsBinaries, this.TargetFramework);
                var nugspecDocument = XDocument.Load(nuspecFile);
                var projectDocument = XDocument.Load(hydraInterfacesProjectFile);
                var namespaceManager = new XmlNamespaceManager(new NameTable());
                XElement elementVersion;
                XElement elementFiles;
                string hydraPackagePng;
                string hydraEULATxt;
                string targetVersion;
                NugetCommandHandler nugetCommandHandler;

                files.AddRange(utilsFiles.Where(f => !files.Any(f2 => f2.target == f.target)));

                namespaceManager.AddNamespace("nu", "http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd");
                namespaceManager.AddNamespace("nu", "http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd");

                elementVersion = nugspecDocument.XPathSelectElement("/nu:package/nu:metadata/nu:version", namespaceManager);
                elementFiles = nugspecDocument.XPathSelectElement("/nu:package/nu:files", namespaceManager);

                targetVersion = projectDocument.XPathSelectElement("/Project/PropertyGroup/AssemblyVersion").Value;
                elementVersion.Value = targetVersion;

                elementFiles.Elements().Remove();

                hydraPackagePng = Path.Combine(solutionPath, @"ApplicationGenerator.Interfaces\images\HydraPackage.png");
                hydraEULATxt = Path.Combine(solutionPath, @"Hydra.InstallerStandalone\Hydra - EULA.txt");

                files.Add(new file(new FileInfo(hydraPackagePng), Path.GetDirectoryName(hydraPackagePng)));
                files.Add(new file(new FileInfo(hydraEULATxt), Path.GetDirectoryName(hydraEULATxt)));

                AddFiles(elementFiles, files);

                using (var writer = new XmlTextWriter(nuspecFile, null))
                {
                    writer.Formatting = Formatting.Indented;
                    nugspecDocument.Save(writer);
                }

                nugetCommandHandler = new NugetCommandHandler();
                nugetCommandHandler.Pack(hydraInterfacesProjectFolder, "Hydra.Interfaces.nuspec");
            }
            catch (Exception ex)
            {
                var error = string.Format("Error updating Hydra Nuget. \r\nError: {0}", ex.ToString());

                var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, error, "", "", DateTime.Now);
                this.BuildEngine.LogErrorEvent(message);

                Console.WriteLine(error);

                return false;
            }

            return true;
        }

        private void AddFiles(XElement elementFiles, List<file> files)
        {
            foreach (var file in files)
            {
                elementFiles.Add(file.ToXElement<file>());
            }
        }
    }
}
