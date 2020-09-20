using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using Utils;

namespace VisualStudioProvider.PDB.Headers
{
    public class QualifiedType
    {
        private unsafe CppSharp.Parser.AST.QualifiedType qualifiedType;

        public unsafe QualifiedType(CppSharp.Parser.AST.QualifiedType qualifiedType)
        {
            this.qualifiedType = qualifiedType;
            this.qualifiedType.AssertNotNullAndOfType<CppSharp.Parser.AST.QualifiedType>();
        }

        public Type Type
        {
            get
            {
                if (qualifiedType.Type == null)
                {
                    return null;
                }
                else
                {
                    return qualifiedType.Type.GetRealTypeInternal();
                }
            }
        }

        public string Qualifiers
        {
            get
            {
                var builder = new StringBuilder();

                if (qualifiedType.Qualifiers.IsConst)
                {
                    builder.Append("Const");
                }

                if (qualifiedType.Qualifiers.IsRestrict)
                {
                    builder.AppendWithLeadingIfLength(",", "Restrict");
                }

                if (qualifiedType.Qualifiers.IsVolatile)
                {
                    builder.AppendWithLeadingIfLength(",", "Volatile");
                }

                return builder.ToString();
            }
        }
    }
}
