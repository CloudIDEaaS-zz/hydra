using AbstraX;
using ApplicationGenerator.Tests.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace ApplicationGenerator.Tests
{

    [TestClass]
    public class GenerationTests
    {

        [TestInitialize()]
        public void Initialize()
        {
            ClearProjectFolders();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            ClearProjectFolders();
        }

        private void ClearProjectFolders()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var rootPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(assembly.Location), @"..\..\..\"));
            var hydraJsonContent = typeof(BasicTests).ReadResource<string>(@"TestApps\Ionic_Angular\Config\hydra.json");
            var hydraJsonContentFile = new FileInfo(Path.Combine(rootPath, @"ApplicationGenerator.Tests\TestApps\Ionic_Angular\Config\hydra.json"));
            var hydraJsonContentDirectory = hydraJsonContentFile.Directory;
            var hydraJsonPath = Path.Combine(rootPath, @"ApplicationGenerator.Tests\TestApps\Ionic_Angular\Project\hydra.json");
            var hydraJsonFile = new FileInfo(hydraJsonPath);
            var projectDirectory = new DirectoryInfo(hydraJsonFile.DirectoryName);

            if (hydraJsonFile.Exists)
            {
                hydraJsonFile.Delete();
            }

            projectDirectory.ForceDeleteAllFilesAndSubFolders(false, (f) =>
            {
                if (f.FileSystemInfo is DirectoryInfo directory)
                {
                    if (directory.FullName.AsCaseless() == hydraJsonContentDirectory.FullName)
                    {
                        return false;
                    }
                }
                else if (f.FileSystemInfo is FileInfo file)
                {
                    if (file.FullName.AsCaseless() == hydraJsonContentFile.FullName)
                    {
                        return false;
                    }
                }

                return true;
            });
        }

        [TestMethod]
        public void ApplicationGenerator_RunWaitForInputGenerateAllNoFrontendBlank_FilesGenerated()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var rootPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(assembly.Location), @"..\..\..\"));
                var hydraJsonContent = typeof(BasicTests).ReadResource<string>(@"TestApps\Ionic_Angular\Config\hydra.json");
                var hydraJsonPath = Path.Combine(rootPath, @"ApplicationGenerator.Tests\TestApps\Ionic_Angular\Project\hydra.json");
                var hydraJsonFile = new FileInfo(hydraJsonPath);
                var projectPath = hydraJsonFile.DirectoryName;
                var projectDirectory = new DirectoryInfo(projectPath);
                var outputBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();
                var stopwatch = new Stopwatch();
                string appGenPath;
                AppGenCommandHandler appGenCommandHandler;
                AppFolderStructureSurveyor appFolderStructureSurveyor;

                Console.WriteLine($"Running { nameof(GenerationTests) }.{ nameof(ApplicationGenerator_RunWaitForInputGenerateAllNoFrontendBlank_FilesGenerated) }");

                if (!projectDirectory.Exists)
                {
                    projectDirectory.Create();
                }

                File.WriteAllText(hydraJsonPath, hydraJsonContent);
#if DEBUG
                appGenPath = Path.Combine(rootPath, @"ApplicationGenerator\bin\Debug\ApplicationGenerator.exe");
#else
                appGenPath = Path.Combine(rootPath, @"ApplicationGenerator\bin\Release\ApplicationGenerator.exe");
#endif
                appGenCommandHandler = new AppGenCommandHandler(appGenPath);

                appGenCommandHandler.OutputWriteLine = (format, args) =>
                {
                    outputBuilder.AppendLineFormat(format, args);
                    Console.WriteLine("\t" + string.Format(format, args));

                };

                appGenCommandHandler.ErrorWriteLine = (format, args) =>
                {
                    errorBuilder.AppendLineFormat(format, args);
                    Console.WriteLine("\t" + string.Format(format, args));
                };

                stopwatch.Start();

#if DEBUG
                appGenCommandHandler.RunEmptyCommand(projectPath);
#else
                appGenCommandHandler.RunAsAutomated(projectPath);
#endif

                while (!appGenCommandHandler.Process.WaitForExit(1))
                {
                    Thread.Sleep(100);

                    if (stopwatch.ElapsedMilliseconds > 50000)
                    {
                        throw new TimeoutException("Timeout waiting for process to complete");
                    }
                }

                Assert.IsTrue(errorBuilder.ToString() == string.Empty, $"Unexpected error output for GenerateWorkspace: { errorBuilder.ToString() }");
                Assert.IsTrue(appGenCommandHandler.Process.ExitCode == 0, $"Unexpected exit code for GenerateWorkspace: { appGenCommandHandler.Process.ExitCode.ToHexString(true) }");

                Assert.IsTrue(outputBuilder.ToString().Contains("GenerateWorkspace Complete."), $"Unexpected output for GenerateWorkspace: { outputBuilder.ToString() }");
                Assert.IsTrue(outputBuilder.ToString().Contains("GenerateBusinessModelBlank Complete."), $"Unexpected output for GenerateBusinessModelBlank: { outputBuilder.ToString() }");
                Assert.IsTrue(outputBuilder.ToString().Contains("GenerateEntitiesFromTemplateBlank Complete."), $"Unexpected output for GenerateEntitiesFromTemplateBlank: { outputBuilder.ToString() }");
                Assert.IsTrue(outputBuilder.ToString().Contains("GenerateEntitiesFromJson Complete."), $"Unexpected output for GenerateEntitiesFromJson: { outputBuilder.ToString() }");

                appFolderStructureSurveyor = new AppFolderStructureSurveyor();

                appFolderStructureSurveyor.DetermineLayout(projectPath);

                Assert.IsNotNull(appFolderStructureSurveyor.WebProjectPath, "WebProjectPath is null");
                Assert.IsNotNull(appFolderStructureSurveyor.EntitiesProjectPath, "EntitiesProjectPath is null");
                Assert.IsNotNull(appFolderStructureSurveyor.ServicesProjectPath, "ServicesProjectPath is null");
                Assert.IsNotNull(appFolderStructureSurveyor.BusinessModelFilePath, "BusinessModelFilePath is null");
                Assert.IsNotNull(appFolderStructureSurveyor.HydraJsonPath, "HydraJsonPath is null");

                Assert.IsTrue(File.Exists(appFolderStructureSurveyor.WebProjectPath), "WebProjectPath does not exist");
                Assert.IsTrue(File.Exists(appFolderStructureSurveyor.EntitiesProjectPath), "EntitiesProjectPath does not exist");
                Assert.IsTrue(File.Exists(appFolderStructureSurveyor.ServicesProjectPath), "ServicesProjectPath does not exist");
                Assert.IsTrue(File.Exists(appFolderStructureSurveyor.BusinessModelFilePath), "BusinessModelFilePath does not exist");
                Assert.IsTrue(File.Exists(appFolderStructureSurveyor.HydraJsonPath), "HydraJsonPath does not exist");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\t" + ex.Message);
                throw;
            }
        }
    }
}
