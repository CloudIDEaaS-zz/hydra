using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class DecayedType : Type
    {
        private unsafe CppSharp.Parser.AST.DecayedType decayedType;

        public unsafe DecayedType(CppSharp.Parser.AST.DecayedType decayedType) : base(decayedType)
        {
            this.decayedType = decayedType;
            this.decayedType.AssertNotNullAndOfType<CppSharp.Parser.AST.DecayedType>();
        }

        public QualifiedType Decayed
        {
            get
            {
                if (decayedType.Decayed == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(decayedType.Decayed);
                }
            }
        }

        public QualifiedType Original
        {
            get
            {
                if (decayedType.Original == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(decayedType.Original);
                }
            }
        }

        public QualifiedType Pointee
        {
            get
            {
                if (decayedType.Pointee == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(decayedType.Pointee);
                }
            }
        }
    }
}
