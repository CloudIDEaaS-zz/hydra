using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeInterfaces.CodeParsers;

namespace CodeInterfaces.CodeParsers
{
    public class CompilationUnit : ICompilationUnit
    {
        public List<ICodeNode> Declarations
        {
            get { throw new NotImplementedException(); }
        }

        public ICodeFile File
        {
            get { throw new NotImplementedException(); }
        }
    }
}
