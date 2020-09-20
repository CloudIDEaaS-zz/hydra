using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class Statement
    {
        private unsafe CppSharp.Parser.AST.Statement statement;

        public unsafe Statement(CppSharp.Parser.AST.Statement statement)
        {
            this.statement = statement;
            this.statement.AssertNotNull();
        }

        public string Class
        {
            get
            {
                if (statement.Class == null)
                {
                    return null;
                }
                else
                {
                    return statement.Class.ToString();
                }
            }
        }

        public Declaration Decl
        {
            get
            {
                if (statement.Decl == null)
                {
                    return null;
                }
                else
                {
                    return statement.Decl.GetRealDeclarationInternal();
                }
            }
        }

        public string String
        {
            get
            {
                return statement.String;
            }
        }
    }
}
