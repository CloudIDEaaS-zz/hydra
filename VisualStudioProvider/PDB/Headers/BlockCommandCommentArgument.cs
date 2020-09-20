using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class BlockCommandCommentArgument
    {
        public BlockCommandComment OwningBlockCommandComment { get; set; }
        private unsafe CppSharp.Parser.AST.BlockCommandComment.Argument argument;

        public unsafe BlockCommandCommentArgument(BlockCommandComment blockCommandComment, CppSharp.Parser.AST.BlockCommandComment.Argument argument)
        {
            this.OwningBlockCommandComment = blockCommandComment;
            this.argument = argument;
        }

        public string Text
        {
            get
            {
                return argument.Text;
            }
        }
    }
}
