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

namespace BuildTasks
{
    public class UpdateHydraInstaller : ITask
    {
        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }
        [Required] 
        public string Configuration { get; set; }
        [Required]
        public string TargetFramework { get; set; }
        [Required]
        public string TargetAssembly { get; set; }

        public bool Execute()
        {
            try
            {
                if (Keys.ControlKey.IsPressed())
                {
                    if (MessageBox.Show("Debug?", "Debug UpdateHydraInstaller", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Debugger.Launch();
                    }
                }

                var solutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
                var productFilePath = Path.Combine(solutionPath, @"Hydra.InstallerStandalone\Product.wxs");
                var applicationGeneratorBinaries = Path.Combine(solutionPath, @"ApplicationGenerator\bin", this.Configuration);
                var applicationGeneratorProject = Path.Combine(solutionPath, @"ApplicationGenerator\ApplicationGenerator.csproj");
                var components = ComponentFinder.GetComponents(applicationGeneratorBinaries, applicationGeneratorProject, this.TargetFramework, this.TargetAssembly, productFilePath);
                var directories = ComponentFinder.GetDirectories(applicationGeneratorBinaries, applicationGeneratorProject, this.TargetFramework, this.TargetAssembly, productFilePath);
                var document = XDocument.Load(productFilePath);
                var namespaceManager = new XmlNamespaceManager(new NameTable());
                var programMenuXml = typeof(UpdateHydraInstaller).ReadResource<string>("ApplicationGeneratorBuildTasks.ProgramMenu.xml");
                XElement elementFeature;
                XElement elementDirectory;
                ApplicationGeneratorBuildTasks.Directory groupDirectory;
                ApplicationGeneratorBuildTasks.Directory topLeveDirectory;
                ComponentGroupRef componentGroupRef;
                XElement elementGroupDirectory;
                XElement elementComponentGroupRef;
                XElement elementProgramMenu;
                XElement elementProgramMenuDirectory;
                XElement elementApplicationShortcutComponentRef;
                XElement elementFragmentComponentGroup;
                ApplicationGeneratorBuildTasks.Directory programMenuDirectory;

                namespaceManager.AddNamespace("wi", "http://schemas.microsoft.com/wix/2006/wi");

                elementFeature = document.XPathSelectElement("/wi:Wix/wi:Product/wi:Feature", namespaceManager);
                elementDirectory = document.XPathSelectElement("/wi:Wix/wi:Fragment/wi:Directory", namespaceManager);

                elementFeature.Elements().Remove();
                elementDirectory.Elements().Remove();

                foreach (var component in components)
                {
                    var elementComponent = component.ToXElement<Component>();
                    var elementFile = component.File.ToXElement<ApplicationGeneratorBuildTasks.File>();
                    var elementComponentRef = component.CreateComponentRef().ToXElement<ComponentRef>();

                    elementComponent.Add(elementFile);

                    elementDirectory.Add(elementComponent);
                    elementFeature.Add(elementComponentRef);
                }

                groupDirectory = new ApplicationGeneratorBuildTasks.Directory(this.TargetAssembly + ".Binaries");
                componentGroupRef = groupDirectory.CreateComponentGroupRef();
                elementGroupDirectory = groupDirectory.ToXElement<ApplicationGeneratorBuildTasks.Directory>();
                elementComponentGroupRef = componentGroupRef.ToXElement<ComponentGroupRef>();

                elementDirectory.Add(elementGroupDirectory);
                elementFeature.Add(elementComponentGroupRef);

                topLeveDirectory = new ApplicationGeneratorBuildTasks.Directory();
                topLeveDirectory.Directories.AddRange(directories);

                AddDirectories(elementFeature, elementDirectory, topLeveDirectory);

                // add start menu elements

                elementProgramMenuDirectory = elementDirectory.XPathSelectElement("wi:Directory[@Id='ProgramMenuFolder']", namespaceManager);

                if (elementProgramMenuDirectory != null)
                {
                    elementProgramMenuDirectory.Remove();
                }

                programMenuDirectory = new ApplicationGeneratorBuildTasks.Directory("ProgramMenuFolder") { Directories = new List<ApplicationGeneratorBuildTasks.Directory> { new ApplicationGeneratorBuildTasks.Directory("HydraShortcuts", "CloudIDEaaS Hydra") } };
                elementProgramMenuDirectory = programMenuDirectory.ToXElement<ApplicationGeneratorBuildTasks.Directory>();

                foreach (var shortcutDirectory in programMenuDirectory.Directories)
                {
                    elementProgramMenuDirectory.Add(shortcutDirectory.ToXElement<ApplicationGeneratorBuildTasks.Directory>());
                }

                elementDirectory.Add(elementProgramMenuDirectory);

                elementApplicationShortcutComponentRef = elementFeature.XPathSelectElement("wi:ComponentRef[@Id='ApplicationShortcut']", namespaceManager);

                if (elementApplicationShortcutComponentRef != null)
                {
                    elementApplicationShortcutComponentRef.Remove();
                }

                elementApplicationShortcutComponentRef = new ComponentRef("ApplicationShortcut").ToXElement<ComponentRef>();

                elementFeature.Add(new ComponentRef("ApplicationShortcut").ToXElement<ComponentRef>());

                elementFragmentComponentGroup = document.Root.XPathSelectElement("wi:Fragment/wi:ComponentGroup[@Id='ProductComponents']", namespaceManager);

                if (elementFragmentComponentGroup != null)
                {
                    elementFragmentComponentGroup.Remove();
                }

                elementProgramMenu = XElement.Parse(programMenuXml);
                document.Root.Add(elementProgramMenu);

                document.Save(productFilePath, SaveOptions.DisableFormatting);
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

        private void AddDirectories(XElement elementFeature, XElement elementTopDirectory, ApplicationGeneratorBuildTasks.Directory topLeveDirectory)
        {
            topLeveDirectory.GetDescendants<ApplicationGeneratorBuildTasks.Directory, XElement>(d => d.Directories, (d, p) =>
            {
                XElement elementParentDirectory = p;
                ApplicationGeneratorBuildTasks.Directory directory = d;
                var elementDirectory = directory.ToXElement<ApplicationGeneratorBuildTasks.Directory>();

                if (elementParentDirectory == null)
                {
                    elementParentDirectory = elementTopDirectory;
                }

                elementParentDirectory.Add(elementDirectory);

                foreach (var component in d.Files)
                {
                    var elementComponent = component.ToXElement<Component>();
                    var elementFile = component.File.ToXElement<ApplicationGeneratorBuildTasks.File>();
                    var elementComponentRef = component.CreateComponentRef().ToXElement<ComponentRef>();

                    elementComponent.Add(elementFile);

                    elementDirectory.Add(elementComponent);
                    elementFeature.Add(elementComponentRef);
                }

                return elementDirectory;
            });
        }
    }
}
