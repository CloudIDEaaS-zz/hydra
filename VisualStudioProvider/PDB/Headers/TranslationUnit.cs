using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class TranslationUnit : Namespace
    {
        private unsafe CppSharp.Parser.AST.TranslationUnit translationUnit;

        public unsafe TranslationUnit(CppSharp.Parser.AST.TranslationUnit translationUnit) : base(translationUnit)
        {
            this.translationUnit = translationUnit;
            this.translationUnit.AssertNotNullAndOfType<CppSharp.Parser.AST.TranslationUnit>();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return translationUnit.Location.ID;
            }
        }

        public string FileName
        {
            get
            {
                return translationUnit.FileName;
            }
        }
        
        public bool IsSystemHeader
        {
            get
            {
                return translationUnit.IsSystemHeader;
            }
        }

        public IEnumerable<MacroDefinition> Macros
        {
            get
            {
                foreach (var macro in translationUnit.GetMacros())
                {
                    yield return new MacroDefinition(this, macro);
                }
            }
        }
    }
}
