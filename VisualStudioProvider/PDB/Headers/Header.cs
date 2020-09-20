using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Diagnostics;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    [DebuggerDisplay(" { FileName } ")]
    public class Header : Namespace
    {
        private unsafe CppSharp.Parser.AST.TranslationUnit unit;
        private List<MacroDefinition> macros;

        public Header(CppSharp.Parser.AST.TranslationUnit unit) : base(unit)
        {
            this.unit = unit;
            this.unit.AssertNotNullAndOfType<CppSharp.Parser.AST.TranslationUnit>();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return unit.Location.ID;
            }
        }

        public override string Name
        {
            get
            {
                return this.FileName;
            }
        }

        public string FileName
        {
            get
            {
                return unit.FileName;
            }
        }

        public IEnumerable<MacroDefinition> Macros
        {
            get
            {
                if (macros != null)
                {
                    foreach (var macro in macros)
                    {
                        yield return macro;
                    }
                }
                else
                {
                    foreach (var macro in unit.GetMacros())
                    {
                        yield return new MacroDefinition(this, macro);
                    }
                }
            }
        }

        internal int CacheObjects()
        {
            var count = 0;

            macros = this.Macros.ToList();

            count += base.CacheObjects();
            count += macros.Count;

            return count;
        }
    }
}
