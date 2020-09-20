using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.ServerInterfaces;
using AbstraX.Contracts;
using System.Linq.Expressions;
using AbstraX.XPathBuilder;
using AbstraX.TypeMappings;

namespace AbstraX.BindingsTreeEntities
{
    public class SurrogateElement : ISurrogateElement
    {
        private IElement surrogateSource;
        private IElement referencedFromElement;

        public SurrogateElement(IElement surrogateSource, IElement referencedFromElement)
        {
            this.surrogateSource = surrogateSource;
            this.referencedFromElement = referencedFromElement;
        }

        public IElement SurrogateSource
        {
            get 
            {
                return surrogateSource;
            }
        }

        public IElement ReferencedFrom
        {
            get 
            {
                return referencedFromElement;
            }
        }

        public bool IsContainer
        {
            get
			{
                return surrogateSource.IsContainer;
			}
        }
            
        public BaseType DataType
        {
            get
			{
                return surrogateSource.DataType;
			}
        }

        public IEnumerable<IAttribute> Attributes
        {
            get
			{
                return surrogateSource.Attributes;
			}
        }

        public IEnumerable<IOperation> Operations
        {
            get
			{
                return surrogateSource.Operations;
			}
        }

        public ContainerType DefaultContainerType
        {
            get
			{
                return surrogateSource.DefaultContainerType;
			}
        }

        public ConstructType DefaultConstructType
        {
            get
			{
                return surrogateSource.DefaultConstructType;
			}
        }

        public ContainerType AllowableContainerTypes
        {
            get
			{
                return surrogateSource.AllowableContainerTypes;
			}
        }

        public ConstructType AllowableConstructTypes
        {
            get
			{
                return surrogateSource.AllowableConstructTypes;
			}
        }

        public IEnumerable<IElement> ChildElements
        {
            get
			{
                return surrogateSource.ChildElements;
			}
        }

        public IRoot Root
        {
            get
			{
                return surrogateSource.Root;
			}
        }

        public string ID
        {
            get
			{
                return surrogateSource.ID;
			}
        }

        public string ParentID
        {
            get
			{
                return surrogateSource.ParentID;
			}
        }

        public string Name
        {
            get
			{
                return surrogateSource.Name;
			}
        }

        public string DesignComments
        {
            get
			{
                return surrogateSource.DesignComments;
			}
        }

        public string Documentation
        {
            get
			{
                return surrogateSource.Documentation;
			}
        }

        public bool HasDocumentation
        {
            get
            {
                return surrogateSource.HasDocumentation;
            }
            
        }

        public string DocumentationSummary
        {
            get
            {
                return surrogateSource.DocumentationSummary;
            }
        }

        public string ImageURL
        {
            get
			{
                return surrogateSource.ImageURL;
			}
        }

        public float ChildOrdinal
        {
            get
			{
                return surrogateSource.ChildOrdinal;
			}
        }

        public string DebugInfo
        {
            get
			{
                return surrogateSource.DebugInfo;
			}
        }

        public IBase Parent
        {
            get
			{
                return ReferencedFrom.Parent;
			}
        }

        public string FolderKeyPair
        {
            get
            {
                return surrogateSource.FolderKeyPair;
            }
            set
            {
                surrogateSource.FolderKeyPair = value;
            }
        }

        public DefinitionKind Kind
        {
            get
			{
                return surrogateSource.Kind;
			}
        }

        public Modifiers Modifiers
        {
            get
			{
                return surrogateSource.Modifiers;
			}
        }

        public bool HasChildren
        {
            get
			{
                return surrogateSource.HasChildren;
			}
        }

        public Facet[] Facets
        {
            get
			{
                return surrogateSource.Facets;
			}
        }

        public IAbstraXExtension LoadExtension()
        {
            return surrogateSource.LoadExtension();
        }

        public IProviderEntityService ProviderEntityService
        {
            get
			{
                return surrogateSource.ProviderEntityService;
			}
        }

        public IQueryable ExecuteWhere(string property, object value)
        {
            return surrogateSource.ExecuteWhere(property, value);
        }

        public IQueryable ExecuteWhere(Expression expression)
        {
            return surrogateSource.ExecuteWhere(expression);
        }

        public IQueryable ExecuteWhere(XPathAxisElement element)
        {
            return surrogateSource.ExecuteWhere(element);
        }

        public void ClearPredicates()
        {
            surrogateSource.ClearPredicates();
        }
    }
}
