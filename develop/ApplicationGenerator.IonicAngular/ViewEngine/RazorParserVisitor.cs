using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor;
using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;
using Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using AbstraX.ViewEngine.SemanticTree;
using System.Web.Razor.Tokenizer.Symbols;
using System.Web.Razor.Generator;
using AbstraX.ViewEngine;

namespace AbstraX.ViewEngine
{
    public class RazorParserVisitor : ParserVisitor
    {
        private string content;
        public ViewProject ViewProject { get; }
        public LinkedList<CodeNode> CodeNodes { get; }
        private string viewRelativePath;
        public RootNode RootNode { get; set; }
        private Stack<BaseNode> nodeStack;
        private BlockType currentBlockType;
        private bool enterAtModel;
        private bool enterExpression;
        private bool renderStarted;
        private bool enterScriptsSection;
        public event PartialViewEventHandler OnPartialView;

        public RazorParserVisitor(StreamReader reader, string viewRelativePath, ViewProject viewProject)
        {
            this.ViewProject = viewProject;
            this.CodeNodes = new LinkedList<CodeNode>();

            this.viewRelativePath = viewRelativePath;
            nodeStack = new Stack<BaseNode>();

            reader.Rewind();

            using (reader.MarkForReset())
            {
                content = reader.ReadToEnd();

                if (content.HasBOM())
                {
                    content = content.RemoveBOM();
                }
            }
        }

        private void VisitEnter<T>(T node, string contentPart) where T : SyntaxTreeNode
        {
            var line = node.Start.LineIndex;

            if (node is Block)
            {
                var block = (Block)(object)node;
                var type = block.Type;

                switch (type)
                {
                    case BlockType.Markup:
                        break;
                    case BlockType.Statement:
                        break;
                    case BlockType.Expression:
                        break;
                    case BlockType.Section:
                        break;
                    case BlockType.Directive:
                        break;
                    case BlockType.Comment:
                        break;
                    default:
                        DebugUtils.Break();
                        break;
                }
            }
            else if (node is Span)
            {
                var span = (Span)(object)node;
                var kind = span.Kind;

                switch (kind)
                {
                    case SpanKind.Code:
                        break;
                    case SpanKind.Markup:
                        break;
                    case SpanKind.MetaCode:
                        break;
                    case SpanKind.Transition:
                        break;
                    case SpanKind.Comment:
                        break;
                    default:
                        DebugUtils.Break();
                        break;
                }
            }
            else
            {
                DebugUtils.Break();
            }
        }

        internal View CreatePartialView(string method, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            var args = new PartialViewEventArgs(method, arguments);

            OnPartialView(this, args);

            return args.View;
        }

        public override void VisitBlock(Block block)
        {
            if (block.Length > 0)
            {
                var contentPart = content.Substring(block.Start.AbsoluteIndex, block.Length);
                var type = block.Type;

                VisitEnter(block, contentPart);
                currentBlockType = type;

                switch (type)
                {
                    case BlockType.Markup:

                        if (this.RootNode == null)
                        {
                            this.RootNode = new RootNode(block, contentPart);
                            this.Push(this.RootNode);
                        }
                        else
                        {
                            var markupNode = new MarkupBlockNode(block, contentPart);

                            AddPush(markupNode);
                        }

                        break;

                    case BlockType.Comment:

                        var commentNode = new CommentNode(block, contentPart);

                        AddPush(commentNode);

                        break;

                    case BlockType.Statement:
                        break;
                    case BlockType.Expression:

                        if (contentPart == "@model")
                        {
                            enterAtModel = true;
                        }
                        else
                        {
                            enterExpression = true;
                        }

                        break;
                    case BlockType.Section:

                        var name = ((SectionCodeGenerator)block.CodeGenerator).SectionName;
                        SectionNode sectionNode;

                        if (name == "CustomScripts")
                        {
                            sectionNode = new CustomScriptsSectionNode(block, contentPart, name);
                            enterScriptsSection = true;
                        }
                        else
                        {
                            sectionNode = new SectionNode(block, contentPart, name);
                        }

                        AddPush(sectionNode);

                        break;

                    case BlockType.Directive:
                        break;
                    default:
                        DebugUtils.Break();
                        break;
                }
            }

            base.VisitBlock(block);
        }

        public override void VisitSpan(Span span)
        {
            if (span.Length > 0)
            {
                var contentPart = content.Substring(span.Start.AbsoluteIndex, span.Length);
                var kind = span.Kind;

                VisitEnter(span, contentPart);

                switch (kind)
                {
                    case SpanKind.Code:

                        switch (currentBlockType)
                        {
                            case BlockType.Directive:

                                var rootNode = (RootNode)this.CurrentNode;
                                var directiveNode = new DirectiveNode(this, span, contentPart);

                                this.Push(directiveNode);

                                rootNode.Directives.Add(directiveNode);
                                this.CodeNodes.AddLast(directiveNode);

                                break;

                            default:

                                CodeNode codeNode;

                                if (renderStarted)
                                {
                                    if (enterExpression)
                                    {
                                        if (IsHelperExpression(contentPart))
                                        {
                                            codeNode = new HelperExpressionNode(this, span, contentPart);

                                            AddPushCode(codeNode);
                                        }
                                        else
                                        {
                                            if (enterScriptsSection)
                                            {
                                                codeNode = new ScriptExpressionNode(this, span, contentPart);

                                                AddPushCode(codeNode);
                                            }
                                            else
                                            {
                                                codeNode = new CodeExpressionNode(this, span, contentPart);

                                                AddPushCode(codeNode);
                                            }
                                        }

                                        enterExpression = false;
                                    }
                                    else
                                    {
                                        codeNode = new RenderNode(this, span, contentPart);

                                        AddPushCode(codeNode);
                                    }
                                }
                                else
                                {
                                    codeNode = new CodeNode(this, span, contentPart);

                                    AddCode(codeNode);
                                }

                                break;
                        }

                        break;

                    case SpanKind.Markup:

                        var markupNode = new MarkupNode(span, contentPart);

                        if (enterAtModel)
                        {
                            var rootNode = (RootNode)this.CurrentNode;
                            var modelNode = new ModelNode(this, span, contentPart);

                            rootNode.ModelNode = modelNode;
                            this.CodeNodes.AddLast(modelNode);

                            enterAtModel = false;

                            markupNode.IsModelMarkup = true;
                        }

                        this.CurrentNode.AddChild(markupNode);

                        break;

                    case SpanKind.MetaCode:

                        if (currentBlockType == BlockType.Section)
                        {
                            var identifiers = span.Symbols.Cast<CSharpSymbol>().Where(s => s.Type == CSharpSymbolType.Identifier);
                            var firstIdentifier = identifiers.First();

                            if (firstIdentifier.Content == "section")
                            {
                                var lastIdentifier = identifiers.ElementAt(1);
                            }
                            else
                            {
                                DebugUtils.Break();
                            }
                        }

                        break;
                    case SpanKind.Transition:
                        break;
                    case SpanKind.Comment:
                        break;
                    default:
                        DebugUtils.Break();
                        break;
                }
            }

            base.VisitSpan(span);
        }

        private bool IsHelperExpression(string code)
        {
            SyntaxTree syntaxTree;
            CompilationUnitSyntax syntaxRoot;
            MethodDeclarationSyntax renderMethod;
            ExpressionStatementSyntax statement;
            ExpressionSyntax expression;
            var builder = new StringBuilder();

            StartClass(builder);
            StartRender(builder);

            builder.AppendLineFormat("{0}{1};", " ".Repeat(8), code);

            WrapRenderAndClass(builder);

            syntaxTree = CSharpSyntaxTree.ParseText(builder.ToString());
            syntaxRoot = (CompilationUnitSyntax)syntaxTree.GetRoot();

            renderMethod = syntaxRoot.GetRenderMethod();
            statement = (ExpressionStatementSyntax) renderMethod.Body.Statements.Single();
            expression = statement.Expression;

            if (expression is InvocationExpressionSyntax)
            {
                var invocationExpression = (InvocationExpressionSyntax)expression;

                if (invocationExpression.Expression is MemberAccessExpressionSyntax)
                {
                    var memberAccessExpression = (MemberAccessExpressionSyntax)invocationExpression.Expression;

                    if (memberAccessExpression.Expression is IdentifierNameSyntax)
                    {
                        return ((IdentifierNameSyntax)memberAccessExpression.Expression).Identifier.Text == "Html";
                    }
                }
            }

            return false;
        }

        public override void VisitStartBlock(Block block)
        {
            var contentPart = content.Substring(block.Start.AbsoluteIndex, block.Length);

            base.VisitStartBlock(block);
        }

        public override void VisitEndBlock(Block block)
        {
            var contentPart = content.Substring(block.Start.AbsoluteIndex, block.Length);

            base.VisitEndBlock(block);

            if (block.Length > 0)
            {
                Pop();
            }
        }

        public override void VisitError(RazorError err)
        {
            base.VisitError(err);
        }

        public void StartClass(StringBuilder builder)
        {
            builder.AppendLineFormat("\r\nclass {0}", Path.GetFileNameWithoutExtension(this.viewRelativePath));
            builder.AppendLine("{");

            builder.AppendLineFormat("{0}Dictionary<string, object> ViewData", " ".Repeat(4));
            builder.AppendLineFormat("{0}string Layout", " ".Repeat(4));
        }

        public void StartRender(StringBuilder builder)
        {
            this.renderStarted = true;

            builder.AppendLineFormat("\r\n{0}void Render()", " ".Repeat(4));
            builder.AppendLineSpaceIndent(4, "{");
        }

        public string WrapRenderAndClass(StringBuilder builder)
        {
            builder.AppendLineSpaceIndent(4, "}");
            builder.AppendLine("}");

            return builder.ToString();
        }

        public override void OnComplete()
        {
            base.OnComplete();
        }

        private void Push(BaseNode node)
        {
            nodeStack.Push(node);
        }

        private void AddPush(BaseNode node)
        {
            this.CurrentNode.AddChild(node);

            Push(node);
        }

        private void AddPushCode(CodeNode node)
        {
            AddPush(node);
            this.CodeNodes.AddLast(node);
        }

        private void AddCode(CodeNode node)
        {
            this.CurrentNode.AddChild(node);
            this.CodeNodes.AddLast(node);
        }

        public BaseNode Pop()
        {
            if (!enterAtModel)
            {
                return nodeStack.Pop();
            }
            else
            {
                return null;
            }
        }
            
        private BaseNode CurrentNode
        {
            get
            {
                return nodeStack.Peek();
            }
        }
    }
}
