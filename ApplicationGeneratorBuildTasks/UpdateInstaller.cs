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
    public class UpdateInstaller : ITask
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
        [Required]
        public bool ProdReleaseBuild { get; set; }

        public bool Execute()
        {
            try
            {
                if (Keys.ControlKey.IsPressed())
                {
                    if (MessageBox.Show("Debug?", "Debug UpdateInstaller", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Debugger.Launch();
                    }
                }

                var solutionDir = this.SolutionDir;
                var productFilePath = Path.Combine(solutionDir, this.ProductRelativePath);
                var targetBinariesPath = GetBinariesPath(this.TargetDir);
                var projectPath = this.ProjectPath;
                var appGenAppConfigPath = Path.Combine(solutionDir, @"ApplicationGenerator\App.config");
                var projectFileNameNoExt = Path.GetFileNameWithoutExtension(projectPath);
                var (components, packageFiles) = ComponentFinder.GetComponents(targetBinariesPath, projectPath, this.TargetFramework, this.TargetAssembly, productFilePath);
                var directories = ComponentFinder.GetDirectories(targetBinariesPath, projectPath, this.TargetFramework, this.TargetAssembly, productFilePath);
                var document = XDocument.Load(productFilePath);
                var appGenAppConfigDocument = XDocument.Load(appGenAppConfigPath);
                var namespaceManager = new XmlNamespaceManager(new NameTable());
                var programMenuXml = typeof(UpdateInstaller).ReadResource<string>($"ApplicationGeneratorBuildTasks.Resources.{ projectFileNameNoExt }ProgramMenu.xml");
                XElement elementFeature;
                XElement elementTargetDirectory;
                XElement elementUtilitiesDirectory;
                ApplicationGeneratorBuildTasks.Directory utilitiesDirectory;
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
                elementTargetDirectory = document.XPathSelectElement("/wi:Wix/wi:Fragment/wi:Directory[@Id='TARGETDIR']", namespaceManager);
                //elementUtilitiesDirectory = document.XPathSelectElement("/wi:Wix/wi:Fragment/wi:Directory[@Id='UTILITIESDIR']", namespaceManager);

                utilitiesDirectory = new ApplicationGeneratorBuildTasks.Directory("UTILITIESDIR", "Hydra Utilities");
                elementUtilitiesDirectory = utilitiesDirectory.ToXElement<ApplicationGeneratorBuildTasks.Directory>();
                //elementTargetDirectory.Parent.Add( @"<SetDirectory Id='UTILITIESDIR' Value='[ProgramFilesFolder][Manufacturer]\[ProductName] Utilities' />");

                elementFeature.Elements().Remove();
                elementTargetDirectory.Elements().Remove();

                elementTargetDirectory.Add(elementUtilitiesDirectory);

                foreach (var component in components)
                {
                    var elementComponent = component.ToXElement<Component>();
                    var elementFile = component.File.ToXElement<ApplicationGeneratorBuildTasks.File>();
                    var elementComponentRef = component.CreateComponentRef().ToXElement<ComponentRef>();

                    elementComponent.Add(elementFile);

                    elementTargetDirectory.Add(elementComponent);
                    elementFeature.Add(elementComponentRef);
                }

                groupDirectory = new ApplicationGeneratorBuildTasks.Directory("ApplicationGenerator.Binaries");
                componentGroupRef = groupDirectory.CreateComponentGroupRef();
                elementGroupDirectory = groupDirectory.ToXElement<ApplicationGeneratorBuildTasks.Directory>();
                elementComponentGroupRef = componentGroupRef.ToXElement<ComponentGroupRef>();

                elementTargetDirectory.Add(elementGroupDirectory);
                elementFeature.Add(elementComponentGroupRef);

                if (!this.IsProdRelease)
                {
                    var signedDirectory = directories.SingleOrDefault(d => d.Name == "Signed");

                    if (signedDirectory != null)
                    {
                        directories.Remove(signedDirectory);
                    }
                }

                topLeveDirectory = new ApplicationGeneratorBuildTasks.Directory();
                topLeveDirectory.Directories.AddRange(directories);

                AddDirectories(elementFeature, elementTargetDirectory, topLeveDirectory);

                // add start menu elements

                elementProgramMenuDirectory = elementTargetDirectory.XPathSelectElement("wi:Directory[@Id='ProgramMenuFolder']", namespaceManager);

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

                elementTargetDirectory.Add(elementProgramMenuDirectory);

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
                    elementFragmentComponentGroup.Parent.Remove();
                }

                elementProgramMenu = XElement.Parse(programMenuXml);
                document.Root.Add(elementProgramMenu);

                // handle utitilities
                
                HandleUtilities(this.TargetDir, targetBinariesPath, projectPath, productFilePath, elementUtilitiesDirectory, elementFeature, appGenAppConfigDocument);

                using (var writer = new XmlTextWriter(productFilePath, null))
                {
                    writer.Formatting = Formatting.Indented;
                    document.Save(writer);
                }
            }
            catch (Exception ex)
            {
                var error = string.Format("Error updating Installer. \r\nError: {0}", ex.ToString());

                var message = new BuildErrorEventArgs(string.Empty, string.Empty, string.Empty, 0, 0, 0, 0, error, "", "", DateTime.Now);
                this.BuildEngine.LogErrorEvent(message);

                Console.WriteLine(error);

                return false;
            }

            return true;
        }

        private void HandleUtilities(string originalBinariesPath, string targetBinariesPath, string projectPath, string productFilePath, XElement elementUtilitiesDirectory, XElement elementFeature, XDocument appGenAppConfigDocument)
        {
            XElement elementShimServiceExePath;
            XElement elementCrashAnalyzerExePath;
            XElement elementUserExperienceExePath;
            string shimServiceExePath;
            string crashAnalyzerExePath;
            string userExperienceExePath;
            string appGenIonicAngular;
            string appGenOverrides;
            string appGenVisualStudio; XElement parentElement;
            XNamespace _namespace = "http://schemas.microsoft.com/wix/2006/wi";
            List<FileInfo> utilityExeFiles;

            elementShimServiceExePath = appGenAppConfigDocument.XPathSelectElement("configuration/appSettings/add[@key='ShimServiceExeLocation']");
            elementCrashAnalyzerExePath = appGenAppConfigDocument.XPathSelectElement("configuration/appSettings/add[@key='CrashAnalyzerExeLocation']");
            elementUserExperienceExePath = appGenAppConfigDocument.XPathSelectElement("configuration/appSettings/add[@key='UserExperienceExeLocation']");

            shimServiceExePath = Path.GetFullPath(Path.Combine(originalBinariesPath, elementShimServiceExePath.Attribute("value").Value));
            crashAnalyzerExePath = Path.GetFullPath(Path.Combine(originalBinariesPath, elementCrashAnalyzerExePath.Attribute("value").Value));
            userExperienceExePath = Path.GetFullPath(Path.Combine(originalBinariesPath, elementUserExperienceExePath.Attribute("value").Value));
            appGenIonicAngular = Path.GetFullPath(Path.Combine(originalBinariesPath, "ApplicationGenerator.IonicAngular.dll"));
            appGenOverrides = Path.GetFullPath(Path.Combine(originalBinariesPath, "ApplicationGenerator.Overrides.dll"));
            appGenVisualStudio = Path.GetFullPath(Path.Combine(originalBinariesPath, "ApplicationGenerator.VisualStudio.dll"));

            utilityExeFiles = new List<FileInfo>
            {
                new FileInfo(crashAnalyzerExePath),
                new FileInfo(shimServiceExePath),
                new FileInfo(userExperienceExePath),
                new FileInfo(appGenIonicAngular),
                new FileInfo(appGenOverrides),
                new FileInfo(appGenVisualStudio)
            };

            foreach (var utilityExeFile in utilityExeFiles)
            {
                var binaryFolder = new DirectoryInfo(GetBinariesPath(utilityExeFile.Directory.FullName));

                DebugUtils.AssertThrow(utilityExeFile.Exists, $"Utility file not located as expected, expected path { utilityExeFile.FullName }");

                //foreach (var file in binaryFolder.GetFiles("*.*", SearchOption.AllDirectories))
                //{
                //    var relativePath = file.GetRelativePath(projectPath);

                //    parentElement = elementUtilitiesDirectory;

                //    foreach (var part in relativePath.Split(@"\").Skip(1))
                //    {
                //        if (part == file.Name)
                //        {
                //            var component = new Component(file, projectPath, productFilePath);
                //            var elementComponentRef = component.CreateComponentRef().ToXElement<ComponentRef>();
                //            var elementComponent = component.ToXElement<Component>();
                //            var elementFile = component.File.ToXElement<ApplicationGeneratorBuildTasks.File>();

                //            parentElement.Add(elementComponent);
                //            elementFeature.Add(elementComponentRef);

                //            elementComponent.Add(elementFile);
                //        }
                //        else if (!this.IsProdRelease && part == "Signed")
                //        {
                //            break;
                //        }
                //        else
                //        {
                //            var elementDirectory = parentElement.Elements().SingleOrDefault(e => e.Name.LocalName == "Directory" && e.Attribute("Name").Value == part);

                //            if (elementDirectory == null)
                //            {
                //                var id = "dir_" + Guid.NewGuid().ToString().Replace("-", "_");
                //                var directory = new ApplicationGeneratorBuildTasks.Directory(id, part);

                //                elementDirectory = directory.ToXElement<ApplicationGeneratorBuildTasks.Directory>();

                //                parentElement.Add(elementDirectory);
                //            }

                //            parentElement = elementDirectory;
                //        }
                //    }
                //}
            }
        }

        private string GetBinariesPath(string targetDir)
        {
            if (this.IsProdRelease)
            {
                return Path.Combine(targetDir, "Signed");
            }
            else
            {
                return targetDir;
            }
        }

        private bool IsProdRelease
        {
            get
            {
                return this.ProdReleaseBuild && this.Configuration == "Release";
            }
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
