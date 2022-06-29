using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;

namespace AbstraX.ViewEngine.SemanticTree
{
    public class MarkupNode : BaseNode
    {
        public bool IsModelMarkup { get; internal set; }

        public MarkupNode(SyntaxTreeNode syntaxTreeNode, string contentPart) : base(NodeKind.Markup, syntaxTreeNode, contentPart)
        {
        }

        public MarkupNode(NodeKind kind, SyntaxTreeNode syntaxTreeNode, string contentPart) : base(kind, syntaxTreeNode, contentPart)
        {
        }

        public override void Accept(RazorSemanticVisitor visitor)
        {
        }
    }
}
