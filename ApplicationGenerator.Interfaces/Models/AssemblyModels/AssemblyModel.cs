// file:	Models\AssemblyModels\AssemblyModel.cs
//
// summary:	Implements the assembly model class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Runtime.Serialization;
using AbstraX;
using AbstraX.TypeMappings;
using AbstraX.ServerInterfaces;
using System.IO;
using Utils;
using AbstraX.XPathBuilder;
using AbstraX.Models.Interfaces;

namespace AssemblyModelEntityProvider.Web.Entities
{
    /// <summary>   A data Model for the assembly. </summary>
    ///
    /// <remarks>   Ken, 11/1/2020. </remarks>

    [DebuggerDisplay(" { Name } ")]
    public class AssemblyModel : AssemblyModelEntityBase, IElement, IRootWithOptions, IModel
    {
        /// <summary>   The query where property. </summary>
        private string queryWhereProperty;
        /// <summary>   The query where value. </summary>
        private object queryWhereValue;
        /// <summary>   Name of the model. </summary>
        private string modelName;

        /// <summary>   Gets or sets the key value pairs. </summary>
        ///
        /// <value> The key value pairs. </value>

        public Dictionary<string, object> KeyValuePairs { get; set; }

        /// <summary>   Gets or sets the application generator engine. </summary>
        ///
        /// <value> The application generator engine. </value>

        public IAppGeneratorEngine AppGeneratorEngine { get; set; }

        /// <summary>   Gets the URL base. </summary>
        ///
        /// <value> The URL base. </value>

        public string UrlBase { get; }

        /// <summary>   Gets the variables. </summary>
        ///
        /// <value> The variables. </value>

        public Dictionary<string, object> Variables { get;  }

        /// <summary>   Gets or sets the namespace. </summary>
        ///
        /// <value> The namespace. </value>

        public new string Namespace { get; private set; }

        /// <summary>   Gets a value indicating whether this  is generated model. </summary>
        ///
        /// <value> True if this  is generated model, false if not. </value>

        public bool IsGeneratedModel { get; } = true;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="assembly">         The assembly. </param>
        /// <param name="typesProvider">    The types provider. </param>

        public AssemblyModel(AssemblyProvider.Web.Entities.Assembly assembly, ITypesProvider typesProvider)
        {
            modelName = assembly.Name;

            this.assembly = assembly;
            this.typesProvider = typesProvider;

            this.Namespace = assembly.Namespace;
            this.KeyValuePairs = new Dictionary<string, object>();
        }

        /// <summary>   Gets the attributes. </summary>
        ///
        /// <value> The attributes. </value>

        public IEnumerable<IAttribute> Attributes
        {
            get { return null; }
        }

        /// <summary>   Gets the containers. </summary>
        ///
        /// <value> The containers. </value>

        public List<AssemblyModelEntityContainer> Containers
        {
            get
            {
                var containers = new List<AssemblyModelEntityContainer>();

                try
                {
                    foreach (var type in this.assembly.Types)
                    {
                        containers.Add(new AssemblyModelEntityContainer(type, this.assembly, this.typesProvider, this));
                    }
                }
                catch
                {
                }

                return containers;
            }
        }

        /// <summary>   Gets the child elements. </summary>
        ///
        /// <value> The child elements. </value>

        public IEnumerable<IElement> ChildElements
        {
            get 
            {
                var elements = this.Containers.AsQueryable().Cast<IElement>().Select(e => e);

                return elements;
            }
        }

        List<IEntityContainer> IModel.Containers
        {
            get
            {
                return this.Containers.Cast<IEntityContainer>().ToList();
            }
        }

        /// <summary>   Gets the child nodes. </summary>
        ///
        /// <value> The child nodes. </value>

        public IEnumerable<IBase> ChildNodes
        {
            get
            {
                var nodes = this.Containers.AsQueryable().Cast<IBase>();

                return nodes;
            }
        }

        /// <summary>   Gets the operations. </summary>
        ///
        /// <value> The operations. </value>

        public IEnumerable<IOperation> Operations
        {
            get { return null; }
        }

        /// <summary>   Gets or sets the identifier. </summary>
        ///
        /// <value> The identifier. </value>

        public override string ID
        {
            get
            {
                return this.MakeID("Model='" + modelName + "'");
            }

            protected set
            {
            }
        }

        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>

        public override string Name
        {
            get 
            {
                return modelName;
            }
        }

        /// <summary>   Gets a value indicating whether this  is container. </summary>
        ///
        /// <value> True if this  is container, false if not. </value>

        public bool IsContainer
        {
            get 
            {
                return true;
            }
        }

        /// <summary>   Gets the type of the data. </summary>
        ///
        /// <value> The type of the data. </value>

        public BaseType DataType
        {
            get
            {
                return null;
            }
        }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="property"> The property. </param>
        /// <param name="value">    The value. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(string property, object value)
        {
            if (value is XPathElement && (property == "ID" || property == "ParentID"))
            {
                var predicate = ((XPathElement)value).Predicates.First();

                queryWhereProperty = predicate.Left.ToString();
                queryWhereValue = predicate.Right.ToString();
            }
            else
            {
                System.Diagnostics.Debugger.Break();

                queryWhereProperty = property;
                queryWhereValue = value;
            }

            return this.ChildElements.AsQueryable();
        }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="expression">   The expression. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="element">  The element. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(XPathElement element)
        {
            var predicate = element.Predicates.First();

            queryWhereProperty = predicate.Left.ToString();
            queryWhereValue = predicate.Right.ToString();

            return this.ChildElements.AsQueryable();
        }

        /// <summary>   Clears the predicates. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>

        public void ClearPredicates()
        {
            queryWhereProperty = null;
            queryWhereValue = null;
        }

        /// <summary>   Gets items of type. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <typeparam name="T">    Generic type parameter. </typeparam>
        /// <param name="doStandardTraversal">  [out] True to do standard traversal. </param>
        /// <param name="traversalDepthLimit">  [out] The traversal depth limit. </param>
        /// <param name="elements">             [out] The elements. </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>

        public bool GetItemsOfType<T>(out bool doStandardTraversal, out int traversalDepthLimit, out IEnumerable<T> elements)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Executes the global where operation. </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>
        ///
        /// <param name="element">  The element. </param>

        public void ExecuteGlobalWhere(XPathElement element)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ///
        /// <remarks>   Ken, 11/1/2020. </remarks>

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>   Gets the root. </summary>
        ///
        /// <value> The root. </value>

        public override IRoot Root
        {
            get
            {
                IBase baseObject = this;

                while (baseObject != null)
                {
                    baseObject = baseObject.Parent;

                    if (baseObject is IRoot)
                    {
                        return (IRoot)baseObject;
                    }
                }

                return null;
            }
        }

        /// <summary>   Gets the design comments. </summary>
        ///
        /// <value> The design comments. </value>

        public override string DesignComments
        {
            get
            {
                return string.Format("Type: {0}, Kind {1}, ID:'{2}'", this.GetType().Name, this.Kind, this.ID);
            }
        }

        /// <summary>   Gets the documentation. </summary>
        ///
        /// <value> The documentation. </value>

        public override string Documentation
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>   Gets a value indicating whether this  has documentation. </summary>
        ///
        /// <value> True if this  has documentation, false if not. </value>

        public override bool HasDocumentation
        {
            get
            {
                return false;
            }
        }

        /// <summary>   Gets the documentation summary. </summary>
        ///
        /// <value> The documentation summary. </value>

        public override string DocumentationSummary
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>   Gets the child ordinal. </summary>
        ///
        /// <value> The child ordinal. </value>

        public override float ChildOrdinal
        {
            get
            {
                return 0;
            }
        }

        /// <summary>   Gets information describing the debug. </summary>
        ///
        /// <value> Information describing the debug. </value>

        public override string DebugInfo
        {
            get
            {
                return this.GetDebugInfo();
            }
        }

        /// <summary>   Gets the parent. </summary>
        ///
        /// <value> The parent. </value>

        public override IBase Parent
        {
            get
            {
                return null;
            }
        }

        /// <summary>   Gets or sets the folder key pair. </summary>
        ///
        /// <value> The folder key pair. </value>

        public override string FolderKeyPair { get; set; }

        /// <summary>   Gets the kind. </summary>
        ///
        /// <value> The kind. </value>

        public override DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.Model;
            }
        }

        /// <summary>   Gets a value indicating whether this  has children. </summary>
        ///
        /// <value> True if this  has children, false if not. </value>

        public override bool HasChildren
        {
            get
            {
                return this.Containers.Any();
            }
        }

        /// <summary>   Gets the facets. </summary>
        ///
        /// <value> The facets. </value>

        public override Facet[] Facets
        {
            get
            {
                return null;
            }
        }

        /// <summary>   Gets a list of types of the allowable containers. </summary>
        ///
        /// <value> A list of types of the allowable containers. </value>

        public ContainerType AllowableContainerTypes
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>   Gets a list of types of the allowable constructs. </summary>
        ///
        /// <value> A list of types of the allowable constructs. </value>

        public ConstructType AllowableConstructTypes
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>   Gets the default container type. </summary>
        ///
        /// <value> The default container type. </value>

        public ContainerType DefaultContainerType
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>   Gets the default construct type. </summary>
        ///
        /// <value> The default construct type. </value>

        public ConstructType DefaultConstructType
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>   Gets the modifiers. </summary>
        ///
        /// <value> The modifiers. </value>

        public override Modifiers Modifiers
        {
            get
            {
                return Modifiers.Unknown;
            }
        }

        /// <summary>   Gets the identifier of the parent. </summary>
        ///
        /// <value> The identifier of the parent. </value>

        public override string ParentID
        {
            get
            {
                return null;
            }
        }

        /// <summary>   Gets the name of the parent field. </summary>
        ///
        /// <value> The name of the parent field. </value>

        public string ParentFieldName => throw new NotImplementedException();

        /// <summary>   Gets the type of the provider. </summary>
        ///
        /// <value> The type of the provider. </value>

        public ProviderType ProviderType => throw new NotImplementedException();

        /// <summary>   Gets the root elements. </summary>
        ///
        /// <value> The root elements. </value>

        public IEnumerable<IElement> RootElements => throw new NotImplementedException();
    }
}
