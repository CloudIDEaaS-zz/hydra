using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeInterfaces.TypeMappings;

namespace CodeInterfaces
{
    public interface IElement : IParentBase
    {
        bool IsContainer { get; }
        BaseType DataType { get; }
        IEnumerable<IAttribute> Attributes { get; }
        IEnumerable<IOperation> Operations { get; }
        ContainerType DefaultContainerType { get; }
        ConstructType DefaultConstructType { get; }
        ContainerType AllowableContainerTypes { get; }
        ConstructType AllowableConstructTypes { get; }
    }
}
