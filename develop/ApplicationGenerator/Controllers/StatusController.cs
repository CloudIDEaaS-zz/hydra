// file:	Controllers\StatusController.cs
//
// summary:	Implements the status controller class

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
    /// <summary>   A controller for handling status. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/15/2021. </remarks>

    public class StatusController : ApiController
    {
        /// <summary>   The package cache status provider. </summary>
        private IPackageCacheStatusProvider packageCacheStatusProvider;
        /// <summary>   The installs from cache status testing. </summary>
        private static PackageInstallsFromCacheStatus installsFromCacheStatusTesting;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/15/2021. </remarks>
        ///
        /// <param name="packageCacheStatusProvider">   The package cache status provider. </param>

        public StatusController(IPackageCacheStatusProvider packageCacheStatusProvider)
        {
            this.packageCacheStatusProvider = packageCacheStatusProvider;
        }

        /// <summary>   Gets cache status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/15/2021. </remarks>
        ///
        /// <param name="mode">             The mode. </param>
        /// <param name="setAsReported">    (Optional) True if set as reported. </param>
        ///
        /// <returns>   The cache status. </returns>

        public PackageCacheStatusInfo GetCacheStatus(string mode, bool setAsReported = false)
        {
            return packageCacheStatusProvider.GetCacheStatus(mode, setAsReported);
        }

        /// <summary>   Gets install from cache status. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/15/2021. </remarks>
        ///
        /// <param name="mode"> The mode. </param>
        ///
        /// <returns>   The install from cache status. </returns>

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
