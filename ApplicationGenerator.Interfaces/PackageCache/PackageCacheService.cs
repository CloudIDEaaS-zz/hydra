using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;
using Utils.ProcessHelpers;

namespace AbstraX.PackageCache
{
    public class PackageCacheService : BaseThreadedService, IPackageCacheStatusProvider, ILogWriter  
    {
        private string root;
        private string packageCachePath;
        private string nodeModulesPath;
        private LogWriter logWriter;
        private string sweepsPath;
        private FileSystemWatcher fileSystemWatcher;
        private List<string> pathsToProcess;
        private List<string> pathsToNotProcess;
        private Dictionary<string, PackageWorkingInstallFromCache> installsFromCacheToProcess;
        private Dictionary<string, List<PathCacheStatus>> pathsProcessed;
        private IManagedLockObject lockObject;
        private MemoryStatus memoryStatus;
        private CacheStatusType cacheStatusType;
        private InstallStatusType installStatusType;
        private DateTime lastCacheStatusRequest;
        private DateTime lastSweepStart;
        private int sweepIndex;
        private int sweepCount;
        private string lastCacheStatusMode;
        private const int PATH_ERROR_LIMIT = 5;
        private Stack<CacheStatusType> statusStack;
        private string currentActionVerb;
        private PackageInstallsFromCacheStatus installsFromCacheStatus;
        private LogWriter sweepingLogWriter;
        private string lastInstallsFromCacheStatusSummary;
        private NpmNodeModules packageModules;
        private const bool NO_CACHING = true;
        private const bool NO_INSTALL_FROM_CACHE = true;
        public bool NothingToPoll { get; set; }
        public int InstallCount { get; private set; }

        public PackageCacheService(string root, string packageCachePath) : base(ThreadPriority.Lowest, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMinutes(10), TimeSpan.FromSeconds(15))
        {
            this.root = root;
            this.packageCachePath = packageCachePath;
            this.logWriter = new LogWriter(Path.Combine(this.packageCachePath, "Log.txt"));

            sweepsPath = Path.Combine(packageCachePath, "sweeps");
            nodeModulesPath = Path.Combine(root, "node_modules");
            pathsToProcess = new List<string>();
            pathsToNotProcess = new List<string>();
            pathsProcessed = new Dictionary<string, List<PathCacheStatus>>();
            installsFromCacheToProcess = new Dictionary<string, PackageWorkingInstallFromCache>();
            statusStack = new Stack<CacheStatusType>();
            cacheStatusType = CacheStatusType.Initialized;
            lockObject = LockManager.CreateObject();
            memoryStatus = MemoryStatus.Create();
            currentActionVerb = "No current action";
            packageModules = new NpmNodeModules(nodeModulesPath);

            this.NothingToPoll = true;

            if (!Directory.Exists(nodeModulesPath))
            {
                Directory.CreateDirectory(nodeModulesPath);
            }

            fileSystemWatcher = new FileSystemWatcher(nodeModulesPath);
            fileSystemWatcher.EnableRaisingEvents = true;

            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void DoWork(bool stopping)
        {
            Process();
        }

        public PackageInstallsFromCacheStatus GetInstallFromCacheStatus(string mode)
        {
            if (installsFromCacheStatus == null)
            {
                installsFromCacheStatus = new PackageInstallsFromCacheStatus("No activity reported");
            }
            else if (lastInstallsFromCacheStatusSummary == installsFromCacheStatus.StatusSummary)
            {
                installsFromCacheStatus.StatusSummary = null;
                installsFromCacheStatus.StatusIsError = false;
                installsFromCacheStatus.StatusIsSuccess = false;
            }

            installsFromCacheStatus.AddInstalls(installsFromCacheToProcess, DateTime.Now);

            installsFromCacheStatus.NothingToPoll = this.NothingToPoll;
            lastInstallsFromCacheStatusSummary = installsFromCacheStatus.StatusSummary;

            return installsFromCacheStatus;
        }

        public bool QueueInstallFromCache(string mode, string install, string cachePath, string packagePath)
        {
            using (lockObject.Lock())
            {
                if (NO_INSTALL_FROM_CACHE)
                {
                    return false;
                }
                else
                {
                    if (!installsFromCacheToProcess.ContainsKey(install))
                    {
                        var workingInstallFromCache = new PackageWorkingInstallFromCache(mode, install, cachePath, packagePath, packageModules);

                        if (installsFromCacheStatus == null)
                        {
                            installsFromCacheStatus = new PackageInstallsFromCacheStatus("Queueing installs from cache");
                        }

                        if (!workingInstallFromCache.IsMissingPeer)
                        {
                            workingInstallFromCache.OnUpdateCacheStatus += WorkingInstallFromCache_OnUpdateCacheStatus;
                            workingInstallFromCache.OnAddInstallStatus += WorkingInstallFromCache_OnAddInstallStatus;

                            this.NothingToPoll = false;

                            this.WriteLineNoLock("Adding '{0}' to install from cache", install);

                            installsFromCacheToProcess.AddToDictionaryIfNotExist(install, workingInstallFromCache);
                        }
                    }
                }

                return true;
            }
        }

        private void WorkingInstallFromCache_OnAddInstallStatus(object sender, AddInstallStatusEventArgs e)
        {
            this.AddInstallStatus(e.InstallFromCache, e.StatusMode, e.Status, e.Args);
        }

        private void WorkingInstallFromCache_OnUpdateCacheStatus(object sender, CacheStatusEventArgs e)
        {
            using (lockObject.Lock())
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
            }
        }

        private bool InstallFromCache(PackageWorkingInstallFromCache installFromCache)
        {
            var mode = installFromCache.Mode;
            var install = installFromCache.Install;
            var packageDirectory = new DirectoryInfo(installFromCache.PackagePath);
            var path = packageDirectory.FullName.RemoveStart(nodeModulesPath + @"\");

            try
            {
                var elapsed = TimeSpan.MinValue;

                SkipPath(packageDirectory.Name);
                AddInstallStatus(installFromCache, StatusMode.Normal, $"Installing '{ install }' from cache");

                using (this.StartStopwatch((s) => elapsed = s))
                {
                    installFromCache.InstallPackage();

                    AddPathStatus(path, CacheStatusType.PathProcessed);
                }

                AddInstallStatus(installFromCache, StatusMode.Success, "Install of '{0}' from cache took {1} seconds", path, elapsed.ToDecimalSeconds());
                AddPackageCacheStatus(path, install, CacheStatusType.PathCopiedFromCache);
            }
            catch (Exception ex)
            {
                AddInstallStatus(installFromCache, StatusMode.Error, "Error installing '{0}', ", ex.Message);
                AddPackageCacheStatus(path, install, ex);

                DebugUtils.Break();
                return false;
            }

            return true;
        }

        private void AddInstallStatus(PackageWorkingInstallFromCache installFromCache, StatusMode mode, string status, params object[] args)
        {
            using (lockObject.Lock())
            {
                if (args.Length > 0)
                {
                    status = string.Format(status, args);
                }

                installFromCache.InstallStatusList.Add(installFromCache.CreateStatus(status, mode));
            }
        }

        private void Process()
        {
            List<string> paths = null;
            Dictionary<string, PackageWorkingInstallFromCache> installs = null;
            var foldersCopied = 0;
            var foldersWithErrors = 0;
            var foldersSkipped = 0;
            var skipped = false;
            var stopwatch = new Stopwatch();

            using (lockObject.Lock())
            {
                paths = pathsToProcess.ToList();
                installs = installsFromCacheToProcess.ToDictionary();

                if (cacheStatusType < CacheStatusType.ServiceStarted)
                {
                    cacheStatusType = CacheStatusType.ServiceStarted;
                }
            }

            if (installs != null)
            {
                if (installs.Count > 0)
                {
                    using (lockObject.Lock())
                    {
                        if (cacheStatusType < CacheStatusType.Installing)
                        {
                            cacheStatusType = CacheStatusType.Installing;
                            installsFromCacheStatus.StatusSummary = "Installing from cache";
                        }
                    }

                    foreach (var install in installs.Values)
                    {
                        if (this.InstallFromCache(install))
                        {
                            using (lockObject.Lock())
                            {
                                installsFromCacheToProcess.Remove(install.Install);
                            }
                        }

                        Thread.Sleep(1);
                    }
                }
            }

            if (paths != null)
            {
                if (paths.Count > 0)
                {
                    using (lockObject.Lock())
                    {
                        if (cacheStatusType < CacheStatusType.Processing)
                        {
                            cacheStatusType = CacheStatusType.Processing;
                        }
                    }

                    using (this.PushSetStatus(CacheStatusType.SweepingFiles))
                    {
                        stopwatch.Start();

                        using (lockObject.Lock())
                        {
                            sweepIndex = 0;
                            sweepCount = paths.Count;
                            lastSweepStart = DateTime.Now;

                            if (paths.Count == 1)
                            {
                                this.WriteSweepLine("Sweeping {0} directory {1}", paths.Count, "*".Repeat(100));
                            }
                            else
                            {
                                this.WriteSweepLine("Sweeping {0} directories {1}", paths.Count, "*".Repeat(100));
                            }
                        }

                        foreach (var path in paths)
                        {
                            try
                            {
                                var packagePath = Path.Combine(nodeModulesPath, path);
                                var cachePath = Path.Combine(packageCachePath, path);
                                var cacheDirectory = new DirectoryInfo(cachePath);
                                var packageDirectory = new DirectoryInfo(packagePath);

                                this.WriteSweepLine("Processing '{0}'", path);

                                using (lockObject.Lock())
                                {
                                    sweepIndex++;
                                }

                                if (!Directory.Exists(cachePath))
                                {
                                    cacheDirectory.Create();

                                    this.WriteSweepLine("Syncing directories for '{0}'", path);
                                    SyncDirectories(path, cacheDirectory, packageDirectory, ref skipped);

                                    if (skipped)
                                    {
                                        foldersSkipped++;
                                    }
                                    else
                                    {
                                        foldersCopied++;
                                    }
                                }
                                else
                                {
                                    TimeSpan timeSpan;

                                    if (cacheDirectory.CompareTo(packageDirectory, out timeSpan))
                                    {
                                        AddPathStatus(path, CacheStatusType.PathProcessed);

                                        using (lockObject.Lock())
                                        {
                                            this.WriteSweepLine("Cache directory for '{0}' up to date.  Comparison took {1} seconds", path, Math.Round(stopwatch.Elapsed.ToDecimalSeconds()));
                                            pathsToProcess.Remove(path);

                                            foldersSkipped++;
                                        }
                                    }
                                    else
                                    {
                                        if (CheckVersions(cachePath, packagePath))
                                        {
                                            this.WriteSweepLine("Syncing directories for '{0}'.  Comparison took {1} seconds", path, Math.Round(stopwatch.Elapsed.ToDecimalSeconds()));
                                            SyncDirectories(path, cacheDirectory, packageDirectory, ref skipped);

                                            if (skipped)
                                            {
                                                foldersSkipped++;
                                            }
                                            else
                                            {
                                                foldersCopied++;
                                            }
                                        }
                                        else
                                        {
                                            foldersSkipped++;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                AddPathStatus(path, ex);

                                this.WriteSweepLine("Error for '{0}'.  Exception: \r\n{1}", path, ex.Message);

                                if (PackageCacheStatusInfo.GetPathErrorCount(path, pathsProcessed) > PATH_ERROR_LIMIT)
                                {
                                    using (lockObject.Lock())
                                    {
                                        this.WriteSweepLine("'{0}' reached error limit of {1}.  Removing from list", path, PATH_ERROR_LIMIT);
                                        foldersWithErrors++;

                                        pathsToProcess.Remove(path);
                                    }
                                }
                                else
                                {
                                    foldersSkipped++;
                                }
                            }

                            Thread.Sleep(1);
                        }

                        stopwatch.Stop();

                        this.WriteSweepLine("End of sweep. Copied: {0}, Errored: {1}, Skipped: {2}, Sweep time elapsed: {3} seconds", foldersCopied, foldersWithErrors, foldersSkipped, Math.Round(stopwatch.Elapsed.ToDecimalSeconds()));
                    }

                    using (lockObject.Lock())
                    {
                        cacheStatusType = CacheStatusType.EndOfProcessing;
                    }
                }
            }
        }

        private bool CheckVersions(string cachePath, string packagePath)
        {
            var fileInfoCacheJson = new FileInfo(Path.Combine(cachePath.BackSlashes(), "package.json"));
            var cacheDirectory = new DirectoryInfo(cachePath);
            var packageDirectory = new DirectoryInfo(packagePath);

            if (fileInfoCacheJson.Exists)
            {
                using (var readerCache = fileInfoCacheJson.OpenText())
                {
                    var cacheJson = JsonExtensions.ReadJson<PackageJson>(readerCache);
                    var cacheName = cacheJson.Name;
                    NpmVersion cacheVersion = cacheJson.Version;

                    if (Directory.Exists(packagePath))
                    {
                        var fileInfoPackageJson = new FileInfo(Path.Combine(packagePath, "package.json"));

                        if (fileInfoPackageJson.Exists)
                        {
                            using (var readerPackage = fileInfoPackageJson.OpenText())
                            {
                                var packageJson = JsonExtensions.ReadJson<PackageJson>(readerPackage);
                                var packageName = packageJson.Name;
                                var packageVersion = new NpmVersion(packageJson.Version);

                                if (cacheVersion == packageVersion)
                                {
                                    return true;
                                }
                                else if (cacheVersion < packageVersion)
                                {
                                    this.WriteSweepLine("Cached version for '{0}{1}' less than package version '{2}'. Skipping cache.", packageName, cacheVersion.VersionString, packageVersion.VersionString);

                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public IDisposable PushSetStatus(CacheStatusType statusType)
        {
            using (lockObject.Lock())
            {
                statusStack.Push(this.cacheStatusType);

                this.cacheStatusType = statusType;

                if (statusType == CacheStatusType.SweepingFiles)
                {
                    if (!Directory.Exists(sweepsPath))
                    {
                        Directory.CreateDirectory(sweepsPath);
                    }

                    sweepingLogWriter = new LogWriter(Path.Combine(sweepsPath, DateTime.Now.ToSortableDateTimeText() + ".txt"));
                }
            }

            return statusStack.AsDisposable(() =>
            {
                PopStatus();
            });
        }

        public void PopStatus()
        {
            using (lockObject.Lock())
            {
                sweepingLogWriter = null;
                this.cacheStatusType = statusStack.Pop();
            }
        }

        public void SkipPath(string path)
        {
            using (lockObject.Lock())
            {
                this.WriteLineNoLock("Adding '{0}' to paths to not process", path);
                pathsToNotProcess.Add(path);
            }
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                var path = e.FullPath.RemoveStart(nodeModulesPath + @"\");

                if (NO_CACHING)
                {
                    return;
                }

                if (path != ".staging")
                {
                    using (lockObject.Lock())
                    {
                        if (cacheStatusType < CacheStatusType.AddingPathsToList)
                        {
                            cacheStatusType = CacheStatusType.AddingPathsToList;
                        }

                        if (!pathsToProcess.Contains(path) && !pathsToNotProcess.Contains(path))
                        {
                            this.WriteLineNoLock("Adding '{0}' to paths to process", path);
                            pathsToProcess.Add(path);

                            this.AddPathStatusNoLock(path, CacheStatusType.PathAddedToList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugUtils.Break();
            }
        }

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                var path = e.FullPath.RemoveStart(nodeModulesPath + @"\");

                if (NO_CACHING)
                {
                    return;
                }

                if (path != ".staging")
                {
                    using (lockObject.Lock())
                    {
                        if (cacheStatusType < CacheStatusType.AddingPathsToList)
                        {
                            cacheStatusType = CacheStatusType.AddingPathsToList;
                        }

                        if (!pathsToProcess.Contains(path) && !pathsToNotProcess.Contains(path))
                        {
                            this.WriteLineNoLock("Adding '{0}' to paths to process", path);
                            pathsToProcess.Add(path);

                            this.AddPathStatusNoLock(path, CacheStatusType.PathAddedToList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugUtils.Break();
            }
        }

        public void LastPassProcess()
        {
            var modulesDirectory = new DirectoryInfo(nodeModulesPath);

            using (lockObject.Lock())
            {
                var paths = modulesDirectory.GetDirectories().Select(d => d.FullName.RemoveStart(nodeModulesPath + @"\")).Where(p => !pathsToNotProcess.Contains(p)).ToList();

                if (paths.Count > 0)
                {
                    this.WriteLineNoLock("\r\nPerforming last pass process on the following paths:\r\n");
                    this.WriteLineNoLock(paths.ToMultiLineList());
                    this.WriteLineNoLock();

                    cacheStatusType = CacheStatusType.LastPassProcessing;

                    pathsToProcess.AddRange(paths);
                }
            }
        }

        public override void Stop()
        {
            var count = 1;
            var modulesDirectory = new DirectoryInfo(nodeModulesPath);
            var lastCount = 0;

            this.WriteLine("Request to stop service. Waiting on {0} paths to finish processing", count);

            while (count > 0)
            {
                using (lockObject.Lock())
                {
                    count = pathsToProcess.Count;

                    if (cacheStatusType < CacheStatusType.ServiceStopping)
                    {
                        cacheStatusType = CacheStatusType.ServiceStopping;
                    }

                    if (lastCount != count)
                    {
                        this.WriteLineNoLock("Waiting on {0} paths to finish processing", count);
                    }

                    lastCount = count;
                }

                Thread.Sleep(1);
            }

            using (lockObject.Lock())
            {
                cacheStatusType = CacheStatusType.ServiceStopping;
            }

            this.WriteLine("Stopping service");

            base.Stop();

            using (lockObject.Lock())
            {
                cacheStatusType = CacheStatusType.ServiceStopped;
            }

            this.WriteLine("Service stopped");
        }

        private bool AssureNoOpen(DirectoryInfo directory, out TimeSpan timeSpan)
        {
            //var elapsed = TimeSpan.MinValue;
            //bool result;

            //using (this.StartStopwatch((t) => elapsed = t))
            //{
            //    var processes = directory.FindLockingProcesses(p => p.ProcessName == "node");

            //    if (processes.Count() > 0)
            //    {
            //        result = false;
            //    }
            //}

            //timeSpan = elapsed;
            //result = true;

            //return result;

            timeSpan = TimeSpan.FromMilliseconds(0);

            return true;
        }

        private void SyncDirectories(string path, DirectoryInfo cacheDirectory, DirectoryInfo packageDirectory, ref bool skipped)
        {
            TimeSpan timeSpan;

            if (AssureNoOpen(packageDirectory, out timeSpan))
            {
                var elapsed = TimeSpan.MinValue;

                this.WriteLine("Copying '{0}' to '{1}", path, cacheDirectory.FullName);

                AddPathStatus(path, CacheStatusType.PathProcessing);

                using (this.StartStopwatch((s) => elapsed = s))
                {
                    packageDirectory.CopyTo(cacheDirectory.FullName, true, true);

                    AddPathStatus(path, CacheStatusType.PathProcessed);
                }

                this.WriteLine("Copy of '{0}' took {1} seconds", path, elapsed.ToDecimalSeconds());

                using (lockObject.Lock())
                {
                    pathsToProcess.Remove(path);
                }
            }
            else
            {
                this.WriteLine("Skipping '{0}'.  Files still open", path);
                skipped = true;
            }

            this.WriteLine("No open check took {0} seconds", timeSpan.ToDecimalSeconds());
        }

        public string SetInstallStatus(string status)
        {
            using (lockObject.Lock())
            {
                this.installStatusType = EnumUtils.GetValue<InstallStatusType>(status);

                if (this.installStatusType == InstallStatusType.InstallsStarted)
                {
                    if (this.InstallCount > 0)
                    {
                        currentActionVerb = "Caching";

                        this.cacheStatusType = CacheStatusType.WatchingForPackages;
                    }
                }
                else if (this.installStatusType == InstallStatusType.Finalized)
                {
                    currentActionVerb = "Last pass syncing";

                    this.LastPassProcess();
                    NothingToPoll = true;
                }

                return $"Install status set to { this.installStatusType.ToString() }";
            }
        }

        public void SetInstallCount(int count)
        {
            using (lockObject.Lock())
            {
                if (count == 0)
                {
                    this.cacheStatusType = CacheStatusType.EndOfProcessing;
                }

                this.InstallCount = count;
            }
        }

        public PackageCacheStatusInfo GetCacheStatus(string mode, bool setAsReported = false)
        {
            PackageCacheStatusInfo cacheStatus;

            using (lockObject.Lock())
            {
                cacheStatus = new PackageCacheStatusInfo(this.cacheStatusType, this.currentActionVerb, this.installStatusType, sweepIndex, sweepCount, lastSweepStart, lastCacheStatusRequest, pathsProcessed, memoryStatus);

                if (mode != "Agent")
                {
                    cacheStatus = new PackageCacheStatusInfo(cacheStatus.CacheStatus, cacheStatus.StatusText);
                }

                cacheStatus.NoCaching = NO_CACHING;
                cacheStatus.NoInstallFromCache = NO_INSTALL_FROM_CACHE;

                if (setAsReported)
                {
                    lastCacheStatusRequest = DateTime.Now;
                    lastCacheStatusMode = mode;

                    cacheStatus.SetLastStatusReported(pathsProcessed, lastCacheStatusRequest);
                }
            }

            return cacheStatus;
        }

        public void AddPackageCacheStatus(string path, string install, CacheStatusType type)
        {
            using (lockObject.Lock())
            {
                pathsProcessed.AddToDictionaryListCreateIfNotExist(path, new PathCacheStatus(path, type));
            }
        }

        public void AddPackageCacheStatus(string path, string install, Exception exception)
        {
            using (lockObject.Lock())
            {
                pathsProcessed.AddToDictionaryListCreateIfNotExist(path, new PathCacheStatus(path, exception));
            }
        }

        public void AddPathStatus(string path, CacheStatusType type)
        {
            using (lockObject.Lock())
            {
                var pathCacheStatus = new PathCacheStatus(path, type);

                if (type == CacheStatusType.PathProcessed)
                {
                    if (pathsProcessed.ContainsKey(path))
                    {
                        if (pathsProcessed[path].Any(p => path != p.Path && path.StartsWith(p.Path)))
                        {
                            pathCacheStatus.IsSubPath = true;
                        }
                    }
                }

                pathsProcessed.AddToDictionaryListCreateIfNotExist(path, pathCacheStatus);
            }
        }

        public void AddPathStatus(string path, CacheStatusType type, string message)
        {
            using (lockObject.Lock())
            {
                pathsProcessed.AddToDictionaryListCreateIfNotExist(path, new PathCacheStatus(path, type, message));
            }
        }

        public void AddPathStatus(string path, Exception exception)
        {
            using (lockObject.Lock())
            {
                pathsProcessed.AddToDictionaryListCreateIfNotExist(path, new PathCacheStatus(path, exception));
            }
        }

        public void AddPathStatusNoLock(string path, CacheStatusType type)
        {
            pathsProcessed.AddToDictionaryListCreateIfNotExist(path, new PathCacheStatus(path, type));
        }

        public void AddPathStatusNoLock(string path, CacheStatusType type, string message)
        {
            pathsProcessed.AddToDictionaryListCreateIfNotExist(path, new PathCacheStatus(path, type, message));
        }

        public void AddPathStatusNoLock(string path, Exception exception)
        {
            pathsProcessed.AddToDictionaryListCreateIfNotExist(path, new PathCacheStatus(path, exception));
        }

        public void Write(string value)
        {
            using (lockObject.Lock())
            {
                logWriter.Write(value);
            }
        }

        public void Write(string format, params object[] args)
        {
            using (lockObject.Lock())
            {
                logWriter.Write(format, args);
            }
        }

        public void WriteLine(string value)
        {
            using (lockObject.Lock())
            {
                logWriter.WriteLine(value);
            }
        }

        public void WriteSweepLine(string format, params object[] args)
        {
            using (lockObject.Lock())
            {
                logWriter.WriteLine(format, args);

                if (sweepingLogWriter != null)
                {
                    sweepingLogWriter.WriteLine(format, args);
                }
            }
        }

        public void WriteLine(string format, params object[] args)
        {
            using (lockObject.Lock())
            {
                logWriter.WriteLine(format, args);
            }
        }

        public void WriteLine()
        {
            using (lockObject.Lock())
            {
                logWriter.WriteLine();
            }
        }

        public void WriteNoLock(string format, params object[] args)
        {
            logWriter.Write(format, args);
        }

        public void WriteLineNoLock(string value)
        {
            logWriter.WriteLine(value);
        }

        public void WriteLineNoLock(string format, params object[] args)
        {
            logWriter.WriteLine(format, args);
        }

        public void WriteLineNoLock()
        {
            logWriter.WriteLine();
        }
    }
}
