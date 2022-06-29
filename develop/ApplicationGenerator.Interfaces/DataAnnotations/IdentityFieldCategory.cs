// file:	DataAnnotations\IdentityFieldCategory.cs
//
// summary:	Implements the identity field category class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   A bit-field of flags for specifying identity field categories. </summary>
    ///
    /// <remarks>   Ken, 10/10/2020. </remarks>

    [Flags]
    public enum IdentityFieldCategory
    {
        /// <summary>   A binary constant representing the client flag. </summary>
        Client = 1,
        /// <summary>   A binary constant representing the login flag. </summary>
        Login = 1 << 1,
        /// <summary>   A binary constant representing the register flag. </summary>
        Register = 1 << 2 | Login,
        /// <summary>   A binary constant representing the server flag. </summary>
        Server = 1 << 2 | Register | Client,
        /// <summary>   A binary constant representing the role flag. </summary>
        Role = 1 << 3
    }
}
