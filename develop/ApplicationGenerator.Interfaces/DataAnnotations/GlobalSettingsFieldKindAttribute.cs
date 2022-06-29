// file:	DataAnnotations\GlobalSettingsFieldKindAttribute.cs
//
// summary:	Implements the global settings field kind attribute class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Attribute for global settings field kind. </summary>
    ///
    /// <remarks>   Ken, 10/10/2020. </remarks>

    public class GlobalSettingsFieldKindAttribute : Attribute
    {
        /// <summary>   Gets the global settings field kind. </summary>
        ///
        /// <value> The global settings field kind. </value>

        public GlobalSettingsFieldKind GlobalSettingsFieldKind { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="globalSettingsFieldKind">  The global settings field kind. </param>

        public GlobalSettingsFieldKindAttribute(GlobalSettingsFieldKind globalSettingsFieldKind)
        {
            this.GlobalSettingsFieldKind = globalSettingsFieldKind;
        }
    }
}
