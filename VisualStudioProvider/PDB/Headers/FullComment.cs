using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class FullComment : Comment
    {
        private unsafe CppSharp.Parser.AST.FullComment fullComment;
        public Comment OwningComment { get; set; }
        public RawComment OwningRawComment { get; set; }

        public unsafe FullComment(Comment owningComment, CppSharp.Parser.AST.FullComment fullComment) : base(fullComment)
        {
            this.OwningComment = owningComment;
            this.fullComment = fullComment;
        }

        public unsafe FullComment(RawComment owningRawComment, CppSharp.Parser.AST.FullComment fullComment) : base(fullComment)
        {
            this.OwningRawComment = owningRawComment;
            this.fullComment = fullComment;
        }

        public IEnumerable<BlockContentComment> Blocks
        {
            get
            {
                foreach (var block in fullComment.GetBlocks())
                {
                    yield return new BlockContentComment(this, block);
                }
            }
        }
    }
}
