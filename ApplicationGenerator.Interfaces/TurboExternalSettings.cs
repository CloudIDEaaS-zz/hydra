using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    /// <summary>   A turbo external settings. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/20/2021. </remarks>

    public class TurboExternalSettings : IRegistryKey
    {
        /// <summary>   Gets the name of the key. </summary>
        ///
        /// <value> The name of the key. </value>

        public string KeyName => "TurboExternalSettings";

        /// <summary>   Gets or sets the initial npm registry global. </summary>
        ///
        /// <value> The initial npm registry global. </value>

        public string NpmGlobalRegistry { get; set; }

        /// <summary>   Gets or sets the initial npm registry local. </summary>
        ///
        /// <value> The initial npm registry local. </value>

        public string NpmLocalRegistry { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the initial firewall domain is enabled.
        /// </summary>
        ///
        /// <value> True if initial firewall domain enabled, false if not. </value>

        [RegistryNonStringValue(Microsoft.Win32.RegistryValueKind.DWord)]
        public bool FirewallDomainEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the initial firewall private is enabled.
        /// </summary>
        ///
        /// <value> True if initial firewall private enabled, false if not. </value>

        [RegistryNonStringValue(Microsoft.Win32.RegistryValueKind.DWord)]
        public bool FirewallPrivateEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the initial firewall public is enabled.
        /// </summary>
        ///
        /// <value> True if initial firewall public enabled, false if not. </value>

        [RegistryNonStringValue(Microsoft.Win32.RegistryValueKind.DWord)]
        public bool FirewallPublicEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the initial path exclusions is enabled.
        /// </summary>
        ///
        /// <value> True if initial path exclusions enabled, false if not. </value>

        [RegistryNonStringValue(Microsoft.Win32.RegistryValueKind.DWord)]
        public bool PathExclusionsEnabled { get; set; }

        /// <summary>   Gets or sets a value indicating whether the finalized properly. </summary>
        ///
        /// <value> True if finalized properly, false if not. </value>

        public string FinalizedProperly { get; set; }
    }
}
