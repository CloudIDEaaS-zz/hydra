using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;

namespace AbstraX.ViewEngine.SemanticTree
{
    public class ViewBagNode : AssignmentNode
    {
        public string PropertyName { get; }

        public ViewBagNode(SyntaxTreeNode syntaxTreeNode, string propertyName, string contentPart) : base(NodeKind.ViewData, syntaxTreeNode, contentPart)
        {
            this.PropertyName = propertyName;
        }
    }
}
