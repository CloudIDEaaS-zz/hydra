namespace Microsoft.Build.CommandLine
{
    using Microsoft.Build.Shared;
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal sealed class CommandLineSwitches
    {
        private string badCommandLineArg;
        private string errorMessage;
        private Exception innerException;
        private bool isParameterError;
        private static readonly string[] noParameters;
        private DetectedParameterizedSwitch[] parameterizedSwitches = new DetectedParameterizedSwitch[0x18];
        private static readonly ParameterizedSwitchInfo[] parameterizedSwitchesMap;
        private DetectedParameterlessSwitch[] parameterlessSwitches = new DetectedParameterlessSwitch[0x13];
        private static readonly ParameterlessSwitchInfo[] parameterlessSwitchesMap = new ParameterlessSwitchInfo[] { 
            new ParameterlessSwitchInfo(new string[] { "help", "h", "?" }, ParameterlessSwitch.Help, null, null), new ParameterlessSwitchInfo(new string[] { "version", "ver" }, ParameterlessSwitch.Version, null, null), new ParameterlessSwitchInfo(new string[] { "nologo" }, ParameterlessSwitch.NoLogo, null, null), new ParameterlessSwitchInfo(new string[] { "noautoresponse", "noautorsp" }, ParameterlessSwitch.NoAutoResponse, null, null), new ParameterlessSwitchInfo(new string[] { "noconsolelogger", "noconlog" }, ParameterlessSwitch.NoConsoleLogger, null, null), new ParameterlessSwitchInfo(new string[] { "filelogger", "fl" }, ParameterlessSwitch.FileLogger, null, null), new ParameterlessSwitchInfo(new string[] { "filelogger1", "fl1" }, ParameterlessSwitch.FileLogger1, null, null), new ParameterlessSwitchInfo(new string[] { "filelogger2", "fl2" }, ParameterlessSwitch.FileLogger2, null, null), new ParameterlessSwitchInfo(new string[] { "filelogger3", "fl3" }, ParameterlessSwitch.FileLogger3, null, null), new ParameterlessSwitchInfo(new string[] { "filelogger4", "fl4" }, ParameterlessSwitch.FileLogger4, null, null), new ParameterlessSwitchInfo(new string[] { "filelogger5", "fl5" }, ParameterlessSwitch.FileLogger5, null, null), new ParameterlessSwitchInfo(new string[] { "filelogger6", "fl6" }, ParameterlessSwitch.FileLogger6, null, null), new ParameterlessSwitchInfo(new string[] { "filelogger7", "fl7" }, ParameterlessSwitch.FileLogger7, null, null), new ParameterlessSwitchInfo(new string[] { "filelogger8", "fl8" }, ParameterlessSwitch.FileLogger8, null, null), new ParameterlessSwitchInfo(new string[] { "filelogger9", "fl9" }, ParameterlessSwitch.FileLogger9, null, null), new ParameterlessSwitchInfo(new string[] { "oldom" }, ParameterlessSwitch.OldOM, null, null), 
            new ParameterlessSwitchInfo(new string[] { "distributedfilelogger", "dfl" }, ParameterlessSwitch.DistributedFileLogger, null, null), new ParameterlessSwitchInfo(new string[] { "debug", "d" }, ParameterlessSwitch.Debugger, null, "DebuggerEnabled"), new ParameterlessSwitchInfo(new string[] { "detailedsummary", "ds" }, ParameterlessSwitch.DetailedSummary, null, null)
         };
        private static readonly char[] parameterSeparators;

        static CommandLineSwitches()
        {
            ParameterizedSwitchInfo[] infoArray2 = new ParameterizedSwitchInfo[0x18];
            string[] switchNames = new string[1];
            infoArray2[0] = new ParameterizedSwitchInfo(switchNames, ParameterizedSwitch.Project, "DuplicateProjectSwitchError", false, null, true);
            infoArray2[1] = new ParameterizedSwitchInfo(new string[] { "target", "t" }, ParameterizedSwitch.Target, null, true, "MissingTargetError", true);
            infoArray2[2] = new ParameterizedSwitchInfo(new string[] { "property", "p" }, ParameterizedSwitch.Property, null, true, "MissingPropertyError", true);
            infoArray2[3] = new ParameterizedSwitchInfo(new string[] { "logger", "l" }, ParameterizedSwitch.Logger, null, false, "MissingLoggerError", false);
            infoArray2[4] = new ParameterizedSwitchInfo(new string[] { "distributedlogger", "dl" }, ParameterizedSwitch.DistributedLogger, null, false, "MissingLoggerError", false);
            infoArray2[5] = new ParameterizedSwitchInfo(new string[] { "verbosity", "v" }, ParameterizedSwitch.Verbosity, null, false, "MissingVerbosityError", true);
            infoArray2[6] = new ParameterizedSwitchInfo(new string[] { "validate", "val" }, ParameterizedSwitch.Validate, null, false, null, true);
            infoArray2[7] = new ParameterizedSwitchInfo(new string[] { "consoleloggerparameters", "clp" }, ParameterizedSwitch.ConsoleLoggerParameters, null, false, "MissingConsoleLoggerParameterError", true);
            infoArray2[8] = new ParameterizedSwitchInfo(new string[] { "nodemode", "nmode" }, ParameterizedSwitch.NodeMode, null, false, null, false);
            infoArray2[9] = new ParameterizedSwitchInfo(new string[] { "maxcpucount", "m" }, ParameterizedSwitch.MaxCPUCount, null, false, "MissingMaxCPUCountError", true);
            infoArray2[10] = new ParameterizedSwitchInfo(new string[] { "ignoreprojectextensions", "ignore" }, ParameterizedSwitch.IgnoreProjectExtensions, null, true, "MissingIgnoreProjectExtensionsError", true);
            infoArray2[11] = new ParameterizedSwitchInfo(new string[] { "toolsversion", "tv" }, ParameterizedSwitch.ToolsVersion, null, false, "MissingToolsVersionError", true);
            infoArray2[12] = new ParameterizedSwitchInfo(new string[] { "fileloggerparameters", "flp" }, ParameterizedSwitch.FileLoggerParameters, null, false, "MissingFileLoggerParameterError", true);
            infoArray2[13] = new ParameterizedSwitchInfo(new string[] { "fileloggerparameters1", "flp1" }, ParameterizedSwitch.FileLoggerParameters1, null, false, "MissingFileLoggerParameterError", true);
            infoArray2[14] = new ParameterizedSwitchInfo(new string[] { "fileloggerparameters2", "flp2" }, ParameterizedSwitch.FileLoggerParameters2, null, false, "MissingFileLoggerParameterError", true);
            infoArray2[15] = new ParameterizedSwitchInfo(new string[] { "fileloggerparameters3", "flp3" }, ParameterizedSwitch.FileLoggerParameters3, null, false, "MissingFileLoggerParameterError", true);
            infoArray2[0x10] = new ParameterizedSwitchInfo(new string[] { "fileloggerparameters4", "flp4" }, ParameterizedSwitch.FileLoggerParameters4, null, false, "MissingFileLoggerParameterError", true);
            infoArray2[0x11] = new ParameterizedSwitchInfo(new string[] { "fileloggerparameters5", "flp5" }, ParameterizedSwitch.FileLoggerParameters5, null, false, "MissingFileLoggerParameterError", true);
            infoArray2[0x12] = new ParameterizedSwitchInfo(new string[] { "fileloggerparameters6", "flp6" }, ParameterizedSwitch.FileLoggerParameters6, null, false, "MissingFileLoggerParameterError", true);
            infoArray2[0x13] = new ParameterizedSwitchInfo(new string[] { "fileloggerparameters7", "flp7" }, ParameterizedSwitch.FileLoggerParameters7, null, false, "MissingFileLoggerParameterError", true);
            infoArray2[20] = new ParameterizedSwitchInfo(new string[] { "fileloggerparameters8", "flp8" }, ParameterizedSwitch.FileLoggerParameters8, null, false, "MissingFileLoggerParameterError", true);
            infoArray2[0x15] = new ParameterizedSwitchInfo(new string[] { "fileloggerparameters9", "flp9" }, ParameterizedSwitch.FileLoggerParameters9, null, false, "MissingFileLoggerParameterError", true);
            infoArray2[0x16] = new ParameterizedSwitchInfo(new string[] { "nodereuse", "nr" }, ParameterizedSwitch.NodeReuse, null, false, "MissingNodeReuseParameterError", true);
            infoArray2[0x17] = new ParameterizedSwitchInfo(new string[] { "preprocess", "pp" }, ParameterizedSwitch.Preprocess, null, false, null, true);
            parameterizedSwitchesMap = infoArray2;
            parameterSeparators = new char[] { ',', ';' };
            noParameters = new string[0];
        }

        internal CommandLineSwitches()
        {
        }

        internal void Append(CommandLineSwitches switchesToAppend)
        {
            if (!this.HaveErrors() && switchesToAppend.HaveErrors())
            {
                this.errorMessage = switchesToAppend.errorMessage;
                this.badCommandLineArg = switchesToAppend.badCommandLineArg;
                this.innerException = switchesToAppend.innerException;
                this.isParameterError = switchesToAppend.isParameterError;
            }
            for (int i = 0; i < 0x13; i++)
            {
                if (switchesToAppend.IsParameterlessSwitchSet((ParameterlessSwitch) i))
                {
                    if (!this.IsParameterlessSwitchSet((ParameterlessSwitch) i) || (parameterlessSwitchesMap[i].duplicateSwitchErrorMessage == null))
                    {
                        this.parameterlessSwitches[i].commandLineArg = switchesToAppend.parameterlessSwitches[i].commandLineArg;
                    }
                    else
                    {
                        this.SetSwitchError(parameterlessSwitchesMap[i].duplicateSwitchErrorMessage, switchesToAppend.GetParameterlessSwitchCommandLineArg((ParameterlessSwitch) i));
                    }
                }
            }
            for (int j = 0; j < 0x18; j++)
            {
                if (switchesToAppend.IsParameterizedSwitchSet((ParameterizedSwitch) j))
                {
                    if (!this.IsParameterizedSwitchSet((ParameterizedSwitch) j) || (parameterizedSwitchesMap[j].duplicateSwitchErrorMessage == null))
                    {
                        if (this.parameterizedSwitches[j].commandLineArg == null)
                        {
                            this.parameterizedSwitches[j].parameters = new ArrayList();
                        }
                        this.parameterizedSwitches[j].commandLineArg = switchesToAppend.parameterizedSwitches[j].commandLineArg;
                        this.parameterizedSwitches[j].parameters.AddRange(switchesToAppend.parameterizedSwitches[j].parameters);
                    }
                    else
                    {
                        this.SetSwitchError(parameterizedSwitchesMap[j].duplicateSwitchErrorMessage, switchesToAppend.GetParameterizedSwitchCommandLineArg((ParameterizedSwitch) j));
                    }
                }
            }
        }

        internal string[][] GetFileLoggerParameters()
        {
            return new string[][] { this.GetSpecificFileLoggerParameters(ParameterlessSwitch.FileLogger, ParameterizedSwitch.FileLoggerParameters), this.GetSpecificFileLoggerParameters(ParameterlessSwitch.FileLogger1, ParameterizedSwitch.FileLoggerParameters1), this.GetSpecificFileLoggerParameters(ParameterlessSwitch.FileLogger2, ParameterizedSwitch.FileLoggerParameters2), this.GetSpecificFileLoggerParameters(ParameterlessSwitch.FileLogger3, ParameterizedSwitch.FileLoggerParameters3), this.GetSpecificFileLoggerParameters(ParameterlessSwitch.FileLogger4, ParameterizedSwitch.FileLoggerParameters4), this.GetSpecificFileLoggerParameters(ParameterlessSwitch.FileLogger5, ParameterizedSwitch.FileLoggerParameters5), this.GetSpecificFileLoggerParameters(ParameterlessSwitch.FileLogger6, ParameterizedSwitch.FileLoggerParameters6), this.GetSpecificFileLoggerParameters(ParameterlessSwitch.FileLogger7, ParameterizedSwitch.FileLoggerParameters7), this.GetSpecificFileLoggerParameters(ParameterlessSwitch.FileLogger8, ParameterizedSwitch.FileLoggerParameters8), this.GetSpecificFileLoggerParameters(ParameterlessSwitch.FileLogger9, ParameterizedSwitch.FileLoggerParameters9) };
        }

        internal string GetParameterizedSwitchCommandLineArg(ParameterizedSwitch parameterizedSwitch)
        {
            return this.parameterizedSwitches[(int) parameterizedSwitch].commandLineArg;
        }

        internal string GetParameterlessSwitchCommandLineArg(ParameterlessSwitch parameterlessSwitch)
        {
            return this.parameterlessSwitches[(int) parameterlessSwitch].commandLineArg;
        }

        private string[] GetSpecificFileLoggerParameters(ParameterlessSwitch parameterlessSwitch, ParameterizedSwitch parameterizedSwitch)
        {
            string[] strArray = null;
            if (this.IsParameterizedSwitchSet(parameterizedSwitch))
            {
                return this[parameterizedSwitch];
            }
            if (this.IsParameterlessSwitchSet(parameterlessSwitch))
            {
                strArray = new string[0];
            }
            return strArray;
        }

        internal bool HaveAnySwitchesBeenSet()
        {
            for (int i = 0; i < 0x13; i++)
            {
                if (this.IsParameterlessSwitchSet((ParameterlessSwitch) i))
                {
                    return true;
                }
            }
            for (int j = 0; j < 0x18; j++)
            {
                if (this.IsParameterizedSwitchSet((ParameterizedSwitch) j))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool HaveErrors()
        {
            return (this.errorMessage != null);
        }

        internal static bool IsParameterizedSwitch(string switchName, out ParameterizedSwitch parameterizedSwitch, out string duplicateSwitchErrorMessage, out bool multipleParametersAllowed, out string missingParametersErrorMessage, out bool unquoteParameters)
        {
            parameterizedSwitch = ParameterizedSwitch.Invalid;
            duplicateSwitchErrorMessage = null;
            multipleParametersAllowed = false;
            missingParametersErrorMessage = null;
            unquoteParameters = false;
            foreach (ParameterizedSwitchInfo info in parameterizedSwitchesMap)
            {
                foreach (string str in info.switchNames)
                {
                    if (string.Compare(switchName, str, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        parameterizedSwitch = info.parameterizedSwitch;
                        duplicateSwitchErrorMessage = info.duplicateSwitchErrorMessage;
                        multipleParametersAllowed = info.multipleParametersAllowed;
                        missingParametersErrorMessage = info.missingParametersErrorMessage;
                        unquoteParameters = info.unquoteParameters;
                        break;
                    }
                }
            }
            return (parameterizedSwitch != ParameterizedSwitch.Invalid);
        }

        internal bool IsParameterizedSwitchSet(ParameterizedSwitch parameterizedSwitch)
        {
            return (this.parameterizedSwitches[(int) parameterizedSwitch].commandLineArg != null);
        }

        internal static bool IsParameterlessSwitch(string switchName)
        {
            ParameterlessSwitch switch2;
            string str;
            return IsParameterlessSwitch(switchName, out switch2, out str);
        }

        internal static bool IsParameterlessSwitch(string switchName, out ParameterlessSwitch parameterlessSwitch, out string duplicateSwitchErrorMessage)
        {
            parameterlessSwitch = ParameterlessSwitch.Invalid;
            duplicateSwitchErrorMessage = null;
            foreach (ParameterlessSwitchInfo info in parameterlessSwitchesMap)
            {
                if (IsParameterlessSwitchEnabled(info))
                {
                    foreach (string str in info.switchNames)
                    {
                        if (string.Compare(switchName, str, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            parameterlessSwitch = info.parameterlessSwitch;
                            duplicateSwitchErrorMessage = info.duplicateSwitchErrorMessage;
                            break;
                        }
                    }
                }
            }
            return (parameterlessSwitch != ParameterlessSwitch.Invalid);
        }

        private static bool IsParameterlessSwitchEnabled(ParameterlessSwitchInfo parameterlessSwitch)
        {
            if (parameterlessSwitch.lightUpKey == null)
            {
                return true;
            }
            if (!parameterlessSwitch.lightUpKeyRead)
            {
                parameterlessSwitch.lightUpKeyRead = true;
                parameterlessSwitch.lightUpKeyResult = ReadLightupBool(parameterlessSwitch.lightUpKey);
            }
            return parameterlessSwitch.lightUpKeyResult;
        }

        internal bool IsParameterlessSwitchSet(ParameterlessSwitch parameterlessSwitch)
        {
            return (this.parameterlessSwitches[(int) parameterlessSwitch].commandLineArg != null);
        }

        private static bool ReadLightupBool(string valueName)
        {
            bool? nullable2 = ReadLightupBool("hkey_current_user", valueName);
            bool? nullable = nullable2.HasValue ? new bool?(nullable2.GetValueOrDefault()) : ReadLightupBool("hkey_local_machine", valueName);
            if (nullable.HasValue)
            {
                return nullable.Value;
            }
            return false;
        }

        private static bool? ReadLightupBool(string root, string valueName)
        {
            try
            {
                object obj2 = Registry.GetValue(string.Format(@"{0}\software\microsoft\msbuild\4.0", root), valueName, null);
                if (obj2 != null)
                {
                    bool flag;
                    TypeCode typeCode = Type.GetTypeCode(obj2.GetType());
                    if (typeCode != TypeCode.Int32)
                    {
                        if (typeCode != TypeCode.String)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return new bool?(((int) obj2) != 0);
                    }
                    if (bool.TryParse((string) obj2, out flag))
                    {
                        return new bool?(flag);
                    }
                    return null;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void SetError(string messageResourceNameValue, string badCommandLineArgValue, Exception innerExceptionValue, bool isParameterErrorValue)
        {
            if (!this.HaveErrors())
            {
                this.errorMessage = messageResourceNameValue;
                this.badCommandLineArg = badCommandLineArgValue;
                this.innerException = innerExceptionValue;
                this.isParameterError = isParameterErrorValue;
            }
        }

        internal void SetParameterError(string messageResourceNameValue, string badCommandLineArgValue)
        {
            this.SetParameterError(messageResourceNameValue, badCommandLineArgValue, null);
        }

        internal void SetParameterError(string messageResourceNameValue, string badCommandLineArgValue, Exception innerExceptionValue)
        {
            this.SetError(messageResourceNameValue, badCommandLineArgValue, innerExceptionValue, true);
        }

        internal bool SetParameterizedSwitch(ParameterizedSwitch parameterizedSwitch, string commandLineArg, string switchParameters, bool multipleParametersAllowed, bool unquoteParameters)
        {
            bool flag = false;
            if (this.parameterizedSwitches[(int) parameterizedSwitch].commandLineArg == null)
            {
                this.parameterizedSwitches[(int) parameterizedSwitch].parameters = new ArrayList();
            }
            this.parameterizedSwitches[(int) parameterizedSwitch].commandLineArg = commandLineArg;
            if (multipleParametersAllowed)
            {
                int num;
                this.parameterizedSwitches[(int) parameterizedSwitch].parameters.AddRange(QuotingUtilities.SplitUnquoted(switchParameters, 0x7fffffff, false, unquoteParameters, out num, parameterSeparators));
                return (num == 0);
            }
            if (unquoteParameters)
            {
                switchParameters = QuotingUtilities.Unquote(switchParameters);
            }
            if (switchParameters.Length > 0)
            {
                this.parameterizedSwitches[(int) parameterizedSwitch].parameters.Add(switchParameters);
                flag = true;
            }
            return flag;
        }

        internal void SetParameterlessSwitch(ParameterlessSwitch parameterlessSwitch, string commandLineArg)
        {
            this.parameterlessSwitches[(int) parameterlessSwitch].commandLineArg = commandLineArg;
        }

        internal void SetSwitchError(string messageResourceNameValue, string badCommandLineArgValue)
        {
            this.SetError(messageResourceNameValue, badCommandLineArgValue, null, false);
        }

        internal void SetUnexpectedParametersError(string badCommandLineArgValue)
        {
            this.SetSwitchError("UnexpectedParametersError", badCommandLineArgValue);
        }

        internal void SetUnknownSwitchError(string badCommandLineArgValue)
        {
            this.SetSwitchError("UnknownSwitchError", badCommandLineArgValue);
        }

        internal void ThrowErrors()
        {
            if (this.HaveErrors())
            {
                if (this.isParameterError)
                {
                    InitializationException.Throw(this.errorMessage, this.badCommandLineArg, this.innerException, false);
                }
                else
                {
                    CommandLineSwitchException.Throw(this.errorMessage, this.badCommandLineArg);
                }
            }
        }

        internal bool this[ParameterlessSwitch parameterlessSwitch]
        {
            get
            {
                return (this.parameterlessSwitches[(int) parameterlessSwitch].commandLineArg != null);
            }
        }

        internal string[] this[ParameterizedSwitch parameterizedSwitch]
        {
            get
            {
                if (this.parameterizedSwitches[(int) parameterizedSwitch].commandLineArg == null)
                {
                    return noParameters;
                }
                return (string[]) this.parameterizedSwitches[(int) parameterizedSwitch].parameters.ToArray(typeof(string));
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DetectedParameterizedSwitch
        {
            internal string commandLineArg;
            internal ArrayList parameters;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DetectedParameterlessSwitch
        {
            internal string commandLineArg;
        }

        internal enum ParameterizedSwitch
        {
            ConsoleLoggerParameters = 7,
            DistributedLogger = 4,
            FileLoggerParameters = 12,
            FileLoggerParameters1 = 13,
            FileLoggerParameters2 = 14,
            FileLoggerParameters3 = 15,
            FileLoggerParameters4 = 0x10,
            FileLoggerParameters5 = 0x11,
            FileLoggerParameters6 = 0x12,
            FileLoggerParameters7 = 0x13,
            FileLoggerParameters8 = 20,
            FileLoggerParameters9 = 0x15,
            IgnoreProjectExtensions = 10,
            Invalid = -1,
            Logger = 3,
            MaxCPUCount = 9,
            NodeMode = 8,
            NodeReuse = 0x16,
            NumberOfParameterizedSwitches = 0x18,
            Preprocess = 0x17,
            Project = 0,
            Property = 2,
            Target = 1,
            ToolsVersion = 11,
            Validate = 6,
            Verbosity = 5
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ParameterizedSwitchInfo
        {
            internal string[] switchNames;
            internal string duplicateSwitchErrorMessage;
            internal bool multipleParametersAllowed;
            internal string missingParametersErrorMessage;
            internal bool unquoteParameters;
            internal CommandLineSwitches.ParameterizedSwitch parameterizedSwitch;
            internal ParameterizedSwitchInfo(string[] switchNames, CommandLineSwitches.ParameterizedSwitch parameterizedSwitch, string duplicateSwitchErrorMessage, bool multipleParametersAllowed, string missingParametersErrorMessage, bool unquoteParameters)
            {
                this.switchNames = switchNames;
                this.duplicateSwitchErrorMessage = duplicateSwitchErrorMessage;
                this.multipleParametersAllowed = multipleParametersAllowed;
                this.missingParametersErrorMessage = missingParametersErrorMessage;
                this.unquoteParameters = unquoteParameters;
                this.parameterizedSwitch = parameterizedSwitch;
            }
        }

        internal enum ParameterlessSwitch
        {
            Debugger = 0x11,
            DetailedSummary = 0x12,
            DistributedFileLogger = 0x10,
            FileLogger = 5,
            FileLogger1 = 6,
            FileLogger2 = 7,
            FileLogger3 = 8,
            FileLogger4 = 9,
            FileLogger5 = 10,
            FileLogger6 = 11,
            FileLogger7 = 12,
            FileLogger8 = 13,
            FileLogger9 = 14,
            Help = 0,
            Invalid = -1,
            NoAutoResponse = 3,
            NoConsoleLogger = 4,
            NoLogo = 2,
            NumberOfParameterlessSwitches = 0x13,
            OldOM = 15,
            Version = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ParameterlessSwitchInfo
        {
            internal string[] switchNames;
            internal string duplicateSwitchErrorMessage;
            internal CommandLineSwitches.ParameterlessSwitch parameterlessSwitch;
            internal string lightUpKey;
            internal bool lightUpKeyRead;
            internal bool lightUpKeyResult;
            internal ParameterlessSwitchInfo(string[] switchNames, CommandLineSwitches.ParameterlessSwitch parameterlessSwitch, string duplicateSwitchErrorMessage, string lightUpRegistryKey)
            {
                this.switchNames = switchNames;
                this.duplicateSwitchErrorMessage = duplicateSwitchErrorMessage;
                this.parameterlessSwitch = parameterlessSwitch;
                this.lightUpKey = lightUpRegistryKey;
                this.lightUpKeyRead = false;
                this.lightUpKeyResult = false;
            }
        }
    }
}

