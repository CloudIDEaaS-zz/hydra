using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FormFieldAttribute : NavigableAttribute
    {
        public FormFieldKind FormFieldKind { get; }

        public FormFieldAttribute(string uiHierarchyPath, FormFieldKind fieldKind = FormFieldKind.DataType) : base(uiHierarchyPath)
        {
            this.FormFieldKind = fieldKind;
        }

        public FormFieldAttribute(string uiHierarchyPath, string foreignKeyField, string dropdownKeyField, string dropdownDisplayField) : base(uiHierarchyPath)
        {
            this.FormFieldKind = FormFieldKind.Dropdown;
        }
    }
}
