using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.CommandHandlers
{
    public class TypescriptCommandHandler : BaseWindowsCommandHandler
    {
        public TypescriptCommandHandler() : base("tsc")
        {
        }

        public void Compile()
        {
            base.RunCommand("-p .\\", Environment.CurrentDirectory, Array.Empty<string>());
        }
    }
}
