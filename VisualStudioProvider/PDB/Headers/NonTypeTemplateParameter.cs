using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class NonTypeTemplateParameter : TemplateParameter
    {
        private unsafe CppSharp.Parser.AST.NonTypeTemplateParameter nonTypeTemplateParameter;

        public unsafe NonTypeTemplateParameter(CppSharp.Parser.AST.NonTypeTemplateParameter nonTypeTemplateParameter) : base(nonTypeTemplateParameter)
        {
            this.nonTypeTemplateParameter = nonTypeTemplateParameter;
            this.nonTypeTemplateParameter.AssertNotNullAndOfType<CppSharp.Parser.AST.NonTypeTemplateParameter>();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return nonTypeTemplateParameter.Location.ID;
            }
        }

        public Expression DefaultArgument
        {
            get
            {
                if (nonTypeTemplateParameter.DefaultArgument == null)
                {
                    return null;
                }
                else
                {
                    return nonTypeTemplateParameter.DefaultArgument.GetRealExpressionInternal();
                }
            }
        }

        public bool IsExpandedParameterPack
        {
            get
            {
                return nonTypeTemplateParameter.IsExpandedParameterPack;
            }
        }

        public bool IsPackExpansion
        {
            get
            {
                return nonTypeTemplateParameter.IsPackExpansion;
            }
        }

        public IntegerValue Position
        {
            get
            {
                return new IntegerValue(nonTypeTemplateParameter.Position);
            }
        }
    }
}
