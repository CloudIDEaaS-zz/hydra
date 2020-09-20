using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class FunctionTemplate : Template
    {
        private unsafe CppSharp.Parser.AST.FunctionTemplate functionTemplate;

        public unsafe FunctionTemplate(DeclarationContext owningDeclarationContext, CppSharp.Parser.AST.FunctionTemplate functionTemplate) : base(owningDeclarationContext, functionTemplate)
        {
            this.functionTemplate = functionTemplate;
            this.functionTemplate.AssertNotNullAndOfType<CppSharp.Parser.AST.FunctionTemplate>();
        }

        public unsafe FunctionTemplate(CppSharp.Parser.AST.FunctionTemplate functionTemplate) : base(functionTemplate)
        {
            this.functionTemplate = functionTemplate;
            this.functionTemplate.AssertNotNullAndOfType<CppSharp.Parser.AST.FunctionTemplate>();
        }
        public IEnumerable<FunctionTemplateSpecialization> Specializations
        {
            get
            {
                foreach (var specialization in functionTemplate.GetSpecializations())
                {
                    yield return new FunctionTemplateSpecialization(this, specialization);
                }
            }
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return functionTemplate.Location.ID;
            }
        }
    }
}
