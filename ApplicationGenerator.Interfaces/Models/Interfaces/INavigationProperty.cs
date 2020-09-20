using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AbstraX.ServerInterfaces;
using AbstraX.TypeMappings;
using AbstraX.XPathBuilder;

namespace AbstraX.Models.Interfaces
{
    public interface INavigationProperty : IEntityParent, IRelationProperty, IEntityWithFacets
    {
        string DefaultValue { get; }
        List<IEntityType> ChildEntities { get; }
    }
}