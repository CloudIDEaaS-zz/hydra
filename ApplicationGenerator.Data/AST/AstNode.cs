using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationGenerator.AST
{
    public abstract class AstNode
    {
        public NodeKind NodeKind { get; }
        public AstNode ParentNode { get; private set; }
        public LinkedList<AstNode> Children { get; }
        public abstract void AcceptVisitor(IAstVisitor visitor);

        public AstNode(NodeKind nodeKind)
        {
            this.NodeKind = nodeKind;
            this.Children = new LinkedList<AstNode>();
        }

        public AstNode AddChild(AstNode astNode)
        {
            astNode.ParentNode = this;
            this.Children.AddLast(astNode);

            return astNode;
        }
    }
}
