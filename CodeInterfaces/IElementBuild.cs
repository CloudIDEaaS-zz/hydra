using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeInterfaces.Bindings;

namespace CodeInterfaces
{
    public delegate void GetDerivedTypesHandler(out List<IElementBuild> elementBuilds);

    public interface IElementBuild
    {
        event GetDerivedTypesHandler GetDerivedTypes;
        string SuggestedNamespace { get; set; }
        IElement Element { get; set; }
        List<IPropertyBinding> Bindings { get; }
        List<NavigationItem> NavigationItems { get; }
        OptionalDataContextOperation SupportedOperations { get; set; }
        List<IOperation> RemoteCalls { get; set; } 
        bool IsBuilt { get; set; }
        bool IsSaved { get; set; }
        bool IsRootObject { get; set; }
        List<IElementBuild> DerivedTypes { get; }
    }
}
