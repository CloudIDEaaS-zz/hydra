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
    public class PackageCacheStatus
    {
        public string CacheStatus { get; set; }
        public string CurrentActionVerb { get; set; }
        public string InstallStatus { get; set; }
        public int SweepIndex { get; set; }
        public int SweepCount { get; set; }
        public DateTime LastSweepStart { get; set; }
        public int AddingToCacheCount { get; set; }
        public int ProcessingCount { get; set; }
        public int AddedToCacheCount { get; set; }
        public int WithErrorsCount { get; set; }
        public string[] InstallsFromCache { get; set; }
        public string[] CopiedToCache { get; set; }
        public string[] InstallErrorsFromCache { get; set; }
        public string StatusText { get; set; }
        public MemoryStatus MemoryStatus { get; set; }
        public bool NoCaching { get; set; }
        public bool NoInstallFromCache { get; set; }
    }
}
