using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ApplicationGenerator.AST
{
    public class NewExpressionNode : ExpressionNode
    {
        public Type Type { get; }
        public ConstructorInfo Constructor { get; }
        public MemberInfo[] Members { get; }
        public List<ExpressionNode> Arguments { get; set; }

        public NewExpressionNode(Type type, ConstructorInfo constructor, MemberInfo[] members) : base(NodeKind.NewExpression)
        {
            this.Type = type;
            this.Constructor = constructor;
            this.Members = members;
            this.Arguments = new List<ExpressionNode>();
        }

        public override void AcceptVisitor(IAstVisitor visitor)
        {
            visitor.VisitNewExpression(this);
        }
    }
}
