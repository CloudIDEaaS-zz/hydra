using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Utils;

namespace ApplicationGenerator.Data
{
    internal class AbstraXProviderLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return DebugUtils.BreakReturnNull();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Debug.WriteLine(state.ToString());
        }
    }
}