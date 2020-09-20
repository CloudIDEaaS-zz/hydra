using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class VarTemplatePartialSpecialization : VarTemplateSpecialization
    {
        private unsafe CppSharp.Parser.AST.VarTemplatePartialSpecialization varTemplatePartialSpecialization;
        public VarTemplate OwningVarTemplate { get; set; }

        public unsafe VarTemplatePartialSpecialization(CppSharp.Parser.AST.VarTemplatePartialSpecialization varTemplatePartialSpecialization) : base(varTemplatePartialSpecialization)
        {
            this.varTemplatePartialSpecialization = varTemplatePartialSpecialization;
            this.varTemplatePartialSpecialization.AssertNotNullAndOfType<CppSharp.Parser.AST.VarTemplatePartialSpecialization>();
        }

        public unsafe VarTemplatePartialSpecialization(VarTemplate owningVarTemplate, CppSharp.Parser.AST.VarTemplatePartialSpecialization varTemplatePartialSpecialization) : base(varTemplatePartialSpecialization)
        {
            this.OwningVarTemplate = owningVarTemplate;
            this.varTemplatePartialSpecialization = varTemplatePartialSpecialization;
            this.varTemplatePartialSpecialization.AssertNotNullAndOfType<CppSharp.Parser.AST.VarTemplatePartialSpecialization>();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return varTemplatePartialSpecialization.Location.ID;
            }
        }
    }
}
