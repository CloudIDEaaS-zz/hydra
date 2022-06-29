// file:	DataAnnotations\QueryKind.cs
//
// summary:	Implements the query kind class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Values that represent query kinds. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/29/2020. </remarks>

    public enum QueryKind
    {
        /// <summary>   An enum constant representing the none option. </summary>
        None,
        /// <summary>   An enum constant representing the load parent reference option. </summary>
        LoadParentReference
    }
}
