using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class AccessSpecifierDecl : Declaration
    {
        private unsafe CppSharp.Parser.AST.AccessSpecifierDecl accessSpecifierDecl;
        public Class OwningClass { get; set; }

        public unsafe AccessSpecifierDecl(Class owningClass, CppSharp.Parser.AST.AccessSpecifierDecl accessSpecifierDecl) : base(accessSpecifierDecl)
        {
            this.OwningClass = owningClass;
            this.accessSpecifierDecl = accessSpecifierDecl;

            this.OwningClass.AssertNotNull();
            this.accessSpecifierDecl.AssertNotNullAndOfType<CppSharp.Parser.AST.AccessSpecifierDecl>();
        }

        public unsafe AccessSpecifierDecl(CppSharp.Parser.AST.AccessSpecifierDecl accessSpecifierDecl) : base(accessSpecifierDecl)
        {
            this.accessSpecifierDecl = accessSpecifierDecl;
            this.accessSpecifierDecl.AssertNotNullAndOfType<CppSharp.Parser.AST.AccessSpecifierDecl>();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return accessSpecifierDecl.Location.ID;
            }
        }
    }
}
