using System;
using System.Collections.Generic;
using System.Text;

namespace AssemblyAttributesShim
{
    public static class SwitchCommands
    {
        public const string DEBUG = "debug";
        public const string WAIT_FOR_INPUT = "waitForInput";
        public const string CWD = "cwd";
        public const string TEST_CRASH_ANALYZER = "testCrashAnalyzer";
        public const string LOG_LOCATION = "logLocation";
    }

    public static partial class ServerCommands
    {
        public const string GET_ASSEMBLY_ATTRIBUTES = "getAssemblyAttributes";
        public const string TERMINATE = "terminate";
        public const string CONNECT = "connect";
        public const string PING = "ping";
        public const string GET_VERSION = "getversion";
    }
}
