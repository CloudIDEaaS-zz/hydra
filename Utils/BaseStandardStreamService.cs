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
        public Action<string> JsonTextReadCallback { set; private get; }
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
                    var commandPacket = reader.ReadJsonCommand(this.JsonTextReadCallback);
                    var threadTimer = new OneTimeThreadTimer(1);

                    if (commandPacket == null)
                    {
                        return;
                    }
                    else if (processingHalted)
                    {
                        HandleCommand(commandPacket);
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

        protected override void HaltProcessing()
        {
            base.HaltProcessing();
        }

        public override void Stop()
        {
            reader.Dispose();

            outputWriter.Dispose();
            errorWriter.Dispose();

            base.Stop();
        }

        public void Start(Process parentProcess, string workingDirectory)
        {
            var output = Console.OpenStandardOutput();
            var input = Console.OpenStandardInput();
            var error = Console.OpenStandardError();

            currentWorkingDirectory = workingDirectory;
            reader = new StreamReader(input);
            outputWriter = new StreamWriter(output);
            errorWriter = new StreamWriter(error);

            parentProcess.Exited += (sender, e) =>
            {
                this.Stop();
            };

            base.Start();
        }
    }
}
