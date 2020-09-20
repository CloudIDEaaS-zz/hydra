using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class Macro : PreprocessedEntity
    {
        private unsafe CppSharp.Parser.AST.MacroDefinition macro;
        public unsafe Namespace OwningNamespace { get; set; }

        public unsafe Macro(Namespace owningNamespace, CppSharp.Parser.AST.MacroDefinition macro) : base(owningNamespace, macro)
        {
            this.OwningNamespace = owningNamespace;
            this.macro = macro;

            this.OwningNamespace.AssertNotNull();
            this.macro.AssertNotNullAndOfType<CppSharp.Parser.AST.MacroDefinition>();
        }

        public string Name
        {
            get
            {
                return macro.Name;
            }
        }

        public string Expression
        {
            get
            {
                return macro.Expression;
            }
        }
    
        public int LineNumberStart
        {
            get
            {
                return macro.LineNumberStart;
            }
        }

        public int LineNumberEnd
        {
            get
            {
                return macro.LineNumberEnd;
            }
        }
    }
}
