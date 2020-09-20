using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class AttributedType : Type
    {
        private unsafe CppSharp.Parser.AST.AttributedType attributedType;

        public unsafe AttributedType(CppSharp.Parser.AST.AttributedType attributedType) : base(attributedType)
        {
            this.attributedType = attributedType;
            this.attributedType.AssertNotNullAndOfType<CppSharp.Parser.AST.AttributedType>();
        }

        public QualifiedType Equivalent
        {
            get
            {
                if (attributedType.Equivalent == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(attributedType.Equivalent);
                }
            }
        }

        public QualifiedType Modified
        {
            get
            {
                if (attributedType.Modified == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(attributedType.Modified);
                }
            }
        }
    }
}
