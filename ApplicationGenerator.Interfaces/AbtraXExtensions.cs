// file:	AbtraXExtensions.cs
//
// summary:	Implements the abtra x coordinate extensions class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX;
using System.Linq.Expressions;
using System.Xml.Serialization;
using System.IO;
using AbstraX.ServerInterfaces;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using Utils;
using System.Reflection;
using CodePlex.XPathParser;
using AbstraX.XPathBuilder;
using System.Xml.Linq;
using System.Xml.XPath;
using AbstraX.Models;
using System.Configuration;
using AbstraX.Models.Interfaces;
using AbstraX.Generators;
using DynamicTemplateEngine;
using System.ComponentModel.DataAnnotations;
using AbstraX.DataAnnotations;
using System.Data.Entity;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AbstraX.Builds;
using AbstraX.Resources;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace AbstraX
{
    /// <summary>   A bit-field of flags for specifying debug Information show options. </summary>
    ///
    /// <remarks>   Ken, 11/1/2020. </remarks>

    [Flags]
    public enum DebugInfoShowOptions
    {
        /// <summary>   A binary constant representing the show Identifier flag. </summary>
        ShowID = 1,
        /// <summary>   A binary constant representing the show condensed Identifier flag. </summary>
        ShowCondensedID = 2,
        /// <summary>   A binary constant representing the show name flag. </summary>
        ShowName = 4,
        /// <summary>   A binary constant representing the show datatype flag. </summary>
        ShowDatatype = 8,
        /// <summary>   A binary constant representing the show description flag. </summary>
        ShowDescription = 16,
        /// <summary>   A binary constant representing the show modifiers flag. </summary>
        ShowModifiers = 32,
        /// <summary>   A binary constant representing the show in comment mode flag. </summary>
        ShowInCommentMode = 64
    }

    /// <summary>   An abstra x coordinate extensions. </summary>
    ///
    /// <remarks>   Ken, 11/1/2020. </remarks>

    public static class AbstraXExtensions
    {
        /// <summary>   Gets or sets options for controlling the debug information show. </summary>
        ///
        /// <value> Options that control the debug information show. </value>

        public static DebugInfoShowOptions DebugInfoShowOptions { get; set; }

        /// <summary>   Gets or sets the debug information line terminator. </summary>
        ///
        /// <value> The debug information line terminator. </value>

        public static string DebugInfoLineTerminator { get; set; }

        /// <summary>   Gets or sets the debug comment initiator. </summary>
        ///
        /// <value> The debug comment initiator. </value>

        public static string DebugCommentInitiator { get; set; }

        /// <summary>   Gets or sets the debug indent. </summary>
        ///
        /// <value> The debug indent. </value>

        public static int DebugIndent { get; set; }

        /// <summary>   Static constructor. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>

        static AbstraXExtensions()
        {
            AbstraXExtensions.DebugInfoShowOptions = DebugInfoShowOptions.ShowID | DebugInfoShowOptions.ShowName | DebugInfoShowOptions.ShowDatatype | DebugInfoShowOptions.ShowDescription | AbstraX.DebugInfoShowOptions.ShowModifiers;
            AbstraXExtensions.DebugInfoLineTerminator = "\r\n";
            AbstraXExtensions.DebugCommentInitiator = "/// ";

            AbstraXExtensions.ShowCondensedID = false;
        }

        /// <summary>   Gets the prefix. </summary>
        ///
        /// <value> The prefix. </value>
        
        public static async Task<byte[]> GetKey()
        {
            return await Task.FromResult(new byte[] { 0xAB });
        }

        /// <summary>
        /// Logs request start.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 5/5/2021. </remarks>

        public static void LogRequestStart()
        {
        }

        /// <summary>
        /// Logs request end.
        /// </summary>
        ///
        /// <remarks>   CloudIDEaaS, 5/5/2021. </remarks>

        public static void LogRequestEnd()
        {
        }

        /// <summary>   Logs the captcha. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 5/9/2021. </remarks>

        public static int LogCaptchaStart()
        {
            return 1234;
        }

        private static string Prefix
        {
            get
            {
                var prefix = new string('\t', AbstraXExtensions.DebugIndent);

                if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowInCommentMode))
                {
                    prefix += AbstraXExtensions.DebugCommentInitiator;
                }

                return prefix;
            }
        }

        /// <summary>   Gets the double suffix. </summary>
        ///
        /// <value> The double suffix. </value>

        private static string DoubleSuffix
        {
            get
            {
                var suffix = AbstraXExtensions.DebugInfoLineTerminator;

                if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowInCommentMode))
                {
                    suffix += new string('\t', AbstraXExtensions.DebugIndent) + AbstraXExtensions.DebugCommentInitiator;
                }

                suffix += AbstraXExtensions.DebugInfoLineTerminator;

                return suffix;
            }
        }

        /// <summary>   Gets the new line. </summary>
        ///
        /// <value> The new line. </value>

        private static string NewLine
        {
            get
            {
                var suffix = string.Empty;

                if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowInCommentMode))
                {
                    suffix += new string('\t', AbstraXExtensions.DebugIndent) + AbstraXExtensions.DebugCommentInitiator;
                }

                suffix += AbstraXExtensions.DebugInfoLineTerminator;

                return suffix;
            }
        }

        /// <summary>   Gets the suffix. </summary>
        ///
        /// <value> The suffix. </value>

        private static string Suffix
        {
            get
            {
                var suffix = AbstraXExtensions.DebugInfoLineTerminator;

                return suffix;
            }
        }

        public static IHydraAppsAdminServicesClientConfig Decrypt(this IHydraAppsAdminServicesClientConfig hydraAppsAdminServicesClientConfig)
        {
            return null;
        }


        /// <summary>   An Exception extension method that handles the exit exception. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/21/2021. </remarks>
        ///
        /// <param name="exception">        The exception to act on. </param>
        /// <param name="workingDirectory"> Pathname of the working directory. </param>
        /// <param name="runAsAutomated">   True if run as automated. </param>

        public static void HandleExitException(this Exception exception, string workingDirectory, bool runAsAutomated)
        {
            var process = Process.GetCurrentProcess();
            var thread = Thread.CurrentThread;
            var debugLogDirectory = new DirectoryInfo(Path.Combine(workingDirectory, string.Format(@"Logs\{0}", DateTime.Now.ToSortableDateTimeText())));
            var debugLogFile = new FileInfo(Path.Combine(debugLogDirectory.FullName, string.Format("{0}-Debug.log", process.ProcessName)));
            var dumpLogFile = new FileInfo(Path.Combine(debugLogDirectory.FullName, string.Format("{0}.dmp", process.ProcessName)));
            var assembly = Assembly.GetEntryAssembly();
            var attributes = assembly.GetAttributes();
            var analyzerClient = new CrashAnalyzerClient();
            string message;

            if (!debugLogDirectory.Exists)
            {
                debugLogFile.Directory.Create();
            }

            using (var logWriter = System.IO.File.AppendText(debugLogFile.FullName))
            {
                logWriter.Write("\r\nLog Entry : ");
                logWriter.WriteLine($"{ DateTime.Now.ToLongTimeString() } { DateTime.Now.ToLongDateString() }");
                logWriter.WriteLine("  :");
                logWriter.WriteLine($"  :{ exception.ToString() }");

                if (exception is ReflectionTypeLoadException reflectionTypeLoadException)
                {
                    logWriter.Write("\r\nLoader exceptions : ");

                    foreach (var loaderException in reflectionTypeLoadException.LoaderExceptions)
                    {
                        logWriter.WriteLine($"  :{ loaderException.ToString() }");
                    }
                }

                if (exception.InnerException != null)
                {
                    logWriter.Write("\r\nInner exception : ");
                    logWriter.WriteLine($"  :{ exception.InnerException.ToString() }");
                }

                logWriter.WriteLine("-------------------------------");
            }

            message = exception.Message;

            analyzerClient.RunAnalysis(process.Id, thread.ManagedThreadId, DebugUtils.GetCurrentThreadId(), dumpLogFile, runAsAutomated);

            if (!runAsAutomated)
            {
                MessageBox.Show($"Hydra has run into an unexpected error.  A crash analysis will be performed. Error message: { message }", attributes.Product);
            }

            analyzerClient.Wait();

            if (runAsAutomated)
            {
                Environment.Exit(1);
            }
        }

        /// <summary>   Debug attach. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/7/2021. </remarks>
        ///
        /// <param name="writeToConsole">   True to write to console. </param>

        [DebuggerHidden()]
        public static void DebugAttach(bool writeToConsole)
        {
            var devIde = GetDevIDE();
            var processes = Process.GetProcessesByName(devIde.ProcessName);

            if (processes.Length > 0)
            {
                devIde.DebugAttach(processes, writeToConsole);
            }
        }

        /// <summary>   Executes the services operation. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/11/2022. </remarks>
        ///
        /// <param name="workspaceName"> Full pathname of the service project file. </param>
        /// <param name="projectName">  Name of the project. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        public static bool LaunchServices(string workspaceName, string projectName)
        {
            var devIde = GetDevIDE();
            var processes = Process.GetProcessesByName(devIde.ProcessName);

            foreach (var vsProcess in processes)
            {
                var hwndVisualStudio = vsProcess.MainWindowHandle;
                var text = ControlExtensions.GetWindowText(hwndVisualStudio);

                if (devIde.WindowTextMatches(workspaceName, text))
                {
                    devIde.DebugStart(workspaceName, projectName);

                    return true;
                }
            }

            return false;
        }

        /// <summary>   Gets resource defaults. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/11/2021. </remarks>
        ///
        /// <returns>   The resource defaults. </returns>

        public static ResourceDefaults GetResourceDefaults()
        {
            var assemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Dimensions dimensions;
            ResourceDefaults resourceDefaults;

            using (var reader = File.OpenRead(Path.Combine(assemblyLocation, @"Defaults\Dimensions.json")))
            {
                dimensions = JsonExtensions.ReadJson<Dimensions>(File.ReadAllText(Path.Combine(assemblyLocation, @"Defaults\Dimensions.json")));
            }

            resourceDefaults = new ResourceDefaults
            {
                Dimensions = dimensions,
                AboutBannerFile = Path.Combine(assemblyLocation, @"Defaults\AboutBanner.jpg"),
                AboutBanner = Bitmap.FromFile(Path.Combine(assemblyLocation, @"Defaults\AboutBanner.jpg")),
                LogoFile = Path.Combine(assemblyLocation, @"Defaults\Logo.jpg"),
                Logo = Bitmap.FromFile(Path.Combine(assemblyLocation, @"Defaults\Logo.jpg")),
                SplashScreenFile = Path.Combine(assemblyLocation, @"Defaults\SplashScreen.jpg"),
                SplashScreen = Bitmap.FromFile(Path.Combine(assemblyLocation, @"Defaults\SplashScreen.jpg"))
            };

            return resourceDefaults;
        }

        /// <summary>   A DynamicTemplateEngineHost extension method that gets template file. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/4/2021. </remarks>
        ///
        /// <param name="host"> The host to act on. </param>
        /// <param name="type"> The type to act on. </param>
        /// <param name="generatorIdentifier"></param>
        ///
        /// ### <typeparam name="T">    Generic type parameter. </typeparam>
        ///
        /// ### <returns>   The template file. </returns>

        public static void SetGenerator(this ITemplateEngineHost host, Type type, string generatorIdentifier)
        {
            if (host is DynamicTemplateEngineHost dynamicHost)
            {
                var assembly = type.Assembly;
                var generatorTemplateAttribute = type.GetCustomAttributes<GeneratorTemplateAttribute>().Single(a => a.GeneratorIdentifier == generatorIdentifier);
                var relativePath = generatorTemplateAttribute.TemplateRelativePath;
                var path = Path.Combine(Path.GetDirectoryName(assembly.Location), relativePath);

                dynamicHost.TemplateFile = path;
            }
        }

        /// <summary>   A DirectoryInfo extension method that gets comparison score. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 2/1/2021. </remarks>
        ///
        /// <param name="directory1">   The directory1 to act on. </param>
        /// <param name="directory2">   The second directory. </param>
        ///
        /// <returns>   The comparison score. </returns>

        public static float GetComparisonScore(this DirectoryInfo directory1, DirectoryInfo directory2)
        {
            var exceptions = directory1.GetComparisonExceptions(directory2);

            if (exceptions.Count > 0)
            {
                return 0;
            }

            return 1;
        }


        /// <summary>   Gets application targets builder. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/9/2021. </remarks>
        ///
        /// <param name="generatorHandlerType"> Type of the generator handler. </param>
        ///
        /// <returns>   The application targets builder. </returns>

        public static IAppTargetsBuilder GetAppTargetsBuilder(string generatorHandlerType)
        {
            var generatorHandler = AbstraXExtensions.GetGeneratorHandler(generatorHandlerType);
            var assembly = generatorHandler.GetType().Assembly;
            var type = assembly.GetTypes().Where(t => t.Implements<IAppTargetsBuilder>()).Where(t => t.HasCustomAttribute<SupportsGeneratorHandlerAttribute>()).SingleOrDefault(t => t.GetCustomAttribute<SupportsGeneratorHandlerAttribute>().GeneratorHandlerType == generatorHandlerType);
            var builder = type.CreateInstance<IAppTargetsBuilder>();

            return builder;
        }

        /// <summary>   Gets the plafort builders in this collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/9/2021. </remarks>
        ///
        /// <param name="targetsBuilder">       Type of the generator handler. </param>
        /// <param name="generatorHandlerType"> Type of the generator handler. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the plafort builders in this
        /// collection.
        /// </returns>

        public static IEnumerable<IPlatformBuilder> GetPlaformBuilders(this IAppTargetsBuilder targetsBuilder, string generatorHandlerType)
        {
            var assembly = targetsBuilder.GetType().Assembly;
            var types = assembly.GetTypes().Where(t => t.Implements<IPlatformBuilder>()).Where(t => t.HasCustomAttribute<SupportsGeneratorHandlerAttribute>()).Where(t => t.GetCustomAttribute<SupportsGeneratorHandlerAttribute>().GeneratorHandlerType == generatorHandlerType);
            var builders = types.Select(t => t.CreateInstance<IPlatformBuilder>(targetsBuilder));

            return builders;
        }


        /// <summary>   Gets the overrides in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="engine">           The engine to act on. </param>
        /// <param name="useOverrides">     True to use overrides. </param>
        /// <param name="workingDirectory"> (Optional) Pathname of the working directory. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the overrides in this collection.
        /// </returns>

        public static IEnumerable<KeyValuePair<string, IGeneratorOverrides>> GetOverrides(this IGeneratorEngine engine, bool useOverrides, string workingDirectory = null)
        {
            return GetOverrides(useOverrides, workingDirectory);
        }

        /// <summary>   An IGeneratorEngine extension method that adds a predictive analytics. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/31/2021. </remarks>
        ///
        /// <param name="generatorConfiguration">   The engine to act on. </param>
        /// <param name="entitiesAssembly">         The entities assembly. </param>
        /// <param name="analyticsReporter">        The analytics reporter. </param>

        public static void AddPredictiveAnalytics(this IGeneratorConfiguration generatorConfiguration, Assembly entitiesAssembly, IAnalyticsReporter analyticsReporter)
        {
            foreach (var type in entitiesAssembly.GetTypes().Where(t => t.HasCustomAttribute<MetadataTypeAttribute>()))
            {
                var metadataTypeAttribute = type.GetCustomAttribute<MetadataTypeAttribute>();
                var metadataClassType = metadataTypeAttribute.MetadataClassType;
                var definitionKind = DefinitionKind.Unknown;

                if (type.InheritsFrom(typeof(DbContext)))
                {
                    definitionKind = DefinitionKind.StaticContainer;
                }

                if (type.InheritsFrom(typeof(DbContext)))
                {
                    definitionKind = DefinitionKind.StaticContainer;
                }

                if (metadataClassType.HasCustomAttribute<UIAttribute>())
                {
                    generatorConfiguration.AddPredictiveAnalytics(metadataClassType, analyticsReporter, definitionKind);
                }
            }
        }

        /// <summary>   An IGeneratorEngine extension method that adds a predictive analytics. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 4/1/2021. </remarks>
        ///
        /// <param name="generatorConfiguration">   The engine to act on. </param>
        /// <param name="memberInfo">               Information describing the member. </param>
        /// <param name="analyticsReporter">        The analytics reporter. </param>
        /// <param name="definitionKind">           (Optional) The definition kind. </param>

        public static void AddPredictiveAnalytics(this IGeneratorConfiguration generatorConfiguration, MemberInfo memberInfo, IAnalyticsReporter analyticsReporter, DefinitionKind definitionKind = DefinitionKind.Unknown)
        {
            var analytics = generatorConfiguration.PredictiveAnalytics;
            var attributes = memberInfo.GetCustomAttributes().Where(a => a is UIAttribute).Cast<UIAttribute>();
            float totalWeightRanking;

            foreach (var uiAttribute in attributes)
            {
                var analytic = new PredictiveAnalytic(memberInfo, uiAttribute, 1);

                switch (uiAttribute.UILoadKind)
                {
                    case UILoadKind.HomePage:
                        analytic.WeightRanking += 1;
                        break;
                    case UILoadKind.MainPage:
                        analytic.WeightRanking += 2;
                        break;
                    case UILoadKind.Default:
                        break;
                }

                if (memberInfo is Type type)
                {
                    if (definitionKind == DefinitionKind.Unknown && type.IsClass)
                    {
                        definitionKind = DefinitionKind.Class;
                    }
                }
                else if (memberInfo is PropertyInfo property)
                {
                    if (definitionKind == DefinitionKind.Unknown)
                    {
                        if (property.PropertyType.IsScalar())
                        {
                            definitionKind = DefinitionKind.SimpleProperty;
                        }
                        else
                        {
                            definitionKind = DefinitionKind.ComplexProperty;
                        }
                    }
                }

                analytic.DefinitionKind = definitionKind;
                analytic.Reporter = analyticsReporter;

                analytics.AddToDictionaryListCreateIfNotExist(memberInfo, analytic);
            }

            if (memberInfo is Type type2)
            {
                foreach (var property in type2.GetProperties())
                {
                    generatorConfiguration.AddPredictiveAnalytics(property, analyticsReporter);
                }
            }

            totalWeightRanking = analytics.Values.Sum(v => v.Sum(a => a.WeightRanking));

            foreach (var analytic in analytics.Values.SelectMany(v => v))
            {
                var weight = analytic.WeightRanking / totalWeightRanking;

                analytic.WeightPercentage = weight;
            }
        }

        /// <summary>   Gets the overrides in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="parseResult">      The handler to act on. </param>
        /// <param name="useOverrides">     True to use overrides. </param>
        /// <param name="workingDirectory"> (Optional) Pathname of the working directory. </param>
        /// <param name="argumentsKind">    (Optional) The arguments kind. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the overrides in this collection.
        /// </returns>

        public static IEnumerable<KeyValuePair<string, IGeneratorOverrides>> GetOverrides(this ParseResultBase parseResult, bool useOverrides, string workingDirectory = null, string argumentsKind = null)
        {
            return GetOverrides(useOverrides, workingDirectory, argumentsKind);
        }

        /// <summary>   Gets the overrides in this collection. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/4/2021. </remarks>
        ///
        /// <param name="assembly">         The assembly. </param>
        /// <param name="useOverrides">     True to use overrides. </param>
        /// <param name="workingDirectory"> (Optional) Pathname of the working directory. </param>
        /// <param name="argumentsKind">    (Optional) The arguments kind. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the overrides in this collection.
        /// </returns>

        public static IEnumerable<KeyValuePair<string, IGeneratorOverrides>> GetOverrides(this Assembly assembly, bool useOverrides, string workingDirectory = null, string argumentsKind = null)
        {
            return GetOverrides(useOverrides, workingDirectory, argumentsKind);
        }

        /// <summary>   Gets the overrides in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the overrides in this collection.
        /// </returns>

        public static IEnumerable<KeyValuePair<string, IGeneratorOverrides>> GetOverrides(bool useOverridesAssembly, string workingDirectory = null, string argumentsKind = null)
        {
            if (useOverridesAssembly)
            {
                var overrides = (List<Override>)ConfigurationManager.GetSection("generatorOverridesSection");

                if (overrides != null)
                {
                    foreach (var _override in overrides)
                    {
                        var overrideAssembly = _override.Assembly;
                        var types = Assembly.Load(overrideAssembly.AssemblyName).GetTypes().Where(t => t.Implements<IGeneratorOverrides>());

                        foreach (var type in types)
                        {
                            var generatorOverride = (IGeneratorOverrides)Activator.CreateInstance(type);

                            yield return new KeyValuePair<string, IGeneratorOverrides>(_override.ArgumentsKind, generatorOverride);
                        }
                    }
                }
            }
            else
            {
                var appDomainName = string.Format("{0}[{1}]", "GeneratorOverridesKeyValuePair", workingDirectory);
                KeyValuePair<string, IGeneratorOverrides>? keyValuePair;

                if (argumentsKind == null)
                {
                    argumentsKind = "GenerateFromConfig";
                }

                keyValuePair = (KeyValuePair<string, IGeneratorOverrides>?) AppDomain.CurrentDomain.GetData(appDomainName);

                if (keyValuePair == null)
                {
                    var packageCachePath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\hydra\cache");
                    var generatorHandlerAssembly = GetGeneratorHandlerAssembly("Default");
                    var type = generatorHandlerAssembly.GetTypes().Where(t => !t.IsInterface && t.Implements<IGeneratorOverrides>()).Single();
                    var generatorOverride = (IGeneratorOverrides)Activator.CreateInstance(type);
                    var arguments = generatorOverride.GetHandlerArguments(packageCachePath, argumentsKind, workingDirectory);
                    string kind;

                    if (arguments != null)
                    {
                        kind = (string)arguments["ArgumentsKind"];
                        keyValuePair = new KeyValuePair<string, IGeneratorOverrides>(kind, generatorOverride);
                    }
                    else
                    {
                        yield break;
                    }

                    AppDomain.CurrentDomain.SetData(string.Format("{0}[{1}]", "GeneratorOverridesKeyValuePair", workingDirectory), keyValuePair);
                }

                yield return keyValuePair.Value;
            }
        }

        /// <summary>   Gets the generatorHandlers in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="engine">   The engine to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the generatorHandlers in this collection.
        /// </returns>

        public static IEnumerable<KeyValuePair<string, IGeneratorHandler>> GetGeneratorHandlers(this IGeneratorEngine engine)
        {
            return GetGeneratorHandlers();
        }

        /// <summary>   Gets the generatorHandlers in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="handler">  The handler to act on. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the generatorHandlers in this collection.
        /// </returns>

        public static IEnumerable<KeyValuePair<string, IGeneratorHandler>> GetGeneratorHandlers(this IGeneratorHandler handler)
        {
            return GetGeneratorHandlers();
        }

        /// <summary>   Gets the generatorHandlers in this collection. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the generatorHandlers in this collection.
        /// </returns>

        public static IEnumerable<KeyValuePair<string, IGeneratorHandler>> GetGeneratorHandlers()
        {
            var generatorHandlers = (List<GeneratorHandler>)ConfigurationManager.GetSection("generatorHandlersSection");

            if (generatorHandlers != null)
            {
                foreach (var _generatorHandler in generatorHandlers)
                {
                    var generatorHandlerAssembly = _generatorHandler.Assembly;
                    var types = Assembly.Load(generatorHandlerAssembly.AssemblyName).GetTypes().Where(t => t.Implements<IGeneratorHandler>());

                    foreach (var type in types)
                    {
                        var generatorHandler = (IGeneratorHandler)Activator.CreateInstance(type);

                        yield return new KeyValuePair<string, IGeneratorHandler>(_generatorHandler.HandlerType, generatorHandler);
                    }
                }
            }
        }

        /// <summary>   Handler, called when the get generator. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/17/2020. </remarks>
        ///
        /// <param name="handlerType">  Type of the handler. </param>
        ///
        /// <returns>   The generator handler. </returns>

        public static IGeneratorHandler GetGeneratorHandler(string handlerType)
        {
            var generatorHandlers = (List<GeneratorHandler>)ConfigurationManager.GetSection("generatorHandlersSection");

            if (generatorHandlers != null)
            {
                var _generatorHandler = generatorHandlers.Single(h => h.HandlerType == handlerType);
                var generatorHandlerAssembly = _generatorHandler.Assembly;
                var type = Assembly.Load(generatorHandlerAssembly.AssemblyName).GetTypes().Single(t => !t.IsInterface && t.Implements<IGeneratorHandler>());
                var generatorHandler = (IGeneratorHandler)Activator.CreateInstance(type);

                return generatorHandler;
            }

            return null;
        }

        /// <summary>   Gets application store. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 7/26/2021. </remarks>
        ///
        /// <param name="appStoreType"> Type of the application store. </param>
        ///
        /// <returns>   The application store. </returns>

        public static IAppStore GetAppStore(string appStoreType)
        {
            var appStores = (List<AppStore>)ConfigurationManager.GetSection("appStoresSection");

            if (appStores != null)
            {
                var _appStore = appStores.Single(s => s.AppStoreType == appStoreType);
                var appStoreAssembly = _appStore.Assembly;
                var type = Assembly.Load(appStoreAssembly.AssemblyName).GetTypes().Single(t => !t.IsInterface && t.Implements<IAppStore>());
                var appStore = (IAppStore)Activator.CreateInstance(type);

                return appStore;
            }

            return null;
        }

        /// <summary>   Gets development IDE. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 6/27/2022. </remarks>
        ///
        /// <param name="devToolType">  Type of the development tool. </param>
        ///
        /// <returns>   The development IDE. </returns>

        public static IDevIDEInstance GetDevIDE()
        {
            var devTools = (List<DevTool>)ConfigurationManager.GetSection("devToolsSection");

            if (devTools != null)
            {
                var _devTool = devTools.Single(s => s.DevToolType == "DevIDE");
                var appStoreAssembly = _devTool.Assembly;
                var type = Assembly.Load(appStoreAssembly.AssemblyName).GetTypes().Single(t => !t.IsInterface && t.Implements<IDevIDEInstance>());
                var devIDE = (IDevIDEInstance)Activator.CreateInstance(type);

                return devIDE;
            }

            return null;
        }

        /// <summary>   Gets generator handler assembly. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 3/14/2021. </remarks>
        ///
        /// <param name="handlerType">  Type of the handler. </param>
        ///
        /// <returns>   The generator handler assembly. </returns>

        public static Assembly GetGeneratorHandlerAssembly(string handlerType)
        {
            var generatorHandlers = (List<GeneratorHandler>)ConfigurationManager.GetSection("generatorHandlersSection");

            if (generatorHandlers != null)
            {
                var _generatorHandler = generatorHandlers.Single(h => h.HandlerType == handlerType);
                var generatorHandlerAssembly = _generatorHandler.Assembly;
                var assembly = Assembly.Load(generatorHandlerAssembly.AssemblyName);

                return assembly;
            }

            return null;
        }

        /// <summary>   Clears the indent. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>

        public static void ClearIndent()
        {
            AbstraXExtensions.DebugIndent = 0;
        }

        /// <summary>   An IEntityContainer extension method that gets full name. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/18/2020. </remarks>
        ///
        /// <param name="entityContainer">  The entityContainer to act on. </param>
        ///
        /// <returns>   The full name. </returns>

        public static string GetFullName(this IEntityContainer entityContainer)
        {
            return entityContainer.Namespace + "." + entityContainer.Name;
        }

        /// <summary>   Gets the models in this collection. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="assembly">         The assembly. </param>
        /// <param name="typesProvider">    The types provider. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the models in this collection.
        /// </returns>

        public static IEnumerable<AssemblyModelEntityProvider.Web.Entities.AssemblyModel> GetModels(this AssemblyProvider.Web.Entities.Assembly assembly, ITypesProvider typesProvider)
        {
            return new List<AssemblyModelEntityProvider.Web.Entities.AssemblyModel> { new AssemblyModelEntityProvider.Web.Entities.AssemblyModel(assembly, typesProvider) };
        }

        /// <summary>   An XElement extension method that gets a sequence. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="element">  The element to act on. </param>
        ///
        /// <returns>   The sequence. </returns>

        public static XElement GetSequence(this XElement element)
        {
            XNamespace xs = "http://www.w3.org/2001/XMLSchema";
            var nsmgr = new XmlNamespaceManager(new NameTable());
            XElement sequence;
            
            nsmgr.AddNamespace("xs", xs.NamespaceName);

            sequence = element.XPathSelectElement("xs:complexType/xs:sequence", nsmgr);

            if (sequence == null)
            {
                var complexTypeElement = element.GetComplexType();
                complexTypeElement.AddFirst(new XElement(xs + "sequence"));

                sequence = element.XPathSelectElement("xs:complexType/xs:sequence", nsmgr);
            }

            return sequence;
        }

        /// <summary>   An XElement extension method that searches for the first element. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="parentElement">    The parentElement to act on. </param>
        /// <param name="baseObject">       The baseObject to act on. </param>
        ///
        /// <returns>   The found element. </returns>

        public static XElement FindElement(this XElement parentElement, IBase baseObject)
        {
            XNamespace xs = "http://www.w3.org/2001/XMLSchema";
            var nsmgr = new XmlNamespaceManager(new NameTable());
            XElement element;

            nsmgr.AddNamespace("xs", xs.NamespaceName);

            element = parentElement.XPathSelectElement($"xs:element[@name='{ baseObject.Name }']", nsmgr);

            return element;
        }

        /// <summary>   An XElement extension method that searches for the first attribute. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="parentElement">    The parentElement to act on. </param>
        /// <param name="baseObject">       The baseObject to act on. </param>
        ///
        /// <returns>   The found attribute. </returns>

        public static XElement FindAttribute(this XElement parentElement, IBase baseObject)
        {
            XNamespace xs = "http://www.w3.org/2001/XMLSchema";
            var nsmgr = new XmlNamespaceManager(new NameTable());
            XElement element;

            nsmgr.AddNamespace("xs", xs.NamespaceName);

            element = parentElement.XPathSelectElement($"xs:attribute[@name='{ baseObject.Name }']", nsmgr);

            return element;
        }

        /// <summary>   An XElement extension method that gets complex type. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="element">  The element to act on. </param>
        ///
        /// <returns>   The complex type. </returns>

        public static XElement GetComplexType(this XElement element)
        {
            XNamespace xs = "http://www.w3.org/2001/XMLSchema";
            var nsmgr = new XmlNamespaceManager(new NameTable());
            XElement sequence;

            nsmgr.AddNamespace("xs", xs.NamespaceName);

            sequence = element.XPathSelectElement("xs:complexType", nsmgr);

            if (sequence == null)
            {
                element.Add(new XElement(xs + "complexType"));

                sequence = element.XPathSelectElement("xs:complexType", nsmgr);
            }

            return sequence;
        }

        /// <summary>   Gets or sets a value indicating whether the in comments mode is shown. </summary>
        ///
        /// <value> True if show in comments mode, false if not. </value>

        public static bool ShowInCommentsMode
        {
            get
            {
                return AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowInCommentMode);
            }

            set
            {
                if (value)
                {
                    AbstraXExtensions.DebugInfoShowOptions = EnumUtils.SetFlag<DebugInfoShowOptions>(AbstraXExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowInCommentMode);
                }
                else
                {
                    AbstraXExtensions.DebugInfoShowOptions = EnumUtils.RemoveFlag<DebugInfoShowOptions>(AbstraXExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowInCommentMode);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the condensed identifier is shown.
        /// </summary>
        ///
        /// <value> True if show condensed identifier, false if not. </value>

        public static bool ShowCondensedID
        {
            get
            {
                return AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowCondensedID);
            }

            set
            {
                if (value)
                {
                    AbstraXExtensions.DebugInfoShowOptions = EnumUtils.SetFlag<DebugInfoShowOptions>(AbstraXExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowCondensedID);
                    AbstraXExtensions.DebugInfoShowOptions = EnumUtils.RemoveFlag<DebugInfoShowOptions>(AbstraXExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowID);
                }
                else
                {
                    AbstraXExtensions.DebugInfoShowOptions = EnumUtils.SetFlag<DebugInfoShowOptions>(AbstraXExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowID);
                    AbstraXExtensions.DebugInfoShowOptions = EnumUtils.RemoveFlag<DebugInfoShowOptions>(AbstraXExtensions.DebugInfoShowOptions, DebugInfoShowOptions.ShowCondensedID);
                }
            }
        }

        /// <summary>   An IBase extension method that query if 'baseObject' is local. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="baseObject">   The baseObject to act on. </param>
        ///
        /// <returns>   True if local, false if not. </returns>

        public static bool IsLocal(this IBase baseObject)
        {
            if (baseObject is IElement)
            {
                var element = (IElement)baseObject;

                return element.Modifiers.HasFlag(Modifiers.IsLocal);
            }
            else if (baseObject is IAttribute)
            {
                var attribute = (IAttribute)baseObject;

                return attribute.Modifiers.HasFlag(Modifiers.IsLocal);
            }
            else if (baseObject is IOperation)
            {
                var operation = (IOperation)baseObject;

                return operation.Modifiers.HasFlag(Modifiers.IsLocal);
            }
            else
            {
                throw new Exception("IsLocal only applies to elements, attributes, and operations");
            }
        }

        /// <summary>
        /// An IAbstraXProviderService extension method that gets query method for identifier.
        /// </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="service">  The service to act on. </param>
        /// <param name="id">       The identifier. </param>
        ///
        /// <returns>   The query method for identifier. </returns>

        public static string GetQueryMethodForID(this IAbstraXProviderService service, string id)
        {
            var baseObject = service.GenerateByID(id);
            var method = service.GetType().GetMethods().Single(m =>
            {
                var returnType = m.ReturnType;
                var args = returnType.GetGenericArguments();

                if (returnType.Name == "IQueryable`1" && args.Length == 1 && args.First().Name == baseObject.GetType().Name && m.GetParameters().Length == 0)
                {
                    return true;
                }
                else if (returnType.Name == baseObject.GetType().Name)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            return method.Name;
        }

        /// <summary>   An IBase extension method that makes an identifier. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="baseObject">   The baseObject to act on. </param>
        /// <param name="predicate">    The predicate. </param>
        ///
        /// <returns>   A string. </returns>

        public static string MakeID(this BaseObject baseObject, string predicate)
        {
            var id = "/" + baseObject.GetType().Name + "[@" + predicate + "]";

            if (baseObject.Parent != null)
            {
                id = baseObject.Parent.ID + id;
            }

            id = baseObject.GetOverrideId(predicate, id);

            return id;
        }

        /// <summary>   An IBase extension method that gets condensed identifier. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="baseObject">   The baseObject to act on. </param>
        ///
        /// <returns>   The condensed identifier. </returns>

        public static string GetCondensedID(this IBase baseObject)
        {
            var pattern = @"/.+?\[\@.+?='(?<part>.+?)'\]";
            var regex = new Regex(pattern);

            if (regex.IsMatch(baseObject.ID))
            {
                var matches = regex.Matches(baseObject.ID);
                var builder = new StringBuilder();

                foreach (var match in matches.Cast<Match>())
                {
                    var group = match.Groups["part"];
                    builder.AppendWithLeadingIfLength("/", group.Value);
                }

                return builder.ToString();
            }

            return null;
        }

        /// <summary>   An IBase extension method that makes an identifier. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="baseObject">   The baseObject to act on. </param>
        /// <param name="typeName">     Name of the type. </param>
        /// <param name="predicate">    The predicate. </param>
        ///
        /// <returns>   A string. </returns>

        public static string MakeID(this IBase baseObject, string typeName, string predicate)
        {
            var id = "/" + typeName + "[@" + predicate + "]";

            if (baseObject.Parent != null)
            {
                id = baseObject.Parent.ID + id;
            }

            return id;
        }

        /// <summary>   A BaseType extension method that gets generic arguments. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="dataType"> The dataType to act on. </param>
        ///
        /// <returns>   An array of base type. </returns>

        public static BaseType[] GetGenericArguments(this BaseType dataType)
        {
            var type = dataType.UnderlyingType;
            var list = new List<BaseType>();

            foreach (var argType in type.GetGenericArguments())
            {
                if (dataType.SourceParent != null && dataType.SourceParent.FullyQualifiedName == argType.FullName)
                {
                    list.Add(new BaseType(argType, dataType, true));
                }
                else
                {
                    list.Add(new BaseType(argType, dataType));
                }
            }

            return list.ToArray();
        }

        /// <summary>   A BaseType extension method that gets base interfaces. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="dataType"> The dataType to act on. </param>
        ///
        /// <returns>   An array of base type. </returns>

        public static BaseType[] GetBaseInterfaces(this BaseType dataType)
        {
            var interfaces = new List<BaseType>();

            try
            {
                foreach (var _interface in dataType.UnderlyingType.GetInterfaces())
                {
                    if (dataType.SourceParent != null && dataType.SourceParent.FullyQualifiedName == _interface.FullName)
                    {
                        interfaces.Add(new BaseType(_interface, dataType, true));
                    }
                    else
                    {
                        interfaces.Add(new BaseType(_interface, dataType));
                    }
                }
            }
            catch (Exception ex)
            {
                return dataType.BaseDataType.GetBaseInterfaces();
            }

            return interfaces.ToArray();
        }

        /// <summary>   A BaseType extension method that implements. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="type">             The type to act on. </param>
        /// <param name="implementsType">   Type of the implements. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool Implements(this BaseType type, BaseType implementsType)
        {
            var baseType = type;

            if (!implementsType.IsInterface || type.IsInterface)
            {
                return false;
            }

            while (baseType != null)
            {
                foreach (var interfaceType in type.Interfaces)
                {
                    if (interfaceType.FullyQualifiedName == implementsType.FullyQualifiedName)
                    {
                        return true;
                    }
                    else if (interfaceType.BaseDataType != null && Implements(baseType, interfaceType.BaseDataType))
                    {
                        return true;
                    }
                }

                baseType = baseType.BaseDataType;
            }

            return false;
        }

        /// <summary>   A BaseType extension method that query if 'type' is same namespace. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="type">         The type to act on. </param>
        /// <param name="otherType">    Type of the other. </param>
        ///
        /// <returns>   True if same namespace, false if not. </returns>

        public static bool IsSameNamespace(this BaseType type, BaseType otherType)
        {
            var baseType = type.BaseDataType;

            if (type.Namespace == otherType.Namespace)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>   A Type extension method that inherits from. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="type">             The type to act on. </param>
        /// <param name="inheritsFromType"> Type of the inherits from. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool InheritsFrom(this BaseType type, BaseType inheritsFromType)
        {
            var baseType = type.BaseDataType;

            if (type.FullyQualifiedName == inheritsFromType.FullyQualifiedName)
            {
                return false;
            }

            while (baseType != null)
            {
                if (baseType.FullyQualifiedName == inheritsFromType.FullyQualifiedName)
                {
                    return true;
                }

                baseType = baseType.BaseDataType;
            }

            return false;
        }

        /// <summary>   A Type extension method that inherits from. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="type">             The type to act on. </param>
        /// <param name="inheritsFromType"> Type of the inherits from. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public static bool InheritsFrom(this Type type, BaseType inheritsFromType)
        {
            var baseType = type.BaseType;

            while (baseType != null)
            {
                if (baseType.FullName == inheritsFromType.FullyQualifiedName)
                {
                    return true;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }

        /// <summary>   A Type extension method that gets base data type. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="dataType"> The dataType to act on. </param>
        ///
        /// <returns>   The base data type. </returns>

        public static BaseType GetBaseDataType(this Type dataType)
        {
            var type = dataType.BaseType;

            if (type != null)
            {
                return new BaseType(type);
            }

            return null;
        }

        //public static string GetDebugInfo(this NavigationItem navigationItem)
        //{
        //    var builder = new StringBuilder();

        //    builder.AppendFormat("{0}------------------------------------ NavigationItem ------------------------------------{1}", AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
        //    builder.AppendFormat("{0}{1}{2}", AbstraXExtensions.Prefix, navigationItem.DebugInfo, AbstraXExtensions.Suffix);
        //    builder.AppendFormat("{0}CanRead: {1}{2}", AbstraXExtensions.Prefix, navigationItem.CanRead ? "true" : "false", AbstraXExtensions.Suffix);
        //    builder.AppendFormat("{0}CanWrite: {1}{2}", AbstraXExtensions.Prefix, navigationItem.CanWrite ? "true" : "false", AbstraXExtensions.Suffix);
        //    builder.AppendFormat("{0}{1}----------------------------------------------------------------------------------{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.Suffix);

        //    return builder.ToString();
        //}

        /// <summary>   An IBase extension method that gets debug information. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="baseObject">   The baseObject to act on. </param>
        /// <param name="additional">   (Optional) The additional. </param>
        ///
        /// <returns>   The debug information. </returns>

        public static string GetDebugInfo(this IBase baseObject, StringBuilder additional = null)
        {
            var builder = new StringBuilder();

            if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowID) || AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowCondensedID))
            {
                builder.AppendFormat("{0}ID={1}{2}", AbstraXExtensions.Prefix, baseObject.GetID(), AbstraXExtensions.Suffix);
            }

            if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowName))
            {
                builder.AppendFormat("{0}Name={1}{2}", AbstraXExtensions.Prefix, baseObject.Name, AbstraXExtensions.Suffix);
            }

            if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowDatatype))
            {
                builder.Append(baseObject.GetDataTypeInfo());
            }

            if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowDescription))
            {
                builder.Append(baseObject.GetDesignComments());
            }

            if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowModifiers))
            {
                builder.Append(baseObject.GetModifiersList());
            }

            builder.AppendFormat("{0}**********************************************{1}", AbstraXExtensions.Prefix, AbstraXExtensions.Suffix);

            if (additional != null)
            {
                builder.Append(additional.ToString());
            }

            return builder.ToString();
        }

        /// <summary>   An IBase extension method that gets an identifier. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="baseObject">   The baseObject to act on. </param>
        ///
        /// <returns>   The identifier. </returns>

        public static string GetID(this IBase baseObject)
        {
            if (AbstraXExtensions.DebugInfoShowOptions.HasFlag(DebugInfoShowOptions.ShowCondensedID))
            {
                var parser = new XPathParser<string>();
                var builder = new XPathStringBuilder();
                var id = string.Empty;

                parser.Parse(baseObject.ID, builder);

                id = string.Join("../", builder.PartQueue.OfType<XPathElement>().Select(e => e.Text));
                id += "[" + builder.PartQueue.OfType<XPathElement>().Last().Predicates.First().ToString() + "]";

                return id;
            }
            else
            {
                return baseObject.ID;
            }
        }

        /// <summary>   An IBase extension method that gets modifiers list. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="baseObject">   The baseObject to act on. </param>
        ///
        /// <returns>   The modifiers list. </returns>

        public static string GetModifiersList(this IBase baseObject)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("{0}{1}****************** Modifiers ******************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);

            foreach (var modifier in Enum<Modifiers>.GetValues())
            {
                if (modifier != Modifiers.Unknown)
                {
                    var name = Enum<Modifiers>.GetName(modifier);

                    builder.AppendFormat("{0}{1}={2}{3}", AbstraXExtensions.Prefix, name, baseObject.Modifiers.HasFlag(modifier) ? "true" : "false", AbstraXExtensions.Suffix);
                }
            }

            builder.Append(AbstraXExtensions.NewLine);

            return builder.ToString();
        }

        /// <summary>   An IBase extension method that gets design comments. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="baseObject">   The baseObject to act on. </param>
        ///
        /// <returns>   The design comments. </returns>

        public static string GetDesignComments(this IBase baseObject)
        {
            var builder = new StringBuilder();

            if (!string.IsNullOrEmpty(baseObject.DesignComments))
            {
                builder.AppendFormat("{0}{1}****************** Description ******************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
                builder.AppendFormat("{0}{1}{2}", AbstraXExtensions.Prefix, baseObject.DesignComments, AbstraXExtensions.DoubleSuffix);
            }

            return builder.ToString();
        }

        /// <summary>   An IBase extension method that gets data type information. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="baseType"> The baseType to act on. </param>
        ///
        /// <returns>   The data type information. </returns>

        public static string GetDataTypeInfo(this BaseType baseType)
        {
            var builder = new StringBuilder();

            if (baseType is ScalarType)
            {
                var scalarType = (ScalarType)baseType;

                if (scalarType != null)
                {
                    builder.AppendFormat("{0}{1}****************** DataType ******************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
                    builder.AppendFormat("{0}Type='{1}'{2}", AbstraXExtensions.Prefix, scalarType.Name, AbstraXExtensions.Suffix);
                    builder.AppendFormat("{0}TypeCode='{1}'{2}", AbstraXExtensions.Prefix, Enum.GetName(typeof(TypeCode), scalarType.TypeCode), AbstraXExtensions.DoubleSuffix);
                }
            }
            else
            {
                if (baseType != null)
                {
                    builder.AppendFormat("{0}{1}****************** DataType ******************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
                    builder.AppendFormat("{0}Type='{1}'{2}", AbstraXExtensions.Prefix, baseType.Name, AbstraXExtensions.Suffix);
                    builder.AppendFormat("{0}FQN='{1}'{2}", AbstraXExtensions.Prefix, baseType.FullyQualifiedName, AbstraXExtensions.Suffix);
                    builder.AppendFormat("{0}IsCollectionType='{1}'{2}", AbstraXExtensions.Prefix, baseType.IsCollectionType, AbstraXExtensions.DoubleSuffix);
                }
            }

            return builder.ToString();
        }

        /// <summary>   An IBase extension method that gets data type information. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="baseObject">   The baseObject to act on. </param>
        ///
        /// <returns>   The data type information. </returns>

        public static string GetDataTypeInfo(this IBase baseObject)
        {
            var builder = new StringBuilder();

            if (baseObject is IAttribute)
            {
                var attribute = (IAttribute)baseObject;

                if (attribute.DataType != null)
                {
                    builder.AppendFormat("{0}{1}****************** DataType ******************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
                    builder.AppendFormat("{0}Type='{1}'{2}", AbstraXExtensions.Prefix, attribute.DataType.Name, AbstraXExtensions.Suffix);
                    builder.AppendFormat("{0}TypeCode='{1}'{2}", AbstraXExtensions.Prefix, Enum.GetName(typeof(TypeCode), attribute.DataType.TypeCode), AbstraXExtensions.DoubleSuffix);
                }
            }
            else if (baseObject is IElement)
            {
                var element = (IElement)baseObject;

                if (element.DataType != null)
                {
                    builder.AppendFormat("{0}{1}****************** DataType ******************{2}", AbstraXExtensions.NewLine, AbstraXExtensions.Prefix, AbstraXExtensions.DoubleSuffix);
                    builder.AppendFormat("{0}Type='{1}'{2}", AbstraXExtensions.Prefix, element.DataType.Name, AbstraXExtensions.Suffix);
                    builder.AppendFormat("{0}FQN='{1}'{2}", AbstraXExtensions.Prefix, element.DataType.FullyQualifiedName, AbstraXExtensions.Suffix);
                    builder.AppendFormat("{0}IsCollectionType='{1}'{2}", AbstraXExtensions.Prefix, element.DataType.IsCollectionType, AbstraXExtensions.DoubleSuffix);
                }
            }

            return builder.ToString();
        }

        /// <summary>   Gets root identifier. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="id">   The identifier. </param>
        ///
        /// <returns>   The root identifier. </returns>

        public static string GetRootID(string id)
        {
            var regex = new Regex(@"/(?<providertype>\w+?)\[\@URL=\'(?<url>.*?)\'\]");

            if (regex.IsMatch(id))
            {
                var match = regex.Match(id);
                var rootID = "/" + match.Groups["providertype"].Value + "[@URL='" + match.Groups["url"].Value + "']";

                return rootID;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// An IAbstraXProviderService extension method that generates a by identifier.
        /// </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="service">  The service to act on. </param>
        /// <param name="id">       The identifier. </param>
        ///
        /// <returns>   The by identifier. </returns>

        public static IBase GenerateByID(this IAbstraXProviderService service, string id)
        {
            var queue = new Queue<string>();
            var parser = new XPathParser<string>();
            var builder = new XPathStringBuilder();

            parser.Parse(id, builder);

            if (builder.PartQueue.Count == 1)
            {
                var axisElement = builder.PartQueue.OfType<XPathElement>().Single();

                var method = service.GetType().GetMethods().Single(m => m.ReturnType.Name == axisElement.Text && m.GetParameters().Length == 0);

                service.LogGenerateByID(id, method);

                var rootObject = (IBase)method.Invoke(service, null);

                service.PostLogGenerateByID();

                return rootObject;
            }
            else
            {
                var axisElement = builder.PartQueue.OfType<XPathElement>().Last();

                var method = service.GetType().GetMethods().Single(m => m.ReturnType.Name == "IQueryable`1" && m.GetParameters().Length == 0 && m.ReturnType.GetGenericArguments().Any(a => a.Name == axisElement.Text));

                service.LogGenerateByID(id, method);

                var results = (IQueryable<IBase>)method.Invoke(service, null);

                service.PostLogGenerateByID();

                return results.Where(b => b.ID == id).Single();
            }
        }

        /// <summary>
        /// An IAbstraXProviderService extension method that generates a by identifier.
        /// </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="service">  The service to act on. </param>
        /// <param name="id">       The identifier. </param>
        ///
        /// <returns>   The by identifier. </returns>

        public static IQueryable<T> GenerateByID<T>(this IAbstraXProviderService service, string id)
        {
            var queue = new Queue<string>();
            var parser = new XPathParser<string>();
            var builder = new XPathStringBuilder();

            parser.Parse(id, builder);

            var axisElement = builder.PartQueue.OfType<XPathElement>().Last();

            var method = service.GetType().GetMethods().Single(m => m.ReturnType.Name == "IQueryable`1" && m.GetParameters().Length == 0 && m.ReturnType.GetGenericArguments().Any(a => a.Name == axisElement.Text));

            service.LogGenerateByID(id, method);

            var results = (IQueryable<IBase>)method.Invoke(service, null);

            service.PostLogGenerateByID();

            return results.Where(b => b.ID == id).Cast<T>();
        }
    }
}
