// file:	DataAnnotations\StakeholderKind.cs
//
// summary:	Implements the stakeholder kind class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Values that represent stakeholder kinds. </summary>
    ///
    /// <remarks>   Ken, 10/4/2020. </remarks>

    public enum StakeholderKind
    {
        /// <summary>   An enum constant representing the normal option. </summary>
        Normal,
        /// <summary>   An enum constant representing the internal root option. </summary>
        InternalRoot,
        /// <summary>   An enum constant representing the external root option. </summary>
        ExternalRoot,
        /// <summary>   An enum constant representing the customer option. </summary>
        Customer,
        /// <summary>   An enum constant representing the supplier option. </summary>
        Supplier,
    }
}
