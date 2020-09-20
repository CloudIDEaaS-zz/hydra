using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class ClassTemplateSpecialization : Class
    {
        private unsafe CppSharp.Parser.AST.ClassTemplateSpecialization classTemplateSpecialization;
        public ClassTemplate OwningClassTemplate { get; set; }

        public unsafe ClassTemplateSpecialization(ClassTemplate owningClassTemplate, CppSharp.Parser.AST.ClassTemplateSpecialization classTemplateSpecialization) : base(classTemplateSpecialization)
        {
            this.OwningClassTemplate = owningClassTemplate;
            this.classTemplateSpecialization = classTemplateSpecialization;

            this.OwningClassTemplate.AssertNotNull();
            this.classTemplateSpecialization.AssertNotNullAndOfType<CppSharp.Parser.AST.ClassTemplateSpecialization>();
        }

        public unsafe ClassTemplateSpecialization(CppSharp.Parser.AST.ClassTemplateSpecialization classTemplateSpecialization) : base(classTemplateSpecialization)
        {
            this.classTemplateSpecialization = classTemplateSpecialization;
            this.classTemplateSpecialization.AssertNotNull();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return classTemplateSpecialization.Location.ID;
            }
        }

        public string SpecializationKind
        {
            get
            {
                return classTemplateSpecialization.SpecializationKind.ToString();
            }
        }

        public ClassTemplate TemplatedDecl
        {
            get
            {
                if (classTemplateSpecialization.TemplatedDecl == null)
                {
                    return null;
                }
                else
                {
                    return new ClassTemplate(classTemplateSpecialization.TemplatedDecl);
                }
            }
        }

        public IEnumerable<TemplateArgument> Arguments
        {
            get
            {
                foreach (var argument in classTemplateSpecialization.GetArguments())
                {
                    yield return new TemplateArgument(this, argument);
                }
            }
        }
    }
}
