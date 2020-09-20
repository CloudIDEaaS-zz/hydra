using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class TParamCommandComment : BlockCommandComment
    {
        private unsafe CppSharp.Parser.AST.TParamCommandComment tParamCommandComment;

        public unsafe TParamCommandComment(Comment owningComment, CppSharp.Parser.AST.TParamCommandComment tParamCommandComment) : base(owningComment, tParamCommandComment)
        {
            this.tParamCommandComment = tParamCommandComment;

            this.tParamCommandComment.AssertNotNullAndOfType<CppSharp.Parser.AST.TParamCommandComment>();
        }

        public IEnumerable<IntegerValue> Positions
        {
            get
            {
                foreach (var position in tParamCommandComment.GetPositions())
                {
                    yield return new IntegerValue(position);
                }
            }
        }
    }
}