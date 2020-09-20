using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class HTMLTagComment : InlineContentComment
    {
        private unsafe CppSharp.Parser.AST.HTMLTagComment htmlTagComment;

        public unsafe HTMLTagComment(Headers.Comment owningComment, CppSharp.Parser.AST.HTMLTagComment htmlTagComment) : base(owningComment, htmlTagComment)
        {
            this.htmlTagComment = htmlTagComment;
        }
    }
}
