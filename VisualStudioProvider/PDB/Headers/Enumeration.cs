using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class Enumeration : DeclarationContext
    {
        private unsafe CppSharp.Parser.AST.Enumeration enumeration;
        public int EnumIndex { get; set; }

        public unsafe Enumeration(Declaration owningDeclaration, CppSharp.Parser.AST.Enumeration enumeration, int enumIndex) : base(owningDeclaration, enumeration)
        {
            this.enumeration = enumeration;
            this.EnumIndex = enumIndex;

            this.enumeration.AssertNotNullAndOfType<CppSharp.Parser.AST.Enumeration>();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return enumeration.Location.ID;
            }
        }

        public unsafe Enumeration(CppSharp.Parser.AST.Enumeration enumeration) : base(enumeration)
        {
            this.enumeration = enumeration;
        }

        public Type BuiltInType
        {
            get
            {
                if (enumeration.BuiltinType == null)
                {
                    return null;
                }
                else
                {
                    return new Type(enumeration.BuiltinType);
                }
            }
        }

        public IEnumerable<EnumerationItem> Items
        {
            get
            {
                foreach (var item in enumeration.GetItems())
                {
                    yield return new EnumerationItem(this, item);
                }
            }
        }
    }
}
