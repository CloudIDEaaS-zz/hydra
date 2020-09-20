using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AbstraX.ServerInterfaces;
using AbstraX.TypeMappings;
using AbstraX.XPathBuilder;

namespace AbstraX.Models.Interfaces
{
    public interface IEntityContainer : IEntityWithFacets, IElement
    {
        List<IEntitySet> EntitySets { get; }
        bool PreventRecursion { get; }
    }
}