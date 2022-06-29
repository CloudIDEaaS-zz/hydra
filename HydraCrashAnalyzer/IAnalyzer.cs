using System;
using Utils;

namespace HydraCrashAnalyzer
{
    public interface IAnalyzer
    {
        void SetDebugInfo(DebugState debugState, string dumpFile);
        void SetDumpFileCreated();
        void SetInternalException(Exception ex);
    }
}