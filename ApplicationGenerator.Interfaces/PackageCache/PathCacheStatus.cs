using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.PackageCache
{
    public enum CacheStatusType
    {
        None,
        PathAddedToList,
        PathProcessing,
        PathProcessed,
        PathHasErrors,
        PathCopiedFromCache,
        PathFoundInCacheError,
        NotStarted,
        Initialized,
        ServiceStarted,
        WatchingForPackages,
        AddingPathsToList,
        Installing,
        Processing,
        SweepingFiles,
        EndOfProcessing,
        LastPassProcessing,
        ServiceStopping,
        ServiceStopped
    }
    public enum InstallStatusType
    {
        None,
        InstallsStarted,
        InstallsComplete,
        Finalized
    }

    [DebuggerDisplay(" { DebugInfo }")]
    public struct PathCacheStatus
    {
        public DateTime Timestamp { get; set; }
        public string Install { get; set; }
        public string Path { get; set; }
        public DateTime LastStatusReported { get; set; }
        public CacheStatusType PathCacheStatusType { get; set; }
        public Exception Exception { get; set; }
        public string StatusMessage { get; set; }
        public bool IsSubPath { get; set; }

        public PathCacheStatus(string path, CacheStatusType type)
        {
            this.Path = path;
            this.PathCacheStatusType = type;
            this.Timestamp = DateTime.Now;
            this.LastStatusReported = DateTime.MinValue;
            this.StatusMessage = string.Empty;
            this.Exception = null;
            this.Install = string.Empty;
            this.IsSubPath = false;
        }

        public PathCacheStatus(string path, string install, CacheStatusType type) : this(path, type)
        {
            this.Install = install;
            this.Timestamp = DateTime.Now;
        }

        public PathCacheStatus(string path, string install, Exception exception) : this(path, CacheStatusType.PathFoundInCacheError)
        {
            this.Install = install;
            this.Exception = exception;
            this.Timestamp = DateTime.Now;
        }

        public PathCacheStatus(string path, CacheStatusType type, string message) : this(path, type)
        {
            this.StatusMessage = message;
        }

        public PathCacheStatus(string path, Exception exception) : this(path, CacheStatusType.PathHasErrors, exception.Message)
        {
            this.Exception = exception;
        }

        public string DebugInfo
        {
            get
            {
                var builder = new StringBuilder();

                builder.AppendFormat("Path: '{0}', ", this.Path);
                builder.AppendFormat("Status: '{0}'", this.PathCacheStatusType);

                if (this.LastStatusReported != DateTime.MinValue)
                {
                    builder.AppendWithLeadingIfLength(", ", "LastStatusReport: {0}", this.LastStatusReported.ToShortTimeString());
                }

                return builder.ToString();
            }
        }
    }
}
