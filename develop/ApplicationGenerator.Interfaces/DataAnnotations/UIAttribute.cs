// file:	DataAnnotations\UIAttribute.cs
//
// summary:	Implements the attribute class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AbstraX.DataAnnotations
{
    /// <summary>   An attribute. </summary>
    ///
    /// <remarks>   Ken, 10/5/2020. </remarks>

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple =true)]
    public class UIAttribute : Attribute, IFacetHandlerKindAttribute
    {
        /// <summary>   Gets or sets the full pathname of the hierarchy file. </summary>
        ///
        /// <value> The full pathname of the hierarchy file. </value>

        public string UIHierarchyPath { get; set; }

        /// <summary>   Gets or sets the path root alias. </summary>
        ///
        /// <value> The path root alias. </value>

        public string PathRootAlias { get; set; }

        /// <summary>   Gets or sets the kind. </summary>
        ///
        /// <value> The user interface kind. </value>

        public UIKind UIKind { get; set; }

        /// <summary>   Gets or sets the load kind. </summary>
        ///
        /// <value> The user interface load kind. </value>

        public UILoadKind UILoadKind { get; set; }

        /// <summary>   Gets or sets a value indicating whether this  use router. </summary>
        ///
        /// <value> True if use router, false if not. </value>

        public bool UseRouter { get; set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/5/2020. </remarks>
        ///
        /// <param name="uiHierarchyPath">  Full pathname of the hierarchy file. </param>
        /// <param name="uiKind">             (Optional) The kind. </param>
        /// <param name="useRouter">        (Optional) True to use router. </param>

        public UIAttribute(string uiHierarchyPath, UIKind uiKind = UIKind.None, bool useRouter = false)
        {
            this.UIHierarchyPath = uiHierarchyPath;
            this.UIKind = uiKind;
            this.UseRouter = useRouter;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/5/2020. </remarks>
        ///
        /// <param name="uiHierarchyPath">  Full pathname of the hierarchy file. </param>
        /// <param name="pathRootAlias">    The path root alias. </param>
        /// <param name="uiKind">             (Optional) The kind. </param>
        /// <param name="useRouter">        (Optional) True to use router. </param>

        public UIAttribute(string uiHierarchyPath, string pathRootAlias, UIKind uiKind = UIKind.None, bool useRouter = false)
        {
            this.UIHierarchyPath = uiHierarchyPath;
            this.UIKind = uiKind;
            this.PathRootAlias = pathRootAlias;
            this.UseRouter = useRouter;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/5/2020. </remarks>
        ///
        /// <param name="uiKind">         The kind. </param>
        /// <param name="uiLoadKind">     The load kind. </param>
        /// <param name="useRouter">    (Optional) True to use router. </param>

        public UIAttribute(UIKind uiKind, UILoadKind uiLoadKind, bool useRouter = false)
        {
            this.UIKind = uiKind;
            this.UILoadKind = uiLoadKind;
            this.UseRouter = useRouter;
        }

        /// <summary>   Gets the kind. </summary>
        ///
        /// <value> The kind. </value>

        public Guid Kind
        {
            get
            {
                var field = Utils.EnumUtils.GetField(this.UIKind);
                var kindGuidAttribute = field.GetCustomAttribute<KindGuidAttribute>();

                return kindGuidAttribute.Guid;
            }
        }
    }
}
