using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Utils;
using System.Linq;
using AssemblyAttributesShim;
using AssemblyAttributesService.JsonTypes;
using System.IO;
using Serilog;
using CodeInterfaces;

namespace AssemblyAttributesService
{
    public class StandardStreamService : BaseStandardStreamService, IAssemblyAttributesService
    {

        private ILogger logger;
        private bool runAsAutomated;

        public StandardStreamService(ILogger logger)
        {
            this.logger = logger;

            logger.Information("Instantiated");
        }

        protected override void HandleCommand(CommandPacket commandPacket)
        {
            try
            {
                logger.Information(string.Format("Received command packet, command: {0}, packet: {1}", commandPacket.Command, commandPacket.GetSnippet()));

                switch (commandPacket.Command)
                {
                    case ServerCommands.TERMINATE:
                        {
                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "Terminating");
                            outputWriter.WriteJsonCommand(commandPacket);

                            logger.Information(string.Format("Sending command packet, command: {0}, packet: {1}", commandPacket.Command, commandPacket.GetSnippet()));

                            outputWriter.WriteLine(Environment.NewLine);

                            this.Stop();
                            
                            Environment.Exit(0);
                        }

                        break;

                    case ServerCommands.CONNECT:
                        {
                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "Connected successfully");

                            logger.Information(string.Format("Sending command packet, command: {0}, packet: {1}", commandPacket.Command, commandPacket.GetSnippet()));

                            outputWriter.WriteJsonCommand(commandPacket);
                        }

                        break;

                    case ServerCommands.PING:
                        {
                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "Success");

                            logger.Information(string.Format("Sending command packet, command: {0}, packet: {1}", commandPacket.Command, commandPacket.GetSnippet()));

                            outputWriter.WriteJsonCommand(commandPacket);
                        }

                        break;

                    case ServerCommands.GET_VERSION:
                        {
                            var version = Assembly.GetEntryAssembly().GetAttributes().Version;

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, version);

                            logger.Information(string.Format("Sending command packet, command: {0}, packet: {1}", commandPacket.Command, commandPacket.GetSnippet()));

                            outputWriter.WriteJsonCommand(commandPacket);
                        }

                        break;

                    case ServerCommands.GET_ASSEMBLY_ATTRIBUTES:
                        {
                            var assemblyPath = (string) commandPacket.Arguments.Single().Value;
                            var file = new FileInfo(assemblyPath);
                            var assembly = Assembly.LoadFrom(assemblyPath);
                            var attributes = AssemblyExtensions.GetAttributes(assembly);
                            var attributesJson = new AssemblyAttributesJson
                            {
                                Title = attributes.Title,
                                Description = attributes.Description,
                                Company = attributes.Company,
                                Product = attributes.Product,
                                Copyright = attributes.Copyright,
                                Version = attributes.Version,
                                VersionVariant = attributes.GetCustom<AssemblyVersionVariantAttribute>("Variant")
                            };

                            attributesJson.Hash = file.GetHash();
                            
                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, attributesJson);

                            logger.Information(string.Format("Sending command packet, command: {0}, packet: {1}", commandPacket.Command, commandPacket.GetSnippet()));

                            outputWriter.WriteJsonCommand(commandPacket);
                        }

                        break;

                    default:
                        {
                            DebugUtils.Break();
                        }

                        break;
                }
            }
            catch (Exception ex)
            {

                logger.Error(string.Format("Error occured, exiting, error: {0}", ex.ToString()));

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
