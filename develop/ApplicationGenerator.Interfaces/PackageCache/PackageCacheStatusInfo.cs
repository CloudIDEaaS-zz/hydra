// file:	PackageCache\PackageCacheStatusInfo.cs
//
// summary:	Implements the package cache status information class

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
    /// <summary>   Information about the package cache status. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/16/2021. </remarks>

    [DebuggerDisplay(" { StatusText } ")]
    public class PackageCacheStatusInfo
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

        /// <summary>   Gets the zero-based index of the sweep. </summary>
        ///
        /// <value> The sweep index. </value>

        public int SweepIndex { get; }

        /// <summary>   Gets the number of sweeps. </summary>
        ///
        /// <value> The number of sweeps. </value>

        public int SweepCount { get; }

        /// <summary>   Gets the Date/Time of the last sweep start. </summary>
        ///
        /// <value> The last sweep start. </value>

        public DateTime LastSweepStart { get; }

        /// <summary>   Gets the memory status. </summary>
        ///
        /// <value> The memory status. </value>

        public MemoryStatus MemoryStatus { get; }

        /// <summary>   Gets or sets a value indicating whether the no caching. </summary>
        ///
        /// <value> True if no caching, false if not. </value>

        public bool NoCaching { get; set; }

        /// <summary>   Gets or sets a value indicating whether the no install from cache. </summary>
        ///
        /// <value> True if no install from cache, false if not. </value>

        public bool NoInstallFromCache { get; set; }

        /// <summary>   The paths processed. </summary>
        private Dictionary<string, List<PathCacheStatus>> pathsProcessed;
        /// <summary>   The paths processed delta. </summary>
        private Dictionary<string, List<PathCacheStatus>> pathsProcessedDelta;
        /// <summary>   The status text. </summary>
        private string statusText;
        private int statusProgressPercent;
        private const bool ShowDetailedStatusInfo = false;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/16/2021. </remarks>
        ///
        /// <param name="overallStatus">            The overall status. </param>
        /// <param name="statusText">               The status text. </param>
        /// <param name="statusProgressPercent">    The status progress percent. </param>

        public PackageCacheStatusInfo(string overallStatus, string statusText, int statusProgressPercent)
        {
            this.statusText = statusText;
            this.CacheStatus = overallStatus;
            this.statusProgressPercent = statusProgressPercent;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/16/2021. </remarks>
        ///
        /// <param name="cacheStatusType">          Type of the cache status. </param>
        /// <param name="currentActionVerb">        The current action verb. </param>
        /// <param name="installStatusType">        Type of the install status. </param>
        /// <param name="sweepIndex">               Zero-based index of the sweep. </param>
        /// <param name="sweepCount">               Number of sweeps. </param>
        /// <param name="lastSweepStart">           The last sweep start Date/Time. </param>
        /// <param name="lastCacheStatusRequest">   The last cache status request Date/Time. </param>
        /// <param name="pathsProcessed">           The paths processed. </param>
        /// <param name="statusProgressPercent">    The status progress percent. </param>
        /// <param name="memoryStatus">             The memory status. </param>

        public PackageCacheStatusInfo(CacheStatusType cacheStatusType, string currentActionVerb, InstallStatusType installStatusType, int sweepIndex, int sweepCount, DateTime lastSweepStart, DateTime lastCacheStatusRequest, Dictionary<string, List<PathCacheStatus>> pathsProcessed, int statusProgressPercent, MemoryStatus memoryStatus)
        {
            this.CacheStatus = cacheStatusType.ToString();
            this.InstallStatus = installStatusType.ToString();
            this.SweepIndex = sweepIndex;
            this.SweepCount = sweepCount;
            this.LastSweepStart = lastSweepStart;
            this.statusProgressPercent = statusProgressPercent;

            Debug.Assert(currentActionVerb != null);

            this.CurrentActionVerb = currentActionVerb;

            this.pathsProcessed = pathsProcessed.ToDictionary();
            this.pathsProcessedDelta = pathsProcessed.Where(p => p.Value.Last().Timestamp >= lastCacheStatusRequest).ToDictionary();

            if (memoryStatus != null)
            {
                memoryStatus.Update();

                this.MemoryStatus = memoryStatus;
            }
        }

        /// <summary>   Gets path error count. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/16/2021. </remarks>
        ///
        /// <param name="path">             Full pathname of the file. </param>
        /// <param name="pathsProcessed">   The paths processed. </param>
        ///
        /// <returns>   The path error count. </returns>

        public static int GetPathErrorCount(string path, Dictionary<string, List<PathCacheStatus>> pathsProcessed)
        {
            return pathsProcessed[path].Count(s => s.Exception != null);
        }

        /// <summary>   Gets the empty. </summary>
        ///
        /// <value> The empty. </value>

        public static PackageCacheStatusInfo Empty
        {
            get
            {
                return new PackageCacheStatusInfo(CacheStatusType.NotStarted, string.Empty, InstallStatusType.None, 0, 0, DateTime.MinValue, DateTime.MinValue, new Dictionary<string, List<PathCacheStatus>>(), 0, null);
            }
        }

        /// <summary>   Sets last status reported. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/16/2021. </remarks>
        ///
        /// <param name="pathsProcessed">           The paths processed. </param>
        /// <param name="lastCacheStatusRequest">   The last cache status request Date/Time. </param>

        public void SetLastStatusReported(Dictionary<string, List<PathCacheStatus>> pathsProcessed, DateTime lastCacheStatusRequest)
        {
            pathsProcessed.ForEach(p => p.Value.ForEach(s =>
            {
                var debugInfo = s.DebugInfo;

                s.LastStatusReported = lastCacheStatusRequest;
            }));
        }

        /// <summary>   Gets the number of adding to caches. </summary>
        ///
        /// <value> The number of adding to caches. </value>

        public int AddingToCacheCount
        {
            get
            {
                if (pathsProcessedDelta == null)
                {
                    return 0;
                }

                return pathsProcessedDelta.Count(p => p.Value.Any(s => s.PathCacheStatusType == CacheStatusType.PathAddedToList) && !p.Value.Any(s => s.PathCacheStatusType == CacheStatusType.PathProcessed));
            }
        }

        /// <summary>   Gets the number of processings. </summary>
        ///
        /// <value> The number of processings. </value>

        public int ProcessingCount
        {
            get
            {
                if (pathsProcessedDelta == null)
                {
                    return 0;
                }

                return pathsProcessedDelta.Count(p => p.Value.Any(s => s.PathCacheStatusType == CacheStatusType.PathProcessing) && !p.Value.Any(s => s.PathCacheStatusType == CacheStatusType.PathProcessed));
            }
        }

        /// <summary>   Gets the number of added to caches. </summary>
        ///
        /// <value> The number of added to caches. </value>

        public int AddedToCacheCount
        {
            get
            {
                if (pathsProcessed == null)
                {
                    return 0;
                }

                return pathsProcessed.Count(p => p.Value.Any(s => s.PathCacheStatusType == CacheStatusType.PathProcessed));
            }
        }

        /// <summary>   Gets the number of with errors. </summary>
        ///
        /// <value> The number of with errors. </value>

        public int WithErrorsCount
        {
            get
            {
                if (pathsProcessed == null)
                {
                    return 0;
                }

                return pathsProcessed.Count(p => p.Value.Any(s => s.PathCacheStatusType == CacheStatusType.PathHasErrors) && !p.Value.Any(s => s.PathCacheStatusType == CacheStatusType.PathProcessed));
            }
        }

        /// <summary>   Gets the installs from cache. </summary>
        ///
        /// <value> The installs from cache. </value>

        public string[] InstallsFromCache
        {
            get
            {
                if (pathsProcessed == null)
                {
                    return new string[0];
                }

                return pathsProcessed.Where(p => p.Value.Any(s => s.PathCacheStatusType == CacheStatusType.PathCopiedFromCache)).Select(p => p.Value.Last(s => s.PathCacheStatusType == CacheStatusType.PathCopiedFromCache).Install).ToArray();
            }
        }

        /// <summary>   Gets the copied to cache. </summary>
        ///
        /// <value> The copied to cache. </value>

        public string[] CopiedToCache
        {
            get
            {
                if (pathsProcessed == null)
                {
                    return new string[0];
                }

                return pathsProcessed.Where(p => p.Value.Any(s => s.IsSubPath == false && s.PathCacheStatusType == CacheStatusType.PathProcessed)).Select(p => p.Key).ToArray();
            }
        }

        /// <summary>   Gets the install errors from cache. </summary>
        ///
        /// <value> The install errors from cache. </value>

        public string[] InstallErrorsFromCache
        {
            get
            {
                if (pathsProcessed == null)
                {
                    return new string[0];
                }

                return pathsProcessed.Where(p => p.Value.Any(s => s.PathCacheStatusType == CacheStatusType.PathFoundInCacheError)).Select(p => {

                    var lastError = p.Value.Last(s => s.PathCacheStatusType == CacheStatusType.PathFoundInCacheError);

                    return string.Format("Package '{0}' found in cache.  Error copying: {1}", lastError.Install, lastError.Exception.Message);

                }).ToArray();
            }
        }

        /// <summary>   Gets the status progress percent. </summary>
        ///
        /// <value> The status progress percent. </value>

        public int StatusProgressPercent
        {
            get
            {
                return statusProgressPercent;
            }
        }

        /// <summary>   Gets the status text. </summary>
        ///
        /// <value> The status text. </value>

        public string StatusText
        {
            get
            {
                if (statusText != null)
                {
                    return statusText;
                }
                else
                {
                    var builder = new StringBuilder();

                    builder.AppendLine();

                    builder.AppendLineFormat("Cache status: '{0}' **************************************\r\n", this.CacheStatus);

                    builder.AppendLineFormatTabIndent("Progress: {0}%", this.StatusProgressPercent);
                    builder.AppendLineFormatTabIndent("Processed: {0}", this.SweepIndex);
                    builder.AppendLineFormatTabIndent("Total count: {0}", this.SweepCount);
                    builder.AppendLineFormatTabIndent("Current action: {0}", this.CurrentActionVerb);

                    if (ShowDetailedStatusInfo)
                    {
                        builder.AppendLineFormatTabIndent("Adding to cache path count: {0}", this.AddingToCacheCount);
                        builder.AppendLineFormatTabIndent("Processing path count: {0}", this.ProcessingCount);
                        builder.AppendLineFormatTabIndent("Added to cache path count: {0}", this.AddedToCacheCount);
                        builder.AppendLineFormatTabIndent("Paths with errors count: {0}", this.WithErrorsCount);
                        builder.AppendLineFormatTabIndent("Copied to cache count: {0}", this.CopiedToCache.Length);
                        builder.AppendLineFormatTabIndent("Installed from cache count: {0}", this.InstallsFromCache.Length);
                        builder.AppendLineFormatTabIndent("Install from cache error count: {0}", this.InstallErrorsFromCache.Length);
                    }

                    builder.AppendLineFormatTabIndent("Last request: {{ lastRequest }}");
                    builder.AppendLineFormatTabIndent("Last update: {0}", DateTime.Now.ToDateTimeText());

                    builder.AppendLine("\r\nEnd cache status: *****************************************************");

                    return builder.ToString();
                }
            }
        }
    }
}
