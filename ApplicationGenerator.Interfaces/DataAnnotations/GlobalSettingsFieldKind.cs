// file:	DataAnnotations\GlobalSettingsFieldKind.cs
//
// summary:	Implements the global settings field kind class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Values that represent global settings field kinds. </summary>
    ///
    /// <remarks>   Ken, 10/10/2020. </remarks>

    public enum GlobalSettingsFieldKind
    {
        /// <summary>   An enum constant representing the setting name option. </summary>
        SettingName,
        /// <summary>   An enum constant representing the setting type option. </summary>
        SettingType,
        /// <summary>   An enum constant representing the setting value option. </summary>
        SettingValue,
        /// <summary>   An enum constant representing the allow user customize option. </summary>
        AllowUserCustomize,
        /// <summary>   An enum constant representing the reset customize option. </summary>
        ResetCustomize
    }
}
