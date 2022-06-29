// file:	PrintMode.cs
//
// summary:	Implements the print mode class

using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    /// <summary>   A bit-field of flags for specifying print modes. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/15/2020. </remarks>

    [Flags]
    public enum PrintMode
    {
        /// <summary>   A binary constant representing any flag. </summary>
        Any = 1,
        /// <summary>   A binary constant representing all flag. </summary>
        All = 2,
        /// <summary>   A binary constant representing the print facets only flag. </summary>
        PrintFacets = 4,
        /// <summary>
        /// A binary constant representing the print User interface hierarchy path only flag.
        /// </summary>
        PrintUIHierarchyPath = 8 | ExcludeFromAll,
        /// <summary>
        /// A binary constant representing the print User interface hierarchy path and module assemblies
        /// stack only flag.
        /// </summary>
        PrintUIHierarchyPathAndModuleAssembliesStackOnly = 16 | PrintUIHierarchyPath,
        /// <summary>   A binary constant representing the exclude from all flag. </summary>
        ExcludeFromAll = 4096
    }
}
