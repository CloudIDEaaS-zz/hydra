using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreReflectionShim
{
    public static class SwitchCommands
    {
        public const string DEBUG = "debug";
        public const string WAIT_FOR_INPUT = "waitForInput";
        public const string CWD = "cwd";
        public const string TEST_CRASH_ANALYZER = "testCrashAnalyzer";
        public const string RUN_AS_AUTOMATED = "runAsAutomated";
    }

    public static partial class ServerCommands
    {
        public const string LOAD_ASSEMBLY = "loadAssembly";
        public const string TERMINATE = "terminate";
        public const string CONNECT = "connect";
        public const string PING = "ping";
        public const string GET_VERSION = "getversion";
        public const string PARAMETERINFO_GETCUSTOMATTRIBUTES__B = "parameterinfo_getcustomattributes__b";
        public const string PARAMETERINFO_GETCUSTOMATTRIBUTES__TB = "parameterinfo_getcustomattributes__tb";
        public const string PARAMETERINFO_TOSTRING = "parameterinfo_tostring";
        public const string PARAMETERINFO_GETHASHCODE = "parameterinfo_gethashcode";
        public const string METHODINFO_GETHASHCODE = "methodinfo_gethashcode";
        public const string MEMBERINFO_GETCUSTOMATTRIBUTES__B = "memberinfo_getcustomattributes__b";
        public const string MEMBERINFO_GETCUSTOMATTRIBUTES__TB = "memberinfo_getcustomattributes__tb";
        public const string METHODINFO_TOSTRING = "methodinfo_tostring";
        public const string PROPERTYINFO_GETHASHCODE = "propertyinfo_gethashcode";
        public const string TYPE_GETPROPERTIES__ = "type_getproperties__";
        public const string TYPE_GETHASHCODE = "type_gethashcode";
        public const string TYPE_GETPROPERTIES__B = "type_getproperties__b";
        public const string TYPE_TOSTRING = "type_tostring";
        public const string TYPE_ISVALUETYPEIMPL = "type_isvaluetypeimpl";
        public const string TYPE_GETATTRIBUTEFLAGSIMPL = "type_getattributeflagsimpl";
        public const string ASSEMBLYNAME_TOSTRING = "assemblyname_tostring";
        public const string ASSEMBLY_GETHASHCODE = "assembly_gethashcode";
        public const string ASSEMBLY_GETEXPORTEDTYPES = "assembly_getexportedtypes";
        public const string ASSEMBLY_GETTYPES = "assembly_gettypes";
        public const string ASSEMBLY_GETCUSTOMATTRIBUTES__B = "assembly_getcustomattributes__b";
        public const string CONSTRUCTORINFO_GETHASHCODE = "constructorinfo_gethashcode";
        public const string CUSTOMATTRIBUTETYPEDARGUMENT_TOSTRING = "customattributetypedargument_tostring";
        public const string CUSTOMATTRIBUTETYPEDARGUMENT_GETHASHCODE = "customattributetypedargument_gethashcode";
        public const string CUSTOMATTRIBUTENAMEDARGUMENT_GETHASHCODE = "customattributenamedargument_gethashcode";
        public const string CUSTOMATTRIBUTENAMEDARGUMENT_TOSTRING = "customattributenamedargument_tostring";
        public const string CUSTOMATTRIBUTEDATA_GETHASHCODE = "customattributedata_gethashcode";
        public const string CUSTOMATTRIBUTEDATA_TOSTRING = "customattributedata_tostring";
        public const string ASSEMBLY_GETCUSTOMATTRIBUTES__TB = "assembly_getcustomattributes__tb";
        public const string ASSEMBLY_GETREFERENCEDASSEMBLIES = "assembly_getreferencedassemblies";
        public const string ASSEMBLY_TOSTRING = "assembly_tostring";
        public const string MEMBERINFO_GETHASHCODE = "memberinfo_gethashcode";
        public const string METHODBASE_GETHASHCODE = "methodbase_gethashcode";
        public const string VALUETYPE_TOSTRING = "valuetype_tostring";
        public const string VALUETYPE_GETHASHCODE = "valuetype_gethashcode";
    }
}
