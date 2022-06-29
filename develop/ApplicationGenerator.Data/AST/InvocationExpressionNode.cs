using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace ApplicationGenerator.AST
{
    public class InvocationExpressionNode : AstNode
    {
        public MethodKind MethodKind { get; }
        public ExpressionNode Target { get; set; }
        public List<ExpressionNode> Arguments { get; set; }

        public InvocationExpressionNode(string name) : base(NodeKind.InvocationExpression)
        {
            this.MethodKind = Enum.Parse<MethodKind>(name);
            this.Arguments = new List<ExpressionNode>();
        }

        public override void AcceptVisitor(IAstVisitor visitor)
        {
            visitor.VisitInvocationExpression(this);
        }
    }
}
