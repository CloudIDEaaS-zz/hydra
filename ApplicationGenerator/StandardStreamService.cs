using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbstraX.Handlers.CommandHandlers;
using AbstraX.FolderStructure;
using Utils;

namespace AbstraX
{
    public class StandardStreamService : BaseStandardStreamService
    {
        private GeneratorHandler generatorHandler;
        private WebApiService webApiService;

        public StandardStreamService(WebApiService webApiService)
        {
            this.webApiService = webApiService;
        }

        protected override void HandleCommand(CommandPacket commandPacket)
        {
            try
            {
                switch (commandPacket.Command)
                {
                    case ServerCommands.GENERATE:
                        {
                            var kind = commandPacket.Arguments.Single(a => a.Key == "Kind").Value;
                            var generatorKind = EnumUtils.GetValue<GeneratorKind>(((string)kind).ToTitleCase());

                            if (generatorKind == GeneratorKind.App)
                            { 
                                var entitiesProjectPath = commandPacket.Arguments.Single(a => a.Key == "EntitiesProjectPath").Value;
                                var servicesProjectPath = commandPacket.Arguments.Single(a => a.Key == "ServicesProjectPath").Value;
                                var generatorPass = EnumUtils.GetValue<GeneratorPass>((string)commandPacket.Arguments.Single(a => a.Key == "GeneratorPass").Value);
                                var noFileCreation = bool.Parse(commandPacket.Arguments.Single(a => a.Key == "NoFileCreation").Value.ToString());
                                var projectFolderRoot = currentWorkingDirectory;
                                object packageCachePath = null;

                                if (commandPacket.Arguments.Any(a => a.Key == "PackageCachePath"))
                                {
                                    packageCachePath = commandPacket.Arguments.Single(a => a.Key == "PackageCachePath").Value;
                                }

                                generatorHandler = new GeneratorHandler();

                                webApiService.GeneratorHandler = generatorHandler;
                                generatorHandler.SuppressDebugOutput = true;

                                generatorHandler.Execute(new Dictionary<string, object>
                                {
                                    { "GeneratorKind", generatorKind },
                                    { "EntitiesProjectPath", entitiesProjectPath },
                                    { "ServicesProjectPath", servicesProjectPath },
                                    { "PackageCachePath", packageCachePath },
                                    { "GeneratorMode", GeneratorMode.RedirectedConsole },
                                    { "GeneratorOptions", new RedirectedGeneratorOptions(outputWriter, errorWriter, generatorPass, noFileCreation) }
                                });
                            }
                            else if (generatorKind == GeneratorKind.BusinessModel)
                            {
                                var generatorPass = EnumUtils.GetValue<GeneratorPass>((string)commandPacket.Arguments.Single(a => a.Key == "GeneratorPass").Value);
                                var noFileCreation = bool.Parse(commandPacket.Arguments.Single(a => a.Key == "NoFileCreation").Value.ToString());
                                var templateFile = commandPacket.Arguments.Single(a => a.Key == "TemplateFile").Value.ToString();
                                var projectFolderRoot = currentWorkingDirectory;

                                generatorHandler = new GeneratorHandler();

                                webApiService.GeneratorHandler = generatorHandler;
                                generatorHandler.SuppressDebugOutput = true;

                                generatorHandler.Execute(new Dictionary<string, object>
                                {
                                    { "GeneratorKind", generatorKind },
                                    { "TemplateFile", templateFile },
                                    { "GeneratorMode", GeneratorMode.RedirectedConsole },
                                    { "GeneratorOptions", new RedirectedGeneratorOptions(outputWriter, errorWriter, generatorPass, noFileCreation) }
                                });
                            }
                            else if (generatorKind == GeneratorKind.Entities)
                            {
                                var generatorPass = EnumUtils.GetValue<GeneratorPass>((string)commandPacket.Arguments.Single(a => a.Key == "GeneratorPass").Value);
                                var noFileCreation = bool.Parse(commandPacket.Arguments.Single(a => a.Key == "NoFileCreation").Value.ToString());
                                var templateFile = commandPacket.Arguments.Single(a => a.Key == "TemplateFile").Value.ToString();
                                var jsonFile = commandPacket.Arguments.Single(a => a.Key == "JsonFile").Value.ToString();
                                var businessModelFile = commandPacket.Arguments.Single(a => a.Key == "BusinessModelFile").Value.ToString();
                                var entitiesProjectPath = commandPacket.Arguments.Single(a => a.Key == "EntitiesProjectPath").Value;
                                var projectFolderRoot = currentWorkingDirectory;

                                generatorHandler = new GeneratorHandler();

                                webApiService.GeneratorHandler = generatorHandler;
                                generatorHandler.SuppressDebugOutput = true;

                                generatorHandler.Execute(new Dictionary<string, object>
                                {
                                    { "GeneratorKind", generatorKind },
                                    { "TemplateFile", templateFile },
                                    { "JsonFile", jsonFile },
                                    { "BusinessModelFile", businessModelFile },
                                    { "EntitiesProjectPath", entitiesProjectPath },
                                    { "GeneratorMode", GeneratorMode.RedirectedConsole },
                                    { "GeneratorOptions", new RedirectedGeneratorOptions(outputWriter, errorWriter, generatorPass, noFileCreation) }
                                });
                            }
                            else if (generatorKind == GeneratorKind.Workspace)
                            {
                                var generatorPass = EnumUtils.GetValue<GeneratorPass>((string)commandPacket.Arguments.Single(a => a.Key == "GeneratorPass").Value);
                                var noFileCreation = bool.Parse(commandPacket.Arguments.Single(a => a.Key == "NoFileCreation").Value.ToString());
                                var appName = commandPacket.Arguments.Single(a => a.Key == "AppName").Value.ToString();
                                var projectFolderRoot = currentWorkingDirectory;

                                generatorHandler = new GeneratorHandler();

                                webApiService.GeneratorHandler = generatorHandler;
                                generatorHandler.SuppressDebugOutput = true;

                                generatorHandler.Execute(new Dictionary<string, object>
                                {
                                    { "GeneratorKind", generatorKind },
                                    { "AppName", appName },
                                    { "GeneratorMode", GeneratorMode.RedirectedConsole },
                                    { "GeneratorOptions", new RedirectedGeneratorOptions(outputWriter, errorWriter, generatorPass, noFileCreation) }
                                });
                            }
                        }

                        break;

                    case ServerCommands.TERMINATE:
                        {
                            if (generatorHandler != null)
                            {
                                var config = generatorHandler.GeneratorConfiguration;

                                commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "Terminating");
                                outputWriter.WriteJsonCommand(commandPacket, null);

                                config.StopServices();

                                outputWriter.WriteLine(Environment.NewLine);
                            }
                            else
                            {
                                commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "Terminating");
                                outputWriter.WriteJsonCommand(commandPacket);
                            }

                            this.Stop();
                        }

                        break;

                    case ServerCommands.CONNECT:
                        {
                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "Connected successfully");
                            outputWriter.WriteJsonCommand(commandPacket);
                        }

                        break;

                    case ServerCommands.PING:
                        {
                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "Success");
                            outputWriter.WriteJsonCommand(commandPacket);
                        }

                        break;

                    case ServerCommands.GET_FOLDER:
                        {
                            var relativePath = (string)commandPacket.Arguments.Single(a => a.Key == "relativePath").Value;
                            var config = generatorHandler.GeneratorConfiguration;
                            var folder = (Folder)config.FileSystem[relativePath];

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, folder);
                            outputWriter.WriteJsonCommand(commandPacket);
                        }

                        break;

                    case ServerCommands.GET_FILE:
                        {
                            var relativePath = (string)commandPacket.Arguments.Single(a => a.Key == "relativePath").Value;
                            var config = generatorHandler.GeneratorConfiguration;
                            var file = (File)config.FileSystem[relativePath];

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, file);
                            outputWriter.WriteJsonCommand(commandPacket);
                        }

                        break;

                    case ServerCommands.GET_FOLDERS:
                        {
                            var relativePath = (string)commandPacket.Arguments.Single(a => a.Key == "relativePath").Value;
                            var config = generatorHandler.GeneratorConfiguration;
                            var folder = (Folder)config.FileSystem[relativePath];

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, folder.Folders.ToArray());
                            outputWriter.WriteJsonCommand(commandPacket);
                        }

                        break;

                    case ServerCommands.GET_FILES:
                        {
                            var relativePath = (string)commandPacket.Arguments.Single(a => a.Key == "relativePath").Value;
                            var config = generatorHandler.GeneratorConfiguration;
                            var folder = (Folder)config.FileSystem[relativePath];

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, folder.Files.ToArray());
                            outputWriter.WriteJsonCommand(commandPacket);
                        }

                        break;

                    case ServerCommands.GET_PACKAGE_INSTALLS:
                        {
                            var config = generatorHandler.GeneratorConfiguration;
                            var packageInstalls = config.PackageInstalls;

                            // kn todo - comment
                            // packageInstalls = packageInstalls.Randomize().Take(5).ToList();
                            // packageInstalls = new List<string> { "@gracesnoh/tiny" };

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, packageInstalls.ToArray());
                            outputWriter.WriteJsonCommand(commandPacket);
                        }

                        break;

                    case ServerCommands.GET_PACKAGE_DEV_INSTALLS:
                        {
                            var config = generatorHandler.GeneratorConfiguration;
                            var packageDevInstalls = config.PackageDevInstalls;

                            // kn todo - comment
                            // packageDevInstalls = packageDevInstalls.Randomize().Take(2).ToList();
                            // packageDevInstalls = new List<string> { "@gracesnoh/tiny" };

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, packageDevInstalls.ToArray());
                            outputWriter.WriteJsonCommand(commandPacket);
                        }

                        break;

                    case ServerCommands.GET_CACHE_STATUS:
                        {
                            var config = generatorHandler.GeneratorConfiguration;
                            var mode = (string)commandPacket.Arguments.Single(a => a.Key == "mode").Value;
                            var cacheStatus = config.GetCacheStatus(mode, true);

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, cacheStatus);
                            outputWriter.WriteJsonCommand(commandPacket);
                        }

                        break;

                    case ServerCommands.SET_INSTALL_STATUS:
                        {
                            var config = generatorHandler.GeneratorConfiguration;
                            var status = (string)commandPacket.Arguments.Single(a => a.Key == "status").Value;
                            var result = config.SetInstallStatus(status);

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, result);
                            outputWriter.WriteJsonCommand(commandPacket);
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

                            outputWriter.WriteJsonCommand(commandPacket);
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

                            outputWriter.WriteJsonCommand(commandPacket);
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
#else
                errorWriter.Write(ex.Message);
#endif
                errorWriter.Flush();

                Environment.Exit(1);
            }
        }
    }
}
