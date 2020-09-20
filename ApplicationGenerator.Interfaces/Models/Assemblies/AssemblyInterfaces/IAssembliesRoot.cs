using System;
using System.Collections.Generic;
using AbstraX.ServerInterfaces;
using AbstraX.XPathBuilder;
using System.Linq.Expressions;
using System.Linq;
using CodeInterfaces.XPathBuilder;

namespace AbstraX.AssemblyInterfaces
{
    public interface IAssembliesRoot : IBase
    {
        IEnumerable<IAssembly> Assemblies { get; }
        BaseType DataType { get; }
        void ClearPredicates();
        void Dispose();
        void ExecuteGlobalWhere(XPathAxisElement element);
        IQueryable ExecuteWhere(XPathAxisElement element);
        IQueryable ExecuteWhere(Expression expression);
        IQueryable ExecuteWhere(string property, object value);
        Facet[] Facets { get; }
        string FolderKeyPair { get; set; }
        bool HasChildren { get; }
        string ID { get; }
        string ImageURL { get; }
        DefinitionKind Kind { get; }
        string Name { get; }
        IBase Parent { get; }
        string ParentFieldName { get; }
        string ParentID { get; }
        ProviderType ProviderType { get; }
        IEnumerable<IElement> RootElements { get; }
        string URL { get; }
        Modifiers Modifiers { get; }
    }
}
