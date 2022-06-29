using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;

namespace AbstraX.ViewEngine.SemanticTree
{
    public class SectionNode : BaseNode
    {
        public string Name { get; }

        public SectionNode(SyntaxTreeNode syntaxTreeNode, string contentPart, string name) : base(NodeKind.Section, syntaxTreeNode, contentPart)
        {
            this.Name = name;
        }

        public SectionNode(NodeKind kind, SyntaxTreeNode syntaxTreeNode, string contentPart, string name) : base(kind, syntaxTreeNode, contentPart)
        {
            this.Name = name;
        }

        public override void Accept(RazorSemanticVisitor visitor)
        {
        }
    }
}
