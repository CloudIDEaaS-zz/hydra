using System;
using System.Collections.Generic;
using System.Text;

namespace HydraDebugAssistant
{
    public static class Commands
    {
        public const string AttachToProcess = "AttachToProcess";
        public const string AttachedToProcess = "AttachedToProcess";
        public const string IsBreakpointSet = "IsBreakpointSet";
        public const string BreakpointSet = "BreakpointSet";
        public const string BreakpointNotSet = "BreakpointNotSet";
        public const string RetryAndBreak = "RetryAndBreak";
        public const string Continue = "Continue";
    }
}
