using AbstraX.Handlers.WorkspaceHandlers.CSharpWorkspaceFileType.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Handlers.WorkspaceHandlers.CSharpWorkspaceFileType
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A directive syntax walker.
    ///             
    ///             Only supports single condition without operators
    ///             TODO: support multi-condition, and operators
    ///             
    ///             Other future features:
    ///                 Use with facet handler output
    ///                 Use to mark user modified code regions
    ///
    /// <remarks>   Ken, 9/6/2020. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class DirectiveSyntaxWalker : CSharpSyntaxWalker
    {
        private string[] supportedTokens;
        private Stack<IfDirectiveBlock> ifDirectiveBlockStack;
        public List<IfDirectiveBlock> RootIfDirectiveBlocks { get; }
        public List<IfDirectiveBlock> IfDirectiveBlocks { get; }

        public DirectiveSyntaxWalker(string[] supportedTokens) : base(Microsoft.CodeAnalysis.SyntaxWalkerDepth.StructuredTrivia)
        {
            this.supportedTokens = supportedTokens;
            this.ifDirectiveBlockStack = new Stack<IfDirectiveBlock>();

            this.RootIfDirectiveBlocks = new List<IfDirectiveBlock>();
            this.IfDirectiveBlocks = new List<IfDirectiveBlock>();
        }

        public List<IfDirectiveBlock> GeneratorDirectiveBlocks
        {
            get
            {
                return this.IfDirectiveBlocks.Where(b => b.IsGeneratorToken).ToList();
            }
        }

        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            base.VisitCompilationUnit(node);

            foreach (var ifDirectiveBlock in this.IfDirectiveBlocks)
            {
                ifDirectiveBlock.CalculateSpans();
            }
        }

        public override void VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
        {
            var conditionToken = node.Condition.GetText().ToString();
            var isGeneratorToken = conditionToken.StartsWith("GENERATOR_TOKEN_");
            IfDirectiveBlock ifDirectiveBlock = new IfDirectiveBlock(node, conditionToken, isGeneratorToken);

            if (ifDirectiveBlockStack.Count == 0)
            {
                this.RootIfDirectiveBlocks.Add(ifDirectiveBlock);
            }
            else
            {
                var ifDirectiveBlockPeek = ifDirectiveBlockStack.Peek();
                var activeBlock = ifDirectiveBlockPeek.ActiveBlock;

                activeBlock.AddChild(ifDirectiveBlock);
            }

            this.IfDirectiveBlocks.Add(ifDirectiveBlock);
            ifDirectiveBlockStack.Push(ifDirectiveBlock);

            base.VisitIfDirectiveTrivia(node);
        }

        public override void VisitElifDirectiveTrivia(ElifDirectiveTriviaSyntax node)
        {
            var ifDirectiveBlock = ifDirectiveBlockStack.Peek();
            var activeBlock = ifDirectiveBlock.ActiveBlock;
            var elifBlock = new ElifBlock(node);

            activeBlock.AddChild(elifBlock);

            base.VisitElifDirectiveTrivia(node);
        }

        public override void VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node)
        {
            var ifDirectiveBlock = ifDirectiveBlockStack.Peek();
            var elseBlock = new ElseBlock(node);

            ifDirectiveBlock.AddChild(elseBlock);

            base.VisitElseDirectiveTrivia(node);
        }

        public override void VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node)
        {
            var ifDirectiveBlock = ifDirectiveBlockStack.Pop();
            var endIfBlock = new EndIfBlock(node);

            ifDirectiveBlock.AddChild(endIfBlock);

            base.VisitEndIfDirectiveTrivia(node);
        }
    }
}
