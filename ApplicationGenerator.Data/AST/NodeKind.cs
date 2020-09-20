using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationGenerator.AST
{
    public enum NodeKind
    {
        SyntaxTree,
        InvocationExpression,
        BinaryOperatorExpression,
        MemberExpression,
        ConstantExpression,
        ParameterExpression,
        NewExpression
    }
}
