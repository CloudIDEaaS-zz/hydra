using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class TemplateSpecializationType : Type
    {
        private unsafe CppSharp.Parser.AST.TemplateSpecializationType templateSpecializationType;

        public unsafe TemplateSpecializationType(CppSharp.Parser.AST.TemplateSpecializationType templateSpecializationType) : base(templateSpecializationType)
        {
            this.templateSpecializationType = templateSpecializationType;
            this.templateSpecializationType.AssertNotNullAndOfType<CppSharp.Parser.AST.TemplateSpecializationType>();
        }

        public Template Template
        {
            get
            {
                if (templateSpecializationType.Template == null)
                {
                    return null;
                }
                else
                {
                    return templateSpecializationType.Template.GetRealTemplateInternal();
                }
            }
        }

        public QualifiedType Desugard
        {
            get
            {
                if (templateSpecializationType.Desugared == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(templateSpecializationType.Desugared);
                }
            }
        }

        public IEnumerable<TemplateArgument> Arguments
        {
            get
            {
                foreach (var argument in templateSpecializationType.GetArguments())
                {
                    yield return new TemplateArgument(this, argument);
                }
            }
        }
    }
}
