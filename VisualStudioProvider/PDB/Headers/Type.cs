using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class Type
    {
        private unsafe CppSharp.Parser.AST.Type type;

        public unsafe Type(CppSharp.Parser.AST.Type type)
        {
            this.type = type;
            this.type.AssertNotNull();
        }

        public string Kind
        {
            get
            {
                return type.Kind.ToString();
            }
        }

        public bool IsDependent
        {
            get
            {
                return type.IsDependent;
            }
        }
    }
}
