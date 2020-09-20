using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.PackageCache
{
    public enum StatusMode
    {
        Normal,
        Error,
        Success
    }

    public class PackageInstallFromCacheStatus
    {
        public string StatusText { get; }
        public bool StatusTextIsError { get; }
        public bool StatusTextIsSuccess { get; }
        public string Install { get; set; }
        public string CacheInstall { get; set; }
        public string CachePath { get; set; }
        public string PackagePath { get; set; }
        public DateTime TimeReported { get; set; }

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

    [DebuggerDisplay(" { StatusText } ")]
    public class PackageInstallsFromCacheStatus
    {
        public int Total { get; set; }
        public int TotalRemaining { get; set; }
        public int Requested { get; set; }
        public int RequestedRemaining { get; set; }
        public PackageInstallFromCacheStatus[] InstallFromCacheStatus { get; set; }
        public string StatusSummary { get; set; }
        public bool StatusIsError { get; set; }
        public bool StatusIsSuccess { get; set; }
        public bool NothingToPoll { get; set; }

        public PackageInstallsFromCacheStatus(string statusSummary)
        {
            this.StatusSummary = statusSummary;
            this.InstallFromCacheStatus = new PackageInstallFromCacheStatus[0];
        }

        public void IncrementAll()
        {
            this.Total = ++this.Requested;
            this.RequestedRemaining = this.Requested;
            this.TotalRemaining = this.Total;
        }

        public void IncrementTotal()
        {
            this.TotalRemaining = ++this.Total;
        }

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