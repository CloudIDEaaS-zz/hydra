using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class FunctionTemplateSpecialization
    {
        private unsafe CppSharp.Parser.AST.FunctionTemplateSpecialization specialization;
        public FunctionTemplate OwningFunctionTemplate { get; set; }  

        public unsafe FunctionTemplateSpecialization(FunctionTemplate owningFunctionTemplate, CppSharp.Parser.AST.FunctionTemplateSpecialization specialization)
        {
            this.OwningFunctionTemplate = owningFunctionTemplate;
            this.specialization = specialization;

            this.OwningFunctionTemplate.AssertNotNull();
            this.specialization.AssertNotNullAndOfType<CppSharp.Parser.AST.FunctionTemplateSpecialization>();
        }

        public string SpecializationKind
        {
            get
            {
                return specialization.SpecializationKind.ToString();
            }
        }

        public Function SpecializedFunction
        {
            get
            {
                if (specialization.SpecializedFunction == null)
                {
                    return null;
                }
                else if (specialization.SpecializedFunction is CppSharp.Parser.AST.Method)
                {
                    return new Method((CppSharp.Parser.AST.Method) specialization.SpecializedFunction);
                }
                else
                {
                    return new Function(specialization.SpecializedFunction);
                }
            }
        }
        
        public IEnumerable<TemplateArgument> Arguments
        {
            get
            {
                foreach (var argument in specialization.GetArguments())
                {
                    yield return new TemplateArgument(this, argument);
                }
            }
        }
    }
}
