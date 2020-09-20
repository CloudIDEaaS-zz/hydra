using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class TypeDefDecl : TypeDefNameDecl
    {
        private unsafe CppSharp.Parser.AST.TypedefDecl typedefDecl;

        public unsafe TypeDefDecl(CppSharp.Parser.AST.TypedefDecl typedefDecl) : base(typedefDecl)
        {
            this.typedefDecl = typedefDecl;
            this.typedefDecl.AssertNotNullAndOfType<CppSharp.Parser.AST.TypedefDecl>();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return typedefDecl.Location.ID;
            }
        }
    }
}
