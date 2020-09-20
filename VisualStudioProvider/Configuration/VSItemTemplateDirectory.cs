using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Microsoft.VisualStudio.Shell;
using Utils;
using System.Diagnostics;
using System.IO;
using CodeInterfaces;
using System.Threading;

namespace VisualStudioProvider.Configuration
{
    [DebuggerDisplay("{ DebugInfo }")]
    public class VSItemTemplateDirectory : VSTemplateDirectory
    {
        public VSTemplateDirectory ProjectDirectory { get; private set; }
        public VSProjectFactoryProject SpecialProject { get; private set; }

        public VSItemTemplateDirectory(RegistryKey key, VSTemplateDirectory projectDirectory, VSProjectFactoryProject specialProject) : base()
        {
            var directoryKey = key.ToIndexable();

            if (projectDirectory != null)
            {
                this.Package = projectDirectory.Package;
            }
            else
            {
                this.Package = specialProject.Package;
            }

            this.SubDirectories = new List<VSTemplateDirectory>();
            this.Guid = Guid.Parse(directoryKey.SubName);
            this.ProjectDirectory = projectDirectory;
            this.SpecialProject = specialProject;

            var keys = key.Enumerate();

            if (keys.Count() == 1)
            {
                var subKey = key.Enumerate().First();

                ProcessSubKey(subKey, false);
            }
            else
            {
                foreach (var subKey in keys)
                {
                    ProcessSubKey(subKey, true);
                }
            }
        }
    }
}
