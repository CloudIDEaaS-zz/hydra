using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;

namespace AbstraX.ViewEngine.SemanticTree
{
    public class ViewDataNode : AssignmentNode
    {
        public string Key { get; }

        public ViewDataNode(SyntaxTreeNode syntaxTreeNode, string key, string contentPart) : base(NodeKind.ViewData, syntaxTreeNode, contentPart)
        {
            this.Key = key;
        }
    }
}
