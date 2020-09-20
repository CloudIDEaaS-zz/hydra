using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class BlockContentComment : Comment
    {
        private unsafe CppSharp.Parser.AST.BlockContentComment block;
        public Comment OwningComment { get; set; }

        public unsafe BlockContentComment(Comment owningComment, CppSharp.Parser.AST.BlockContentComment block) : base(block)
        {
            this.OwningComment = owningComment;
            this.block = block;
            this.block.AssertNotNullAndOfType<CppSharp.Parser.AST.BlockContentComment>();
        }
    }
}
