using System;
using System.Collections.Generic;
using System.Reflection;
using Utils;
using AssemblyAttributesService.JsonTypes;
using System.Diagnostics;

namespace AssemblyAttributesShim.Agent
{
    public interface IAssemblyAttributesAgent : IDisposable
    {
        event EventHandler Exited;
        event DataReceivedEventHandler ErrorDataReceived;
        AssemblyAttributesService.JsonTypes.AssemblyAttributesJson GetAssemblyAttributes(string fullName);
        bool Debug { get; set; }
        Guid HandlerId { get; set; }
    }
}
