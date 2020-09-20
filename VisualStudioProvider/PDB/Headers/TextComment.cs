using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class TextComment : InlineContentComment
    {
        private unsafe CppSharp.Parser.AST.TextComment textComment;

        public unsafe TextComment(Headers.Comment owningComment, CppSharp.Parser.AST.TextComment textComment) : base(owningComment, textComment)
        {
            this.textComment = textComment;
        }

        public string Text
        {
            get
            {
                return textComment.Text;
            }
        }
    }
}
