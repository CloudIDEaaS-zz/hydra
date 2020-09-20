using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class MemberPointerType : Type
    {
        private unsafe CppSharp.Parser.AST.MemberPointerType memberPointerType;

        public unsafe MemberPointerType(CppSharp.Parser.AST.MemberPointerType memberPointerType) : base(memberPointerType)
        {
            this.memberPointerType = memberPointerType;
            this.memberPointerType.AssertNotNullAndOfType<CppSharp.Parser.AST.MemberPointerType>();
        }

        public QualifiedType Pointee
        {
            get
            {
                if (memberPointerType.Pointee == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(memberPointerType.Pointee);
                }
            }
        }
    }
}
