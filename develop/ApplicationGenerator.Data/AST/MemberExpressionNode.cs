using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationGenerator.AST
{
    public class MemberExpressionNode : ExpressionNode
    {
        public string Name { get; }
        public ExpressionNode Expression { get; set; }

        public MemberExpressionNode(string name) : base(NodeKind.MemberExpression)
        {
            this.Name = name;
        }

        public override void AcceptVisitor(IAstVisitor visitor)
        {
            visitor.VisitMemberExpression(this);
        }
    }
}
