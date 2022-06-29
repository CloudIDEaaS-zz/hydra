// file:	PackageCache\PackageCacheManager.cs
//
// summary:	Implements the package cache manager class

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;
using AbstraX.PackageExtensions;
using Newtonsoft.Json.Serialization;
using System.Configuration;

namespace AbstraX.PackageCache
{
    /// <summary>   Manager for package caches. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

    public class PackageCacheManager : ILogWriter, IPackageCacheStatusProvider, IDisposable
    {
        /// <summary>   The root. </summary>
        private string root;
        /// <summary>   Full pathname of the package cache file. </summary>
        private string packageCachePath;
        private List<string> debugPackageInstalls;

        /// <summary>   The log writer. </summary>
        private LogWriter logWriter;
        /// <summary>   Full pathname of the node modules file. </summary>
        private string nodeModulesPath;
        /// <summary>   The cache service. </summary>
        private PackageCacheService cacheService;
        /// <summary>   The package cache status process. </summary>
        private Process packageCacheStatusProcess;
        private bool noCaching;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="root">                             The root. </param>
        /// <param name="packageCachePath">                 Full pathname of the package cache file. </param>
        /// <param name="installFromCacheReportDocument">   The install from cache report document. </param>
        /// <param name="installFromCacheReportFile">       The install from cache report file. </param>
        /// <param name="debugPackageInstalls">             The debug package installs. </param>

        public PackageCacheManager(string root, string packageCachePath, HtmlDocument installFromCacheReportDocument, string installFromCacheReportFile, List<string> debugPackageInstalls)
        {
            var process = Process.GetProcessesByName("PackageCacheStatus").SingleOrDefault();

            noCaching = ConfigurationSettings.AppSettings["CachePackages"].AsCaseless() == "false";

            if (!noCaching)
            {
                this.root = root;
                this.packageCachePath = packageCachePath;
                this.debugPackageInstalls = debugPackageInstalls;
                this.logWriter = new LogWriter(Path.Combine(packageCachePath, "Log.txt"), true);

                if (process != null)
                {
                    packageCacheStatusProcess = process;
                }
                else
                {
                    packageCacheStatusProcess = new Process();

                    packageCacheStatusProcess.StartInfo = new ProcessStartInfo
                    {
                        FileName = Path.Combine(packageCachePath, @"tools\PackageCacheStatus\PackageCacheStatus.exe")
                    };

                    packageCacheStatusProcess.Start();
                }

                nodeModulesPath = Path.Combine(root, "node_modules");
                cacheService = new PackageCacheService(root, packageCachePath, installFromCacheReportDocument, installFromCacheReportFile, this.debugPackageInstalls);

                cacheService.Start();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

        public void Dispose()
        {
            if (!noCaching)
            {
                cacheService.Stop();

                if (!packageCacheStatusProcess.HasExited)
                {
                    packageCacheStatusProcess.CloseMainWindow();
                }
            }
        }

        /// <summary>   Handled. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="mode">     The mode. </param>
        /// <param name="install">  The install. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool Handled(string mode, string install)
        {
            var regex = new Regex("(?<package>@?[^@]*?)($|@(?<version>.*$))");

            if (noCaching)
            {
                return false;
            }

            if (regex.IsMatch(install)) 
            {
                var match = regex.Match(install);
                var name = match.GetGroupValue("package");
                NpmVersion installVersion = match.GetGroupValue("version");
                var packageCacheDirectory = new DirectoryInfo(packageCachePath);

                if (name.IsNullWhiteSpaceOrEmpty())
                {
                    DebugUtils.Break();
                }
                else
                {
                    List<DirectoryInfo> cacheDirectories;
                    var parts = name.Split('/');
                    var firstPart = parts.First();
                    var directories = packageCacheDirectory.GetDirectories().Where(d => d.MatchesPackageName(firstPart)).ToArray();
                    var packagePath = Path.Combine(nodeModulesPath, name.BackSlashes());
                    var packageDirectory = new DirectoryInfo(packagePath);

                    if (directories.Length > 0)
                    {
                        if (parts.Length > 1)
                        {
                            var lastPart = parts.Last();

                            foreach (var part in parts.Skip(1).Take(parts.Length - 2))
                            {
                                foreach (var directory in directories.ToList())
                                {
                                    if (directory.GetDirectories().Any(d => d.Name == part))
                                    {
                                        directories = directory.GetDirectories();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                            }

                            foreach (var directory in directories.ToList())
                            {
                                if (directory.GetDirectories().Any(d => d.MatchesPackageName(lastPart)))
                                {
                                    directories = directory.GetDirectories().Where(d => d.MatchesPackageName(lastPart)).ToArray();
                                }
                                else
                                {
                                    break;
                                }
                            }

                            cacheDirectories = directories.ToList();
                        }
                        else
                        {
                            cacheDirectories = directories.ToList();
                        }

                        if (cacheDirectories.Count > 1)
                        {
                            
                        }

                        foreach (var cacheDirectory in cacheDirectories)
                        {
                            var fileInfoCacheJson = new FileInfo(Path.Combine(cacheDirectory.FullName, "package.json"));

                            if (fileInfoCacheJson.Exists)
                            {
                                try
                                {
                                    using (var readerCache = fileInfoCacheJson.OpenText())
                                    {
                                        var cacheJson = JsonExtensions.ReadJson<PackageJson>(readerCache, new CamelCaseNamingStrategy());
                                        var cacheName = cacheJson.Name;
                                        NpmVersion cacheVersion = cacheJson.Version;

                                        if (name != cacheName)
                                        {
                                            DebugUtils.Break();
                                        }

                                        // see if install matches whats in cache

                                        if (installVersion.Matches(cacheVersion))
                                        {
                                            if (Directory.Exists(packagePath))
                                            {
                                                var fileInfoPackageJson = new FileInfo(Path.Combine(packagePath, "package.json"));

                                                if (fileInfoPackageJson.Exists)
                                                {
                                                    using (var readerPackage = fileInfoPackageJson.OpenText())
                                                    {
                                                        var packageJson = JsonExtensions.ReadJson<PackageJson>(readerPackage);
                                                        var packageName = packageJson.Name;
                                                        var packageVersion = packageJson.Version;

                                                        if (cacheVersion == packageVersion)
                                                        {
                                                            return true;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    TimeSpan timeSpan;

                                                    if (cacheDirectory.CompareTo(packageDirectory, out timeSpan))
                                                    {
                                                        return true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                return QueueInstallFromCache(mode, install, cacheDirectory, packageDirectory);
                                            }
                                        }
                                        else
                                        {

                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }
            else
            {
                DebugUtils.Break();
            }

            return false;
        }

        /// <summary>   Queue install from cache. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="mode">             The mode. </param>
        /// <param name="install">          The install. </param>
        /// <param name="cacheDirectory">   Pathname of the cache directory. </param>
        /// <param name="packageDirectory"> Pathname of the package directory. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        private bool QueueInstallFromCache(string mode, string install, DirectoryInfo cacheDirectory, DirectoryInfo packageDirectory)
        {
            if (noCaching)
            {
                return false;
            }


            return cacheService.QueueInstallFromCache(mode, install, cacheDirectory.FullName, packageDirectory.FullName);
        }

        /// <summary>   Gets cache status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="mode">             The mode. </param>
        /// <param name="setAsReported">    (Optional) True if set as reported. </param>
        ///
        /// <returns>   The cache status. </returns>

        public PackageCacheStatusInfo GetCacheStatus(string mode, bool setAsReported = false)
        {
            if (noCaching)
            {
                return null;
            }

            return cacheService.GetCacheStatus(mode, setAsReported);
        }

        /// <summary>   Gets install from cache status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="mode"> The mode. </param>
        ///
        /// <returns>   The install from cache status. </returns>

        public PackageInstallsFromCacheStatus GetInstallFromCacheStatus(string mode)
        {
            if (noCaching)
            {
                return null;
            }

            return cacheService.GetInstallFromCacheStatus(mode);
        }

        /// <summary>   Sets install status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="status">   The status. </param>
        ///
        /// <returns>   A string. </returns>

        public string SetInstallStatus(string status)
        {
            if (noCaching)
            {
                return string.Empty;
            }

            return cacheService.SetInstallStatus(status);
        }

        /// <summary>   Sets install count. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="count">    Number of. </param>

        internal void SetInstallCount(int count)
        {
            if (noCaching)
            {
                return;
            }

            cacheService.SetInstallCount(count);
        }

        /// <summary>   Writes. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="value">    The value. </param>

        public void Write(string value)
        {
            logWriter.Write(value);
        }

        /// <summary>   Writes. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        public void Write(string format, params object[] args)
        {
            logWriter.Write(format, args);
        }

        /// <summary>   Writes the line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="value">    The value. </param>

        public void WriteLine(string value)
        {
            logWriter.WriteLine(value);
        }

        /// <summary>   Writes the line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        public void WriteLine(string format, params object[] args)
        {
            logWriter.WriteLine(format, args);
        }

        /// <summary>   Writes the line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

        public void WriteLine()
        {
            logWriter.WriteLine();
        }

        public IDisposable ErrorMode()
        {
            throw new NotImplementedException();
        }
    }
}
