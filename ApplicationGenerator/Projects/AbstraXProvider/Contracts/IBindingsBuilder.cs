using System;
using System.Collections.Generic;
using AbstraX.Bindings;

namespace AbstraX.Contracts
{
    public interface IBindingsBuilder
    {
        IBindingsTreeNode BreakOnNode { set; }
        Dictionary<string, IElementBuild> GenerateBuilds(List<IBindingsTree> bindings);
    }
}
