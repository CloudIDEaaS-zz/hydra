// file:	DataAnnotations\UILoadKind.cs
//
// summary:	Implements the load kind class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Values that represent load kinds. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 2/14/2021. </remarks>

    public enum UILoadKind
    {
        /// <summary>   An enum constant representing the default option. </summary>
        Default,
        /// <summary>   An enum constant representing the home page option. </summary>
        HomePage,
        /// <summary>   An enum constant representing the main page option. </summary>
        MainPage,
    }
}
