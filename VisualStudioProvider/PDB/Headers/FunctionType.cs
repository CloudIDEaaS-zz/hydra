using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class FunctionType : Type
    {
        private unsafe CppSharp.Parser.AST.FunctionType functionType;

        public unsafe FunctionType(CppSharp.Parser.AST.FunctionType functionType) : base(functionType)
        {
            this.functionType = functionType;
            this.functionType.AssertNotNullAndOfType<CppSharp.Parser.AST.FunctionType>();
        }

        public string CallingConvention
        {
            get
            {
                return functionType.CallingConvention.ToString();
            }
        }

        public QualifiedType ReturnType
        {
            get
            {
                if (functionType.ReturnType == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(functionType.ReturnType);
                }
            }
        }

        public IEnumerable<Parameter> Parameters
        {
            get
            {
                foreach (var parm in functionType.GetParameters())
                {
                    yield return new Parameter(this, parm);
                }
            }
        }
    }
}
