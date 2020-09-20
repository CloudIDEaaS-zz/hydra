using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class VerbatimLineComment : BlockCommandComment
    {
        private unsafe CppSharp.Parser.AST.VerbatimLineComment verbatimLineComment;

        public unsafe VerbatimLineComment(Comment owningComment, CppSharp.Parser.AST.VerbatimLineComment verbatimLineComment) : base(owningComment, verbatimLineComment)
        {
            this.verbatimLineComment = verbatimLineComment;

            this.verbatimLineComment.AssertNotNullAndOfType<CppSharp.Parser.AST.VerbatimLineComment>();
        }

        public string Text
        {
            get
            {
                return verbatimLineComment.Text;
            }
        }
    }
}