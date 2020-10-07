// file:	AppSettingsKindHandlerAttribute.cs
//
// summary:	Implements the application settings kind handler attribute class

using AbstraX.DataAnnotations;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    /// <summary>   Attribute for application settings kind handler. </summary>
    ///
    /// <remarks>   Ken, 10/4/2020. </remarks>

    [AttributeUsage(AttributeTargets.Class)]
    public class AppSettingsKindHandlerAttribute : Attribute
    {
        /// <summary>   Gets or sets the application settings kind. </summary>
        ///
        /// <value> The application settings kind. </value>

        public AppSettingsKind AppSettingsKind { get; private set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="appSettingsKind">  The application settings kind. </param>

        public AppSettingsKindHandlerAttribute(AppSettingsKind appSettingsKind)
        {
            this.AppSettingsKind = appSettingsKind;
        }
    }
}
