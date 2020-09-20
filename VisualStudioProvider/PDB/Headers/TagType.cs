using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class TagType : Type
    {
        private unsafe CppSharp.Parser.AST.TagType tagType;

        public unsafe TagType(CppSharp.Parser.AST.TagType tagType) : base(tagType)
        {
            this.tagType = tagType;
            this.tagType.AssertNotNullAndOfType<CppSharp.Parser.AST.TagType>();
        }

        public Declaration Declaration
        {
            get
            {
                if (tagType.Declaration == null)
                {
                    return null;
                }
                else
                {
                    return tagType.Declaration.GetRealDeclarationInternal();
                }
            }
        }
    }
}
