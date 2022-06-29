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

    public class GruntCommandHandler : BaseWindowsCommandHandler
    {
        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/17/2021. </remarks>

        public GruntCommandHandler() : base("grunt")
        {
        }

        /// <summary>   Bumps this. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/28/2022. </remarks>

        public void Bump()
        {
            base.RunCommand("bump", Environment.CurrentDirectory);
        }
    }
}
