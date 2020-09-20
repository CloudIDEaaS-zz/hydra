#if !SILVERLIGHT
using System;
using System.Net;

namespace AbstraX.TypeMappings
{
    [Flags]
    public enum ConstructType
    {
        NotSet = 0,
        Class = 1,
        Struct = 2,
        Enum = 4,
        Interface = 8
    }

    [Flags]
    public enum ContainerType
    {
        NotSet = 0,
        Construct = 1,
        Property = 2,
        ReducedToParent = 4
    }
}
#endif