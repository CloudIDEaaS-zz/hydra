using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class DependentNameType : Type
    {
        private unsafe CppSharp.Parser.AST.DependentNameType dependentNameType;

        public unsafe DependentNameType(CppSharp.Parser.AST.DependentNameType dependentNameType) : base(dependentNameType)
        {
            this.dependentNameType = dependentNameType;
            this.dependentNameType.AssertNotNullAndOfType<CppSharp.Parser.AST.DependentNameType>();
        }

        public QualifiedType Desugared
        {
            get
            {
                if (dependentNameType.Desugared == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(dependentNameType.Desugared);
                }
            }
        }
    }
}
