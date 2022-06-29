using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;

namespace AbstraX.ViewEngine.SemanticTree
{
    public class AssignmentNode : BaseNode
    {
        public object Left { get; set; }
        public object Right { get; set; }

        public AssignmentNode(NodeKind kind, SyntaxTreeNode syntaxTreeNode, string contentPart) : base(kind, syntaxTreeNode, contentPart)
        {
        }

        public override void Accept(RazorSemanticVisitor visitor)
        {
        }
    }
}
