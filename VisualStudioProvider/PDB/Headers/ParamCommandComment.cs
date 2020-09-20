using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class ParamCommandComment : BlockCommandComment
    {
        private unsafe CppSharp.Parser.AST.ParamCommandComment paramCommandComment;

        public unsafe ParamCommandComment(Comment owningComment, CppSharp.Parser.AST.ParamCommandComment paramCommandComment) : base(owningComment, paramCommandComment)
        {
            this.paramCommandComment = paramCommandComment;

            this.paramCommandComment.AssertNotNullAndOfType<CppSharp.Parser.AST.ParamCommandComment>();
        }

        public IntegerValue ParamIndex
        {
            get
            {
                return new IntegerValue(paramCommandComment.ParamIndex);
            }
        }

        public string Direction
        {
            get
            {
                return paramCommandComment.Direction.ToString();
            }
        }
    }
}