using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class ClassTemplate : Template
    {
        private unsafe CppSharp.Parser.AST.ClassTemplate classTemplate;

        public unsafe ClassTemplate(DeclarationContext owningDeclarationContext, CppSharp.Parser.AST.ClassTemplate classTemplate) : base(owningDeclarationContext, classTemplate)
        {
            this.classTemplate = classTemplate;
            this.classTemplate.AssertNotNullAndOfType<CppSharp.Parser.AST.ClassTemplate>();
        }

        public unsafe ClassTemplate(CppSharp.Parser.AST.ClassTemplate classTemplate) : base(classTemplate)
        {
            this.classTemplate = classTemplate;
            this.classTemplate.AssertNotNullAndOfType<CppSharp.Parser.AST.ClassTemplate>();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return classTemplate.Location.ID;
            }
        }

        public IEnumerable<ClassTemplateSpecialization> Specializations
        {
            get
            {
                foreach (var specialization in classTemplate.GetSpecializations())
                {
                    yield return (ClassTemplateSpecialization)specialization.GetRealClassInternal();
                }
            }
        }
    }
}
