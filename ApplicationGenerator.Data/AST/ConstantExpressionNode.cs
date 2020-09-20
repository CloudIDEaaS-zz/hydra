using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationGenerator.AST
{
    public class ConstantExpressionNode : ExpressionNode
    {
        public ConstantKind ConstantKind { get; }
        public Type Type { get; }
        public object Value { get; }

        public ConstantExpressionNode(ConstantKind constantKind, Type type, object value) : base(NodeKind.ConstantExpression)
        {
            this.ConstantKind = constantKind;
            this.Type = type;
            this.Value = value;
        }

        public ConstantExpressionNode(ConstantKind constantKind) : base(NodeKind.ConstantExpression)
        {
            this.ConstantKind = constantKind;
        }

        public override void AcceptVisitor(IAstVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
