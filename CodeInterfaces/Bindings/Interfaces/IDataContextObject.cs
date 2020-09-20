using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if SILVERLIGHT
using CodeInterfaces.ClientInterfaces;
using CodeInterfaces.TypeMappings;
#else
using CodeInterfaces.TypeMappings;
#endif

namespace CodeInterfaces.Bindings
{
    public interface IDataContextObject
    {
        IElement ContextObject { get; set; }
        OptionalDataContextOperation SupportedOperations { get; set; }
        List<IOperation> RemoteCalls { get; set; }
        ConstructType ConstructType { get; set; }
        ContainerType ContainerType { get; set; }
        string UpdatedName { get; set; }
    }
}
