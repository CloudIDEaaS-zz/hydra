using System;
using System.Net;
using AbstraX.BindingsTreeEntities;
using AbstraX.Bindings;
using System.Collections.Generic;
using System.Linq;

namespace AbstraX.ServerInterfaces
{
    public static class BindingsTreeExtensions
    {
        public static int GetNodeCount(this IEnumerable<IBindingsTreeNode> nodes)
        {
            if (nodes != null)
            {
                var count = nodes.Count();

                nodes.ToList<IBindingsTreeNode>().ForEach(n => count += n.ChildNodes.GetNodeCount());

                return count;
            }
            else
            {
                return 0;
            }
        }

        public static int GetNodeCount(this List<IBindingsTree> tree)
        {
            return tree.Sum(t => t.RootBindings.Sum(n => n.ChildNodes.GetNodeCount()));
        }

        public static string GetID(this BaseBindingsTreeNode entity)
        {
            var id = (string) entity.GetType().GetProperty("ID").GetGetMethod().Invoke(entity, null);

            return id;
        }

        public static string GetParentID(this BaseBindingsTreeNode entity)
        {
            var parentId = (string)entity.GetType().GetProperty("ParentID").GetGetMethod().Invoke(entity, null);

            return parentId;
        }

        public static string GetName(this BaseBindingsTreeNode entity)
        {
            var name = (string)entity.GetType().GetProperty("Name").GetGetMethod().Invoke(entity, null);

            return name;
        }

        public static bool GetHasChildren(this BaseBindingsTreeNode entity)
        {
            var hasChildren = (bool)entity.GetType().GetProperty("HasChildren").GetGetMethod().Invoke(entity, null);

            return hasChildren;
        }

        public static float GetChildOrdinal(this BaseBindingsTreeNode entity)
        {
            var ordinal = (float)entity.GetType().GetProperty("ChildOrdinal").GetGetMethod().Invoke(entity, null);

            return ordinal;
        }
    }
}
