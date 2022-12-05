using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunDllExport
{
    public static class SwitchCommands
    {
        public const string DEBUG = "debug";
        public const string WAIT_FOR_INPUT = "waitForInput";
        public const string JSON = "json";
    }

    public class ServerCommands
    {
        public const string TERMINATE = "terminate";
        public const string EXECUTE = "execute";
        public const string CONNECT = "connect";
        public const string PING = "ping";
    }
}
