using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.WorkspaceHandlers.CSharpWorkspaceFileType.Nodes
{
    public class ElseBlock : IfDirectiveNodeBase
    {
        public ElseBlock(ElseDirectiveTriviaSyntax node) : base(node)
        {
        }

        public override string Code
        {
            get
            {
                return this.Node.ToFullString();
            }
        }
    }
}
