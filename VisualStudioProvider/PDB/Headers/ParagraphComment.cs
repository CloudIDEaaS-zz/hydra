using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class ParagraphComment : BlockContentComment
    {
        private unsafe CppSharp.Parser.AST.ParagraphComment paragraphComment;

        public unsafe ParagraphComment(Comment owningComment, CppSharp.Parser.AST.ParagraphComment paragraphComment) : base(owningComment, paragraphComment)
        {
            this.paragraphComment = paragraphComment;

            this.paragraphComment.AssertNotNullAndOfType<CppSharp.Parser.AST.ParagraphComment>();
        }

        public IEnumerable<InlineContentComment> Content
        {
            get
            {
                foreach (var content in paragraphComment.GetContent())
                {
                    yield return new InlineContentComment(this, content);
                }
            }
        }

        public bool IsWhitespace
        {
            get
            {
                return paragraphComment.IsWhitespace;
            }
        }
    }
}