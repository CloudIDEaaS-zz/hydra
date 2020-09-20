using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class TypeAliasTemplate : Template
    {
        private unsafe CppSharp.Parser.AST.TypeAliasTemplate typeAliasTemplate;

        public unsafe TypeAliasTemplate(DeclarationContext owningDeclarationContext, CppSharp.Parser.AST.TypeAliasTemplate typeAliasTemplate) : base(owningDeclarationContext, typeAliasTemplate)
        {
            this.typeAliasTemplate = typeAliasTemplate;
            this.typeAliasTemplate.AssertNotNullAndOfType<CppSharp.Parser.AST.TypeAliasTemplate>();
        }

        public unsafe TypeAliasTemplate(CppSharp.Parser.AST.TypeAliasTemplate typeAliasTemplate) : base(typeAliasTemplate)
        {
            this.typeAliasTemplate = typeAliasTemplate;
            this.typeAliasTemplate.AssertNotNullAndOfType<CppSharp.Parser.AST.TypeAliasTemplate>();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return typeAliasTemplate.Location.ID;
            }
        }
    }
}
