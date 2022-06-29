using AbstraX.DataAnnotations;
using AbstraX.FolderStructure;
using AbstraX.XPathBuilder;
using CodePlex.XPathParser;
//using Ripley.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;
//using Db = Ripley.Entities;
using Utils.ProcessHelpers;
using System.Diagnostics;
using AbstraX.PackageCache;
using AbstraX.TypeMappings;
using Utils.NamedPipes;
using System.Threading;
using HydraDebugAssistant;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.ShellProperties;
using Microsoft.WindowsAPICodePack.Shell;
using System.Drawing;
using System.Drawing.Imaging;

namespace AbstraX
{ 
    public static class UnitTests
    {
        private static string testName = string.Empty;
        private static bool testWritingToConsole;

        private static string TestName
        {
            get
            {
                return testName;
            }

            set
            {
                Console.WriteLine("");
                Console.Write("Running {0}", value);

                testWritingToConsole = false;

                testName = value;
            }
        }

        private static string WriteToConsoleTestName
        {
            get
            {
                return testName;
            }

            set
            {
                Console.WriteLine("");
                Console.WriteLine("\tRunning {0}", value);

                testWritingToConsole = true;

                testName = value;
            }
        }

        /// <summary>   A string extension method that starts. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/18/2021. </remarks>
        ///
        /// <param name="testName">         The testName to act on. </param>
        /// <param name="writesToConsole">  (Optional) True to writes to console. </param>
        ///
        /// <returns>   An IDisposable. </returns>

        public static IDisposable Start(this string testName, bool writesToConsole = false)
        {
            if (writesToConsole)
            {
                WriteToConsoleTestName = testName;
                return testName.CreateDisposable(() => Console.WriteLine("\t{0} Completed", testName));
            }
            else
            {
                TestName = testName;
                return testName.CreateDisposable(() => Console.Write("\t... completed"));
            }
        }

        internal static void RunUnitTests()
        {
            try
            {
                ControlExtensions.ShowConsoleInSecondaryMonitor(FormWindowState.Normal);

                Console.WriteLine("Starting Unit Tests");

                // TestShortcuts
                {
                    using ("Test Shortcuts".Start(true))
                    {
                        TestShortcuts();
                    }
                }

                // FormatEscape
                {
                    using ("Test FormatEscape".Start(true))
                    {
                        TestFormatEscape();
                    }
                }

                // LockingProcesses
                {
                    using ("Test LockingProcesses".Start(true))
                    {
                        TestLockingProcesses();
                    }
                }

                // IterationStateEnumerable
                {
                    using ("Test IterationStateEnumerable".Start(true))
                    {
                        IterationStateEnumerable();
                    }
                }

                // Shell Properties
                {
                    using ("Test ShellProperties".Start(true))
                    {
                        GetShellProperties();
                        TestShellProperties();
                    }
                }

                // Folder comparison tests
                {
                    using ("Test FolderComparision".Start(true))
                    {
                        TestDynamicTemplateDebugAssistant();
                    }
                }

                // Folder comparison tests
                {
                    using ("Test FolderComparision".Start(true))
                    {
                        TestFolderComparison();
                    }
                }

                // PackageCacheManager tests
                {
                    using ("Test PackageCacheManager".Start(true))
                    {
                        TestPackageCacheManager();
                    }
                }

                // XPathParser tests
                {
                    using ("Test XPathParser".Start(true))
                    {
                        TestXPathParser();
                    }
                }

                // ModuleImports Creation tests
                {
                    using ("Test ModuleImports Creation".Start(true))
                    {
                        TestModuleImportsCreation();
                    }
                }

                // JSON tests
                {
                    using ("Test Folder To JSON".Start(true))
                    {
                        TestFolderToJson();
                    }
                }

                // navigation property test
                {
                    using ("Test Navigation Properties".Start(true))
                    {
                        //TestNavigationProperties();
                    }
                }

                // LoadParentPathAttribute test
                {
                    using ("Test Queries".Start(true))
                    {
                        // TestQueries();
                    }
                }

                // LoadParentPathAttribute test
                {
                    using ("Test Web Proxy".Start(true))
                    {
                        // TestWebProxy();
                    }
                }

                // TestFileLocks test
                {
                    using ("Test File Locks".Start(true))
                    {
                        // TestFileLocks();
                    }
                }

                // GetDecimalSeconds test
                {
                    using ("Get Decimal Seconds".Start(true))
                    {
                        TestGetDecimalSeconds();
                    }
                }

                // MemoryUsageForCurrentProcess test
                {
                    using ("Test Memory Usage".Start(true))
                    {
                        TestMemoryUsage();
                    }
                }

                // SemVer test
                {
                    using ("Test SemVer".Start(true))
                    {
                        TestSemVer();
                    }
                }

                // DotNetType mappings test
                {
                    using ("Test DotNetType".Start(true))
                    {
                        TestDotNetType();
                    }
                }

                // PackageWorkingInstallFromCache test
                {
                    using ("Test PackageWorkingInstallFromCache".Start(true))
                    {
                        // TestPackageWorkingInstallFromCache();
                    }
                }

                // JSON cloning test
                {
                    using ("Test JSON cloning".Start(true))
                    {
                        //TestJsonCloning();
                    }
                }
            }
            catch (Exception ex)
            {
                var hwndConsole = ControlExtensions.GetConsoleWindow();

                Console.WriteLine("UnitTest: '{0}' failed.  Exception: {1}", TestName, ex.Message);
                ControlExtensions.Flash(hwndConsole, FlashWindowFlags.FLASHW_ALL | FlashWindowFlags.FLASHW_TIMERNOFG, 0, 1000);
                return;
            }

            Console.WriteLine("\r\nUnit Tests Completed Successfully!\r\n");
        }

        private static void TestShortcuts()
        {
            var fileInfo = new FileInfo(@"d:\Users\Ken\Documents\HydraProjects\BlankApp1\hydra.json");
            var result = fileInfo.CreateShortcut(fileInfo.DirectoryName);
        }

        private static void TestFormatEscape()
        {
            var str = "Hello";

            str = str.FormatEscape();

            str = "He{llo}";

            str = str.FormatEscape();

            str = "He{{llo}}";

            str = str.FormatEscape();
        }

        private static void TestLockingProcesses()
        {
            var file = @"D:\MC\CloudIDEaaS\root\ApplicationGenerator\TestOutput\contoso.Web\contoso\.git\objects\pack";
            var frmLockingProcesses = new frmLockingProcesses();

            Debugger.Launch();

            frmLockingProcesses.LockedPath = file;

            frmLockingProcesses.ShowDialog();
        }

        private static void IterationStateEnumerable()
        {
            var cars = new[] { "Audi", "BMW", "Toyota" }.WithState();

            foreach (var car in cars)
            {

            }
        }

        private static void TestShellProperties()
        {
            var fileInfoImage = new FileInfo(@"D:\MC\CloudIDEaaS\root\ApplicationGenerator\TestOutput\contoso.Web\contoso\src\assets\img\about\contoso-inc-contoso-about.jpg");
            Bitmap bitmapImage;
            Utils.FileDetails fileDetails;
            string copyImage;

            if (!fileInfoImage.Exists)
            {
                return;
            }

            copyImage = Path.Combine(fileInfoImage.DirectoryName,  "~" + fileInfoImage.Name);
            
            fileInfoImage.CopyTo(copyImage);

            // following adds Exif info to the file to further processing
            // see https://en.wikipedia.org/wiki/Exif

            using (bitmapImage = (Bitmap)Image.FromFile(copyImage))
            {
                bitmapImage.SetMetaValue(MetaProperty.Author, "Me");
                bitmapImage.Save(fileInfoImage.FullName, ImageFormat.Jpeg);
            }

            System.IO.File.Delete(copyImage);

            fileDetails = new FileDetails(fileInfoImage);

            fileDetails.SetGeneralAuthorProperty("Us");
            fileDetails.SetGeneralRatingProperty((uint) 90);
            fileDetails.SetGeneralTitleProperty("End User License Image");
            fileDetails.SetGeneralCommentProperty("These are my comments");
            fileDetails.SetGeneralCopyrightProperty("Copyright 2021");
            fileDetails.SetGeneralKeywordsProperty(new[] { "Licenses", "EULA" });
        }

        private static void GetShellProperties()
        {
            var type = typeof(PropertySystem);
            var builder = new StringBuilder();
            var builder2 = new StringBuilder();

            builder.AppendLineTabIndent(1, "public static class ShellProperties");
            builder.AppendLineTabIndent(1, "{");

            builder.AppendLineTabIndent(2, "public enum GeneralProperties");
            builder.AppendLineTabIndent(2, "{");

            foreach (var property in type.GetProperties().Where(p => p.PropertyType.Name.StartsWith("ShellProperty")))
            {
                var propertyType = property.PropertyType.GenericTypeArguments[0];
                var propertyTypeName = propertyType.GetCodeDeclaration();

                builder.AppendLineFormatTabIndent(3, "[FilePropertyDataType(typeof({0}))]", propertyTypeName);
                builder.AppendLineFormatTabIndent(3, "{0},", property.Name);

                builder2.AppendLineFormatTabIndent(2, "public static void Set{0}{1}Property(this FileDetails fileDetails, {2} value)", "General", property.Name, propertyTypeName);
                builder2.AppendLineTabIndent(2, "{");
                builder2.AppendLineFormatTabIndent(3, "fileDetails[\"{0}\"] = value;", property.Name);
                builder2.AppendLineTabIndent(2, "}\r\n");
            }

            builder.AppendLineTabIndent(2, "}");

            foreach (var property in type.GetProperties().Where(p => !p.PropertyType.Name.StartsWith("ShellProperty")))
            {
                var propertyType = property.PropertyType;
                var metadataTypeName = propertyType.GetCodeDeclaration();

                builder.AppendLineFormatTabIndent(2, "public enum {0}Properties", propertyType.Name.RemoveStartIfMatches("PropertySystem"));
                builder.AppendLineTabIndent(2, "{");

                foreach (var property2 in propertyType.GetProperties().Where(p => p.PropertyType.Name.StartsWith("ShellProperty")))
                {
                    var propertyType2 = property2.PropertyType.GenericTypeArguments[0];
                    var propertyTypeName = propertyType2.GetCodeDeclaration();

                    builder.AppendLineFormatTabIndent(3, "[FilePropertyDataType(typeof({0}), typeof({1}))]", metadataTypeName, propertyTypeName);
                    builder.AppendLineFormatTabIndent(3, "{0},", property2.Name);

                    builder2.AppendLineFormatTabIndent(2, "public static void Set{0}{1}Property(this FileDetails fileDetails, {2} value)", property.Name, property2.Name, propertyTypeName);
                    builder2.AppendLineTabIndent(2, "{");
                    builder2.AppendLineFormatTabIndent(3, "fileDetails[\"{0}/{1}\"] = value;", property.Name, property2.Name);
                    builder2.AppendLineTabIndent(2, "}\r\n");
                }

                builder.AppendLineTabIndent(2, "}");
           }

            builder.AppendLineTabIndent(1, "}");
        }

        private static void TestDynamicTemplateDebugAssistant()
        {
            var connected = false;
            NamedPipeClient client = null;
            PipeMessageEventHandler serverMessage = null;
            PipeExceptionEventHandler pipeError = null;
            var templateFile = @"D:\MC\CloudIDEaaS\root\ApplicationGenerator\bin\Debug\Generators\Server\WebAPIContainerController\WebAPIContainerControllerClassTemplate.tt";

            var connectClient = new Action(() =>
            {
                var address = "pipe://localhost/hydradebugassistant";

                try
                {
                    client = new NamedPipeClient(address);
                    client.Connect();

                    connected = true;
                }
                catch (Exception ex)
                {
                    connected = false;
                }
            });

            var postProcess = new Func<PostProcessResult>(() =>
            {
                if (connected)
                {
                    var continuePolling = true;
                    var postProcessResult = PostProcessResult.None;
                    var handlingEvents = false;
                    var loopCount = 0;
                    var announcedPause = false;
                    var resetEvent = new ManualResetEvent(false);

                    while (continuePolling)
                    {
                        resetEvent.Reset();

                        serverMessage = (sender, e) =>
                        {
                            var response = (string)e.Command;

                            switch (response)
                            {
                                case Commands.BreakpointSet:

                                    if (!announcedPause)
                                    {
                                        Console.WriteLine("HydraDebugAssistant paused on breakpoint");
                                        announcedPause = true;
                                    }

                                    continuePolling = true;
                                    break;

                                case Commands.BreakpointNotSet:

                                    continuePolling = false;
                                    postProcessResult = PostProcessResult.Continue;
                                    break;

                                case Commands.RetryAndBreak:

                                    Console.WriteLine("HydraDebugAssistant retry and break");

                                    continuePolling = false;
                                    postProcessResult = PostProcessResult.RedoGenerate;
                                    break;

                                case Commands.Continue:

                                    continuePolling = false;
                                    postProcessResult = PostProcessResult.Continue;
                                    break;
                            }

                            resetEvent.Set();
                        };

                        pipeError = (s, e) =>
                        {
                            Console.WriteLine("Disconnected from HydraDebugAssistant, Error: {0}", e.Message);

                            resetEvent.Set();
                        };

                        if (!handlingEvents)
                        {
                            client.ServerMessage += serverMessage;
                            client.PipeError += pipeError;

                            handlingEvents = true;
                        }

                        client.Send(new CommandPacket("IsBreakpointSet", new KeyValuePair<string, object>("TemplateFile", templateFile)));

                        while (!resetEvent.WaitOne(100))
                        {
                            Application.DoEvents();
                        }

                        loopCount++;
                        Console.WriteLine(loopCount);
                    }

                    client.ServerMessage -= serverMessage;
                    client.PipeError -= pipeError;

                    return postProcessResult;
                }

                return PostProcessResult.Continue;
            });

            connectClient();

            do
            {
                Thread.Sleep(1000);
            }
            while (postProcess() == PostProcessResult.RedoGenerate);

            client.Dispose();
        }

        private static void TestFolderComparison()
        {
            var packageCachePath = @"C:\Users\Ken\AppData\Roaming\hydra\cache";
            var nodeModulesPath = @"D:\MC\CloudIDEaaS\root\ApplicationGenerator\TestOutput\contoso.Web\contoso\node_modules";
            var modulesDirectory = new DirectoryInfo(nodeModulesPath);
            var cacheDirectory = new DirectoryInfo(packageCachePath);
            var dateOfLastPassProcess = DateTime.MinValue;
            var modulesSubDirectories = modulesDirectory.GetDirectories();
            var cacheSubDirectories = cacheDirectory.GetDirectories();
            var shouldRun = false;

            foreach (var modulesSubDirectory in modulesSubDirectories)
            {
                var cacheSubDirectory = cacheSubDirectories.SingleOrDefault(d => d.Name == modulesSubDirectory.Name);

                if (cacheSubDirectory == null)
                {
                    shouldRun = true;
                }
                else if (cacheSubDirectory.GetComparisonScore(modulesSubDirectory) < .10)
                {
                    shouldRun = true;
                }
                else
                {
                    shouldRun = false;
                }
            }
        }

        private static void TestPackageCacheManager()
        {
            var packageCacheManager = new PackageCacheManager(@"D:\MC\CloudIDEaaS\root\ApplicationGenerator\TestOutput\contoso.Web\contoso", @"C:\Users\Ken\AppData\Roaming\hydra\cache", null, null, null);

            packageCacheManager.Handled("installs", "@types/node@^14.14.21");
            packageCacheManager.Handled("installs", "@types/jasmine@^3.6.2");
        }

        private static void TestJsonCloning()
        {
            // 1
            {
                var json = typeof(UnitTests).ReadResource<string>(@"Tests\Test1.json");
                var jsonObject = JsonExtensions.ReadJson<dynamic>(json);
                var objClone = ((object)jsonObject).CloneJson();
            }

            // 2
            {
                var json = typeof(UnitTests).ReadResource<string>(@"Tests\Test2.json");
                var jsonObject = JsonExtensions.ReadJson<dynamic>(json);
                var objClone = ((object)jsonObject).CloneJson();
            }

            // 3
            {
                var json = typeof(UnitTests).ReadResource<string>(@"Tests\Test3.json");
                var jsonObject = JsonExtensions.ReadJson<dynamic>(json);
                var objClone = ((object)jsonObject).CloneJson();
            }

        }

        private static void TestDotNetType()
        {
            //var xmlType = TypeMapper.GetXMLType("Integer");
            //var type = xmlType.GetDotNetType();
        }

        private static void TestPackageWorkingInstallFromCache()
        {
            var name = "ionic";
            var folderRoot = Environment.CurrentDirectory;
            var nodeModulesPath = Path.GetFullPath(Path.Combine(folderRoot, @"..\..\TestOutput\AngularProject\node_modules"));
            var cachePath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\hydra\cache\" + name);
            var packagePath = Path.Combine(nodeModulesPath, name.BackSlashes());
            var packageModules = new NpmNodeModules(nodeModulesPath);
            var workingInstallFromCache = new PackageWorkingInstallFromCache("installs", name, cachePath, packagePath, null, packageModules);
            var installsFromCacheStatus = new PackageInstallsFromCacheStatus("No activity reported");
            string printTree;
            bool validate;

            TestPeerExists();

            workingInstallFromCache.OnUpdateCacheStatus += (sender, e) =>
            {
                switch (e.IncrementKind)
                {
                    case IncrementKind.IncrementAll:

                        Debug.Assert(e.Increment == 1);
                        installsFromCacheStatus.IncrementAll();
                        break;

                    case IncrementKind.IncrementTotal:

                        Debug.Assert(e.Increment == 1);
                        installsFromCacheStatus.IncrementTotal();
                        break;

                    case IncrementKind.TotalRemaining:

                        installsFromCacheStatus.TotalRemaining += e.Increment;
                        break;

                    case IncrementKind.Requested:

                        installsFromCacheStatus.Requested += e.Increment;
                        break;

                    case IncrementKind.RequestedRemaining:

                        installsFromCacheStatus.RequestedRemaining += e.Increment;

                        if (installsFromCacheStatus.RequestedRemaining < 0)
                        {
                            DebugUtils.Break();
                        }

                        break;
                }
            };

            workingInstallFromCache.OnAddInstallStatus += (sender, e) =>
            {
                var installFromCache = e.InstallFromCache;
                var mode = e.StatusMode;
                var status = e.Status;
                var args = e.Args;

                if (args.Length > 0)
                {
                    status = string.Format(status, args);
                }

                installFromCache.InstallStatusList.Add(installFromCache.CreateStatus(status, mode));
            };

            workingInstallFromCache.InstallPackage(false);

            validate = workingInstallFromCache.ValidateTree(installsFromCacheStatus);
            printTree = workingInstallFromCache.PrintTree();

            workingInstallFromCache.ExecuteActions();

            Console.WriteLine(installsFromCacheStatus.StatusText);
            Console.WriteLine();
            Console.WriteLine(printTree);
        }

        private static void TestPeerExists()
        {
            var name = "@ionic/angular";
            var folderRoot = Environment.CurrentDirectory;
            var packageNodeModulesPath = Path.GetFullPath(Path.Combine(folderRoot, @"..\..\TestOutput\AngularProject\node_modules"));
            var packageNodeModulesDirectory = new DirectoryInfo(packageNodeModulesPath);
            var cacheRoot = Environment.ExpandEnvironmentVariables(@"%APPDATA%\hydra\cache");
            var cacheRootDirectory = new DirectoryInfo(cacheRoot);
            NpmNodeModules npmPackageModules = null;

            if (packageNodeModulesDirectory.Exists)
            {
                var packagePath = Path.Combine(packageNodeModulesPath, name.BackSlashes());

                npmPackageModules = new NpmNodeModules(packageNodeModulesPath);
            }

            if (cacheRootDirectory.Exists)
            {
                var npmCacheModules = new NpmNodeModules(cacheRoot);
                var cachePath = Path.Combine(cacheRoot, name.BackSlashes());
                var cacheDirectory = new DirectoryInfo(cachePath);

                if (cacheDirectory.Exists)
                {
                    if (NpmPackage.HasPackage(cachePath))
                    {
                        var npmCachePackage = new NpmPackage(cachePath);
                        bool packagePeerExists;
                        bool cachePeerExists;

                        npmCachePackage.Load();

                        if (npmPackageModules != null)
                        {
                            packagePeerExists = npmCachePackage.PeerDependencies.PeerExists(npmPackageModules);
                        }

                        cachePeerExists = npmCachePackage.PeerDependencies.PeerExists(npmCacheModules);
                    }
                }
            }
        }

        private static void TestSemVer()
        {
            NpmVersion npmVersion1;
            NpmVersion npmVersion2;

            /*
            public enum NpmVersionComparison
            {
                NoComparison = 0,
                Equals = 1 << 2,
                GreaterThan = 1 << 3,
                LessThan = 1 << 4,
                Matches = 1 << 5
            }
            */

            /*** comparison *************************************/

            npmVersion1 = "1.7.249";
            npmVersion2 = "1.7.249";

            Debug.Assert(NpmVersion.GetComparison(npmVersion1, npmVersion2).HasFlag(NpmVersionComparison.Equals));
            Debug.Assert(NpmVersion.GetComparison(npmVersion1, npmVersion2).HasFlag(NpmVersionComparison.Matches));
            Debug.Assert(npmVersion1 == npmVersion2);
            Debug.Assert(npmVersion1 <= npmVersion2);
            Debug.Assert(npmVersion1 >= npmVersion2);

            npmVersion1 = "1.7.248";
            npmVersion2 = "1.7.249";

            Debug.Assert(NpmVersion.GetComparison(npmVersion1, npmVersion2).HasFlag(NpmVersionComparison.LessThan));
            Debug.Assert(npmVersion1 < npmVersion2);
            Debug.Assert(npmVersion1 <= npmVersion2);

            npmVersion1 = "1.7.24";
            npmVersion2 = "1.7.25";

            Debug.Assert(NpmVersion.GetComparison(npmVersion1, npmVersion2).HasFlag(NpmVersionComparison.LessThan));
            Debug.Assert(npmVersion1 < npmVersion2);
            Debug.Assert(npmVersion1 <= npmVersion2);

            npmVersion1 = "1.7.249";
            npmVersion2 = "1.7.248";

            Debug.Assert(NpmVersion.GetComparison(npmVersion1, npmVersion2).HasFlag(NpmVersionComparison.GreaterThan));
            Debug.Assert(npmVersion1 > npmVersion2);
            Debug.Assert(npmVersion1 >= npmVersion2);

            npmVersion1 = "1.7";
            npmVersion2 = "1.7.249";

            Debug.Assert(NpmVersion.GetComparison(npmVersion1, npmVersion2).HasFlag(NpmVersionComparison.LessThan));
            Debug.Assert(npmVersion1 < npmVersion2);

            npmVersion1 = "1.7.249";
            npmVersion2 = "1.7";

            Debug.Assert(NpmVersion.GetComparison(npmVersion1, npmVersion2).HasFlag(NpmVersionComparison.GreaterThan));
            Debug.Assert(npmVersion1 > npmVersion2);

            npmVersion1 = "1.7";
            npmVersion2 = "1.7.249";

            Debug.Assert(NpmVersion.GetComparison(npmVersion1, npmVersion2).HasFlag(NpmVersionComparison.LessThan));
            Debug.Assert(npmVersion1 < npmVersion2);

            npmVersion1 = "1.7";
            npmVersion2 = "1.03";

            Debug.Assert(NpmVersion.GetComparison(npmVersion1, npmVersion2).HasFlag(NpmVersionComparison.GreaterThan));
            Debug.Assert(npmVersion1 > npmVersion2);

            npmVersion1 = "1.7";
            npmVersion2 = "1";

            Debug.Assert(NpmVersion.GetComparison(npmVersion1, npmVersion2).HasFlag(NpmVersionComparison.GreaterThan));
            Debug.Assert(npmVersion1 > npmVersion2);

            npmVersion1 = "1";
            npmVersion2 = "1.7";

            Debug.Assert(NpmVersion.GetComparison(npmVersion1, npmVersion2).HasFlag(NpmVersionComparison.LessThan));
            Debug.Assert(npmVersion1 < npmVersion2);

            npmVersion1 = null;
            npmVersion2 = "1.8";

            Debug.Assert(npmVersion1 != npmVersion2);

            /*** greater than match *************************************/

            // positive test 

            npmVersion1 = ">1";
            npmVersion2 = "1.7";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">1";
            npmVersion2 = "1.7.249";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">1.6";
            npmVersion2 = "1.7.249";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">1.7.1";
            npmVersion2 = "1.7.249";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            // negative test 

            npmVersion1 = ">2";
            npmVersion2 = "1.7";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">2";
            npmVersion2 = "1.7.249";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">1.7";
            npmVersion2 = "1.6.249";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">1.7.2";
            npmVersion2 = "1.7.1";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">2.5.2";
            npmVersion2 = "1.7.149";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">1.7.149";
            npmVersion2 = "1.7.149";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">1.7";
            npmVersion2 = "1.7";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">2";
            npmVersion2 = "2";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            /*** less than match *************************************/

            // positive test 

            npmVersion1 = "<2";
            npmVersion2 = "1.7";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<1.7.249";
            npmVersion2 = "1";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<1.8";
            npmVersion2 = "1.7.249";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<1.7.249";
            npmVersion2 = "1.7.1";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            // negative test 

            npmVersion1 = "<1";
            npmVersion2 = "1.7";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<1.7.249";
            npmVersion2 = "2";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<1.6.249";
            npmVersion2 = "1.7";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<1.7.1";
            npmVersion2 = "1.7.2";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<1.5.2";
            npmVersion2 = "1.7.149";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<1.7.149";
            npmVersion2 = "1.7.149";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<1.7";
            npmVersion2 = "1.7";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<2";
            npmVersion2 = "2";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            /*** greater than or equals match *************************************/

            // positive test 

            npmVersion1 = ">=1";
            npmVersion2 = "1.7";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">=1";
            npmVersion2 = "1.7.249";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">=1.6";
            npmVersion2 = "1.7.249";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">=1.7.1";
            npmVersion2 = "1.7.249";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">=1.7.149";
            npmVersion2 = "1.7.149";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">=1.7";
            npmVersion2 = "1.7";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">=2";
            npmVersion2 = "2";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            // negative test 

            npmVersion1 = ">=2";
            npmVersion2 = "1.7";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">=2";
            npmVersion2 = "1.7.249";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">=1.7";
            npmVersion2 = "1.6.249";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">=1.7.2";
            npmVersion2 = "1.7.1";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = ">=2.5.2";
            npmVersion2 = "1.7.149";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            /*** less than or equals match *************************************/

            // positive test 

            npmVersion1 = "<=2";
            npmVersion2 = "1.7";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<=1.7.249";
            npmVersion2 = "1";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<=1.8";
            npmVersion2 = "1.7.249";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<=1.7.249";
            npmVersion2 = "1.7.1";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<=1.7.149";
            npmVersion2 = "1.7.149";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<=1.7";
            npmVersion2 = "1.7";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<=2";
            npmVersion2 = "2";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            // negative test 

            npmVersion1 = "<=1";
            npmVersion2 = "1.7";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<=1.7.249";
            npmVersion2 = "2";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<=1.6.249";
            npmVersion2 = "1.7";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<=1.7.1";
            npmVersion2 = "1.7.2";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "<=1.5.2";
            npmVersion2 = "1.7.149";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            /*** version range match *************************************/

            // positive test 

            npmVersion1 = "1.5 - 1.8";
            npmVersion2 = "1.7";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1 - 2";
            npmVersion2 = "1.7";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.5.2 - 1.8";
            npmVersion2 = "1.7.149";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.5 - 1.8";
            npmVersion2 = "1.7.149";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1-2";
            npmVersion2 = "1.7.149";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.7.149  -   1.7.150";
            npmVersion2 = "1.7.149";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            // negative test 

            npmVersion1 = "1.5 - 1.8";
            npmVersion2 = "1.0";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "2 - 4";
            npmVersion2 = "1.7";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.5.2 - 1.8";
            npmVersion2 = "1.8.149";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.5 - 1.8";
            npmVersion2 = "1.4.149";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1-2";
            npmVersion2 = "2.7.149";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.7.149  -   1.7.150";
            npmVersion2 = "1.7.148";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.7.149  -   1.7.150";
            npmVersion2 = "1.7.151";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            /*** x-range match *************************************/

            // positive test 

            npmVersion1 = "*";
            npmVersion2 = "1.7";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.*";
            npmVersion2 = "1.7";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.x.2";
            npmVersion2 = "1.7.2";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.7.X";
            npmVersion2 = "1.7.149";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.x";
            npmVersion2 = "1.7";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "x";
            npmVersion2 = "1.7";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.X.2";
            npmVersion2 = "1.7.2";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.7.*";
            npmVersion2 = "1.7.149";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.x.x";
            npmVersion2 = "1.7.149";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "x.7.x";
            npmVersion2 = "1.7.149";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            // negative test 

            npmVersion1 = "2.*";
            npmVersion2 = "1.7";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.x.2";
            npmVersion2 = "1.7.3";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.7.X";
            npmVersion2 = "2.7.149";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.x";
            npmVersion2 = "2";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.X.3";
            npmVersion2 = "1.7.2";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.7.*";
            npmVersion2 = "2.7.149";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "1.x.x";
            npmVersion2 = "2.7.149";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "x.7.x";
            npmVersion2 = "1.6.149";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            /*** tilde match *************************************/

            // positive test 

            npmVersion1 = "~1.7.249";
            npmVersion2 = "1.7.249";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "~1.7.247";
            npmVersion2 = "1.7.248";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "~1.7.24";
            npmVersion2 = "1.7.26";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "~1.7.4";
            npmVersion2 = "1.7.8";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "~1.7.2";
            npmVersion2 = "1.7.3";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            // negative test

            npmVersion1 = "~1.7.249";
            npmVersion2 = "1.7.248";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "~1.7.249";
            npmVersion2 = "1.7";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "~1.7.249";
            npmVersion2 = "1.8.26";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "~1.7.249";
            npmVersion2 = "2.7.249";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            /*** caret match *************************************/

            // positive test 

            npmVersion1 = "^1.7.249";
            npmVersion2 = "1.7.249";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "^1.7.247";
            npmVersion2 = "1.8.246";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "^1.7";
            npmVersion2 = "1.8.26";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "^1.7";
            npmVersion2 = "1.9";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            npmVersion1 = "^1.7.2";
            npmVersion2 = "1.7.3";

            Debug.Assert(npmVersion1.Matches(npmVersion2));

            // negative test

            npmVersion1 = "^1.7.249";
            npmVersion2 = "2.0";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "^1.7.249";
            npmVersion2 = "1.6";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "^1.7.249";
            npmVersion2 = "^1.7.247";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));

            npmVersion1 = "^1.7.249";
            npmVersion2 = "2.7.249";

            Debug.Assert(!npmVersion1.Matches(npmVersion2));
        }

        private static void TestMemoryUsage()
        {
            var result = SystemInfo.MemoryUsageForCurrentProcess;
            var memoryStatus = MemoryStatus.Create();
        }

        private static void TestGetDecimalSeconds()
        {
            var timeSpan = new TimeSpan(0, 0, 0, 0, 300);
            var seconds = timeSpan.GetDecimalTimeComponent((t) => t.Milliseconds, 2);
        }

        private static void TestFileLocks()
        {
            var file = new FileInfo(@"C:\Users\Ken\AppData\Roaming\hydra\cache\Log.txt");
            var directory = file.Directory;
            var filter = new Func<Process, bool>(p => p.ProcessName.AsCaseless() == "ApplicationGenerator");
            var processes = file.FindLockingProcesses(filter);
            var count = processes.Count();

            Console.WriteLine("{0}File locks with no open files: '{1}'", '\t'.Repeat(2), count);

            using (var stream = file.OpenWrite())
            {
                var process = Process.GetProcesses().Single(filter);
                var openFiles = process.GetOpenFiles().Where(f => f.LocalPath != null && f.LocalPath.Contains("hydra"));

                processes = file.FindLockingProcesses(filter);
                count = processes.Count();

                Console.WriteLine("{0}File locks with 1 open file: '{1}'", '\t'.Repeat(2), count);
            }

            processes = directory.FindLockingProcesses(filter);
            count = processes.Count();

            Console.WriteLine("{0}Directory locks with no open files: '{1}'", '\t'.Repeat(2), count);

            using (var stream = file.OpenWrite())
            {
                processes = directory.FindLockingProcesses(filter);
                count = processes.Count();

                Console.WriteLine("{0}Directory with 1 open file: '{1}'", '\t'.Repeat(2), count);
            }
        }

        //private static void TestQueries()
        //{
        //    string expression;

        //    var queries = new AppResources().GetQueries();
        //    var query = queries.Single(q => q.ServiceControllerMethodName == "GetPublisherForCurrentUser");
        //    var queryCode = query.QueryExpression.Replace(new Dictionary<string, string> { { "containerVariable", "entities" }, { "identityNameVariable", "userName" } });

        //    expression = "/Users[@Name=\"John\"]";
        //    Console.WriteLine("{0}Parsing: '{1}'", '\t'.Repeat(2), expression);

        //    new QueryPathAttribute(expression, QueryKind.LoadParentReference);

        //    expression = "/Users[last()]";
        //    Console.WriteLine("{0}Parsing: '{1}'", '\t'.Repeat(2), expression);

        //    new QueryPathAttribute(expression);

        //    expression = "/Users[@UserName=identity_name()]/UserToPublishers/Publisher";
        //    Console.WriteLine("{0}Parsing: '{1}'", '\t'.Repeat(2), expression);

        //    new QueryPathAttribute(expression);
        //}

        //private static void TestNavigationProperties()
        //{
        //    var entities = new Db.RipleyEntities();
        //    var id = Guid.Parse("6c507edc-8845-4591-80b6-34bf1dc6045a");

        //    if (entities.LessonAuthorizedForUser(id))
        //    {
        //        var lesson = entities.Lessons.Single(l => l.LessonId == id);
        //        var user = lesson.Course.Publisher.UserToPublishers.Single().User;
        //    }
        //}

        //private static bool LessonAuthorizedForUser(this Db.RipleyEntities entities, Guid id)
        //{
        //    if (entities.Database.SqlQuery<bool>("EXISTS()").Single())
        //    {
        //        var lesson = entities.Lessons.Single(l => l.LessonId == id);
        //        var user = lesson.Course.Publisher.UserToPublishers.Single().User;
        //    }

        //    return false;
        //}

        private static void TestFolderToJson()
        {
            var folder = new Folder(null, "/");
            var packet = new CommandPacket("", DateTime.Now, folder);
        }

        public static void TestModuleImportsCreation()
        {
            var packageGuids = new Dictionary<object, Guid>();
            var packages = new List<object>();
            var helper = new ModuleImportsHelper.Helper();
            var creationObjects = new List<object>()
            {
                new { NgxTranslate = new { Modules = new List<string> { "TranslateService" }, ImportPath = "@ngx-translate/core" }},
                new { Angular = new { Modules = new List<string> { "Component" }, ImportPath = "@angular/core" }},
                new { Ionic = new { Modules = new List<string> { "Page", "NavController" }, ImportPath = "ionic-angular", CombineName = "IONIC_BASIC_PAGE_IMPORTS", AddFrom = "NgxTranslate" }},
                new { CombineName = "IONIC_ANGULAR_BASIC_PAGE_IMPORTS", AddFrom = "Ionic, Angular, NgxTranslate" }
            };

            helper.AddPackages(creationObjects);
        }

        private static void TestXPathParser()
        {
            var parser = new XPathParser<string>();
            var builder = new XPathStringBuilder();
            Queue<IXPathPart> queue;

            parser.Parse("/contosoAdministrator[@Name='contoso Administrator' and @RoleView='Administrators']", builder);

            queue = builder.PartQueue;

            foreach (var part in queue)
            {
                var debugInfo = part.ToString();
            }

            foreach (var parts in queue.SplitElementParts())
            {

            }

            parser.Parse("/ManageUsers[@Users]", builder);

            queue = builder.PartQueue;

            foreach (var part in queue)
            {
                var debugInfo = part.ToString();
            }

            foreach (var parts in queue.SplitElementParts())
            {

            }

            parser.Parse("/ManageUsers[@Name='Manage Users']", builder);

            queue = builder.PartQueue;

            foreach (var part in queue)
            {
                var debugInfo = part.ToString();
            }

            foreach (var parts in queue.SplitElementParts())
            {

            }

            parser.Parse("/ManageUsers[@Name='Manage Users' and @Users]", builder);

            queue = builder.PartQueue;

            foreach (var part in queue)
            {
                var debugInfo = part.ToString();
            }

            foreach (var parts in queue.SplitElementParts())
            {

            }

            //parser = new XPathParser<string>();
            //builder = new XPathStringBuilder();

            //parser.Parse("/Person[@FirstName]", builder);

            //queue = builder.PartQueue;

            //foreach (var part in queue)
            //{
            //    var debugInfo = part.ToString();
            //}

            //parser = new XPathParser<string>();
            //builder = new XPathStringBuilder();

            //parser.Parse("$ABCCompany/Person[@FirstName]", builder);

            //queue = builder.PartQueue;

            //foreach (var part in queue)
            //{
            //    var debugInfo = part.ToString();
            //}
        }
    }
}
