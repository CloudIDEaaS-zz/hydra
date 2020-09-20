using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class TemplateParameterSubstitutionType : Type
    {
        private unsafe CppSharp.Parser.AST.TemplateParameterSubstitutionType templateParameterSubstitutionType;

        public unsafe TemplateParameterSubstitutionType(CppSharp.Parser.AST.TemplateParameterSubstitutionType templateParameterSubstitutionType) : base(templateParameterSubstitutionType)
        {
            this.templateParameterSubstitutionType = templateParameterSubstitutionType;
            this.templateParameterSubstitutionType.AssertNotNullAndOfType<CppSharp.Parser.AST.TemplateParameterSubstitutionType>();
        }

        public QualifiedType Replacement
        {
            get
            {
                if (templateParameterSubstitutionType.Replacement == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(templateParameterSubstitutionType.Replacement);
                }
            }
        }
    }
}
