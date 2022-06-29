// file:	Models\Assemblies\Assembly.cs
//
// summary:	Implements the assembly class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.ServerInterfaces;
using AbstraX;
using System.ComponentModel.DataAnnotations;
using Reflection = System.Reflection;
using E = System.Linq.Expressions;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using AbstraX.XPathBuilder;
using System.Diagnostics;
using AbstraX.TypeMappings;
using AbstraX.AssemblyInterfaces;
using CodeInterfaces.XPathBuilder;
using AbstraX.Models;
using Utils;

namespace AssemblyProvider.Web.Entities
{
    /// <summary>   An assembly. </summary>
    ///
    /// <remarks>   Ken, 10/30/2020. </remarks>

    public class Assembly : BaseObject, IElement, IAssembly
    {
        /// <summary>   The assembly file. </summary>
        private string assemblyFile;
        /// <summary>   The assembly. </summary>
        private Reflection.Assembly assembly;
        /// <summary>   The query where property. </summary>
        private string queryWhereProperty;
        /// <summary>   The query where value. </summary>
        private object queryWhereValue;
        /// <summary>   A filter specifying the types. </summary>
        private Func<Type, bool> typesFilter;

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>
        ///
        /// <param name="assembly">     The assembly. </param>
        /// <param name="file">         The file. </param>
        /// <param name="typesFilter">  (Optional) A filter specifying the types. </param>

        public Assembly(Reflection.Assembly assembly, string file, Func<Type, bool> typesFilter = null)
        {
            this.assembly = assembly;
            this.assemblyFile = file;
            this.typesFilter = typesFilter;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>
        ///
        /// <param name="assembly">     The assembly. </param>
        /// <param name="typesFilter">  (Optional) A filter specifying the types. </param>

        public Assembly(Reflection.Assembly assembly, Func<Type, bool> typesFilter = null)
        {
            this.assembly = assembly;
            this.typesFilter = typesFilter;
        }

        /// <summary>   Gets the namespace. </summary>
        ///
        /// <value> The namespace. </value>

        public string Namespace
        {
            get
            {
                var nameParts = AssemblyExtensions.GetNameParts(assembly.FullName);

                return nameParts.AssemblyName;
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

        /// <summary>   Gets or sets the identifier. </summary>
        ///
        /// <value> The identifier. </value>

        public override string ID
        {
            get 
            {
                if (assemblyFile != null)
                {
                    return this.MakeID("AssemblyFile='" + this.assemblyFile + "'");
                }
                else
                {
                    return this.MakeID("Assembly='" + this.assembly.FullName + "'");
                }
            }

            protected set
            {
            }
        }

        /// <summary>   Gets or sets the folder key pair. </summary>
        ///
        /// <value> The folder key pair. </value>

        public override string FolderKeyPair { get; set; }

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

        /// <summary>   Gets the attributes. </summary>
        ///
        /// <value> The attributes. </value>

        public IEnumerable<IAttribute> Attributes
        {
            get 
            {
                return null;
            }
        }

        /// <summary>   Gets the types. </summary>
        ///
        /// <value> The types. </value>

        [Association("Assembly_Types", "ID", "ParentID")]
        public List<AssemblyType> Types
        {
            get
            {
                var types = new List<AssemblyType>();

                try
                {
                    if (typesFilter != null)
                    {
                        foreach (System.Type type in assembly.GetTypes().Where(t => typesFilter(t))) 
                        {
                            types.Add(new AssemblyType(type, this));
                        }
                    }
                    else if (queryWhereProperty != null && queryWhereProperty == "Type")
                    {
                        var type = assembly.GetType((string)queryWhereValue);

                        Debug.Assert(type != null);

                        types.Add(new AssemblyType(type, this));
                    }
                    else
                    {
                        foreach (System.Type type in assembly.GetTypes().Where(t => t.IsPublic))
                        {
                            types.Add(new AssemblyType(type, this));
                        }
                    }
                }
                catch
                {
                }

                return types;
            }
        }

        /// <summary>   Gets the types no filter. </summary>
        ///
        /// <value> The types no filter. </value>

        public List<AssemblyType> TypesNoFilter
        {
            get
            {
                var types = new List<AssemblyType>();

                try
                {
                    foreach (System.Type type in assembly.GetTypes())
                    {
                        types.Add(new AssemblyType(type, this));
                    }
                }
                catch
                {
                }

                return types;
            }
        }

        /// <summary>   Gets the child elements. </summary>
        ///
        /// <value> The child elements. </value>

        public IEnumerable<IElement> ChildElements
        {
            get 
            {
                var elements = this.Types.AsQueryable().Cast<IElement>().Select(e => e);

                return elements;
            }
        }

        /// <summary>   Gets the operations. </summary>
        ///
        /// <value> The operations. </value>

        public IEnumerable<IOperation> Operations
        {
            get 
            {
                return null;
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

        /// <summary>   Gets URL of the image. </summary>
        ///
        /// <value> The image URL. </value>

        public string ImageURL
        {
            get 
            {
                return string.Empty;
            }
        }

        /// <summary>   Gets the name. </summary>
        ///
        /// <value> The name. </value>

        public override string Name
        {
            get
            {
                return assembly.FullName;
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

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>
        ///
        /// <param name="property"> The property. </param>
        /// <param name="value">    The value. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(string property, object value)
        {
            if (value is XPathAxisElement && (property == "ID" || property == "ParentID"))
            {
                var predicate = ((XPathAxisElement)value).Predicates.First();

                queryWhereProperty = predicate.Left;
                queryWhereValue = predicate.Right;
            }
            else
            {
                Debugger.Break();

                queryWhereProperty = property;
                queryWhereValue = value;
            }

            return this.ChildElements.AsQueryable();
        }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>
        ///
        /// <param name="expression">   The expression. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(Expression expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>
        ///
        /// <param name="element">  The element. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(XPathAxisElement element)
        {
            if (element.Predicates.Count > 0)
            {
                var predicate = element.Predicates.First();

                queryWhereProperty = predicate.Left;
                queryWhereValue = predicate.Right;
            }

            return this.ChildElements.AsQueryable();
        }

        /// <summary>   Clears the predicates. </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>

        public void ClearPredicates()
        {
            queryWhereProperty = null;
            queryWhereValue = null;
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

        /// <summary>   Gets the kind. </summary>
        ///
        /// <value> The kind. </value>

        public override DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.NotApplicable;
            }
        }

        /// <summary>   Gets a value indicating whether this  has children. </summary>
        ///
        /// <value> True if this  has children, false if not. </value>

        public override bool HasChildren
        {
            get 
            {
                return assembly.GetTypes().Any();
            }
        }

        /// <summary>   Gets the types. </summary>
        ///
        /// <value> The types. </value>

        IEnumerable<IAssemblyType> IAssembly.Types
        {
            get
            {
                foreach (var type in this.Types)
                {
                    yield return type;
                }
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
            get 
            {
                return ContainerType.NotSet;
            }
        }

        /// <summary>   Gets the default construct type. </summary>
        ///
        /// <value> The default construct type. </value>

        public ConstructType DefaultConstructType
        {
            get 
            {
                return ConstructType.NotSet;
            }
        }

        /// <summary>   Executes the where operation. </summary>
        ///
        /// <remarks>   Ken, 10/30/2020. </remarks>
        ///
        /// <param name="element">  The element. </param>
        ///
        /// <returns>   An IQueryable. </returns>

        public IQueryable ExecuteWhere(XPathElement element)
        {
            throw new NotImplementedException();
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

        /// <summary>   Gets the child nodes. </summary>
        ///
        /// <value> The child nodes. </value>

        public IEnumerable<IBase> ChildNodes => throw new NotImplementedException();
    }
}
 
