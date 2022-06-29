// file:	DataAnnotations\AppSettingsKindAttribute.cs
//
// summary:	Implements the application settings kind attribute class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Attribute for application settings kind. </summary>
    ///
    /// <remarks>   Ken, 10/10/2020. </remarks>

    public class AppSettingsKindAttribute : Attribute
    {
        /// <summary>   Gets the application settings kind. </summary>
        ///
        /// <value> The application settings kind. </value>

        public AppSettingsKind AppSettingsKind { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="appSettingsKind">  The application settings kind. </param>

        public AppSettingsKindAttribute(AppSettingsKind appSettingsKind)
        {
            this.AppSettingsKind = appSettingsKind;
        }
    }
}
