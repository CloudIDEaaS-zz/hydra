using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.PackageCache
{
    public class PackageCacheManager : ILogWriter, IPackageCacheStatusProvider, IDisposable
    {
        private string root;
        private string packageCachePath;
        private LogWriter logWriter;
        private string nodeModulesPath;
        private PackageCacheService cacheService;
        private Process packageCacheStatusProcess;

        public PackageCacheManager(string root, string packageCachePath)
        {
            var process = Process.GetProcessesByName("PackageCacheStatus").SingleOrDefault();

            this.root = root;
            this.packageCachePath = packageCachePath;
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
            cacheService = new PackageCacheService(root, packageCachePath);

            cacheService.Start();
        }

        public void Dispose()
        {
            cacheService.Stop();

            if (!packageCacheStatusProcess.HasExited)
            {
                packageCacheStatusProcess.CloseMainWindow();
            }
        }

        public bool Handled(string mode, string install)
        {
            var regex = new Regex("(?<package>@?[^@]*?)($|@(?<version>.*$))");

            if (regex.IsMatch(install)) 
            {
                var match = regex.Match(install);
                var name = match.GetGroupValue("package");
                NpmVersion installVersion = match.GetGroupValue("version");
                var cachePath = Path.Combine(packageCachePath, name.BackSlashes());

                if (name.IsNullWhiteSpaceOrEmpty())
                {
                    DebugUtils.Break();
                }

                if (Directory.Exists(cachePath))
                {
                    var fileInfoCacheJson = new FileInfo(Path.Combine(cachePath.BackSlashes(), "package.json"));
                    var cacheDirectory = new DirectoryInfo(cachePath);
                    var packagePath = Path.Combine(nodeModulesPath, name.BackSlashes());
                    var packageDirectory = new DirectoryInfo(packagePath);

                    if (fileInfoCacheJson.Exists)
                    {
                        using (var readerCache = fileInfoCacheJson.OpenText())
                        {
                            var cacheJson = JsonExtensions.ReadJson<PackageJson>(readerCache);
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
                                            else if (cacheVersion > packageVersion)
                                            {
                                                return QueueInstallFromCache(mode, install, cacheDirectory, packageDirectory);
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
                                        else
                                        {
                                            return QueueInstallFromCache(mode, install, cacheDirectory, packageDirectory);
                                        }
                                    }
                                }
                                else
                                {
                                    return QueueInstallFromCache(mode, install, cacheDirectory, packageDirectory);
                                }
                            }
                        }
                    }
                    else
                    {
                        // cache package.json does not exist

                        return QueueInstallFromCache(mode, install, cacheDirectory, packageDirectory);
                    }
                }
                else
                {
                    // cache path does not exist
                }
            }
            else
            {
                DebugUtils.Break();
            }

            return false;
        }

        private bool QueueInstallFromCache(string mode, string install, DirectoryInfo cacheDirectory, DirectoryInfo packageDirectory)
        {
            return cacheService.QueueInstallFromCache(mode, install, cacheDirectory.FullName, packageDirectory.FullName);
        }

        public PackageCacheStatusInfo GetCacheStatus(string mode, bool setAsReported = false)
        {
            return cacheService.GetCacheStatus(mode, setAsReported);
        }

        public PackageInstallsFromCacheStatus GetInstallFromCacheStatus(string mode)
        {
            return cacheService.GetInstallFromCacheStatus(mode);
        }

        public string SetInstallStatus(string status)
        {
            return cacheService.SetInstallStatus(status);
        }

        internal void SetInstallCount(int count)
        {
            cacheService.SetInstallCount(count);
        }

        public void Write(string value)
        {
            logWriter.Write(value);
        }

        public void Write(string format, params object[] args)
        {
            logWriter.Write(format, args);
        }

        public void WriteLine(string value)
        {
            logWriter.WriteLine(value);
        }

        public void WriteLine(string format, params object[] args)
        {
            logWriter.WriteLine(format, args);
        }

        public void WriteLine()
        {
            logWriter.WriteLine();
        }
    }
}
