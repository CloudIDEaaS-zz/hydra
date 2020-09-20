using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;

namespace AbstraX.ViewEngine.SemanticTree
{
    public class MarkupBlockNode : BaseNode
    {
        public MarkupBlockNode(SyntaxTreeNode syntaxTreeNode, string contentPart) : base(NodeKind.MarkupBlock, syntaxTreeNode, contentPart)
        {
        }

        public MarkupBlockNode(NodeKind kind, SyntaxTreeNode syntaxTreeNode, string contentPart) : base(kind, syntaxTreeNode, contentPart)
        {
        }

        public override void Accept(RazorSemanticVisitor visitor)
        {
        }
    }
}
