using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationGenerator.AST
{
    public class SyntaxTreeNode : AstNode
    {
        public SyntaxTreeNode() : base(NodeKind.SyntaxTree)
        {
        }

        public override void AcceptVisitor(IAstVisitor visitor)
        {
            foreach (var childNode in this.Children)
            {
                childNode.AcceptVisitor(visitor);
            }
        }
    }
}
