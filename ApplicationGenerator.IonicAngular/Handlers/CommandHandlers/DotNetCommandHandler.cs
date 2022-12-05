// file:	Handlers\CommandHandlers\NugetCommandHandler.cs
//
// summary:	Implements the nuget command handler class

using AbstraX.Handlers.CommandHandlers.DotNetTools;
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

    public class DotNetCommandHandler : BaseWindowsCommandHandler 
    {
        public EntityFrameworkTool EF { get; }
        public DotNetTool Tool { get; }
        public DotNetCommandHandler.NugetCommand Nuget { get; }

        public DotNetCommandHandler() : base("dotnet.exe")
        {
            this.EF = new EntityFrameworkTool(this);
            this.Tool = new DotNetTool(this);
        }

        public void Build(string directory, string verbosity = "normal")
        {
            base.RunCommand("build", directory, "--verbosity", verbosity);
        }

        public class NugetCommand
        {
            private DotNetCommandHandler commandHandler;

            public NugetCommand(DotNetCommandHandler commandHandler)
            {
                this.commandHandler = commandHandler;
            }

            public void Push(string fileName, string apiKey, string source)
            {
                this.commandHandler.RunCommand("nuget push", Environment.CurrentDirectory, new string[] { fileName, "--api-key", apiKey, "--source", source });
            }
        }
    }
}
