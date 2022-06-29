using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace BuildTasks.Utility
{
    public class DllExportCommandHandler : BaseWindowsCommandHandler
    {
        public DllExportCommandHandler(string exe) : base(exe)
        {
        }

        public void RunEmptyCommand()
        {
            base.RunCommand(null, null, null);
        }

        public void RunEmptyAsAutomated()
        {
            base.RunCommand(null, null, "-runAsAutomated");
        }


        public void RunAsAutomated(string workingDirectory)
        {
            base.RunCommand(null, workingDirectory, "-runAsAutomated");
        }

        public void RunEmptyCommand(string workingDirectory)
        {
            base.RunCommand(null, workingDirectory, null);
        }

        public void RunForStdio(params string[] arguments)
        {
            base.LaunchForStdio(Environment.CurrentDirectory, arguments);
        }

        public void RunForStdio(string currentDirectory, params string[] arguments)
        {
            base.LaunchForStdio(currentDirectory, arguments);
        }
    }
}
