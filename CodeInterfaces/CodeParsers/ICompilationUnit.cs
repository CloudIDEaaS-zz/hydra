using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces.CodeParsers
{
    public interface ICompilationUnit
    {
        List<ICodeNode> Declarations { get; }
        ICodeFile File { get; }
    }
}
