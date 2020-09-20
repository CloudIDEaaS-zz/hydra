using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class VectorType : Type
    {
        private unsafe CppSharp.Parser.AST.VectorType vectorType;

        public unsafe VectorType(CppSharp.Parser.AST.VectorType vectorType) : base(vectorType)
        {
            this.vectorType = vectorType;
            this.vectorType.AssertNotNullAndOfType<CppSharp.Parser.AST.VectorType>();
        }

        public QualifiedType ElementType
        {
            get
            {
                if (vectorType.ElementType == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(vectorType.ElementType);
                }
            }
        }

        public IntegerValue NumElements
        {
            get
            {
                return new IntegerValue(vectorType.NumElements);
            }
        }
    }
}
