// file:	RegistrySettings.cs
//
// summary:	Implements the registry settings class

using Utils;

namespace AbstraX
{
    /// <summary>   A turbo options. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/20/2021. </remarks>

    public class TurboOptions : IRegistryKey
    {
        /// <summary>   Gets the name of the key. </summary>
        ///
        /// <value> The name of the key. </value>

        public string KeyName => "TurboOptionsSaved";

        /// <summary>   Gets or sets a value indicating whether the skip backups. </summary>
        ///
        /// <value> True if skip backups, false if not. </value>

        [RegistryNonStringValue(Microsoft.Win32.RegistryValueKind.DWord)]
        public bool SkipBackups { get; set; }

        /// <summary>   Gets or sets a value indicating whether the obtain network priority. </summary>
        ///
        /// <value> True if obtain network priority, false if not. </value>

        [RegistryNonStringValue(Microsoft.Win32.RegistryValueKind.DWord)]
        public bool ObtainNetworkPriority { get; set; }

        /// <summary>   Gets or sets a value indicating whether the pause firewall. </summary>
        ///
        /// <value> True if pause firewall, false if not. </value>

        [RegistryNonStringValue(Microsoft.Win32.RegistryValueKind.DWord)]
        public bool PauseFirewall { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  use HTTP over HTTPS. </summary>
        ///
        /// <value> True if use HTTP over https, false if not. </value>

        [RegistryNonStringValue(Microsoft.Win32.RegistryValueKind.DWord)]
        public bool UseHttpOverHttps { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the exclude folders from virus scan.
        /// </summary>
        ///
        /// <value> True if exclude folders from virus scan, false if not. </value>

        [RegistryNonStringValue(Microsoft.Win32.RegistryValueKind.DWord)]
        public bool ExcludeFoldersFromVirusScan { get; set; }

        /// <summary>   Gets or sets a value indicating whether the remember settings. </summary>
        ///
        /// <value> True if remember settings, false if not. </value>

        [RegistryNonStringValue(Microsoft.Win32.RegistryValueKind.DWord)]
        public bool RememberSettings { get; set; }
    }
}