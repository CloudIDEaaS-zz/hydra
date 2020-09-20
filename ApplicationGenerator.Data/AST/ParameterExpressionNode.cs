using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationGenerator.AST
{
    public class ParameterExpressionNode : ExpressionNode
    {
        public string Name { get; }
        public Type Type { get; }
        public bool IsByRef { get; }

        public ParameterExpressionNode(string name, Type type, bool isByRef) : base(NodeKind.ParameterExpression)
        {
            this.Name = name;
            this.Type = type;
            this.IsByRef = isByRef;
        }

        public override void AcceptVisitor(IAstVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
