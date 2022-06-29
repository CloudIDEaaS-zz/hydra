using AbstraX.ServerInterfaces;
using AbstraX.ViewEngine.SemanticTree;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor;
using Utils;

namespace AbstraX.ViewEngine
{
    [DebuggerDisplay(" ViewPath={ File } ")]
    public class View : IView
    {
        private ViewProject viewProject;
        public string Name { get; }
        public string File { get; }
        public RootNode SemanticTree { get; }
        public bool Generated { get; set; }
        public Dictionary<string, object> ViewDataDictionary { get; }
        public IBase Model { get; set; }

        public IViewProject ViewProject
        {
            get
            {
                return viewProject;
            }
        }

        public View(string viewRelativePath, ViewProject viewProject)
        {
            var viewFile = Path.Combine(viewProject.ViewsPath, viewRelativePath.BackSlashes()) + ".cshtml";
            var viewName = Path.GetFileNameWithoutExtension(viewFile);
            var viewReader = System.IO.File.OpenText(Path.Combine(viewFile));
            var host = new RazorEngineHost(new CSharpRazorCodeLanguage());
            var engine = new RazorTemplateEngine(host);
            var parserResults = engine.ParseTemplate(viewReader);
            var document = parserResults.Document;
            var visitor = new RazorParserVisitor(viewReader, viewRelativePath, viewProject);

            this.Name = viewName;
            this.viewProject = viewProject;
            this.File = viewFile;
            this.ViewDataDictionary = new Dictionary<string, object>();

            visitor.OnPartialView += Visitor_OnPartialView;

            document.Accept(visitor);

            this.SemanticTree = visitor.RootNode;
        }

        public string GetLayout()
        {
            var layoutNode = this.SemanticTree.AllNodes.OfType<PropertyNode>().SingleOrDefault(p => p.Left is string && ((string)p.Left) == "Layout");

            if (layoutNode == null)
            {
                return null;
            }
            else
            {
                return (string)((LiteralExpressionSyntax) layoutNode.Right).Token.ValueText;
            }
        }

        private void Visitor_OnPartialView(object target, PartialViewEventArgs e)
        {
            var arguments = e.Arguments;
            var partialViewPath = ((LiteralExpressionSyntax)arguments[0].Expression).Token.Text.RemoveQuotes();
            var relativePath = partialViewPath;

            if (!viewProject.ContainsView(relativePath))
            {
                e.View = viewProject.AddView(relativePath);
            }
            else
            {
                e.View = viewProject.Views[relativePath];
            }
        }
    }
}
