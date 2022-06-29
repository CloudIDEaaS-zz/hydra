// file:	PackageCache\InstallAction.cs
//
// summary:	Implements the install action class

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.PackageCache
{
    /// <summary>   An install action. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/30/2021. </remarks>

    public class InstallAction
    {
        /// <summary>   Gets or sets the working install. </summary>
        ///
        /// <value> The working install. </value>

        public PackageWorkingInstallFromCache WorkingInstall { get; set; }

        /// <summary>   Gets or sets the action. </summary>
        ///
        /// <value> The action. </value>

        public Action<PackageWorkingInstallFromCache, DirectoryInfo, DirectoryInfo> Action { get; set; }

        /// <summary>   Gets or sets the pathname of the cache directory. </summary>
        ///
        /// <value> The pathname of the cache directory. </value>

        public DirectoryInfo CacheDirectory { get; set; }

        /// <summary>   Gets or sets the pathname of the package directory. </summary>
        ///
        /// <value> The pathname of the package directory. </value>

        public DirectoryInfo PackageDirectory { get; set; }

        /// <summary>   Gets or sets the creation time. </summary>
        ///
        /// <value> The creation time. </value>

        public DateTime CreationTime { get; set; }

        /// <summary>   Gets or sets the executed time. </summary>
        ///
        /// <value> The executed time. </value>

        public DateTime ExecutedTime { get; set; }

        /// <summary>   Gets or sets the exception. </summary>
        ///
        /// <value> The exception. </value>

        public Exception Exception { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/30/2021. </remarks>

        public InstallAction()
        {
            this.CreationTime = DateTime.Now;
        }
    }
}
