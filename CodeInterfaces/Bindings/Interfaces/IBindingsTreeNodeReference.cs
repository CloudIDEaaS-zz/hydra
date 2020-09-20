using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces.Bindings
{
    public interface IBindingsTreeNodeReference : IBindingsTreeNode
    {
        string ID { get; set; }
        string Name { get; set; }
        IElement ReferencedFrom { get; set; }
        IBindingsTreeNode Node { get; set; }
    }
}
