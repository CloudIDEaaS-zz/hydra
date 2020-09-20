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
    public abstract class AppResourcesBase<TEntityContainer> : IAppResources
    {
        protected IIdentity identity;
        protected TEntityContainer container;
        public abstract List<QueryInfo> GetQueries();
        public abstract dynamic GetResources(UIKind componentKind);

        public QueryInfo RegisterQuery<TEntity>(string queryName, Expression<Func<TEntity, object>> expression)
        {
            return null;
        }

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