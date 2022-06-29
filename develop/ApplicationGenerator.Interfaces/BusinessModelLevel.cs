// file:	BusinessModelLevel.cs
//
// summary:	Implements the business model level class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Values that represent business model levels. </summary>
    ///
    /// <remarks>   Ken, 10/4/2020. </remarks>

    public enum BusinessModelLevel
    {
        /// <summary>   An enum constant representing the business model option. </summary>
        BusinessModel,
        /// <summary>   An enum constant representing the stakeholder option. </summary>
        Stakeholder,
        /// <summary>   An enum constant representing the role option. </summary>
        Role,
        /// <summary>   An enum constant representing the system option. </summary>
        System,
        /// <summary>   An enum constant representing the responsibility option. </summary>
        Responsibility,
        /// <summary>   An enum constant representing the task option. </summary>
        Task,
        /// <summary>   An enum constant representing the data item option. </summary>
        DataItem
    }
}
