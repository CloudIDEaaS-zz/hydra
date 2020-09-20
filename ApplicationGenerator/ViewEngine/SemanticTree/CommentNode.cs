using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;
using Utils;

namespace AbstraX.ViewEngine.SemanticTree
{

    public class CommentNode : MarkupNode
    {
        public string Comment { get; set; }

        public CommentNode(SyntaxTreeNode syntaxTreeNode, string contentPart) : base(NodeKind.Comment, syntaxTreeNode, contentPart)
        {
            this.Comment = contentPart.Replace("@", "/");
        }
    }
}
