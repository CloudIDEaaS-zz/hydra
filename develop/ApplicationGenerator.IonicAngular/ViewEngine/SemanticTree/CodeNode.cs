using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;
using VisualStudioProvider;

namespace AbstraX.ViewEngine.SemanticTree
{
    public class CodeNode : BaseNode
    {
        protected SyntaxTree syntaxTree;
        protected CompilationUnitSyntax syntaxRoot;
        protected RazorParserVisitor visitor;
        protected SemanticModel model;
        private ViewProject viewProject;
        private VSProject project;
        public string Code { get; protected set; }
        public string CumulativeCode { get; protected set; }

        public CodeNode(RazorParserVisitor visitor, SyntaxTreeNode syntaxTreeNode, string contentPart) : base(NodeKind.Code, syntaxTreeNode, contentPart)
        {
            this.visitor = visitor;
            this.viewProject = visitor.ViewProject;
            this.project = viewProject.Project;

            Parse();
        }

        public CodeNode(RazorParserVisitor visitor, NodeKind kind, SyntaxTreeNode syntaxTreeNode, string contentPart) : base(kind, syntaxTreeNode, contentPart)
        {
            this.visitor = visitor;
            this.viewProject = visitor.ViewProject;
            this.project = viewProject.Project;
        }

        private CSharpCompilation CreateCompilation()
        {
            var projectName = project.Name;
            var compilation = CSharpCompilation.Create(projectName);
            var referencedAssemblies = project.Items.Where(i => i.ItemType == "Reference");

            compilation = compilation.AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

            foreach (var reference in referencedAssemblies)
            {
                var referencePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(project.FileName), reference.RelativePath));

                compilation = compilation.AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            }

            return compilation;
        }

        protected CompilationUnitSyntax Parse()
        {
            var compilation = CreateCompilation();
            var code = GetCumulativeCode();

            syntaxTree = CSharpSyntaxTree.ParseText(code);
            syntaxRoot = (CompilationUnitSyntax)syntaxTree.GetRoot();

            compilation = compilation.AddSyntaxTrees(syntaxTree);
            model = compilation.GetSemanticModel(syntaxTree);

            return syntaxRoot;
        }

        protected virtual string GetCumulativeCode()
        {
            var codeNodes = visitor.CodeNodes;
            var builder = new StringBuilder();

            foreach (var node in codeNodes)
            {
                builder.Append(node.Code);
            }

            this.Code = this.ContentPart;
            builder.Append(this.Code);

            this.CumulativeCode = builder.ToString();

            return visitor.WrapRenderAndClass(builder);
        }

        public override void Accept(RazorSemanticVisitor visitor)
        {
        }
    }
}
