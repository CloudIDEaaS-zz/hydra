using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class HTMLStartTagComment : HTMLTagComment
    {
        private unsafe CppSharp.Parser.AST.HTMLStartTagComment htmlStartTagComment;

        public unsafe HTMLStartTagComment(Headers.Comment owningComment, CppSharp.Parser.AST.HTMLStartTagComment htmlStartTagComment) : base(owningComment, htmlStartTagComment)
        {
            this.htmlStartTagComment = htmlStartTagComment;
        }

        public string TagName
        {
            get
            {
                return htmlStartTagComment.TagName;
            }
        }

        public IEnumerable<HTMLStartTagCommentAttribute> Attributes
        {
            get
            {
                foreach (var attribute in htmlStartTagComment.GetAttributes())
                {
                    yield return new HTMLStartTagCommentAttribute(this, attribute);
                }
            }
        }
    }
}
