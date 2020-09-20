using AbstraX.ServerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Models.Interfaces
{
    public interface IElementWithFacetHanderTypes : IElement
    {
        IEnumerable<Type> GetFacetHandlerTypes();
        void RegisterFacetHandlerType<T>();
    }
}
