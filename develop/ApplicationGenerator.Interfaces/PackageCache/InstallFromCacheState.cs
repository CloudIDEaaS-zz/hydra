// file:	PackageCache\InstallFromCacheState.cs
//
// summary:	Implements the install from cache state class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.PackageCache
{
    /// <summary>   Values that represent install from cache states. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/30/2021. </remarks>

    public enum InstallFromCacheState
    {
        /// <summary>   An enum constant representing the created option. </summary>
        Created,
        /// <summary>   An enum constant representing the queued option. </summary>
        Queued,
        /// <summary>   An enum constant representing the installing option. </summary>
        Installing,
        /// <summary>   An enum constant representing the installed option. </summary>
        Installed,
        /// <summary>   An enum constant representing the errored option. </summary>
        Complete,
        /// <summary>   An enum constant representing the errored option. </summary>
        Errored
    }
}
