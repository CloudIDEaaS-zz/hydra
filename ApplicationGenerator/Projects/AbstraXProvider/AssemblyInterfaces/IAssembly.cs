using System;
using System.Collections.Generic;
using AbstraX.ServerInterfaces;
using AbstraX.XPathBuilder;
using System.Linq.Expressions;
using System.Linq;

namespace AbstraX.AssemblyInterfaces
{
    public interface IAssembly 
    {
        IEnumerable<IAttribute> Attributes { get; }
        BaseType DataType { get; }
        IEnumerable<IElement> ChildElements { get; }
        float ChildOrdinal { get; }
        void ClearPredicates();
        string DebugInfo { get; }
        string DesignComments { get; }
        bool HasDocumentation { get; }
        string DocumentationSummary { get; }
        string Documentation { get; }
        IQueryable ExecuteWhere(XPathAxisElement element);
        IQueryable ExecuteWhere(Expression expression);
        IQueryable ExecuteWhere(string property, object value);
        Facet[] Facets { get; }
        string FolderKeyPair { get; set; }
        bool HasChildren { get; }
        string ID { get; }
        string ImageURL { get; }
        bool IsContainer { get; }
        DefinitionKind Kind { get; }
        string Name { get; }
        IEnumerable<IOperation> Operations { get; }
        IBase Parent { get; }
        string ParentID { get; }
        IEnumerable<IAssemblyType> Types { get; }
        Modifiers Modifiers { get; }
    }
}
