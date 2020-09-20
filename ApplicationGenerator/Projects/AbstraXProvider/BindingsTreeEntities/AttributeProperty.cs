using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.ServerInterfaces;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;
using AbstraX.Contracts;
using AbstraX;

namespace AbstraX.BindingsTreeEntities
{
    [DataContract, KnownType(typeof(ScalarType))]
    public class AttributeProperty : BaseBindingsTreeNode, IAttribute
    {
        private IAttribute attributeInternal;
        protected IProviderEntityService providerEntityService;

        public AttributeProperty()
        {
        }

        public AttributeProperty(IAttribute attribute, BaseBindingsTreeNode parentNode)
        {
            this.ParentNode = parentNode;

            if (attribute == null)
            {
                this.name = "<Null Attribute>";
            }
            else
            {
                this.name = attribute.Name;
                attributeInternal = attribute;
            }
        }

        public AttributeProperty(IAttribute attribute, string type, BaseBindingsTreeNode parentNode)
        {
            attributeInternal = attribute;
            this.ParentNode = parentNode;

            this.name = type + "=" + attribute.Name;
        }

        [Exclude]
        public override object InternalObject
        {
            get
            {
                return attributeInternal;
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

        [Include, Association("Parent_BaseType", "ID", "ParentID")]
        public ScalarType DataType
        {
            get
            {
                if (attributeInternal != null)
                {
                    return new ScalarType(attributeInternal.DataType, this);
                }
                else
                {
                    return null;
                }
            }
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
                name = value;
            }
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
        public override float ChildOrdinal
        {
            get 
            {
                return 0;
            }
        }

        [DataMember]
        public override string DebugInfo
        {
            get
            {
                return this.GetDebugInfo(attributeInternal.Name);
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

        public IAbstraXExtension LoadExtension()
        {
            throw new NotImplementedException();
        }


        public Modifiers Modifiers
        {
            get { throw new NotImplementedException(); }
        }

        [DataMember]
        public string DefaultValue
        {
            get 
            {
                return null;
            }
        }
    }
}
