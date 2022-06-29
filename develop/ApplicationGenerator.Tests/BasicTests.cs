using AbstraX;
using ApplicationGenerator.Tests.Utility;
using Hydra.ReleaseManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Utils;

namespace ApplicationGenerator.Tests
{
    /// <summary>   (Unit Test Class) an application generator tests.  </summary>
    ///     
    /// <remarks>
    ///  CloudIDEaaS, 7/28/2021. 
    ///  Naming convention: [UnitOfWork_StateUnderTest_ExpectedBehavior]
    ///  </remarks>

    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void ApplicationGenerator_DirectRunNoParms_ReportsThatCannotBeRunDirectly()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var rootPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(assembly.Location), @"..\..\..\"));
                var outputBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();
                string appGenPath;
                AppGenCommandHandler appGenCommandHandler;
                var stopwatch = new Stopwatch();

                Console.WriteLine($"Running { nameof(BasicTests) }.{ nameof(ApplicationGenerator_DirectRunNoParms_ReportsThatCannotBeRunDirectly) }");
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

                appGenCommandHandler.RunEmptyAsAutomated();

                while (!appGenCommandHandler.Process.WaitForExit(1))
                {
                    Thread.Sleep(100);

                    if (stopwatch.ElapsedMilliseconds > 10000)
                    {
                        throw new TimeoutException("Timeout waiting for process to complete");
                    }
                }

                Assert.IsTrue(errorBuilder.ToString() == string.Empty);
                Assert.IsTrue(appGenCommandHandler.Process.ExitCode == 0);
                Assert.IsTrue(outputBuilder.ToString().Contains("This program was not intended to be run directly"), $"Unexpected output: { outputBuilder.ToString() }");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\t" + ex.Message);
                throw;
            }
        }

        [TestMethod]
        public void ApplicationGenerator_RunWaitForInputGetVersion_ReportsVersionMatchingInstaller()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var rootPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(assembly.Location), @"..\..\..\"));
                var outputBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();
                var stopwatch = new Stopwatch();
                string appGenPath;
                string installerPath;
                AppGenCommandHandler appGenCommandHandler;
                StreamWriter writer;
                StreamReader reader;
                CommandPacket commandPacketReturn;
                HydraInstaller hydraInstaller;
                string version;

                Console.WriteLine($"Running { nameof(BasicTests) }.{ nameof(ApplicationGenerator_DirectRunNoParms_ReportsThatCannotBeRunDirectly) }");
#if DEBUG
                appGenPath = Path.Combine(rootPath, @"ApplicationGenerator\bin\Debug\ApplicationGenerator.exe");
                installerPath = Path.Combine(rootPath, @"Hydra.Installer\Installers\Debug\Hydra.InstallerStandalone.msi");
#else
                appGenPath = Path.Combine(rootPath, @"ApplicationGenerator\bin\Release\ApplicationGenerator.exe");
                installerPath = Path.Combine(rootPath, @"Hydra.Installer\Installers\Release\Hydra.InstallerStandalone.msi");
#endif
                hydraInstaller = new HydraInstaller();
                appGenCommandHandler = new AppGenCommandHandler(appGenPath);

                hydraInstaller.MsiPath = installerPath;

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

                appGenCommandHandler.RunForStdio("-waitForInput");

                Assert.IsTrue(errorBuilder.ToString() == string.Empty);
                Assert.IsFalse(appGenCommandHandler.Process.HasExited);

                writer = appGenCommandHandler.Writer;
                reader = appGenCommandHandler.Reader;

                writer.WriteJsonCommand(new CommandPacket
                {
                    Command = ServerCommands.CONNECT
                });

                commandPacketReturn = reader.ReadJsonCommand();

                Assert.IsTrue(commandPacketReturn.Response.ToString() == "Connected successfully", $"Connection to assembly failed with { commandPacketReturn.Response }");

                writer.WriteJsonCommand(new CommandPacket
                {
                    Command = ServerCommands.GET_VERSION
                });

                commandPacketReturn = reader.ReadJsonCommand();
                version = hydraInstaller.GetProductVersion();

#if !DEBUG
                // This should pass in devops.  If this is not passing locally, assure you compile Hydra.InstallerStandalone.msi
                // Also assure that the Assembly and File versions are the same in ApplicationGenerator\Properties\AssemblyInfo.cs

                Assert.AreEqual(commandPacketReturn.Response.ToString(), version, "Installer version does not match assembly version");
#endif

                writer.WriteJsonCommand(new CommandPacket
                {
                    Command = ServerCommands.TERMINATE
                });

                while (!appGenCommandHandler.Process.WaitForExit(1))
                {
                    Thread.Sleep(100);

                    if (stopwatch.ElapsedMilliseconds > 10000)
                    {
                        throw new TimeoutException("Timeout waiting for process to complete");
                    }
                }

                Assert.IsTrue(appGenCommandHandler.Process.ExitCode == 0);

                stopwatch.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("\t" + ex.Message);
                throw;
            }
        }
    }
}