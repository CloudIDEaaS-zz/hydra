using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class CallExpr : Expression
    {
        private unsafe CppSharp.Parser.AST.CallExpr callExpr;

        public unsafe CallExpr(CppSharp.Parser.AST.CallExpr callExpr) : base(callExpr)
        {
            this.callExpr = callExpr;

            this.callExpr.AssertNotNullAndOfType<CppSharp.Parser.AST.CallExpr>();
        }

        public IEnumerable<Expression> Arguments
        {
            get
            {
                foreach (var argument in callExpr.GetArguments())
                {
                    yield return argument.GetRealExpressionInternal();
                }
            }
        }
    }
}