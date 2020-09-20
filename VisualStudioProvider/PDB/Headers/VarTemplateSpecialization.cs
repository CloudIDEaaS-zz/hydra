using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class VarTemplateSpecialization : Variable
    {
        private unsafe CppSharp.Parser.AST.VarTemplateSpecialization varTemplateSpecialization;

        public unsafe VarTemplateSpecialization(Declaration owningDeclaration, CppSharp.Parser.AST.VarTemplateSpecialization varTemplateSpecialization) : base(varTemplateSpecialization)
        {
            this.varTemplateSpecialization = varTemplateSpecialization;
            this.varTemplateSpecialization.AssertNotNullAndOfType<CppSharp.Parser.AST.VarTemplateSpecialization>();
        }

        public unsafe VarTemplateSpecialization(CppSharp.Parser.AST.VarTemplateSpecialization varTemplateSpecialization) : base(varTemplateSpecialization)
        {
            this.varTemplateSpecialization = varTemplateSpecialization;
            this.varTemplateSpecialization.AssertNotNull();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return varTemplateSpecialization.Location.ID;
            }
        }

        public IEnumerable<TemplateArgument> Arguments
        {
            get
            {
                foreach (var argument in varTemplateSpecialization.GetArguments())
                {
                    yield return new TemplateArgument(this, argument);
                }
            }
        }

        public string SpecializationKind
        {
            get
            {
                return varTemplateSpecialization.SpecializationKind.ToString();
            }
        }

        public VarTemplate TemplateDecl
        {
            get
            {
                if (varTemplateSpecialization.TemplatedDecl == null)
                {
                    return null;
                }
                else
                {
                    return new VarTemplate(varTemplateSpecialization.TemplatedDecl);
                }
            }
        }
    }
}
