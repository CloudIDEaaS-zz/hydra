using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AbstraX.Models.Interfaces;
using AbstraX.ServerInterfaces;
using AbstraX.TypeMappings;
using AbstraX.XPathBuilder;

namespace AbstraX.Models.Interfaces
{
    public interface IModel : IElement
    {
        List<IEntityContainer> Containers { get; }
        string Namespace { get; }
    }
}