// file:	Handlers\CommandHandlers\NugetCommandHandler.cs
//
// summary:	Implements the nuget command handler class

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.CommandHandlers
{
    /// <summary>   A nuget command handler. </summary>
    ///
    /// <remarks>   Ken, 10/12/2020. </remarks>

    public class NodeCommandHandler : BaseWindowsCommandHandler
    {
        /// <summary>   Gets the full pathname of the npm bin file. </summary>
        ///
        /// <value> The full pathname of the npm bin file. </value>

        public static string NpmBinPath { get; }

        static NodeCommandHandler()
        {
            NpmBinPath = Path.Combine(Environment.ExpandEnvironmentVariables("%ProgramW6432%"), @"nodejs\node_modules\npm\bin");

            if (!Directory.Exists(NpmBinPath))
            {
                DebugUtils.Break();
            }
        }


        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

        public NodeCommandHandler() : base("node.exe")
        {
        }

        /// <summary>   Executes the script operation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///
        /// <param name="command">  The script. </param>
        /// <param name="args">     A variable-length parameters list containing arguments. </param>

        public void RunNpmCommand(string command, params string[] args)
        {
            command = string.Format(@".load ./npm-cli.js {0}", command);

            base.RunCommand(command, null, args);
        }

        /// <summary>   Debug process. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/6/2021. </remarks>
        ///
        /// <param name="process">  The process. </param>

        public void DebugProcess(Process process)
        {
            var command = string.Format("-e \"process._debugProcess({0})\"", process.Id);

            base.RunCommand(command, null);
        }

        /// <summary>   Executes the script interactively operation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>
        ///

        public void RunInteractively()
        {
            base.LaunchInteractively(Environment.CurrentDirectory);
        }

        /// <summary>   Executes the npm interactively operation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

        public void RunInteractively(string workingDirectory)
        {
            base.LaunchInteractively(workingDirectory);
        }
    }
}
