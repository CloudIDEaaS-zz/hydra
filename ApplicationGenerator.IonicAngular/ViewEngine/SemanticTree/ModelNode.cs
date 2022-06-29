using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Utils;

namespace AbstraX.ViewEngine.SemanticTree
{
    public class ModelNode : CodeNode
    {
        public string ModelType { get; set; }

        public ModelNode(RazorParserVisitor visitor, SyntaxTreeNode syntaxTreeNode, string contentPart) : base(visitor, NodeKind.Model, syntaxTreeNode, contentPart)
        {
            this.OnParent += ModelNode_OnParent;
        }

        private void ModelNode_OnParent(object sender, Utils.EventArgs<BaseNode> e)
        {
            var root = Parse();
            var _class = (ClassDeclarationSyntax)root.Members.OfType<ClassDeclarationSyntax>().Single();
            var field = (FieldDeclarationSyntax)_class.Members.OfType<FieldDeclarationSyntax>().Last();
            var type = (GenericNameSyntax) field.Declaration.Type;
            var identifier = type.Identifier.ValueText;

            if (identifier == "EntityModel")
            {
                var argType = (IdentifierNameSyntax) type.TypeArgumentList.Arguments.Single();

                this.ModelType = argType.Identifier.ValueText;
            }
        }

        protected override string GetCumulativeCode()
        {
            var codeNodes = visitor.CodeNodes;
            var builder = new StringBuilder();

            foreach (var node in codeNodes.ToList())
            {
                if (node.ContentPart == "model")
                {
                    codeNodes.Remove(node);

                    visitor.StartClass(builder);

                    builder.Append(this.ContentPart.Trim().Prepend(" ".Repeat(4)).Append(" "));
                    builder.AppendLine("Model;");

                    visitor.StartRender(builder);

                    this.Code = builder.ToString();
                }
                else
                {
                    builder.Append(node.Code);
                }
            }

            this.CumulativeCode = builder.ToString();

            return visitor.WrapRenderAndClass(builder);
        }
    }
}
