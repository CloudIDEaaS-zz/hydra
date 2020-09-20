using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class CommentBlock : Comment
    {
        private unsafe CppSharp.Parser.AST.FullComment fullComment;

        public unsafe CommentBlock(CppSharp.Parser.AST.FullComment fullComment) : base(fullComment)
        {
            this.fullComment = fullComment;
        }

        public string Kind 
        {
            get
            {
                return fullComment.Kind.ToString();
            }
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
