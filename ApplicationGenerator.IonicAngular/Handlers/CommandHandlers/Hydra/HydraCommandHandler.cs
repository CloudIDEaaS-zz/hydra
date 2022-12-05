

// file:	Handlers\CommandHandlers\NugetCommandHandler.cs
//
// summary:	Implements the nuget command handler class

using Logging.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Utils;
using Utils.InProcessTransactions;
using Utils.ProcessHelpers;

namespace AbstraX.Handlers.CommandHandlers
{
    /// <summary>   A nuget command handler. </summary>
    ///
    /// <remarks>   Ken, 10/12/2020. </remarks>

    public class HydraCommandHandler : BaseWindowsCommandHandler
    {
        private Dictionary<string, object> arguments;
        private Process electronProcess;

        public HydraCommandHandler(Dictionary<string, object> arguments = null) : base("hydra")
        {
            this.arguments = arguments;
        }

        public Process ElectronProcess
        {
            get
            {
                return electronProcess;
            }
        }

        public void LaunchRenderer()
        {
            this.NoWait = true;

            base.LaunchForStdio(Directory.GetCurrentDirectory(), "launchRenderer", "--logClient");
        }

        public void GenerateApp(string appName, string directory, bool debug)
        {
            if (arguments.ContainsKey("ClientPipe"))
            {
                RunRemoteCommand("generate app" + (debug ? " --logServiceMessages --logClient --debug" : string.Empty), directory, true);
            }
            else
            {
                base.RunCommand("generate app" + (debug ? " --logServiceMessages --logClient --debug" : string.Empty), directory);
            }
        }

        public void Serve(string directory)
        {
            this.NoWait = true;

            base.RunCommand("serve", directory);
        }

        public void Start(string appName, string directory, bool debug)
        {
            if (arguments.ContainsKey("ClientPipe"))
            {
                string[] args;

                if (debug)
                {
                    args = new[] { appName, "--debug", "--useAgent" };
                }
                else
                {
                    args = new[] { appName, "--useAgent" };
                }

                RunRemoteCommand("start", directory, true, args);
            }
            else
            {
                base.RunCommand("start", directory, appName);
            }
        }

        public void Build(string directory, bool debug = false)
        {
            if (arguments != null && arguments.ContainsKey("ClientPipe"))
            {
                string[] args;

                if (debug)
                {
                    args = new[] { "--debug", "--useAgent" };
                }
                else
                {
                    args = new[] { "--useAgent" };
                }

                RunRemoteCommand("build", directory, true, args);
            }
            else
            {
                base.RunCommand("build", directory);
            }
        }

        public void Build(string directory, params string[] arguments)
        {
            base.RunCommand("build", directory, arguments);
        }

        public void Add(string directory, params string[] arguments)
        {
            base.RunCommand("add", directory, arguments);
        }

        private void RunRemoteCommand(string command, string directory, bool waitForPreviousExit, params string[] args)
        {
            var clientPipe = (string)arguments["ClientPipe"];
            string remainingKinds = null;

            if (arguments.ContainsKey("RemainingKinds"))
            {
                remainingKinds = (string)arguments["RemainingKinds"];
            }

            using (var pipeClient = new NamedPipeClientStream(".", clientPipe))
            {
                string ack;
                StreamReader reader;
                StreamWriter writer;

                try
                {
                    pipeClient.Connect(1000);

                    reader = new StreamReader(pipeClient);

                    Console.WriteLine("Generator waiting for acknowledgement...");
                    ack = reader.ReadLine();

                    if (ack == "Connected!")
                    {
                        CommandPacket commandPacket;

                        Console.WriteLine(ack);

                        writer = new StreamWriter(pipeClient);

                        commandPacket = new CommandPacket("runcommand", new Dictionary<string, object>
                        {
                            { "commandExe", CommandExe },
                            { "command", command },
                            { "remainingKinds", remainingKinds },
                            { "workingDirectory", directory },
                            { "waitForPreviousExit", waitForPreviousExit },
                            { "arguments", args },
                        });

                        writer.WriteJsonCommand(commandPacket);
                    }
                    else
                    {
                        Console.WriteLine(ack);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not connect to pipe '{0}'. Error '{1}'", clientPipe, ex.Message);
                }
            }
        }
    }
}
