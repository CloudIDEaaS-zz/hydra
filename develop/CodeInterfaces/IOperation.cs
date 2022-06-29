using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces
{
    public interface IOperation : IParentBase
    {
        OperationDirection Direction { get; }
        BaseType ReturnType { get; }
    }
}
