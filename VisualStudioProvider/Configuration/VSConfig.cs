using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;

namespace VisualStudioProvider.Configuration
{
    public static class VSConfig
    {
        public static string VisualStudioInstallDirectory
        {
            get
            {
                string installationPath = null;

                if (Environment.Is64BitOperatingSystem)
                {
                    installationPath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Microsoft\\VisualStudio\\10.0\\", "InstallDir", null);
                }
                else
                {
                    installationPath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\10.0\\", "InstallDir", null);
                }

                return installationPath;
            }
        }

        public static string ItemTemplateDirectory
        {
            get
            {
                return Path.Combine(VSConfig.VisualStudioInstallDirectory, @"ItemTemplates\");
            }
        }

        public static string ProjectTemplateDirectory
        {
            get
            {
                return Path.Combine(VSConfig.VisualStudioInstallDirectory,  @"ProjectTemplates\");
            }
        }
    }
}
