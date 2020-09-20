using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces.Bindings
{
    public class BindingsTree : List<IBindingsTreeNode>
    {
        public bool Contains(IElement element, out IBindingsTreeNode nodeReference)
        {
            nodeReference = null;

            foreach (var node in this)
            {
                foreach (var context in node.DataContext)
                {
                    if (context.ContextObject.DataType != null && element.DataType != null && context.ContextObject.DataType.FullyQualifiedName == element.DataType.FullyQualifiedName)
                    {
                        nodeReference = node;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
