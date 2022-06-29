// file:	IGeneratorHandler.cs
//
// summary:	Declares the IGeneratorHandler interface

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   Interface for generator handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/17/2020. </remarks>

    public interface IGeneratorHandler
    {
        /// <summary>   Gets the generator engine. </summary>
        ///
        /// <value> The generator engine. </value>

        IGeneratorEngine GeneratorEngine { get; }

        /// <summary>   Gets or sets a value indicating whether the suppress debug output. </summary>
        ///
        /// <value> True if suppress debug output, false if not. </value>

        bool SuppressDebugOutput { get; set; }

        /// <summary>   Gets or sets the generator configuration. </summary>
        ///
        /// <value> The generator configuration. </value>

        IGeneratorConfiguration GeneratorConfiguration { get; }

        /// <summary>   Executes the given arguments. </summary>
        ///
        /// <param name="arguments">    The arguments. </param>

        void Execute(Dictionary<string, object> arguments);

        /// <summary>   Ends a processing. </summary>
        ///
        /// <param name="generatorConfiguration">   The generator configuration. </param>

        void EndProcessing(IGeneratorConfiguration generatorConfiguration);
    }
}
