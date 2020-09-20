using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CodeInterfaces.Bindings.Interfaces;
#if !SILVERLIGHT
#endif

namespace CodeInterfaces.Bindings
{
    public interface IBindingsTree
    {
        string BindingsTreeName { get; set; }
        bool GenerateStatelessConstructs { get; set; }
        IEnumerable<IBindingsTreeNode> RootBindings { get; }
#if !SILVERLIGHT
        IProviderService ProviderService { get; set; }
#endif
    }
}
