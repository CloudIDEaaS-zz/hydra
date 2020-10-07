// file:	DataAnnotations\FormFieldAttribute.cs
//
// summary:	Implements the form field attribute class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Attribute for form field. </summary>
    ///
    /// <remarks>   Ken, 10/4/2020. </remarks>

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FormFieldAttribute : NavigableAttribute
    {
        /// <summary>   Gets the form field kind. </summary>
        ///
        /// <value> The form field kind. </value>

        public FormFieldKind FormFieldKind { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="uiHierarchyPath">  Full pathname of the hierarchy file. </param>
        /// <param name="fieldKind">        (Optional) The field kind. </param>

        public FormFieldAttribute(string uiHierarchyPath, FormFieldKind fieldKind = FormFieldKind.DataType) : base(uiHierarchyPath)
        {
            this.FormFieldKind = fieldKind;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/4/2020. </remarks>
        ///
        /// <param name="uiHierarchyPath">      Full pathname of the hierarchy file. </param>
        /// <param name="foreignKeyField">      The foreign key field. </param>
        /// <param name="dropdownKeyField">     The dropdown key field. </param>
        /// <param name="dropdownDisplayField"> The dropdown display field. </param>

        public FormFieldAttribute(string uiHierarchyPath, string foreignKeyField, string dropdownKeyField, string dropdownDisplayField) : base(uiHierarchyPath)
        {
            this.FormFieldKind = FormFieldKind.Dropdown;
        }
    }
}
