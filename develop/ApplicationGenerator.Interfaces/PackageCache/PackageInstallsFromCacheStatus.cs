// file:	PackageCache\PackageInstallsFromCacheStatus.cs
//
// summary:	Implements the package installs from cache status class

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.PackageCache
{
    /// <summary>   Values that represent status modes. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>

    public enum StatusMode
    {
        /// <summary>   An enum constant representing the normal option. </summary>
        Normal,
        /// <summary>   An enum constant representing the error option. </summary>
        Error,
        /// <summary>   An enum constant representing the success option. </summary>
        Success
    }

    /// <summary>   A package install from cache status. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>

    public class PackageInstallFromCacheStatus
    {
        /// <summary>   Gets the status text. </summary>
        ///
        /// <value> The status text. </value>

        public string StatusText { get; }

        /// <summary>   Gets a value indicating whether the status text is error. </summary>
        ///
        /// <value> True if status text is error, false if not. </value>

        public bool StatusTextIsError { get; }

        /// <summary>   Gets a value indicating whether the status text is success. </summary>
        ///
        /// <value> True if status text is success, false if not. </value>

        public bool StatusTextIsSuccess { get; }

        /// <summary>   Gets or sets the install. </summary>
        ///
        /// <value> The install. </value>

        public string Install { get; set; }

        /// <summary>   Gets or sets the cache install. </summary>
        ///
        /// <value> The cache install. </value>

        public string CacheInstall { get; set; }

        /// <summary>   Gets or sets the full pathname of the cache file. </summary>
        ///
        /// <value> The full pathname of the cache file. </value>

        public string CachePath { get; set; }

        /// <summary>   Gets or sets the full pathname of the package file. </summary>
        ///
        /// <value> The full pathname of the package file. </value>

        public string PackagePath { get; set; }

        /// <summary>   Gets or sets the Date/Time of the time reported. </summary>
        ///
        /// <value> The time reported. </value>

        public DateTime TimeReported { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>
        ///
        /// <param name="install">      The install. </param>
        /// <param name="cachePath">    Full pathname of the cache file. </param>
        /// <param name="packagePath">  Full pathname of the package file. </param>
        /// <param name="status">       The status. </param>
        /// <param name="mode">         The mode. </param>

        public PackageInstallFromCacheStatus(string install, string cachePath, string packagePath, string status, StatusMode mode)
        {
            Install = install;
            CachePath = cachePath;
            PackagePath = packagePath;
            this.StatusText = status;

            switch (mode)
            {
                case StatusMode.Error:
                    this.StatusTextIsError = true;
                    break;
                case StatusMode.Success:
                    this.StatusTextIsSuccess = true;
                    break;
            }
        }
    }

    /// <summary>   A package installs from cache status. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>

    [DebuggerDisplay(" { StatusText } ")]
    public class PackageInstallsFromCacheStatus
    {
        /// <summary>   Gets or sets the number of.  </summary>
        ///
        /// <value> The total. </value>

        public int Total { get; set; }

        /// <summary>   Gets or sets the total number of remaining. </summary>
        ///
        /// <value> The total number of remaining. </value>

        public int TotalRemaining { get; set; }

        /// <summary>   Gets or sets the requested. </summary>
        ///
        /// <value> The requested. </value>

        public int Requested { get; set; }

        /// <summary>   Gets or sets the requested remaining. </summary>
        ///
        /// <value> The requested remaining. </value>

        public int RequestedRemaining { get; set; }

        /// <summary>   Gets or sets the install from cache status. </summary>
        ///
        /// <value> The install from cache status. </value>

        public PackageInstallFromCacheStatus[] InstallFromCacheStatus { get; set; }

        /// <summary>   Gets or sets the status summary. </summary>
        ///
        /// <value> The status summary. </value>

        public string StatusSummary { get; set; }

        /// <summary>   Gets or sets a value indicating whether the status is error. </summary>
        ///
        /// <value> True if status is error, false if not. </value>

        public bool StatusIsError { get; set; }

        /// <summary>   Gets or sets a value indicating whether the status is success. </summary>
        ///
        /// <value> True if status is success, false if not. </value>

        public bool StatusIsSuccess { get; set; }

        /// <summary>   Gets or sets a value indicating whether the nothing to poll. </summary>
        ///
        /// <value> True if nothing to poll, false if not. </value>

        public bool NothingToPoll { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>
        ///
        /// <param name="statusSummary">    The status summary. </param>

        public PackageInstallsFromCacheStatus(string statusSummary)
        {
            this.StatusSummary = statusSummary;
            this.InstallFromCacheStatus = new PackageInstallFromCacheStatus[0];
        }

        /// <summary>   Increment all. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>

        public void IncrementAll()
        {
            this.Requested++;
            this.Total++;
            this.RequestedRemaining = this.Requested;
            this.TotalRemaining++;

            if (this.RequestedRemaining < 0)
            {
                DebugUtils.Break();
            }
        }

        /// <summary>   Increment total. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>

        public void IncrementTotal()
        {
            this.Total++;
            this.TotalRemaining++;
        }

        /// <summary>   Gets the status text. </summary>
        ///
        /// <value> The status text. </value>

        public string StatusText
        {
            get
            {
                var builder = new StringBuilder();

                builder.AppendLine();

                builder.AppendLineFormat("Package install from cache status: '{0}' **************************************\r\n", this.StatusSummary);

                builder.AppendLineFormatTabIndent("Total package install remaining: {0}", this.TotalRemaining);
                builder.AppendLineFormatTabIndent("Total package install count: {0}", this.Total);
                builder.AppendLineFormatTabIndent("Requested package install remaining count: {0}", this.RequestedRemaining);
                builder.AppendLineFormatTabIndent("Requested package install count: {0}", this.Requested);
                builder.AppendLineFormatTabIndent("Last update: {0}", DateTime.Now.ToDateTimeText());

                builder.AppendLine("\r\nEnd cache status: *****************************************************");

                return builder.ToString();
            }
        }

        /// <summary>   Adds the installs to 'now'. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>
        ///
        /// <param name="installsFromCacheToProcess">   The installs from cache to process. </param>
        /// <param name="now">                          The now Date/Time. </param>

        public void AddInstalls(Dictionary<string, PackageWorkingInstallFromCache> installsFromCacheToProcess, DateTime now)
        {
            var installFromCacheStatus = new List<PackageInstallFromCacheStatus>();

            foreach (var installPair in installsFromCacheToProcess)
            {
                var install = installPair.Key;
                var workingInstall = installPair.Value;

                foreach (var status in workingInstall.InstallStatusList.Where(i => i.TimeReported != DateTime.MinValue))
                {
                    status.TimeReported = now;
                    installFromCacheStatus.Add(status);
                }
            }

            if (installFromCacheStatus.Count > 0)
            {
                this.InstallFromCacheStatus = installFromCacheStatus.ToArray();
            }
        }
    }
}