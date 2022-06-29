// file:	TemplateObjects\EntityPropertyItem.cs
//
// summary:	Implements the entity property item class

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.TemplateObjects
{
    /// <summary>   Values that represent entity marker kinds. </summary>
    ///
    /// <remarks>   Ken, 10/8/2020. </remarks>

    public enum EntityMarkerKind
    {
        /// <summary>   An enum constant representing the Application settings kind option. </summary>
        AppSettingsKind,
        /// <summary>   An enum constant representing the identity user option. </summary>
        IdentityEntityUser,
        /// <summary>   An enum constant representing the UI group option. </summary>
        UIGroup
    }
}
