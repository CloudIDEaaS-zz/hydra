using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.CommandHandlers
{
    public class CommandAttribute : Attribute
    {
        public string Command { get; private set; }

        public CommandAttribute(string command)
        {
            this.Command = command;
        }
    }
}
