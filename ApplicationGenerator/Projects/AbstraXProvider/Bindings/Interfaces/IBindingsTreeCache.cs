using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstraX.BindingsTreeEntities;

namespace AbstraX.Bindings.Interfaces
{
    public interface IBindingsTreeCache
    {
        Dictionary<string, IBindingsTreeCachedTree> CachedTrees { get; }
        void ResetCache();
    }

    public interface IBindingsTreeCachedTree
    {
        string RootElementID { get; set; }
        IBindingsTreeCachedNodes CachedNodes { get; }
        void AddToCache(IEnumerable<BaseBindingsTreeNode> nodes);
        void AddToCache(BaseBindingsTreeNode node);
        bool HasCachedNode(string nodeId);
    }

    public interface IBindingsTreeCachedNodes
    {
        IBindingsTree BindingsTree { get; set; }
        Dictionary<string, BindingsTreeEntities.BaseBindingsTreeNode> Nodes { get; }
    }
}
