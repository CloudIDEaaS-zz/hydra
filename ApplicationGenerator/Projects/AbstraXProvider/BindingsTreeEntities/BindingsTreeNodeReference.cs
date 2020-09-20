using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using AbstraX.Bindings;
using AbstraX.ServerInterfaces;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;

namespace AbstraX.BindingsTreeEntities
{
    [DataContract, DebuggerDisplay("{ DebugInfo }")]
    public class BindingsTreeNodeReference : BindingsTreeNode, IBindingsTreeNodeReference
    {
        private IBindingsTreeNodeReference bindingsTreeNodeReferenceInternal;
        public IElement ReferencedFrom { get; set; }

        public BindingsTreeNodeReference()
        {
        }

        public BindingsTreeNodeReference(IBindingsTreeNodeReference bindingsTreeNodeReference, BaseBindingsTreeNode parentNode)
        {
            bindingsTreeNodeReferenceInternal = bindingsTreeNodeReference;
            this.ParentNode = parentNode;
            this.name = bindingsTreeNodeReference.Name;
            // TODO - this.ReferencedFrom = b
        }

        [Exclude]
        public override object InternalObject
        {
            get
            {
                return bindingsTreeNodeReferenceInternal;
            }
        }

        [DataMember]
        public override string ParentID
        {
            get
            {
                return base.ParentNode.ID;
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

        [Exclude]
        public IEnumerable<IPropertyBinding> PropertyBindings
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
        public IEnumerable<IDataContextObject> DataContext
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

        [DataMember]
        public override string DebugInfo
        {
            get
            {
                return this.GetDebugInfo(bindingsTreeNodeReferenceInternal.Name);
            }
        }

        IBindingsTreeNode IBindingsTreeNode.ParentNode
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

        public override bool IsReference
        {
            get
            {
                return bindingsTreeNodeReferenceInternal.IsReference;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        [Exclude]
        public IBindingsTreeNode Node
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
    }
}
