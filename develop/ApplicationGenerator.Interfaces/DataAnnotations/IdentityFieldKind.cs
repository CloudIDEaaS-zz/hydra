// file:	DataAnnotations\IdentityFieldKind.cs
//
// summary:	Implements the identity field kind class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Values that represent identity field kinds. </summary>
    ///
    /// <remarks>   Ken, 10/4/2020. </remarks>

    public enum IdentityFieldKind
    {
        /// <summary>   An enum constant representing the user Identifier option. </summary>
        [IdentityFieldCategory(IdentityFieldCategory.Client)]
        UserId,
        /// <summary>   An enum constant representing the client option. </summary>
        [IdentityFieldCategory(IdentityFieldCategory.Client | IdentityFieldCategory.Register)]
        Client,
        /// <summary>   An enum constant representing the user name option. </summary>
        [IdentityFieldCategory(IdentityFieldCategory.Login | IdentityFieldCategory.Client)]
        UserName,
        /// <summary>   An enum constant representing the password hash option. </summary>
        [IdentityFieldCategory(IdentityFieldCategory.Register)]
        PasswordHash,
        [IdentityFieldCategory(IdentityFieldCategory.Role)]
        /// <summary>   An enum constant representing the role name option. </summary>
        RoleName,
        [IdentityFieldCategory(IdentityFieldCategory.Role)]
        /// <summary>   An enum constant representing the is admin role option. </summary>
        IsAdminRole,
    }
}
