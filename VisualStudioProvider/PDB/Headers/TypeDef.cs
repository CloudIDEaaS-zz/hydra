using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class TypeDef : TypeDefNameDecl
    {
        private unsafe CppSharp.Parser.AST.TypedefDecl typeDef;
        public unsafe DeclarationContext OwningDeclarationContext { get; set; }

        public unsafe TypeDef(DeclarationContext owningDeclarationContext, CppSharp.Parser.AST.TypedefDecl typeDef) : base(typeDef)
        {
            this.OwningDeclarationContext = owningDeclarationContext;
            this.typeDef = typeDef;

            this.OwningDeclarationContext.AssertNotNull();
            this.typeDef.AssertNotNullAndOfType<CppSharp.Parser.AST.TypedefDecl>();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return typeDef.Location.ID;
            }
        }

        public QualifiedType QualifiedType
        {
            get
            {
                if (typeDef.QualifiedType == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(typeDef.QualifiedType);
                }
            }
        }
    }
}
