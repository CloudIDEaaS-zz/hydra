using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class TypeDefNameDecl : Declaration
    {
        private unsafe CppSharp.Parser.AST.TypedefNameDecl typedefNameDecl;

        public unsafe TypeDefNameDecl(CppSharp.Parser.AST.TypedefNameDecl typedefNameDecl) : base(typedefNameDecl)
        {
            this.typedefNameDecl = typedefNameDecl;
            this.typedefNameDecl.AssertNotNull();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return typedefNameDecl.Location.ID;
            }
        }

        public virtual QualifiedType QualifiedType
        {
            get
            {
                if (typedefNameDecl.QualifiedType == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(typedefNameDecl.QualifiedType);
                }
            }
        }
    }
}
