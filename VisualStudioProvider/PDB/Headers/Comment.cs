using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace VisualStudioProvider.PDB.Headers
{
    public class Comment
    {
        private unsafe CppSharp.Parser.AST.Comment comment;
        public unsafe Declaration OwningDeclaration { get; set; }

        public unsafe Comment(Declaration owningDeclaration, CppSharp.Parser.AST.RawComment rawComment)
        {
            this.OwningDeclaration = owningDeclaration;
        }

        public unsafe Comment(CppSharp.Parser.AST.Comment comment)
        {
            this.comment = comment;
            this.comment.AssertNotNull();
        }

        public string Kind
        {
            get
            {
                return comment.Kind.ToString();
            }
        }
    }
}
