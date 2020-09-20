using System;
using CodeInterfaces;
using CodeInterfaces.XPathBuilder;
using System.Linq.Expressions;
using System.Linq;

namespace CodeInterfaces.AssemblyInterfaces
{
    public interface IOperatorOperation
    {
        System.Collections.Generic.IEnumerable<IElement> ChildElements { get; }
        float ChildOrdinal { get; }
        event ChildrenLoadedHandler ChildrenLoaded;
        void ClearPredicates();
        System.Reflection.ConstructorInfo Constructor { get; set; }
        string DebugInfo { get; }
        string DesignComments { get; }
        bool HasDocumentation { get; }
        OperationDirection Direction { get; }
        string Documentation { get; }
        string DocumentationSummary { get; }
        IQueryable ExecuteWhere(XPathAxisElement element);
        IQueryable ExecuteWhere(Expression expression);
        IQueryable ExecuteWhere(string property, object value);
        string FolderKeyPair { get; set; }
        bool HasChildren { get; }
        string ImageURL { get; }
        bool IsConstructor { get; }
        DefinitionKind Kind { get; }
        string Name { get; }
        IBase Parent { get; }
        string ParentID { get; }
        Facet[] Facets { get; }
        Modifiers Modifiers { get; }
    }
}
