using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.CommandHandlers.DotNetTools
{
    public class DotNetTool
    {
        private IBaseWindowsCommandHandler commandHandler;

        public DotNetTool(IBaseWindowsCommandHandler commandHandler)
        {
            this.commandHandler = commandHandler;
        }

        public void Install(string directory, string package, bool global = false)
        {
            commandHandler.RunCommand("tool install", directory, package, global ? "--global" : string.Empty);
        }

        public void Update(string directory, string package, bool global = false)
        {
            commandHandler.RunCommand("tool update", directory, package, global ? "--global" : string.Empty);
        }
    }
}
