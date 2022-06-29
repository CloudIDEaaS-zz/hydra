using System;
using System.Collections.Generic;
using AbstraX.ServerInterfaces;
using AbstraX.XPathBuilder;
using System.Linq.Expressions;
using System.Linq;
using CodeInterfaces.XPathBuilder;

namespace AbstraX.AssemblyInterfaces
{
    public interface IMethodOperation
    {
        IEnumerable<IElement> ChildElements { get; }
        float ChildOrdinal { get; }
        event ChildrenLoadedHandler ChildrenLoaded;
        void ClearPredicates();
        string DebugInfo { get; }
        string DesignComments { get; }
        bool HasDocumentation { get; }
        OperationDirection Direction { get; }
        string DocumentationSummary { get; }
        string Documentation { get; }
        IQueryable ExecuteWhere(XPathAxisElement element);
        IQueryable ExecuteWhere(Expression expression);
        IQueryable ExecuteWhere(string property, object value);
        string FolderKeyPair { get; set; }
        bool HasChildren { get; }
        string ImageURL { get; }
        bool IsConstructor { get; }
        DefinitionKind Kind { get; }
        System.Reflection.MethodInfo Method { get; set; }
        string Name { get; }
        IBase Parent { get; }
        string ParentID { get; }
        Facet[] Facets { get; }
        BaseType ReturnType { get; }
        Modifiers Modifiers { get; }
    }
}
