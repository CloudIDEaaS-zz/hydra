using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace PackageCacheStatus
{
    public class CacheStatusProperties
    {
        public event PropertyChangedEventHandler PropertyChanged;
        [Category("Environment"), Description("The working directory currently being processed")]
        public string CurrentWorkingDirectory { get; internal set; }
        [Category("Environment"), Description("Location of the pakage cache")]
        public string PackageCachePath { get; internal set; }
        [Category("Statistics"), Description("Number of folders queued to be added to the cache")]
        public int AddingToCacheCount { get; internal set; }
        [Category("Statistics"), Description("Number of folders being processed")]
        public int ProcessingCount { get; internal set; }
        [Category("Statistics"), Description("Number of folders added to the cache")]
        public int AddedToCacheCount { get; internal set; }
        [Category("Statistics"), Description("Number of folders with errors during processing")]
        public int WithErrorsCount { get; internal set; }
        [Category("Listings"), Description("List of installs copied from the cache")]
        public string[] InstallsFromCache { get; internal set; }
        [Category("Listings"), Description("List top level folders copied to the cache")]
        public string[] CopiedToCache { get; internal set; }
        [Category("Listings"), Description("List of installs with errors copied from cache")]
        public string[] InstallErrorsFromCache { get; internal set; }
        [Category("Cache Status"), Description("Status of npm installs")]
        public string InstallStatus { get; internal set; }
        [Category("Cache Status"), Description("Status text"), DisplayName("Status")]
        public string CacheStatus { get; internal set; }
        [Category("Cache Status"), Description("Status text")]
        public string CacheStatusText { get; internal set; }
        [Category("Cache Status"), Description("Last update date/time from cache service")]
        public string LastUpdate { get; internal set; }
        [Category("Cache Status"), Description("Last attempt for update fom cache service")]
        public string LastAttemptedUpdate { get; internal set; }
        [Category("Cache Status"), Description("Error on last attempt to receive update from cache service")]
        public string LastAttemptedError { get; internal set; }
        [Category("Performance"), Description("Memory status of the cache service")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MemoryStatusProperties MemoryStatus { get; internal set; }
        [Category("Install From Cache Status"), Description("Total packages installing")]
        public int Total { get; internal set; }
        [Category("Install From Cache Status"), Description("Total packages remaining")]
        public int TotalRemaining { get; internal set; }
        [Category("Install From Cache Status"), Description("Requested packages installing")]
        public int Requested { get; internal set; }
        [Category("Install From Cache Status"), Description("Requested packages remaining")]
        public int RequestedRemaining { get; internal set; }
        [Category("Install From Cache Status"), Description("Status text"), DisplayName("Status")]
        public string StatusSummary { get; internal set; }
        [Category("Install From Cache Status"), Description("Status text")]
        public string StatusText { get; internal set; }

        public CacheStatusProperties()
        {
            this.StatusText = "No report";
        }

        public void RaisePropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            PropertyChanged(sender, eventArgs);
        }
    }
}
