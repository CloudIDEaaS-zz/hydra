using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using AbstraX.ServerInterfaces;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;
using AbstraX.TypeMappings;
using AbstraX.Contracts;

namespace AbstraX.BindingsTreeEntities
{
    [DataContract, KnownType(typeof(BaseType)), DebuggerDisplay("{ DebugInfo }")]
    public class Element : BaseBindingsTreeNode, IElement
    {
        private IElement elementInternal;
        private ConstructType updatedConstructType;
        private ContainerType updatedContainerType;
        private bool isTransient;
        protected IProviderEntityService providerEntityService;

        public Element()
        {
            isTransient = true;
        }

        public Element(IElement element, BaseBindingsTreeNode parentNode)
        {
            elementInternal = element;
            this.ParentNode = parentNode;
            this.name = element.Name;
        }

        public void CloneFrom(Element element)
        {
            this.ElementInternal = element.ElementInternal;
            this.ParentNode = element.ParentNode;
        }

        [Exclude]
        public override object InternalObject
        {
            get
            {
                return elementInternal;
            }
        }

        [Exclude]
        IProviderEntityService IBase.ProviderEntityService
        {
            get
            {
                return providerEntityService;
            }
        }

        [Exclude()]
        public IElement ElementInternal
        {
            get 
            { 
                return elementInternal; 
            }
            
            set 
            { 
                elementInternal = value; 
            }
        }

        [Exclude]
        public ConstructType UpdatedConstructType
        {
            get
            {
                return updatedConstructType;
            }
        }

        [Exclude]
        public ContainerType UpdatedContainerType
        {
            get
            {
                return updatedContainerType;
            }
        }

        [DataMember]
        public bool IsContainer
        {
            get 
            {
                return false;
            }
        }

        [Include, Association("Parent_BaseType", "ID", "ParentID")]
        public BaseType DataType
        {
            get 
            {
                var baseType = new BaseType(elementInternal.DataType.UnderlyingType, this.ID);

                return baseType;
            }
        }

        [Exclude]
        public IEnumerable<IAttribute> Attributes
        {
            get { throw new NotImplementedException(); }
        }

        [Exclude]
        public IEnumerable<IOperation> Operations
        {
            get { throw new NotImplementedException(); }
        }

        [Exclude]
        public IEnumerable<IElement> ChildElements
        {
            get { throw new NotImplementedException(); }
        }

        [DataMember, Key]
        public override string ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        [DataMember]
        public override string ParentID
        {
            get
            {
                return this.ParentNode.ID;
            }
        }

        [DataMember]
        public override string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (isTransient)
                {
                    name = value;
                }
                else
                {
                    var dataContextObject = (DataContextObject)this.ParentNode.ParentNode;

                    name = value;
                    dataContextObject.DataContextObjectInternal.UpdatedName = value;
                }
            }
        }

        [DataMember]
        public override float ChildOrdinal
        {
            get
            {
                return 0;
            }
        }

        [DataMember]
        public string DesignComments
        {
            get 
            {
                return "";
            }
        }

        [DataMember]
        public string Documentation
        {
            get
            {
                return "";
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
                return "";
            }
            set
            {
                
            }
        }

        [DataMember]
        public string ImageURL
        {
            get 
            {
                return null;
            }
        }

        [DataMember]
        public override string DebugInfo
        {
            get
            {
                return this.GetDebugInfo(elementInternal.Name);
            }
        }

        [Exclude]
        public IBase Parent
        {
            get { throw new NotImplementedException(); }
        }

        [DataMember]
        public string FolderKeyPair
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        [Exclude]
        public DefinitionKind Kind
        {
            get { throw new NotImplementedException(); }
        }

        [DataMember]
        public override bool HasChildren
        {
            get 
            {
                return false;
            }
        }

        [DataMember]
        public ConstructType ConstructType 
        {
            get
            {
                if (this.ParentNode.ParentNode is DataContextObject)
                {
                    var dataContextObject = (DataContextObject)this.ParentNode.ParentNode;

                    return dataContextObject.DataContextObjectInternal.ConstructType;
                }
                else
                {
                    return ConstructType.NotSet;
                }
            }

            set
            {
                if (isTransient)
                {
                    updatedConstructType = value;
                }
                else
                {
                    var dataContextObject = (DataContextObject)this.ParentNode.ParentNode;

                    dataContextObject.DataContextObjectInternal.ConstructType = value;
                }
            }
        }

        [DataMember]
        public ContainerType ContainerType 
        {
            get
            {
                if (this.ParentNode.ParentNode is DataContextObject)
                {
                    var dataContextObject = (DataContextObject)this.ParentNode.ParentNode;

                    return dataContextObject.DataContextObjectInternal.ContainerType;
                }
                else
                {
                    return ContainerType.NotSet;
                }
            }

            set
            {
                if (isTransient)
                {
                    updatedContainerType = value;
                }
                else
                {
                    var dataContextObject = (DataContextObject)this.ParentNode.ParentNode;

                    dataContextObject.DataContextObjectInternal.ContainerType = value;
                }
            }
        }

        [DataMember]
        public ContainerType AllowableContainerTypes
        {
            get
            {
                return elementInternal.AllowableContainerTypes;
            }
        }

        [DataMember]
        public ConstructType AllowableConstructTypes
        {
            get
            {
                return elementInternal.AllowableConstructTypes;
            }
        }

        public IQueryable ExecuteWhere(string property, object value)
        {
            throw new NotImplementedException();
        }

        public IQueryable ExecuteWhere(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable ExecuteWhere(XPathBuilder.XPathAxisElement element)
        {
            throw new NotImplementedException();
        }

        public void ClearPredicates()
        {
            throw new NotImplementedException();
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

        public Facet[] Facets
        {
            get 
            {
                return null;
            }
        }

        [Exclude]
        public ContainerType DefaultContainerType
        {
            get 
            {
                return elementInternal.DefaultContainerType;
            }
        }

        [Exclude]
        public ConstructType DefaultConstructType
        {
            get 
            {
                return elementInternal.DefaultConstructType;
            }
        }

        public IAbstraXExtension LoadExtension()
        {
            throw new NotImplementedException();
        }

        public Modifiers Modifiers
        {
            get { throw new NotImplementedException(); }
        }
    }
}
