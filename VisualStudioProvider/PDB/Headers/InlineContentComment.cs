using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class InlineContentComment : Comment
    {
        public Comment OwningComment { get; set; }
        private unsafe CppSharp.Parser.AST.InlineContentComment content;

        public unsafe InlineContentComment(Comment owningComment, CppSharp.Parser.AST.InlineContentComment content) : base(content)
        {
            this.OwningComment = owningComment;
            this.content = content;
        }

        public bool HasTrailingNewline
        {
            get
            {
                return content.HasTrailingNewline;
            }
        }
    }
}
