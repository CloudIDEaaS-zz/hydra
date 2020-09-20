using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class TypeAlias : TypeDefNameDecl
    {
        private unsafe CppSharp.Parser.AST.TypeAlias typeAlias;
        public unsafe DeclarationContext OwningDeclarationContext { get; set; }

        public unsafe TypeAlias(DeclarationContext owningDeclarationContext, CppSharp.Parser.AST.TypeAlias typeAlias) : base(typeAlias)
        {
            this.OwningDeclarationContext = owningDeclarationContext;
            this.typeAlias = typeAlias;

            this.OwningDeclarationContext.AssertNotNull();
            this.typeAlias.AssertNotNullAndOfType<CppSharp.Parser.AST.TypeAlias>();
        }

        public unsafe TypeAlias(CppSharp.Parser.AST.TypeAlias typeAlias) : base(typeAlias)
        {
            this.typeAlias = typeAlias;
            this.typeAlias.AssertNotNullAndOfType<CppSharp.Parser.AST.TypeAlias>();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return typeAlias.Location.ID;
            }
        }

        public override QualifiedType QualifiedType
        {
            get
            {
                if (typeAlias.QualifiedType == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(typeAlias.QualifiedType);
                }
            }
        }

        public Template DescribedAliasTemplate
        {
            get
            {
                if (typeAlias.DescribedAliasTemplate == null)
                {
                    return null;
                }
                else
                {
                    return typeAlias.DescribedAliasTemplate.GetRealTemplateInternal();
                }
            }
        }
    }
}
