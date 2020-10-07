// file:	DataAnnotations\AppSettingsKind.cs
//
// summary:	Implements the application settings kind class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Values that represent Application settings kinds. </summary>
    ///
    /// <remarks>   Ken, 10/4/2020. </remarks>

    public enum AppSettingsKind
    {
        /// <summary>   An enum constant representing the none option. </summary>
        None,
        /// <summary>   An enum constant representing the global settings option. </summary>
        GlobalSettings,
        /// <summary>   An enum constant representing the user preferences option. </summary>
        UserPreferences,
        /// <summary>   An enum constant representing the profile preferences option. </summary>
        ProfilePreferences
    }
}
