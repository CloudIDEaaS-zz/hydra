using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;
using Utils;

namespace AbstraX.ViewEngine.SemanticTree
{
    public class CodeExpressionNode : RenderNode
    {
        public bool IsModelExpression { get; }
        public string ModelProperty { get; }

        public CodeExpressionNode(RazorParserVisitor visitor, SyntaxTreeNode syntaxTreeNode, string contentPart) : base(visitor, NodeKind.CodeExpression, syntaxTreeNode, contentPart)
        {
            Parse();

            if (contentPart.StartsWith("Model.Entity"))
            {
                this.IsModelExpression = true;

                if (contentPart.StartsWith("Model.Entity."))
                {
                    var modelProperty = contentPart.RemoveStart("Model.Entity.");

                    this.ModelProperty = modelProperty;
                }
            }
        }

        protected override string GetCumulativeCode()
        {
            var codeNodes = visitor.CodeNodes;
            var builder = new StringBuilder();

            foreach (var node in codeNodes.ToList())
            {
                builder.Append(node.Code);
            }

            this.Code = string.Format("\r\n\r\n{0}this.Write({1});", " ".Repeat(8), this.ContentPart.Trim());

            builder.AppendLine(this.Code);

            this.CumulativeCode = builder.ToString();

            return visitor.WrapRenderAndClass(builder);
        }
    }
}
