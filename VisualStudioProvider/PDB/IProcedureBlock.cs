using Pdb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Utils.Parsing;

namespace VisualStudioProvider.PDB
{
    public interface IProcedureBlock
    {
        DebugSymbol DebugSymbol { get; }
        string MemberName { get; }
        ProcessModule MsEnvModule { get; }
        IntPtr ReadBinaryAddress { get; }
        TypeInformation TypeInformation { get; }
    }
}