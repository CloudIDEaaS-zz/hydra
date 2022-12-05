using AbstraX.Handlers.WorkspaceHandlers.CSharpWorkspaceFileType.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Handlers.WorkspaceHandlers.CSharpWorkspaceFileType
{
    public static class Extensions
    {
        public static void ShiftUp(this IEnumerable<IfDirectiveNodeBase> childNodes, int startIndex, int lengthToShift)
        {
            foreach (var childNode in childNodes.Where(n => n.Start > startIndex))
            {
                childNode.ShiftAmount -= lengthToShift;
            }
        }
    }
}
