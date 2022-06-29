// file:	GeneratorKind.cs
//
// summary:	Implements the generator kind class

using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    /// <summary>   Values that represent generator kinds. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/15/2020. </remarks>

    public enum GeneratorKind
    {
        /// <summary>   An enum constant representing the Application option. </summary>
        App,
        /// <summary>   An enum constant representing the workspace option. </summary>
        Workspace,
        /// <summary>   An enum constant representing the business model option. </summary>
        BusinessModel,
        /// <summary>   An enum constant representing the entities option. </summary>
        Entities,
        /// <summary>   An enum constant representing the utility option. </summary>
        Utility
    }
}
