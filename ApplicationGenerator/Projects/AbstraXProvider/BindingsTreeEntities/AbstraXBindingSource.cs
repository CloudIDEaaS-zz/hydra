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
    public class AbstraXBindingSource : BaseBindingsTreeNode, IAbstraXBindingSource
    {
        private IAbstraXBindingSource bindingSourceInternal;
        private AttributeProperty attributeProperty;
        private NodeProperty isSearchableProperty;

        public AbstraXBindingSource()
        {
        }

        public AbstraXBindingSource(IAbstraXBindingSource bindingSource, BaseBindingsTreeNode parentNode)
        {
            bindingSourceInternal = bindingSource;
            this.ParentNode = parentNode;
            this.name = bindingSourceInternal.BindingAttribute.Name;
        }

        [Exclude]
        public override object InternalObject
        {
            get
            {
                return bindingSourceInternal;
            }
        }

        [Association("AbstraXBindingSource_IsSearchable", "ID", "ParentID")]
        public NodeProperty IsSearchableProperty
        {
            get
            {
                if (isSearchableProperty == null)
                {
                    isSearchableProperty = new NodeProperty("IsSearchable", bindingSourceInternal.IsSearchable.ToString(), this);
                }

                return isSearchableProperty;
            }
        }

        [Association("AbstraXBindingSource_BindingAttribute", "ID", "ParentID")]
        public AttributeProperty BindingAttributeProperty
        {
            get
            {
                if (attributeProperty == null)
                {
                    attributeProperty = new AttributeProperty(bindingSourceInternal.BindingAttribute, this);
                }

                return attributeProperty;
            }
        }

        [Exclude]
        public IAttribute BindingAttribute
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
        public bool IsSearchable
        {
            get
            {
                return bindingSourceInternal.IsSearchable;
            }
            set
            {
                throw new NotImplementedException();
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
        public override float ChildOrdinal
        {
            get 
            {
                return 0;
            }
        }

        [DataMember]
        public override bool HasChildren
        {
            get
            {
                return true;
            }
        }

        [DataMember]
        public override string DebugInfo
        {
            get
            {
                return this.GetDebugInfo(bindingSourceInternal.BindingAttribute.Name);
            }
        }
    }
}
