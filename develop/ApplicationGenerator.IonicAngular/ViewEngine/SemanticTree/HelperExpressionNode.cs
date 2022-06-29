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
    public class HelperExpressionNode : RenderNode
    {
        public string Method;
        public SeparatedSyntaxList<ArgumentSyntax> Arguments { get; }
        public View PartialView { get; private set; }

        public HelperExpressionNode(RazorParserVisitor visitor, SyntaxTreeNode syntaxTreeNode, string contentPart) : base(visitor, NodeKind.HelperExpression, syntaxTreeNode, contentPart)
        {
            SyntaxTree syntaxTree;
            CompilationUnitSyntax syntaxRoot;
            MethodDeclarationSyntax renderMethod;
            ExpressionStatementSyntax statement;
            ExpressionSyntax expression;
            var builder = new StringBuilder();

            visitor.StartClass(builder);
            visitor.StartRender(builder);

            builder.AppendLineFormat("{0}{1};", " ".Repeat(8), contentPart);

            visitor.WrapRenderAndClass(builder);

            syntaxTree = CSharpSyntaxTree.ParseText(builder.ToString());
            syntaxRoot = (CompilationUnitSyntax)syntaxTree.GetRoot();

            renderMethod = syntaxRoot.GetRenderMethod();
            statement = (ExpressionStatementSyntax)renderMethod.Body.Statements.Single();
            expression = statement.Expression;

            if (expression is InvocationExpressionSyntax)
            {
                var invocationExpression = (InvocationExpressionSyntax)expression;

                if (invocationExpression.Expression is MemberAccessExpressionSyntax)
                {
                    var memberAccessExpression = (MemberAccessExpressionSyntax)invocationExpression.Expression;

                    this.Method = memberAccessExpression.Name.Identifier.Text;

                    if (this.Method == "Partial")
                    {
                        this.PartialView = visitor.CreatePartialView(this.Method, invocationExpression.ArgumentList.Arguments);
                    }
                }
                else
                {
                    DebugUtils.Break();
                }

                this.Arguments = invocationExpression.ArgumentList.Arguments;
            }
            else
            {
                DebugUtils.Break();
            }

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

            this.Code = string.Format("\r\n\r\n{0}this.Write({1});", " ".Repeat(8), this.ContentPart.Trim());

            builder.AppendLine(this.Code);

            this.CumulativeCode = builder.ToString();

            return visitor.WrapRenderAndClass(builder);
        }
    }
}
