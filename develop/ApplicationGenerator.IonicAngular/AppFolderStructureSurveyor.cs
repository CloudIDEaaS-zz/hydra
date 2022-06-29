using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    public class AppFolderStructureSurveyor : IAppFolderStructureSurveyor
    {
        public List<Exception> Exceptions { get; private set; }
        public string TopMostFolder { get; private set; }
        public string WebProjectPath { get; private set; }
        public string WebFrontEndRootPath { get; private set; }
        public string BusinessModelFilePath { get; private set; }
        public string EntitiesProjectPath { get; private set; }
        public string ServicesProjectPath { get; private set; }
        public string ProjectFolderRoot { get; private set; }
        public FileInfo HydraJsonFile { get; private set; }
        public string HydraJsonPath { get; private set; }
        public FileInfo WorkspaceFile { get; private set; }
        public string AppName { get; set; }
        public bool FoundationGenerated { get; private set; }
        public bool TemplateGenerated { get; private set; }
        public bool AppGenerated { get; private set; }
        public ConfigObject ConfigObject { get; private set; }
        public Dictionary<string, DirectoryInfo> Builds { get; private set; }

        public AppFolderStructureSurveyor()
        {
            this.Exceptions = new List<Exception>();
            this.Builds = new Dictionary<string, DirectoryInfo>();
        }

        public void DetermineLayout(string path = null)
        {
            var folderRoot = Environment.CurrentDirectory;
            string webFrontEndRootPath = null;
            string businessModelFile = null;
            string entitiesProjectPath = null;
            string servicesProjectPath = null;
            string webProjectPath = null;
            string appName = this.AppName;
            FileInfo jsonFile;
            var projectFolderRoot = folderRoot;
            DirectoryInfo rootDirectory;
            FileInfo solutionFile = null;
            ConfigObject configObject = null;

            if (path != null)
            {
                folderRoot = path;
                projectFolderRoot = folderRoot;
            }

            rootDirectory = new DirectoryInfo(projectFolderRoot);

            if (rootDirectory.Exists)
            {
                solutionFile = rootDirectory.GetFiles("*.sln", SearchOption.TopDirectoryOnly).SingleOrDefault();
                jsonFile = rootDirectory.GetFiles("hydra.json", SearchOption.TopDirectoryOnly).SingleOrDefault();

                if (jsonFile == null)
                {
                    var jsonFileLnk = rootDirectory.GetFiles("hydra.json.lnk", SearchOption.TopDirectoryOnly).OrderBy(f => f.FullName.Length).FirstOrDefault();

                    if (jsonFileLnk != null)
                    {
                        jsonFile = jsonFileLnk.GetShortcutTarget();
                    }
                    else
                    {
                        var parentDirectory = rootDirectory.Parent;

                        while (parentDirectory != null)
                        {
                            if (solutionFile == null)
                            {
                                solutionFile = parentDirectory.GetFiles("*.sln", SearchOption.TopDirectoryOnly).SingleOrDefault();
                            }

                            if (jsonFile == null)
                            {
                                jsonFile = parentDirectory.GetFiles("hydra.json", SearchOption.TopDirectoryOnly).SingleOrDefault();
                                jsonFileLnk = parentDirectory.GetFiles("hydra.json.lnk", SearchOption.TopDirectoryOnly).OrderBy(f => f.FullName.Length).FirstOrDefault();

                                if (jsonFileLnk != null)
                                {
                                    jsonFile = jsonFileLnk.GetShortcutTarget();
                                }
                            }

                            if (CompareExtensions.AllAreNotNull(solutionFile, jsonFile))
                            {
                                break;
                            }

                            parentDirectory = parentDirectory.Parent;
                        }
                    }
                }

                if (jsonFile != null && jsonFile.Exists)
                {
                    configObject = ConfigObject.Load(jsonFile.FullName);

                    appName = configObject.AppName;
                }
                else
                {
                    appName = this.AppName;
                }

                if (solutionFile == null)
                {
                    solutionFile = rootDirectory.Parent.Parent.GetFiles(string.Format("{0}.sln", appName), SearchOption.TopDirectoryOnly).SingleOrDefault();
                    webFrontEndRootPath = rootDirectory.FullName;
                }
            }
            else
            {
                this.TopMostFolder = rootDirectory.FullName;
                jsonFile = null;
            }

            if (solutionFile != null && rootDirectory.GetParts().Length > solutionFile.Directory.GetParts().Length)
            {
                rootDirectory = solutionFile.Directory;

                folderRoot = rootDirectory.FullName;
                TopMostFolder = folderRoot;

                if (servicesProjectPath == null)
                {
                    servicesProjectPath = Path.GetFullPath(Path.Combine(path, string.Format(@"..\..\{0}.Services\{0}.Services.csproj", appName)));
                }

                if (entitiesProjectPath == null)
                {
                    entitiesProjectPath = Path.GetFullPath(Path.Combine(path, string.Format(@"..\..\{0}.Entities\{0}.Entities.csproj", appName)));
                }

                if (webProjectPath == null)
                {
                    webProjectPath = Path.GetFullPath(Path.Combine(path, string.Format(@"..\..\{0}.Web\{0}.Web.csproj", appName)));
                }
            }
            else
            {
                folderRoot = rootDirectory.FullName;
                TopMostFolder = folderRoot;

                if (servicesProjectPath == null)
                {
                    servicesProjectPath = Path.GetFullPath(Path.Combine(this.TopMostFolder, string.Format(@"{0}.Services\{0}.Services.csproj", appName)));
                }

                if (entitiesProjectPath == null)
                {
                    entitiesProjectPath = Path.GetFullPath(Path.Combine(this.TopMostFolder, string.Format(@"{0}.Entities\{0}.Entities.csproj", appName)));
                }

                if (webProjectPath == null)
                {
                    webProjectPath = Path.GetFullPath(Path.Combine(this.TopMostFolder, string.Format(@"\{0}.Web\{0}.Web.csproj", appName)));
                }
            }

            if (!File.Exists(entitiesProjectPath))
            {
                this.Exceptions.Add(new IOException(string.Format("Entities project does not exist. Project file: '{0}'", entitiesProjectPath)));
            }

            if (!File.Exists(servicesProjectPath))
            {
                this.Exceptions.Add(new IOException(string.Format("Services project does not exist. Project file: '{0}'", servicesProjectPath)));
            }

            if (!File.Exists(webProjectPath))
            {
                this.Exceptions.Add(new IOException(string.Format("Web project does not exist. Project file: '{0}'", webProjectPath)));
            }

            businessModelFile = Path.Combine(this.TopMostFolder, "businessModel.json");

            if (appName.IsNullOrEmpty())
            {
                webFrontEndRootPath = Path.Combine(Path.GetDirectoryName(webProjectPath), "Unnamed");
            }
            else
            {
                webFrontEndRootPath = Path.Combine(Path.GetDirectoryName(webProjectPath), appName);
            }

            if (File.Exists(webProjectPath))
            {
                this.FoundationGenerated = true;
            }

            if (Directory.Exists(webFrontEndRootPath))
            {
                var directory = new DirectoryInfo(webFrontEndRootPath);
                var packageJson = directory.GetFiles("package.json").SingleOrDefault();
                var schemaDocument = directory.GetFiles(string.Format("{0}*.xsd", appName)).SingleOrDefault();

                if (schemaDocument != null)
                {
                    this.TemplateGenerated = true;
                    this.AppGenerated = true;
                }
                else if (packageJson != null)
                {
                    this.TemplateGenerated = true;
                }
            }

            this.AppName = appName;
            this.ConfigObject = configObject;
            this.WebProjectPath = webProjectPath;
            this.WebFrontEndRootPath = webFrontEndRootPath;
            this.BusinessModelFilePath = businessModelFile;
            this.EntitiesProjectPath = entitiesProjectPath;
            this.ServicesProjectPath = servicesProjectPath;
            this.ProjectFolderRoot = projectFolderRoot;
            this.WorkspaceFile = solutionFile;
            this.HydraJsonFile = jsonFile;

            if (jsonFile != null)
            {
                this.HydraJsonPath = jsonFile.FullName;
            }
            else
            {
                this.HydraJsonPath = Path.Combine(this.ProjectFolderRoot, "hydra.json");
            }

            if (File.Exists(this.HydraJsonPath) && Directory.Exists(this.WebFrontEndRootPath))
            {
                DetermineBuilds();
            }
        }

        public void DetermineLayout(Dictionary<string, object> arguments)
        {
            var folderRoot = Environment.CurrentDirectory;
            var webFrontEndRootPath = (string)arguments.SingleOrDefault(a => a.Key == "WebFrontEndRootPath").Value;
            var jsonFilePath = (string)arguments.SingleOrDefault(a => a.Key == "JsonFile").Value;
            var businessModelFile = (string)arguments.SingleOrDefault(a => a.Key == "BusinessModelFile").Value;
            var entitiesProjectPath = (string)arguments.SingleOrDefault(a => a.Key == "EntitiesProjectPath").Value;
            var servicesProjectPath = (string)arguments.SingleOrDefault(a => a.Key == "ServicesProjectPath").Value;
            var webProjectPath = (string)arguments.SingleOrDefault(a => a.Key == "WebProjectPath").Value;
            var appName = (string)arguments.SingleOrDefault(a => a.Key == "AppName").Value;
            var projectFolderRoot = folderRoot;
            DirectoryInfo rootDirectory;
            FileInfo solutionFile;
            FileInfo jsonFile;

            rootDirectory = new DirectoryInfo(projectFolderRoot);
            solutionFile = rootDirectory.GetFiles("*.sln", SearchOption.TopDirectoryOnly).SingleOrDefault();

            if (jsonFilePath.IsNullOrEmpty())
            {
                jsonFile = rootDirectory.GetFiles("hydra.json", SearchOption.TopDirectoryOnly).SingleOrDefault();
            }
            else
            {
                jsonFile = new FileInfo(jsonFilePath);
            }

            if (solutionFile == null)
            {
                solutionFile = rootDirectory.Parent.Parent.GetFiles(string.Format("{0}.sln", appName), SearchOption.TopDirectoryOnly).SingleOrDefault();
            }

            if (solutionFile != null && rootDirectory.GetParts().Length > solutionFile.Directory.GetParts().Length)
            {
                rootDirectory = solutionFile.Directory;

                folderRoot = rootDirectory.FullName;
                TopMostFolder = folderRoot;

                if (servicesProjectPath == null)
                {
                    servicesProjectPath = Path.GetFullPath(Path.Combine(this.TopMostFolder, string.Format(@"..\..\{0}.Services\{0}.Services.csproj", appName)));
                }

                if (entitiesProjectPath == null)
                {
                    entitiesProjectPath = Path.GetFullPath(Path.Combine(this.TopMostFolder, string.Format(@"..\..\{0}.Entities\{0}.Entities.csproj", appName)));
                }

                if (webProjectPath == null)
                {
                    webProjectPath = Path.GetFullPath(Path.Combine(this.TopMostFolder, string.Format(@"..\..\{0}.Web\{0}.Web.csproj", appName)));
                }
            }
            else
            {
                folderRoot = rootDirectory.FullName;
                TopMostFolder = folderRoot;

                if (servicesProjectPath == null)
                {
                    servicesProjectPath = Path.GetFullPath(Path.Combine(this.TopMostFolder, string.Format(@"{0}.Services\{0}.Services.csproj", appName)));
                }

                if (entitiesProjectPath == null)
                {
                    entitiesProjectPath = Path.GetFullPath(Path.Combine(this.TopMostFolder, string.Format(@"{0}.Entities\{0}.Entities.csproj", appName)));
                }

                if (webProjectPath == null)
                {
                    webProjectPath = Path.GetFullPath(Path.Combine(this.TopMostFolder, string.Format(@"{0}.Web\{0}.Web.csproj", appName)));
                }
            }

            if (!File.Exists(entitiesProjectPath))
            {
                this.Exceptions.Add(new IOException(string.Format("Entities project does not exist. Project file: '{0}'", entitiesProjectPath)));
            }

            if (!File.Exists(servicesProjectPath))
            {
                this.Exceptions.Add(new IOException(string.Format("Services project does not exist. Project file: '{0}'", servicesProjectPath)));
            }

            if (!File.Exists(webProjectPath))
            {
                this.Exceptions.Add(new IOException(string.Format("Web project does not exist. Project file: '{0}'", webProjectPath)));
            }

            this.WebProjectPath = webProjectPath;
            this.WebFrontEndRootPath = webFrontEndRootPath;
            this.BusinessModelFilePath = businessModelFile;
            this.EntitiesProjectPath = entitiesProjectPath;
            this.ServicesProjectPath = servicesProjectPath;
            this.ProjectFolderRoot = projectFolderRoot;
            this.WorkspaceFile = solutionFile;
            this.HydraJsonFile = jsonFile;

            if (jsonFile != null)
            {
                this.HydraJsonPath = jsonFile.FullName;
            }
            else
            {
                this.HydraJsonPath = Path.Combine(this.ProjectFolderRoot, "hydra.json");
            }

            if (File.Exists(this.HydraJsonPath) && Directory.Exists(this.WebFrontEndRootPath))
            {
                DetermineBuilds();
            }
        }

        private void DetermineBuilds()
        {
            var webBuild = new DirectoryInfo(Path.Combine(this.WebFrontEndRootPath, "www"));
            var androidBuild = new DirectoryInfo(Path.Combine(this.WebFrontEndRootPath, @"platforms\android"));
            var iosBuild = new DirectoryInfo(Path.Combine(this.WebFrontEndRootPath, @"platforms\ios"));

            if (webBuild.Exists && webBuild.GetFiles().Length > 1)
            {
                if (!this.Builds.ContainsKey("Web"))
                {
                    this.Builds.Add("Web", webBuild);
                }
            }
            else if(this.Builds.ContainsKey("Web"))
            {
                this.Builds.Remove("Web");
            }

            if (androidBuild.Exists && androidBuild.GetFiles().Length > 1)
            {
                if (!this.Builds.ContainsKey("Android"))
                {
                    this.Builds.Add("Android", androidBuild);
                }
            }
            else if (this.Builds.ContainsKey("Android"))
            {
                this.Builds.Remove("Android");
            }

            if (iosBuild.Exists && androidBuild.GetFiles().Length > 1)
            {
                if (!this.Builds.ContainsKey("iOS"))
                {
                    this.Builds.Add("iOS", iosBuild);
                }
            }
            else if (this.Builds.ContainsKey("iOS"))
            {
                this.Builds.Remove("iOS");
            }
        }

        public void Refresh()
        {
            DetermineLayout(this.ProjectFolderRoot);
        }

        public ConfigObject CreateConfig(string generatorHandlerType)
        {
            var hydraJsonFile = new FileInfo(Path.Combine(this.ProjectFolderRoot, "hydra.json"));
            var configObject = new ConfigObject
            {
                AppName = this.AppName,
                GeneratorHandlerType = generatorHandlerType,
                WebProjectPath = this.WebProjectPath,
                ServicesProjectPath = this.ServicesProjectPath,
                EntitiesProjectPath = this.EntitiesProjectPath,
                WebFrontEndRootPath = this.WebFrontEndRootPath,
            };

            if (this.WorkspaceFile != null)
            {
                configObject.WorkspacePath = this.WorkspaceFile.FullName;
            }

            if (!Directory.Exists(this.ProjectFolderRoot))
            {
                Directory.CreateDirectory(this.ProjectFolderRoot);
            }

            configObject.Save(hydraJsonFile.FullName);

            this.ConfigObject = configObject;

            return configObject;
        }
    }
}
