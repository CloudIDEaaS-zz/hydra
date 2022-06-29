// file:	IUtilityHandler.cs
//
// summary:	Declares the IUtilityHandler interface

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   An utility handler. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 1/4/2021. </remarks>

    public interface IUtilityHandler : IHandler, IDisposable
    {
        /// <summary>   Generates a base application frontend. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/4/2021. </remarks>
        ///
        /// <param name="appName">          Name of the application. </param>
        /// <param name="currentDirectory"> Pathname of the current directory. </param>
        /// <param name="debug">            True to debug. </param>

        void GenerateStarterAppFrontend(string appName, string currentDirectory, bool debug);

        /// <summary>   Generates an application frontend. </summary>
        ///
        /// <param name="appName">          Name of the application. </param>
        /// <param name="currentDirectory"> Pathname of the current directory. </param>
        /// <param name="debug">            True to debug. </param>

        void GenerateCompleteAppFrontend(string appName, string currentDirectory, bool debug);

        /// <summary>   Gets a value indicating whether the default IDE exists. </summary>
        ///
        /// <value> True if default IDE exists, false if not. </value>

        bool DefaultIDEExists { get; }

        /// <summary>   Default IDE open file. </summary>
        ///
        /// <param name="filePath"> Full pathname of the file. </param>

        void DefaultIDEOpenFile(string filePath);

        /// <summary>   Default IDE open folder. </summary>
        ///
        /// <param name="folderPath">   Full pathname of the folder file. </param>

        void DefaultIDEOpenFolder(string folderPath);
    }
}
