using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.ViewEngine.SemanticTree
{
    [Flags]
    public enum NodeKind : int
    {
        Root = 1 << 0,
        Directive = 1 << 1,
        MarkupBlock = 1 << 2,
        Model = 1 << 3,
        Code = 1 << 4,
        ViewData = 1 << 5,
        Property = 1 << 6,
        Variable = 1 << 7,
        Section = 1 << 8,
        Render = 1 << 9,
        Markup = 1 << 10,
        Comment = 1 << 11,
        CodeExpression = 1 << 12,
        HelperExpression = 1 << 13,
        CustomScriptsSection = 1 << 14,
        ScriptExpression = 1 << 15,
        ModelInvocation = 1 << 16
    }
}
