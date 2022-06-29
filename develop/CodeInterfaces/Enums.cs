using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces
{
    public enum OperationDirection : int
    {
        Incoming,
        Outgoing
    }

    [Flags]
    public enum Modifiers : int
    {
        Unknown = 0,
        CanRead = 1,
        CanWrite = 2,
        IsLocal = 4,
        NotNavigable = 8,
        NotApplicable = 4096
    }

    public enum DefinitionKind : int 
    {
        Unknown = 0,
        Class = 1,
        Delegate = 2,
        Enumeration = 3,
        Interface = 4,
        Structure = 5,
        Constructor = -1,
        Event = -2,
        Field = -3,
        Method = -4,
        Operator = -5,
        Property = -6,
        NotApplicable = 100
    }

    public enum ProviderType : int
    {
        BusinessModel,
        ViewModel,
        EntityMap,
        AssemblyMap,
        ParsedCode,
        ResourceMap,
        WebService,
        CodeModel,
        ServerModel,
        ODATA,
        JSON,
        ASPX,
        XAML,
        URLDocument,
        HTML,
        XML,
        SQL,
        Custom
    }
}
