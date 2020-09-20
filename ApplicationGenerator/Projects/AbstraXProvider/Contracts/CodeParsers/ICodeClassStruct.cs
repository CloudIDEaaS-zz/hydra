using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstraX.Contracts.CodeParsers
{
    public interface ICodeClassStruct : ICodeNode
    {
        List<ICodeNode> MemberDeclarations { get; }
    }
}
