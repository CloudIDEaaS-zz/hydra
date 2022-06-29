// file:	DataAnnotations\AuthorizeAttribute.cs
//
// summary:	Implements the authorize attribute class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Attribute for authorize. </summary>
    ///
    /// <remarks>   Ken, 10/10/2020. </remarks>

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeAttribute : Attribute
    {
        /// <summary>   Gets or sets the roles. </summary>
        ///
        /// <value> The roles. </value>

        public string Roles { get; set; }

        /// <summary>   Gets the state. </summary>
        ///
        /// <value> The state. </value>

        public AuthorizationState State { get; }

        /// <summary>   Gets the kind. </summary>
        ///
        /// <value> The user interface kind. </value>

        public UIKind UIKind { get; }

        /// <summary>   Gets the load kind. </summary>
        ///
        /// <value> The load kind. </value>

        public UILoadKind LoadKind { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="uiKind">   The kind. </param>
        /// <param name="uiLoadKind"> (Optional) The load kind. </param>

        public AuthorizeAttribute(UIKind uiKind, UILoadKind uiLoadKind = UILoadKind.Default)
        {
            this.UIKind = uiKind;
            this.LoadKind = uiLoadKind;
        }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   Ken, 10/10/2020. </remarks>
        ///
        /// <param name="authorizationState">   State of the authorization. </param>
        /// <param name="uiKind">               The kind. </param>
        /// <param name="uiLoadKind">             (Optional) The load kind. </param>

        public AuthorizeAttribute(AuthorizationState authorizationState, UIKind uiKind, UILoadKind uiLoadKind = UILoadKind.Default)
        {
            this.State = authorizationState;
        }
    }
}
