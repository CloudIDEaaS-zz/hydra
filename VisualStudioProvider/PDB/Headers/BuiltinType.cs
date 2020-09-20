using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class BuiltinType : Type
    {
        private unsafe CppSharp.Parser.AST.BuiltinType builtinType;

        public unsafe BuiltinType(CppSharp.Parser.AST.BuiltinType builtinType) : base(builtinType)
        {
            this.builtinType = builtinType;
            this.builtinType.AssertNotNullAndOfType<CppSharp.Parser.AST.BuiltinType>();
        }

        public string Type
        {
            get
            {
                return builtinType.Type.ToString();
            }
        }
    }
}
