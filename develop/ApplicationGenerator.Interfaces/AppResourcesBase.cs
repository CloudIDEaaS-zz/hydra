// file:	AppResourcesBase.cs
//
// summary:	Implements the application resources base class

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using AbstraX.DataAnnotations;
using Utils;

namespace AbstraX
{
    /// <summary>   An application resources base. </summary>
    ///
    /// <remarks>   CloudIDEaaS, 12/20/2020. </remarks>
    ///
    /// <typeparam name="TEntityContainer"> Type of the entity container. </typeparam>

    public abstract class AppResourcesBase<TEntityContainer> : IAppResources
    {
        /// <summary>   The identity. </summary>
        protected IIdentity identity;
        /// <summary>   The container. </summary>
        protected TEntityContainer container;

        /// <summary>   Gets the queries. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/20/2020. </remarks>
        ///
        /// <returns>   The queries. </returns>

        public abstract List<QueryInfo> GetQueries();

        /// <summary>   Gets the resources. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/20/2020. </remarks>
        ///
        /// <param name="componentKind">    The component kind. </param>
        ///
        /// <returns>   The resources. </returns>

        public abstract dynamic GetResources(UIKind componentKind);

        /// <summary>   Registers the query. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/20/2020. </remarks>
        ///
        /// <typeparam name="TEntity">  Type of the entity. </typeparam>
        /// <param name="queryName">    Name of the query. </param>
        /// <param name="expression">   The expression. </param>
        ///
        /// <returns>   A QueryInfo. </returns>

        public QueryInfo RegisterQuery<TEntity>(string queryName, Expression<Func<TEntity, object>> expression)
        {
            return null;
        }

        /// <summary>   Registers the query. </summary>
        ///
        /// <remarks>   CloudIDEaaS, 12/20/2020. </remarks>
        ///
        /// <param name="controllerMethodName"> Name of the controller method. </param>
        /// <param name="expression">           The expression. </param>
        ///
        /// <returns>   A QueryInfo. </returns>

        public QueryInfo RegisterQuery(string controllerMethodName, Expression<Func<object>> expression)
        {
            var regex = new Regex(@"(?<replaceExpression>value\(.*?\)\.(?<variable>[^\.]+?))(\.(?<trailingExpression>[\w]+)|$)");
            var expressionCode = expression.ToString();
            var matches = regex.Matches(expressionCode);

            foreach (var match in matches.Cast<Match>())
            {
                var variable = match.GetGroupValue("variable");
                var replaceExpression = match.GetGroupValue("replaceExpression");
                var trailingExpression = match.GetGroupValue("trailingExpression");

                switch (variable)
                {
                    case "container":

                        expressionCode = expressionCode.Replace(replaceExpression, "{ containerVariable }");
                        break;
                    case "identity":

                        switch (trailingExpression)
                        {
                            case "Name":
                                expressionCode = expressionCode.Replace(replaceExpression + "." + trailingExpression, "{ identityName }");
                                break;
                        }

                        break;
                }
            }

            expressionCode = expressionCode.RemoveStart("() => ");

            return new QueryInfo(controllerMethodName, expressionCode);
        }
    }
}