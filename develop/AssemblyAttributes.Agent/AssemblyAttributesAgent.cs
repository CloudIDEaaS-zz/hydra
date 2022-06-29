using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;
using TypeExtensions = Utils.TypeExtensions;

namespace AssemblyAttributesShim.Agent
{
    public partial class AssemblyAttributesAgent : IDisposable, IAssemblyAttributesAgent
    {
        private Process serviceProcess;
        private TextReader reader;
        private TextWriter writer;
        private bool debug;
        private bool runAsAutomated;
        public bool TestMode { get; set; }
        private IConfiguration configuration;
        private string exeCopyFullName;
        public Guid HandlerId { get; set; }
        public event StartServiceEventHandler OnStartService;
        public event EventHandler Exited;
        public event DataReceivedEventHandler ErrorDataReceived;
        public event DataReceivedEventHandler OutputDataReceived;

        public AssemblyAttributesAgent(IConfiguration configuration, bool debug = false, bool testMode = false, bool runAsAutomated = false)
        {
            this.debug = debug;
            this.runAsAutomated = runAsAutomated;
            this.configuration = configuration;
            this.TestMode = testMode;
        }

        public bool Debug
        {
            get
            {
                return debug;
            }

            set
            {
                debug = value;
            }
        }

        public AssemblyAttributesService.JsonTypes.AssemblyAttributesJson GetAssemblyAttributes(string fullName)
        {
            try
            {
                CommandPacket<AssemblyAttributesService.JsonTypes.AssemblyAttributesJson> commandPacketReturn;

                var commandPacket = new CommandPacket
                {
                    Command = ServerCommands.GET_ASSEMBLY_ATTRIBUTES,
                    Arguments = new Dictionary<string, object>
                {
                    { "AssemblyFullName", fullName }
                }
                    .Select(a => new KeyValuePair<string, object>(a.Key, a.Value)).ToArray()
                };

                if (serviceProcess == null)
                {
                    StartService();
                }

                writer.WriteJsonCommand(commandPacket);
                commandPacketReturn = reader.ReadJsonCommand<AssemblyAttributesService.JsonTypes.AssemblyAttributesJson>();

                return commandPacketReturn.Response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void StartService()
        {
            CommandPacket commandPacket;

            if (this.TestMode)
            {
                var args = new StartServiceEventArgs();

                OnStartService(this, args);

                reader = args.Reader;
                writer = args.Writer;

                serviceProcess = new Process();
            }
            else
            {
                ProcessStartInfo startInfo;
                var entryAssembly = Assembly.GetEntryAssembly();
                var relativeAssemblyAttributesServiceExeLocation = configuration["AssemblyAttributesServiceExeLocation"];
                var arguments = configuration["AssemblyAttributesServiceArguments"];
                var shimServiceExeFile = new FileInfo(Path.GetFullPath(Path.Combine(entryAssembly.Location, relativeAssemblyAttributesServiceExeLocation)));
                var exeCopyName = Path.GetFileNameWithoutExtension(shimServiceExeFile.Name) + "." + this.HandlerId + ".exe";
                var location = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var logLocation = Path.GetFullPath(Path.Combine(location, @"..\..\..\Logs"));
                var logDirectory = new DirectoryInfo(logLocation);

                if (!logDirectory.Exists)
                {
                    logDirectory.Create();
                }

                exeCopyFullName = Path.Combine(shimServiceExeFile.DirectoryName, exeCopyName);

                shimServiceExeFile.CopyTo(exeCopyFullName, true);

                foreach (var process in Process.GetProcessesByName(exeCopyName))
                {
                    process.Kill();
                }

                if (debug)
                {
                    arguments += " -debug";
                }

                if (runAsAutomated)
                {
                    arguments += " -runAsAutomated";
                }

                arguments += " -logLocation:" + "\"" + logLocation + "\"";

                startInfo = new ProcessStartInfo
                {
                    FileName = exeCopyFullName,
                    Arguments = arguments
                };

                serviceProcess = new Process();

                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardError = true;
                startInfo.WorkingDirectory = Directory.GetCurrentDirectory();

                serviceProcess.StartInfo = startInfo;

                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;

                serviceProcess.ErrorDataReceived += (s, e2) =>
                {
                    var data = e2.Data;

                    if (data != null)
                    {
                        ErrorDataReceived(s, e2);
                        Console.WriteLine(data);
                    }
                };

                serviceProcess.Exited += (s, e2) =>
                {
                    var exitCode = serviceProcess.ExitCode;

                    if (exitCode != 0)
                    {
                        Exited(s, e2);

                        Console.WriteLine("ShimServiceProcess failed with ExitCode={0}", exitCode);
                    }
                };

                serviceProcess.Start();

                reader = serviceProcess.StandardOutput;
                writer = serviceProcess.StandardInput;
            }

            commandPacket = new CommandPacket
            {
                Command = ServerCommands.CONNECT
            };

            writer.WriteJsonCommand(commandPacket);
            commandPacket = reader.ReadJsonCommand();
        }

        public void Dispose()
        {
            if (serviceProcess != null)
            {
                if (writer != null)
                {
                    var commandPacket = new CommandPacket
                    {
                        Command = ServerCommands.TERMINATE
                    };

                    writer.WriteJsonCommand(commandPacket);

                    Thread.Sleep(1000);

                    if (!serviceProcess.HasExited)
                    {
                        serviceProcess.Kill();
                    }
                }

                Thread.Sleep(1000);
                serviceProcess = null;

                try
                {
                    var exeFile = new FileInfo(exeCopyFullName);

                    if (exeFile.Exists)
                    {
                        exeFile.Delete();
                    }
                }
                catch
                {
                }
            }
        }
    }
}
