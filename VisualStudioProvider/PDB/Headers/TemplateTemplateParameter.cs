using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class TemplateTemplateParameter : Template
    {
        private unsafe CppSharp.Parser.AST.TemplateTemplateParameter templateTemplateParameter;

        public unsafe TemplateTemplateParameter(DeclarationContext owningDeclarationContext, CppSharp.Parser.AST.TemplateTemplateParameter templateTemplateParameter) : base(owningDeclarationContext, templateTemplateParameter)
        {
            this.templateTemplateParameter = templateTemplateParameter;
            this.templateTemplateParameter.AssertNotNullAndOfType<CppSharp.Parser.AST.TemplateTemplateParameter>();
        }

        public unsafe TemplateTemplateParameter(CppSharp.Parser.AST.TemplateTemplateParameter templateTemplateParameter) : base(templateTemplateParameter)
        {
            this.templateTemplateParameter = templateTemplateParameter;
            this.templateTemplateParameter.AssertNotNullAndOfType<CppSharp.Parser.AST.TemplateTemplateParameter>();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return templateTemplateParameter.Location.ID;
            }
        }

        public bool IsExpandedParameterPack
        {
            get
            {
                return templateTemplateParameter.IsExpandedParameterPack;
            }
        }

        public bool IsPackExpansion
        {
            get
            {
                return templateTemplateParameter.IsPackExpansion;
            }
        }

        public bool IsParameterPack
        {
            get
            {
                return templateTemplateParameter.IsParameterPack;
            }
        }
    }
}
