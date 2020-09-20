using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.PackageCache
{
    public class PackageCacheStatusProvider : IPackageCacheStatusProvider
    {
        internal IPackageCacheStatusProvider Source { get; set; }

        public PackageCacheStatusInfo GetCacheStatus(string mode, bool setAsReported = false)
        {
            if (this.Source == null)
            {
                return PackageCacheStatusInfo.Empty;
            }
            else
            {
                return this.Source.GetCacheStatus(mode, setAsReported);
            }
        }

        public PackageInstallsFromCacheStatus GetInstallFromCacheStatus(string mode)
        {
            if (this.Source == null)
            {
                return new PackageInstallsFromCacheStatus("No source to report from");
            }
            else
            {
                return this.Source.GetInstallFromCacheStatus(mode);
            }
        }
    }
}
