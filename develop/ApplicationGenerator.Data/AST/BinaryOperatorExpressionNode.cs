using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ApplicationGenerator.AST
{
    public class BinaryOperatorExpressionNode : ExpressionNode
    {
        public Operator Operator { get; set; }
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }

        public BinaryOperatorExpressionNode() : base(NodeKind.BinaryOperatorExpression)
        {
        }

        public override void AcceptVisitor(IAstVisitor visitor)
        {
            visitor.BinaryOperatorExpression(this);
        }
    }
}
