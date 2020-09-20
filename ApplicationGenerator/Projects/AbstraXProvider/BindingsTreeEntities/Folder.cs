using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;

namespace AbstraX.BindingsTreeEntities
{
    [DataContract, DebuggerDisplay("{ DebugInfo }")]
    public class Folder : BaseBindingsTreeNode
    {
        private List<BaseBindingsTreeNode> childNodes;

        public Folder()
        {
        }

        public Folder(BaseBindingsTreeNode parentNode, string name)
        {
            this.ParentNode = parentNode;
            this.name = name;
        }

        [Exclude]
        public override object InternalObject
        {
            get
            {
                return name;
            }
        }

        [Exclude]
        public List<BaseBindingsTreeNode> ChildNodes
        {
            get
            {
                if (childNodes == null)
                {
                    childNodes = new List<BaseBindingsTreeNode>();
                }

                return childNodes;
            }
        }

        [Association("Folder_BindingsTreeNodes", "ID", "ParentID")]
        public List<BindingsTreeNode> BindingsTreeNodes
        {
            get
            {
                return childNodes.Where(n => n is BindingsTreeNode && !((BindingsTreeNode) n).IsReference).Cast<BindingsTreeNode>().ToList();
            }
        }

        [Association("Folder_BindingsTreeNodeReferences", "ID", "ParentID")]
        public List<BindingsTreeNodeReference> BindingsTreeNodeReferences
        {
            get
            {
                return childNodes.Where(n => n is BindingsTreeNodeReference).Cast<BindingsTreeNodeReference>().ToList();
            }
        }

        [Association("Folder_Elements", "ID", "ParentID")]
        public List<Element> Elements
        {
            get
            {
                return childNodes.Where(n => n is Element).Cast<Element>().ToList();
            }
        }

        [Association("Folder_DataContextObjects", "ID", "ParentID")]
        public List<DataContextObject> ContextObjects
        {
            get
            {
                return childNodes.Where(n => n is DataContextObject).Cast<DataContextObject>().ToList();
            }
        }

        [Association("Folder_ContextObject", "ID", "ParentID")]
        public Element ContextObject
        {
            get
            {
                return (Element) childNodes.Single(n => n is Element);
            }
        }

        [Association("Folder_Operations", "ID", "ParentID")]
        public List<Operation> Operations
        {
            get
            {
                return childNodes.Where(n => n is Operation).Cast<Operation>().ToList();
            }
        }

        [Association("Folder_Properties", "ID", "ParentID")]
        public List<NodeProperty> Properties
        {
            get
            {
                return childNodes.Where(n => n is NodeProperty).Cast<NodeProperty>().ToList();
            }
        }

        [Association("Folder_PropertyBindings", "ID", "ParentID")]
        public List<PropertyBinding> PropertyBindings
        {
            get
            {
                return childNodes.Where(n => n is PropertyBinding).Cast<PropertyBinding>().ToList();
            }
        }

        [Association("Folder_AbstraXBindingSource", "ID", "ParentID")]
        public AbstraXBindingSource AbstraXBindingSource
        {
            get
            {
                if (childNodes.Any(n => n is AbstraXBindingSource))
                {
                    return (AbstraXBindingSource)childNodes.Single(n => n is AbstraXBindingSource);
                }
                else
                {
                    return null;
                }
            }
        }

        [Association("Folder_QueryBindingSources", "ID", "ParentID")]
        public QueryBindingSource QueryBindingSource
        {
            get
            {
                if (childNodes.Any(n => n is QueryBindingSource))
                {
                    return (QueryBindingSource)childNodes.Single(n => n is QueryBindingSource);
                }
                else
                {
                    return null;
                }
            }
        }

        [Association("Folder_SupportedOperations", "ID", "ParentID")]
        public List<NodeProperty> SupportedOperations
        {
            get
            {
                return childNodes.Where(n => n is NodeProperty).Cast<NodeProperty>().ToList();
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
                return this.ParentNode == null ? null : this.ParentNode.ID;
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
                return childNodes.Count > 0;
            }
        }

        [DataMember]
        public override string DebugInfo
        {
            get
            {
                return this.GetDebugInfo(name);
            }
        }
    }
}
