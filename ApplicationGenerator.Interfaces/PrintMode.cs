using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    [Flags]
    public enum PrintMode
    {
        Any = 1,
        All = 2,
        PrintFacetsOnly = 4,
        PrintUIHierarchyPathOnly = 8 | ExcludeFromAll,
        PrintUIHierarchyPathAndModuleAssembliesStackOnly = 16 | PrintUIHierarchyPathOnly,
        ExcludeFromAll = 4096
    }
}
