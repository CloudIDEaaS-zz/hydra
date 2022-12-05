// file:	StandardStreamService.cs
//
// summary:	Implements the standard stream service class

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.FolderStructure;
using Utils;
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
using Utils.Controls.ScreenCapture;
using MailSlot;
using System.Net;
using System.IO;
using File = AbstraX.FolderStructure.File;

namespace AbstraX
{
    /// <summary>   A service for accessing standard streams information. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 11/24/2020. </remarks>

    public class StandardStreamService : BaseStandardStreamService
    {
        /// <summary>   The generator handler. </summary>
        private IGeneratorHandler generatorHandler;
        /// <summary>   The web API service. </summary>
        private WebApiService webApiService;
        private bool logServiceMessages;
        private bool useDynamicTemplates;
        private string debugAssistantAddress;
        private List<string> debugPackageInstalls;
        private AlertInfo alertInfo;
        private MailslotClient mailslotClient;
        private bool runAsAutomated;
        private System.IO.DirectoryInfo logServiceMessagesDirectory;
        private EventDrivenStreamWriter eventDrivenStreamWriter;
        private OnWriteLineHandler onWriteLine;
        private DirectoryInfo logInstallsDirectory;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/24/2020. </remarks>
        ///
        /// <param name="webApiService">            The web API service. </param>
        /// <param name="mailslotClient">           . </param>
        /// <param name="mailslotException">        The mailslot exception. </param>
        /// <param name="logServiceMessages">       True to log service messages. </param>
        /// <param name="useDynamicTemplates">      True to use dynamic templates. </param>
        /// <param name="runAsAutomated">           True if run as automated. </param>
        /// <param name="debugAssistantAddress">    . </param>
        /// <param name="alertInfo">                Information describing the alert. </param>
        /// <param name="debugPackageInstallsList"> List of debug package installs. </param>

        public StandardStreamService(WebApiService webApiService, MailslotClient mailslotClient, Exception mailslotException, bool logServiceMessages, bool useDynamicTemplates, bool runAsAutomated, string debugAssistantAddress, AlertInfo alertInfo, List<string> debugPackageInstallsList) : base()
        {
            this.webApiService = webApiService;
            this.logServiceMessages = logServiceMessages;
            this.useDynamicTemplates = useDynamicTemplates;
            this.debugAssistantAddress = debugAssistantAddress;
            this.debugPackageInstalls = debugPackageInstallsList;
            this.alertInfo = alertInfo;
            this.mailslotClient = mailslotClient;
            this.runAsAutomated = runAsAutomated;

            if (logServiceMessages)
            {
                logServiceMessagesDirectory = AbstraX.LoggingExtensions.FindCurrentLogFolder("Messages");
            }

            logInstallsDirectory = AbstraX.LoggingExtensions.CreateLogFolder("Installs");

            base.JsonTextReadCallback = this.JsonTextRead;
        }

        /// <summary>   Callback, called when the JSON text read. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/15/2021. </remarks>
        ///
        /// <param name="jsonText"> The JSON text. </param>

        private void JsonTextRead(string jsonText)
        {
            if (logServiceMessages)
            {
                var fileName = DateTime.Now.ToSortableDateTimeText() + "_ApplicationGenerator_In.json";

                logServiceMessagesDirectory.WriteToLog(fileName, jsonText);
            }
        }

        /// <summary>   Callback, called when the JSON text write. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/15/2021. </remarks>
        ///
        /// <param name="jsonText"> The JSON text. </param>

        private void JsonTextWrite(string jsonText)
        {
            if (logServiceMessages)
            {
                var fileName = DateTime.Now.ToSortableDateTimeText() + "_ApplicationGenerator_Out.json";

                logServiceMessagesDirectory.WriteToLog(fileName, jsonText);
            }
        }

        /// <summary>   Handles the command described by commandPacket. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/24/2020. </remarks>
        ///
        /// <param name="commandPacket">    Message describing the command. </param>

        protected override void HandleCommand(CommandPacket commandPacket)
        {
            try
            {
                switch (commandPacket.Command)
                {
                    case ServerCommands.SEND_HYDRA_STATUS:
                        {
                            var status = (string) commandPacket.Arguments.Single(a => a.Key == "Status").Value;
                            var alertLevel = (string)commandPacket.Arguments.Single(a => a.Key == "AlertLevel").Value;

                            SendHydraStatus(status, alertLevel);
                        }

                        break;
                    case ServerCommands.LAUNCH_SERVICES:
                        {
                            FileInfo hydraJsonFile;
                            ConfigObject config;

                            onWriteLine = (sender, e) =>
                            {
                                commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, e.Output, true);
                                outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);

                            };

                            hydraJsonFile = new FileInfo(Path.Combine(currentWorkingDirectory, "hydra.json"));

                            if (hydraJsonFile.Exists)
                            {
                                string projectName;
                                
                                config = ConfigObject.Load(hydraJsonFile.FullName);
                                projectName = Path.GetFileNameWithoutExtension(config.ServicesProjectPath);

                                try
                                {
                                    if (AbstraXExtensions.LaunchServices(config.AppName, projectName))
                                    {
                                        commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "Launching services for " + projectName);
                                        outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                                    }
                                    else
                                    {
                                        commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "Unable to find Visual Studio services for " + projectName);
                                        outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "Error: " + ex.Message);
                                    outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                                }
                            }
                            else
                            {
                                commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "hydra.json does not exist at location '" + currentWorkingDirectory + "'");
                                outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                            }

                        }

                        break;
                    case ServerCommands.GENERATE:
                        {
                            var kind = commandPacket.Arguments.Single(a => a.Key == "Kind").Value;
                            var generatorKind = EnumUtils.GetValue<GeneratorKind>(((string)kind).ToTitleCase());
                            var args = Environment.GetCommandLineArgs();
                            var debugShimService = false;
                            string clientPipe = null;
                            LocalDataStoreSlot localDataStoreSlot;
                            var outputBuilder = new StringBuilder();
                            var parseResult = CommandLineParser.ParseArgs<ParseResult>(args, (result, arg) =>
                            {
                            },
                            (result, _switch, switchArg) =>
                            {
                                switch (_switch)
                                {
                                    case SwitchCommands.CLIENT_PIPE:
                                        clientPipe = switchArg;
                                        break;
                                    case SwitchCommands.DEBUG_SHIM_SERVICE:
                                        debugShimService = true;
                                        break;
                                }
                            });

                            if (generatorKind == GeneratorKind.App)
                            { 
                                var entitiesProjectPath = commandPacket.Arguments.Single(a => a.Key == "EntitiesProjectPath").Value;
                                var servicesProjectPath = commandPacket.Arguments.Single(a => a.Key == "ServicesProjectPath").Value;
                                var generatorHandlerType = (string) commandPacket.Arguments.Single(a => a.Key == "GeneratorHandlerType").Value;
                                var generatorPass = EnumUtils.GetValue<GeneratorPass>((string)commandPacket.Arguments.Single(a => a.Key == "GeneratorPass").Value);
                                var noFileCreation = bool.Parse(commandPacket.Arguments.Single(a => a.Key == "NoFileCreation").Value.ToString());
                                var projectFolderRoot = currentWorkingDirectory;
                                var parentProcess = Process.GetCurrentProcess().GetParent();
                                var packageCachePath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\hydra\cache");
                                var argumentsKind = GeneratorArgumentsKind.GenerateAppFromHydraCLI;
                                Dictionary<string, object> arguments;
                                KeyValuePair<string, IGeneratorOverrides> keyValuePair;
                                IGeneratorOverrides generatorOverrides;

                                eventDrivenStreamWriter = new EventDrivenStreamWriter();

                                onWriteLine = (sender, e) =>
                                {
                                    commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, e.Output, true);
                                    outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);

                                };

                                eventDrivenStreamWriter.OnWriteLine += onWriteLine;

                                keyValuePair = parseResult.GetOverrides(false, currentWorkingDirectory, argumentsKind).LastOrDefault();

                                generatorOverrides = keyValuePair.Value;

                                arguments = generatorOverrides.GetHandlerArguments(packageCachePath, argumentsKind, currentWorkingDirectory);
                                argumentsKind = (string)arguments["ArgumentsKind"];

                                generatorHandler = AbstraXExtensions.GetGeneratorHandler(generatorHandlerType);

                                webApiService.GeneratorHandler = generatorHandler;
                                generatorHandler.SuppressDebugOutput = true;

                                arguments.AddRange(new Dictionary<string, object>
                                {
                                    { "GeneratorKind", generatorKind },
                                    { "EntitiesProjectPath", entitiesProjectPath },
                                    { "ServicesProjectPath", servicesProjectPath },
                                    { "PackageCachePath", packageCachePath },
                                    { "ParentProcess", parentProcess },
                                    { "GeneratorMode", GeneratorMode.RedirectedConsole },
                                    { "ClientPipe", clientPipe },
                                    { "GeneratorOptions", new RedirectedGeneratorOptions(outputWriter, errorWriter, eventDrivenStreamWriter, generatorPass, noFileCreation, debugShimService, useDynamicTemplates, debugAssistantAddress) }
                                });

                                if (mailslotClient != null)
                                {
                                    arguments.Add("MailslotClient", mailslotClient);
                                }

                                if (debugPackageInstalls != null)
                                {
                                    arguments.Add("DebugPackageInstalls", debugPackageInstalls);
                                }

                                generatorHandler.Execute(arguments);
                            }
                            else if (generatorKind == GeneratorKind.BusinessModel)
                            {
                                var generatorHandlerType = (string)commandPacket.Arguments.Single(a => a.Key == "GeneratorHandlerType").Value;
                                var generatorPass = EnumUtils.GetValue<GeneratorPass>((string)commandPacket.Arguments.Single(a => a.Key == "GeneratorPass").Value);
                                var noFileCreation = bool.Parse(commandPacket.Arguments.Single(a => a.Key == "NoFileCreation").Value.ToString());
                                var templateFile = commandPacket.Arguments.Single(a => a.Key == "TemplateFile").Value.ToString();
                                var parentProcess = Process.GetCurrentProcess().GetParent();
                                var projectFolderRoot = currentWorkingDirectory;

                                eventDrivenStreamWriter = new EventDrivenStreamWriter();

                                onWriteLine = (sender, e) =>
                                {
                                    commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, e.Output, true);
                                    outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);

                                };

                                eventDrivenStreamWriter.OnWriteLine += onWriteLine;

                                generatorHandler = AbstraXExtensions.GetGeneratorHandler(generatorHandlerType);

                                webApiService.GeneratorHandler = generatorHandler;
                                generatorHandler.SuppressDebugOutput = true;

                                generatorHandler.Execute(new Dictionary<string, object>
                                {
                                    { "GeneratorKind", generatorKind },
                                    { "TemplateFile", templateFile },
                                    { "ParentProcess", parentProcess },
                                    { "GeneratorMode", GeneratorMode.RedirectedConsole },
                                    { "GeneratorOptions", new RedirectedGeneratorOptions(outputWriter, errorWriter, eventDrivenStreamWriter, generatorPass, noFileCreation, debugShimService, useDynamicTemplates, debugAssistantAddress) }
                                });
                            }
                            else if (generatorKind == GeneratorKind.Entities)
                            {
                                var generatorHandlerType = (string)commandPacket.Arguments.Single(a => a.Key == "GeneratorHandlerType").Value;
                                var generatorPass = EnumUtils.GetValue<GeneratorPass>((string)commandPacket.Arguments.Single(a => a.Key == "GeneratorPass").Value);
                                var noFileCreation = bool.Parse(commandPacket.Arguments.Single(a => a.Key == "NoFileCreation").Value.ToString());
                                var templateFile = commandPacket.Arguments.Single(a => a.Key == "TemplateFile").Value.ToString();
                                var jsonFile = commandPacket.Arguments.Single(a => a.Key == "JsonFile").Value.ToString();
                                var businessModelFile = commandPacket.Arguments.Single(a => a.Key == "BusinessModelFile").Value.ToString();
                                var entitiesProjectPath = commandPacket.Arguments.Single(a => a.Key == "EntitiesProjectPath").Value;
                                var parentProcess = Process.GetCurrentProcess().GetParent();
                                var projectFolderRoot = currentWorkingDirectory;

                                eventDrivenStreamWriter = new EventDrivenStreamWriter();

                                onWriteLine = (sender, e) =>
                                {
                                    commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, e.Output, true);
                                    outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);

                                };

                                eventDrivenStreamWriter.OnWriteLine += onWriteLine;

                                generatorHandler = AbstraXExtensions.GetGeneratorHandler(generatorHandlerType);

                                webApiService.GeneratorHandler = generatorHandler;
                                generatorHandler.SuppressDebugOutput = true;

                                generatorHandler.Execute(new Dictionary<string, object>
                                {
                                    { "GeneratorKind", generatorKind },
                                    { "TemplateFile", templateFile },
                                    { "JsonFile", jsonFile },
                                    { "BusinessModelFile", businessModelFile },
                                    { "EntitiesProjectPath", entitiesProjectPath },
                                    { "ParentProcess", parentProcess },
                                    { "GeneratorMode", GeneratorMode.RedirectedConsole },
                                    { "GeneratorOptions", new RedirectedGeneratorOptions(outputWriter, errorWriter, eventDrivenStreamWriter, generatorPass, noFileCreation, debugShimService, useDynamicTemplates, debugAssistantAddress) }
                                });
                            }
                            else if (generatorKind == GeneratorKind.Workspace)
                            {
                                var generatorHandlerType = (string)commandPacket.Arguments.Single(a => a.Key == "GeneratorHandlerType").Value;
                                var generatorPass = EnumUtils.GetValue<GeneratorPass>((string)commandPacket.Arguments.Single(a => a.Key == "GeneratorPass").Value);
                                var noFileCreation = bool.Parse(commandPacket.Arguments.Single(a => a.Key == "NoFileCreation").Value.ToString());
                                var appName = commandPacket.Arguments.Single(a => a.Key == "AppName").Value.ToString();
                                var appDescription = commandPacket.Arguments.Single(a => a.Key == "AppDescription").Value.ToString();
                                var organizationName = commandPacket.Arguments.Single(a => a.Key == "OrganizationName").Value.ToString();
                                var parentProcess = Process.GetCurrentProcess().GetParent();
                                var projectFolderRoot = currentWorkingDirectory;
                                var packageCachePath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\hydra\cache");
                                var argumentsKind = GeneratorArgumentsKind.GenerateWorkspaceFromHydraCLI;
                                KeyValuePair<string, IGeneratorOverrides> keyValuePair;
                                IGeneratorOverrides generatorOverrides;
                                Dictionary<string, object> arguments;
                                FileInfo hydraJsonFile;
                                ConfigObject config;

                                eventDrivenStreamWriter = new EventDrivenStreamWriter();

                                onWriteLine = (sender, e) =>
                                {
                                    commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, e.Output, true);
                                    outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);

                                };

                                eventDrivenStreamWriter.OnWriteLine += onWriteLine;

                                keyValuePair = parseResult.GetOverrides(false, currentWorkingDirectory, argumentsKind).LastOrDefault();

                                generatorOverrides = keyValuePair.Value;
                                
                                arguments = generatorOverrides.GetHandlerArguments(packageCachePath, argumentsKind, currentWorkingDirectory);
                                argumentsKind = (string) arguments["ArgumentsKind"];

                                hydraJsonFile = new FileInfo(Path.Combine(currentWorkingDirectory, "hydra.json"));

                                if (!hydraJsonFile.Exists)
                                {
                                    config = new ConfigObject();

                                    config.AppName = appName;
                                    config.AppDescription = appDescription;

                                    config.Save(hydraJsonFile.FullName);
                                }

                                generatorHandler = AbstraXExtensions.GetGeneratorHandler(generatorHandlerType);

                                webApiService.GeneratorHandler = generatorHandler;
                                generatorHandler.SuppressDebugOutput = true;

                                arguments.AddRange(new Dictionary<string, object>
                                {
                                    { "AppName", appName },
                                    { "AppDescription", appDescription },
                                    { "OrganizationName", organizationName },
                                    { "ParentProcess", parentProcess },
                                    { "GeneratorMode", GeneratorMode.RedirectedConsole },
                                    { "GeneratorOptions", new RedirectedGeneratorOptions(outputWriter, errorWriter, eventDrivenStreamWriter, generatorPass, noFileCreation, debugShimService, useDynamicTemplates, debugAssistantAddress) }
                                });

                                generatorHandler.Execute(arguments);
                            }

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "Generation complete!");
                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;

                    case ServerCommands.END_PROCESSING:
                        {
                            if (generatorHandler == null)
                            {
                                using (base.Lock())
                                {
                                    commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "End Processing");
                                    outputWriter.WriteJsonCommand(commandPacket, null, this.JsonTextWrite);

                                    this.HaltProcessing();

                                    webApiService.Dispose();

                                    if (mailslotClient != null)
                                    {
                                        ((IDisposable)mailslotClient).Dispose();
                                    }

                                    outputWriter.WriteLine(Environment.NewLine);
                                    outputWriter.Flush();
                                }
                            }
                            else
                            {
                                var config = generatorHandler.GeneratorConfiguration;

                                commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "End Processing");
                                outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);

                                this.HaltProcessing();

                                webApiService.Dispose();
                                config.StopServices();

                                generatorHandler.EndProcessing(config);

                                if (mailslotClient != null)
                                {
                                    ((IDisposable)mailslotClient).Dispose();
                                }

                                //outputWriter.WriteLine(Environment.NewLine);
                                //outputWriter.Flush();

                                Task.Run(() =>
                                {
                                    try
                                    {
                                        this.Stop();
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            this.ForceStop();
                                        }
                                        catch
                                        {
                                        }
                                    }
                                });
                            }
                        }

                        break;

                    case ServerCommands.TERMINATE:
                        {
                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "Terminated");
                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);

                            using (base.Lock())
                            {
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        this.Stop();
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            this.ForceStop();
                                        }
                                        catch
                                        {
                                        }
                                    }
                                });
                            }

                        }

                        break;

                    case ServerCommands.CONNECT:
                        {
                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "Connected successfully");
                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;

                    case ServerCommands.PING:
                        {
                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "Success");
                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;

                    case ServerCommands.GET_VERSION:
                        {
                            var version = Assembly.GetEntryAssembly().GetAttributes().Version;

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, version);
                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;

                    case ServerCommands.GET_FOLDER:
                        {
                            var relativePath = (string)commandPacket.Arguments.Single(a => a.Key == "relativePath").Value;
                            var config = generatorHandler.GeneratorConfiguration;
                            var folder = (Folder)config.FileSystem[relativePath];

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, folder);
                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;

                    case ServerCommands.GET_FILE:
                        {
                            var relativePath = (string)commandPacket.Arguments.Single(a => a.Key == "relativePath").Value;
                            var config = generatorHandler.GeneratorConfiguration;
                            var file = (File)config.FileSystem[relativePath];

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, file);
                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;

                    case ServerCommands.GET_FOLDERS:
                        {
                            var relativePath = (string)commandPacket.Arguments.Single(a => a.Key == "relativePath").Value;
                            var config = generatorHandler.GeneratorConfiguration;
                            var folder = (Folder)config.FileSystem[relativePath];

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, folder.Folders.ToArray());
                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;

                    case ServerCommands.GET_FILES:
                        {
                            var relativePath = (string)commandPacket.Arguments.Single(a => a.Key == "relativePath").Value;
                            var config = generatorHandler.GeneratorConfiguration;
                            var folder = (Folder)config.FileSystem[relativePath];

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, folder.Files.ToArray());
                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;

                    case ServerCommands.GET_PACKAGE_INSTALLS:
                        {
                            var config = generatorHandler.GeneratorConfiguration;
                            var packageInstalls = config.PackageInstalls;
                            var fileName = DateTime.Now.ToSortableDateTimeText() + "_ApplicationGenerator_PackageInstalls.txt";

                            // kn todo - comment
                            //packageInstalls = packageInstalls.Randomize().Take(5).ToList();
                            //packageInstalls = new List<string> { "@gracesnoh/tiny" };

                            logInstallsDirectory.WriteToLog(fileName, packageInstalls.ToMultiLineList());

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, (object)packageInstalls.ToArray());
                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;

                    case ServerCommands.GET_PACKAGE_DEV_INSTALLS:
                        {
                            var config = generatorHandler.GeneratorConfiguration;
                            var packageDevInstalls = config.PackageDevInstalls;
                            var fileName = DateTime.Now.ToSortableDateTimeText() + "_ApplicationGenerator_PackageDevInstalls.txt";

                            // kn todo - comment
                            // packageDevInstalls = packageDevInstalls.Randomize().Take(2).ToList();
                            // packageDevInstalls = new List<string> { "@gracesnoh/tiny" };

                            logInstallsDirectory.WriteToLog(fileName, packageDevInstalls.ToMultiLineList());

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, (object)packageDevInstalls.ToArray());
                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;

                    case ServerCommands.GET_CACHE_STATUS:
                        {
                            var config = generatorHandler.GeneratorConfiguration;
                            var mode = (string)commandPacket.Arguments.Single(a => a.Key == "mode").Value;
                            var cacheStatus = config.GetCacheStatus(mode, true);

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, (object)cacheStatus);
                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;

                    case ServerCommands.SET_INSTALL_STATUS:
                        {
                            var config = generatorHandler.GeneratorConfiguration;
                            var status = (string)commandPacket.Arguments.Single(a => a.Key == "status").Value;
                            var result = config.SetInstallStatus(status);

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, (object)result);
                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;

                    case ServerCommands.GET_FILE_ICON:
                        {
                            var relativePath = (string)commandPacket.Arguments.Single(a => a.Key == "relativePath").Value;
                            var config = generatorHandler.GeneratorConfiguration;
                            var file = (File)config.FileSystem[relativePath];
                            var bitmap = file.Icon.ToBitmap();

                            bitmap.MakeTransparent(Color.Black);

                            using (var stream = new System.IO.MemoryStream())
                            {
                                bitmap.Save(stream, ImageFormat.Gif);
                                stream.Flush();
                                stream.Rewind();

                                commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, stream.ToBase64() + Environment.NewLine.Repeat(2));
                            }

                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;

                    case ServerCommands.GET_FILE_CONTENTS:
                        {
                            var relativePath = (string)commandPacket.Arguments.Single(a => a.Key == "relativePath").Value;
                            var config = generatorHandler.GeneratorConfiguration;
                            var file = (File)config.FileSystem[relativePath];
                            var fileInfo = file.SystemLocalFile;
                            byte[] contents;

                            if (fileInfo.Exists)
                            {
                                contents = System.IO.File.ReadAllBytes(fileInfo.FullName);
                            }
                            else
                            {
                                contents = file.Info.ToString().ToBytes();
                            }

                            using (var stream = new System.IO.MemoryStream())
                            {
                                stream.Write(contents, 0, contents.Length);
                                stream.Flush();
                                stream.Rewind();

                                commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, stream.ToBase64() + Environment.NewLine.Repeat(2));
                            }

                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;

                    case ServerCommands.ADD_RESOURCE_BROWSE_FILE:
                        {
                            Thread thread;
                            ThreadStart browseFileAction;

                            browseFileAction = new ThreadStart(() =>
                            {
                                var filePath = AddResourceBrowseFile(commandPacket.Arguments);
                                
                                commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, filePath.NullToEmpty());
                            });


                            thread = new Thread(browseFileAction);
                            thread.SetApartmentState(ApartmentState.STA);

                            thread.Start();
                            thread.Join();

                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;

                    case ServerCommands.ADD_RESOURCE_CAPTURE_IMAGE:
                        {
                            Thread thread;
                            ThreadStart threadStartAction;

                            threadStartAction = new ThreadStart(() =>
                            {
                                var filePath = AddResourceCaptureImage(commandPacket.Arguments);

                                commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, filePath.NullToEmpty());
                            });


                            thread = new Thread(threadStartAction);
                            thread.SetApartmentState(ApartmentState.STA);

                            thread.Start();
                            thread.Join();

                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;

                    case ServerCommands.ADD_RESOURCE_CHOOSE_COLOR:
                        {
                            Thread thread;
                            ThreadStart threadStartAction;

                            threadStartAction = new ThreadStart(() =>
                            {
                                var color = AddResourceChooseColor(commandPacket.Arguments);

                                commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, color.NullToEmpty());
                            });


                            thread = new Thread(threadStartAction);
                            thread.SetApartmentState(ApartmentState.STA);

                            thread.Start();
                            thread.Join();

                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;

                    case ServerCommands.SHOW_DESIGNER:
                        {
                            Thread thread;
                            ThreadStart threadStartAction;

                            threadStartAction = new ThreadStart(() =>
                            {
                                ShowDesigner(commandPacket.Arguments);

                                commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, string.Empty);
                            });


                            thread = new Thread(threadStartAction);
                            thread.SetApartmentState(ApartmentState.STA);

                            thread.Start();
                            thread.Join();

                            outputWriter.WriteJsonCommand(commandPacket, this.JsonTextWrite);
                        }

                        break;


                    default:
                        DebugUtils.Break();
                        break;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                errorWriter.Write(ex.ToString());
                SendHydraStatus(string.Format("Error handling command {0}, Error: {1}", commandPacket.Command, ex.Message), StatusAlertLevel.CRITICAL);
#else
                errorWriter.Write(ex.Message);
#endif
                errorWriter.Flush();

                ex.HandleExitException(currentWorkingDirectory, runAsAutomated);

                Environment.Exit(1);
            }
        }

        private void SendHydraStatus(string status, string alertLevel)
        {
            if (mailslotClient != null)
            {
                mailslotClient.SendMessage(status);
            }

            if (this.alertInfo.AlertWhenLevel.Any(a => a == alertLevel) && this.alertInfo.AlertToAddress != null)
            {
                string subject;
                string body;
                var idleTime = ControlExtensions.GetIdleTime();

                switch (alertLevel)
                {
                    case StatusAlertLevel.CRITICAL:
                        subject = "Hydra Processing Critical!";
                        break;
                    case StatusAlertLevel.IMPORTANT:
                        subject = "Hydra Processing Important";
                        break;
                    case StatusAlertLevel.INFO:
                        subject = "Hydra Processing Info";
                        break;
                    default:
                        DebugUtils.Break();
                        subject = null;
                        break;
                }

                body = string.Format("Status: {0}, Server Name: {1}, Host Name: {2}, IP Address {3}, Timestamp {4}", status, Environment.MachineName, Dns.GetHostName(), NetworkExtensions.GetLocalIPAddress(), DateTime.Now);

                if (idleTime >= this.alertInfo.AlertWhenIdle)
                {
                    EmailExtensions.SendEmail(this.alertInfo.AlertFromAddress, this.alertInfo.AlertToAddress, subject, body);
                }
            }

            if (this.alertInfo.AlertUseSounds)
            {
                switch (alertLevel)
                {
                    case StatusAlertLevel.CRITICAL:
                        AlertingExtensions.Beep(AlertingExtensions.BeepType.Exclamation);
                        break;
                    case StatusAlertLevel.IMPORTANT:
                        AlertingExtensions.Beep(AlertingExtensions.BeepType.OK);
                        break;
                    case StatusAlertLevel.INFO:
                        AlertingExtensions.Beep(AlertingExtensions.BeepType.SimpleBeep);
                        break;
                    default:
                        DebugUtils.Break();
                        break;
                }
            }
        }

        /// <summary>   Starts application frontend. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/4/2021. </remarks>
        ///
        /// <param name="arguments">    The arguments. </param>

        public static void GenerateStarterAppFrontend(KeyValuePair<string, object>[] arguments)
        {
            string root = null;
            string appName;
            var currentWorkingDirectory = Environment.CurrentDirectory;
            var generatorHandlerType = (string)arguments.Single(a => a.Key == "GeneratorHandlerType").Value;
            var debug = (bool)arguments.Single(a => a.Key == "Debug").Value;
            var generatorHandler = AbstraXExtensions.GetGeneratorHandler(generatorHandlerType);
            var assembly = generatorHandler.GetType().Assembly;
            IGeneratorConfiguration generatorConfiguration;
            List<Type> types;

            types = assembly.GetAllTypes(null).Distinct().OrderBy(t => t.FullName).ToList();

            generatorConfiguration = new GeneratorConfiguration(types);

            using (var utilityHandler = generatorConfiguration.GetUtilityHandler(arguments.ToDictionary()))
            {
                System.IO.DirectoryInfo directory;

                if (arguments != null)
                {
                    root = (string)arguments.SingleOrDefault(a => a.Key.AsCaseless() == "webFrontEndRootPath").Value;
                }

                if (root == null)
                {
                    root = currentWorkingDirectory;
                }

                directory = new System.IO.DirectoryInfo(root);
                Environment.CurrentDirectory = directory.Parent.FullName;

                appName = directory.Name;

                utilityHandler.GenerateStarterAppFrontend(appName, Environment.CurrentDirectory, debug);
            }
        }

        /// <summary>   Generates an application frontend. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 1/9/2021. </remarks>
        ///
        /// <param name="arguments">    The arguments. </param>
        /// <param name="debug">        True to debug. </param>

        public static void GenerateCompleteAppFrontend(KeyValuePair<string, object>[] arguments, bool debug)
        {
            string root = null;
            string appName;
            var currentWorkingDirectory = Environment.CurrentDirectory;
            var generatorHandlerType = (string)arguments.Single(a => a.Key == "GeneratorHandlerType").Value;
            var generatorHandler = AbstraXExtensions.GetGeneratorHandler(generatorHandlerType);
            var assembly = generatorHandler.GetType().Assembly;
            IGeneratorConfiguration generatorConfiguration;
            List<Type> types;

            types = assembly.GetAllTypes(null).Distinct().OrderBy(t => t.FullName).ToList();

            generatorConfiguration = new GeneratorConfiguration(types);

            using (var utilityHandler = generatorConfiguration.GetUtilityHandler(arguments.ToDictionary()))
            {
                System.IO.DirectoryInfo directory;

                if (arguments != null)
                {
                    root = (string)arguments.SingleOrDefault(a => a.Key.AsCaseless() == "webFrontEndRootPath").Value;
                }

                if (root == null)
                {
                    root = currentWorkingDirectory;
                }

                directory = new System.IO.DirectoryInfo(root);
                appName = directory.Name;

                utilityHandler.GenerateCompleteAppFrontend(appName, Environment.CurrentDirectory, debug);
            }
        }

        /// <summary>   Shows the designer. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/27/2020. </remarks>
        ///
        /// <param name="keyValuePairs">    The arguments. </param>

        public static void ShowDesigner(KeyValuePair<string, object>[] keyValuePairs)
        {
            var currentWorkingDirectory = Environment.CurrentDirectory;
            var assemblyLocation = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var localThemeFolder = System.IO.Path.Combine(assemblyLocation, @"Themes");
            var localThemePath = System.IO.Path.Combine(localThemeFolder, @"variables.scss");
            var sassContent = System.IO.File.ReadAllText(localThemePath);
            string root = null;
            IResourceData resourceData;
            frmAppDesigner designer;

            if (keyValuePairs != null)
            {
                root = (string)keyValuePairs.SingleOrDefault(a => a.Key.AsCaseless() == "webFrontEndRootPath").Value;
            }

            if (root == null)
            {
                root = currentWorkingDirectory;
            }

            root = System.IO.Path.Combine(root, "hydra_resources");

            using (var resourceManager = new ResourceManager(root, sassContent))
            {
                resourceData = resourceManager.ResourceData;
            }

            using (resourceData)
            {
                designer = new frmAppDesigner(resourceData, localThemeFolder, currentWorkingDirectory);

                designer.ShowDialog();
            }
        }

        /// <summary>   Adds a resource capture image. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/26/2020. </remarks>
        ///
        /// <param name="keyValuePairs">    The arguments. </param>
        ///
        /// <returns>   A string. </returns>

        public static string AddResourceCaptureImage(KeyValuePair<string, object>[] keyValuePairs)
        {
            var controlPanel = new ScreenCaptureControlPanel();
            var resourceName = (string)keyValuePairs.Single(a => a.Key.AsCaseless() == "resourceName").Value;
            var root = (string)keyValuePairs.SingleOrDefault(a => a.Key.AsCaseless() == "webFrontEndRootPath").Value;
            var currentWorkingDirectory = Environment.CurrentDirectory;

            if (root == null)
            {
                root = currentWorkingDirectory;
            }

            root = System.IO.Path.Combine(root, "hydra_resources");

            if (controlPanel.ShowDialog() == DialogResult.OK)
            {
                var image = controlPanel.Image;
                var filePath = System.IO.Path.Combine(root, resourceName + ".jpeg");

                image.Save(filePath, ImageFormat.Jpeg);
                image.Dispose();

                using (var resourceManager = new ResourceManager(root))
                {
                    resourceManager.AddFileResource(resourceName, filePath, true);
                }

                return filePath;
            }
            else
            {
                return null;
            }
        }

        /// <summary>   Adds a resource choose color. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/24/2020. </remarks>
        ///
        /// <param name="keyValuePairs">    The arguments. </param>
        ///
        /// <returns>   A string. </returns>

        public static string AddResourceChooseColor(KeyValuePair<string, object>[] keyValuePairs)
        {
            var assemblyLocation = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var localThemeFolder = System.IO.Path.Combine(assemblyLocation, @"Themes");
            var localThemePath = System.IO.Path.Combine(localThemeFolder, @"variables.scss");
            var resourceName = (string)keyValuePairs.Single(a => a.Key.AsCaseless() == "resourceName").Value;
            var root = (string)keyValuePairs.SingleOrDefault(a => a.Key.AsCaseless() == "webFrontEndRootPath").Value;
            var currentWorkingDirectory = Environment.CurrentDirectory;
            var frmColorChooser = new frmColorChooser();
            var sassContent = System.IO.File.ReadAllText(localThemePath);
            string htmlColor;
            IResourceData resourceData;

            if (root == null)
            {
                root = currentWorkingDirectory;
            }

            root = System.IO.Path.Combine(root, "hydra_resources");

            using (var resourceManager = new ResourceManager(root, sassContent))
            {
                htmlColor = resourceManager.GetResourceValue(resourceName);
                resourceData = resourceManager.ResourceData;
            }

            if (htmlColor != null)
            {
                frmColorChooser.Color = ColorTranslator.FromHtml(htmlColor);
            }

            frmColorChooser.ResourceName = resourceName;
            frmColorChooser.ResourceData = resourceData;

            if (frmColorChooser.ShowDialog() == DialogResult.OK)
            {
                htmlColor = ColorTranslator.ToHtml(frmColorChooser.Color.Value);

                using (var resourceManager = new ResourceManager(root, sassContent))
                {
                    resourceManager.AddResource(resourceName, htmlColor);
                }

                return htmlColor;
            }

            return string.Empty;
        }

        /// <summary>   Adds a resource browse file. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 11/24/2020. </remarks>
        ///
        /// <param name="keyValuePairs">    Message describing the command. </param>
        ///
        /// <returns>   path. </returns>

        public static string AddResourceBrowseFile(KeyValuePair<string, object>[] keyValuePairs)
        {
            var openFileDialog = new OpenFileDialog();
            var filter = (string)keyValuePairs.Single(a => a.Key.AsCaseless() == "filter").Value;
            var resourceName = (string)keyValuePairs.Single(a => a.Key.AsCaseless() == "resourceName").Value;
            var root = (string)keyValuePairs.SingleOrDefault(a => a.Key.AsCaseless() == "webFrontEndRootPath").Value;
            var currentWorkingDirectory = Environment.CurrentDirectory;

            if (root == null)
            {
                root = currentWorkingDirectory;
            }

            root = System.IO.Path.Combine(root, "hydra_resources");

            openFileDialog.Filter = filter;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var filePath = openFileDialog.FileName;

                using (var resourceManager = new ResourceManager(root))
                {
                    resourceManager.AddFileResource(resourceName, filePath);
                }

                return filePath;
            }
            else
            {
                return null;
            }
        }
    }
}
