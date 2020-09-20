using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class EnumerationItem : Declaration
    {
        public Enumeration OwningEnumeration { get; set; }
        private unsafe CppSharp.Parser.AST.Enumeration.Item item;

        public unsafe EnumerationItem(Enumeration owningEnumeration, CppSharp.Parser.AST.Enumeration.Item item) : base(item)
        {
            this.OwningEnumeration = owningEnumeration;
            this.item = item;

            this.OwningEnumeration.AssertNotNull();
            this.item.AssertNotNullAndOfType<CppSharp.Parser.AST.Enumeration.Item>();
        }

        public unsafe EnumerationItem(CppSharp.Parser.AST.Enumeration.Item item) : base(item)
        {
            this.item = item;
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return item.Location.ID;
            }
        }

        public string Expression
        {
            get
            {
                return item.Expression;
            }
        }

        public IntegerValue Value
        {
            get
            {
                return new IntegerValue(item.Value);
            }
        }
    }
}
