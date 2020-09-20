using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.Bindings.Interfaces;
using AbstraX.BindingsTreeEntities;
using AbstraX.Bindings;

namespace AbstraX
{
    public class BindingsTreeCachedNodes : IBindingsTreeCachedNodes
    {
        private IBindingsTree bindingsTree;
        private Dictionary<string, BaseBindingsTreeNode> nodes;

        public BindingsTreeCachedNodes(IBindingsTree bindingsTree)
        {
            this.bindingsTree = bindingsTree;
        }

        public IBindingsTree BindingsTree
        {
            get
            {
                return bindingsTree;
            }
            set
            {
                bindingsTree = value;
            }
        }

        public Dictionary<string, BaseBindingsTreeNode> Nodes
        {
            get 
            {
                if (nodes == null)
                {
                    nodes = new Dictionary<string, BaseBindingsTreeNode>();
                }

                return nodes;
            }
        }
    }
}
