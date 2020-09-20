using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class VerbatimBlockComment : BlockCommandComment
    {
        private unsafe CppSharp.Parser.AST.VerbatimBlockComment verbatimBlockComment;

        public unsafe VerbatimBlockComment(Comment owningComment, CppSharp.Parser.AST.VerbatimBlockComment verbatimBlockComment) : base(owningComment, verbatimBlockComment)
        {
            this.verbatimBlockComment = verbatimBlockComment;

            this.verbatimBlockComment.AssertNotNullAndOfType<CppSharp.Parser.AST.VerbatimBlockComment>();
        }

        public IEnumerable<VerbatimBlockLineComment> Lines
        {
            get
            {
                foreach (var line in verbatimBlockComment.GetLines())
                {
                    yield return new VerbatimBlockLineComment(this, line);
                }
            }
        }
    }
}