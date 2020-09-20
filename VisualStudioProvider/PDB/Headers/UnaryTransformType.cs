using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class UnaryTransformType : Type
    {
        private unsafe CppSharp.Parser.AST.UnaryTransformType unaryTransformType;

        public unsafe UnaryTransformType(CppSharp.Parser.AST.UnaryTransformType unaryTransformType) : base(unaryTransformType)
        {
            this.unaryTransformType = unaryTransformType;
            this.unaryTransformType.AssertNotNullAndOfType<CppSharp.Parser.AST.UnaryTransformType>();
        }

        public QualifiedType Desugarded
        {
            get
            {
                if (unaryTransformType.Desugared == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(unaryTransformType.Desugared);
                }
            }
        }

        public QualifiedType BaseType
        {
            get
            {
                if (unaryTransformType.BaseType == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(unaryTransformType.BaseType);
                }
            }
        }
    }
}
