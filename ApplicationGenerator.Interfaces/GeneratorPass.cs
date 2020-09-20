using System;
using System.Collections.Generic;
using System.Text;

namespace AbstraX
{
    public enum GeneratorPass
    {
        None,
        All,
        HierarchyOnly,
        Files,
    }

    public static class GeneratorPassCommon
    {
        public const GeneratorPass Last = GeneratorPass.Files;
    }
}
