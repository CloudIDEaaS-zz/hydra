using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace Utils
{
    public abstract class BaseStandardStreamService : BaseThreadedService
    {
        protected string currentWorkingDirectory;
        protected StreamReader reader;
        protected StreamWriter outputWriter;
        protected StreamWriter errorWriter;
        protected abstract void HandleCommand(CommandPacket commandPacket);

        public BaseStandardStreamService() : base(ThreadPriority.Lowest, TimeSpan.FromMilliseconds(100), TimeSpan.MaxValue, TimeSpan.FromSeconds(15))
        {
        }

        public override void DoWork(bool stopping)
        {
            if (!stopping)
            {
                try
                {
                    var commandPacket = reader.ReadJsonCommand();
                    var threadTimer = new OneTimeThreadTimer(1);

                    if (commandPacket == null)
                    {
                        return;
                    }

                    threadTimer.Start(() =>
                    {
                        HandleCommand(commandPacket);
                    });
                }
                catch (Exception ex)
                {
                }
            }
        }

        public override void Stop()
        {
            reader.Dispose();

            outputWriter.Write("\r\n\r\n");

            outputWriter.Dispose();
            errorWriter.Dispose();

            base.Stop();
        }

        public void Start(Process parentProcess, string workingDirectory, Stream input = null, Stream output = null)
        {
            var error = Console.OpenStandardError();

            if (output == null)
            {
                output = Console.OpenStandardOutput();
            }

            if (input == null)
            {
                input = Console.OpenStandardInput();
            }

            currentWorkingDirectory = workingDirectory;
            reader = new StreamReader(input);
            outputWriter = new StreamWriter(output);
            errorWriter = new StreamWriter(error);

            parentProcess.Exited += (sender, e) =>
            {
                this.Stop();
                Environment.Exit(0);
            };

            base.Start();
        }
    }
}
