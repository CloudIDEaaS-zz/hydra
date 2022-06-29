// file:	IModuleObject.cs
//
// summary:	Declares the IModuleObject interface

using AbstraX.ClientFolderStructure;
using AbstraX.DataAnnotations;
using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Interface for module object. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 2/25/2021. </remarks>

    public interface IModuleObject
    {
        /// <summary>   Gets or sets the base object. </summary>
        ///
        /// <value> The base object. </value>

        IBase BaseObject { get; set; }

        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>

        string Name { get; set; }

        /// <summary>   Gets the runtime facets. </summary>
        ///
        /// <value> The runtime facets. </value>

        List<Facet> RuntimeFacets { get; }

        /// <summary>   Gets or sets the routing name expression. </summary>
        ///
        /// <value> The routing name expression. </value>

        string RoutingNameExpression { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  is component. </summary>
        ///
        /// <value> True if this  is component, false if not. </value>

        bool IsComponent { get; set; }

        /// <summary>   Gets user interface attribute. </summary>
        ///
        /// <param name="folder">   Pathname of the folder. </param>
        ///
        /// <returns>   The user interface attribute. </returns>

        UIAttribute GetUIAttribute(AbstraX.FolderStructure.Folder folder);

        /// <summary>   Validates the module name. </summary>
        ///
        /// <param name="name">     The name. </param>
        /// <param name="error">    [out] The error. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool ValidateModuleName(string name, out string error);
    }
}
