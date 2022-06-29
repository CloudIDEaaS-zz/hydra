// file:	DataAnnotations\TaskCapabilities.cs
//
// summary:	Implements the task capabilities class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Values that represent task capabilities. </summary>
    ///
    /// <remarks>   Ken, 10/4/2020. </remarks>

    [Flags()]
    public enum TaskCapabilities
    {
        /// <summary>   An enum constant representing the none option. </summary>
        None = 0,
        /// <summary>   An enum constant representing the search option. </summary>
        Search = 1,
        /// <summary>   An enum constant representing the list option. </summary>
        List = 1 << 1,
        /// <summary>   An enum constant representing the create option. </summary>
        Create = 1 << 2,
        /// <summary>   An enum constant representing the read option. </summary>
        Read = 1 << 3,
        /// <summary>   An enum constant representing the update option. </summary>
        Update = 1 << 4,
        /// <summary>   An enum constant representing the delete option. </summary>
        Delete = 1 << 5,
        /// <summary>   A binary constant representing the enter flag. </summary>
        Enter = 1 << 6,
    }
}
