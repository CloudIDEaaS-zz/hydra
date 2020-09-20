using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class BuiltinTypeExpression : Expression
    {
        private unsafe CppSharp.Parser.AST.Expression builtinTypeExpression;

        public unsafe BuiltinTypeExpression(CppSharp.Parser.AST.Expression builtinTypeExpression) : base(builtinTypeExpression)
        {
            this.builtinTypeExpression = builtinTypeExpression;
        }
    }
}