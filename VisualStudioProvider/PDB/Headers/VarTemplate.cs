using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class VarTemplate : Template
    {
        private unsafe CppSharp.Parser.AST.VarTemplate varTemplate;

        public unsafe VarTemplate(DeclarationContext owningDeclarationContext, CppSharp.Parser.AST.VarTemplate varTemplate) : base(owningDeclarationContext, varTemplate)
        {
            this.varTemplate = varTemplate;
            this.varTemplate.AssertNotNullAndOfType<CppSharp.Parser.AST.VarTemplate>();
        }

        public unsafe VarTemplate(CppSharp.Parser.AST.VarTemplate varTemplate) : base(varTemplate)
        {
            this.varTemplate = varTemplate;
            this.varTemplate.AssertNotNullAndOfType<CppSharp.Parser.AST.VarTemplate>();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return varTemplate.Location.ID;
            }
        }

        public IEnumerable<VarTemplateSpecialization> Specializations
        {
            get
            {
                foreach (var specialization in varTemplate.GetSpecializations())
                {
                    if (specialization is CppSharp.Parser.AST.VarTemplatePartialSpecialization)
                    {
                        yield return new VarTemplatePartialSpecialization(this, (CppSharp.Parser.AST.VarTemplatePartialSpecialization) specialization);
                    }
                    else
                    {
                        yield return new VarTemplateSpecialization(this, specialization);
                    }
                }
            }
        }
    }
}
