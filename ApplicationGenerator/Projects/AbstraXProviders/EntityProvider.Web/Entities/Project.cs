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
    [DataContract, NodeImage("EntityProvider.Web.Images.Project.png"), DebuggerDisplay("{ DebugInfo }"), ClientCodeGeneration(typeof(AbstraXClientInterfaceGenerator))]
    public class Project : EntitiesBase, IElement
    {
        private IAbstraXService service;
        private string name;
        private string url;
        private string queryWhereProperty;
        private object queryWhereValue;
        private IVSProject project;
        private Solution parent;
        protected IProviderEntityService providerEntityService;

        public Project()
        {
            providerEntityService = ((IBase)parent).ProviderEntityService;
        }

        public Project(IAbstraXService service)
        {
            this.service = service;

            providerEntityService = ((IBase)parent).ProviderEntityService;
        }

        public Project(IVSProject project, Solution parent)
        {
            this.project = project;
            this.parent = parent;

            name = project.Name;

            providerEntityService = ((IBase)parent).ProviderEntityService;
        }

        [AbstraXExtensionAttribute("IEntityProviderExtension", "EntityProviderExtension")]
        public IAbstraXExtension LoadExtension()
        {
            return null;
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
        public string DefaultNamespace
        {
            get
            {
                return project.DefaultNamespace;
            }
        }

        public IEnumerable<IAttribute> Attributes
        {
            get { return null; }
        }

        [Association("Project_Models", "ID", "ParentID")]
        public List<Model> Models
        {
            get
            {
                var models = new List<Model>();

                try
                {
                    if (queryWhereProperty != null && queryWhereProperty == "Model")
                    {
                        var model = project.EDMXModels.Where(pi => pi.Name == (string) queryWhereValue).Single();

                        Debug.Assert(model != null);

                        models.Add(new Model(model, this));
                    }
                    else
                    {
                        foreach (IVSProjectItem model in project.EDMXModels)
                        {
                            models.Add(new Model(model, this));
                        }
                    }
                }
                catch
                {
                }

                return models;
            }
        }
        public IEnumerable<IElement> ChildElements
        {
            get 
            {
                var elements = this.Models.AsQueryable().Cast<IElement>().Select(e => e);

                return elements;
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

        public IEnumerable<IOperation> Operations
        {
            get { return null; }
        }

        [DataMember, Key]
        public string ID
        {
            get
            {
                return this.MakeID("Project='" + project.Name + "'");
            }
        }

        [DataMember]
        public string FolderKeyPair { get; set; }

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
                return name;
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

        public IQueryable ExecuteWhere(AbstraX.XPathBuilder.XPathAxisElement element)
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
                return this.Models.Any();
            }
        }

        [DataMember]
        public string DesignComments
        {
            get { return string.Empty; }
        }

        [DataMember]
        public string Documentation
        {
            get { return string.Empty; }
        }

        [DataMember]
        public bool HasDocumentation
        {
            get { return false; }
            set { }
        }

        [DataMember]
        public string DocumentationSummary
        {
            get { return string.Empty; }
            set { }
        }

        public IBase Parent
        {
            get 
            {
                return this.parent;
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
