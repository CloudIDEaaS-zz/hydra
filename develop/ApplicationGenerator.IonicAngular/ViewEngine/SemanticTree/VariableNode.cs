using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Parser.SyntaxTree;

namespace AbstraX.ViewEngine.SemanticTree
{
    public class VariableNode : AssignmentNode
    {
        public string Name { get; set; }

        public VariableNode(string name, SyntaxTreeNode syntaxTreeNode, string contentPart) : base(NodeKind.Variable, syntaxTreeNode, contentPart)
        {
            this.Name = name;
            this.Left = name;
        }
    }
}
