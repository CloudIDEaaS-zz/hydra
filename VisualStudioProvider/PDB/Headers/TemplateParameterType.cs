using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class TemplateParameterType : Type
    {
        private unsafe CppSharp.Parser.AST.TemplateParameterType templateParameterType;

        public unsafe TemplateParameterType(CppSharp.Parser.AST.TemplateParameterType templateParameterType) : base(templateParameterType)
        {
            this.templateParameterType = templateParameterType;
            this.templateParameterType.AssertNotNullAndOfType<CppSharp.Parser.AST.TemplateParameterType>();
        }

        public IntegerValue Depth
        {
            get
            {
                return new IntegerValue(templateParameterType.Depth);
            }
        }

        public IntegerValue Index
        {
            get
            {
                return new IntegerValue(templateParameterType.Index);
            }
        }

        public bool IsParameterPack
        {
            get
            {
                return templateParameterType.IsParameterPack;
            }
        }

        public TypeTemplateParameter Parameter
        {
            get
            {
                if (templateParameterType.Parameter == null)
                {
                    return null;
                }
                else
                {
                    return new TypeTemplateParameter(templateParameterType.Parameter);
                }
            }
        }
    }
}
