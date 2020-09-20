using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class Field : Declaration
    {
        public Class OwningClass { get; set; }
        public int FieldIndex { get; set; }
        private unsafe CppSharp.Parser.AST.Field field;

        public unsafe Field(Class owningClass, CppSharp.Parser.AST.Field field, int fieldIndex) : base(field)
        {
            this.OwningClass = owningClass;
            this.field = field;
            this.FieldIndex = fieldIndex;
            this.field.AssertNotNullAndOfType<CppSharp.Parser.AST.Field>();
        }

        public unsafe Field(CppSharp.Parser.AST.Field field) : base(field)
        {
            this.field = field;
            this.field.AssertNotNullAndOfType<CppSharp.Parser.AST.Field>();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return field.Location.ID;
            }
        }

        public IntegerValue BitWidth
        {
            get
            {
                return new IntegerValue(field.BitWidth);
            }
        }

        public bool IsBitField
        {
            get
            {
                return field.IsBitField;
            }
        }

        public QualifiedType QualifiedType
        {
            get
            {
                if (field.QualifiedType == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(field.QualifiedType);
                }
            }
        }
    }
}
