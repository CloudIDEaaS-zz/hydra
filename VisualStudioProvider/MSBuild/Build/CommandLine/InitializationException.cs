namespace Microsoft.Build.CommandLine
{
    using Microsoft.Build.Shared;
    using System;

    internal sealed class InitializationException : Exception
    {
        private string invalidSwitch;

        private InitializationException()
        {
        }

        private InitializationException(string message) : base(message)
        {
        }

        private InitializationException(string message, string invalidSwitch) : this(message)
        {
            this.invalidSwitch = invalidSwitch;
        }

        internal static void Throw(string message, string invalidSwitch)
        {
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrow(message != null, "The string must exist.");
            throw new InitializationException(message, invalidSwitch);
        }

        internal static void Throw(string messageResourceName, string invalidSwitch, Exception e, bool showStackTrace)
        {
            string unformatted = Microsoft.Build.Shared.AssemblyResources.GetString(messageResourceName);
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrow(unformatted != null, "The resource string must exist.");
            if (showStackTrace)
            {
                unformatted = unformatted + Environment.NewLine + e.ToString();
            }
            else
            {
                unformatted = Microsoft.Build.Shared.ResourceUtilities.FormatString(unformatted, new object[] { (e == null) ? string.Empty : e.Message });
            }
            Throw(unformatted, invalidSwitch);
        }

        internal static void VerifyThrow(bool condition, string messageResourceName)
        {
            VerifyThrow(condition, messageResourceName, null);
        }

        internal static void VerifyThrow(bool condition, string messageResourceName, string invalidSwitch)
        {
            if (!condition)
            {
                Throw(messageResourceName, invalidSwitch, null, false);
            }
        }

        public override string Message
        {
            get
            {
                if (this.invalidSwitch == null)
                {
                    return base.Message;
                }
                return (base.Message + Environment.NewLine + Microsoft.Build.Shared.ResourceUtilities.FormatResourceString("InvalidSwitchIndicator", new object[] { this.invalidSwitch }));
            }
        }
    }
}

