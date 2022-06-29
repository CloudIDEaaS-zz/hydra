// file:	GetTypeProxyEventHandler.cs
//
// summary:	Implements the get type proxy event handler class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreReflectionShim.Service
{
    /// <summary>   Delegate for handling GetTypeProxy events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/8/2020. </remarks>
    ///
    /// <param name="sender">   Source of the event. </param>
    /// <param name="e">        Get type proxy event information. </param>

    public delegate void GetTypeProxyEventHandler(object sender, GetTypeProxyEventArgs e);

    /// <summary>   Additional information for get type proxy events. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/8/2020. </remarks>

    public class GetTypeProxyEventArgs : EventArgs
    {
        public Type TypeToProxy { get; }
        public Type ProxyType { get; set; }

        public GetTypeProxyEventArgs(Type type)
        {
            this.TypeToProxy = type;
        }
    }
}
