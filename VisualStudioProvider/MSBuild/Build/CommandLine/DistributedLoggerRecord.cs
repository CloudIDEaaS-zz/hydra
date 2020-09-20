namespace Microsoft.Build.CommandLine
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Logging;
    using System;

    internal class DistributedLoggerRecord
    {
        private ILogger centralLogger;
        private LoggerDescription forwardingLoggerDescription;

        internal DistributedLoggerRecord(ILogger centralLogger, LoggerDescription forwardingLoggerDescription)
        {
            this.centralLogger = centralLogger;
            this.forwardingLoggerDescription = forwardingLoggerDescription;
        }

        internal ILogger CentralLogger
        {
            get
            {
                return this.centralLogger;
            }
        }

        internal LoggerDescription ForwardingLoggerDescription
        {
            get
            {
                return this.forwardingLoggerDescription;
            }
        }
    }
}

