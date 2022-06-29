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
    /// <summary>   A grunt command handler. </summary>
    ///
    /// <remarks>   Ken, 10/12/2020. </remarks>

    public class GitCommandHandler : BaseWindowsCommandHandler
    {
        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

        public GitCommandHandler() : base("git.exe")
        {
        }

        /// <summary>   Pulls this.  </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/28/2022. </remarks>

        public void Pull()
        {
            base.RunCommand("pull", Environment.CurrentDirectory);
        }

        /// <summary>   Adds location. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/28/2022. </remarks>
        ///
        /// <param name="location"> The location to add. </param>

        public void Add(string location)
        {
            base.RunCommand("add", Environment.CurrentDirectory, location);
        }

        /// <summary>   Commits. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/28/2022. </remarks>
        ///
        /// <param name="message">  The message. </param>

        public void Commit(string message)
        {
            base.RunCommand("commit", Environment.CurrentDirectory, "-m", message.SurroundWithQuotes());
        }

        /// <summary>   Pushes an object onto this stack. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/28/2022. </remarks>

        public void Push()
        {
            base.RunCommand("push", Environment.CurrentDirectory);
        }
    }
}
