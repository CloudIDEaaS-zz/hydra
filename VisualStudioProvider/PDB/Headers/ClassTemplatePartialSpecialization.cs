using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class ClassTemplatePartialSpecialization : ClassTemplateSpecialization
    {
        private unsafe CppSharp.Parser.AST.ClassTemplatePartialSpecialization classTemplatePartialSpecialization;

        public unsafe ClassTemplatePartialSpecialization(CppSharp.Parser.AST.ClassTemplatePartialSpecialization classTemplatePartialSpecialization) : base(classTemplatePartialSpecialization)
        {
            this.classTemplatePartialSpecialization = classTemplatePartialSpecialization;
            this.classTemplatePartialSpecialization.AssertNotNullAndOfType<CppSharp.Parser.AST.ClassTemplatePartialSpecialization>();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return classTemplatePartialSpecialization.Location.ID;
            }
        }
    }
}
