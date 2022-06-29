// file:	StandardStreamService.cs
//
// summary:	Implements the standard stream service class

using System;
using System.Collections.Generic;

namespace AbstraX
{
    /// <summary>   Information about the alert. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>

    public class AlertInfo
    {
        /// <summary>   Gets or sets the alert from address. </summary>
        ///
        /// <value> The alert from address. </value>

        public string AlertFromAddress { get; internal set; }

        /// <summary>   Gets or sets the alert to address. </summary>
        ///
        /// <value> The alert to address. </value>

        public string AlertToAddress { get; internal set; }

        /// <summary>   Gets or sets the alert when level. </summary>
        ///
        /// <value> The alert when level. </value>

        public List<string> AlertWhenLevel { get; internal set; }

        /// <summary>   Gets or sets the alert when idle. </summary>
        ///
        /// <value> The alert when idle. </value>

        public TimeSpan AlertWhenIdle { get; internal set; }

        /// <summary>   Gets or sets a value indicating whether the alert use sounds. </summary>
        ///
        /// <value> True if alert use sounds, false if not. </value>

        public bool AlertUseSounds { get; internal set; }
    }
}