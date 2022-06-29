using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationGenerator.AST
{
    public interface IAstVisitor
    {
        void VisitInvocationExpression(InvocationExpressionNode invocationExpression);
        void BinaryOperatorExpression(BinaryOperatorExpressionNode binaryOperatorExpressionNode);
        void VisitMemberExpression(MemberExpressionNode memberExpressionNode);
        void VisitNewExpression(NewExpressionNode newExpressionNode);
    }
}
