using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class Class : DeclarationContext
    {
        private unsafe CppSharp.Parser.AST.Class _class;

        public unsafe Class(CppSharp.Parser.AST.Class _class) : base(_class)
        {
            this._class = _class;
            this._class.AssertNotNull();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return _class.Location.ID;
            }
        }

        public string ClassName
        {
            get
            {
                return _class.Name;
            }
        }

        public IEnumerable<BaseClassSpecifier> BaseClassSpecifiers
        {
            get
            {
                foreach (var specifier in _class.GetBaseClassSpecifiers())
                {
                    yield return new BaseClassSpecifier(this, specifier);
                }
            }
        }

        public IEnumerable<Field> Fields
        {
            get
            {
                var x = 0;

                foreach (var field in _class.GetFields())
                {
                    yield return new Field(this, field, x++);
                }
            }
        }

        public IEnumerable<Method> Methods
        {
            get
            {
                foreach (var method in _class.GetMethods())
                {
                    yield return new Method(this, method);
                }
            }
        }

        public IEnumerable<AccessSpecifierDecl> Specifiers
        {
            get
            {
                foreach (var specifier in _class.GetSpecifiers())
                {
                    yield return new AccessSpecifierDecl(this, specifier);
                }
            }
        }

        public bool HasNonTrivialCopyConstructor
        {
            get
            {
                return _class.HasNonTrivialCopyConstructor;
            }
        }

        public bool HasNonTrivialDefaultConstructor
        {
            get
            {
                return _class.HasNonTrivialDefaultConstructor;
            }
        }

        public bool HasNonTrivialDestructor
        {
            get
            {
                return _class.HasNonTrivialDestructor;
            }
        }

        public bool IsAbstract
        {
            get
            {
                return _class.IsAbstract;
            }
        }

        public bool IsDynamic
        {
            get
            {
                return _class.IsDynamic;
            }
        }

        public bool IsExternCContext
        {
            get
            {
                return _class.IsExternCContext;
            }
        }

        public bool IsPOD
        {
            get
            {
                return _class.IsPOD;
            }
        }

        public bool IsPolymorphic
        {
            get
            {
                return _class.IsPolymorphic;
            }
        }

        public bool IsUnion
        {
            get
            {
                return _class.IsUnion;
            }
        }

        public ClassLayout Layout
        {
            get
            {
                if (_class.Layout == null)
                {
                    return null;
                }
                else
                {
                    return new ClassLayout(_class.Layout);
                }
            }
        }
    }
}
