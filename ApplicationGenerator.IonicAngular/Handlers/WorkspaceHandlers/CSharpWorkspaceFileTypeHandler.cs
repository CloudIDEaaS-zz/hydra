using AbstraX.Handlers.WorkspaceHandlers.CSharpWorkspaceFileType;
using AbstraX.Handlers.WorkspaceHandlers.CSharpWorkspaceFileType.Nodes;
using CodeInterfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Utils.Hierarchies;

namespace AbstraX.Handlers.WorkspaceHandlers
{
    public class CSharpWorkspaceFileTypeHandler : IWorkspaceFileTypeHandler
    {
        public string[] FileNameExpressions { get; private set; }
        public string[] TokensToProcess { get; private set; }
        public string OutputContent { get; private set; }
        public float Priority => 1.0f;
        private string[] supportedTokens;

        public CSharpWorkspaceFileTypeHandler()
        {
            this.FileNameExpressions = new string[] { @".*?\.cs$" };
        }

        public bool PreProcess(Guid projectType, string appName, string rawFileRelativePath, string outputFileName, string[] supportedTokens, string content, IGeneratorConfiguration generatorConfiguration)
        {
            var options = CSharpParseOptions.Default.WithPreprocessorSymbols(supportedTokens);
            var syntaxTree = CSharpSyntaxTree.ParseText(content, options);
            var root = syntaxTree.GetCompilationUnitRoot();
            var walker = new DirectiveSyntaxWalker(supportedTokens);
            var tokensToProcess = new List<string>();
            List<IfDirectiveBlock> generatorDirectiveBlocks;

            this.supportedTokens = supportedTokens;
            this.OutputContent = content;

            walker.Visit(root);

            generatorDirectiveBlocks = walker.GeneratorDirectiveBlocks;

            foreach (var generatorDirectiveBlock in generatorDirectiveBlocks)
            {
                var childNodes = generatorDirectiveBlock.ChildNodes.Reverse();
                IfDirectiveNodeBase branchTakenNode = null;

                foreach (var childNode in childNodes.Where(n => !n.IsOfType<EndIfBlock>()))
                {
                    if (childNode.BranchTaken.Value)
                    {
                        branchTakenNode = childNode;
                    }
                }

                if (branchTakenNode != null)
                {
                    var startSpan = branchTakenNode.Node.Span;
                    var endSpan = branchTakenNode.NextNode.Node.Span;

                    switch (branchTakenNode)
                    {
                        case IfBlock ifBlock:

                            tokensToProcess.Add(ifBlock.Condition);
                            break;

                        case ElifBlock elifBlock:

                            tokensToProcess.Add(elifBlock.Condition);
                            break;

                    }
                }
            }

            this.TokensToProcess = tokensToProcess.ToArray();

            return true;
        }

        public bool Process(Dictionary<string, IWorkspaceTokenContentHandler> tokenContentHandlers, IWorkspaceTemplate workspaceTemplate, Guid projectType, string appName, string rawFileRelativePath, string outputFileName, string content, IGeneratorConfiguration generatorConfiguration)
        {
            var processing = true;
            var options = CSharpParseOptions.Default.WithPreprocessorSymbols(supportedTokens);
            IEnumerable<Diagnostic> diagnostics = null;

            this.OutputContent = content;

            while (processing)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(content, options);
                var root = syntaxTree.GetCompilationUnitRoot();
                var walker = new DirectiveSyntaxWalker(supportedTokens);
                List<IfDirectiveBlock> generatorDirectiveBlocks;

                walker.Visit(root);

                generatorDirectiveBlocks = walker.GeneratorDirectiveBlocks;

                if (generatorDirectiveBlocks.Count > 0)
                {
                    var generatorDirectiveBlock = generatorDirectiveBlocks.First();
                    var childNodes = generatorDirectiveBlock.ChildNodes.Reverse();
                    IfDirectiveNodeBase branchTakenNode = null;

                    foreach (var childNode in childNodes.Where(n => !n.IsOfType<EndIfBlock>()))
                    {
                        if (childNode.BranchTaken.Value)
                        {
                            branchTakenNode = childNode;
                        }
                        else if (branchTakenNode != null)
                        {
                            var offsetLength = branchTakenNode.Node.FullSpan.Length;
                            var length = childNode.Length - offsetLength;

                            content = content.Remove(childNode.Start, length);
                            childNodes.ShiftUp(childNode.Start, length);
                        }
                        else
                        {
                            content = content.Remove(childNode.Start, childNode.Length);
                            childNodes.ShiftUp(childNode.Start, childNode.Length);
                        }
                    }

                    if (branchTakenNode != null)
                    {
                        var startSpan = new TextSpan(branchTakenNode.Node.FullSpan.Start + branchTakenNode.ShiftAmount, branchTakenNode.Node.FullSpan.Length);
                        var endSpan = new TextSpan(branchTakenNode.NextNode.Node.FullSpan.Start + branchTakenNode.ShiftAmount, branchTakenNode.NextNode.Node.FullSpan.Length);

                        switch (branchTakenNode)
                        {
                            case IfBlock ifBlock:

                                if (tokenContentHandlers.ContainsKey(ifBlock.Condition))
                                {
                                    var tokenContentHandler = tokenContentHandlers[ifBlock.Condition];
                                    var insertContent = tokenContentHandler.Content;

                                    content = content.Remove(ifBlock.Start, ifBlock.Length);
                                    content = content.Insert(ifBlock.Start, insertContent);
                                }
                                else
                                {
                                    content = content.Remove(endSpan.Start, endSpan.Length);
                                    content = content.Remove(startSpan.Start, startSpan.Length);
                                }

                                break;

                            case ElifBlock elifBlock:

                                DebugUtils.Break();  // untested

                                if (tokenContentHandlers.ContainsKey(elifBlock.Condition))
                                {
                                    var tokenContentHandler = tokenContentHandlers[elifBlock.Condition];
                                    var insertContent = tokenContentHandler.Content;

                                    content = content.Remove(elifBlock.Start, elifBlock.Length);
                                    content = content.Insert(elifBlock.Start, insertContent);
                                }
                                else
                                {
                                    content = content.Remove(endSpan.Start, endSpan.Length);
                                    content = content.Remove(startSpan.Start, startSpan.Length);
                                }

                                break;

                            case ElseBlock elseBlock:

                                content = content.Remove(endSpan.Start, endSpan.Length);
                                content = content.Remove(startSpan.Start, startSpan.Length);

                                break;
                        }
                    }
                }
                else
                {
                    processing = false;
                }

                diagnostics = syntaxTree.GetDiagnostics();
            }

            if (diagnostics.Count() > 0)
            {
                DebugUtils.Break();
            }

            this.OutputContent = content;

            return true;
        }

        public void PostProcess(IAppFolderStructureSurveyor surveyor)
        {
        }
    }
}
