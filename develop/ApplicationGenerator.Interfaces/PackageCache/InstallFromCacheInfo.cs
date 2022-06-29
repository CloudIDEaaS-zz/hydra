using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.PackageCache
{
    /// <summary>   Information about the install from cache. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/30/2021. </remarks>

    public class InstallFromCacheInfo
    {
        /// <summary>   Gets the header. </summary>
        ///
        /// <value> The header. </value>

        [JsonIgnore]
        public string Header { get; }

        /// <summary>   Gets or sets the current install. </summary>
        ///
        /// <value> The current install. </value>

        public string CurrentInstall { get; set; }

        /// <summary>   Gets the package installs from cache status. </summary>
        ///
        /// <value> The package installs from cache status. </value>

        public PackageInstallsFromCacheStatus PackageInstallsFromCacheStatus { get; set; }

        /// <summary>   Gets the root installs. </summary>
        ///
        /// <value> The root installs. </value>

        public List<InstallFromCachePackageInfo> RootInstalls { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/30/2021. </remarks>

        public InstallFromCacheInfo()
        {
            this.RootInstalls = new List<InstallFromCachePackageInfo>();
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/30/2021. </remarks>
        ///
        /// <param name="packageInstallsFromCacheStatus">   The package installs from cache status. </param>
        /// <param name="packageWorkingInstalls">           The package working installs. </param>
        /// <param name="currentInstallFromCache">          The current install from cache. </param>
        /// <param name="postInstall"></param>

        public InstallFromCacheInfo(PackageInstallsFromCacheStatus packageInstallsFromCacheStatus, List<PackageWorkingInstallFromCache> packageWorkingInstalls, PackageWorkingInstallFromCache currentInstallFromCache, bool postInstall = false)
        {
            this.PackageInstallsFromCacheStatus = packageInstallsFromCacheStatus;
            this.Header = string.Format("{0} {1} {2}", postInstall ? "Finishing" : "Adding", currentInstallFromCache.Install, "*".Repeat(25));
            this.CurrentInstall = currentInstallFromCache.Install;

            this.RootInstalls = new List<InstallFromCachePackageInfo>();

            AddInstalls(this.RootInstalls, packageWorkingInstalls);
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/30/2021. </remarks>
        ///
        /// <param name="packageInstallsFromCacheStatus">   The package installs from cache status. </param>
        /// <param name="packageWorkingInstalls">           The package working installs. </param>

        public InstallFromCacheInfo(PackageInstallsFromCacheStatus packageInstallsFromCacheStatus, List<PackageWorkingInstallFromCache> packageWorkingInstalls)
        {
            this.PackageInstallsFromCacheStatus = packageInstallsFromCacheStatus;
            this.Header = "Debug log";

            this.RootInstalls = new List<InstallFromCachePackageInfo>();

            AddInstalls(this.RootInstalls, packageWorkingInstalls);
        }

        private void AddInstalls(List<InstallFromCachePackageInfo> installInfos, List<PackageWorkingInstallFromCache> installs)
        {
            foreach (var install in installs)
            {
                var installInfo = new InstallFromCachePackageInfo(install);

                installInfos.Add(installInfo);

                AddInstalls(installInfo.DependencyInstalls, install.DependenciesFromCacheToProcess.Values.ToList());
            }
        }
    }
}
