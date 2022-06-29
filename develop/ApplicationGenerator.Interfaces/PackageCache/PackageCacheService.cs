// file:	PackageCache\PackageCacheService.cs
//
// summary:	Implements the package cache service class

using HtmlAgilityPack;
using MailSlot;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;
using Utils.ProcessHelpers;
using AbstraX.PackageExtensions;

namespace AbstraX.PackageCache
{
    /// <summary>   A service for accessing package caches information. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

    [System.Runtime.InteropServices.Guid("D4362F7E-0989-424E-935C-2A1F33F6822C")]
    public class PackageCacheService : BaseThreadedService, IPackageCacheStatusProvider, ILogWriter
    {
        private string packageJsonFile;
        private RegistrySettings registrySettings;

        /// <summary>   The root. </summary>
        private string root;
        /// <summary>   Full pathname of the package cache file. </summary>
        private string packageCachePath;
        /// <summary>   Full pathname of the node modules file. </summary>
        private string nodeModulesPath;
        /// <summary>   The log writer. </summary>
        private LogWriter logWriter;
        private LogWriter installsFromCacheStatusLogWriter;

        /// <summary>   Full pathname of the sweeps file. </summary>
        private string sweepsPath;
        /// <summary>   The file system watcher. </summary>
        private FileSystemWatcher fileSystemWatcher;
        /// <summary>   The paths to process. </summary>
        private List<string> pathsToProcess;
        /// <summary>   The paths to not process. </summary>
        private List<string> pathsToNotProcess;
        /// <summary>   The installs from cache to process. </summary>
        private Dictionary<string, PackageWorkingInstallFromCache> installsFromCacheToProcess;
        /// <summary>   The paths processed. </summary>
        private Dictionary<string, List<PathCacheStatus>> pathsProcessed;
        /// <summary>   The lock object. </summary>
        private IManagedLockObject lockObject;
        /// <summary>   The memory status. </summary>
        private MemoryStatus memoryStatus;
        /// <summary>   Type of the cache status. </summary>
        private CacheStatusType cacheStatusType;
        /// <summary>   Type of the install status. </summary>
        private InstallStatusType installStatusType;
        /// <summary>   The last cache status request Date/Time. </summary>
        private DateTime lastCacheStatusRequest;
        /// <summary>   The last sweep start Date/Time. </summary>
        private DateTime lastSweepStart;
        /// <summary>   Zero-based index of the sweep. </summary>
        private int sweepIndex;
        /// <summary>   Number of sweeps. </summary>
        private int sweepCount;
        /// <summary>   The last cache status mode. </summary>
        private string lastCacheStatusMode;
        /// <summary>   The path error limit. </summary>
        private const int PATH_ERROR_LIMIT = 5;
        /// <summary>   Stack of status. </summary>
        private Stack<CacheStatusType> statusStack;
        /// <summary>   The current action verb. </summary>
        private string currentActionVerb;
        /// <summary>   The installs from cache status. </summary>
        private PackageInstallsFromCacheStatus installsFromCacheStatus;
        /// <summary>   The sweeping log writer. </summary>
        private LogWriter sweepingLogWriter;
        /// <summary>   The last installs from cache status summary. </summary>
        private string lastInstallsFromCacheStatusSummary;
        /// <summary>   The package modules. </summary>
        private NpmNodeModules packageModules;
        /// <summary>   True to no caching. </summary>
        private bool noCaching = true;
        /// <summary>   True to no install from cache. </summary>
        private bool noInstallFromCache = true;
        private int statusProgressPercent;
        private MailslotClient mailslotClient;
        private int postInstalls;
        private int preInstalls;
        private HtmlDocument installFromCacheReportDocument;
        private string installFromCacheReportFile;
        private List<string> debugPackageInstalls;
        private HtmlNode installFromCacheReportContainerElement;
        private HtmlNode installFromCacheReportTabsElement;
        private HtmlNode installFromCacheCurrentContentElement;

        /// <summary>   Gets or sets a value indicating whether the nothing to poll. </summary>
        ///
        /// <value> True if nothing to poll, false if not. </value>

        public bool NothingToPoll { get; set; }

        /// <summary>   Gets or sets the number of installs. </summary>
        ///
        /// <value> The number of installs. </value>

        public int InstallCount { get; private set; }

        /// <summary>   Gets or sets the folders skipped. </summary>
        ///
        /// <value> The folders skipped. </value>

        public int FoldersSkipped { get; private set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="root">                             The root. </param>
        /// <param name="packageCachePath">                 Full pathname of the package cache file. </param>
        /// <param name="installFromCacheReportDocument">   The install from cache report document. </param>
        /// <param name="installFromCacheReportFile">       The install from cache report file. </param>
        /// <param name="debugPackageInstalls">             The debug package installs. </param>

        public PackageCacheService(string root, string packageCachePath, HtmlDocument installFromCacheReportDocument, string installFromCacheReportFile, List<string> debugPackageInstalls) : base(ThreadPriority.Lowest, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMinutes(10), TimeSpan.FromSeconds(15))
        {
            var mailslotName = "HydraInstallFromCacheStatus";
            
            registrySettings = new RegistrySettings();

            registrySettings.Initialize();

            this.root = root;
            this.packageCachePath = packageCachePath;
            this.logWriter = new LogWriter(Path.Combine(this.packageCachePath, "Log.txt"));
            this.installsFromCacheStatusLogWriter = new LogWriter(Path.Combine(this.packageCachePath, "InstallsFromCache.log"), true);
            this.installFromCacheReportDocument = installFromCacheReportDocument;
            this.installFromCacheReportFile = installFromCacheReportFile;
            this.debugPackageInstalls = debugPackageInstalls;

            noCaching = ConfigurationSettings.AppSettings["CachePackages"].AsCaseless() == "false";
            noInstallFromCache = ConfigurationSettings.AppSettings["InstallPackagesFromCache"].AsCaseless() == "false";

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
            packageJsonFile = Path.Combine(root, "package.json");

            if (!File.Exists(packageJsonFile))
            {
                DebugUtils.Break();
            }

            this.NothingToPoll = true;

            if (!Directory.Exists(nodeModulesPath))
            {
                Directory.CreateDirectory(nodeModulesPath);
            }

            fileSystemWatcher = new FileSystemWatcher(nodeModulesPath);
            fileSystemWatcher.EnableRaisingEvents = true;

            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;

            if (installFromCacheReportDocument != null)
            {
                installFromCacheReportContainerElement = installFromCacheReportDocument.DocumentNode.SelectSingleNode("//div[@class='container']");
                installFromCacheReportTabsElement = installFromCacheReportContainerElement.SelectSingleNode("//div[@class='tab']");
            }

            try
            {
                mailslotClient = new MailslotClient(mailslotName);
            }
            catch
            {
            }
        }

        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath.EndsWith("@ag-grid-community/all-modules") || e.FullPath.EndsWith("JSONPath"))
            {

            }
        }

        private void SendInstallFromCacheStatusMessage(string json)
        {
            mailslotClient.SendMessage(json);
        }

        private void LogInstallFromCacheStatus()
        {
            var installFromCacheInfo = new InstallFromCacheInfo(installsFromCacheStatus, this.installsFromCacheToProcess.Values.ToList());
            var json = installFromCacheInfo.ToJsonText(Newtonsoft.Json.Formatting.Indented);

            installsFromCacheStatusLogWriter.WriteLine(installFromCacheInfo.Header + "\r\n");
            installsFromCacheStatusLogWriter.WriteLine(json);

            installsFromCacheStatusLogWriter.WriteLine();
        }

        /// <summary>   Starts this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

        public override void Start()
        {
            if (!noCaching)
            {
                base.Start();
            }
        }

        /// <summary>   Executes the work operation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="stopping"> True to stopping. </param>

        public override void DoWork(bool stopping)
        {
            Process();
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
            var log = false;

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

            if (log)
            {
                LogInstallFromCacheStatus();
            }

            return installsFromCacheStatus;
        }

        /// <summary>   Queue install from cache. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="mode">         The mode. </param>
        /// <param name="install">      The install. </param>
        /// <param name="cachePath">    Full pathname of the cache file. </param>
        /// <param name="packagePath">  Full pathname of the package file. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool QueueInstallFromCache(string mode, string install, string cachePath, string packagePath)
        {
            using (lockObject.Lock())
            {
                if (noInstallFromCache)
                {
                    return false;
                }
                else
                {
                    if (!installsFromCacheToProcess.ContainsKey(install))
                    {
                        var workingInstallFromCache = new PackageWorkingInstallFromCache(mode, install, cachePath, packagePath, debugPackageInstalls, packageModules);

                        if (this.debugPackageInstalls != null && this.debugPackageInstalls.Any(i => install.StartsWith(i)))
                        {
                            Debugger.Break();
                        }

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

        /// <summary>
        /// Event handler. Called by WorkingInstallFromCache for on add install status events.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Add install status event information. </param>

        private void WorkingInstallFromCache_OnAddInstallStatus(object sender, AddInstallStatusEventArgs e)
        {
            this.AddInstallStatus(e.InstallFromCache, e.StatusMode, e.Status, e.Args);
        }

        /// <summary>
        /// Event handler. Called by WorkingInstallFromCache for on update cache status events.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Cache status event information. </param>

        private void WorkingInstallFromCache_OnUpdateCacheStatus(object sender, CacheStatusEventArgs e)
        {
            using (lockObject.Lock())
            {
                switch (e.IncrementKind)
                {
                    case IncrementKind.IncrementAll:

                        Debug.Assert(e.Increment == 1);
                        installsFromCacheStatus.IncrementAll();

                        UpdateLogs(e.RequestedBy, false);

                        break;

                    case IncrementKind.IncrementTotal:

                        Debug.Assert(e.Increment == 1);
                        installsFromCacheStatus.IncrementTotal();

                        UpdateLogs(e.RequestedBy, false);

                        break;

                    case IncrementKind.TotalRemaining:

                        installsFromCacheStatus.TotalRemaining += e.Increment;

                        UpdateLogs(e.RequestedBy, e.Increment < 0);

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

                e.HandledBy.Add(this);
            }
        }

        /// <summary>   Updates the report. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/31/2021. </remarks>
        ///
        /// <param name="workingInstallFromCache">  The working install from cache. </param>
        /// <param name="postInstall">              True to post install. </param>

        public void UpdateLogs(PackageWorkingInstallFromCache workingInstallFromCache, bool postInstall)
        {
            InstallFromCacheInfo installFromCacheInfo;
            string json;
            List<PackageWorkingInstallFromCache> installs;

            using (lockObject.Lock())
            {
                installs = this.installsFromCacheToProcess.Values.ToList();
            }

            installFromCacheInfo = new InstallFromCacheInfo(installsFromCacheStatus, installs, workingInstallFromCache, postInstall);
            json = installFromCacheInfo.ToJsonText(Newtonsoft.Json.Formatting.Indented);

            if (this.mailslotClient != null)
            {
                SendInstallFromCacheStatusMessage(json);
            }

            installsFromCacheStatusLogWriter.WriteLine(installFromCacheInfo.Header + "\r\n");
            installsFromCacheStatusLogWriter.WriteLine(json);

            installsFromCacheStatusLogWriter.WriteLine();

            AddInstallFromCacheElements(installFromCacheInfo, json, installFromCacheInfo.CurrentInstall, postInstall ? '-' : '+');
        }

        private void AddInstallFromCacheElements(InstallFromCacheInfo installFromCacheInfo, string json, string tabName, char prefix)
        {
            HtmlNode currentInstallNode;
            HtmlNode jsonNode;

            installFromCacheReportTabsElement.AppendChild(HtmlNode.CreateNode($"<button name=\"{ tabName }\" class=\"tablinks\" onclick=\"openTab(event, '{ tabName + prefix }')\">{ prefix }{ tabName }</button>"));
            installFromCacheCurrentContentElement = HtmlNode.CreateNode($"<div id=\"{ tabName + prefix }\" class=\"tabcontent\">");

            installFromCacheReportContainerElement.AppendChild(installFromCacheCurrentContentElement);

            currentInstallNode = installFromCacheCurrentContentElement.CreateSmallBox("Current Install");
            jsonNode = installFromCacheCurrentContentElement.CreateBox("Json");

            currentInstallNode.AppendChild(HtmlNode.CreateNode(installFromCacheInfo.CurrentInstall.HtmlEncodeWithWhitespace()));
            jsonNode.AppendChild(HtmlNode.CreateNode(json.HtmlEncodeWithWhitespace().SurroundWithTag("textarea")));

            installFromCacheReportDocument.Save(installFromCacheReportFile);
        }

        /// <summary>   Installs from cache described by installFromCache. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="installFromCache"> The install from cache. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

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

        /// <summary>   Adds an install status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="installFromCache"> The install from cache. </param>
        /// <param name="mode">             The mode. </param>
        /// <param name="status">           The status. </param>
        /// <param name="args">             A variable-length parameters list containing arguments. </param>

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

        /// <summary>   Process this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

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
                        if (this.debugPackageInstalls != null && this.debugPackageInstalls.Any(i => install.Install.StartsWith(i)))
                        {
                            Debugger.Break();
                        }

                        if (this.InstallFromCache(install))
                        {
                            using (lockObject.Lock())
                            {
                                install.AssureComplete();
                                install.UpdatePackageJson(packageJsonFile);

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

                        this.FoldersSkipped = 0;
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
                                var packageVersion = GetPackageVersion(packagePath);
                                var cachePath = Path.Combine(packageCachePath, path + (packageVersion != null ? "@" + packageVersion.Version : string.Empty));
                                var packageDirectory = new DirectoryInfo(packagePath);
                                var cacheDirectory = new DirectoryInfo(cachePath);

                                this.WriteSweepLine("Processing '{0}'", path);

                                using (lockObject.Lock())
                                {
                                    sweepIndex++;
                                }

                                if (!Directory.Exists(cachePath))
                                {
                                    cacheDirectory.Create();

                                    this.WriteSweepLine("Syncing directories for '{0}'", path);
                                    SyncDirectories(path, cacheDirectory, packageDirectory, packageVersion, ref skipped);

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
                                            SyncDirectories(path, cacheDirectory, packageDirectory, packageVersion, ref skipped);

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
                                            using (lockObject.Lock())
                                            {
                                                pathsToProcess.Remove(path);
                                                foldersSkipped++;
                                            }
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

                            this.statusProgressPercent = Math.Max(sweepIndex.ToPercentageOf(sweepCount), 1);
                        }

                        stopwatch.Stop();

                        this.WriteSweepLine("End of sweep. Copied: {0}, Errored: {1}, Skipped: {2}, Sweep time elapsed: {3} seconds", foldersCopied, foldersWithErrors, foldersSkipped, Math.Round(stopwatch.Elapsed.ToDecimalSeconds()));
                    }

                    using (lockObject.Lock())
                    {
                        cacheStatusType = CacheStatusType.EndOfProcessing;
                        this.statusProgressPercent = 100;
                    }
                }
                else
                {
                    using (lockObject.Lock())
                    {
                        cacheStatusType = CacheStatusType.EndOfProcessing;
                        this.statusProgressPercent = 100;
                    }
                }
            }
            else
            {
                using (lockObject.Lock())
                {
                    cacheStatusType = CacheStatusType.EndOfProcessing;
                    this.statusProgressPercent = 100;
                }
            }

            using (lockObject.Lock())
            {
                this.FoldersSkipped = foldersSkipped;
            }
        }

        /// <summary>   Check versions. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="cachePath">        Full pathname of the cache file. </param>
        /// <param name="packagePath">      Full pathname of the package file. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        private bool CheckVersions(string cachePath, string packagePath)
        {
            var fileInfoCacheJson = new FileInfo(Path.Combine(cachePath.BackSlashes(), "package.json"));
            var cacheDirectory = new DirectoryInfo(cachePath);
            var packageDirectory = new DirectoryInfo(packagePath);
            var fileInfoPackageJson = new FileInfo(Path.Combine(packagePath, "package.json"));

            if (fileInfoPackageJson.Exists)
            {
                using (var readerPackage = fileInfoPackageJson.OpenText())
                {
                    var packageJson = JsonExtensions.ReadJson<PackageJson>(readerPackage);
                    var packageName = packageJson.Name;
                    var packageVersion = new NpmVersion(packageJson.Version);

                    if (fileInfoCacheJson.Exists)
                    {
                        using (var readerCache = fileInfoCacheJson.OpenText())
                        {
                            var cacheJson = JsonExtensions.ReadJson<PackageJson>(readerCache);
                            var cacheName = cacheJson.Name;
                            NpmVersion cacheVersion = cacheJson.Version;

                            if (Directory.Exists(packagePath))
                            {
                                if (cacheVersion == packageVersion)
                                {
                                    return true;
                                }
                                else if (cacheVersion < packageVersion)
                                {
                                    this.WriteSweepLine("Cached version for '{0}@{1}' less than package version '{2}'. Skipping cache.", packageName, cacheVersion.VersionString, packageVersion.VersionString);

                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        private NpmVersion GetPackageVersion(string packagePath)
        {
            var packageDirectory = new DirectoryInfo(packagePath);
            var fileInfoPackageJson = new FileInfo(Path.Combine(packagePath, "package.json"));

            if (fileInfoPackageJson.Exists)
            {
                using (var readerPackage = fileInfoPackageJson.OpenText())
                {
                    var packageJson = JsonExtensions.ReadJson<PackageJson>(readerPackage);
                    var packageName = packageJson.Name;
                    var packageVersion = new NpmVersion(packageJson.Version);

                    return packageVersion;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>   Pushes a set status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="statusType">   Type of the status. </param>
        ///
        /// <returns>   An IDisposable. </returns>

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

            return statusStack.CreateDisposable(() =>
            {
                PopStatus();
            });
        }

        /// <summary>   Pops the status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

        public void PopStatus()
        {
            using (lockObject.Lock())
            {
                sweepingLogWriter = null;
                this.cacheStatusType = statusStack.Pop();
            }
        }

        /// <summary>   Skip path. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>

        public void SkipPath(string path)
        {
            using (lockObject.Lock())
            {
                this.WriteLineNoLock("Adding '{0}' to paths to not process", path);
                pathsToNotProcess.Add(path);
            }
        }

        /// <summary>   Event handler. Called by FileSystemWatcher for changed events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        File system event information. </param>

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                var path = e.FullPath.RemoveStart(nodeModulesPath + @"\");

                if (noCaching)
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

        /// <summary>   Event handler. Called by FileSystemWatcher for created events. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        File system event information. </param>

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                var path = e.FullPath.RemoveStart(nodeModulesPath + @"\");

                if (noCaching)
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

        /// <summary>   Last pass process. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

        public void LastPassProcess()
        {
            if (this.CheckShouldRun())
            {
                var modulesDirectory = new DirectoryInfo(nodeModulesPath);
                NodeModulePath nodeModulePath = null;

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

                nodeModulePath = registrySettings.NodeModulePaths.SingleOrDefault(p => p.Path == modulesDirectory.FullName);

                if (nodeModulePath == null)
                {
                    nodeModulePath = new NodeModulePath { Path = modulesDirectory.FullName };
                    registrySettings.NodeModulePaths.Add(nodeModulePath);
                }

                nodeModulePath.DateOfLastPassProcess = DateTime.Now.ToString();
                nodeModulePath.Path = nodeModulesPath;

                registrySettings.Save();
            }
        }

        private bool CheckShouldRun()
        {
            var modulesDirectory = new DirectoryInfo(nodeModulesPath);
            var cacheDirectory = new DirectoryInfo(packageCachePath);
            var nodeModulePath = registrySettings.NodeModulePaths.SingleOrDefault(p => p.Path == modulesDirectory.FullName);
            var dateOfLastPassProcess = DateTime.MinValue;

            if (nodeModulePath != null)
            {
                var dateOfLastPassProcessString = nodeModulePath.DateOfLastPassProcess;

                if (dateOfLastPassProcessString != null)
                {
                    DateTime.TryParse(dateOfLastPassProcessString, out dateOfLastPassProcess);
                }
            }
            else
            {
                return true;
            }
            
            if (DateTime.Now - dateOfLastPassProcess > TimeSpan.FromDays(30))
            {
                return true;
            }
            else
            {
                var modulesSubDirectories = modulesDirectory.GetDirectories();
                var cacheSubDirectories = cacheDirectory.GetDirectories();
                var shouldRun = false;

                foreach (var modulesSubDirectory in modulesSubDirectories)
                {
                    var matchingSubDirectories = cacheSubDirectories.Where(d => d.MatchesPackageName(modulesSubDirectory.Name)).ToList();

                    if (matchingSubDirectories.Count == 0)
                    {
                        shouldRun = true;
                    }
                    else if (matchingSubDirectories.Any(m => m.GetComparisonScore(modulesSubDirectory) < .10))
                    {
                        shouldRun = true;
                    }
                    else
                    {
                        pathsToNotProcess.Add(modulesSubDirectory.Name);
                    }
                }

                return shouldRun;
            }
        }

        /// <summary>   Stops this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

        public override void Stop()
        {
            if (!noCaching)
            {
                var count = 1;
                var modulesDirectory = new DirectoryInfo(nodeModulesPath);
                var lastCount = 0;

                this.WriteLine("Request to stop service. Waiting on {0} paths to finish processing", count);

                while (count > 0)
                {
                    using (lockObject.Lock())
                    {
                        count = pathsToProcess.Count - this.FoldersSkipped;

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
        }

        /// <summary>   Queries if a given assure no open. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="directory">    Pathname of the directory. </param>
        /// <param name="timeSpan">     [out] The time span. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

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

        /// <summary>   Synchronizes the directories. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="path">             Full pathname of the file. </param>
        /// <param name="cacheDirectory">   Pathname of the cache directory. </param>
        /// <param name="packageDirectory"> Pathname of the package directory. </param>
        /// <param name="packageVersion">   The package version. </param>
        /// <param name="skipped">          [in,out] True if skipped. </param>

        private void SyncDirectories(string path, DirectoryInfo cacheDirectory, DirectoryInfo packageDirectory, NpmVersion packageVersion, ref bool skipped)
        {
            TimeSpan timeSpan;

            if (AssureNoOpen(packageDirectory, out timeSpan))
            {
                var elapsed = TimeSpan.MinValue;

                this.WriteLine("Copying '{0}' to '{1}", path, cacheDirectory.FullName);

                AddPathStatus(path, CacheStatusType.PathProcessing);

                if (packageVersion == null && packageDirectory.Name.StartsWith("@"))
                {
                    packageDirectory.GetFiles().ForEach(f => f.CopyTo(Path.Combine(cacheDirectory.FullName, f.Name)));

                    foreach (var packageSubdirectory in packageDirectory.GetDirectories())
                    {
                        var packageSubPath = packageSubdirectory.FullName;
                        var packageSubVersion = GetPackageVersion(packageSubPath);
                        var cacheSubPath = Path.Combine(packageCachePath, path, packageSubdirectory.Name + "@" + packageSubVersion.Version);
                        var packageSubDirectory = new DirectoryInfo(packageSubPath);
                        var cacheSubDirectory = new DirectoryInfo(cacheSubPath);

                        if (!Directory.Exists(cacheSubPath))
                        {
                            cacheSubDirectory.Create();

                            packageSubDirectory.CopyTo(cacheSubDirectory.FullName, true, true);
                        }
                    }
                }
                else
                {
                    using (this.StartStopwatch((s) => elapsed = s))
                    {
                        packageDirectory.CopyTo(cacheDirectory.FullName, true, true);

                        AddPathStatus(path, CacheStatusType.PathProcessed);
                    }
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

        /// <summary>   Sets install status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="status">   The status. </param>
        ///
        /// <returns>   A string. </returns>

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

        /// <summary>   Sets install count. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="count">    Number of. </param>

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
            PackageCacheStatusInfo cacheStatus;

            using (lockObject.Lock())
            {
                cacheStatus = new PackageCacheStatusInfo(this.cacheStatusType, this.currentActionVerb, this.installStatusType, sweepIndex, sweepCount, lastSweepStart, lastCacheStatusRequest, pathsProcessed, this.statusProgressPercent, memoryStatus);

                if (mode != "Agent")
                {
                    cacheStatus = new PackageCacheStatusInfo(cacheStatus.CacheStatus, cacheStatus.StatusText, cacheStatus.StatusProgressPercent);
                }

                cacheStatus.NoCaching = noCaching;
                cacheStatus.NoInstallFromCache = noInstallFromCache;

                if (setAsReported)
                {
                    lastCacheStatusRequest = DateTime.Now;
                    lastCacheStatusMode = mode;

                    cacheStatus.SetLastStatusReported(pathsProcessed, lastCacheStatusRequest);
                }
            }

            return cacheStatus;
        }

        /// <summary>   Adds a package cache status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="path">     Full pathname of the file. </param>
        /// <param name="install">  The install. </param>
        /// <param name="type">     The type. </param>

        public void AddPackageCacheStatus(string path, string install, CacheStatusType type)
        {
            using (lockObject.Lock())
            {
                pathsProcessed.AddToDictionaryListCreateIfNotExist(path, new PathCacheStatus(path, type));
            }
        }

        /// <summary>   Adds a package cache status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="path">         Full pathname of the file. </param>
        /// <param name="install">      The install. </param>
        /// <param name="exception">    The exception. </param>

        public void AddPackageCacheStatus(string path, string install, Exception exception)
        {
            using (lockObject.Lock())
            {
                pathsProcessed.AddToDictionaryListCreateIfNotExist(path, new PathCacheStatus(path, exception));
            }
        }

        /// <summary>   Adds a path status to 'exception'. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        /// <param name="type"> The type. </param>

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

        /// <summary>   Adds a path status to 'exception'. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="path">     Full pathname of the file. </param>
        /// <param name="type">     The type. </param>
        /// <param name="message">  The message. </param>

        public void AddPathStatus(string path, CacheStatusType type, string message)
        {
            using (lockObject.Lock())
            {
                pathsProcessed.AddToDictionaryListCreateIfNotExist(path, new PathCacheStatus(path, type, message));
            }
        }

        /// <summary>   Adds a path status to 'exception'. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="path">         Full pathname of the file. </param>
        /// <param name="exception">    The exception. </param>

        public void AddPathStatus(string path, Exception exception)
        {
            using (lockObject.Lock())
            {
                pathsProcessed.AddToDictionaryListCreateIfNotExist(path, new PathCacheStatus(path, exception));
            }
        }

        /// <summary>   Adds a path status no lock to 'exception'. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        /// <param name="type"> The type. </param>

        public void AddPathStatusNoLock(string path, CacheStatusType type)
        {
            pathsProcessed.AddToDictionaryListCreateIfNotExist(path, new PathCacheStatus(path, type));
        }

        /// <summary>   Adds a path status no lock to 'exception'. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="path">     Full pathname of the file. </param>
        /// <param name="type">     The type. </param>
        /// <param name="message">  The message. </param>

        public void AddPathStatusNoLock(string path, CacheStatusType type, string message)
        {
            pathsProcessed.AddToDictionaryListCreateIfNotExist(path, new PathCacheStatus(path, type, message));
        }

        /// <summary>   Adds a path status no lock to 'exception'. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="path">         Full pathname of the file. </param>
        /// <param name="exception">    The exception. </param>

        public void AddPathStatusNoLock(string path, Exception exception)
        {
            pathsProcessed.AddToDictionaryListCreateIfNotExist(path, new PathCacheStatus(path, exception));
        }

        /// <summary>   Writes. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="value">    The value. </param>

        public void Write(string value)
        {
            using (lockObject.Lock())
            {
                logWriter.Write(value);
            }
        }

        /// <summary>   Writes. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        public void Write(string format, params object[] args)
        {
            using (lockObject.Lock())
            {
                logWriter.Write(format, args);
            }
        }

        /// <summary>   Writes the line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="value">    The value. </param>

        public void WriteLine(string value)
        {
            using (lockObject.Lock())
            {
                logWriter.WriteLine(value);
            }
        }

        /// <summary>   Writes a sweep line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

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

        /// <summary>   Writes the line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        public void WriteLine(string format, params object[] args)
        {
            using (lockObject.Lock())
            {
                logWriter.WriteLine(format, args);
            }
        }

        /// <summary>   Writes the line. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

        public void WriteLine()
        {
            using (lockObject.Lock())
            {
                logWriter.WriteLine();
            }
        }

        /// <summary>   Writes a no lock. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        public void WriteNoLock(string format, params object[] args)
        {
            logWriter.Write(format, args);
        }

        /// <summary>   Writes the line no lock. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="value">    The value. </param>

        public void WriteLineNoLock(string value)
        {
            logWriter.WriteLine(value);
        }

        /// <summary>   Writes the line no lock. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        public void WriteLineNoLock(string format, params object[] args)
        {
            logWriter.WriteLine(format, args);
        }

        /// <summary>   Writes the line no lock. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>

        public void WriteLineNoLock()
        {
            logWriter.WriteLine();
        }

        public IDisposable ErrorMode()
        {
            throw new NotImplementedException();
        }
    }
}
