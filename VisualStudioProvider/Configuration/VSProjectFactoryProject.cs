using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Utils;

namespace VisualStudioProvider.Configuration
{
    public delegate void LoadPackageHandler(Guid packageGuid, out VSPackage package);
    public delegate void LoadDirectoryHandler(Guid packageGuid, out VSTemplateDirectory templateDirectory);

    public class VSProjectFactoryProject
    {
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public VSPackage Package { get; private set; }
        public List<VSItemTemplateDirectory> TemplateDirectories { get; private set; }
        public Guid ProjectGuid { get; private set; }
        public static event LoadPackageHandler LoadPackage;
        public static event LoadDirectoryHandler LoadDirectory;

        public VSProjectFactoryProject(Guid projectGuid, RegistryKey key)
        {
            var projectKey = key.ToIndexable();
            var name = (string) projectKey.Default;
            string displayName = null;
            string packageGuid = null;
            var package = (VSPackage) null;

            if (projectKey["DisplayName"] != null)
            {
                displayName = projectKey["DisplayName"].ToString();
            }

            if (projectKey["Package"] != null)
            {
                packageGuid = (string)projectKey["Package"];
            }

            TemplateDirectories = new List<VSItemTemplateDirectory>();

            if (packageGuid != null)
            {
                LoadPackage(Guid.Parse(packageGuid), out package);

                if (displayName != null)
                {
                    if (displayName.StartsWith("#"))
                    {
                        this.DisplayName = package.GetString(displayName);
                    }
                    else
                    {
                        this.DisplayName = displayName;
                    }
                }
            }
            else
            {
                this.DisplayName = displayName;
            }

            this.ProjectGuid = projectGuid;
            this.Package = package;
            this.Name = name;

            var templateDirsKey = key.Enumerate().Where(k => k.SubName == "AddItemTemplates")
                .Select(k => k.Key.Enumerate().Where(k2 => k2.SubName == "TemplateDirs").SingleOrDefault()).SingleOrDefault();

            if (templateDirsKey != null)
            {
                foreach (var dirKey in templateDirsKey.Key.Enumerate())
                {
                    var projectDirectory = (VSTemplateDirectory) null;
                    var guid = new Guid(dirKey.SubName);

                    LoadDirectory(guid, out projectDirectory);

                    var directory = new VSItemTemplateDirectory(dirKey.Key, projectDirectory, this);

                    this.TemplateDirectories.Add(directory);
                }
            }
        }

        public Guid PackageGuid
        {
            get
            {
                if (this.Package != null)
                {
                    return this.Package.PackageGuid;
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }
    }
}
