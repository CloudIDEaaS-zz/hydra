using AbstraX.CommandHandlers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;
using Utils.ProcessHelpers;

namespace AbstraX.Handlers.ProcessHandlers
{
    [ProcessHandler(ProcessHandlerKind.RunningProcesses)]
    public class WindowsProcessHandler : IProcessHandler
    {
        private bool enableDebuggerForNode;
        Dictionary<int, Process> debuggerEnabledProcesses;
        public event ProcessEventHandler ProcessEvent;
        private ILogWriter logWriter;

        public WindowsProcessHandler()
        {
            enableDebuggerForNode = bool.Parse(ConfigurationSettings.AppSettings["EnableDebuggerForNode"]);
            debuggerEnabledProcesses = new Dictionary<int, Process>();
        }

        public Process[] Find(string workingDirectory)
        {
            var list = new List<Process>();
            var processList = new[] { "node", "ApplicationGenerator", };
            var processes = Process.GetProcesses().Where(p => p.ProcessName.IsOneOf(processList));

            foreach (var process in processes)
            {
                list.Add(process);
            }

            return list.ToArray();
        }

        public void Kill(Process[] processes)
        {
        }

        public void ReportProcess(Process process, ILogWriter logWriter)
        {
            if (debuggerEnabledProcesses.ContainsKey(process.Id))
            {
                return;
            }

            this.logWriter = logWriter;

            if (enableDebuggerForNode && process.ProcessName == "node")
            {
                EnableDebuggerForNode(process);
                debuggerEnabledProcesses.Add(process.Id, process);
            }
        }

        private void EnableDebuggerForNode(Process process)
        {
            var thread = new Thread(() =>
            {
                var nodeCommandHandler = new NodeCommandHandler();
                string error = null;

                process.EnableRaisingEvents = true;
                process.Exited += (sender, e) =>
                {
                    ProcessEvent(this, new ProcessEventArgs(process, null));
                };

                nodeCommandHandler.OutputWriteLine += (format, parms) =>
                {
                    logWriter.WriteLine(format, parms);
                };

                nodeCommandHandler.ErrorWriteLine += (format, parms) =>
                {
                    error = string.Format(format, parms);

                    logWriter.WriteLine(format, parms);
                };

                nodeCommandHandler.DebugProcess(process);

                nodeCommandHandler.Wait();

                if (error == null)
                {
                    ProcessEvent(this, new ProcessEventArgs(process, $"Node debugger attached to process with id: { process.Id } "));
                }
            });

            thread.Priority = ThreadPriority.AboveNormal;

            thread.Start();
        }
    }
}
