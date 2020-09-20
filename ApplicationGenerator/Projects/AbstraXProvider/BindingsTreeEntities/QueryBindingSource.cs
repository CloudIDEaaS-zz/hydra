using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using AbstraX.Bindings;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;

namespace AbstraX.BindingsTreeEntities
{
    [DataContract, DebuggerDisplay("{ DebugInfo }")]
    public class QueryBindingSource : BaseBindingsTreeNode, IQueryBindingSource
    {
        private IQueryBindingSource bindingSourceInternal;

        public QueryBindingSource()
        {
        }

        public QueryBindingSource(IQueryBindingSource bindingSource, BaseBindingsTreeNode parentNode)
        {
            bindingSourceInternal = bindingSource;
            this.ParentNode = parentNode;
        }

        [Exclude]
        public override object InternalObject
        {
            get
            {
                return bindingSourceInternal;
            }
        }

        [Exclude]
        public IQuery Query
        {
            get
            {
                return bindingSourceInternal.Query;
            }
            set
            {
                bindingSourceInternal.Query = value;
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

        [DataMember]
        public override string DebugInfo
        {
            get
            {
                return this.GetDebugInfo(bindingSourceInternal.Query.Name);
            }
        }
    }
}
