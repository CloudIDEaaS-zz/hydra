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

namespace AbstraX.PackageCache
{
    [DebuggerDisplay(" { DebugInfo }")]
    public class PackageWorkingInstallFromCache
    {
        public string Mode { get; }
        public string Install { get; }
        public string CachePath { get; private set; }
        public string PackagePath { get; private set; }
        public List<PackageInstallFromCacheStatus> InstallStatusList { get; }
        public PackageWorkingInstallFromCache Parent { get; set; }
        public Dictionary<string, PackageWorkingInstallFromCache> DependenciesFromCacheToProcess { get; }
        public Dictionary<string, PackageWorkingInstallFromCache> AllDependenciesToProcess { get; }
        public bool InstallToLocalModules { get; set; }
        public bool IsScoped { get; set; }
        public Queue<InstallAction> InstallActionsQueue { get; }
        public NpmNodeModules PackageModules { get; }

        public event EventHandlerT<InstallAction> OnQueueInstallAction;
        public event EventHandlerT<KeyValuePair<string, PackageWorkingInstallFromCache>> OnAddToAllDependencies;
        public event AddInstallStatusEventHandler OnAddInstallStatus;
        private string nodeModulesPath;
        private string cachePathRoot;
        private bool incrementRaised;
        private event UpdateCacheStatusEventHandler internalOnUpdateCacheStatus;

        public PackageWorkingInstallFromCache(string mode, string install, string cachePath, string packagePath, NpmNodeModules packageModules, PackageWorkingInstallFromCache parent = null)
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

            nodeModulesPath = packageDirectory.GetParentPathToFolder("node_modules", true);
            cachePathRoot = cacheDirectory.GetParentPathToFolder("cache", true);

            this.DependenciesFromCacheToProcess = new Dictionary<string, PackageWorkingInstallFromCache>();
            this.AllDependenciesToProcess = new Dictionary<string, PackageWorkingInstallFromCache>();
        }

        public bool IsRoot
        {
            get
            {
                return this.Parent == null;
            }
        }

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

            this.ExecuteOrQueue(cacheDirectory, packageDirectory, (c, p) =>
            {
                AddInstallStatus(this, StatusMode.Normal, "Installing");

                c.CopyTo(p.FullName, true, false);

                UpdateCacheStatus(IncrementKind.RequestedRemaining, -1);

            }, executeDirectly);

            if (executeActions)
            {
                ExecuteActions();
            }
        }

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

        private void InstallPeerDependencies()
        {
            // kn - todo.  Have to scour the whole cache
        }

        private void AddInstallStatus(object sender, PackageWorkingInstallFromCache installFromCache, StatusMode mode, string status, params object[] args)
        {
            var installStatusEventArgs = new AddInstallStatusEventArgs(installFromCache, mode, status, args);

            OnAddInstallStatus(sender, installStatusEventArgs);
        }

        private void AddInstallStatus(PackageWorkingInstallFromCache installFromCache, StatusMode mode, string status, params object[] args)
        {
            var installStatusEventArgs = new AddInstallStatusEventArgs(installFromCache, mode, status, args);

            OnAddInstallStatus(this, installStatusEventArgs);
        }

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

        public void ExecuteActions()
        {
            while (this.InstallActionsQueue.Count > 0)
            {
                var installAction = this.InstallActionsQueue.Dequeue();

                try
                {
                    installAction.Action(installAction.CacheDirectory, installAction.PackageDirectory);
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

        private void ExecuteOrQueue(DirectoryInfo cacheDirectory, DirectoryInfo packageDirectory, Action<DirectoryInfo, DirectoryInfo> action, bool executeDirectly = false)
        {
            if (executeDirectly)
            {
                try
                {
                    action(cacheDirectory, packageDirectory);
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
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

        private void InstallDependency(string name, NpmVersion installVersion, DirectoryInfo cachePackageNodeModulesDirectory, bool executeActions = true, bool executeDirectly = false)
        {
            var cachePath = Path.Combine(cachePathRoot, name.BackSlashes());

            if (name.IsNullWhiteSpaceOrEmpty())
            {
                DebugUtils.Break();
            }

            InstallDependency(name, installVersion, cachePath, cachePackageNodeModulesDirectory, executeActions, executeDirectly);
        }

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

        internal bool ValidateTree(PackageInstallsFromCacheStatus installsFromCacheStatus)
        {
            var dependencies = this.GetDependencyCount(true);

            Debug.Assert(installsFromCacheStatus.Requested == 1);
            Debug.Assert(installsFromCacheStatus.Total == dependencies + 1);

            return true;
        }

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

        private void InstallFromCache(string name, NpmVersion installVersion, DirectoryInfo cacheDirectory, DirectoryInfo packageDirectory, bool executeActions = true, bool executeDirectly = false)
        {
            var install = name + "@" + installVersion.Version;

            if (!this.ContainsKey(install))
            {
                var workingInstallFromCache = new PackageWorkingInstallFromCache(this.Mode, install, cacheDirectory.FullName, packageDirectory.FullName, this.PackageModules, this);

                HandleEvents(workingInstallFromCache);

                this.Add(install, workingInstallFromCache);

                workingInstallFromCache.InstallPackage(executeActions, executeDirectly);
            }
        }

        private void HandleEvents(PackageWorkingInstallFromCache workingInstallFromCache)
        {
            workingInstallFromCache.OnUpdateCacheStatus += (sender, e) =>
            {
                internalOnUpdateCacheStatus(sender, e);
            };

            workingInstallFromCache.OnQueueInstallAction += (sender, e) =>
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

            workingInstallFromCache.OnAddToAllDependencies += (sender, e) =>
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

            workingInstallFromCache.OnAddInstallStatus += (sender, e) =>
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
        }

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

        private void UpdateCacheStatus(IncrementKind incrementKind, int increment = 1)
        {
            internalOnUpdateCacheStatus(this, new CacheStatusEventArgs(incrementKind, increment));
        }

        public override string ToString()
        {
            return this.DebugInfo;
        }

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
