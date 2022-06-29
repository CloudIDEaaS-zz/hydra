using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Utils;
using System.Linq;
using AbstraX;

namespace NetCoreReflectionShim.Service
{
    public class StandardStreamService : BaseStandardStreamService, INetCoreReflectionService
    {
        public Dictionary<int, ICustomAttributeProvider> CachedTokenObjects { get; }
        public Dictionary<string, Assembly> CachedAssemblies { get; }
        public Dictionary<int, object> CachedObjects { get; }
        public event GetTypeProxyEventHandler GetTypeProxyEvent;
        private bool runAsAutomated;

        public StandardStreamService(bool runAsAutomated)
        {
            var appDomain = AppDomain.CurrentDomain;

            this.runAsAutomated = runAsAutomated;
            this.CachedTokenObjects = new Dictionary<int, ICustomAttributeProvider>();
            this.CachedAssemblies = new Dictionary<string, Assembly>();
            this.CachedObjects = new Dictionary<int, object>();

            appDomain.AssemblyResolve += Utils.AssemblyExtensions.AssemblyResolve;
        }

        protected override void HandleCommand(CommandPacket commandPacket)
        {
            try
            {
                switch (commandPacket.Command)
                {
                    case ServerCommands.TERMINATE:
                        {
                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, "Terminating");
                            outputWriter.WriteJsonCommand(commandPacket);

                            outputWriter.WriteLine(Environment.NewLine);

                            this.Stop();
                            
                            Environment.Exit(0);
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

                    case ServerCommands.GET_VERSION:
                        {
                            var version = Assembly.GetEntryAssembly().GetAttributes().Version;

                            commandPacket = new CommandPacket(commandPacket.Command, commandPacket.SentTimestamp, version);
                            outputWriter.WriteJsonCommand(commandPacket);
                        }

                        break;

                    default:
                        {
                            this.HandleReflectionCommand(outputWriter, commandPacket);
                        }

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

                ex.HandleExitException(currentWorkingDirectory, runAsAutomated);

                Environment.Exit(1);
            }
        }

        public Type GetTypeProxy(Type type)
        {
            var args = new GetTypeProxyEventArgs(type);

            GetTypeProxyEvent(this, args);

            return args.ProxyType;
        }
    }
}
