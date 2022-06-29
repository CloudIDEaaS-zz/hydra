using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationGenerator.AST
{
    public abstract class ExpressionNode : AstNode
    {
        public ExpressionNode(NodeKind nodeKind) : base(nodeKind)
        {
        }
    }
}
