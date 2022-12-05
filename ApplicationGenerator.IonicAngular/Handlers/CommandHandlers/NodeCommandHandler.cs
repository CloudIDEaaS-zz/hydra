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

    public class NodeCommandHandler : BaseWindowsCommandHandler
    {
        public NodeCommandHandler() : base("node.exe")
        {
        }

        public void RunScript(string script)
        {
            base.RunCommand(script, null);
        }
    }
}
