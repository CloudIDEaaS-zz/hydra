using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;

namespace AbstraX.ViewEngine.SemanticTree
{
    public class PropertyNode : AssignmentNode
    {
        public PropertyNode(SyntaxTreeNode syntaxTreeNode, string contentPart) : base(NodeKind.Property, syntaxTreeNode, contentPart)
        {
        }
    }
}
