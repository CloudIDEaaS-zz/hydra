// file:	DataAnnotations\CustomQueryAttribute.cs
//
// summary:	Implements the custom query attribute class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.DataAnnotations
{
    /// <summary>   Attribute for custom query. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/29/2020. </remarks>

    public class CustomQueryAttribute : ResourcesAttribute
    {
        /// <summary>   Gets the name of the controller method. </summary>
        ///
        /// <value> The name of the controller method. </value>

        public string ControllerMethodName { get; }

        /// <summary>   Gets the query kind. </summary>
        ///
        /// <value> The query kind. </value>

        public QueryKind QueryKind { get; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/29/2020. </remarks>
        ///
        /// <param name="resourcesType">        Type of the resources. </param>
        /// <param name="controllerMethodName"> Name of the controller method. </param>
        /// <param name="queryKind">            (Optional) The query kind. </param>

        public CustomQueryAttribute(Type resourcesType, string controllerMethodName, QueryKind queryKind = QueryKind.None) : base(resourcesType)
        {
            this.ControllerMethodName = controllerMethodName;
            this.QueryKind = queryKind;
        }
    }
}
