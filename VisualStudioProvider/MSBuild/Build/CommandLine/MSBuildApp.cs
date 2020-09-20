namespace Microsoft.Build.CommandLine
{
    using Microsoft.Build.BuildEngine;
    using Microsoft.Build.Evaluation;
    using Microsoft.Build.Exceptions;
    using Microsoft.Build.Execution;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Logging;
    using Microsoft.Build.Shared;
    using Microsoft.Internal.Performance;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Threading;

    public static class MSBuildApp
    {
        private static BuildSubmission activeBuild;
        private const string autoResponseFileName = "MSBuild.rsp";
        private static ManualResetEvent buildComplete = new ManualResetEvent(false);
        private static object buildLock = new object();
        private static ManualResetEvent cancelComplete = new ManualResetEvent(true);
        private static readonly string exePath;
        private static ArrayList includedResponseFiles;
        private static bool initialized;
        private const string msbuildLogFileName = "msbuild.log";
        private static readonly char[] propertyValueSeparator = new char[] { '=' };
        private static int receivedCancel;
        internal static bool usingSwitchesFromAutoResponseFile = false;
        private static readonly char[] wildcards = new char[] { '*', '?' };

        static MSBuildApp()
        {
            try
            {
                exePath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().EscapedCodeBase).LocalPath);
                initialized = true;
            }
            catch (TypeInitializationException exception)
            {
                if ((exception.InnerException == null) || (exception.InnerException.GetType() != typeof(ConfigurationErrorsException)))
                {
                    throw;
                }
                HandleConfigurationException(exception);
            }
            catch (ConfigurationException exception2)
            {
                HandleConfigurationException(exception2);
            }
        }

        internal static string AggregateParameters(string anyPrefixingParameter, string[] parametersToAggregate)
        {
            for (int i = 0; i < parametersToAggregate.Length; i++)
            {
                parametersToAggregate[i] = parametersToAggregate[i].Trim(new char[] { ';' });
            }
            string str = anyPrefixingParameter ?? string.Empty;
            return (str + string.Join(";", parametersToAggregate));
        }

        internal static bool BuildProject(string projectFile, string[] targets, string toolsVersion, Dictionary<string, string> globalProperties, ILogger[] loggers, LoggerVerbosity verbosity, DistributedLoggerRecord[] distributedLoggerRecords, bool needToValidateProject, string schemaFile, int cpuCount, bool enableNodeReuse, TextWriter preprocessWriter, bool debugger, bool detailedSummary)
        {
            if (string.Equals(Path.GetExtension(projectFile), ".vcproj", StringComparison.OrdinalIgnoreCase) || string.Equals(Path.GetExtension(projectFile), ".dsp", StringComparison.OrdinalIgnoreCase))
            {
                InitializationException.Throw(Microsoft.Build.Shared.ResourceUtilities.FormatResourceString("ProjectUpgradeNeededToVcxProj", new object[] { projectFile }), null);
            }
            bool flag = false;
            ProjectCollection projectCollection = null;
            bool onlyLogCriticalEvents = false;
            try
            {
                List<ForwardingLoggerRecord> list = new List<ForwardingLoggerRecord>();
                foreach (DistributedLoggerRecord record in distributedLoggerRecords)
                {
                    list.Add(new ForwardingLoggerRecord(record.CentralLogger, record.ForwardingLoggerDescription));
                }
                if ((((loggers.Length == 1) && (verbosity == LoggerVerbosity.Quiet)) && ((loggers[0].Parameters.IndexOf("ENABLEMPLOGGING", StringComparison.OrdinalIgnoreCase) != -1) && (loggers[0].Parameters.IndexOf("DISABLEMPLOGGING", StringComparison.OrdinalIgnoreCase) == -1))) && ((loggers[0].Parameters.IndexOf("V=", StringComparison.OrdinalIgnoreCase) == -1) && (loggers[0].Parameters.IndexOf("VERBOSITY=", StringComparison.OrdinalIgnoreCase) == -1)))
                {
                    Type type = loggers[0].GetType();
                    Type type2 = typeof(Microsoft.Build.Logging.ConsoleLogger);
                    if (type == type2)
                    {
                        onlyLogCriticalEvents = true;
                    }
                }
                projectCollection = new ProjectCollection(globalProperties, loggers, null, Microsoft.Build.Evaluation.ToolsetDefinitionLocations.Registry | Microsoft.Build.Evaluation.ToolsetDefinitionLocations.ConfigurationFile, cpuCount, onlyLogCriticalEvents);
                if (debugger)
                {
                    Environment.SetEnvironmentVariable("MSBUILDDEBUGGING", "1");
                }
                if ((toolsVersion != null) && !projectCollection.ContainsToolset(toolsVersion))
                {
                    ThrowInvalidToolsVersionInitializationException(projectCollection.Toolsets, toolsVersion);
                }
                if (needToValidateProject && !Microsoft.Build.Shared.FileUtilities.IsSolutionFilename(projectFile))
                {
                    Microsoft.Build.Evaluation.Project project = projectCollection.LoadProject(projectFile, globalProperties, toolsVersion);
                    Microsoft.Build.Evaluation.Toolset toolset = projectCollection.GetToolset((toolsVersion == null) ? project.ToolsVersion : toolsVersion);
                    if (toolset == null)
                    {
                        ThrowInvalidToolsVersionInitializationException(projectCollection.Toolsets, project.ToolsVersion);
                    }
                    Microsoft.Build.CommandLine.ProjectSchemaValidationHandler.VerifyProjectSchema(projectFile, schemaFile, toolset.ToolsPath);
                    projectCollection.UnloadProject(project);
                }
                if ((preprocessWriter != null) && !Microsoft.Build.Shared.FileUtilities.IsSolutionFilename(projectFile))
                {
                    Microsoft.Build.Evaluation.Project project2 = projectCollection.LoadProject(projectFile, globalProperties, toolsVersion);
                    project2.SaveLogicalProject(preprocessWriter);
                    projectCollection.UnloadProject(project2);
                    return flag;
                }
                BuildRequestData requestData = new BuildRequestData(projectFile, globalProperties, toolsVersion, targets, null);
                BuildParameters parameters = new BuildParameters(projectCollection);
                if (!string.Equals(Environment.GetEnvironmentVariable("MSBUILDLOGASYNC"), "1", StringComparison.Ordinal))
                {
                    parameters.UseSynchronousLogging = true;
                }
                parameters.EnableNodeReuse = enableNodeReuse;
                parameters.NodeExeLocation = Assembly.GetExecutingAssembly().Location;
                parameters.MaxNodeCount = cpuCount;
                parameters.Loggers = projectCollection.Loggers;
                parameters.ForwardingLoggers = list;
                parameters.ToolsetDefinitionLocations = Microsoft.Build.Evaluation.ToolsetDefinitionLocations.Registry | Microsoft.Build.Evaluation.ToolsetDefinitionLocations.ConfigurationFile;
                parameters.DetailedSummary = detailedSummary;
                if (!string.IsNullOrEmpty(toolsVersion))
                {
                    parameters.DefaultToolsVersion = toolsVersion;
                }
                string environmentVariable = Environment.GetEnvironmentVariable("MSBUILDMEMORYUSELIMIT");
                if (!string.IsNullOrEmpty(environmentVariable))
                {
                    parameters.MemoryUseLimit = Convert.ToInt32(environmentVariable, CultureInfo.InvariantCulture);
                    if (parameters.MemoryUseLimit < parameters.MaxNodeCount)
                    {
                        parameters.MemoryUseLimit = parameters.MaxNodeCount;
                    }
                }
                BuildManager defaultBuildManager = BuildManager.DefaultBuildManager;
                Microsoft.Build.Execution.BuildResult result = null;
                defaultBuildManager.BeginBuild(parameters);
                Exception exception = null;
                try
                {
                    try
                    {
                        lock (buildLock)
                        {
                            activeBuild = defaultBuildManager.PendBuildRequest(requestData);
                        }
                        result = activeBuild.Execute();
                    }
                    finally
                    {
                        defaultBuildManager.EndBuild();
                    }
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    flag = false;
                }
                if ((result != null) && (exception == null))
                {
                    flag = result.OverallResult == BuildResultCode.Success;
                    exception = result.Exception;
                }
                if (exception != null)
                {
                    flag = false;
                    if (!(exception.GetType() != typeof(Microsoft.Build.Exceptions.InvalidProjectFileException)))
                    {
                        return flag;
                    }
                    if ((exception.GetType() == typeof(LoggerException)) || (exception.GetType() == typeof(Microsoft.Build.Exceptions.InternalLoggerException)))
                    {
                        throw exception;
                    }
                    if (!(exception.GetType() == typeof(BuildAbortedException)))
                    {
                        Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("FatalError"));
                        Console.WriteLine(exception.ToString());
                        Console.WriteLine();
                        throw exception;
                    }
                }
                return flag;
            }
            catch (Microsoft.Build.Exceptions.InvalidProjectFileException exception3)
            {
                Microsoft.Build.Shared.ErrorUtilities.VerifyThrow(exception3.HasBeenLogged, "Should have been logged");
                flag = false;
            }
            finally
            {
                Microsoft.Build.Shared.FileUtilities.ClearCacheDirectory();
                if (projectCollection != null)
                {
                    projectCollection.Dispose();
                }
            }
            return flag;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool BuildProjectWithOldOM(string projectFile, string[] targets, string toolsVersion, BuildPropertyGroup propertyBag, ILogger[] loggers, LoggerVerbosity verbosity, DistributedLoggerRecord[] distributedLoggerRecords, bool needToValidateProject, string schemaFile, int cpuCount)
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetAssembly(typeof(MSBuildApp)).Location);
            string localNodeProviderParameters = "msbuildlocation=" + directoryName + ";nodereuse=false";
            Engine engine = new Engine(propertyBag, Microsoft.Build.BuildEngine.ToolsetDefinitionLocations.Registry | Microsoft.Build.BuildEngine.ToolsetDefinitionLocations.ConfigurationFile, cpuCount, localNodeProviderParameters);
            bool flag = false;
            try
            {
                foreach (ILogger logger in loggers)
                {
                    engine.RegisterLogger(logger);
                }
                if ((((loggers.Length == 1) && (verbosity == LoggerVerbosity.Quiet)) && ((loggers[0].Parameters.IndexOf("ENABLEMPLOGGING", StringComparison.OrdinalIgnoreCase) != -1) && (loggers[0].Parameters.IndexOf("DISABLEMPLOGGING", StringComparison.OrdinalIgnoreCase) == -1))) && ((loggers[0].Parameters.IndexOf("V=", StringComparison.OrdinalIgnoreCase) == -1) && (loggers[0].Parameters.IndexOf("VERBOSITY=", StringComparison.OrdinalIgnoreCase) == -1)))
                {
                    Type type = loggers[0].GetType();
                    Type type2 = typeof(Microsoft.Build.Logging.ConsoleLogger);
                    if (type == type2)
                    {
                        engine.OnlyLogCriticalEvents = true;
                    }
                }
                Microsoft.Build.BuildEngine.Project project = null;
                try
                {
                    project = new Microsoft.Build.BuildEngine.Project(engine, toolsVersion);
                }
                catch (InvalidOperationException exception)
                {
                    InitializationException.Throw("InvalidToolsVersionError", toolsVersion, exception, false);
                }
                project.IsValidated = needToValidateProject;
                project.SchemaFile = schemaFile;
                project.Load(projectFile);
                flag = engine.BuildProject(project, targets);
            }
            catch (Microsoft.Build.Exceptions.InvalidProjectFileException)
            {
            }
            finally
            {
                engine.Shutdown();
            }
            return flag;
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (e.SpecialKey == ConsoleSpecialKey.ControlBreak)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
                if (Interlocked.CompareExchange(ref receivedCancel, 1, 0) != 1)
                {
                    Console.WriteLine(Microsoft.Build.Shared.ResourceUtilities.FormatResourceString("AbortingBuild", new object[0]));
                    WaitCallback callBack = delegate {
                        cancelComplete.Reset();
                        if (buildComplete.WaitOne(0, false))
                        {
                            cancelComplete.Set();
                        }
                        else
                        {
                            BuildSubmission activeBuild = null;
                            lock (buildLock)
                            {
                                activeBuild = MSBuildApp.activeBuild;
                            }
                            if (activeBuild != null)
                            {
                                BuildManager.DefaultBuildManager.CancelAllSubmissions();
                                buildComplete.WaitOne();
                            }
                            cancelComplete.Set();
                        }
                    };
                    ThreadPool.QueueUserWorkItem(callBack);
                }
            }
        }

        private static ILogger CreateAndConfigureLogger(Microsoft.Build.Logging.LoggerDescription loggerDescription, LoggerVerbosity verbosity, string unquotedParameter)
        {
            ILogger logger = null;
            try
            {
                logger = loggerDescription.CreateLogger();
                if (logger == null)
                {
                    InitializationException.VerifyThrow(logger != null, "LoggerNotFoundError", unquotedParameter);
                }
            }
            catch (IOException exception)
            {
                InitializationException.Throw("LoggerCreationError", unquotedParameter, exception, false);
            }
            catch (BadImageFormatException exception2)
            {
                InitializationException.Throw("LoggerCreationError", unquotedParameter, exception2, false);
            }
            catch (SecurityException exception3)
            {
                InitializationException.Throw("LoggerCreationError", unquotedParameter, exception3, false);
            }
            catch (ReflectionTypeLoadException exception4)
            {
                InitializationException.Throw("LoggerCreationError", unquotedParameter, exception4, false);
            }
            catch (MemberAccessException exception5)
            {
                InitializationException.Throw("LoggerCreationError", unquotedParameter, exception5, false);
            }
            catch (TargetInvocationException exception6)
            {
                InitializationException.Throw("LoggerFatalError", unquotedParameter, exception6.InnerException, true);
            }
            try
            {
                logger.Verbosity = verbosity;
                if (loggerDescription.LoggerSwitchParameters != null)
                {
                    logger.Parameters = loggerDescription.LoggerSwitchParameters;
                }
            }
            catch (LoggerException)
            {
                throw;
            }
            catch (Exception exception7)
            {
                InitializationException.Throw("LoggerFatalError", unquotedParameter, exception7, true);
            }
            return logger;
        }

        private static DistributedLoggerRecord CreateForwardingLoggerRecord(ILogger logger, string loggerParameters, LoggerVerbosity defaultVerbosity)
        {
            string str2 = ExtractAnyParameterValue(ExtractAnyLoggerParameter(loggerParameters, new string[] { "verbosity", "v" }));
            LoggerVerbosity verbosity = defaultVerbosity;
            if (!string.IsNullOrEmpty(str2))
            {
                verbosity = ProcessVerbositySwitch(str2);
            }
            Assembly assembly = Assembly.GetAssembly(typeof(ProjectCollection));
            string loggerClassName = "Microsoft.Build.Logging.ConfigurableForwardingLogger";
            string fullName = assembly.GetName().FullName;
            return new DistributedLoggerRecord(logger, new Microsoft.Build.Logging.LoggerDescription(loggerClassName, fullName, null, loggerParameters, verbosity));
        }

        private static void DisplayCopyrightMessage()
        {
            Console.WriteLine(Microsoft.Build.Shared.ResourceUtilities.FormatResourceString("CopyrightMessage", new object[] { ProjectCollection.Version.ToString(), Environment.Version.ToString() }));
        }

        private static void DumpAllInCategory(string currentInstance, PerformanceCounterCategory category, bool initializeOnly)
        {
            if (category.CategoryName.IndexOf("remoting", StringComparison.OrdinalIgnoreCase) == -1)
            {
                System.Diagnostics.PerformanceCounter[] counters;
                try
                {
                    counters = category.GetCounters(currentInstance);
                }
                catch (InvalidOperationException)
                {
                    return;
                }
                if (!initializeOnly)
                {
                    Console.WriteLine("\n{0}{1}{0}", new string('=', 0x29 - (category.CategoryName.Length / 2)), category.CategoryName);
                }
                foreach (System.Diagnostics.PerformanceCounter counter in counters)
                {
                    DumpCounter(counter, initializeOnly);
                }
                if (!initializeOnly)
                {
                    Console.WriteLine("{0}{0}", new string('=', 0x29));
                }
            }
        }

        private static void DumpCounter(System.Diagnostics.PerformanceCounter counter, bool initializeOnly)
        {
            if (counter.CounterName.IndexOf("not displayed", StringComparison.OrdinalIgnoreCase) == -1)
            {
                float num = counter.NextValue();
                if (!initializeOnly)
                {
                    string friendlyCounterType = GetFriendlyCounterType(counter.CounterType, counter.CounterName);
                    string format = (num < 10f) ? "{0,20:N2}" : "{0,20:N0}";
                    string str3 = string.Format(CultureInfo.CurrentCulture, format, new object[] { num });
                    Console.WriteLine("||{0,50}|{1}|{2,8}|", counter.CounterName, str3, friendlyCounterType);
                }
            }
        }

        private static void DumpCounters(bool initializeOnly)
        {
            Process currentProcess = Process.GetCurrentProcess();
            if (!initializeOnly)
            {
                Console.WriteLine("\n{0}{1}{0}", new string('=', 0x29 - ("Process".Length / 2)), "Process");
                Console.WriteLine("||{0,50}|{1,20:N0}|{2,8}|", "Peak Working Set", currentProcess.PeakWorkingSet64, "bytes");
                Console.WriteLine("||{0,50}|{1,20:N0}|{2,8}|", "Peak Paged Memory", currentProcess.PeakPagedMemorySize64, "bytes");
                Console.WriteLine("||{0,50}|{1,20:N0}|{2,8}|", "Peak Virtual Memory", currentProcess.PeakVirtualMemorySize64, "bytes");
                Console.WriteLine("||{0,50}|{1,20:N0}|{2,8}|", "Peak Privileged Processor Time", currentProcess.PrivilegedProcessorTime.TotalMilliseconds, "ms");
                Console.WriteLine("||{0,50}|{1,20:N0}|{2,8}|", "Peak User Processor Time", currentProcess.UserProcessorTime.TotalMilliseconds, "ms");
                Console.WriteLine("||{0,50}|{1,20:N0}|{2,8}|", "Peak Total Processor Time", currentProcess.TotalProcessorTime.TotalMilliseconds, "ms");
                Console.WriteLine("{0}{0}", new string('=', 0x29));
            }
            string currentInstance = null;
            PerformanceCounterCategory category = new PerformanceCounterCategory("Process");
            foreach (string str2 in category.GetInstanceNames())
            {
                System.Diagnostics.PerformanceCounter counter = new System.Diagnostics.PerformanceCounter(".NET CLR Memory", "Process ID", str2, true);
                try
                {
                    if (((int) counter.RawValue) == currentProcess.Id)
                    {
                        currentInstance = str2;
                        break;
                    }
                }
                catch (InvalidOperationException)
                {
                }
                finally
                {
                    if (counter != null)
                    {
                        counter.Dispose();
                    }
                }
            }
            foreach (PerformanceCounterCategory category2 in PerformanceCounterCategory.GetCategories())
            {
                DumpAllInCategory(currentInstance, category2, initializeOnly);
            }
        }

        public static ExitType Execute(string projectFile = null, string[] targets = null, string toolsVersion = null, Dictionary<string, string> globalProperties = null, ILogger[] loggers = null, LoggerVerbosity normal = LoggerVerbosity.Normal)
        {
            ExitType success = ExitType.Success;
            ConsoleCancelEventHandler handler = new ConsoleCancelEventHandler(MSBuildApp.Console_CancelKeyPress);
            try
            {
                Microsoft.Internal.Performance.CodeMarkers.Instance.InitPerformanceDll(CodeMarkerApp.MSBUILDPERF, @"Software\Microsoft\MSBuild\4.0");
                Console.CancelKeyPress += handler;
                VerifyThrowSupportedOS();
                SetConsoleUI();
                ResetBuildState();

                if (Environment.GetEnvironmentVariable("MSBUILDDEBUGONSTART") == "1")
                {
                    Debugger.Launch();
                }

                if (targets == null)
                {
                    targets = new string[0];
                }

                if (loggers == null)
                {
                    loggers = new ILogger[0];
                }

                List<DistributedLoggerRecord> distributedLoggerRecords = new List<DistributedLoggerRecord>();
                bool needToValidateProject = false;
                string schemaFile = null;
                int cpuCount = 1;
                bool enableNodeReuse = true;
                TextWriter preprocessWriter = null;
                bool debugger = false;
                bool detailedSummary = false;
                if (Environment.GetEnvironmentVariable("MSBUILDOLDOM") != "1")
                {
                    if (!BuildProject(projectFile, targets, toolsVersion, globalProperties, loggers, normal, distributedLoggerRecords.ToArray(), needToValidateProject, schemaFile, cpuCount, enableNodeReuse, preprocessWriter, debugger, detailedSummary))
                    {
                        success = ExitType.BuildError;
                    }
                    return success;
                }
                return OldOMBuildProject(success, projectFile, targets, toolsVersion, globalProperties, loggers, normal, needToValidateProject, schemaFile, cpuCount);
            }
            catch (CommandLineSwitchException exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine();
                ShowHelpPrompt();
                success = ExitType.SwitchError;
            }
            catch (Microsoft.Build.Exceptions.InvalidToolsetDefinitionException exception2)
            {
                Console.WriteLine(Microsoft.Build.Shared.ResourceUtilities.FormatResourceString("ConfigurationFailurePrefixNoErrorCode", new object[] { exception2.ErrorCode, exception2.Message }));
                success = ExitType.InitializationError;
            }
            catch (InitializationException exception3)
            {
                Console.WriteLine(exception3.Message);
                success = ExitType.InitializationError;
            }
            catch (LoggerException exception4)
            {
                if (exception4.ErrorCode != null)
                {
                    Console.WriteLine(Microsoft.Build.Shared.ResourceUtilities.FormatResourceString("LoggerFailurePrefixNoErrorCode", new object[] { exception4.ErrorCode, exception4.Message }));
                }
                else
                {
                    Console.WriteLine(Microsoft.Build.Shared.ResourceUtilities.FormatResourceString("LoggerFailurePrefixWithErrorCode", new object[] { exception4.Message }));
                }
                if (exception4.InnerException != null)
                {
                    Console.WriteLine(exception4.InnerException.ToString());
                }
                success = ExitType.LoggerAbort;
            }
            catch (Microsoft.Build.Exceptions.InternalLoggerException exception5)
            {
                if (!exception5.InitializationException)
                {
                    Console.WriteLine("MSBUILD : error " + exception5.ErrorCode + ": " + exception5.Message);
                    Console.WriteLine(exception5.InnerException.ToString());
                    return ExitType.LoggerFailure;
                }
                Console.WriteLine("MSBUILD : error " + exception5.ErrorCode + ": " + exception5.Message + ((exception5.InnerException != null) ? (" " + exception5.InnerException.Message) : ""));
                return ExitType.InitializationError;
            }
            catch (BuildAbortedException exception6)
            {
                Console.WriteLine("MSBUILD : error " + exception6.ErrorCode + ": " + exception6.Message + ((exception6.InnerException != null) ? (" " + exception6.InnerException.Message) : string.Empty));
                success = ExitType.Unexpected;
            }
            catch (Exception exception7)
            {
                Console.WriteLine("{0}\r\n{1}", Microsoft.Build.Shared.AssemblyResources.GetString("FatalError"), exception7.ToString());
                throw;
            }
            finally
            {
                buildComplete.Set();
                Console.CancelKeyPress -= handler;
                cancelComplete.WaitOne();
                Microsoft.Internal.Performance.CodeMarkers.Instance.UninitializePerformanceDLL(CodeMarkerApp.MSBUILDPERF);
            }
            return success;
        }


        public static ExitType Execute(string commandLine)
        {
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrowArgumentLength(commandLine, "commandLine");
            ExitType success = ExitType.Success;
            ConsoleCancelEventHandler handler = new ConsoleCancelEventHandler(MSBuildApp.Console_CancelKeyPress);
            try
            {
                Microsoft.Internal.Performance.CodeMarkers.Instance.InitPerformanceDll(CodeMarkerApp.MSBUILDPERF, @"Software\Microsoft\MSBuild\4.0");
                Console.CancelKeyPress += handler;
                VerifyThrowSupportedOS();
                SetConsoleUI();
                ResetBuildState();
                if (Environment.GetEnvironmentVariable("MSBUILDDEBUGONSTART") == "1")
                {
                    Debugger.Launch();
                }
                string projectFile = null;
                string[] targets = new string[0];
                string toolsVersion = null;
                Dictionary<string, string> globalProperties = null;
                ILogger[] loggers = new ILogger[0];
                LoggerVerbosity normal = LoggerVerbosity.Normal;
                List<DistributedLoggerRecord> distributedLoggerRecords = null;
                bool needToValidateProject = false;
                string schemaFile = null;
                int cpuCount = 1;
                bool enableNodeReuse = true;
                TextWriter preprocessWriter = null;
                bool debugger = false;
                bool detailedSummary = false;
                if (!ProcessCommandLineSwitches(GatherAllSwitches(commandLine), ref projectFile, ref targets, ref toolsVersion, ref globalProperties, ref loggers, ref normal, ref distributedLoggerRecords, ref needToValidateProject, ref schemaFile, ref cpuCount, ref enableNodeReuse, ref preprocessWriter, ref debugger, ref detailedSummary))
                {
                    return success;
                }
                if (Environment.GetEnvironmentVariable("MSBUILDOLDOM") != "1")
                {
                    if (!BuildProject(projectFile, targets, toolsVersion, globalProperties, loggers, normal, distributedLoggerRecords.ToArray(), needToValidateProject, schemaFile, cpuCount, enableNodeReuse, preprocessWriter, debugger, detailedSummary))
                    {
                        success = ExitType.BuildError;
                    }
                    return success;
                }
                return OldOMBuildProject(success, projectFile, targets, toolsVersion, globalProperties, loggers, normal, needToValidateProject, schemaFile, cpuCount);
            }
            catch (CommandLineSwitchException exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine();
                ShowHelpPrompt();
                success = ExitType.SwitchError;
            }
            catch (Microsoft.Build.Exceptions.InvalidToolsetDefinitionException exception2)
            {
                Console.WriteLine(Microsoft.Build.Shared.ResourceUtilities.FormatResourceString("ConfigurationFailurePrefixNoErrorCode", new object[] { exception2.ErrorCode, exception2.Message }));
                success = ExitType.InitializationError;
            }
            catch (InitializationException exception3)
            {
                Console.WriteLine(exception3.Message);
                success = ExitType.InitializationError;
            }
            catch (LoggerException exception4)
            {
                if (exception4.ErrorCode != null)
                {
                    Console.WriteLine(Microsoft.Build.Shared.ResourceUtilities.FormatResourceString("LoggerFailurePrefixNoErrorCode", new object[] { exception4.ErrorCode, exception4.Message }));
                }
                else
                {
                    Console.WriteLine(Microsoft.Build.Shared.ResourceUtilities.FormatResourceString("LoggerFailurePrefixWithErrorCode", new object[] { exception4.Message }));
                }
                if (exception4.InnerException != null)
                {
                    Console.WriteLine(exception4.InnerException.ToString());
                }
                success = ExitType.LoggerAbort;
            }
            catch (Microsoft.Build.Exceptions.InternalLoggerException exception5)
            {
                if (!exception5.InitializationException)
                {
                    Console.WriteLine("MSBUILD : error " + exception5.ErrorCode + ": " + exception5.Message);
                    Console.WriteLine(exception5.InnerException.ToString());
                    return ExitType.LoggerFailure;
                }
                Console.WriteLine("MSBUILD : error " + exception5.ErrorCode + ": " + exception5.Message + ((exception5.InnerException != null) ? (" " + exception5.InnerException.Message) : ""));
                return ExitType.InitializationError;
            }
            catch (BuildAbortedException exception6)
            {
                Console.WriteLine("MSBUILD : error " + exception6.ErrorCode + ": " + exception6.Message + ((exception6.InnerException != null) ? (" " + exception6.InnerException.Message) : string.Empty));
                success = ExitType.Unexpected;
            }
            catch (Exception exception7)
            {
                Console.WriteLine("{0}\r\n{1}", Microsoft.Build.Shared.AssemblyResources.GetString("FatalError"), exception7.ToString());
                throw;
            }
            finally
            {
                buildComplete.Set();
                Console.CancelKeyPress -= handler;
                cancelComplete.WaitOne();
                Microsoft.Internal.Performance.CodeMarkers.Instance.UninitializePerformanceDLL(CodeMarkerApp.MSBUILDPERF);
            }
            return success;
        }

        internal static string ExtractAnyLoggerParameter(string parameters, params string[] parameterNames)
        {
            string[] strArray = parameters.Split(new char[] { ';' });
            string str = null;
            foreach (string str2 in strArray)
            {
                foreach (string str3 in parameterNames)
                {
                    if (str2.StartsWith(str3 + "=", StringComparison.OrdinalIgnoreCase) || string.Equals(str3, str2, StringComparison.OrdinalIgnoreCase))
                    {
                        str = str2;
                    }
                }
            }
            return str;
        }

        private static string ExtractAnyParameterValue(string parameter)
        {
            string str = null;
            if (!string.IsNullOrEmpty(parameter))
            {
                string[] strArray = parameter.Split(new char[] { '=' });
                str = (strArray.Length > 1) ? strArray[1] : null;
            }
            return str;
        }

        internal static string ExtractSwitchParameters(string commandLineArg, string unquotedCommandLineArg, int doubleQuotesRemovedFromArg, string switchName, int switchParameterIndicator)
        {
            string str = null;
            int num2;
            int index = commandLineArg.IndexOf(':');
            string str2 = QuotingUtilities.Unquote(commandLineArg.Substring(0, index), out num2);
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrow(switchName == str2.Substring(1), "The switch name extracted from either the partially or completely unquoted arg should be the same.");
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrow(doubleQuotesRemovedFromArg >= num2, "The name portion of the switch cannot contain more quoting than the arg itself.");
            if ((num2 % 2) == 0)
            {
                str = commandLineArg.Substring(index);
            }
            else
            {
                int num3 = commandLineArg.IndexOf('"', index + 1);
                if (((doubleQuotesRemovedFromArg - num2) <= 1) && ((num3 == -1) || (num3 == (commandLineArg.Length - 1))))
                {
                    str = unquotedCommandLineArg.Substring(switchParameterIndicator);
                }
                else
                {
                    str = ":\"" + commandLineArg.Substring(index + 1);
                }
            }
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrow(str != null, "We must be able to extract the switch parameters.");
            return str;
        }

        private static CommandLineSwitches GatherAllSwitches(string commandLine)
        {
            ArrayList commandLineArgs = QuotingUtilities.SplitUnquoted(commandLine, new char[0]);
            CommandLineSwitches commandLineSwitches = new CommandLineSwitches();
            GatherCommandLineSwitches(commandLineArgs, commandLineSwitches);
            return GatherAutoResponseFileSwitches(commandLineSwitches);
        }

        private static CommandLineSwitches GatherAutoResponseFileSwitches(CommandLineSwitches commandLineSwitches)
        {
            CommandLineSwitches switches = commandLineSwitches;
            if (!commandLineSwitches[CommandLineSwitches.ParameterlessSwitch.NoAutoResponse])
            {
                string path = Path.Combine(exePath, "MSBuild.rsp");
                if (!File.Exists(path))
                {
                    return switches;
                }
                switches = new CommandLineSwitches();
                GatherResponseFileSwitch("@" + path, switches);
                if (switches[CommandLineSwitches.ParameterlessSwitch.NoAutoResponse])
                {
                    switches.SetSwitchError("CannotAutoDisableAutoResponseFile", switches.GetParameterlessSwitchCommandLineArg(CommandLineSwitches.ParameterlessSwitch.NoAutoResponse));
                }
                if (switches.HaveAnySwitchesBeenSet())
                {
                    usingSwitchesFromAutoResponseFile = true;
                }
                switches.Append(commandLineSwitches);
            }
            return switches;
        }

        internal static void GatherCommandLineSwitches(ArrayList commandLineArgs, CommandLineSwitches commandLineSwitches)
        {
            foreach (string str in commandLineArgs)
            {
                int num;
                string unquotedCommandLineArg = QuotingUtilities.Unquote(str, out num);
                if (unquotedCommandLineArg.Length > 0)
                {
                    string str3;
                    string str4;
                    CommandLineSwitches.ParameterlessSwitch switch2;
                    string str5;
                    if (unquotedCommandLineArg.StartsWith("@", StringComparison.Ordinal))
                    {
                        GatherResponseFileSwitch(unquotedCommandLineArg, commandLineSwitches);
                        continue;
                    }
                    if (!unquotedCommandLineArg.StartsWith("-", StringComparison.Ordinal) && !unquotedCommandLineArg.StartsWith("/", StringComparison.Ordinal))
                    {
                        str3 = null;
                        str4 = ":" + str;
                    }
                    else
                    {
                        int index = unquotedCommandLineArg.IndexOf(':');
                        if (index == -1)
                        {
                            str3 = unquotedCommandLineArg.Substring(1);
                            str4 = string.Empty;
                        }
                        else
                        {
                            str3 = unquotedCommandLineArg.Substring(1, index - 1);
                            str4 = ExtractSwitchParameters(str, unquotedCommandLineArg, num, str3, index);
                        }
                    }
                    if (string.IsNullOrEmpty(str4) && (string.Equals(str3, "m", StringComparison.OrdinalIgnoreCase) || string.Equals(str3, "maxcpucount", StringComparison.OrdinalIgnoreCase)))
                    {
                        int processorCount = Environment.ProcessorCount;
                        str4 = ":" + processorCount;
                    }
                    if (CommandLineSwitches.IsParameterlessSwitch(str3, out switch2, out str5))
                    {
                        GatherParameterlessCommandLineSwitch(commandLineSwitches, switch2, str4, str5, unquotedCommandLineArg);
                    }
                    else
                    {
                        CommandLineSwitches.ParameterizedSwitch switch3;
                        bool flag;
                        string str6;
                        bool flag2;
                        if (CommandLineSwitches.IsParameterizedSwitch(str3, out switch3, out str5, out flag, out str6, out flag2))
                        {
                            GatherParameterizedCommandLineSwitch(commandLineSwitches, switch3, str4, str5, flag, str6, flag2, unquotedCommandLineArg);
                            continue;
                        }
                        commandLineSwitches.SetUnknownSwitchError(unquotedCommandLineArg);
                    }
                }
            }
        }

        private static void GatherParameterizedCommandLineSwitch(CommandLineSwitches commandLineSwitches, CommandLineSwitches.ParameterizedSwitch parameterizedSwitch, string switchParameters, string duplicateSwitchErrorMessage, bool multipleParametersAllowed, string missingParametersErrorMessage, bool unquoteParameters, string unquotedCommandLineArg)
        {
            if ((switchParameters.Length > 1) || (missingParametersErrorMessage == null))
            {
                if (commandLineSwitches.IsParameterizedSwitchSet(parameterizedSwitch) && (duplicateSwitchErrorMessage != null))
                {
                    commandLineSwitches.SetSwitchError(duplicateSwitchErrorMessage, unquotedCommandLineArg);
                }
                else
                {
                    if (switchParameters.Length > 0)
                    {
                        switchParameters = switchParameters.Substring(1);
                    }
                    if (!commandLineSwitches.SetParameterizedSwitch(parameterizedSwitch, unquotedCommandLineArg, switchParameters, multipleParametersAllowed, unquoteParameters) && (missingParametersErrorMessage != null))
                    {
                        commandLineSwitches.SetSwitchError(missingParametersErrorMessage, unquotedCommandLineArg);
                    }
                }
            }
            else
            {
                commandLineSwitches.SetSwitchError(missingParametersErrorMessage, unquotedCommandLineArg);
            }
        }

        private static void GatherParameterlessCommandLineSwitch(CommandLineSwitches commandLineSwitches, CommandLineSwitches.ParameterlessSwitch parameterlessSwitch, string switchParameters, string duplicateSwitchErrorMessage, string unquotedCommandLineArg)
        {
            if (switchParameters.Length == 0)
            {
                if (!commandLineSwitches.IsParameterlessSwitchSet(parameterlessSwitch) || (duplicateSwitchErrorMessage == null))
                {
                    commandLineSwitches.SetParameterlessSwitch(parameterlessSwitch, unquotedCommandLineArg);
                }
                else
                {
                    commandLineSwitches.SetSwitchError(duplicateSwitchErrorMessage, unquotedCommandLineArg);
                }
            }
            else
            {
                commandLineSwitches.SetUnexpectedParametersError(unquotedCommandLineArg);
            }
        }

        private static void GatherResponseFileSwitch(string unquotedCommandLineArg, CommandLineSwitches commandLineSwitches)
        {
            try
            {
                string path = unquotedCommandLineArg.Substring(1);
                if (path.Length == 0)
                {
                    commandLineSwitches.SetSwitchError("MissingResponseFileError", unquotedCommandLineArg);
                }
                else if (!File.Exists(path))
                {
                    commandLineSwitches.SetParameterError("ResponseFileNotFoundError", unquotedCommandLineArg);
                }
                else
                {
                    path = Path.GetFullPath(path);
                    bool flag = false;
                    foreach (string str2 in includedResponseFiles)
                    {
                        if (string.Compare(path, str2, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            commandLineSwitches.SetParameterError("RepeatedResponseFileError", unquotedCommandLineArg);
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        ArrayList list;
                        includedResponseFiles.Add(path);
                        using (StreamReader reader = new StreamReader(path, Encoding.Default))
                        {
                            list = new ArrayList();
                            while (reader.Peek() != -1)
                            {
                                string name = reader.ReadLine().TrimStart(new char[0]);
                                if (!name.StartsWith("#", StringComparison.Ordinal))
                                {
                                    list.AddRange(QuotingUtilities.SplitUnquoted(Environment.ExpandEnvironmentVariables(name), new char[0]));
                                }
                            }
                        }
                        GatherCommandLineSwitches(list, commandLineSwitches);
                    }
                }
            }
            catch (NotSupportedException exception)
            {
                commandLineSwitches.SetParameterError("ReadResponseFileError", unquotedCommandLineArg, exception);
            }
            catch (SecurityException exception2)
            {
                commandLineSwitches.SetParameterError("ReadResponseFileError", unquotedCommandLineArg, exception2);
            }
            catch (UnauthorizedAccessException exception3)
            {
                commandLineSwitches.SetParameterError("ReadResponseFileError", unquotedCommandLineArg, exception3);
            }
            catch (IOException exception4)
            {
                commandLineSwitches.SetParameterError("ReadResponseFileError", unquotedCommandLineArg, exception4);
            }
        }

        private static string GetFriendlyCounterType(PerformanceCounterType type, string name)
        {
            if (name.IndexOf("bytes", StringComparison.OrdinalIgnoreCase) != -1)
            {
                return "bytes";
            }
            if (name.IndexOf("threads", StringComparison.OrdinalIgnoreCase) != -1)
            {
                return "threads";
            }
            switch (type)
            {
                case PerformanceCounterType.NumberOfItems64:
                case PerformanceCounterType.CounterDelta32:
                case PerformanceCounterType.CounterDelta64:
                case PerformanceCounterType.NumberOfItemsHEX32:
                case PerformanceCounterType.NumberOfItemsHEX64:
                case PerformanceCounterType.NumberOfItems32:
                case PerformanceCounterType.SampleCounter:
                case PerformanceCounterType.CountPerTimeInterval32:
                case PerformanceCounterType.CountPerTimeInterval64:
                case PerformanceCounterType.CounterTimer:
                case PerformanceCounterType.RateOfCountsPerSecond32:
                case PerformanceCounterType.RateOfCountsPerSecond64:
                case PerformanceCounterType.CounterMultiTimer:
                case PerformanceCounterType.CounterTimerInverse:
                case PerformanceCounterType.CounterMultiTimerInverse:
                case PerformanceCounterType.AverageCount64:
                    return "#";

                case PerformanceCounterType.RawFraction:
                case PerformanceCounterType.CounterMultiTimer100Ns:
                case PerformanceCounterType.SampleFraction:
                case PerformanceCounterType.CounterMultiTimer100NsInverse:
                    return "$(";

                case PerformanceCounterType.Timer100NsInverse:
                case PerformanceCounterType.Timer100Ns:
                    return "100ns";

                case PerformanceCounterType.AverageTimer32:
                case PerformanceCounterType.ElapsedTime:
                    return "s";
            }
            return "?";
        }

        private static void HandleConfigurationException(Exception ex)
        {
            StringBuilder builder = new StringBuilder();
            Exception innerException = ex;
            do
            {
                string str = innerException.Message.TrimEnd(new char[0]);
                builder.Append(str);
                if (str[str.Length - 1] != '.')
                {
                    builder.Append(".");
                }
                builder.Append(" ");
                innerException = innerException.InnerException;
            }
            while (innerException != null);
            Console.WriteLine(Microsoft.Build.Shared.ResourceUtilities.FormatResourceString("InvalidConfigurationFile", new object[] { builder.ToString() }));
            initialized = false;
        }

        internal static void Initialize()
        {
        }

        //[MTAThread]
        //public static int Main()
        //{
        //    if (Environment.GetEnvironmentVariable("MSBUILDDUMPPROCESSCOUNTERS") == "1")
        //    {
        //        DumpCounters(true);
        //    }
        //    int num = (initialized && (Execute(Environment.CommandLine) == ExitType.Success)) ? 0 : 1;
        //    if (Environment.GetEnvironmentVariable("MSBUILDDUMPPROCESSCOUNTERS") == "1")
        //    {
        //        DumpCounters(false);
        //    }

        //    Console.ReadKey();

        //    return num;
        //}

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ExitType OldOMBuildProject(ExitType exitType, string projectFile, string[] targets, string toolsVersion, Dictionary<string, string> globalProperties, ILogger[] loggers, LoggerVerbosity verbosity, bool needToValidateProject, string schemaFile, int cpuCount)
        {
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("Using35Engine"));
            BuildPropertyGroup propertyBag = new BuildPropertyGroup();
            foreach (KeyValuePair<string, string> pair in globalProperties)
            {
                propertyBag.SetProperty(pair.Key, pair.Value);
            }
            if (!BuildProjectWithOldOM(projectFile, targets, toolsVersion, propertyBag, loggers, verbosity, null, needToValidateProject, schemaFile, cpuCount))
            {
                exitType = ExitType.BuildError;
            }
            return exitType;
        }

        private static Microsoft.Build.Logging.LoggerDescription ParseLoggingParameter(string parameter, string unquotedParameter, LoggerVerbosity verbosity)
        {
            string str;
            string loggerSwitchParameters = null;
            int num;
            string str5;
            ArrayList list = QuotingUtilities.SplitUnquoted(parameter, 2, true, false, out num, new char[] { ';' });
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrow((list.Count >= 1) && (list.Count <= 2), "SplitUnquoted() must return at least one string, and no more than two.");
            CommandLineSwitchException.VerifyThrow(((string) list[0]).Length > 0, "InvalidLoggerError", unquotedParameter);
            if (list.Count == 2)
            {
                loggerSwitchParameters = QuotingUtilities.Unquote((string) list[1]);
            }
            ArrayList list2 = QuotingUtilities.SplitUnquoted((string) list[0], 2, true, false, out num, new char[] { ',' });
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrow((list2.Count >= 1) && (list2.Count <= 2), "SplitUnquoted() must return at least one string, and no more than two.");
            if (list2.Count == 2)
            {
                str = QuotingUtilities.Unquote((string) list2[0]);
                str5 = QuotingUtilities.Unquote((string) list2[1]);
            }
            else
            {
                str = string.Empty;
                str5 = QuotingUtilities.Unquote((string) list2[0]);
            }
            CommandLineSwitchException.VerifyThrow(str5.Length > 0, "InvalidLoggerError", unquotedParameter);
            string loggerAssemblyName = null;
            string loggerAssemblyFile = null;
            if (string.Compare(str5, "Microsoft.Build.Engine", StringComparison.OrdinalIgnoreCase) == 0)
            {
                str5 = "Microsoft.Build.Engine,Version=4.0.0.0,Culture=neutral,PublicKeyToken=b03f5f7f11d50a3a";
            }
            if (File.Exists(str5))
            {
                loggerAssemblyFile = str5;
            }
            else
            {
                loggerAssemblyName = str5;
            }
            return new Microsoft.Build.Logging.LoggerDescription(str, loggerAssemblyName, loggerAssemblyFile, loggerSwitchParameters, verbosity);
        }

        private static bool ProcessCommandLineSwitches(CommandLineSwitches commandLineSwitches, ref string projectFile, ref string[] targets, ref string toolsVersion, ref Dictionary<string, string> globalProperties, ref ILogger[] loggers, ref LoggerVerbosity verbosity, ref List<DistributedLoggerRecord> distributedLoggerRecords, ref bool needToValidateProject, ref string schemaFile, ref int cpuCount, ref bool enableNodeReuse, ref TextWriter preprocessWriter, ref bool debugger, ref bool detailedSummary)
        {
            bool flag = false;
            if (!commandLineSwitches[CommandLineSwitches.ParameterlessSwitch.NoLogo] && !commandLineSwitches.IsParameterizedSwitchSet(CommandLineSwitches.ParameterizedSwitch.Preprocess))
            {
                DisplayCopyrightMessage();
            }
            if (commandLineSwitches[CommandLineSwitches.ParameterlessSwitch.Help])
            {
                ShowHelpMessage();
            }
            else if (commandLineSwitches.IsParameterizedSwitchSet(CommandLineSwitches.ParameterizedSwitch.NodeMode))
            {
                StartLocalNode(commandLineSwitches);
            }
            else
            {
                commandLineSwitches.ThrowErrors();
                if (commandLineSwitches[CommandLineSwitches.ParameterlessSwitch.Version])
                {
                    ShowVersion();
                }
                else
                {
                    projectFile = ProcessProjectSwitch(commandLineSwitches[CommandLineSwitches.ParameterizedSwitch.Project], commandLineSwitches[CommandLineSwitches.ParameterizedSwitch.IgnoreProjectExtensions], new Microsoft.Build.Shared.DirectoryGetFiles(Directory.GetFiles));
                    targets = ProcessTargetSwitch(commandLineSwitches[CommandLineSwitches.ParameterizedSwitch.Target]);
                    toolsVersion = ProcessToolsVersionSwitch(commandLineSwitches[CommandLineSwitches.ParameterizedSwitch.ToolsVersion]);
                    globalProperties = ProcessPropertySwitch(commandLineSwitches[CommandLineSwitches.ParameterizedSwitch.Property]);
                    cpuCount = ProcessMaxCPUCountSwitch(commandLineSwitches[CommandLineSwitches.ParameterizedSwitch.MaxCPUCount]);
                    enableNodeReuse = ProcessNodeReuseSwitch(commandLineSwitches[CommandLineSwitches.ParameterizedSwitch.NodeReuse]);
                    preprocessWriter = null;
                    if (commandLineSwitches.IsParameterizedSwitchSet(CommandLineSwitches.ParameterizedSwitch.Preprocess))
                    {
                        preprocessWriter = ProcessPreprocessSwitch(commandLineSwitches[CommandLineSwitches.ParameterizedSwitch.Preprocess]);
                    }
                    debugger = commandLineSwitches.IsParameterlessSwitchSet(CommandLineSwitches.ParameterlessSwitch.Debugger);
                    detailedSummary = commandLineSwitches.IsParameterlessSwitchSet(CommandLineSwitches.ParameterlessSwitch.DetailedSummary);
                    string[][] fileLoggerParameters = commandLineSwitches.GetFileLoggerParameters();
                    loggers = ProcessLoggingSwitches(commandLineSwitches[CommandLineSwitches.ParameterizedSwitch.Logger], commandLineSwitches[CommandLineSwitches.ParameterizedSwitch.DistributedLogger], commandLineSwitches[CommandLineSwitches.ParameterizedSwitch.Verbosity], commandLineSwitches[CommandLineSwitches.ParameterlessSwitch.NoConsoleLogger], commandLineSwitches[CommandLineSwitches.ParameterlessSwitch.DistributedFileLogger], commandLineSwitches[CommandLineSwitches.ParameterizedSwitch.FileLoggerParameters], commandLineSwitches[CommandLineSwitches.ParameterizedSwitch.ConsoleLoggerParameters], fileLoggerParameters, out distributedLoggerRecords, out verbosity, ref detailedSummary, cpuCount);
                    if (usingSwitchesFromAutoResponseFile && (LoggerVerbosity.Diagnostic == verbosity))
                    {
                        Console.WriteLine(Microsoft.Build.Shared.ResourceUtilities.FormatResourceString("PickedUpSwitchesFromAutoResponse", new object[] { "MSBuild.rsp" }));
                    }
                    needToValidateProject = commandLineSwitches.IsParameterizedSwitchSet(CommandLineSwitches.ParameterizedSwitch.Validate);
                    schemaFile = ProcessValidateSwitch(commandLineSwitches[CommandLineSwitches.ParameterizedSwitch.Validate]);
                    flag = true;
                }
            }
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrow(!flag || (projectFile != null), "We should have a project file if we're going to build.");
            return flag;
        }

        internal static void ProcessConsoleLoggerSwitch(bool noConsoleLogger, string[] consoleLoggerParameters, List<DistributedLoggerRecord> distributedLoggerRecords, LoggerVerbosity verbosity, int cpuCount, ArrayList loggers)
        {
            if (!noConsoleLogger)
            {
                Microsoft.Build.Logging.ConsoleLogger logger = new Microsoft.Build.Logging.ConsoleLogger(verbosity);
                string anyPrefixingParameter = "SHOWPROJECTFILE=TRUE;";
                if ((consoleLoggerParameters != null) && (consoleLoggerParameters.Length > 0))
                {
                    anyPrefixingParameter = AggregateParameters(anyPrefixingParameter, consoleLoggerParameters);
                }
                if (cpuCount == 1)
                {
                    logger.Parameters = "ENABLEMPLOGGING;" + anyPrefixingParameter;
                    loggers.Add(logger);
                }
                else
                {
                    logger.Parameters = anyPrefixingParameter;
                    DistributedLoggerRecord item = CreateForwardingLoggerRecord(logger, anyPrefixingParameter, verbosity);
                    distributedLoggerRecords.Add(item);
                }
            }
        }

        internal static void ProcessDistributedFileLogger(bool distributedFileLogger, string[] fileLoggerParameters, List<DistributedLoggerRecord> distributedLoggerRecords, ArrayList loggers, int cpuCount)
        {
            if (distributedFileLogger)
            {
                string parameters = string.Empty;
                if ((fileLoggerParameters != null) && (fileLoggerParameters.Length > 0))
                {
                    parameters = AggregateParameters(null, fileLoggerParameters);
                }
                string parameter = ExtractAnyLoggerParameter(parameters, new string[] { "logfile" });
                string str3 = ExtractAnyParameterValue(parameter);
                try
                {
                    if (!string.IsNullOrEmpty(str3) && !Path.IsPathRooted(str3))
                    {
                        parameters = parameters.Replace(parameter, "logFile=" + Path.Combine(Environment.CurrentDirectory, str3));
                    }
                }
                catch (Exception exception)
                {
                    if (Microsoft.Build.Shared.ExceptionHandling.NotExpectedException(exception))
                    {
                        throw;
                    }
                    throw new LoggerException(exception.Message, exception);
                }
                if (string.IsNullOrEmpty(str3))
                {
                    if (!string.IsNullOrEmpty(parameters) && !parameters.EndsWith(";", StringComparison.OrdinalIgnoreCase))
                    {
                        parameters = parameters + ";";
                    }
                    parameters = parameters + "logFile=" + Path.Combine(Environment.CurrentDirectory, "msbuild.log");
                }
                Assembly assembly = Assembly.GetAssembly(typeof(ProjectCollection));
                string loggerClassName = "Microsoft.Build.Logging.DistributedFileLogger";
                string fullName = assembly.GetName().FullName;
                Microsoft.Build.Logging.LoggerDescription forwardingLoggerDescription = new Microsoft.Build.Logging.LoggerDescription(loggerClassName, fullName, null, parameters, LoggerVerbosity.Detailed);
                DistributedLoggerRecord item = new DistributedLoggerRecord(null, forwardingLoggerDescription);
                distributedLoggerRecords.Add(item);
            }
        }

        private static List<DistributedLoggerRecord> ProcessDistributedLoggerSwitch(string[] parameters, LoggerVerbosity verbosity)
        {
            List<DistributedLoggerRecord> list = new List<DistributedLoggerRecord>();
            foreach (string str in parameters)
            {
                int num;
                ArrayList list2 = QuotingUtilities.SplitUnquoted(str, 2, true, false, out num, new char[] { '*' });
                Microsoft.Build.Shared.ErrorUtilities.VerifyThrow((list2.Count >= 1) && (list2.Count <= 2), "SplitUnquoted() must return at least one string, and no more than two.");
                string unquotedParameter = QuotingUtilities.Unquote((string) list2[0]);
                Microsoft.Build.Logging.LoggerDescription loggerDescription = ParseLoggingParameter((string) list2[0], unquotedParameter, verbosity);
                ILogger centralLogger = CreateAndConfigureLogger(loggerDescription, verbosity, unquotedParameter);
                Microsoft.Build.Logging.LoggerDescription forwardingLoggerDescription = loggerDescription;
                if (list2.Count > 1)
                {
                    unquotedParameter = QuotingUtilities.Unquote((string) list2[1]);
                    forwardingLoggerDescription = ParseLoggingParameter((string) list2[1], unquotedParameter, verbosity);
                }
                DistributedLoggerRecord item = new DistributedLoggerRecord(centralLogger, forwardingLoggerDescription);
                list.Add(item);
            }
            return list;
        }

        private static void ProcessFileLoggers(string[][] groupedFileLoggerParameters, List<DistributedLoggerRecord> distributedLoggerRecords, LoggerVerbosity verbosity, int cpuCount, ArrayList loggers)
        {
            for (int i = 0; i < groupedFileLoggerParameters.Length; i++)
            {
                if (groupedFileLoggerParameters[i] != null)
                {
                    string anyPrefixingParameter = "SHOWPROJECTFILE=TRUE;";
                    if (i == 0)
                    {
                        anyPrefixingParameter = anyPrefixingParameter + "logfile=msbuild.log;";
                    }
                    else
                    {
                        object obj2 = anyPrefixingParameter;
                        anyPrefixingParameter = string.Concat(new object[] { obj2, "logfile=msbuild", i, ".log;" });
                    }
                    if (groupedFileLoggerParameters[i].Length > 0)
                    {
                        anyPrefixingParameter = AggregateParameters(anyPrefixingParameter, groupedFileLoggerParameters[i]);
                    }
                    Microsoft.Build.Logging.FileLogger logger = new Microsoft.Build.Logging.FileLogger();
                    LoggerVerbosity detailed = LoggerVerbosity.Detailed;
                    logger.Verbosity = detailed;
                    if (cpuCount == 1)
                    {
                        logger.Parameters = "ENABLEMPLOGGING;" + anyPrefixingParameter;
                        loggers.Add(logger);
                    }
                    else
                    {
                        logger.Parameters = anyPrefixingParameter;
                        DistributedLoggerRecord item = CreateForwardingLoggerRecord(logger, anyPrefixingParameter, detailed);
                        distributedLoggerRecords.Add(item);
                    }
                }
            }
        }

        private static ArrayList ProcessLoggerSwitch(string[] parameters, LoggerVerbosity verbosity)
        {
            ArrayList list = new ArrayList();
            foreach (string str in parameters)
            {
                string unquotedParameter = QuotingUtilities.Unquote(str);
                Microsoft.Build.Logging.LoggerDescription loggerDescription = ParseLoggingParameter(str, unquotedParameter, verbosity);
                list.Add(CreateAndConfigureLogger(loggerDescription, verbosity, unquotedParameter));
            }
            return list;
        }

        private static ILogger[] ProcessLoggingSwitches(string[] loggerSwitchParameters, string[] distributedLoggerSwitchParameters, string[] verbositySwitchParameters, bool noConsoleLogger, bool distributedFileLogger, string[] fileLoggerParameters, string[] consoleLoggerParameters, string[][] groupedFileLoggerParameters, out List<DistributedLoggerRecord> distributedLoggerRecords, out LoggerVerbosity verbosity, ref bool detailedSummary, int cpuCount)
        {
            verbosity = LoggerVerbosity.Normal;
            if (verbositySwitchParameters.Length > 0)
            {
                verbosity = ProcessVerbositySwitch(verbositySwitchParameters[verbositySwitchParameters.Length - 1]);
            }
            ArrayList loggers = ProcessLoggerSwitch(loggerSwitchParameters, verbosity);
            distributedLoggerRecords = ProcessDistributedLoggerSwitch(distributedLoggerSwitchParameters, verbosity);
            ProcessConsoleLoggerSwitch(noConsoleLogger, consoleLoggerParameters, distributedLoggerRecords, verbosity, cpuCount, loggers);
            ProcessDistributedFileLogger(distributedFileLogger, fileLoggerParameters, distributedLoggerRecords, loggers, cpuCount);
            ProcessFileLoggers(groupedFileLoggerParameters, distributedLoggerRecords, verbosity, cpuCount, loggers);
            if (verbosity == LoggerVerbosity.Diagnostic)
            {
                detailedSummary = true;
            }
            return (ILogger[]) loggers.ToArray(typeof(ILogger));
        }

        internal static int ProcessMaxCPUCountSwitch(string[] parameters)
        {
            int num = 1;
            if (parameters.Length > 0)
            {
                try
                {
                    num = int.Parse(parameters[parameters.Length - 1], CultureInfo.InvariantCulture);
                }
                catch (FormatException exception)
                {
                    CommandLineSwitchException.Throw("InvalidMaxCPUCountValue", parameters[parameters.Length - 1], new string[] { exception.Message });
                }
                catch (OverflowException exception2)
                {
                    CommandLineSwitchException.Throw("InvalidMaxCPUCountValue", parameters[parameters.Length - 1], new string[] { exception2.Message });
                }
                CommandLineSwitchException.VerifyThrow((num > 0) && (num < 0x40), "InvalidMaxCPUCountValueOutsideRange", parameters[parameters.Length - 1]);
            }
            return num;
        }

        internal static bool ProcessNodeReuseSwitch(string[] parameters)
        {
            bool flag = true;
            if (parameters.Length > 0)
            {
                try
                {
                    flag = bool.Parse(parameters[parameters.Length - 1]);
                }
                catch (FormatException exception)
                {
                    CommandLineSwitchException.Throw("InvalidNodeReuseValue", parameters[parameters.Length - 1], new string[] { exception.Message });
                }
                catch (ArgumentNullException exception2)
                {
                    CommandLineSwitchException.Throw("InvalidNodeReuseValue", parameters[parameters.Length - 1], new string[] { exception2.Message });
                }
            }
            return flag;
        }

        internal static TextWriter ProcessPreprocessSwitch(string[] parameters)
        {
            TextWriter @out = Console.Out;
            if (parameters.Length > 0)
            {
                try
                {
                    @out = new StreamWriter(parameters[parameters.Length - 1]);
                }
                catch (Exception exception)
                {
                    if (Microsoft.Build.Shared.ExceptionHandling.NotExpectedException(exception))
                    {
                        throw;
                    }
                    CommandLineSwitchException.Throw("InvalidPreprocessPath", parameters[parameters.Length - 1], new string[] { exception.Message });
                }
            }
            return @out;
        }

        internal static string ProcessProjectSwitch(string[] parameters, string[] projectsExtensionsToIgnore, Microsoft.Build.Shared.DirectoryGetFiles getFiles)
        {
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrow(parameters.Length <= 1, "It should not be possible to specify more than 1 project at a time.");
            if (parameters.Length == 0)
            {
                string[] potentialProjectOrSolutionFiles = getFiles(".", "*.*proj");
                string[] strArray2 = getFiles(".", "*.sln");
                Dictionary<string, object> extensionsToIgnoreDictionary = ValidateExtensions(projectsExtensionsToIgnore);
                if (extensionsToIgnoreDictionary.Count > 0)
                {
                    if ((potentialProjectOrSolutionFiles != null) && (potentialProjectOrSolutionFiles.Length > 0))
                    {
                        potentialProjectOrSolutionFiles = RemoveFilesWithExtensionsToIgnore(potentialProjectOrSolutionFiles, extensionsToIgnoreDictionary);
                    }
                    if ((strArray2 != null) && (strArray2.Length > 0))
                    {
                        strArray2 = RemoveFilesWithExtensionsToIgnore(strArray2, extensionsToIgnoreDictionary);
                    }
                }
                if ((potentialProjectOrSolutionFiles.Length == 1) && (strArray2.Length == 1))
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(strArray2[0]);
                    string strB = Path.GetFileNameWithoutExtension(potentialProjectOrSolutionFiles[0]);
                    InitializationException.VerifyThrow(string.Compare(fileNameWithoutExtension, strB, StringComparison.OrdinalIgnoreCase) == 0, "AmbiguousProjectError");
                }
                else if (strArray2.Length > 1)
                {
                    InitializationException.VerifyThrow(false, "AmbiguousProjectError");
                }
                else if (potentialProjectOrSolutionFiles.Length > 1)
                {
                    bool flag = true;
                    if (potentialProjectOrSolutionFiles.Length == 2)
                    {
                        string extension = Path.GetExtension(potentialProjectOrSolutionFiles[0]);
                        string str5 = Path.GetExtension(potentialProjectOrSolutionFiles[1]);
                        if (string.Compare(extension, str5, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            if (string.Compare(extension, ".proj", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                potentialProjectOrSolutionFiles = new string[] { potentialProjectOrSolutionFiles[0] };
                                flag = false;
                            }
                            else if (string.Compare(str5, ".proj", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                potentialProjectOrSolutionFiles = new string[] { potentialProjectOrSolutionFiles[1] };
                                flag = false;
                            }
                        }
                    }
                    InitializationException.VerifyThrow(!flag, "AmbiguousProjectError");
                }
                else if ((potentialProjectOrSolutionFiles.Length == 0) && (strArray2.Length == 0))
                {
                    InitializationException.VerifyThrow(false, "MissingProjectError");
                }
                return ((strArray2.Length == 1) ? strArray2[0] : potentialProjectOrSolutionFiles[0]);
            }
            InitializationException.VerifyThrow(File.Exists(parameters[0]), "ProjectNotFoundError", parameters[0]);
            return parameters[0];
        }

        internal static Dictionary<string, string> ProcessPropertySwitch(string[] parameters)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string str in parameters)
            {
                string[] strArray = str.Split(propertyValueSeparator, 2);
                CommandLineSwitchException.VerifyThrow((strArray[0].Length > 0) && (strArray.Length == 2), "InvalidPropertyError", str);
                dictionary[strArray[0]] = strArray[1];
            }
            return dictionary;
        }

        private static string[] ProcessTargetSwitch(string[] parameters)
        {
            return parameters;
        }

        private static string ProcessToolsVersionSwitch(string[] parameters)
        {
            if (parameters.Length > 0)
            {
                return parameters[parameters.Length - 1];
            }
            return null;
        }

        private static string ProcessValidateSwitch(string[] parameters)
        {
            string str = null;
            foreach (string str2 in parameters)
            {
                InitializationException.VerifyThrow(str == null, "MultipleSchemasError", str2);
                InitializationException.VerifyThrow(File.Exists(str2), "SchemaNotFoundError", str2);
                str = Path.Combine(Directory.GetCurrentDirectory(), str2);
            }
            return str;
        }

        internal static LoggerVerbosity ProcessVerbositySwitch(string value)
        {
            LoggerVerbosity normal = LoggerVerbosity.Normal;
            if (string.Equals(value, "q", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "quiet", StringComparison.OrdinalIgnoreCase))
            {
                return LoggerVerbosity.Quiet;
            }
            if (string.Equals(value, "m", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "minimal", StringComparison.OrdinalIgnoreCase))
            {
                return LoggerVerbosity.Minimal;
            }
            if (string.Equals(value, "n", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "normal", StringComparison.OrdinalIgnoreCase))
            {
                return LoggerVerbosity.Normal;
            }
            if (string.Equals(value, "d", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "detailed", StringComparison.OrdinalIgnoreCase))
            {
                return LoggerVerbosity.Detailed;
            }
            if (string.Equals(value, "diag", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "diagnostic", StringComparison.OrdinalIgnoreCase))
            {
                return LoggerVerbosity.Diagnostic;
            }
            CommandLineSwitchException.Throw("InvalidVerbosityError", value);
            return normal;
        }

        private static string[] RemoveFilesWithExtensionsToIgnore(string[] potentialProjectOrSolutionFiles, Dictionary<string, object> extensionsToIgnoreDictionary)
        {
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrow((potentialProjectOrSolutionFiles != null) && (potentialProjectOrSolutionFiles.Length > 0), "There should be some potential project or solution files");
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrow((extensionsToIgnoreDictionary != null) && (extensionsToIgnoreDictionary.Count > 0), "There should be some extensions to Ignore");
            List<string> list = new List<string>();
            foreach (string str in potentialProjectOrSolutionFiles)
            {
                string extension = Path.GetExtension(str);
                if (!extensionsToIgnoreDictionary.ContainsKey(extension))
                {
                    list.Add(str);
                }
            }
            return list.ToArray();
        }

        private static void ResetBuildState()
        {
            includedResponseFiles = new ArrayList();
            usingSwitchesFromAutoResponseFile = false;
        }

        internal static void SetConsoleUI()
        {
            Thread currentThread = Thread.CurrentThread;
            currentThread.CurrentUICulture = CultureInfo.CurrentUICulture.GetConsoleFallbackUICulture();
            int codePage = Console.OutputEncoding.CodePage;
            if (((codePage != 0xfde9) && (codePage != currentThread.CurrentUICulture.TextInfo.OEMCodePage)) && (codePage != currentThread.CurrentUICulture.TextInfo.ANSICodePage))
            {
                currentThread.CurrentUICulture = new CultureInfo("en-US");
            }
        }

        private static void ShowHelpMessage()
        {
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_1_Syntax"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_2_Description"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_3_SwitchesHeader"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_9_TargetSwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_10_PropertySwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_17_MaximumCPUSwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_23_ToolsVersionSwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_12_VerbositySwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_13_ConsoleLoggerParametersSwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_14_NoConsoleLoggerSwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_20_FileLoggerSwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_22_FileLoggerParametersSwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_18_DistributedLoggerSwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_21_DistributedFileLoggerSwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_11_LoggerSwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_15_ValidateSwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_19_IgnoreProjectExtensionsSwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_24_NodeReuse"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_25_PreprocessSwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_26_DetailedSummarySwitch"));
            if (CommandLineSwitches.IsParameterlessSwitch("debug"))
            {
                Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_27_DebuggerSwitch"));
            }
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_7_ResponseFile"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_8_NoAutoResponseSwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_5_NoLogoSwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_6_VersionSwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_4_HelpSwitch"));
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpMessage_16_Examples"));
        }

        private static void ShowHelpPrompt()
        {
            Console.WriteLine(Microsoft.Build.Shared.AssemblyResources.GetString("HelpPrompt"));
        }

        private static void ShowVersion()
        {
            Console.Write(ProjectCollection.Version.ToString());
        }

        private static void StartLocalNode(CommandLineSwitches commandLineSwitches)
        {
            string[] strArray = commandLineSwitches[CommandLineSwitches.ParameterizedSwitch.NodeMode];
            int nodeNumber = 0;
            if (strArray.Length > 0)
            {
                try
                {
                    nodeNumber = int.Parse(strArray[0], CultureInfo.InvariantCulture);
                }
                catch (FormatException exception)
                {
                    CommandLineSwitchException.Throw("InvalidNodeNumberValue", strArray[0], new string[] { exception.Message });
                }
                catch (OverflowException exception2)
                {
                    CommandLineSwitchException.Throw("InvalidNodeNumberValue", strArray[0], new string[] { exception2.Message });
                }
                CommandLineSwitchException.VerifyThrow(nodeNumber >= 0, "InvalidNodeNumberValueIsNegative", strArray[0]);
            }
            if (!commandLineSwitches[CommandLineSwitches.ParameterlessSwitch.OldOM])
            {
                bool flag = true;
                while (flag)
                {
                    Exception exception3;
                    NodeEngineShutdownReason reason = new OutOfProcNode().Run(out exception3);
                    if (reason == NodeEngineShutdownReason.Error)
                    {
                        throw exception3;
                    }
                    if (reason != NodeEngineShutdownReason.BuildCompleteReuse)
                    {
                        flag = false;
                    }
                }
            }
            else
            {
                StartLocalNodeOldOM(nodeNumber);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void StartLocalNodeOldOM(int nodeNumber)
        {
            LocalNode.StartLocalNodeServer(nodeNumber);
        }

        private static void ThrowInvalidToolsVersionInitializationException(IEnumerable<Microsoft.Build.Evaluation.Toolset> toolsets, string toolsVersion)
        {
            string str = string.Empty;
            foreach (Microsoft.Build.Evaluation.Toolset toolset in toolsets)
            {
                str = str + "\"" + toolset.ToolsVersion + "\", ";
            }
            if (str.Length > 0)
            {
                str = str.Substring(0, str.Length - 2);
            }
            string str2 = Microsoft.Build.Shared.ResourceUtilities.FormatResourceString("UnrecognizedToolsVersion", new object[] { toolsVersion, str });
            InitializationException.Throw(Microsoft.Build.Shared.ResourceUtilities.FormatResourceString("InvalidToolsVersionError", new object[] { str2 }), toolsVersion);
        }

        private static Dictionary<string, object> ValidateExtensions(string[] projectsExtensionsToIgnore)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if ((projectsExtensionsToIgnore != null) && (projectsExtensionsToIgnore.Length > 0))
            {
                string extension = null;
                foreach (string str2 in projectsExtensionsToIgnore)
                {
                    try
                    {
                        extension = Path.GetExtension(str2);
                    }
                    catch (ArgumentException)
                    {
                        InitializationException.Throw("InvalidExtensionToIgnore", str2, null, false);
                    }
                    InitializationException.VerifyThrow(!string.IsNullOrEmpty(extension), "InvalidExtensionToIgnore", str2);
                    InitializationException.VerifyThrow(extension.Length >= 2, "InvalidExtensionToIgnore", str2);
                    if (string.Compare(extension, str2, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        InitializationException.Throw("InvalidExtensionToIgnore", str2, null, false);
                    }
                    if (str2.IndexOfAny(wildcards) > -1)
                    {
                        InitializationException.Throw("InvalidExtensionToIgnore", str2, null, false);
                    }
                    if (!dictionary.ContainsKey(str2))
                    {
                        dictionary.Add(str2, null);
                    }
                }
            }
            return dictionary;
        }

        private static void VerifyThrowSupportedOS()
        {
            if (((Environment.OSVersion.Platform == PlatformID.Win32S) || (Environment.OSVersion.Platform == PlatformID.Win32Windows)) || ((Environment.OSVersion.Platform == PlatformID.WinCE) || ((Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major <= 4))))
            {
                InitializationException.VerifyThrow(false, "UnsupportedOS");
            }
        }

        public enum ExitType
        {
            Success,
            SwitchError,
            InitializationError,
            BuildError,
            LoggerAbort,
            LoggerFailure,
            Unexpected
        }
    }
}

