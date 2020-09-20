using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class PointerType : Type
    {
        private unsafe CppSharp.Parser.AST.PointerType pointerType;

        public unsafe PointerType(CppSharp.Parser.AST.PointerType type) : base(type)
        {
            this.pointerType = type;
            this.pointerType.AssertNotNullAndOfType<CppSharp.Parser.AST.PointerType>();
        }

        public string Modifier
        {
            get
            {
                if (pointerType.Modifier == null)
                {
                    return null;
                }
                else
                {
                    return pointerType.Modifier.ToString();
                }
            }
        }

        public QualifiedType QualifiedPointee
        {
            get
            {
                if (pointerType.QualifiedPointee == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(pointerType.QualifiedPointee);
                }
            }
        }
    }
}
