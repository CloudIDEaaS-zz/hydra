using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;

namespace VisualStudioProvider.PDB.Headers
{
    public class ArrayType : Type
    {
        private unsafe CppSharp.Parser.AST.ArrayType arrayType;

        public unsafe ArrayType(CppSharp.Parser.AST.ArrayType arrayType) : base(arrayType)
        {
            this.arrayType = arrayType;
            this.arrayType.AssertNotNullAndOfType<CppSharp.Parser.AST.ArrayType>();
        }

        public int ElementSize
        {
            get
            {
                return arrayType.ElementSize;
            }
        }

        public QualifiedType QualifiedType
        {
            get
            {
                if (arrayType.QualifiedType == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(arrayType.QualifiedType);
                }
            }
        }

        public int Size
        {
            get
            {
                return arrayType.Size;
            }
        }

        public string SizeType
        {
            get
            {
                return arrayType.SizeType.ToString();
            }
        }
    }
}
