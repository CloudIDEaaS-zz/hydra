using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class InlineCommandCommentArgument
    {
        public InlineCommandComment OwningInlineCommandComment { get; set; }
        private unsafe CppSharp.Parser.AST.InlineCommandComment.Argument argument;

        public unsafe InlineCommandCommentArgument(InlineCommandComment inlineCommandComment, CppSharp.Parser.AST.InlineCommandComment.Argument argument)
        {
            this.OwningInlineCommandComment = inlineCommandComment;
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
