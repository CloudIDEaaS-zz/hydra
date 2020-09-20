using System;
using AbstraX.ServerInterfaces;
using System.Collections.Generic;
using System.Reflection;
using AbstraX.XPathBuilder;
using System.Linq.Expressions;
using System.Linq;
using CodeInterfaces.XPathBuilder;

namespace AbstraX.AssemblyInterfaces
{
    public interface IPropertyElement
    {
        IEnumerable<IAttribute> Attributes { get; }
        BaseType DataType { get; }
        IEnumerable<AbstraX.ServerInterfaces.IElement> ChildElements { get; }
        float ChildOrdinal { get; }
        void ClearPredicates();
        string DebugInfo { get; }
        string DocumentationSummary { get; }
        string Documentation { get; }
        string DesignComments { get; }
        bool HasDocumentation { get; }
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
