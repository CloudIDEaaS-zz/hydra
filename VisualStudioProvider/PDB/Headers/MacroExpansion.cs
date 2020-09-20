using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class MacroExpansion : PreprocessedEntity
    {
        private unsafe CppSharp.Parser.AST.MacroExpansion macroExpansion;

        public unsafe MacroExpansion(Declaration owningDeclaration, CppSharp.Parser.AST.MacroExpansion macroExpansion) : base(owningDeclaration, macroExpansion)
        {
            this.macroExpansion = macroExpansion;

            this.macroExpansion.AssertNotNullAndOfType<CppSharp.Parser.AST.MacroExpansion>();
        }

        public MacroDefinition Definition
        {
            get
            {
                if (macroExpansion.Definition != null)
                {
                    return new MacroDefinition(this.OwningDeclaration, macroExpansion.Definition);
                }
                else
                {
                    return null;
                }
            }
        }

        public string Name
        {
            get
            {
                return macroExpansion.Name;
            }
        }

        public string Text
        {
            get
            {
                return macroExpansion.Text;
            }
        }

        public int LineNumberStart
        {
            get
            {
                return macroExpansion.LineNumberStart;
            }
        }

        public int LineNumberEnd
        {
            get
            {
                return macroExpansion.LineNumberEnd;
            }
        }

        public int ColumnNumberStart
        {
            get
            {
                return macroExpansion.ColumnNumberStart;
            }
        }

        public int ColumnNumberEnd
        {
            get
            {
                return macroExpansion.ColumnNumberEnd;
            }
        }
    }
}