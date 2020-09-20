using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class MacroDefinition : PreprocessedEntity
    {
        private unsafe CppSharp.Parser.AST.MacroDefinition macroDefinition;

        public unsafe MacroDefinition(Declaration owningDeclaration, CppSharp.Parser.AST.MacroDefinition macroDefinition) : base(owningDeclaration, macroDefinition)
        {
            this.macroDefinition = macroDefinition;

            this.macroDefinition.AssertNotNullAndOfType<CppSharp.Parser.AST.MacroDefinition>();
        }

        public string Expression
        {
            get
            {
                return macroDefinition.Expression;
            }
        }

        public string Name
        {
            get
            {
                return macroDefinition.Name;
            }
        }

        public int LineNumberStart
        {
            get
            {
                return macroDefinition.LineNumberStart;
            }
        }

        public int LineNumberEnd
        {
            get
            {
                return macroDefinition.LineNumberEnd;
            }
        }

        public int ColumnNumberStart
        {
            get
            {
                return macroDefinition.ColumnNumberStart;
            }
        }

        public int ColumnNumberEnd
        {
            get
            {
                return macroDefinition.ColumnNumberEnd;
            }
        }
    }
}