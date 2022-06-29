using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeInterfaces.XPathBuilder;

namespace CodeInterfaces
{
    public interface IRoot : IBase, IPathQueryable, IDisposable
    {
        string ParentFieldName { get; }
        string URL { get; }
        ProviderType ProviderType { get; }
        BaseType DataType { get; }
        bool GetItemsOfType<T>(out bool doStandardTraversal, out int traversalDepthLimit, out IEnumerable<T> elements);
        IEnumerable<IElement> RootElements { get; }
        void ExecuteGlobalWhere(XPathAxisElement element);
    }
}
