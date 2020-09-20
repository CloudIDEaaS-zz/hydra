using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class VerbatimBlockLineComment : Comment
    {
        private unsafe CppSharp.Parser.AST.VerbatimBlockLineComment line;
        public Comment OwningComment { get; private set; }

        public unsafe VerbatimBlockLineComment(CppSharp.Parser.AST.VerbatimBlockLineComment line) : base(line)
        {
            this.line = line;
        }

        public unsafe VerbatimBlockLineComment(Comment owningComment, CppSharp.Parser.AST.VerbatimBlockLineComment line) : base(line)
        {
            this.OwningComment = owningComment;
            this.line = line;

            this.OwningComment.AssertNotNull();
            this.line.AssertNotNullAndOfType<CppSharp.Parser.AST.VerbatimBlockLineComment>();
        }

        public string Text
        {
            get
            {
                return line.Text;
            }
        }
    }
}
