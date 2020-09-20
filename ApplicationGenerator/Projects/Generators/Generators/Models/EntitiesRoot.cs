using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AbstraX.ServerInterfaces;
using System.Runtime.Serialization;
using System.Diagnostics;
using AbstraX.XPathBuilder;
using AbstraX;

namespace EntityProvider.Web.Entities
{
    public class EntitiesRoot : EntitiesBase, IRoot
    {
        private IAbstraXService service;
        private string queryWhereProperty;
        private object queryWhereValue;

        public EntitiesRoot()
        {
        }

        public EntitiesRoot(IAbstraXService service)
        {
        }

        public float ChildOrdinal
        {
            get
            {
                return 0;
            }
        }

        public string DebugInfo
        {
            get
            {
                return this.GetDebugInfo();
            }
        }

        public string ID
        {
            get
            {
                return this.MakeID("URL='" + "'");
            }
        }

        public string FolderKeyPair { get; set; }

        public IEnumerable<IElement> RootElements
        {
            get 
            {
                return null;
            }
        }

        public string ParentFieldName
        {
            get
            {
                return "parentID";
            }
        }

        public ProviderType ProviderType
        {
            get
            {
                return ProviderType.EntityMap;
            }
        }

        public BaseType DataType
        {
            get
            {
                return null;
            }
        }

        public void ExecuteGlobalWhere(AbstraX.XPathBuilder.XPathAxisElement element)
        {
        }

        public IQueryable ExecuteWhere(string property, object value)
        {
            ClearPredicates();

            queryWhereProperty = property;
            queryWhereValue = value;

            return this.RootElements.AsQueryable();
        }

        public IQueryable ExecuteWhere(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable ExecuteWhere(XPathAxisElement element)
        {
            var predicate = element.Predicates.First();

            queryWhereProperty = predicate.Left;
            queryWhereValue = predicate.Right;

            return this.RootElements.AsQueryable();
        }

        public void ClearPredicates()
        {
            queryWhereProperty = null;
            queryWhereValue = null;
        }

        public void Dispose()
        {
        }

        public string ParentID
        {
            get
            {
                return null;
            }
        }

        public string DesignComments
        {
            get
            {
                return string.Empty;
            }
        }

        public string Documentation
        {
            get
            {
                return string.Empty;
            }
        }

        public bool HasDocumentation
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public string DocumentationSummary
        {
            get
            {
                return string.Empty;
            }
            set
            {

            }
        }

        public IBase Parent
        {
            get
            {
                return null;
            }
        }

        public DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.NotApplicable;
            }
        }

        public bool HasChildren
        {
            get 
            {
                return false; // this.Solutions.Any();
            }
        }

        public string Name
        {
            get
            {
                return "Entities";
            }
        }

        public Facet[] Facets
        {
            get
            {
                return null;
            }
        }

        public Modifiers Modifiers
        {
            get
            {
                return Modifiers.Unknown;
            }
        }

        public bool GetItemsOfType<T>(out bool doStandardTraversal, out int traversalDepthLimit, out IEnumerable<T> elements)
        {
            elements = null;
            traversalDepthLimit = -1;

            switch (typeof(T).Name)
            {
                case "Solution":
                case "Project":
                case "Model":
                case "EntityType":

                    traversalDepthLimit = 6;
                    doStandardTraversal = true;
                    return true;

                default:

                    doStandardTraversal = false;
                    return false;
            }
        }
    }
}
