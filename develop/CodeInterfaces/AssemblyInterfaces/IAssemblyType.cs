using System;
using System.Collections.Generic;
using CodeInterfaces;
using CodeInterfaces.XPathBuilder;
using System.Linq.Expressions;
using System.Linq;

namespace CodeInterfaces.AssemblyInterfaces
{
    public interface IAssemblyType
    {
        IEnumerable<IAttribute> Attributes { get; }
        BaseType DataType { get; }
        Type SystemType { get; }
        IEnumerable<IAssemblyType> TypeTypes { get; }
        IEnumerable<IAssemblyType> BaseTypes { get; }
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
        IEnumerable<IFieldAttribute> FieldAttributes { get; }
        string FolderKeyPair { get; set; }
        bool HasChildren { get; }
        string ID { get; }
        string ImageURL { get; }
        bool IsContainer { get; }
        DefinitionKind Kind { get; }
        IEnumerable<IOperation> MethodsConstructors { get; }
        string Name { get; }
        IEnumerable<IOperation> Operations { get; }
        IBase Parent { get; }
        string ParentID { get; }
        IEnumerable<IPropertyAttribute> PropertyAttributes { get; }
        IEnumerable<IPropertyElement> PropertyElements { get; }
        Modifiers Modifiers { get; }
    }
}
