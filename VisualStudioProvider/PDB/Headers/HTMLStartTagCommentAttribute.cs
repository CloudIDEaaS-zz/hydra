using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class HTMLStartTagCommentAttribute
    {
        public HTMLStartTagComment OwningHTMLStartTagComment { get; set; }
        private unsafe CppSharp.Parser.AST.HTMLStartTagComment.Attribute attribute;

        public unsafe HTMLStartTagCommentAttribute(HTMLStartTagComment htmlStartTagComment, CppSharp.Parser.AST.HTMLStartTagComment.Attribute attribute)
        {
            this.OwningHTMLStartTagComment = htmlStartTagComment;
            this.attribute = attribute;
        }

        public string Name
        {
            get
            {
                return attribute.Name;
            }
        }

        public string Value
        {
            get
            {
                return attribute.Value;
            }
        }
    }
}
