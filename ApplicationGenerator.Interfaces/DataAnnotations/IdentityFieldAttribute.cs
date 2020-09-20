using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class IdentityFieldAttribute : FormFieldAttribute
    {
        public IdentityFieldKind IdentityFieldKind { get; }

        public IdentityFieldAttribute(string uiHierarchyPath, IdentityFieldKind fieldKind) : base(uiHierarchyPath)
        {
            this.IdentityFieldKind = fieldKind;
        }
    }
}
