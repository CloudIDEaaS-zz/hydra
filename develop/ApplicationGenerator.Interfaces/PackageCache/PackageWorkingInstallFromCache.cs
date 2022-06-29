// file:	PackageCache\PackageWorkingInstallFromCache.cs
//
// summary:	Implements the package working install from cache class

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Utils;
using Utils.Hierarchies;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX;
using System.Dynamic;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Threading;
using AbstraX.PackageExtensions;
using System.Windows.Forms;

namespace AbstraX.PackageCache
{
    /// <summary>   A package working install from cache. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

    [DebuggerDisplay(" { DebugInfo }")]
    public class PackageWorkingInstallFromCache
    {
        /// <summary>   Gets the mode. </summary>
        ///
        /// <value> The mode. </value>

        public string Mode { get; }

        /// <summary>   Gets the install. </summary>
        ///
        /// <value> The install. </value>

        public string Install { get; }

        /// <summary>   Gets or sets the full pathname of the cache file. </summary>
        ///
        /// <value> The full pathname of the cache file. </value>

        public string CachePath { get; private set; }

        /// <summary>   Gets or sets the full pathname of the package file. </summary>
        ///
        /// <value> The full pathname of the package file. </value>

        public string PackagePath { get; private set; }

        /// <summary>   Gets a list of install status. </summary>
        ///
        /// <value> A list of install status. </value>

        public List<PackageInstallFromCacheStatus> InstallStatusList { get; }

        /// <summary>   Gets or sets the parent. </summary>
        ///
        /// <value> The parent. </value>

        public PackageWorkingInstallFromCache Parent { get; set; }

        /// <summary>   Gets the dependencies from cache to process. </summary>
        ///
        /// <value> The dependencies from cache to process. </value>

        public Dictionary<string, PackageWorkingInstallFromCache> DependenciesFromCacheToProcess { get; }

        /// <summary>   Gets all dependencies to process. </summary>
        ///
        /// <value> all dependencies to process. </value>

        public Dictionary<string, PackageWorkingInstallFromCache> AllDependenciesToProcess { get; }

        /// <summary>   Gets or sets a value indicating whether the install to local modules. </summary>
        ///
        /// <value> True if install to local modules, false if not. </value>

        public bool InstallToLocalModules { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is scoped. </summary>
        ///
        /// <value> True if this  is scoped, false if not. </value>

        public bool IsScoped { get; set; }

        /// <summary>   Gets a queue of install actions. </summary>
        ///
        /// <value> A queue of install actions. </value>

        public Queue<InstallAction> InstallActionsQueue { get; }

        /// <summary>   Gets the package modules. </summary>
        ///
        /// <value> The package modules. </value>

        public NpmNodeModules PackageModules { get; }

        /// <summary>   Gets or sets the state of the install from cache. </summary>
        ///
        /// <value> The install from cache state. </value>

        public InstallFromCacheState InstallFromCacheState { get; set; }
        public List<string> DebugPackageInstalls { get; }

        /// <summary>   Gets or sets the install exception. </summary>
        ///
        /// <value> The install exception. </value>

        public Exception InstallException { get; private set; }

        /// <summary>   Event queue for all listeners interested in OnQueueInstallAction events. </summary>
        public event EventHandlerT<InstallAction> OnQueueInstallAction;
        /// <summary>   Event queue for all listeners interested in OnAddToAllDependencies events. </summary>
        public event EventHandlerT<KeyValuePair<string, PackageWorkingInstallFromCache>> OnAddToAllDependencies;
        /// <summary>   Event queue for all listeners interested in OnAddInstallStatus events. </summary>
        public event AddInstallStatusEventHandler OnAddInstallStatus;
        /// <summary>   Event queue for all listeners interested in PreInstall events. </summary>
        /// <summary>   Full pathname of the node modules file. </summary>
        private string nodeModulesPath;
        /// <summary>   The cache path root. </summary>
        private string cachePathRoot;
        /// <summary>   True if increment raised. </summary>
        private bool incrementRaised;
        private List<UpdateCacheStatusEventHandler> updateCacheStatusHandlers;
        private List<EventHandlerT<InstallAction>> queueInstallHandlers;
        private List<EventHandlerT<KeyValuePair<string, PackageWorkingInstallFromCache>>> addToAllDependenciesHandlers;
        private List<AddInstallStatusEventHandler> addInstallStatusHandlers;

        /// <summary>
        /// Event queue for all listeners interested in internalOnUpdateCacheStatus events.
        /// </summary>

        private UpdateCacheStatusEventHandler internalOnUpdateCacheStatus;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="mode">                 The mode. </param>
        /// <param name="install">              The install. </param>
        /// <param name="cachePath">            Full pathname of the cache file. </param>
        /// <param name="packagePath">          Full pathname of the package file. </param>
        /// <param name="debugPackageInstalls"> The debug package installs. </param>
        /// <param name="packageModules">       The package modules. </param>
        /// <param name="parent">               (Optional) The parent. </param>

        public PackageWorkingInstallFromCache(string mode, string install, string cachePath, string packagePath, List<string> debugPackageInstalls, NpmNodeModules packageModules, PackageWorkingInstallFromCache parent = null)
        {
            var packageDirectory = new DirectoryInfo(packagePath);
            var cacheDirectory = new DirectoryInfo(cachePath);
            
            this.Mode = mode;
            this.Install = install;
            this.CachePath = cachePath;
            this.PackagePath = packagePath;
            this.InstallStatusList = new List<PackageInstallFromCacheStatus>();
            this.InstallActionsQueue = new Queue<InstallAction>();
            this.PackageModules = packageModules;
            this.Parent = parent;
            this.InstallFromCacheState = InstallFromCacheState.Created;
            this.DebugPackageInstalls = debugPackageInstalls;

            nodeModulesPath = packageDirectory.GetParentPathToFolder("node_modules", true);
            cachePathRoot = cacheDirectory.GetParentPathToFolder("cache", true);
            updateCacheStatusHandlers = new List<UpdateCacheStatusEventHandler>();
            queueInstallHandlers = new List<EventHandlerT<InstallAction>>();
            addToAllDependenciesHandlers = new List<EventHandlerT<KeyValuePair<string, PackageWorkingInstallFromCache>>>();
            addInstallStatusHandlers = new List<AddInstallStatusEventHandler>();

            this.DependenciesFromCacheToProcess = new Dictionary<string, PackageWorkingInstallFromCache>();
            this.AllDependenciesToProcess = new Dictionary<string, PackageWorkingInstallFromCache>();
        }

        /// <summary>   Gets a value indicating whether this  is root. </summary>
        ///
        /// <value> True if this  is root, false if not. </value>

        public bool IsRoot
        {
            get
            {
                return this.Parent == null;
            }
        }

        /// <summary>   Gets a value indicating whether this  is missing peer. </summary>
        ///
        /// <value> True if this  is missing peer, false if not. </value>

        public bool IsMissingPeer
        {
            get
            {
                var cacheDirectory = new DirectoryInfo(this.CachePath);
                var packageDirectory = new DirectoryInfo(this.PackagePath);
                var fileInfoCacheJson = new FileInfo(Path.Combine(this.CachePath, "package.json"));

                if (cacheDirectory.Exists)
                {
                    if (fileInfoCacheJson.Exists)
                    {
                        using (var readerPackage = fileInfoCacheJson.OpenText())
                        {
                            var cachePackageJson = JsonExtensions.ReadJson<PackageJson>(readerPackage);
                            var cachePackageName = cachePackageJson.Name;
                            var cachePackageVersion = cachePackageJson.Version;

                            if (cachePackageJson.PeerDependencies == null)
                            {
                                return false;
                            }
                            else
                            {
                                return !cachePackageJson.PeerDependencies.PeerExists(this.PackageModules);
                            }
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>   Installs the package. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="debugPackageInstalls"> The debug package installs. </param>
        /// <param name="executeActions">       (Optional) True to execute actions. </param>
        /// <param name="executeDirectly">      (Optional) True to execute directly. </param>
        /// <param name="installDev">           (Optional) True to install development. </param>

        public void InstallPackage(bool executeActions = true, bool executeDirectly = false, bool installDev = false)
        {
            var cacheDirectory = new DirectoryInfo(this.CachePath);
            var packageDirectory = new DirectoryInfo(this.PackagePath);
            var fileInfoCacheJson = new FileInfo(Path.Combine(this.CachePath, "package.json"));

            if (cacheDirectory.Exists)
            {
                if (fileInfoCacheJson.Exists)
                {
                    using (var readerPackage = fileInfoCacheJson.OpenText())
                    {
                        var cachePackageJson = JsonExtensions.ReadJson<PackageJson>(readerPackage);
                        var cachePackageName = cachePackageJson.Name;
                        var cachePackageVersion = cachePackageJson.Version;
                        var cachePackageNodeModulesDirectory = new DirectoryInfo(Path.Combine(this.CachePath, "node_modules"));

                        if (cachePackageJson.Dependencies != null)
                        {
                            foreach (var dependencyPair in cachePackageJson.Dependencies)
                            {
                                this.InstallDependency(dependencyPair.Key, dependencyPair.Value, cachePackageNodeModulesDirectory, executeActions, executeDirectly);
                            }
                        }

                        if (installDev)
                        {
                            if (cachePackageJson.DevDependencies != null)
                            {
                                foreach (var dependencyPair in cachePackageJson.DevDependencies)
                                {
                                    this.InstallDependency(dependencyPair.Key, dependencyPair.Value, cachePackageNodeModulesDirectory, executeActions, executeDirectly);
                                }
                            }
                        }
                    }
                }
                else
                {

                }

                this.InstallPeerDependencies();
            }
            else
            {
                // see if scoped (begins w/ @)

                cacheDirectory = new DirectoryInfo(Path.Combine(this.cachePathRoot, "@" + this.Install));

                if (cacheDirectory.Exists)
                {
                    InstallScopedPackage(cacheDirectory, executeActions, executeDirectly);
                }
                else
                {
                    DebugUtils.Break();
                }
            }

            this.ExecuteOrQueue(this, cacheDirectory, packageDirectory, (w, c, p) =>
            {
                AddInstallStatus(this, StatusMode.Normal, "Installing");

                w.InstallFromCacheState = InstallFromCacheState.Installing;

                if (this.DebugPackageInstalls != null && this.DebugPackageInstalls.Any(i => this.Install.StartsWith(i)))
                {
                    Debugger.Break();
                }

                try
                {
                    c.CopyTo(p.FullName, true, false);
                    w.InstallFromCacheState = InstallFromCacheState.Installed;
                }
                catch (Exception ex)
                {
                    w.InstallFromCacheState = InstallFromCacheState.Errored;
                    w.InstallException = ex;

                    MessageBox.Show(ex.Message);

                    throw;
                }

                if (this.IsRoot)
                {
                    UpdateCacheStatus(IncrementKind.RequestedRemaining, -1);
                    UpdateCacheStatus(IncrementKind.TotalRemaining, -1);
                }
                else
                {
                    UpdateCacheStatus(IncrementKind.TotalRemaining, -1);
                }

            }, executeDirectly);

            if (executeActions)
            {
                ExecuteActions();
            }
        }

        /// <summary>   Installs the scoped package. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="cacheDirectory">   Pathname of the cache directory. </param>
        /// <param name="executeActions">   True to execute actions. </param>
        /// <param name="executeDirectly">  True to execute directly. </param>

        private void InstallScopedPackage(DirectoryInfo cacheDirectory, bool executeActions, bool executeDirectly)
        {
            this.CachePath = cacheDirectory.FullName;
            this.PackagePath = Path.Combine(this.nodeModulesPath, "@" + this.Install);
            this.IsScoped = true;

            foreach (var subDirectory in cacheDirectory.GetDirectories())
            {
                NpmVersion cacheVersion;
                var fileInfoCacheJson = new FileInfo(Path.Combine(subDirectory.FullName, "package.json"));

                if (fileInfoCacheJson.Exists)
                {
                    using (var readerCache = fileInfoCacheJson.OpenText())
                    {
                        var cachePackageJson = JsonExtensions.ReadJson<PackageJson>(readerCache);
                        var cachePackageName = cachePackageJson.Name;
                        var nodeModulesDirectory = new DirectoryInfo(nodeModulesPath);

                        if (cachePackageJson.PeerDependencies == null || cachePackageJson.PeerDependencies.PeerExists(this.PackageModules))
                        {
                            cacheVersion = cachePackageJson.Version;

                            InstallDependency(cacheDirectory.Name + "/" + subDirectory.Name, cacheVersion, subDirectory.FullName, null, executeActions, executeDirectly);
                        }
                        else
                        {
                        }
                    }
                }
                else
                {
                    DebugUtils.Break();
                }
            }
        }

        /// <summary>   Installs the peer dependencies. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

        private void InstallPeerDependencies()
        {
            // kn - todo.  Have to scour the whole cache
        }

        /// <summary>   Adds an install status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="sender">           Source of the event. </param>
        /// <param name="installFromCache"> The install from cache. </param>
        /// <param name="mode">             The mode. </param>
        /// <param name="status">           The status. </param>
        /// <param name="args">             A variable-length parameters list containing arguments. </param>

        private void AddInstallStatus(object sender, PackageWorkingInstallFromCache installFromCache, StatusMode mode, string status, params object[] args)
        {
            var installStatusEventArgs = new AddInstallStatusEventArgs(installFromCache, mode, status, args);

            OnAddInstallStatus(sender, installStatusEventArgs);
        }

        /// <summary>   Adds an install status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="installFromCache"> The install from cache. </param>
        /// <param name="mode">             The mode. </param>
        /// <param name="status">           The status. </param>
        /// <param name="args">             A variable-length parameters list containing arguments. </param>

        private void AddInstallStatus(PackageWorkingInstallFromCache installFromCache, StatusMode mode, string status, params object[] args)
        {
            var installStatusEventArgs = new AddInstallStatusEventArgs(installFromCache, mode, status, args);

            OnAddInstallStatus(this, installStatusEventArgs);
        }

        /// <summary>   Event queue for all listeners interested in OnUpdateCacheStatus events. </summary>
        public event UpdateCacheStatusEventHandler OnUpdateCacheStatus
        {
            add
            {
                internalOnUpdateCacheStatus += value;

                if (!incrementRaised)
                {
                    if (this.IsRoot)
                    {
                        UpdateCacheStatus(IncrementKind.IncrementAll);
                    }
                    else
                    {
                        UpdateCacheStatus(IncrementKind.IncrementTotal);
                    }

                    incrementRaised = true;
                }
            }

            remove
            {
                internalOnUpdateCacheStatus -= value;
            }
        }

        /// <summary>   Executes the actions operation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

        public void ExecuteActions()
        {
            while (this.InstallActionsQueue.Count > 0)
            {
                var installAction = this.InstallActionsQueue.Dequeue();

                try
                {
                    installAction.Action(installAction.WorkingInstall, installAction.CacheDirectory, installAction.PackageDirectory);
                }
                catch (Exception ex)
                {
                    installAction.Exception = ex;
                }
                finally
                {
                    installAction.ExecutedTime = DateTime.Now;
                }
            }
        }

        /// <summary>   Executes the or queue operation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="packageWorkingInstallFromCache">   The package working install from cache. </param>
        /// <param name="cacheDirectory">                   Pathname of the cache directory. </param>
        /// <param name="packageDirectory">                 Pathname of the package directory. </param>
        /// <param name="action">                           The action. </param>
        /// <param name="executeDirectly">                  (Optional) True to execute directly. </param>

        private void ExecuteOrQueue(PackageWorkingInstallFromCache packageWorkingInstallFromCache, DirectoryInfo cacheDirectory, DirectoryInfo packageDirectory, Action<PackageWorkingInstallFromCache, DirectoryInfo, DirectoryInfo> action, bool executeDirectly = false)
        {
            if (executeDirectly)
            {
                try
                {
                    action(packageWorkingInstallFromCache, cacheDirectory, packageDirectory);
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                packageWorkingInstallFromCache.InstallFromCacheState = InstallFromCacheState.Queued;

                if (this.IsRoot)
                {
                    this.InstallActionsQueue.Enqueue(new InstallAction
                    {
                        Action = action,
                        CacheDirectory = cacheDirectory,
                        PackageDirectory = packageDirectory,
                        WorkingInstall = this
                    });
                }
                else
                {
                    OnQueueInstallAction.Raise(this, new InstallAction
                    {
                        Action = action,
                        CacheDirectory = cacheDirectory,
                        PackageDirectory = packageDirectory,
                        WorkingInstall = this
                    });
                }
            }
        }

        /// <summary>   Assure complete. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/31/2021. </remarks>

        public void AssureComplete()
        {
            Debug.Assert(this.InstallFromCacheState == InstallFromCacheState.Installed);

            CheckInstalls(this.DependenciesFromCacheToProcess.Values.ToList());
        }

        private void CheckInstalls(List<PackageWorkingInstallFromCache> installs)
        {
            foreach (var install in installs)
            {
                Debug.Assert(install.InstallFromCacheState == InstallFromCacheState.Installed);

                CheckInstalls(install.DependenciesFromCacheToProcess.Values.ToList());
            }
        }

        /// <summary>   Installs the dependency. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="name">                             The name. </param>
        /// <param name="installVersion">                   The install version. </param>
        /// <param name="cachePackageNodeModulesDirectory"> Pathname of the cache package node modules
        ///                                                 directory. </param>
        /// <param name="executeActions">                   (Optional) True to execute actions. </param>
        /// <param name="executeDirectly">                  (Optional) True to execute directly. </param>

        private void InstallDependency(string name, NpmVersion installVersion, DirectoryInfo cachePackageNodeModulesDirectory, bool executeActions = true, bool executeDirectly = false)
        {
            List<DirectoryInfo> cacheDirectories;
            var cachePathDirectory = new DirectoryInfo(cachePathRoot);
            var parts = name.Split('/');
            var firstPart = parts.First();
            var directories = cachePathDirectory.GetDirectories().Where(d => d.MatchesPackageName(firstPart)).ToArray();
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

                foreach (var cacheDirectory in cacheDirectories)
                {
                    var regex = new Regex("(?<package>@?[^@]*?)($|@(?<version>.*$))");
                    string cacheName = null;
                    string cacheVersion = null;

                    if (regex.IsMatch(cacheDirectory.Name))
                    {
                        var match = regex.Match(cacheDirectory.Name);

                        cacheName = match.GetGroupValue("package");
                        cacheVersion = match.GetGroupValue("version");
                    }
                    else
                    {
                        DebugUtils.Break();
                    }

                    if (installVersion.Matches(cacheVersion))
                    {
                        InstallDependency(name, installVersion, cacheDirectory.FullName, cachePackageNodeModulesDirectory, executeActions, executeDirectly);
                        return;
                    }
                }
            }
        }

        /// <summary>   Updates the package JSON described by packageJsonFile. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/14/2021. </remarks>
        ///
        /// <param name="packageJsonFile">  The package JSON file. </param>

        public void UpdatePackageJson(string packageJsonFile)
        {
            var fileInfoJson = new FileInfo(packageJsonFile);

            if (fileInfoJson.Exists)
            {
                var addToJson = false;
                var regex = new Regex("(?<package>@?[^@]*?)($|@(?<version>.*$))");
                string name = null;
                string version = null;

                if (regex.IsMatch(this.Install))
                {
                    var match = regex.Match(this.Install);
                    
                    name = match.GetGroupValue("package");
                    version = match.GetGroupValue("version");
                }
                else
                {
                    DebugUtils.Break();
                }

                using (var readerPackage = fileInfoJson.OpenText())
                {
                    var packageJson = JsonExtensions.ReadJson<PackageJson>(readerPackage);

                    if (this.Mode == "installs")
                    {
                        if (!packageJson.Dependencies.ContainsKey(name))
                        {
                            addToJson = true;
                        }
                    }
                    else if (this.Mode == "devInstalls")
                    {
                        if (!packageJson.DevDependencies.ContainsKey(name))
                        {
                            addToJson = true;
                        }
                    }
                    else
                    {
                        DebugUtils.Break();
                    }
                }

                if (addToJson)
                {
                    dynamic obj = null;
                    var added = false;
                    var retry = 0;

                    using (var readerPackage = fileInfoJson.OpenText())
                    {
                        var json = readerPackage.ReadToEnd();
                        
                        obj = JsonConvert.DeserializeObject<ExpandoObject>(json);

                        if (this.Mode == "installs")
                        {
                            var dictionary = (IDictionary<string, Object>) obj.dependencies;

                            dictionary.Add(name, version);
                        }
                        else if (this.Mode == "devInstalls")
                        {
                            var dictionary = (IDictionary<string, Object>)obj.devDependencies;

                            dictionary.Add(name, version);
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    }

                    while (!added)
                    {
                        try
                        {
                            using (var fileStream = fileInfoJson.OpenWrite())
                            {
                                var writer = new StreamWriter(fileStream);
                                var json = JsonConvert.SerializeObject(obj, Formatting.Indented);

                                writer.Write(json);
                                writer.Flush();
                            }

                            added = true;
                        }
                        catch (Exception ex)
                        {
                            Thread.Sleep(100);
                            retry++;

                            if (retry == 100)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>   Installs the dependency. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="name">                             The name. </param>
        /// <param name="installVersion">                   The install version. </param>
        /// <param name="cachePath">                        Full pathname of the cache file. </param>
        /// <param name="cachePackageNodeModulesDirectory"> Pathname of the cache package node modules
        ///                                                 directory. </param>
        /// <param name="executeActions">                   (Optional) True to execute actions. </param>
        /// <param name="executeDirectly">                  (Optional) True to execute directly. </param>

        private void InstallDependency(string name, NpmVersion installVersion, string cachePath, DirectoryInfo cachePackageNodeModulesDirectory, bool executeActions = true, bool executeDirectly = false)
        {
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
                        var cachePackageJson = JsonExtensions.ReadJson<PackageJson>(readerCache);
                        var cachePackageName = cachePackageJson.Name;
                        NpmVersion cacheVersion = cachePackageJson.Version;

                        if (name != cachePackageName)
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

                                        if (cacheVersion >= packageVersion)
                                        {
                                            InstallFromCache(name, installVersion, cacheDirectory, packageDirectory, executeActions, executeDirectly);
                                        }
                                    }
                                }
                                else
                                {
                                    TimeSpan timeSpan;

                                    if (!cacheDirectory.CompareTo(packageDirectory, out timeSpan))
                                    {
                                        InstallFromCache(name, installVersion, cacheDirectory, packageDirectory, executeActions, executeDirectly);
                                    }
                                }
                            }
                            else
                            {
                                InstallFromCache(name, installVersion, cacheDirectory, packageDirectory, executeActions, executeDirectly);
                            }
                        }
                    }
                }
                else
                {
                    // cache package.json does not exist

                    InstallFromCache(name, installVersion, cacheDirectory, packageDirectory, executeActions, executeDirectly);
                }
            }
            else
            {
                // cache path does not exist

                DebugUtils.Break();
            }
        }

        /// <summary>   Validates the tree described by installsFromCacheStatus. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="installsFromCacheStatus">  The installs from cache status. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        internal bool ValidateTree(PackageInstallsFromCacheStatus installsFromCacheStatus)
        {
            var dependencies = this.GetDependencyCount(true);

            Debug.Assert(installsFromCacheStatus.Requested == 1);
            Debug.Assert(installsFromCacheStatus.Total == dependencies + 1);

            return true;
        }

        /// <summary>   Gets dependency count. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="recurse">  (Optional) True to process recursively, false to process locally
        ///                         only. </param>
        ///
        /// <returns>   The dependency count. </returns>

        internal int GetDependencyCount(bool recurse = false)
        {
            var count = 0;

            foreach (var installFromCache in this.DependenciesFromCacheToProcess.Values)
            {
                count++;

                if (recurse)
                {
                    count += installFromCache.GetDependencyCount(recurse);
                }
            }

            return count;
        }

        /// <summary>   Print tree. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="indent">   (Optional) The indent. </param>
        ///
        /// <returns>   A string. </returns>

        internal string PrintTree(int indent = 0)
        {
            var builder = new StringBuilder();

            builder.AppendLineFormatSpaceIndent(indent * 2, this.DebugInfo);

            foreach (var installFromCache in this.DependenciesFromCacheToProcess.Values)
            {
                builder.Append(installFromCache.PrintTree(indent + 1));
            }

            return builder.ToString();
        }

        /// <summary>   Installs from cache. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="name">             The name. </param>
        /// <param name="installVersion">   The install version. </param>
        /// <param name="cacheDirectory">   Pathname of the cache directory. </param>
        /// <param name="packageDirectory"> Pathname of the package directory. </param>
        /// <param name="executeActions">   (Optional) True to execute actions. </param>
        /// <param name="executeDirectly">  (Optional) True to execute directly. </param>

        private void InstallFromCache(string name, NpmVersion installVersion, DirectoryInfo cacheDirectory, DirectoryInfo packageDirectory, bool executeActions = true, bool executeDirectly = false)
        {
            var install = name + "@" + installVersion.Version;

            if (!this.ContainsKey(install))
            {
                var workingInstallFromCache = new PackageWorkingInstallFromCache(this.Mode, install, cacheDirectory.FullName, packageDirectory.FullName, this.DebugPackageInstalls, this.PackageModules, this);

                HandleEvents(workingInstallFromCache);

                this.Add(install, workingInstallFromCache);

                workingInstallFromCache.InstallPackage(executeActions, executeDirectly);
            }
        }

        /// <summary>   Handles the events described by workingInstallFromCache. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="workingInstallFromCache">  The working install from cache. </param>

        private void HandleEvents(PackageWorkingInstallFromCache workingInstallFromCache)
        {
            UpdateCacheStatusEventHandler updateCacheStatusHandler;
            EventHandlerT<InstallAction> queueInstallHandler;
            EventHandlerT<KeyValuePair<string, PackageWorkingInstallFromCache>> addToAllDependenciesHandler;
            AddInstallStatusEventHandler addInstallStatusHandler;

            updateCacheStatusHandler = (sender, e) =>
            {
                e.HandledBy.Add(this);

                internalOnUpdateCacheStatus(sender, e);
            };

            queueInstallHandler = (sender, e) =>
            {
                if (this.IsRoot)
                {
                    this.InstallActionsQueue.Enqueue(e.Value);
                }
                else
                {
                    OnQueueInstallAction.Raise(sender, e.Value);
                }
            };

            addToAllDependenciesHandler = (sender, e) =>
            {
                var pair = e.Value;

                if (this.IsRoot)
                {
                    this.AllDependenciesToProcess.Add(pair.Key, pair.Value);
                }
                else
                {
                    OnAddToAllDependencies.Raise(sender, pair);
                }
            };

            addInstallStatusHandler = (sender, e) =>
            {
                if (this.IsRoot)
                {
                    this.AddInstallStatus(sender, e.InstallFromCache, e.StatusMode, e.Status, e.Args);
                }
                else
                {
                    this.AddInstallStatus(e.InstallFromCache, e.StatusMode, e.Status, e.Args);
                }
            };

            updateCacheStatusHandlers.Add(updateCacheStatusHandler);
            queueInstallHandlers.Add(queueInstallHandler);
            addToAllDependenciesHandlers.Add(addToAllDependenciesHandler);
            addInstallStatusHandlers.Add(addInstallStatusHandler);

            workingInstallFromCache.OnUpdateCacheStatus += updateCacheStatusHandler;
            workingInstallFromCache.OnQueueInstallAction += queueInstallHandler;
            workingInstallFromCache.OnAddToAllDependencies += addToAllDependenciesHandler;
            workingInstallFromCache.OnAddInstallStatus += addInstallStatusHandler;
        }

        /// <summary>   Adds install. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="install">                  The install. </param>
        /// <param name="workingInstallFromCache">  The working install from cache. </param>

        private void Add(string install, PackageWorkingInstallFromCache workingInstallFromCache)
        {
            this.DependenciesFromCacheToProcess.AddToDictionaryIfNotExist(install, workingInstallFromCache);

            if (this.IsRoot)
            {
                this.AllDependenciesToProcess.AddToDictionaryIfNotExist(install, workingInstallFromCache);
            }
            else
            {
                OnAddToAllDependencies.Raise(this, new KeyValuePair<string, PackageWorkingInstallFromCache>(install, workingInstallFromCache));
            }
        }

        /// <summary>   Query if 'install' contains key. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="install">  The install. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        private bool ContainsKey(string install)
        {
            var root = this.Root;

            if (this.DependenciesFromCacheToProcess.ContainsInstallKey(root, install))
            {
                return true;
            }
            else
            {
                return root.AllDependenciesToProcess.ContainsInstallKey(root, install);
            }
        }

        /// <summary>   Gets the root. </summary>
        ///
        /// <value> The root. </value>

        public PackageWorkingInstallFromCache Root
        {
            get
            {
                var workingInstallParent = this.Parent;
                PackageWorkingInstallFromCache root = null;

                if (this.Parent == null)
                {
                    return this;
                }
                else
                {
                    while (workingInstallParent != null)
                    {
                        root = workingInstallParent;
                        workingInstallParent = workingInstallParent.Parent;
                    }

                    return root;
                }
            }
        }

        /// <summary>   Updates the cache status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="incrementKind">    The increment kind. </param>
        /// <param name="increment">        (Optional) Amount to increment by. </param>

        private void UpdateCacheStatus(IncrementKind incrementKind, int increment = 1)
        {
            var args = new CacheStatusEventArgs(incrementKind, this, increment);

            internalOnUpdateCacheStatus(this, args);

            if (!(args.HandledBy.Last() is PackageCacheService))
            {
                DebugUtils.Break();
            }
        }

        /// <summary>   Returns a string that represents the current object. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <returns>   A string that represents the current object. </returns>

        public override string ToString()
        {
            return this.DebugInfo;
        }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        public string DebugInfo
        {
            get
            {
                if (this.InstallToLocalModules && this.IsScoped)
                {
                    return string.Format("Install: {0}, "
                        + "InstallToLocalModules: {1}, "
                        + "IsScoped: {2}, "
                        + "CachePath: '{3}', "
                        + "PackagePath: '{4}'",
                        this.Install,
                        this.InstallToLocalModules,
                        this.IsScoped,
                        this.CachePath,
                        this.PackagePath
                    );
                }
                else if (this.InstallToLocalModules)
                {
                    return string.Format("Install: {0}, "
                        + "InstallToLocalModules: {1}, "
                        + "CachePath: '{2}', "
                        + "PackagePath: '{3}'",
                        this.Install,
                        this.InstallToLocalModules,
                        this.CachePath,
                        this.PackagePath
                    );
                }
                else if (this.IsScoped)
                {
                    return string.Format("Install: {0}, "
                        + "IsScoped: {1}, "
                        + "CachePath: '{2}', "
                        + "PackagePath: '{3}'",
                        this.Install,
                        this.IsScoped,
                        this.CachePath,
                        this.PackagePath
                    );
                }
                else
                {
                    return string.Format("Install: {0}, "
                        + "CachePath: '{1}', "
                        + "PackagePath: '{2}'",
                        this.Install,
                        this.CachePath,
                        this.PackagePath
                    );
                }
            }
        }
    }
}
