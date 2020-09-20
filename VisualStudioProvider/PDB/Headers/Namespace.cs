using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class Namespace : DeclarationContext
    {
        private CppSharp.Parser.AST.Namespace _namespace;

        public unsafe Namespace(Declaration owningDeclaration, CppSharp.Parser.AST.Namespace _namespace) : base(owningDeclaration, _namespace)
        {
            this._namespace = _namespace;

            this._namespace.AssertNotNullAndOfType<CppSharp.Parser.AST.Namespace>();
        }

        public unsafe Namespace(CppSharp.Parser.AST.Namespace _namespace) : base(_namespace)
        {
            this._namespace = _namespace;
            this._namespace.AssertNotNull();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return _namespace.Location.ID;
            }
        }

        public override string Name
        {
            get
            {
                return _namespace.Name;
            }
        }

        public bool IsInline
        {
            get
            {
                return _namespace.IsInline;
            }
        }
    }
}
