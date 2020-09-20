using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using Utils;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Web;
using System.Drawing;
using Microsoft.VisualStudio.Shell.Interop;

namespace VisualStudioProvider.Configuration
{
    [DebuggerDisplay("{ Name }")]
    public class VSPackage
    {
        public string Name { get; private set; }
        public Guid PackageGuid { get; private set; }
        public FileInfo SatelliteDll { get; private set; }
        public FileInfo PackageDll { get; private set; }
        public FileInfo CodeBase { get; private set; }
        public List<VSProjectFactoryProject> FactoryProjects { get; private set; }
        private Dictionary<string, VSTemplateProjectItem> itemTemplates;
        private Dictionary<string, VSTemplateProject> projectTemplates;
        private string assemblyName;
        private string className;
        private string codeBase;
        public VSTemplateDirectory TemplateDirectory { get; set; }
        public static Guid IID_IVsPackage = typeof(IVsPackage).GUID;

        public VSPackage(Guid packageGuid, RegistryKey key)
        {
            var packageKey = key.ToIndexable();
            var satelliteDllName = (string) packageKey[@"SatelliteDll\@DllName"];
            var packageDll = (string)packageKey["InprocServer32"];
            var languageID = Thread.CurrentThread.CurrentCulture.LCID;
            
            codeBase = (string)packageKey["CodeBase"];
            className = (string)packageKey["Class"];

            this.PackageGuid = packageGuid;
            this.Name = (string) packageKey.Default;
            this.FactoryProjects = new List<VSProjectFactoryProject>();

            if (codeBase != null)
            {
                if (codeBase.StartsWith("file:"))
                {
                    var uri = new Uri(codeBase);

                    if (uri.Scheme == "file")
                    {
                        codeBase = HttpUtility.UrlDecode(uri.AbsolutePath);
                    }
                }

                this.CodeBase = new FileInfo(codeBase);
            }

            if (satelliteDllName != null)
            {
                var satellitePath = (string)packageKey[@"SatelliteDll\@Path"];

                if (satellitePath == null)
                {
                    this.SatelliteDll = new FileInfo(Path.Combine(VSConfig.VisualStudioInstallDirectory, languageID.ToString(), satelliteDllName));
                }
                else
                {
                    this.SatelliteDll = new FileInfo(Path.Combine(satellitePath, languageID.ToString(), satelliteDllName));
                }
            }

            if (className != null)
            {
                if (codeBase == null)
                {
                    var defaultName = (string) packageKey.Default;
                    
                    assemblyName = (string)packageKey[@"Assembly"];

                    if (!TypeExtensions.IsQualifiedName(assemblyName))
                    {
                        assemblyName = defaultName;
                    }
                }
            }

            if (packageDll != null)
            {
                this.PackageDll = new FileInfo(packageDll);

                if (!this.PackageDll.Exists)
                {
                    this.PackageDll = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), packageDll));
                }
            }
        }

        public Type PackageType 
        {
            get
            {
                if (this.PackageAssembly != null)
                {
                    return this.PackageAssembly.GetType(className);
                }
                else
                {
                    return null;
                }
            }
        }

        public string ClassName
        {
            get
            {
                return className;
            }
        }

        public Assembly PackageAssembly
        {
            get
            {
                if (codeBase != null)
                {
                    try
                    {
                        return Assembly.LoadFrom(codeBase);
                    }
                    catch
                    {
                        return null;
                    }
                }
                else
                {
                    try
                    {
                        return Assembly.Load(assemblyName);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }

        public Icon GetIcon(string resourceID)
        {
            Icon icon = null;

            if (this.PackageDll != null)
            {
                icon = ResourceLoader.LoadIconFrom(this.PackageDll.FullName, resourceID);

                if (icon != null)
                {
                    return icon;
                }
            }
            
            if (this.PackageAssembly != null)
            {
                if (this.CodeBase != null)
                {
                    icon = ResourceLoader.LoadIconFrom(this.CodeBase.FullName, resourceID);
                }
                else if (this.PackageAssembly.CodeBase != null)
                {
                    icon = ResourceLoader.LoadIconFrom(this.PackageAssembly.CodeBase, resourceID);
                }

                if (icon != null)
                {
                    return icon;
                }
            }

            if (this.SatelliteDll != null)
            {
                icon = ResourceLoader.LoadIconFrom(this.SatelliteDll.FullName, resourceID);

                if (icon != null)
                {
                    return icon;
                }
            }

            return null;
        }

        public string GetString(string resourceID)
        {
            if (this.SatelliteDll != null)
            {
                return ResourceLoader.LoadStringFrom(this.SatelliteDll.FullName, resourceID);
            }
            else if (this.CodeBase != null)
            {
                return ResourceLoader.LoadStringFrom(this.CodeBase.FullName, resourceID);
            }
            else
            {
                return null;
            }
        }

        public Dictionary<string, VSTemplateProjectItem> ItemTemplates
        {
            get 
            {
                if (itemTemplates == null)
                {
                    itemTemplates = new Dictionary<string, VSTemplateProjectItem>();
                }

                return itemTemplates; 
            }
        }

        public Dictionary<string, VSTemplateProject> ProjectTemplates
        {
            get 
            {
                if (projectTemplates == null)
                {
                    projectTemplates = new Dictionary<string, VSTemplateProject>();
                }

                return projectTemplates; 
            }
        }
    }
}
