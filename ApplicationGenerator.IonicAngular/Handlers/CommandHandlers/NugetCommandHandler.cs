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

namespace AbstraX.Handlers.CommandHandlers
{
    /// <summary>   A nuget command handler. </summary>
    ///
    /// <remarks>   Ken, 10/12/2020. </remarks>

    public class NugetCommandHandler : BaseWindowsCommandHandler
    {
        public NugetCommandHandler() : base("nuget.exe")
        {
        }

        public void Pack(string workingDirectory, string file)
        {
            base.RunCommand("pack", workingDirectory, file);
        }
    }
}
