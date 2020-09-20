using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class DependentTemplateSpecializationType : Type
    {
        private unsafe CppSharp.Parser.AST.DependentTemplateSpecializationType dependentTemplateSpecializationType;

        public unsafe DependentTemplateSpecializationType(CppSharp.Parser.AST.DependentTemplateSpecializationType dependentTemplateSpecializationType) : base(dependentTemplateSpecializationType)
        {
            this.dependentTemplateSpecializationType = dependentTemplateSpecializationType;
            this.dependentTemplateSpecializationType.AssertNotNullAndOfType<CppSharp.Parser.AST.DependentTemplateSpecializationType>();
        }

        public QualifiedType Desugared
        {
            get
            {
                if (dependentTemplateSpecializationType.Desugared == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(dependentTemplateSpecializationType.Desugared);
                }
            }
        }

        public IEnumerable<TemplateArgument> Arguments
        {
            get
            {
                foreach (var argument in dependentTemplateSpecializationType.GetArguments())
                {
                    yield return new TemplateArgument(this, argument);
                }
            }
        }
    }
}
