using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    public static class SwitchCommands
    {
        public const string DEBUG = "debug";
        public const string WAIT_FOR_INPUT = "waitForInput";
        public const string CWD = "cwd";
    }

    public static class ServerCommands
    {
        public const string GENERATE = "generate";
        public const string TERMINATE = "terminate";
        public const string CONNECT = "connect";
        public const string PING = "ping";
        public const string GET_FOLDER = "getfolder";
        public const string GET_FILE = "getfile";
        public const string GET_FOLDERS = "getfolders";
        public const string GET_FILES = "getfiles";
        public const string GET_PACKAGE_INSTALLS = "getpackageinstalls";
        public const string GET_PACKAGE_DEV_INSTALLS = "getpackagedevinstalls";
        public const string GET_CACHE_STATUS = "getcachestatus";
        public const string SET_INSTALL_STATUS = "setinstallstatus";
        public const string GET_FILE_CONTENTS = "getfilecontents";
        public const string GET_FILE_ICON = "getfileicon";
    }
}
