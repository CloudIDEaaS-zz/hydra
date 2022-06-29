using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Utils;
using System.Windows.Forms;
using System.Web;
using AbstraX;
using System.Drawing;
using AbstraX.ClientFolderStructure;
using System.Drawing.Imaging;
using System.Threading;

namespace ApplicationGenerator.Agent
{
    public class ApplicationGeneratorAgent : IDisposable
    {
        private Process generatorProcess;
        private int fileSequence;
        public event EventHandler Exited;

        private ApplicationGeneratorAgent(bool debug = false)
        {
            Initialize(debug);
        }

        public static ApplicationGeneratorAgent GetInstance(bool debug = false)
        {
            var name = typeof(ApplicationGeneratorAgent).Name;

            if (RunningObjectTable.ObjectRunning(name))
            {
                var agent = RunningObjectTable.GetObject<ApplicationGeneratorAgent>(name);

                agent.ResetSequence();

                return agent;
            }
            else
            {
                var generatorAgent = new ApplicationGeneratorAgent(debug);

                RunningObjectTable.RegisterObject(name, generatorAgent);

                return generatorAgent;
            }
        }

        private void ResetSequence()
        {
            fileSequence = 1;
        }

        public void Initialize(bool debug = false)
        {
            var agentServer = Environment.ExpandEnvironmentVariables(@"%HYDRASOLUTIONPATH%\ApplicationGenerator\bin\Debug\ApplicationGenerator.exe");
            var arguments = debug ? string.Format("{0} {1} {2}:\"{3}\"", "-waitForInput", "-debug", "-cwd", Environment.CurrentDirectory) : string.Format("{0} {1}:\"{2}\"", "-waitForInput", "-cwd", Environment.CurrentDirectory);
            var startInfo = new ProcessStartInfo(agentServer, arguments);

            foreach (var process in Process.GetProcessesByName("ApplicationGenerator"))
            {
                try
                {
                    process.Kill();
                }
                catch
                {
                }
            }

            generatorProcess = new Process();

            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;

            generatorProcess.EnableRaisingEvents = true;
            generatorProcess.StartInfo = startInfo;

            generatorProcess.Exited += new EventHandler(generatorProcess_Exited);

            generatorProcess.Start();

            Console.WriteLine("Generator running, ProcessId: {0}", generatorProcess.Id);
        }

        private void generatorProcess_Exited(object sender, EventArgs e)
        {
            Exited(sender, e);
        }

        public object Ping()
        {
            if (generatorProcess.HasExited)
            {
                throw new ExecutionEngineException("ApplicationGeneratorAgent process has exited!");
            }

            generatorProcess.StandardInput.WriteJson(new CommandPacket(ServerCommands.PING));

            return generatorProcess.StandardOutput.ReadJson();
        }

        public object SendSimpleCommand(string command)
        {
            if (generatorProcess.HasExited)
            {
                throw new ExecutionEngineException("ApplicationGeneratorAgent process has exited!");
            }

            generatorProcess.StandardInput.WriteJson(new CommandPacket(command));

            return generatorProcess.StandardOutput.ReadJson();
        }

        public Folder GetFolder(string relativePath)
        {
            CommandPacket<Folder> commandPacket;

            if (generatorProcess.HasExited)
            {
                throw new ExecutionEngineException("ApplicationGeneratorAgent process has exited!");
            }

            generatorProcess.StandardInput.WriteJson(new CommandPacket(ServerCommands.GET_FOLDER, new KeyValuePair<string, object>("relativePath", relativePath)));
            commandPacket = generatorProcess.StandardOutput.ReadJson<CommandPacket<Folder>>();

            return commandPacket.Response;
        }

        public File GetFile(string relativePath)
        {
            CommandPacket<File> commandPacket;

            if (generatorProcess.HasExited)
            {
                throw new ExecutionEngineException("ApplicationGeneratorAgent process has exited!");
            }

            generatorProcess.StandardInput.WriteJson(new CommandPacket(ServerCommands.GET_FILE, new KeyValuePair<string, object>("relativePath", relativePath)));

            commandPacket = generatorProcess.StandardOutput.ReadJson<CommandPacket<File>>();

            return commandPacket.Response;
        }

        public Folder[] GetFolders(string relativePath)
        {
            CommandPacket<Folder[]> commandPacket;

            if (generatorProcess.HasExited)
            {
                throw new ExecutionEngineException("ApplicationGeneratorAgent process has exited!");
            }

            generatorProcess.StandardInput.WriteJson(new CommandPacket(ServerCommands.GET_FOLDERS, new KeyValuePair<string, object>("relativePath", relativePath)));

            commandPacket = generatorProcess.StandardOutput.ReadJson<CommandPacket<Folder[]>>();

            return commandPacket.Response;
        }

        public File[] GetFiles(string relativePath)
        {
            CommandPacket<File[]> commandPacket;

            if (generatorProcess.HasExited)
            {
                throw new ExecutionEngineException("ApplicationGeneratorAgent process has exited!");
            }

            generatorProcess.StandardInput.WriteJson(new CommandPacket(ServerCommands.GET_FILES, new KeyValuePair<string, object>("relativePath", relativePath)));

            commandPacket = generatorProcess.StandardOutput.ReadJson<CommandPacket<File[]>>();

            return commandPacket.Response;
        }

        public Image GetFileIcon(string relativePath)
        {
            byte[] contents;
            string text;
            CommandPacket<string> commandPacket;

            if (generatorProcess.HasExited)
            {
                throw new ExecutionEngineException("ApplicationGeneratorAgent process has exited!");
            }

            generatorProcess.StandardInput.WriteJson(new CommandPacket(ServerCommands.GET_FILE_ICON, new KeyValuePair<string, object>("relativePath", relativePath)));
            commandPacket = generatorProcess.StandardOutput.ReadJson<CommandPacket<string>>();

            text = commandPacket.Response;
            contents = text.FromBase64();

            using (var stream = contents.ToMemory())
            {
                var bitmap = new Bitmap(Bitmap.FromStream(stream, true));

                bitmap.MakeTransparent(Color.Black);

                return bitmap;
            }
        }

        public byte[] GetFileContents(string relativePath)
        {
            string text;
            CommandPacket<string> commandPacket;

            if (generatorProcess.HasExited)
            {
                throw new ExecutionEngineException("ApplicationGeneratorAgent process has exited!");
            }

            generatorProcess.StandardInput.WriteJson(new CommandPacket(ServerCommands.GET_FILE_CONTENTS, new KeyValuePair<string, object>("relativePath", relativePath)));
            commandPacket = generatorProcess.StandardOutput.ReadJson<CommandPacket<string>>();

            text = commandPacket.Response;

            return text.FromBase64();
        }

        public void Generate(string solutionPath, string entitiesProjectPath, string servicesProjectPath, GeneratorPass generatorPass, Func<string, bool> responseCallback)
        {
            var skippedFirstLine = false;

            if (generatorProcess.HasExited)
            {
                throw new ExecutionEngineException("ApplicationGeneratorAgent process has exited!");
            }

            generatorProcess.StandardInput.WriteJson(new CommandPacket(ServerCommands.GENERATE, new Dictionary<string, object>
            {
                { "SolutionPath", solutionPath },
                { "EntitiesProjectPath", entitiesProjectPath },
                { "ServicesProjectPath", servicesProjectPath },
                { "GeneratorPass", generatorPass.ToString() },
            }));

            while (true)
            {
                var line = generatorProcess.StandardOutput.ReadLine();

                if (!skippedFirstLine)
                {
                    // not sure why this happens
                    skippedFirstLine = true;
                }
                else
                {
                    if (!responseCallback(line))
                    {
                        break;
                    }
                }
            }
        }

        public void Dispose()
        {
            int exitCode;
            var name = typeof(ApplicationGeneratorAgent).Name;

            this.SendSimpleCommand(ServerCommands.TERMINATE);
            Thread.Sleep(100);

            try
            {
                if (!generatorProcess.HasExited)
                {
                    generatorProcess.Kill();
                }
            }
            catch
            {
            }

            if (!generatorProcess.HasExited)
            {
                if (!generatorProcess.WaitForExit(100))
                {
                    DebugUtils.Break();
                }
            }

            RunningObjectTable.RevokeObject(name);

            exitCode = generatorProcess.ExitCode;
        }
    }
}
