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
using AbstraX.TypeMappings;
using AbstraX;

namespace AbstraX.BindingsTreeEntities
{
    [DataContract, DebuggerDisplay("{ DebugInfo }"), KnownType(typeof(BindingsTreeNodeReference))]
    public class BindingsTreeNode : BaseBindingsTreeNode, IBindingsTreeNode
    {
        private IBindingsTreeNode bindingsTreeNodeInternal;
        private Folder parentSourceElement;
        private Folder dataContextList;
        private Folder propertyBindingsFolder;
        private Folder childNodesFolder;

        public BindingsTreeNode()
        {
        }

        public BindingsTreeNode(IBindingsTreeNode bindingsTreeNode, BaseBindingsTreeNode parentNode)
        {
            bindingsTreeNodeInternal = bindingsTreeNode;
            this.ParentNode = parentNode;
            this.name = bindingsTreeNode.Name;
        }

        [Exclude]
        public override object InternalObject
        {
            get
            {
                return bindingsTreeNodeInternal;
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

        [Association("BindingsTreeNode_ChildNodes", "ID", "ParentID")]
        public Folder ChildNodesFolder
        {
            get
            {
                if (childNodesFolder == null)
                {
                    childNodesFolder = new Folder(this, BindingsFolderNames.ChildNodes);
                    childNodesFolder.ChildNodes.AddRange(bindingsTreeNodeInternal.ChildNodes.Where(n => !n.IsReference).Select(n => new BindingsTreeNode(n, childNodesFolder)));
                    childNodesFolder.ChildNodes.AddRange(bindingsTreeNodeInternal.ChildNodes.Where(n => n.IsReference).Cast<IBindingsTreeNodeReference>().Select(n => new BindingsTreeNodeReference(n, childNodesFolder)));
                }

                return childNodesFolder;
            }
        }

        [Association("BindingsTreeNode_PropertyBindings", "ID", "ParentID")]
        public Folder PropertyBindingsFolder
        {
            get
            {
                if (propertyBindingsFolder == null)
                {
                    propertyBindingsFolder = new Folder(this, BindingsFolderNames.PropertyBindings);
                    propertyBindingsFolder.ChildNodes.AddRange(bindingsTreeNodeInternal.PropertyBindings.Select(b => new PropertyBinding(b, propertyBindingsFolder)));
                }

                return propertyBindingsFolder;
            }
        }

        [Association("BindingsTreeNode_ParentSourceElement", "ID", "ParentID")]
        public Folder ParentSourceElementFolder
        {
            get
            {
                if (parentSourceElement == null && bindingsTreeNodeInternal.ParentSourceElement != null)
                {
                    parentSourceElement = new Folder(this, BindingsFolderNames.ParentSourceElement);
                    parentSourceElement.ChildNodes.Add(new Element(bindingsTreeNodeInternal.ParentSourceElement, parentSourceElement));
                }

                return parentSourceElement;
            }
        }

        [Association("BindingsTreeNode_DataContextList", "ID", "ParentID")]
        public Folder DataContextListFolder
        {
            get
            {
                if (dataContextList == null)
                {
                    dataContextList = new Folder(this, BindingsFolderNames.DataContext);
                    bindingsTreeNodeInternal.DataContext.ToList().ForEach(d => dataContextList.ChildNodes.Add(new DataContextObject(d, dataContextList)));
                }

                return dataContextList;
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
                return bindingsTreeNodeInternal.DataContext;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        [Exclude]
        public IElement ParentSourceElement
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
        public IEnumerable<IBindingsTreeNode> ChildNodes
        {
            get
            {
                return bindingsTreeNodeInternal.ChildNodes;
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
                return true;
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
                return this.GetDebugInfo(bindingsTreeNodeInternal.Name);
            }
        }

        IBindingsTreeNode IBindingsTreeNode.ParentNode
        {
            get
            {
                return this.ParentNode.InternalObject as IBindingsTreeNode;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool IsReference
        {
            get
            {
                return bindingsTreeNodeInternal.IsReference;
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public string Origin
        {
            get
            {
                return bindingsTreeNodeInternal.Origin;
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
