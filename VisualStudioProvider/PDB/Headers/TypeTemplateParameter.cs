using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class TypeTemplateParameter : TemplateParameter
    {
        private unsafe CppSharp.Parser.AST.TypeTemplateParameter typeTemplateParameter;

        public unsafe TypeTemplateParameter(CppSharp.Parser.AST.TypeTemplateParameter typeTemplateParameter) : base(typeTemplateParameter)
        {
            this.typeTemplateParameter = typeTemplateParameter;
            this.typeTemplateParameter.AssertNotNullAndOfType<CppSharp.Parser.AST.TypeTemplateParameter>();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return typeTemplateParameter.Location.ID;
            }
        }

        public QualifiedType DefaultArgument
        {
            get
            {
                if (typeTemplateParameter.DefaultArgument == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(typeTemplateParameter.DefaultArgument);
                }
            }
        }
    }
}
