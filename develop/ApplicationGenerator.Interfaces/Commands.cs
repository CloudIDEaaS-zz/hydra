// file:	Commands.cs
//
// summary:	Implements the commands class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   A switch commands. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/27/2020. </remarks>

    public static class SwitchCommands
    {
        /// <summary>   The debug. </summary>
        public const string DEBUG = "debug";
        /// <summary>   The debug shim service. </summary>
        public const string DEBUG_SHIM_SERVICE = "debugshimservice";
        /// <summary>   The wait for input. </summary>
        public const string WAIT_FOR_INPUT = "waitForInput";
        /// <summary>   The run as automated. </summary>
        public const string RUN_AS_AUTOMATED = "runAsAutomated";
        /// <summary>   The client pipe. </summary>
        public const string CLIENT_PIPE = "clientpipe";
        /// <summary>   The use overrides. </summary>
        public const string USE_OVERRIDES_ASSEMBLY = "useOverridesAssembly";
        /// <summary>   The cwd. </summary>
        public const string CWD = "cwd";
        /// <summary>   The log service messages. </summary>
        public const string LOG_SERVICE_MESSAGES = "logServiceMessages";
        /// <summary>   The log package listing. </summary>
        public const string LOG_PACKAGE_LISTING = "logPackageListing";
        /// <summary>   The test crash analyzer. </summary>
        public const string TEST_CRASH_ANALYZER = "testCrashAnalyzer";
    }

    /// <summary>   A server commands. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/27/2020. </remarks>

    public static class ServerCommands
    {
        /// <summary>   The generate. </summary>
        public const string GENERATE = "generate";
        /// <summary>   The add resource browse file. </summary>
        public const string ADD_RESOURCE_BROWSE_FILE = "addresourcebrowsefile";
        /// <summary>   The add resource capture image. </summary>
        public const string ADD_RESOURCE_CAPTURE_IMAGE = "addresourcecaptureimage";
        /// <summary>   The add resource choose color. </summary>
        public const string ADD_RESOURCE_CHOOSE_COLOR = "addresourcechoosecolor";
        /// <summary>   The designer. </summary>
        public const string SHOW_DESIGNER = "showdesigner";
        /// <summary>   The terminate. </summary>
        public const string END_PROCESSING = "endprocessing";
        /// <summary>   The terminate. </summary>
        public const string TERMINATE = "terminate";
        /// <summary>   The connect. </summary>
        public const string CONNECT = "connect";
        /// <summary>   The launch services. </summary>
        public const string LAUNCH_SERVICES = "launchservices";
        /// <summary>   The initialize. </summary>
        public const string INITIALIZE_RENDERER = "initialize_renderer";
        /// <summary>   The load module. </summary>
        public const string LOAD_MODULE = "load_module";
        /// <summary>   The update instance. </summary>
        public const string UPDATE_INSTANCE = "update_instance";
        /// <summary>   The ping. </summary>
        public const string PING = "ping";
        /// <summary>   The get version. </summary>
        public const string GET_VERSION = "getversion";
        /// <summary>   Pathname of the get folder. </summary>
        public const string GET_FOLDER = "getfolder";
        /// <summary>   The get file. </summary>
        public const string GET_FILE = "getfile";
        /// <summary>   The get folders. </summary>
        public const string GET_FOLDERS = "getfolders";
        /// <summary>   The get files. </summary>
        public const string GET_FILES = "getfiles";
        /// <summary>   The get package installs. </summary>
        public const string GET_PACKAGE_INSTALLS = "getpackageinstalls";
        /// <summary>   The get package development installs. </summary>
        public const string GET_PACKAGE_DEV_INSTALLS = "getpackagedevinstalls";
        /// <summary>   The get cache status. </summary>
        public const string GET_CACHE_STATUS = "getcachestatus";
        /// <summary>   The set install status. </summary>
        public const string SET_INSTALL_STATUS = "setinstallstatus";
        /// <summary>   The get file contents. </summary>
        public const string GET_FILE_CONTENTS = "getfilecontents";
        /// <summary>   The get file icon. </summary>
        public const string GET_FILE_ICON = "getfileicon";
        /// <summary>   The send mailslot. </summary>
        public const string SEND_HYDRA_STATUS = "sendhydrastatus";
        /// <summary>   The get full snapshot. </summary>
        public const string GET_FULL_SNAPSHOT = "get_full_snapshot";
    }
}
