// file:	DataAnnotations\IdentityEntityKind.cs
//
// summary:	Implements the identity kind class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Values that represent identity kinds. </summary>
    ///
    /// <remarks>   Ken, 10/4/2020. </remarks>

    public enum IdentityEntityKind
    {
        /// <summary>   An enum constant representing the none option. </summary>
        None,
        /// <summary>   An enum constant representing the users option. </summary>
        User,
        /// <summary>   An enum constant representing the role option. </summary>
        Role,
        /// <summary>   An enum constant representing the user to role option. </summary>
        UserToRole
    }
}
