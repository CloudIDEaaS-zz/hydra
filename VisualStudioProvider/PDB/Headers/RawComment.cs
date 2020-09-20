using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class RawComment
    {
        private unsafe CppSharp.Parser.AST.RawComment rawComment;

        public RawComment(Declaration parentDeclaration, CppSharp.Parser.AST.RawComment rawComment)
        {
            this.rawComment = rawComment;
            this.rawComment.AssertNotNullAndOfType<CppSharp.Parser.AST.RawComment>();
        }

        public string BriefText
        {
            get
            {
                return rawComment.BriefText;
            }
        }

        public string Text
        {
            get
            {
                return rawComment.Text;
            }
        }

        public string Kind
        {
            get
            {
                return rawComment.Kind.ToString();
            }
        }

        public FullComment CommentBlock
        {
            get
            {
                if (rawComment.FullCommentBlock == null)
                {
                    return null;
                }
                else
                {
                    return new FullComment(this, rawComment.FullCommentBlock);
                }
            }
        }
    }
}
