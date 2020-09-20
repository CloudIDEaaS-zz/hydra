using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class TemplateArgument
    {
        private unsafe CppSharp.Parser.AST.TemplateArgument argument;
        public ClassTemplateSpecialization OwningClassTemplateSpecialization { get; set; }
        public DependentTemplateSpecializationType OwningDependentTemplateSpecializationType { get; set; }
        public VarTemplateSpecialization OwningVarTemplateSpecialization { get; set; }
        public TemplateSpecializationType OwningTemplateSpecializationType { get; set; }
        public FunctionTemplateSpecialization OwningFunctionTemplateSpecialization { get; set; }

        public unsafe TemplateArgument(ClassTemplateSpecialization owningClassTemplateSpecialization, CppSharp.Parser.AST.TemplateArgument argument)
        {
            this.OwningClassTemplateSpecialization = owningClassTemplateSpecialization;

            this.OwningClassTemplateSpecialization.AssertNotNull();
            this.argument = argument;
            this.argument.AssertNotNullAndOfType<CppSharp.Parser.AST.TemplateArgument>();
        }

        public unsafe TemplateArgument(DependentTemplateSpecializationType owningDependentTemplateSpecializationType, CppSharp.Parser.AST.TemplateArgument argument)
        {
            this.OwningDependentTemplateSpecializationType = owningDependentTemplateSpecializationType;
            this.argument = argument;

            this.OwningDependentTemplateSpecializationType.AssertNotNull();
            this.argument.AssertNotNullAndOfType<CppSharp.Parser.AST.TemplateArgument>();
        }

        public unsafe TemplateArgument(VarTemplateSpecialization owningVarTemplateSpecialization, CppSharp.Parser.AST.TemplateArgument argument)
        {
            this.OwningVarTemplateSpecialization = owningVarTemplateSpecialization;
            this.argument = argument;

            this.OwningVarTemplateSpecialization.AssertNotNull();
            this.argument.AssertNotNullAndOfType<CppSharp.Parser.AST.TemplateArgument>();
        }

        public unsafe TemplateArgument(TemplateSpecializationType owningTemplateSpecializationType, CppSharp.Parser.AST.TemplateArgument argument)
        {
            this.OwningTemplateSpecializationType = owningTemplateSpecializationType;
            this.argument = argument;

            this.OwningTemplateSpecializationType.AssertNotNull();
            this.argument.AssertNotNullAndOfType<CppSharp.Parser.AST.TemplateArgument>();
        }

        public unsafe TemplateArgument(FunctionTemplateSpecialization owningFunctionTemplateSpecialization, CppSharp.Parser.AST.TemplateArgument argument)
        {
            this.OwningFunctionTemplateSpecialization = owningFunctionTemplateSpecialization;
            this.argument = argument;

            this.OwningFunctionTemplateSpecialization.AssertNotNull();
            this.argument.AssertNotNull();
        }

        public Declaration Declaration
        {
            get
            {
                if (argument.Declaration == null)
                {
                    return null;
                }
                else
                {
                    return argument.Declaration.GetRealDeclarationInternal();
                }
            }
        }

        public int Integral
        {
            get
            {
                return argument.Integral;
            }
        }

        public string Kind
        {
            get
            {
                return argument.Kind.ToString();
            }
        }

        public QualifiedType Type
        {
            get
            {
                return new QualifiedType(argument.Type);
            }
        }
    }
}
