using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using AbstraX.Bindings;
using System.ServiceModel.DomainServices.Server;
using System.ComponentModel.DataAnnotations;
using AbstraX.Bindings.Interfaces;

namespace AbstraX.BindingsTreeEntities
{
    [DataContract, DebuggerDisplay("{ DebugInfo }")]
    public class CachedBindingsTree : BaseBindingsTreeNode, IBindingsTreeCachedTree
    {
        private string rootElementID;
        private IBindingsTree bindingsTree;
        private BindingsTreeCachedNodes cachedNodes;

        public CachedBindingsTree()
        {
        }

        public CachedBindingsTree(string rootElementID, IBindingsTree bindingsTree)
        {
            this.rootElementID = rootElementID;
            this.bindingsTree = bindingsTree;
            this.name = name;
        }

        [Exclude]
        public override object InternalObject
        {
            get
            {
                return bindingsTree;
            }
        }

        [Exclude]
        public BindingsTreeEntity BindingsTree
        {
            get
            {
                return new BindingsTreeEntity(bindingsTree, this.ID);
            }
        }

        [DataMember]
        public string RootElementID
        {
            get
            {
                return rootElementID;
            }

            set
            {
                rootElementID = value;
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
                return this.GetDebugInfo(rootElementID);
            }
        }

        public IBindingsTreeCachedNodes CachedNodes
        {
            get 
            {
                if (cachedNodes == null)
                {
                    cachedNodes = new BindingsTreeCachedNodes(bindingsTree);
                }

                return cachedNodes;
            }
        }

        public void AddToCache(IEnumerable<BaseBindingsTreeNode> nodes)
        {
            nodes.ToList().ForEach(n => 
            {
                if (!HasCachedNode(n.ID))
                {
                    cachedNodes.Nodes.Add(n.ID, n);
                }
            });
        }

        public void AddToCache(BaseBindingsTreeNode node)
        {
            if (!HasCachedNode(node.ID))
            {
                cachedNodes.Nodes.Add(node.ID, node);
            }
        }

        public bool HasCachedNode(string nodeId)
        {
            return cachedNodes.Nodes.ContainsKey(nodeId);
        }
    }
}
