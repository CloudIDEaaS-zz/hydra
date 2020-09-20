using AbstraX.PackageCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Utils;

namespace AbstraX.Controllers
{
    public class StatusController : ApiController
    {
        private IPackageCacheStatusProvider packageCacheStatusProvider;
        private static PackageInstallsFromCacheStatus installsFromCacheStatusTesting;

        public StatusController(IPackageCacheStatusProvider packageCacheStatusProvider)
        {
            this.packageCacheStatusProvider = packageCacheStatusProvider;
        }

        public PackageCacheStatusInfo GetCacheStatus(string mode, bool setAsReported = false)
        {
            return packageCacheStatusProvider.GetCacheStatus(mode, setAsReported);
        }

        public PackageInstallsFromCacheStatus GetInstallFromCacheStatus(string mode)
        {
#if DEBUG
            if (mode == "Testing")
            {
                if (installsFromCacheStatusTesting == null)
                {
                    installsFromCacheStatusTesting = new PackageInstallsFromCacheStatus("Testing");

                    10.Countdown((n) =>
                    {
                        installsFromCacheStatusTesting.IncrementAll();
                    });
                }
                else
                {
                    installsFromCacheStatusTesting.RequestedRemaining--;
                    installsFromCacheStatusTesting.TotalRemaining--;
                }

                return installsFromCacheStatusTesting;
            }
#endif
            return packageCacheStatusProvider.GetInstallFromCacheStatus(mode);
        }
    }
}
