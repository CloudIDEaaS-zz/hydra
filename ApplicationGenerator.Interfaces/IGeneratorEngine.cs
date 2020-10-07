// file:	IGeneratorEngine.cs
//
// summary:	Declares the IGeneratorEngine interface

using AbstraX.FolderStructure;
using AbstraX.Models.Interfaces;
using EntityProvider.Web.Entities;

namespace AbstraX
{
    /// <summary>   Interface for generator engine. </summary>
    ///
    /// <remarks>   Ken, 10/5/2020. </remarks>

    public interface IGeneratorEngine
    {
        /// <summary>   Gets the generator configuration. </summary>
        ///
        /// <value> The generator configuration. </value>

        GeneratorConfiguration GeneratorConfiguration { get; }

        /// <summary>   Gets the current tab text. </summary>
        ///
        /// <value> The current tab text. </value>

        string CurrentTabText { get; }
        /// <summary>   Indents this.  </summary>
        void Indent();
        /// <summary>   Dedents this.  </summary>
        void Dedent();
        /// <summary>   Process this.  </summary>
        void Process();

        /// <summary>   Writes an error. </summary>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        void WriteError(string format, params object[] args);

        /// <summary>   Writes a line. </summary>
        ///
        /// <param name="format">   Describes the format to use. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        void WriteLine(string format, params object[] args);

        /// <summary>   Writes a line. </summary>
        ///
        /// <param name="format">       Describes the format to use. </param>
        /// <param name="printMode">    The print mode. </param>
        /// <param name="args">         A variable-length parameters list containing arguments. </param>

        void WriteLine(string format, PrintMode printMode, params object[] args);
        /// <summary>   Resets this.  </summary>
        void Reset();
    }
}