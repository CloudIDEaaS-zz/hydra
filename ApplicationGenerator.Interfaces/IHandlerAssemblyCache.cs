// file:	IHandlerCache.cs
//
// summary:	Declares the IHandlerCache interface

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AbstraX
{
    /// <summary>   Interface for handler cache. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 4/15/2021. </remarks>

    public interface IHandlerAssemblyCache
    {
        /// <summary>   Gets the handler assemblies. </summary>
        ///
        /// <value> The handler assemblies. </value>

        Dictionary<string, Assembly> HandlerAssemblies { get; }

        /// <summary>   Gets the handler elements. </summary>
        ///
        /// <value> The handler elements. </value>

        IEnumerable<XElement> HandlerElements { get; }
    }
}
