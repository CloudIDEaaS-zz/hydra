using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class TypedefType : Type
    {
        private unsafe CppSharp.Parser.AST.TypedefType typedefType;

        public unsafe TypedefType(CppSharp.Parser.AST.TypedefType typedefType) : base(typedefType)
        {
            this.typedefType = typedefType;
            this.typedefType.AssertNotNullAndOfType<CppSharp.Parser.AST.TypedefType>();
        }

        public Declaration Declaration
        {
            get
            {
                if (typedefType.Declaration == null)
                {
                    return null;
                }
                else
                {
                    return typedefType.Declaration.GetRealDeclarationInternal();
                }
            }
        }
    }
}
