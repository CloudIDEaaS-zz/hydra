using BuildTasks;
using Microsoft.Build.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestApplicationGeneratorBuildTasks
{
    public class BuildEngine : IBuildEngine
    {
        public bool ContinueOnError => true;

        public int LineNumberOfTaskNode => 1;

        public int ColumnNumberOfTaskNode => 1;

        public string ProjectFileOfTaskNode { get; set; }

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
        {
            return true;
        }

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        public void LogErrorEvent(BuildErrorEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var installTemplates = new InstallTemplates();
            var zipAppFiles = new ZipAppFiles();
            var signAssemblies = new SignAssemblies();
            var updateHydraInstaller = new UpdateInstaller();
            var updateHydraNuget = new UpdateHydraNuget();
            var updateExportedProjectTemplates = new UpdateExportedProjectTemplates();
            var assembly = Assembly.GetEntryAssembly();
            var buildEngine = new BuildEngine();
            var solutionPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(assembly.Location), @"..\..\..\"));
            var scannerFile = new FileInfo(Path.Combine(solutionPath, @"ApplicationGenerator\bin\Release\Signed\Hydra.Scanner.dll"));

            //updateHydraInstaller.Configuration = "Release";
            //updateHydraInstaller.TargetFramework = "v4.6.1";
            //updateHydraInstaller.TargetAssembly = "HydraDesktop";
            //updateHydraInstaller.SolutionDir = @"D:\MC\CloudIDEaaS\root";
            //updateHydraInstaller.ProductRelativePath = @"HydraDesktop.InstallerStandalone\Product.wxs";
            //updateHydraInstaller.TargetDir = @"D:\MC\CloudIDEaaS\root\ApplicationGenerator\bin\Debug";
            //updateHydraInstaller.ProjectPath = @"D:\MC\CloudIDEaaS\root\HydraDesktop\HydraDesktop.csproj";

            updateHydraInstaller.Configuration = "Release";
            updateHydraInstaller.TargetFramework = "v4.6.1";
            updateHydraInstaller.TargetAssembly = "ApplicationGenerator";
            updateHydraInstaller.SolutionDir = solutionPath;
            updateHydraInstaller.ProductRelativePath = @"Hydra.InstallerStandalone\Product.wxs";
            updateHydraInstaller.TargetDir = Path.Combine(solutionPath, @"ApplicationGenerator\bin\Debug");
            updateHydraInstaller.ProjectPath = Path.Combine(solutionPath, @"ApplicationGenerator\ApplicationGenerator.csproj");

            signAssemblies.Configuration = "Release";
            signAssemblies.TargetFramework = "v4.6.1";
            signAssemblies.TargetAssembly = "ApplicationGenerator";
            signAssemblies.SolutionDir = solutionPath;
            signAssemblies.ProductRelativePath = @"Hydra.InstallerStandalone\Product.wxs";
            signAssemblies.TargetDir = Path.Combine(solutionPath, @"ApplicationGenerator\bin\Release");
            signAssemblies.ProjectPath = Path.Combine(solutionPath, @"ApplicationGenerator\ApplicationGenerator.csproj");
            signAssemblies.BuildEngine = buildEngine;

            if (scannerFile.Exists)
            {
                //var keyPath = Path.Combine(solutionPath, @"ApplicationGeneratorBuildTasks\HydraCodeCert\CloudIDEaaS.pfx");
                //var password = "Ca1hm%%3na8s#sirJnGa7BGJhN2OQ7Fy584OoxEuD!2FYSb6NjKXNSCSYSCwOfEX#2j3BX8CM$5GC%y!IwUN6bMP83NwM0MLApa!";

                //signAssemblies.HandleScannerFile(new FileInfo(@"D:\MC\CloudIDEaaS\develop\ApplicationGenerator\bin\Release\Signed\Hydra.Scanner.dll"), solutionPath, keyPath);
            }

            //signAssemblies.Execute();
            updateHydraInstaller.Execute();

            //updateExportedProjectTemplates.Execute();

            //updateHydraNuget.Configuration = "Release";
            //updateHydraNuget.TargetFramework = "netcoreapp3.1";

            //updateHydraNuget.Execute();

            //installTemplates.Execute();
            //zipAppFiles.Execute();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
