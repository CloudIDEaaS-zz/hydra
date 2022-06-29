// file:	IGeneratorEngine.cs
//
// summary:	Declares the IGeneratorEngine interface

using AbstraX.FolderStructure;
using AbstraX.Models.Interfaces;
using EntityProvider.Web.Entities;
using MailSlot;
using NetCoreReflectionShim.Agent;
using System.Collections.Generic;

namespace AbstraX
{
    /// <summary>   Interface for generator engine. </summary>
    ///
    /// <remarks>   Ken, 10/5/2020. </remarks>

    public interface IGeneratorEngine
    {
        /// <summary>   Gets a queue of message logs. </summary>
        ///
        /// <value> A queue of message logs. </value>

        Queue<string> LogMessageQueue { get; }

        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>

        string GenerationName { get; }

        /// <summary>   Gets the net core reflection agent. </summary>
        ///
        /// <value> The net core reflection agent. </value>

        NetCoreReflectionAgent NetCoreReflectionAgent { get; }

        /// <summary>   Gets the mailslot client. </summary>
        ///
        /// <value> The mailslot client. </value>

        MailslotClient MailslotClient { get; }

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
        /// <summary>   Tests process. </summary>
        void TestProcess();

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
        /// <summary>   Ends a processing. </summary>
        void EndProcessing(IGeneratorConfiguration generatorConfiguration);
    }
}