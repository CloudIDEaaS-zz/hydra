// file:	PackageReadEventHandler.cs
//
// summary:	Implements the package read event handler class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Delegate for handling PackageRead events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
    ///
    /// <param name="sender">   Source of the event. </param>
    /// <param name="e">        Package read event information. </param>

    public delegate void PackageReadEventHandler(object sender, PackageReadEventArgs e);

    /// <summary>   Additional information for package read events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

    public class PackageReadEventArgs
    {
        /// <summary>   Gets the package. </summary>
        ///
        /// <value> The package. </value>

        public Package Package { get; }

        /// <summary>   Gets the generate package. </summary>
        ///
        /// <value> The generate package. </value>

        public GenPackage GenPackage { get; }

        /// <summary>   Gets the import handler. </summary>
        ///
        /// <value> The import handler. </value>

        public IImportHandler ImportHandler { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="package">          The package. </param>
        /// <param name="genPackage">       The generate package. </param>
        /// <param name="importHandler">    The import handler. </param>

        public PackageReadEventArgs(Package package, GenPackage genPackage, IImportHandler importHandler)
        {
            this.Package = package;
            this.GenPackage = genPackage;
            this.ImportHandler = importHandler;
        }
    }
}
