namespace Microsoft.Build.CommandLine
{
    using Microsoft.Build.Shared;
    using System;

    internal sealed class CommandLineSwitchException : Exception
    {
        private string commandLineArg;

        private CommandLineSwitchException(string message) : base(message)
        {
        }

        private CommandLineSwitchException(string message, string commandLineArg) : this(message)
        {
            this.commandLineArg = commandLineArg;
        }

        internal static void Throw(string messageResourceName, string commandLineArg)
        {
            Throw(messageResourceName, commandLineArg, new string[] { string.Empty });
        }

        internal static void Throw(string messageResourceName, string commandLineArg, params string[] messageArgs)
        {
            string message = Microsoft.Build.Shared.ResourceUtilities.FormatResourceString(messageResourceName, messageArgs);
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrow(message != null, "The resource string must exist.");
            throw new CommandLineSwitchException(message, commandLineArg);
        }

        internal static void VerifyThrow(bool condition, string messageResourceName, string commandLineArg)
        {
            if (!condition)
            {
                Throw(messageResourceName, commandLineArg);
            }
        }

        internal string CommandLineArg
        {
            get
            {
                return this.commandLineArg;
            }
        }

        public override string Message
        {
            get
            {
                if (this.commandLineArg == null)
                {
                    return base.Message;
                }
                return (base.Message + Environment.NewLine + Microsoft.Build.Shared.ResourceUtilities.FormatResourceString("InvalidSwitchIndicator", new object[] { this.commandLineArg }));
            }
        }
    }
}

