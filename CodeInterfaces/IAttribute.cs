using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces
{
    public interface IAttribute : IParentBase
    {
        ScalarType DataType { get; }
        string DefaultValue { get; }
    }
}
