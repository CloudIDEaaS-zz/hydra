using BuildTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApplicationGeneratorBuildTasks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var installTemplates = new InstallTemplates();
            var zipAppFiles = new ZipAppFiles();
            var updateHydraInstaller = new UpdateHydraInstaller();

            updateHydraInstaller.Configuration = "PreRelease";
            updateHydraInstaller.TargetFramework = "v4.6.1";
            updateHydraInstaller.TargetAssembly = "ApplicationGenerator";

            updateHydraInstaller.Execute();
            installTemplates.Execute();
            zipAppFiles.Execute();
        }
    }
}
