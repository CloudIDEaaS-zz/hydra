using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class BaseClassSpecifier
    {
        private unsafe CppSharp.Parser.AST.BaseClassSpecifier specifier;
        public Class OwningClass { get; set; }

        public unsafe BaseClassSpecifier(Class owningClass, CppSharp.Parser.AST.BaseClassSpecifier specifier)
        {
            this.OwningClass = owningClass;
            this.specifier = specifier;

            this.OwningClass.AssertNotNull();
            this.specifier.AssertNotNullAndOfType<CppSharp.Parser.AST.BaseClassSpecifier>();
        }

        public Type Type
        {
            get
            {
                if (specifier.Type == null)
                {
                    return null;
                }
                else
                {
                    return specifier.Type.GetRealTypeInternal();
                }
            }
        }

        public string Access
        {
            get
            {
                return specifier.Access.ToString();
            }
        }

        public bool IsVirtual
        {
            get
            {
                return specifier.IsVirtual;
            }
        }

        public int Offset
        {
            get
            {
                return specifier.Offset;
            }
        }
    }
}
