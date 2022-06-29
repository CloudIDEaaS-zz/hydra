using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces
{
    public interface IBase
    {
        string ID { get; }
        string ParentID { get; }
        string Name { get; }
        string DesignComments { get; }
        string Documentation { get; }
        string DocumentationSummary { get; }
        bool HasDocumentation { get; }
        string ImageURL { get; }
        float ChildOrdinal { get; }
        string DebugInfo { get; }
        IBase Parent { get; }
        string FolderKeyPair { get; set; }
        DefinitionKind Kind { get; }
        Modifiers Modifiers { get; }
        bool HasChildren { get; }
        Facet[] Facets { get; }
        IExtension LoadExtension();
        IProviderEntityService ProviderEntityService { get; }
    }
}
