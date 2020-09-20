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
    [DebuggerDisplay(" { StatusText } ")]
    public class PackageCacheStatusInfo
    {
        public string CacheStatus { get; set; }
        public string CurrentActionVerb { get; set; }
        public string InstallStatus { get; set; }
        public int SweepIndex { get; }
        public int SweepCount { get; }
        public DateTime LastSweepStart { get; }
        public MemoryStatus MemoryStatus { get; }
        public bool NoCaching { get; set; }
        public bool NoInstallFromCache { get; set; }

        private Dictionary<string, List<PathCacheStatus>> pathsProcessed;
        private Dictionary<string, List<PathCacheStatus>> pathsProcessedDelta;
        private string statusText;

        public PackageCacheStatusInfo(string overallStatus, string statusText)
        {
            this.statusText = statusText;
            this.CacheStatus = overallStatus;
        }

        public PackageCacheStatusInfo(CacheStatusType cacheStatusType, string currentActionVerb, InstallStatusType installStatusType, int sweepIndex, int sweepCount, DateTime lastSweepStart, DateTime lastCacheStatusRequest, Dictionary<string, List<PathCacheStatus>> pathsProcessed, MemoryStatus memoryStatus)
        {
            this.CacheStatus = cacheStatusType.ToString();
            this.InstallStatus = installStatusType.ToString();
            this.SweepIndex = sweepIndex;
            this.SweepCount = sweepCount;
            this.LastSweepStart = lastSweepStart;

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

        public static int GetPathErrorCount(string path, Dictionary<string, List<PathCacheStatus>> pathsProcessed)
        {
            return pathsProcessed[path].Count(s => s.Exception != null);
        }

        public static PackageCacheStatusInfo Empty
        {
            get
            {
                return new PackageCacheStatusInfo(CacheStatusType.NotStarted, string.Empty, InstallStatusType.None, 0, 0, DateTime.MinValue, DateTime.MinValue, new Dictionary<string, List<PathCacheStatus>>(), null);
            }
        }

        public void SetLastStatusReported(Dictionary<string, List<PathCacheStatus>> pathsProcessed, DateTime lastCacheStatusRequest)
        {
            pathsProcessed.ForEach(p => p.Value.ForEach(s =>
            {
                var debugInfo = s.DebugInfo;

                s.LastStatusReported = lastCacheStatusRequest;
            }));
        }

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

                    builder.AppendLineFormatTabIndent("Adding to cache path count: {0}", this.AddingToCacheCount);
                    builder.AppendLineFormatTabIndent("Processing path count: {0}", this.ProcessingCount);
                    builder.AppendLineFormatTabIndent("Added to cache path count: {0}", this.AddedToCacheCount);
                    builder.AppendLineFormatTabIndent("Paths with errors count: {0}", this.WithErrorsCount);
                    builder.AppendLineFormatTabIndent("Copied to cache count: {0}", this.CopiedToCache.Length);
                    builder.AppendLineFormatTabIndent("Installed from cache count: {0}", this.InstallsFromCache.Length);
                    builder.AppendLineFormatTabIndent("Install from cache error count: {0}", this.InstallErrorsFromCache.Length);
                    builder.AppendLineFormatTabIndent("Last update: {0}", DateTime.Now.ToDateTimeText());

                    builder.AppendLine("\r\nEnd cache status: *****************************************************");

                    return builder.ToString();
                }
            }
        }
    }
}
