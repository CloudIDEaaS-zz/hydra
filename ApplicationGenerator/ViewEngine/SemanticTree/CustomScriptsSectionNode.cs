using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;

namespace AbstraX.ViewEngine.SemanticTree
{
    public class CustomScriptsSectionNode : SectionNode
    {
        public CustomScriptsSectionNode(SyntaxTreeNode syntaxTreeNode, string contentPart, string name) : base(NodeKind.CustomScriptsSection, syntaxTreeNode, contentPart, name)
        {
        }
    }
}
