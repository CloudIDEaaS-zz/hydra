using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces.CodeParsers
{
    public interface ICodeClassStruct : ICodeNode
    {
        List<ICodeNode> MemberDeclarations { get; }
    }
}
