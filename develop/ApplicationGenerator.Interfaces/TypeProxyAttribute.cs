// file:	TypeProxyAttribute.cs
//
// summary:	Implements the type proxy attribute class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Attribute for type proxy. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/8/2020. </remarks>

    public class TypeProxyAttribute : Attribute
    {
        /// <summary>   Gets the type of the proxied. </summary>
        ///
        /// <value> The type of the proxied. </value>

        public Type ProxiedType { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/8/2020. </remarks>
        ///
        /// <param name="proxiedType">  Type of the proxied. </param>

        public TypeProxyAttribute(Type proxiedType)
        {
            this.ProxiedType = proxiedType;
        }
    }
}
