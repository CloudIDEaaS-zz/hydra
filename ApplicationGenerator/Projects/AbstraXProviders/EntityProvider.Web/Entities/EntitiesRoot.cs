using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AbstraX.ServerInterfaces;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;
using AbstraX.XPathBuilder;
using AbstraX.Contracts;
using AbstraX.Templates;
using AbstraX;

namespace EntityProvider.Web.Entities
{
    [DataContract, NodeImage("EntityProvider.Web.Images.EntitiesRoot.png"), DebuggerDisplay("{ DebugInfo }"), ClientCodeGeneration(typeof(AbstraXClientInterfaceGenerator))]
    public class EntitiesRoot : EntitiesBase, IRoot
    {
        private IAbstraXService service;
        private string queryWhereProperty;
        private object queryWhereValue;
        protected IProviderEntityService providerEntityService;

        public EntitiesRoot()
        {
        }

        public EntitiesRoot(IAbstraXService service)
        {
            this.service = service;
            providerEntityService = (IProviderEntityService)((IAbstraXProviderService)service).DomainServiceHostApplication;
        }

        [Exclude]
        IProviderEntityService IBase.ProviderEntityService
        {
            get
            {
                return providerEntityService;
            }
        }

        [DataMember]
        public float ChildOrdinal
        {
            get
            {
                return 0;
            }
        }

        [DataMember]
        public string DebugInfo
        {
            get
            {
                return this.GetDebugInfo();
            }
        }

        [DataMember, Key]
        public string ID
        {
            get
            {
                return this.MakeID("URL='" + this.URL + "'");
            }
        }

        [DataMember]
        public string FolderKeyPair { get; set; }

        [DataMember]
        public string URL
        {
            get 
            {
                return DNSHelper.MakeURL("EntitiesRoot");
            }
        }

        [DataMember, Association("EntitiesRoot_Solution", "ID", "ParentID")]
        public List<Solution> Solutions
        {
            get
            {
                List<Solution> solutions = new List<Solution>();

                foreach (IVSSolution solution in service.DomainServiceHostApplication.Solutions)
                {
                    solutions.Add(new Solution(solution, this));
                }

                return solutions;
            }
        }

        [DataMember]
        public IEnumerable<IElement> RootElements
        {
            get 
            {
                var elements = this.Solutions.AsQueryable().Cast<IElement>().Select(e => e);

                return elements;
            }
        }

        [DataMember]
        public string ParentFieldName
        {
            get
            {
                return "parentID";
            }
        }

        [DataMember]
        public ProviderType ProviderType
        {
            get
            {
                return ProviderType.EntityMap;
            }
        }

        [DataMember, Include, Association("Parent_BaseType", "ID", "ParentID")]
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

        [Invoke]
        public void Dispose()
        {
        }

        [DataMember]
        public string ParentID
        {
            get
            {
                return null;
            }
        }

        [DataMember]
        public string DesignComments
        {
            get
            {
                return string.Empty;
            }
        }

        [DataMember]
        public string Documentation
        {
            get
            {
                return string.Empty;
            }
        }

        [DataMember]
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

        [DataMember]
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

        [DataMember]
        public IBase Parent
        {
            get
            {
                return null;
            }
        }

        [DataMember]
        public DefinitionKind Kind
        {
            get
            {
                return DefinitionKind.NotApplicable;
            }
        }

        [DataMember]
        public bool HasChildren
        {
            get 
            {
                return this.Solutions.Any();
            }
        }

        [DataMember]
        public string Name
        {
            get
            {
                return "Entities";
            }
        }

        [DataMember]
        public string ImageURL
        {
            get
            {
                return string.Empty;
            }
        }

        [DataMember, Include, Association("Parent_Facet", "ID", "ParentID")]
        public Facet[] Facets
        {
            get
            {
                return null;
            }
        }

        [DataMember]
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
