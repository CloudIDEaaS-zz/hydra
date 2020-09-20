using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class TemplateParameter : Declaration
    {
        private unsafe CppSharp.Parser.AST.TemplateParameter templateParameter;

        public unsafe TemplateParameter(CppSharp.Parser.AST.TemplateParameter templateParameter) : base(templateParameter)
        {
            this.templateParameter = templateParameter;
            this.templateParameter.AssertNotNull();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return templateParameter.Location.ID;
            }
        }

        public IntegerValue Depth
        {
            get
            {
                return new IntegerValue(templateParameter.Depth);
            }
        }

        public IntegerValue Index
        {
            get
            {
                return new IntegerValue(templateParameter.Index);
            }
        }

        public bool IsParameterPack
        {
            get
            {
                return templateParameter.IsParameterPack;
            }
        }
    }
}
