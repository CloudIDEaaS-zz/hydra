// file:	GetTypeShimActivatorHandler.cs
//
// summary:	Implements the get type shim activator handler class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Handler, called when the get type shim activator. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>
    ///
    /// <param name="sender">   Source of the event. </param>
    /// <param name="e">        Get type shim activator event information. </param>

    public delegate void GetTypeShimActivatorHandler(object sender, GetTypeShimActivatorEventArgs e);

    /// <summary>   Additional information for get type shim activator events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/4/2020. </remarks>

    public class GetTypeShimActivatorEventArgs : EventArgs
    {
        /// <summary>   Gets or sets the type shim activator. </summary>
        ///
        /// <value> The type shim activator. </value>

        public ITypeShimActivator Activator { get; set; }
    }
}
