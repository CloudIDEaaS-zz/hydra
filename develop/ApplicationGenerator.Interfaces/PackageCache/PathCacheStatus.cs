// file:	PackageCache\PathCacheStatus.cs
//
// summary:	Implements the path cache status class

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.PackageCache
{
    /// <summary>   Values that represent cache status types. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>

    public enum CacheStatusType
    {
        /// <summary>   An enum constant representing the none option. </summary>
        None,
        /// <summary>   An enum constant representing the path added to list option. </summary>
        PathAddedToList,
        /// <summary>   An enum constant representing the path processing option. </summary>
        PathProcessing,
        /// <summary>   An enum constant representing the path processed option. </summary>
        PathProcessed,
        /// <summary>   An enum constant representing the path has errors option. </summary>
        PathHasErrors,
        /// <summary>   An enum constant representing the path copied from cache option. </summary>
        PathCopiedFromCache,
        /// <summary>   An enum constant representing the path found in cache error option. </summary>
        PathFoundInCacheError,
        /// <summary>   An enum constant representing the not started option. </summary>
        NotStarted,
        /// <summary>   An enum constant representing the initialized option. </summary>
        Initialized,
        /// <summary>   An enum constant representing the service started option. </summary>
        ServiceStarted,
        /// <summary>   An enum constant representing the watching for packages option. </summary>
        WatchingForPackages,
        /// <summary>   An enum constant representing the adding paths to list option. </summary>
        AddingPathsToList,
        /// <summary>   An enum constant representing the installing option. </summary>
        Installing,
        /// <summary>   An enum constant representing the processing option. </summary>
        Processing,
        /// <summary>   An enum constant representing the sweeping files option. </summary>
        SweepingFiles,
        /// <summary>   An enum constant representing the end of processing option. </summary>
        EndOfProcessing,
        /// <summary>   An enum constant representing the last pass processing option. </summary>
        LastPassProcessing,
        /// <summary>   An enum constant representing the service stopping option. </summary>
        ServiceStopping,
        /// <summary>   An enum constant representing the service stopped option. </summary>
        ServiceStopped
    }

    /// <summary>   Values that represent install status types. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>

    public enum InstallStatusType
    {
        /// <summary>   An enum constant representing the none option. </summary>
        None,
        /// <summary>   An enum constant representing the installs started option. </summary>
        InstallsStarted,
        /// <summary>   An enum constant representing the installs complete option. </summary>
        InstallsComplete,
        /// <summary>   An enum constant representing the finalized option. </summary>
        Finalized
    }

    /// <summary>   A path cache status. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>

    [DebuggerDisplay(" { DebugInfo }")]
    public struct PathCacheStatus
    {
        /// <summary>   Gets or sets the Date/Time of the timestamp. </summary>
        ///
        /// <value> The timestamp. </value>

        public DateTime Timestamp { get; set; }

        /// <summary>   Gets or sets the install. </summary>
        ///
        /// <value> The install. </value>

        public string Install { get; set; }

        /// <summary>   Gets or sets the full pathname of the file. </summary>
        ///
        /// <value> The full pathname of the file. </value>

        public string Path { get; set; }

        /// <summary>   Gets or sets the Date/Time of the last status reported. </summary>
        ///
        /// <value> The last status reported. </value>

        public DateTime LastStatusReported { get; set; }

        /// <summary>   Gets or sets the type of the path cache status. </summary>
        ///
        /// <value> The type of the path cache status. </value>

        public CacheStatusType PathCacheStatusType { get; set; }

        /// <summary>   Gets or sets the exception. </summary>
        ///
        /// <value> The exception. </value>

        public Exception Exception { get; set; }

        /// <summary>   Gets or sets a message describing the status. </summary>
        ///
        /// <value> A message describing the status. </value>

        public string StatusMessage { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is sub path. </summary>
        ///
        /// <value> True if this  is sub path, false if not. </value>

        public bool IsSubPath { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>
        ///
        /// <param name="path"> Full pathname of the file. </param>
        /// <param name="type"> The type. </param>

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

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>
        ///
        /// <param name="path">     Full pathname of the file. </param>
        /// <param name="install">  The install. </param>
        /// <param name="type">     The type. </param>

        public PathCacheStatus(string path, string install, CacheStatusType type) : this(path, type)
        {
            this.Install = install;
            this.Timestamp = DateTime.Now;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>
        ///
        /// <param name="path">         Full pathname of the file. </param>
        /// <param name="install">      The install. </param>
        /// <param name="exception">    The exception. </param>

        public PathCacheStatus(string path, string install, Exception exception) : this(path, CacheStatusType.PathFoundInCacheError)
        {
            this.Install = install;
            this.Exception = exception;
            this.Timestamp = DateTime.Now;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>
        ///
        /// <param name="path">     Full pathname of the file. </param>
        /// <param name="type">     The type. </param>
        /// <param name="message">  The message. </param>

        public PathCacheStatus(string path, CacheStatusType type, string message) : this(path, type)
        {
            this.StatusMessage = message;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/18/2021. </remarks>
        ///
        /// <param name="path">         Full pathname of the file. </param>
        /// <param name="exception">    The exception. </param>

        public PathCacheStatus(string path, Exception exception) : this(path, CacheStatusType.PathHasErrors, exception.Message)
        {
            this.Exception = exception;
        }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

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
