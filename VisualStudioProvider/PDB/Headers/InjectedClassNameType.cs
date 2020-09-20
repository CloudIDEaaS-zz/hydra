using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class InjectedClassNameType : Type
    {
        private unsafe CppSharp.Parser.AST.InjectedClassNameType injectedClassNameType;

        public unsafe InjectedClassNameType(CppSharp.Parser.AST.InjectedClassNameType injectedClassNameType) : base(injectedClassNameType)
        {
            this.injectedClassNameType = injectedClassNameType;
            this.injectedClassNameType.AssertNotNullAndOfType<CppSharp.Parser.AST.InjectedClassNameType>();
        }

        public Class Class
        {
            get
            {
                if (injectedClassNameType.Class == null)
                {
                    return null;
                }
                else
                {
                    return injectedClassNameType.Class.GetRealClassInternal();
                }
            }
        }

        public QualifiedType InjectedSpecializationType
        {
            get
            {
                if (injectedClassNameType.InjectedSpecializationType == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(injectedClassNameType.InjectedSpecializationType);
                }
            }
        }
    }
}
