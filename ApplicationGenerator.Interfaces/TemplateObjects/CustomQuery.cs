// file:	TemplateObjects\CustomQuery.cs
//
// summary:	Implements the custom query class

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.TemplateObjects
{
    /// <summary>   A custom query. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/29/2020. </remarks>

    public class CustomQuery
    {
        /// <summary>   Gets or sets the name of the controller method. </summary>
        ///
        /// <value> The name of the controller method. </value>

        public string ControllerMethodName { get; private set; }

        /// <summary>   Gets or sets the expression. </summary>
        ///
        /// <value> The expression. </value>

        public string Expression { get; private set; }

        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/29/2020. </remarks>
        ///
        /// <param name="controllerMethodName"> Name of the controller method. </param>
        /// <param name="expression">           The expression. </param>

        public CustomQuery(string controllerMethodName, string expression)
        {
            this.ControllerMethodName = controllerMethodName;
            this.Expression = expression;
        }
    }
}
