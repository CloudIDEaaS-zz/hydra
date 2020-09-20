using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces.Bindings
{
    public interface IBindingsTreeNode
    {
        string ID { get; set; }
        string Name { get; set; }
        string Origin { get; set; }
        IEnumerable<IPropertyBinding> PropertyBindings { get; set; }
        IEnumerable<IDataContextObject> DataContext { get; set; }   
        IBindingsTreeNode ParentNode { get; set; }
        IElement ParentSourceElement { get; set; }
        IEnumerable<IBindingsTreeNode> ChildNodes { get; set; }
        bool IsReference { get; set; }
    }
}
