using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class CXXConstructExpr : Expression
    {
        private unsafe CppSharp.Parser.AST.CXXConstructExpr cxxConstructExpr;

        public unsafe CXXConstructExpr(CppSharp.Parser.AST.CXXConstructExpr cxxConstructExpr) : base(cxxConstructExpr)
        {
            this.cxxConstructExpr = cxxConstructExpr;

            this.cxxConstructExpr.AssertNotNullAndOfType<CppSharp.Parser.AST.CXXConstructExpr>();
        }

        public IEnumerable<Expression> Arguments
        {
            get
            {
                foreach (var argument in cxxConstructExpr.GetArguments())
                {
                    yield return argument.GetRealExpressionInternal();
                }
            }
        }
    }
}