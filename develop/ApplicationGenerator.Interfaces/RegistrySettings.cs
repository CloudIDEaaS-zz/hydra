// file:	RegistrySettings.cs
//
// summary:	Implements the registry settings class

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX
{
    /// <summary>   A node module path. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/29/2021. </remarks>

    public class NodeModulePath : IRegistryKey
    {
        /// <summary>   Gets or sets the full pathname of the file. </summary>
        ///
        /// <value> The full pathname of the file. </value>

        public string Path { get; set; } 

        /// <summary>   Gets or sets the date of last pass process. </summary>
        ///
        /// <value> The date of last pass process. </value>

        public string DateOfLastPassProcess { get; set; }

        /// <summary>   Gets the name of the key. </summary>
        ///
        /// <value> The name of the key. </value>

        public string KeyName
        {
            get
            {
                var directory = new DirectoryInfo(this.Path);

                return directory.Parent.Name;                
            }
        }

        /// <summary>   Returns a string that represents the current object. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/29/2021. </remarks>
        ///
        /// <returns>   A string that represents the current object. </returns>

        public override string ToString()
        {
            return this.KeyName;
        }

    }

    /// <summary>   A registry settings. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/19/2021. </remarks>

    public class RegistrySettings : RegistrySettingsBase, ITraceResourcePersist
    {
        /// <summary>   Gets or sets the package path cache. </summary>
        ///
        /// <value> The package path cache. </value>

        public string PackagePathCache { get; set; }

        /// <summary>   Gets or sets the pathname of the current working directory. </summary>
        ///
        /// <value> The pathname of the current working directory. </value>

        public string CurrentWorkingDirectory { get; set; }

        /// <summary>   Gets or sets the trace resource. </summary>
        ///
        /// <value> The trace resource. </value>

        public string TraceResourceDocument { get; set; }

        /// <summary>   Gets or sets the trace resource last hash. </summary>
        ///
        /// <value> The trace resource last hash. </value>

        public string TraceResourceLastHash { get; set; }

        /// <summary>   Gets or sets the node module paths. </summary>
        ///
        /// <value> The node module paths. </value>

        [RegistryKeyEnumerable("NodeModulePaths", typeof(NodeModulePath))]
        public List<NodeModulePath> NodeModulePaths { get; set; }

        /// <summary>   Gets or sets the last resource save location. </summary>
        ///
        /// <value> The last resource save location. </value>

        public string LastResourceSaveLocation { get; set; }

        /// <summary>   Gets or sets options for controlling the turbo. </summary>
        ///
        /// <value> Options that control the turbo. </value>

        public TurboOptions TurboOptionsSaved { get; set; }

        /// <summary>   Gets or sets the original external settings. </summary>
        ///
        /// <value> The original external settings. </value>

        public TurboExternalSettings OriginalExternalSettings { get; set; }

        /// <summary>   Gets or sets the release agent client. </summary>
        ///
        /// <value> The release agent client. </value>

        public ReleaseAgentDesktop ReleaseAgentClient { get; set; }

        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/19/2021. </remarks>

        public HydraDesigner HydraDesigner { get; set; }

        public RegistrySettings() : base(@"Software\CloudIDEaaS\Hydra\ApplicationGenerator")
        {
            this.NodeModulePaths = new List<NodeModulePath>();
        }
    }
}
