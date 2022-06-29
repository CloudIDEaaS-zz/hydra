// file:	DataAnnotations\IdentityFieldAttribute.cs
//
// summary:	Implements the identity field attribute class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Attribute for identity field. </summary>
    ///
    /// <remarks>   Ken, 10/5/2020. </remarks>

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class IdentityFieldAttribute : FormFieldAttribute
    {
        /// <summary>   Gets the identity field kind. </summary>
        ///
        /// <value> The identity field kind. </value>

        public IdentityFieldKind IdentityFieldKind { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/5/2020. </remarks>
        ///
        /// <param name="uiHierarchyPath">      Full pathname of the hierarchy file. </param>
        /// <param name="identityFieldKind">    The identity field kind. </param>

        public IdentityFieldAttribute(string uiHierarchyPath, IdentityFieldKind identityFieldKind) : base(uiHierarchyPath)
        {
            this.IdentityFieldKind = identityFieldKind;
        }
    }
}
