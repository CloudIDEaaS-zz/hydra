using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class VTableLayout
    {
        private unsafe CppSharp.Parser.AST.VTableLayout vTableLayout;

        public unsafe VTableLayout(CppSharp.Parser.AST.VTableLayout vTableLayout)
        {
            this.vTableLayout = vTableLayout;
            this.vTableLayout.AssertNotNullAndOfType<CppSharp.Parser.AST.VTableLayout>();
        }

        public IEnumerable<VTableComponent> Components
        {
            get
            {
                foreach (var component in vTableLayout.GetComponents())
                {
                    yield return new VTableComponent(this, component);
                }
            }
        }
    }
}
