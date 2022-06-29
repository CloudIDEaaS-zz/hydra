// file:	FacetHandlerLayer.cs
//
// summary:	Implements the facet handler layer class

using AbstraX.ServerInterfaces;
using EntityProvider.Web.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    /// <summary>   Values that represent facet handler layers. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 2/19/2021. </remarks>

    public enum FacetHandlerLayer
    {
        /// <summary>   An enum constant representing the client option. </summary>
        Client,
        /// <summary>   An enum constant representing the server Configuration option. </summary>
        ServerConfig,
        /// <summary>   An enum constant representing the web service option. </summary>
        WebService
    }
}
