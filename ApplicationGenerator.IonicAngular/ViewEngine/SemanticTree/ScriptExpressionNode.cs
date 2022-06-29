using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;
using Utils;

namespace AbstraX.ViewEngine.SemanticTree
{
    public class ScriptExpressionNode : RenderNode
    {
        public VariableNode MatchingVariable { get; private set; }
        public string ScriptReplace { get; private set; }
        public string Target { get; private set; }
        public string Property { get; private set; }
        public string Method { get; private set; }
        public SeparatedSyntaxList<ArgumentSyntax> Arguments { get; private set; }
        public bool IsModelExpression { get; }
        public string ModelProperty { get; }


        public ScriptExpressionNode(RazorParserVisitor visitor, SyntaxTreeNode syntaxTreeNode, string contentPart) : base(visitor, NodeKind.ScriptExpression, syntaxTreeNode, contentPart)
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
            var allVariables = new List<VariableNode>();
            string contentPart;

            foreach (var node in codeNodes.ToList())
            {
                var variables = node.ChildNodes.OfType<VariableNode>();

                allVariables.AddRange(variables);

                builder.Append(node.Code);
            }

            contentPart = this.ContentPart.Trim();

            if (allVariables.Any(v => v.Name == contentPart))
            {
                this.MatchingVariable = allVariables.Single(v => v.Name == contentPart);
            }
            else
            {
                ParseContent(contentPart);
            }

            this.Code = contentPart.Append(";\r\n");
            this.ScriptReplace = contentPart;

            builder.AppendLine(this.Code);

            this.CumulativeCode = builder.ToString();

            return visitor.WrapRenderAndClass(builder);
        }

        private void ParseContent(string contentPart)
        {
            SyntaxTree syntaxTree;
            CompilationUnitSyntax syntaxRoot;
            ExpressionStatementSyntax statement;
            ExpressionSyntax expression;
            var builder = new StringBuilder();

            visitor.StartClass(builder);
            visitor.StartRender(builder);

            builder.AppendLineFormat("{0}{1};", " ".Repeat(8), contentPart);

            visitor.WrapRenderAndClass(builder);

            syntaxTree = CSharpSyntaxTree.ParseText(builder.ToString());
            syntaxRoot = (CompilationUnitSyntax)syntaxTree.GetRoot();

            MethodDeclarationSyntax renderMethod = syntaxRoot.GetRenderMethod();
            statement = (ExpressionStatementSyntax)renderMethod.Body.Statements.Single();
            expression = statement.Expression;

            if (expression is InvocationExpressionSyntax)
            {
                var invocationExpression = (InvocationExpressionSyntax)expression;

                if (invocationExpression.Expression is MemberAccessExpressionSyntax)
                {
                    var memberAccessExpression = (MemberAccessExpressionSyntax)invocationExpression.Expression;

                    this.Method = memberAccessExpression.Name.Identifier.Text;

                    if (memberAccessExpression.Expression is IdentifierNameSyntax)
                    {
                        this.Target = ((IdentifierNameSyntax)memberAccessExpression.Expression).Identifier.Text;
                    }
                }
                else
                {
                    DebugUtils.Break();
                }

                this.Arguments = invocationExpression.ArgumentList.Arguments;
            }
            else if (expression is MemberAccessExpressionSyntax)
            {
                var memberAccessExpression = (MemberAccessExpressionSyntax)expression;

                this.Property = memberAccessExpression.Name.Identifier.Text;

                if (memberAccessExpression.Expression is IdentifierNameSyntax)
                {
                    this.Target = ((IdentifierNameSyntax)memberAccessExpression.Expression).Identifier.Text;
                }
            }
            else
            {
                DebugUtils.Break();
            }
        }
    }
}
