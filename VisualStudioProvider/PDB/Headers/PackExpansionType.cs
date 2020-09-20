using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class PackExpansionType : Type
    {
        private unsafe CppSharp.Parser.AST.PackExpansionType packExpansionType;

        public unsafe PackExpansionType(CppSharp.Parser.AST.PackExpansionType packExpansionType) : base(packExpansionType)
        {
            this.packExpansionType = packExpansionType;
            this.packExpansionType.AssertNotNullAndOfType<CppSharp.Parser.AST.PackExpansionType>();
        }
    }
}
