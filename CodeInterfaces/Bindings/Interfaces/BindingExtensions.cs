using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeInterfaces.TypeMappings;
using Utils;

namespace CodeInterfaces.Bindings
{
    public static class BindingExtensions
    {
#if !SILVERLIGHT

        public static IEnumerable<IElement> GetContextObjects(this IBindingsTreeNode node)
        {
            if (node is IBindingsTreeNodeReference)
            {
                var nodeReference = (IBindingsTreeNodeReference)node;

                return ((IBindingsTreeNodeReference)node).Node.DataContext.Select(o => new SurrogateElement(o.ContextObject, nodeReference.ReferencedFrom));
            }
            else
            {
                return node.DataContext.Select(o => o.ContextObject);
            }
        }

        public static void GetNavigationPropertyOwner(this IBindingsTreeNode bindingNode, IElement childElement, out IElement owner, out object property, out BaseType propertyType)
        {
            var context = bindingNode.ParentNode.DataContext.Where(e => e.ContextObject.ID == bindingNode.ParentSourceElement.ID).SingleOrDefault();

            if (context != null)
            {
                if (context.ContainerType == ContainerType.Property)
                {
                    owner = bindingNode.ParentNode.ParentSourceElement;
                    property = context.ContextObject;
                    propertyType = context.ContextObject.DataType;
                }
                else
                {
                    owner = context.ContextObject;

                    if (childElement is SurrogateElement)
                    {
                        property = ((SurrogateElement)childElement).ReferencedFrom.Parent.GetProperyName();
                    }
                    else
                    {
                        property = childElement.Parent.GetProperyName();
                    }

                    propertyType = childElement.DataType;
                }
            }
            else
            {
                context = bindingNode.ParentNode.DataContext.Where(e => e.ContextObject.ID == bindingNode.ParentSourceElement.Parent.ID).SingleOrDefault();

                if (context != null)
                {
                    if (context.ContainerType == ContainerType.Property)
                    {
                        owner = bindingNode.ParentNode.ParentSourceElement;
                        property = context.ContextObject;
                        propertyType = context.ContextObject.DataType;
                    }
                    else
                    {
                        owner = context.ContextObject;

                        if (childElement is SurrogateElement)
                        {
                            property = ((SurrogateElement) childElement).ReferencedFrom.Parent.GetProperyName();
                        }
                        else
                        {
                            property = childElement.Parent.GetProperyName();
                        }

                        propertyType = ((IElement)childElement.Parent).DataType;
                    }
                }
                else
                {
                    owner = null;
                    property = null;
                    propertyType = null;
                }
            }
        }
#endif
        public static BindingsTree Tree(this IBindingsTreeNode node)
        {
            var tree = new BindingsTree();
            var nodeParent = node;
            IBindingsTreeNode lastParent = null;

            while (nodeParent != null)
            {
                lastParent = nodeParent;
                nodeParent = nodeParent.ParentNode;
            }

            AddToTree(lastParent, tree);

            return tree;
        }

        private static void AddToTree(IBindingsTreeNode node, BindingsTree tree)
        {
            tree.Add(node);

            foreach (var childNode in node.ChildNodes)
            {
                if (!childNode.IsReference)
                {
                    AddToTree(childNode, tree);
                }
            }
        }
    }
}
