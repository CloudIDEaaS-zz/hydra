using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class Method : Function
    {
        public Class OwningClass { get; set; }
        private unsafe CppSharp.Parser.AST.Method method;

        public unsafe Method(Class owningClass, CppSharp.Parser.AST.Method method) : base(owningClass, method)
        {
            this.method = method;
            this.OwningClass = owningClass;

            this.OwningClass.AssertNotNull();
            this.method.AssertNotNullAndOfType<CppSharp.Parser.AST.Method>();
        }

        public unsafe Method(CppSharp.Parser.AST.Method method) : base(method)
        {
            this.method = method;
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return method.Location.ID;
            }
        }

        public QualifiedType ConversionType
        {
            get
            {
                if (method.ConversionType == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(method.ConversionType);
                }
            }
        }

        public bool IsConst
        {
            get
            {
                return method.IsConst;
            }
        }

        public bool IsCopyConstructor
        {
            get
            {
                return method.IsCopyConstructor;
            }
        }

        public bool IsDefaultConstructor
        {
            get
            {
                return method.IsDefaultConstructor;
            }
        }

        public bool IsExplicit
        {
            get
            {
                return method.IsExplicit;
            }
        }

        public bool IsImplicit
        {
            get
            {
                return method.IsImplicit;
            }
        }

        public bool IsMoveConstructor
        {
            get
            {
                return method.IsMoveConstructor;
            }
        }

        public bool IsOverride
        {
            get
            {
                return method.IsOverride;
            }
        }

        public bool IsPure
        {
            get
            {
                return method.IsPure;
            }
        }
    }
}
