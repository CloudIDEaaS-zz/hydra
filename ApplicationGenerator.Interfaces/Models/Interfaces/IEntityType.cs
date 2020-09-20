using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using AbstraX.ServerInterfaces;
using AbstraX.TypeMappings;
using AbstraX.XPathBuilder;

namespace AbstraX.Models.Interfaces
{
    public interface IEntityType : IElement, IEntityWithFacets
    {
        List<INavigationProperty> NavigationProperties { get; }
        List<IEntityProperty> Properties { get; }
    }
}