using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class InlineCommandComment : InlineContentComment
    {
        private unsafe CppSharp.Parser.AST.InlineCommandComment inlineCommandComment;

        public unsafe InlineCommandComment(Headers.Comment owningComment, CppSharp.Parser.AST.InlineCommandComment inlineCommandComment) : base(owningComment, inlineCommandComment)
        {
            this.inlineCommandComment = inlineCommandComment;
        }

        public IntegerValue CommandId
        {
            get
            {
                return new IntegerValue(inlineCommandComment.CommandId);
            }
        }

        public string CommentRenderKind
        {
            get
            {
                return inlineCommandComment.CommentRenderKind.ToString();
            }
        }

        public IEnumerable<InlineCommandCommentArgument> Arguments
        {
            get
            {
                foreach (var argument in inlineCommandComment.GetArguments())
                {
                    yield return new InlineCommandCommentArgument(this, argument);
                }
            }
        }
    }
}
