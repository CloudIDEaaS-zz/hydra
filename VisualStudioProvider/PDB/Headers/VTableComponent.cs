using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisualStudioProvider.PDB.Headers
{
    public class VTableComponent
    {
        public VTableLayout OwningVTableLayout { get; set; }
        private unsafe CppSharp.Parser.AST.VTableComponent component;

        public unsafe VTableComponent(VTableLayout owningVTableLayout, CppSharp.Parser.AST.VTableComponent component)
        {
            this.OwningVTableLayout = owningVTableLayout;
            this.component = component;

            this.OwningVTableLayout.AssertNotNull();
            this.component.AssertNotNullAndOfType<CppSharp.Parser.AST.VTableComponent>();
        }

        public Declaration Declaration
        {
            get
            {
                if (component.Declaration == null)
                {
                    return null;
                }
                else
                {
                    return component.Declaration.GetRealDeclarationInternal();
                }
            }
        }

        public string Kind
        {
            get
            {
                return component.Kind.ToString();
            }
        }

        public IntegerValue Offset
        {
            get
            {
                return new IntegerValue(component.Offset);
            }
        }
    }
}
