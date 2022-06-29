// file:	GeneratorPass.cs
//
// summary:	Implements the generator pass class

using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    /// <summary>   Values that represent generator pass. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/2/2021. </remarks>

    public enum GeneratorPass
    {
        /// <summary>   An enum constant representing the none option. </summary>
        None,
        /// <summary>   An enum constant representing all option. </summary>
        All,
        /// <summary>   An enum constant representing the hierarchy only option. </summary>
        StructureOnly,
        /// <summary>   An enum constant representing the files option. </summary>
        Files,
    }

    /// <summary>   A generator pass common. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/2/2021. </remarks>

    public static class GeneratorPassCommon
    {
        /// <summary>   The last. </summary>
        public const GeneratorPass Last = GeneratorPass.Files;
    }
}
