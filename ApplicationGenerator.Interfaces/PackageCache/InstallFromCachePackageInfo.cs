using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.PackageCache
{
    /// <summary>   Information about the install from cache package. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/30/2021. </remarks>

    public class InstallFromCachePackageInfo
    {
        /// <summary>   Gets the install. </summary>
        ///
        /// <value> The install. </value>

        public string Install { get; set; }

        /// <summary>   Gets the dependency installs. </summary>
        ///
        /// <value> The dependency installs. </value>

        public List<InstallFromCachePackageInfo> DependencyInstalls { get; set; }

        /// <summary>   Gets a queue of actions. </summary>
        ///
        /// <value> A queue of actions. </value>

        public int ActionQueueCount { get; set; }

        /// <summary>   Gets or sets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        public string DebugInfo { get; set; }

        /// <summary>   Gets the state. </summary>
        ///
        /// <value> The state. </value>

        public string State { get; set; }

        /// <summary>   Gets or sets the exception. </summary>
        ///
        /// <value> The exception. </value>

        public string Exception { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/30/2021. </remarks>
        ///

        public InstallFromCachePackageInfo()
        {
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/30/2021. </remarks>
        ///
        /// <param name="packageWorkingInstallFromCache">   The package working install from cache. </param>

        public InstallFromCachePackageInfo(PackageWorkingInstallFromCache packageWorkingInstallFromCache)
        {
            this.DependencyInstalls = new List<InstallFromCachePackageInfo>();
            this.Install = packageWorkingInstallFromCache.Install;
            this.ActionQueueCount = packageWorkingInstallFromCache.InstallActionsQueue.Count;
            this.DebugInfo = packageWorkingInstallFromCache.DebugInfo;
            this.State = packageWorkingInstallFromCache.InstallFromCacheState.ToString();
            this.Exception = packageWorkingInstallFromCache.InstallException?.Message;
        }
    }
}
