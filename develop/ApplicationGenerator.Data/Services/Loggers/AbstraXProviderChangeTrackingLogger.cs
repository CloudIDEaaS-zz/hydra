using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Utils;

namespace ApplicationGenerator.Data
{
    public class AbstraXProviderChangeTrackingLogger : IDiagnosticsLogger<DbLoggerCategory.ChangeTracking>
    {
        public ILogger Logger
        {
            get
            {
                return new AbstraXProviderLogger();
            }
        }

        public ILoggingOptions Options
        {
            get
            {
                return new AbstraXProviderLoggingOptions();
            }
        }

        public DiagnosticSource DiagnosticSource
        {
            get
            {
                return new AbstraXProviderDiagnosticsSource();
            }
        }

        public IInterceptors Interceptors
        {
            get
            {
                return DebugUtils.BreakReturnNull();
            }
        }

        public LoggingDefinitions Definitions
        {
            get
            {
                return new AbstraXProviderLoggingDefinitions();
            }
        }

        public WarningBehavior GetLogBehavior(EventId eventId, LogLevel logLevel)
        {
            return WarningBehavior.Ignore;
        }

        public bool ShouldLogSensitiveData()
        {
            return true;
        }
    }
}