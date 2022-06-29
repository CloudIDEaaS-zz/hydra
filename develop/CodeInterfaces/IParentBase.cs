using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces
{
    public interface IParentBase : IBase, IPathQueryable
    {
        IEnumerable<IElement> ChildElements { get; }
        IRoot Root { get; }
    }
}
