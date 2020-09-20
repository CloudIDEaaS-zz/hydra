using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using AbstraX.ServerInterfaces;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.ServiceModel.DomainServices.Server;
using AbstraX.XPathBuilder;
using AbstraX.Contracts;
using AbstraX.Templates;
using AbstraX;
using AbstraX.TypeMappings;

namespace EntityProvider.Web.Entities
{
    [DataContract, NodeImage("EntityProvider.Web.Images.Model.png"), DebuggerDisplay("{ DebugInfo }"), ClientCodeGeneration(typeof(AbstraXClientInterfaceGenerator))]
    public class Model : EntitiesBase, IElement
    {
        private IAbstraXService service;
        private IVSProjectItem model;
        private string url;
        private string queryWhereProperty;
        private object queryWhereValue;
        private Project parent;
        protected IProviderEntityService providerEntityService;

        public Model()
        {
        }

        public Model(IAbstraXService service)
        {
            this.service = service;
            providerEntityService = ((IBase)parent).ProviderEntityService;
        }

        public Model(IVSProjectItem model, Project parent)
        {
            this.model = model;
            this.url = model.Name;

            this.EdmxDocument = new XmlDocument();
            this.EdmxDocument.LoadXml(model.GetFileContents<string>());

            this.NamespaceManager = new XmlNamespaceManager(this.EdmxDocument.NameTable);

            this.NamespaceManager.AddNamespace("edmx", "http://schemas.microsoft.com/ado/2008/10/edmx");
            this.NamespaceManager.AddNamespace("s", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl");
            this.NamespaceManager.AddNamespace("e", "http://schemas.microsoft.com/ado/2008/09/edm");

            this.parent = parent;
            providerEntityService = ((IBase)parent).ProviderEntityService;
        }

        [Exclude]
        IProviderEntityService IBase.ProviderEntityService
        {
            get
            {
                return providerEntityService;
            }
        }

        [AbstraXExtensionAttribute("IEntityProviderExtension", "EntityProviderExtension")]
        public IAbstraXExtension LoadExtension()
        {
            return null;
        }

        public IEnumerable<IAttribute> Attributes
        {
            get { return null; }
        }

        [Association("Model_Containers", "ID", "ParentID")]
        public List<Entity_Container> Containers
        {
            get
            {
                var nameSpaces = new List<Entity_Container>();

                try
                {
                    if (queryWhereProperty != null && queryWhereProperty == "Namespace")
                    {
                        var node = this.EdmxDocument.SelectSingleNode("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema/e:EntityContainer/@Name='" + (string)queryWhereValue + "'", this.NamespaceManager);

                        Debug.Assert(node != null);

                        nameSpaces.Add(new Entity_Container(this.EdmxDocument, this.NamespaceManager, node.Value, node.SelectSingleNode("parent::*"), this));
                    }
                    else
                    {
                        foreach (XmlNode node in this.EdmxDocument.SelectNodes("/edmx:Edmx/edmx:Runtime/edmx:ConceptualModels/e:Schema/e:EntityContainer", this.NamespaceManager))
                        {
                            nameSpaces.Add(new Entity_Container(this.EdmxDocument, this.NamespaceManager, node.Attributes["Name"].Value, node, this));
                        }
                    }
                }
                catch
                {
                }

                return nameSpaces;
            }
        }

        [DataMember]
        public IEnumerable<IElement> ChildElements
        {
            get 
            {
                var elements = this.Containers.AsQueryable().Cast<IElement>().Select(e => e);

                return elements;
            }
        }

        public IEnumerable<IOperation> Operations
        {
            get { return null; }
        }

        [DataMember, Key]
        public string ID
        {
            get
            {
                return this.MakeID("Model='" + model.Name + "'");
            }
        }

        [DataMember]
        public string ParentID
        {
            get
            {
                return parent.ID;
            }
        }

        [DataMember]
        public string Name
        {
            get 
            {
                return model.Name;
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

        [DataMember]
        public bool IsContainer
        {
            get 
            {
                return true;
            }
        }

        [Include, Association("Parent_BaseType", "ID", "ParentID")]
        public BaseType DataType
        {
            get
            {
                return null;
            }
        }

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
                System.Diagnostics.Debugger.Break();

                queryWhereProperty = property;
                queryWhereValue = value;
            }

            return this.ChildElements.AsQueryable();
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

            return this.ChildElements.AsQueryable();
        }

        public void ClearPredicates()
        {
            queryWhereProperty = null;
            queryWhereValue = null;
        }

        public IRoot Root
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

        public IBase Parent
        {
            get
            {
                return parent;
            }
        }

        [DataMember]
        public string FolderKeyPair { get; set; }

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
                return this.Containers.Any();
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

        [Exclude]
        public ContainerType AllowableContainerTypes
        {
            get { throw new NotImplementedException(); }
        }

        [Exclude]
        public ConstructType AllowableConstructTypes
        {
            get { throw new NotImplementedException(); }
        }

        [Exclude]
        public ContainerType DefaultContainerType
        {
            get { throw new NotImplementedException(); }
        }

        [Exclude]
        public ConstructType DefaultConstructType
        {
            get { throw new NotImplementedException(); }
        }

        [DataMember]
        public Modifiers Modifiers
        {
            get
            {
                return Modifiers.Unknown;
            }
        }
    }
}
