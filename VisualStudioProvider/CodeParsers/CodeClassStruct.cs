using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeInterfaces.CodeParsers
{
    public class CodeClassStruct : ICodeClassStruct
    {
        public List<ICodeNode> MemberDeclarations
        {
            get { throw new NotImplementedException(); }
        }
    }
}
