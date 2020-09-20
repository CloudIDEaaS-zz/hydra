using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class HTMLEndTagComment : HTMLTagComment
    {
        private unsafe CppSharp.Parser.AST.HTMLEndTagComment htmlEndTagComment;

        public unsafe HTMLEndTagComment(Headers.Comment owningComment, CppSharp.Parser.AST.HTMLEndTagComment htmlEndTagComment) : base(owningComment, htmlEndTagComment)
        {
            this.htmlEndTagComment = htmlEndTagComment;
        }

        public string TagName
        {
            get
            {
                return htmlEndTagComment.TagName;
            }
        }
    }
}
