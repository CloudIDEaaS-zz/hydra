// file:	Models\Assemblies\AssemblyInterfaces\IAssembly.cs
//
// summary:	Declares the IAssembly interface

using System;
using System.Collections.Generic;
using AbstraX.ServerInterfaces;
using AbstraX.XPathBuilder;
using System.Linq.Expressions;
using System.Linq;

namespace AbstraX.AssemblyInterfaces
{
    /// <summary>   Interface for assembly. </summary>
    ///
    /// <remarks>   Ken, 10/30/2020. </remarks>

    public interface IAssembly 
    {
        /// <summary>   Gets the attributes. </summary>
        ///
        /// <value> The attributes. </value>

        IEnumerable<IAttribute> Attributes { get; }

        /// <summary>   Gets the type of the data. </summary>
        ///
        /// <value> The type of the data. </value>

        BaseType DataType { get; }

        /// <summary>   Gets the child elements. </summary>
        ///
        /// <value> The child elements. </value>

        IEnumerable<IElement> ChildElements { get; }

        /// <summary>   Gets the child ordinal. </summary>
        ///
        /// <value> The child ordinal. </value>

        float ChildOrdinal { get; }
        /// <summary>   Clears the predicates. </summary>
        void ClearPredicates();

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        string DebugInfo { get; }

        /// <summary>   Gets the design comments. </summary>
        ///
        /// <value> The design comments. </value>

        string DesignComments { get; }

        /// <summary>   Gets a value indicating whether this  has documentation. </summary>
        ///
        /// <value> True if this  has documentation, false if not. </value>

        bool HasDocumentation { get; }

        /// <summary>   Gets the documentation summary. </summary>
        ///
        /// <value> The documentation summary. </value>

        string DocumentationSummary { get; }

        /// <summary>   Gets the documentation. </summary>
        ///
        /// <value> The documentation. </value>

        string Documentation { get; }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <param name="element">  The element. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        IQueryable ExecuteWhere(XPathElement element);

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <param name="expression">   The expression. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        IQueryable ExecuteWhere(Expression expression);

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <param name="property"> The property. </param>
        /// <param name="value">    The value. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        IQueryable ExecuteWhere(string property, object value);

        /// <summary>   Gets the facets. </summary>
        ///
        /// <value> The facets. </value>

        Facet[] Facets { get; }

        /// <summary>   Gets or sets the folder key pair. </summary>
        ///
        /// <value> The folder key pair. </value>

        string FolderKeyPair { get; set; }

        /// <summary>   Gets a value indicating whether this  has children. </summary>
        ///
        /// <value> True if this  has children, false if not. </value>

        bool HasChildren { get; }

        /// <summary>   Gets the identifier. </summary>
        ///
        /// <value> The identifier. </value>

        string ID { get; }

        /// <summary>   Gets URL of the image. </summary>
        ///
        /// <value> The image URL. </value>

        string ImageURL { get; }

        /// <summary>   Gets a value indicating whether this  is container. </summary>
        ///
        /// <value> True if this  is container, false if not. </value>

        bool IsContainer { get; }

        /// <summary>   Gets the kind. </summary>
        ///
        /// <value> The kind. </value>

        DefinitionKind Kind { get; }

        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>

        string Name { get; }

        /// <summary>   Gets the operations. </summary>
        ///
        /// <value> The operations. </value>

        IEnumerable<IOperation> Operations { get; }

        /// <summary>   Gets the parent. </summary>
        ///
        /// <value> The parent. </value>

        IBase Parent { get; }

        /// <summary>   Gets the identifier of the parent. </summary>
        ///
        /// <value> The identifier of the parent. </value>

        string ParentID { get; }

        /// <summary>   Gets the types. </summary>
        ///
        /// <value> The types. </value>

        IEnumerable<IAssemblyType> Types { get; }

        /// <summary>   Gets the modifiers. </summary>
        ///
        /// <value> The modifiers. </value>

        Modifiers Modifiers { get; }
    }
}
