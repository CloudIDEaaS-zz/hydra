// file:	PackageCache\CacheStatusEventArgs.cs
//
// summary:	Implements the cache status event arguments class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.PackageCache
{
    /// <summary>   Values that represent increment kinds. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/30/2021. </remarks>

    public enum IncrementKind
    {
        /// <summary>   An enum constant representing the increment total option. </summary>
        IncrementTotal,
        /// <summary>   An enum constant representing the increment all option. </summary>
        IncrementAll,
        /// <summary>   An enum constant representing the total option. </summary>
        TotalRemaining,
        /// <summary>   An enum constant representing the requested option. </summary>
        Requested,
        /// <summary>   An enum constant representing the requested remaining option. </summary>
        RequestedRemaining
    }

    /// <summary>   Delegate for handling UpdateCacheStatus events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/30/2021. </remarks>
    ///
    /// <param name="sender">   Source of the event. </param>
    /// <param name="e">        Cache status event information. </param>

    public delegate void UpdateCacheStatusEventHandler(object sender, CacheStatusEventArgs e);

    /// <summary>   Additional information for cache status events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/30/2021. </remarks>

    public class CacheStatusEventArgs
    {
        /// <summary>   Gets the increment kind. </summary>
        ///
        /// <value> The increment kind. </value>

        public IncrementKind IncrementKind { get; }

        /// <summary>   Gets the amount to increment by. </summary>
        ///
        /// <value> The amount to increment by. </value>

        public int Increment { get; }

        /// <summary>   Gets the amount to handled by. </summary>
        ///
        /// <value> Amount to handled by. </value>

        public List<object> HandledBy { get; }

        /// <summary>   Gets the amount to reuqested by. </summary>
        ///
        /// <value> Amount to reuqested by. </value>

        public PackageWorkingInstallFromCache RequestedBy { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/30/2021. </remarks>
        ///
        /// <param name="incrementKind">    The increment kind. </param>
        /// <param name="requestedBy">      Amount to requested by. </param>
        /// <param name="increment">        (Optional) Amount to increment by. </param>

        public CacheStatusEventArgs(IncrementKind incrementKind, PackageWorkingInstallFromCache requestedBy, int increment = 1)
        {
            this.IncrementKind = incrementKind;
            this.Increment = increment;
            this.RequestedBy = requestedBy;
            this.HandledBy = new List<object>();
        }
    }
}
