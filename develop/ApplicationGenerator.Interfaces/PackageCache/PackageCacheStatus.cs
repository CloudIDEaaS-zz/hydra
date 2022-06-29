// file:	PackageCache\PackageCacheStatus.cs
//
// summary:	Implements the package cache status class

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.PackageCache
{
    /// <summary>   A package cache status. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>

    [DebuggerDisplay(" { StatusText } ")]
    public class PackageCacheStatus
    {
        /// <summary>   Gets or sets the cache status. </summary>
        ///
        /// <value> The cache status. </value>

        public string CacheStatus { get; set; }

        /// <summary>   Gets or sets the current action verb. </summary>
        ///
        /// <value> The current action verb. </value>

        public string CurrentActionVerb { get; set; }

        /// <summary>   Gets or sets the install status. </summary>
        ///
        /// <value> The install status. </value>

        public string InstallStatus { get; set; }

        /// <summary>   Gets or sets the zero-based index of the sweep. </summary>
        ///
        /// <value> The sweep index. </value>

        public int SweepIndex { get; set; }

        /// <summary>   Gets or sets the number of sweeps. </summary>
        ///
        /// <value> The number of sweeps. </value>

        public int SweepCount { get; set; }

        /// <summary>   Gets or sets the Date/Time of the last sweep start. </summary>
        ///
        /// <value> The last sweep start. </value>

        public DateTime LastSweepStart { get; set; }

        /// <summary>   Gets or sets the number of adding to caches. </summary>
        ///
        /// <value> The number of adding to caches. </value>

        public int AddingToCacheCount { get; set; }

        /// <summary>   Gets or sets the number of processings. </summary>
        ///
        /// <value> The number of processings. </value>

        public int ProcessingCount { get; set; }

        /// <summary>   Gets or sets the number of added to caches. </summary>
        ///
        /// <value> The number of added to caches. </value>

        public int AddedToCacheCount { get; set; }

        /// <summary>   Gets or sets the number of with errors. </summary>
        ///
        /// <value> The number of with errors. </value>

        public int WithErrorsCount { get; set; }

        /// <summary>   Gets or sets the installs from cache. </summary>
        ///
        /// <value> The installs from cache. </value>

        public string[] InstallsFromCache { get; set; }

        /// <summary>   Gets or sets the copied to cache. </summary>
        ///
        /// <value> The copied to cache. </value>

        public string[] CopiedToCache { get; set; }

        /// <summary>   Gets or sets the install errors from cache. </summary>
        ///
        /// <value> The install errors from cache. </value>

        public string[] InstallErrorsFromCache { get; set; }

        /// <summary>   Gets or sets the status text. </summary>
        ///
        /// <value> The status text. </value>

        public string StatusText { get; set; }

        /// <summary>   Gets or sets the status progress percent. </summary>
        ///
        /// <value> The status progress percent. </value>

        public int StatusProgressPercent { get; set; }

        /// <summary>   Gets or sets the memory status. </summary>
        ///
        /// <value> The memory status. </value>

        public MemoryStatus MemoryStatus { get; set; }

        /// <summary>   Gets or sets a value indicating whether the no caching. </summary>
        ///
        /// <value> True if no caching, false if not. </value>

        public bool NoCaching { get; set; }

        /// <summary>   Gets or sets a value indicating whether the no install from cache. </summary>
        ///
        /// <value> True if no install from cache, false if not. </value>

        public bool NoInstallFromCache { get; set; }
    }
}
