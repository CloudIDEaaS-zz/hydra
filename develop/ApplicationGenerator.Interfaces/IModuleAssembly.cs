// file:	IModuleAssembly.cs
//
// summary:	Declares the IModuleAssembly interface

using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.FolderStructure;
using AbstraX.DataAnnotations;

namespace AbstraX
{
    /// <summary>   Interface for module assembly. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/21/2020. </remarks>

    public interface IModuleAssembly : IModuleObject, IModuleOrAssembly
    {
        /// <summary>   Gets or sets the file. </summary>
        ///
        /// <value> The file. </value>

        File File { get; set; }

        /// <summary>   Gets or sets for child file. </summary>
        ///
        /// <value> for child file. </value>

        File ForChildFile { get; set; }

        /// <summary>   Gets or sets the load kind. </summary>
        ///
        /// <value> The user interface load kind. </value>

        UILoadKind UILoadKind { get; set; }

        /// <summary>   Gets or sets the kind. </summary>
        ///
        /// <value> The user interface kind. </value>

        UIKind UIKind { get; set; }

        /// <summary>   Gets or sets the indentation. </summary>
        ///
        /// <value> The indentation. </value>

        int Indentation { get; set; }

        /// <summary>   Gets or sets a value indicating whether the popped. </summary>
        ///
        /// <value> True if popped, false if not. </value>

        bool Popped { get; set; }

        /// <summary>   Gets or sets the full pathname of the hierarchy file. </summary>
        ///
        /// <value> The full pathname of the hierarchy file. </value>

        string UIHierarchyPath { get; set; }

        /// <summary>   Gets or sets the base route. </summary>
        ///
        /// <value> The base route. </value>

        string BaseRoute { get; set; }

        /// <summary>   Gets the base exports. </summary>
        ///
        /// <value> The base exports. </value>

        Dictionary<string, List<Module>> BaseExports { get; }

        /// <summary>   Gets or sets the exported components. </summary>
        ///
        /// <value> The exported components. </value>

        List<Module> ExportedComponents { get; set; }

        /// <summary>   Searches for the first export. </summary>
        ///
        /// <param name="name"> The name. </param>
        ///
        /// <returns>   The found export. </returns>

        Module FindExport(string name);

        /// <summary>   Gets the exports in this collection. </summary>
        ///
        /// <param name="baseObject">   The base object. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the exports in this collection.
        /// </returns>

        IEnumerable<Module> GetExports(IBase baseObject);

        /// <summary>   Gets the exports in this collection. </summary>
        ///
        /// <param name="attribute">    The attribute. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the exports in this collection.
        /// </returns>

        IEnumerable<Module> GetExports(string attribute);

        /// <summary>   Query if 'baseObject' has exports. </summary>
        ///
        /// <param name="baseObject">       The base object. </param>
        /// <param name="moduleAssembly">   The module assembly. </param>
        ///
        /// <returns>   True if exports, false if not. </returns>

        bool HasExports(IBase baseObject, IModuleAssembly moduleAssembly = null);

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        string DebugInfo { get; }
    }
}
