using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Models.Interfaces
{
    public interface IEntityObjectWithDataType
    {
        BaseType DataType { get; }
        GetOverridesEventHandler OverrideEventHandler { get; }
        string Namespace { get; }
    }
}
