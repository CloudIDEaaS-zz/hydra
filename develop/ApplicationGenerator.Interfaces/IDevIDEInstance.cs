// file:	VisualStudioInstance.cs
//
// summary:	Implements the visual studio instance class

using System;
using System.Diagnostics;

namespace AbstraX
{
    /// <summary>   Interface for visual studio instance. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 6/27/2022. </remarks>

    public interface IDevIDEInstance
    {
        /// <summary>   Gets the process. </summary>
        ///
        /// <value> The process. </value>

        Process Process { get; }

        /// <summary>   Gets the name of the process. </summary>
        ///
        /// <value> The name of the process. </value>

        string ProcessName { get; }

        /// <summary>   Gets window text matcher. </summary>
        ///
        /// <param name="workspaceName">    Name of the workspace. </param>
        ///
        /// <returns>   The window text matcher. </returns>

        bool WindowTextMatches(string text, string workspaceName);

        /// <summary>   Builds. </summary>
        ///
        /// <param name="solutionName"> Name of the solution. </param>
        /// <param name="projectName">  Name of the project. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool Build(string solutionName, string projectName);

        /// <summary>   Closes this.  </summary>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool Close();

        /// <summary>   Debug start. </summary>
        ///
        /// <param name="solutionName"> Name of the solution. </param>
        /// <param name="projectName">  Name of the project. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool DebugStart(string solutionName, string projectName);

        /// <summary>   Debug stop. </summary>
        ///
        /// <param name="disposable">   The disposable. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        bool DebugStop(IDisposable disposable);

        /// <summary>   Query if this  has errors. </summary>
        ///
        /// <returns>   True if errors, false if not. </returns>

        bool HasErrors();

        /// <summary>   Query if this  is debugging. </summary>
        ///
        /// <returns>   True if debugging, false if not. </returns>

        bool IsDebugging();
        void DebugAttach(Process[] processes, bool writeToConsole);
    }
}