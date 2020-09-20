using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class Expression : Statement
    {
        private unsafe CppSharp.Parser.AST.Expression expression;

        public unsafe Expression(CppSharp.Parser.AST.Expression expression) : base(expression)
        {
            this.expression = expression;
            this.expression.AssertNotNullAndOfType<CppSharp.Parser.AST.Expression>();
        }
    }
}
