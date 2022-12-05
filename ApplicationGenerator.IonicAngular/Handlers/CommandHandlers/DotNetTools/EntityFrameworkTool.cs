using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.CommandHandlers.DotNetTools
{
    public class EntityFrameworkTool
    {
        private IBaseWindowsCommandHandler commandHandler;

        public EntityFrameworkTool(IBaseWindowsCommandHandler commandHandler)
        {
            this.commandHandler = commandHandler;
        }

        public void MigrationsAdd(string directory, string name, string dbContext)
        {
            commandHandler.RunCommand("ef migrations add", directory, name, "--context", dbContext);
        }

        public void DatabaseDrop(string directory, bool force = false)
        {
            commandHandler.RunCommand("ef database drop", directory, force ? "--force" : string.Empty);
        }

        public void DatabaseUpdate(string directory)
        {
            commandHandler.RunCommand("ef database update", directory);
        }
    }
}
