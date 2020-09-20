using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.CommandHandlers
{
    public interface ICommandHandler
    {
        void Execute(params KeyValuePair<string, object>[] arguments);
        void Execute(Dictionary<string, object> arguments);
    }
}
