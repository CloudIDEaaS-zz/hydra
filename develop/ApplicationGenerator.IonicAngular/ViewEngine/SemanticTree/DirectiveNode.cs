using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;
using Utils;

namespace AbstraX.ViewEngine.SemanticTree
{
    public class DirectiveNode : CodeNode
    {
        public DirectiveNode(RazorParserVisitor visitor, SyntaxTreeNode syntaxTreeNode, string contentPart) : base(visitor, NodeKind.Directive, syntaxTreeNode, contentPart)
        {
            Parse();
        }

        protected override string GetCumulativeCode()
        {
            var codeNodes = visitor.CodeNodes;
            var builder = new StringBuilder();

            foreach (var node in codeNodes.ToList())
            {
                builder.Append(node.Code);
            }

            this.Code = this.ContentPart.Trim().Append(";\r\n");

            builder.AppendLine(this.Code);

            this.CumulativeCode = builder.ToString();

            return visitor.WrapRenderAndClass(builder);
        }
    }
}
