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
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;
using Utils;
//using Db = Ripley.Entities;
using Utils.ProcessHelpers;
using System.Diagnostics;
using AbstraX.PackageCache;
using AbstraX.TypeMappings;

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

        public static IDisposable Start(this string testName, bool writesToConsole = false)
        {
            if (writesToConsole)
            {
                WriteToConsoleTestName = testName;
                return testName.AsDisposable(() => Console.WriteLine("\t{0} Completed", testName));
            }
            else
            {
                TestName = testName;
                return testName.AsDisposable(() => Console.Write("\t... completed"));
            }
        }

        internal static void RunUnitTests()
        {
            try
            {
                ControlExtensions.ShowConsoleInSecondaryMonitor(FormWindowState.Normal);

                Console.WriteLine("Starting Unit Tests");

                // XPathParser tests
                {
                    using ("Test XPathParser".Start(true))
                    {
                        //TestXPathParser();
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
                        TestJsonCloning();
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
            var xmlType = TypeMapper.GetXMLType("Integer");
            var type = xmlType.GetDotNetType();
        }

        private static void TestPackageWorkingInstallFromCache()
        {
            var name = "ionic";
            var folderRoot = Environment.CurrentDirectory;
            var nodeModulesPath = Path.GetFullPath(Path.Combine(folderRoot, @"..\..\TestOutput\AngularProject\node_modules"));
            var cachePath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\hydra\cache\" + name);
            var packagePath = Path.Combine(nodeModulesPath, name.BackSlashes());
            var packageModules = new NpmNodeModules(nodeModulesPath);
            var workingInstallFromCache = new PackageWorkingInstallFromCache("installs", name, cachePath, packagePath, packageModules);
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

                    case IncrementKind.Total:

                        installsFromCacheStatus.Total += e.Increment;
                        break;

                    case IncrementKind.TotalRemaining:

                        installsFromCacheStatus.TotalRemaining += e.Increment;
                        break;

                    case IncrementKind.Requested:

                        installsFromCacheStatus.Requested += e.Increment;
                        break;

                    case IncrementKind.RequestedRemaining:

                        installsFromCacheStatus.RequestedRemaining += e.Increment;
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

        private static void TestWebProxy()
        {
            var proxyServer = new ProxyServer();
            var hostAddresses = Dns.GetHostAddresses("registry.npmjs.org");

            //locally trust root certificate used by this proxy 
            proxyServer.CertificateManager.TrustRootCertificate(true);

            //optionally set the Certificate Engine
            //Under Mono only BouncyCastle will be supported
            //proxyServer.CertificateManager.CertificateEngine = Network.CertificateEngine.BouncyCastle;

            proxyServer.BeforeRequest += OnRequest;
            proxyServer.BeforeResponse += OnResponse;


            var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8000, true)
            {
                //Use self-issued generic certificate on all https requests
                //Optimizes performance by not creating a certificate for each https-enabled domain
                //Useful when certificate trust is not required by proxy clients
                //GenericCertificate = new X509Certificate2(Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "genericcert.pfx"), "password")
            };

            //Fired when a CONNECT request is received
            //explicitEndPoint.BeforeTunnelConnect += OnBeforeTunnelConnect;

            //An explicit endpoint is where the client knows about the existence of a proxy
            //So client sends request in a proxy friendly manner
            proxyServer.AddEndPoint(explicitEndPoint);
            proxyServer.Start();

            ////Transparent endpoint is useful for reverse proxy (client is not aware of the existence of proxy)
            ////A transparent endpoint usually requires a network router port forwarding HTTP(S) packets or DNS
            ////to send data to this endPoint
            //var transparentEndPoint = new TransparentProxyEndPoint(IPAddress.Any, 8001, true)
            //{
            //    //Generic Certificate hostname to use
            //    //when SNI is disabled by client
            //    GenericCertificateName = "google.com"
            //};

            //proxyServer.AddEndPoint(transparentEndPoint);

            //proxyServer.UpStreamHttpProxy = new ExternalProxy() { HostName = "localhost", Port = 8888 };
            //proxyServer.UpStreamHttpsProxy = new ExternalProxy() { HostName = "localhost", Port = 8888 };

            foreach (var endPoint in proxyServer.ProxyEndPoints)
                Console.WriteLine("Listening on '{0}' endpoint at Ip {1} and port: {2} ",
                    endPoint.GetType().Name, endPoint.IpAddress, endPoint.Port);

            //Only explicit proxies can be set as system proxy!
            proxyServer.SetAsSystemHttpProxy(explicitEndPoint);
            proxyServer.SetAsSystemHttpsProxy(explicitEndPoint);

            //wait here (You can use something else as a wait function, I am using this as a demo)
            Console.Read();

            //Unsubscribe & Quit
            // explicitEndPoint.BeforeTunnelConnect -= OnBeforeTunnelConnect;
            proxyServer.BeforeRequest -= OnRequest;
            proxyServer.BeforeResponse -= OnResponse;
            proxyServer.ServerCertificateValidationCallback -= OnCertificateValidation;
            proxyServer.ClientCertificateSelectionCallback -= OnCertificateSelection;

            proxyServer.Stop();
        }

        private async static Task OnCertificateSelection(object sender, CertificateSelectionEventArgs e)
        {
        }

        private async static Task OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
        }

        private async static Task OnResponse(object sender, SessionEventArgs e)
        {
            Console.WriteLine(e.HttpClient.Request.Url);

            if (e.HttpClient.Response.StatusCode == 200)
            {
                var body = await e.GetResponseBodyAsString();

                if (e.HttpClient.Request.Url.StartsWith("http://registry.npmjs.org/"))
                {
                }

                e.SetResponseBodyString(body);
            }
        }

        private async static Task OnRequest(object sender, SessionEventArgs e)
        {
            if (e.HttpClient.Request.Url.StartsWith("http://registry.npmjs.org/"))
            {
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

            parser.Parse("/Person[@FirstName]", builder);

            queue = builder.PartQueue;

            foreach (var part in queue)
            {
                var debugInfo = part.ToString();
            }

            parser = new XPathParser<string>();
            builder = new XPathStringBuilder();

            parser.Parse("$ABCCompany/Person[@FirstName]", builder);

            queue = builder.PartQueue;

            foreach (var part in queue)
            {
                var debugInfo = part.ToString();
            }
        }
    }
}
