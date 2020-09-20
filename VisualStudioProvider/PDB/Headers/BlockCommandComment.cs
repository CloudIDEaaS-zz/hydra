using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class BlockCommandComment : BlockContentComment
    {
        private unsafe CppSharp.Parser.AST.BlockCommandComment blockCommandComment;

        public unsafe BlockCommandComment(Comment owningComment, CppSharp.Parser.AST.BlockCommandComment blockCommandComment) : base(owningComment, blockCommandComment)
        {
            this.blockCommandComment = blockCommandComment;

            this.blockCommandComment.AssertNotNullAndOfType<CppSharp.Parser.AST.BlockCommandComment>();
        }

        public ParagraphComment ParagraphComment
        {
            get
            {
                return new ParagraphComment(this.OwningComment, blockCommandComment.ParagraphComment);
            }
        }

        public IEnumerable<BlockCommandCommentArgument> Arguments
        {
            get
            {
                foreach (var argument in blockCommandComment.GetArguments())
                {
                    yield return new BlockCommandCommentArgument(this, argument);
                }
            }
        }
    }
}