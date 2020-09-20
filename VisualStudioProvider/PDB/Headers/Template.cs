using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CppParser;
using System.Numerics;

namespace VisualStudioProvider.PDB.Headers
{
    public class Template : Declaration
    {
        private unsafe CppSharp.Parser.AST.Template template;

        public unsafe Template(Declaration owningDeclaration, CppSharp.Parser.AST.Template template) : base(owningDeclaration, template)
        {
            this.template = template;
        }

        public unsafe Template(CppSharp.Parser.AST.Template template) : base(template)
        {
            this.template = template;
            this.template.AssertNotNull();
        }

        public BigInteger LocationIdentifier
        {
            get
            {
                return template.Location.ID;
            }
        }

        public Declaration TemplateDeclaration
        {
            get
            {
                if (template.TemplatedDecl == null)
                {
                    return null;
                }
                else
                {
                    return template.TemplatedDecl.GetRealDeclarationInternal();
                }
            }
        }

        public IEnumerable<Declaration> Parameters
        {
            get
            {
                foreach (var parameter in template.GetParameters())
                {
                    yield return new Declaration(this, parameter);
                }
            }
        }
    }
}
