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
    public class Operation : BaseBindingsTreeNode, IOperation
    {
        private IOperation operationInternal;
        protected IProviderEntityService providerEntityService;

        public Operation()
        {
        }

        public Operation(IOperation operation, BaseBindingsTreeNode parentNode)
        {
            operationInternal = operation;
            this.ParentNode = parentNode;

            this.name = operation.Name;
        }

        [Exclude]
        public override object InternalObject
        {
            get
            {
                return operationInternal;
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
        public override float ChildOrdinal
        {
            get
            {
                return 0;
            }
        }

        [Exclude]
        public OperationDirection Direction
        {
            get { throw new NotImplementedException(); }
        }

        [Exclude]
        public IEnumerable<IElement> ChildElements
        {
            get { throw new NotImplementedException(); }
        }

        [DataMember]
        public string DesignComments
        {
            get { throw new NotImplementedException(); }
        }

        [DataMember]
        public string Documentation
        {
            get { throw new NotImplementedException(); }
        }

        [DataMember]
        public bool HasDocumentation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [DataMember]
        public string DocumentationSummary
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [DataMember]
        public string ImageURL
        {
            get { throw new NotImplementedException(); }
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
                throw new NotImplementedException();
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

        [DataMember]
        public override string DebugInfo
        {
            get
            {
                return this.GetDebugInfo(operationInternal.Name);
            }
        }

        public IAbstraXExtension LoadExtension()
        {
            throw new NotImplementedException();
        }

        public Facet[] Facets
        {
            get { throw new NotImplementedException(); }
        }

        public Modifiers Modifiers
        {
            get { throw new NotImplementedException(); }
        }

        public BaseType ReturnType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
