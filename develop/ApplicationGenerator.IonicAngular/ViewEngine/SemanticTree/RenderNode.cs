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
    public class RenderNode : CodeNode
    {
        public RenderNode(RazorParserVisitor visitor, SyntaxTreeNode syntaxTreeNode, string contentPart) : base(visitor, NodeKind.Render, syntaxTreeNode, contentPart)
        {
            var syntaxRoot = this.Parse();

            ParseStatements(syntaxTree, syntaxRoot, model);
        }

        public RenderNode(RazorParserVisitor visitor, NodeKind kind, SyntaxTreeNode syntaxTreeNode, string contentPart) : base(visitor, kind, syntaxTreeNode, contentPart)
        {
        }


        protected override string GetCumulativeCode()
        {
            var codeNodes = visitor.CodeNodes;
            var builder = new StringBuilder();

            foreach (var node in codeNodes.ToList())
            {
                builder.Append(node.Code);
            }

            this.Code = this.ContentPart.TrimBlankLines().SpaceBlock(4);

            builder.AppendLine(this.Code);

            this.CumulativeCode = builder.ToString();

            return visitor.WrapRenderAndClass(builder);
        }

        private void ParseStatements(SyntaxTree syntaxTree, CompilationUnitSyntax syntaxRoot, SemanticModel model)
        {
            var renderMethod = syntaxRoot.GetRenderMethod();
            var statements = renderMethod.Body.Statements.ToList();

            foreach (var statement in statements)
            {
			    SwitchExtensions.Switch(statement, () => statement.GetType().Name,

				    SwitchExtensions.Case<ExpressionStatementSyntax>("ExpressionStatementSyntax", (expressionStatement) =>
				    {
                        var expression = expressionStatement.Expression;

                        SwitchExtensions.Switch(expression, () => expression.GetType().Name,

				            SwitchExtensions.Case<AssignmentExpressionSyntax>("AssignmentExpressionSyntax", (assignmentExpression) =>
				            {
                                var left = assignmentExpression.Left;
                                var right = assignmentExpression.Right;
                                AssignmentNode assignmentNode = null;

			                    SwitchExtensions.Switch(left, () => left.GetType().Name,

				                    SwitchExtensions.Case<ElementAccessExpressionSyntax>("ElementAccessExpressionSyntax", (elementAccessExpression) =>
				                    {
                                        var name = ((IdentifierNameSyntax)elementAccessExpression.Expression).Identifier.Text;
                                        var arg = elementAccessExpression.ArgumentList.Arguments.Single();
                                        var literal = (LiteralExpressionSyntax)arg.Expression;
                                        var key = (string) literal.Token.Value;

                                        if (name == "ViewData")
                                        {
                                            var viewDataNode = new ViewDataNode(null, key, assignmentExpression.ToFullString());

                                            viewDataNode.Left = key;

                                            assignmentNode = viewDataNode;
                                        }
                                        else
                                        {
                                            DebugUtils.Break();
                                        }

                                        DebugUtils.NoOp();
				                    }),
				                    SwitchExtensions.Case<MemberAccessExpressionSyntax>("MemberAccessExpressionSyntax", (memberAccessExpressionSyntax) =>
				                    {
                                        var name = ((IdentifierNameSyntax)memberAccessExpressionSyntax.Expression).Identifier.Text;
                                        var property = memberAccessExpressionSyntax.Name.Identifier.Text;

                                        if (name == "ViewBag")
                                        {
                                            var viewBagNode = new ViewBagNode(null, property, assignmentExpression.ToFullString());

                                            viewBagNode.Left = name;
                                            
                                            assignmentNode = viewBagNode;
                                        }
                                        else
                                        {
                                            DebugUtils.Break();
                                        }

                                        DebugUtils.NoOp();
				                    }),
                                    SwitchExtensions.Case<IdentifierNameSyntax>("IdentifierNameSyntax", (identifierName) =>
                                    {
                                        var name = identifierName.Identifier.Text;
                                        var propertyNode = new PropertyNode(null, assignmentExpression.ToFullString());

                                        propertyNode.Left = name;

                                        assignmentNode = propertyNode;

                                        DebugUtils.NoOp();
                                    }),
                                    SwitchExtensions.CaseElse(() =>
				                    {
                                        var a = left;
                                        var t = left.GetType().Name;

					                    // implementation here or throw error

					                    DebugUtils.Break();
				                    })
			                    );

                                SwitchExtensions.Switch(right, () => right.GetType().Name,

                                    SwitchExtensions.Case<LiteralExpressionSyntax>("LiteralExpressionSyntax", (literalExpression) =>
                                    {
                                        assignmentNode.Right = literalExpression.Token.Value;
                                    }),
                                    SwitchExtensions.CaseElse(() =>
                                    {
                                        var a = right;
                                        var t = right.GetType().Name;

                                        // implementation here or throw error

                                        DebugUtils.Break();
                                    })
                                );

                                assignmentNode.Right = right;

                                this.AddChild(assignmentNode);
                            }),
				            SwitchExtensions.CaseElse(() =>
				            {
                                var a = expression;
                                var t = expression.GetType().Name;

					            // implementation here or throw error

					            DebugUtils.Break();
				            })
			            );
				    }),
                    SwitchExtensions.Case<LocalDeclarationStatementSyntax>("LocalDeclarationStatementSyntax", (localDeclarationStatement) =>
                    {
                        var variable = localDeclarationStatement.Declaration.Variables.Single();
                        var name = variable.Identifier.Text;
                        var initializer = variable.Initializer;
                        var variableNode = new VariableNode(name, null, localDeclarationStatement.ToFullString());
                        ModelInvocationNode modelInvocationNode = null;

                        SwitchExtensions.Switch(initializer, () => initializer.GetType().Name,

				            SwitchExtensions.Case<EqualsValueClauseSyntax>("EqualsValueClauseSyntax", (equalsValueClause) =>
				            {
                                var value = equalsValueClause.Value;

                                if (value is InvocationExpressionSyntax)
                                {
                                    var invocationExpression = (InvocationExpressionSyntax)value;

                                    if (invocationExpression.Expression is MemberAccessExpressionSyntax)
                                    {
                                        var memberAccessExpression = (MemberAccessExpressionSyntax)invocationExpression.Expression;

                                        if (memberAccessExpression.Expression is IdentifierNameSyntax && ((IdentifierNameSyntax) memberAccessExpression.Expression).Identifier.Text == "Model")
                                        {
                                            if (memberAccessExpression.Name is GenericNameSyntax)
                                            {
                                                var identifierName = (GenericNameSyntax)memberAccessExpression.Name;
                                                var methodName = identifierName.Identifier.Text;
                                                var typeArguments = identifierName.TypeArgumentList.Arguments;

                                                modelInvocationNode = new ModelInvocationNode(null, invocationExpression.ToFullString(), variableNode, methodName, typeArguments);
                                            }
                                            else
                                            {
                                                DebugUtils.Break();
                                            }
                                        }
                                        else
                                        {
                                            DebugUtils.Break();
                                        }
                                    }
                                    else
                                    {
                                        DebugUtils.Break();
                                    }
                                }
                                else
                                {
                                    DebugUtils.Break();
                                }

                                variableNode.Right = value;
                            }),
				            SwitchExtensions.CaseElse(() =>
				            {
                                var a = initializer;
                                var t = initializer.GetType().Name;

					            // implementation here or throw error

					            DebugUtils.Break();
				            })
			            );

                        this.AddChild(variableNode);

                        if (modelInvocationNode != null)
                        {
                            variableNode.AddChild(modelInvocationNode);
                        }

                        DebugUtils.NoOp();
                    }),
                    SwitchExtensions.CaseElse(() =>
				    {
                        var a = statement;
                        var t = statement.GetType().Name;

					    // implementation here or throw error

					    DebugUtils.Break();
				    })
			    );
            }
        }
    }
}
