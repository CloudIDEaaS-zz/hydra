using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class Variable : Declaration
    {
        private unsafe CppSharp.Parser.AST.Variable variable;
        public unsafe DeclarationContext OwningDeclarationContext { get; set; }

        public unsafe Variable(DeclarationContext owningDeclarationContext, CppSharp.Parser.AST.Variable variable) : base(variable)
        {
            this.OwningDeclarationContext = owningDeclarationContext;
            this.variable = variable;

            this.OwningDeclarationContext.AssertNotNull();
            this.variable.AssertNotNullAndOfType<CppSharp.Parser.AST.Variable>(); 
        }

        public unsafe Variable(CppSharp.Parser.AST.Variable variable) : base(variable)
        {
            this.variable = variable;

            this.variable.AssertNotNull();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return variable.Location.ID;
            }
        }

        public string Mangled
        {
            get
            {
                return variable.Mangled;
            }
        }

        public QualifiedType QualifiedType
        {
            get
            {
                if (variable.QualifiedType == null)
                {
                    return null;
                }
                else
                {
                    return new QualifiedType(variable.QualifiedType);
                }
            }
        }
    }
}
