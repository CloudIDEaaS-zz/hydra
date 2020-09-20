using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AbstraX.DataAnnotations;
using AbstraX.ServerInterfaces;
using RestEntityProvider.Web.Entities;
using Utils;

namespace AbstraX.Handlers.ExpressionHandlers
{
    public enum ExpressionResultLocation
    {
        Any,
        Provider,
        Controller
    }

    public enum ExpressionType
    {
        Url,
        QueryString,
        UniqueId,
        UnknownJsonObject,
        Expression,
    }

    public enum ExpressionReturnType
    {
        Attributes,
        MethodVariables,
        QueryString,
        UniqueIdFactory,
        ReturnProperties,
        Expression
    }


    [ExpressionHandler(AbstraXProviderGuids.RestService)]
    public class RestExpressionHandler : IExpressionHandler
    {
        public string Handle(IBase baseObject, Enum type, string expression)
        {
            DebugUtils.Break();
            return expression;
        }

        public T Handle<T>(IBase baseObject, Enum type, Enum expressionType, Enum returnType, string expression, params object[] parms)
        {
            switch ((ExpressionReturnType) returnType)
            {
                case ExpressionReturnType.MethodVariables:
                    return (T) HandleMethodVariables(baseObject, (ExpressionType) expressionType, expression, parms);
                case ExpressionReturnType.QueryString:
                    return (T)HandleQueryString(baseObject, (ExpressionType)expressionType, expression, parms);
                case ExpressionReturnType.ReturnProperties:
                    return (T)HandleReturnProperties(baseObject, (ExpressionType)expressionType, expression, parms);
                case ExpressionReturnType.Expression:
                    return (T)HandleExpression(baseObject, (ExpressionType)expressionType, expression, parms);
            }

            throw new ArgumentException();
        }

        public string Handle(IBase baseObject, Enum type, object jsonObject)
        {
            DebugUtils.Break();
            return null;
        }

        public T Handle<T>(IBase baseObject, Enum type, Enum expressionType, Enum returnType, object jsonObject, params object[] parms)
        {
            switch ((ExpressionReturnType)returnType)
            {
                case ExpressionReturnType.UniqueIdFactory:
                    return (T)HandleUniqueId(baseObject, (ExpressionType)expressionType, jsonObject, parms);
                case ExpressionReturnType.Attributes:
                    return (T)HandleAttributes(baseObject, (ExpressionType)expressionType, jsonObject, parms);
            }

            throw new ArgumentException();
        }

        private object HandleAttributes(IBase baseObject, ExpressionType expressionType, object jsonObject, object[] parms)
        {
            var attributes = new List<Attribute>();

            switch (baseObject.Kind)
            {
                case DefinitionKind.ComplexProperty:
                case DefinitionKind.ComplexSetProperty:

                    if (jsonObject.HasDynamicMember("sortBy"))
                    {
                        var sortByPropertyName = jsonObject.GetDynamicMemberValue<string>("sortBy");
                        var sortByAttribute = typeof(SortByAttribute).CreateInstance<SortByAttribute>(sortByPropertyName, SortDirection.Ascending);

                        attributes.Add(sortByAttribute);
                    }

                    break;

                case DefinitionKind.SimpleProperty:

                    if (jsonObject.HasDynamicMember("isKey"))
                    {
                        var isKey = bool.Parse(jsonObject.GetDynamicMemberValue<string>("isKey"));

                        if (isKey)
                        {
                            var keyAttribute = typeof(KeyAttribute).CreateInstance<KeyAttribute>();
                            var databaseGeneratedAttribute = typeof(DatabaseGeneratedAttribute).CreateInstance<DatabaseGeneratedAttribute>(DatabaseGeneratedOption.None);

                            attributes.Add(keyAttribute);
                            attributes.Add(databaseGeneratedAttribute);
                        }
                    }

                    break;

                default:

                    break;
            }

            return attributes.ToArray();
        }

        private object HandleUniqueId(IBase baseObject, ExpressionType expressionType, object jsonObject, object[] parms)
        {
            Func<string[], string> factory = null;
            var x = 0;
            var expression = jsonObject.GetDynamicMemberValue<string>("keyPattern");
            var patterns = new List<string>()
            {
                @"\[\$\$(w+?)\]",       // global variable = $$
                @"\[\$\w+?\]",          // user variable = $
                @"\[\{/.+?\}\]",        // xpath expression = {/ }
                @"\[\{\{.+?\}\]",       // handle bars expression = {{  }
                @"\[\{\$\.\..+?\}\]",   // json path = {$.. }
            };

            foreach (var pattern in patterns)
            {
                var regex = new Regex(pattern);

                while (regex.IsMatch(expression))
                {
                    var match = regex.Match(expression);

                    if (parms.Length > x)
                    {
                        var replace = (string) parms[x];

                        expression = match.Replace(expression, replace);
                    }
                    else
                    {
                        expression = match.Replace(expression, $"{{{ x }}}");
                    }

                    x++;
                }
            }

            factory = (p) =>
            {
                var builder = new StringBuilder("string.Format(\"" + expression + "\"");

                foreach (var parm in p)
                {
                    builder.AppendWithLeadingIfLength(", ", parm);
                }

                builder.Append(");");
                return builder.ToString();
            };

            return factory;
        }

        private object HandleExpression(IBase baseObject, ExpressionType expressionType, string expression, object[] parms)
        {
            var x = 0;
            var patterns = new List<string>()
            {
                @"\[\$\$(w+?)\]",       // global variable = $$
                @"\[\$\w+?\]",          // user variable = $
                @"\[\{/.+?\}\]",        // xpath expression = {/ }
                @"\[\{\{.+?\}\]",       // handle bars expression = {{  }
                @"\[\{\$\.\..+?\}\]",   // json path = {$.. }
                @"\$\$(w+?)",       // global variable = $$
                @"\$\w+?",          // user variable = $
                @"\{/.+?\}",        // xpath expression = {/ }
                @"\{\{.+?\}",       // handle bars expression = {{  }
                @"\{\$\.\..+?\}",   // json path = {$.. }
            };

            foreach (var pattern in patterns)
            {
                var regex = new Regex(pattern);

                while (regex.IsMatch(expression))
                {
                    var match = regex.Match(expression);

                    if (parms.Length > x)
                    {
                        var replace = (string)parms[x];

                        expression = match.Replace(expression, replace);
                    }
                    else
                    {
                        expression = match.Replace(expression, $"{{{ x }}}");
                    }

                    x++;
                }
            }

            return expression;
        }

        private object HandleQueryString(IBase baseObject, ExpressionType expressionType, string expression, params object[] parms)
        {
            var restBaseObject = (RestEntityBase)baseObject;
            var rootObject = (object) restBaseObject.JsonRootObject;
            var regex = new Regex(@"\[\$(?<variable>\w*?)\]");

            if (regex.IsMatch(expression))
            {
                var match = regex.Match(expression);
                var variable = match.GetValue("variable");
                var queryStringVariables = rootObject.GetDynamicMemberValue<IList>(variable);
                var queryStringBuilder = new StringBuilder(match.Replace(expression, string.Empty));
                var x = 0;

                foreach (var queryStringVariable in queryStringVariables)
                {
                    var pair = queryStringVariable.GetDynamicMemberNameValueDictionary().Single();

                    if (x == 0)
                    {
                        queryStringBuilder.AppendFormat("{0}={1}", pair.Key, pair.Value);
                    }
                    else
                    {
                        queryStringBuilder.AppendFormat("&{0}={1}", pair.Key, pair.Value);
                    }

                    x++;
                }

                return queryStringBuilder.ToString();
            }

            throw new FormatException("Invalid regex expression for " + nameof(HandleQueryString));
        }

        private object HandleReturnProperties(IBase baseObject, ExpressionType expressionType, string expression, params object[] parms)
        {
            var returnProperties = new Dictionary<string, string>();
            var restBaseObject = (RestEntityBase)baseObject;
            var rootObject = restBaseObject.JsonRootObject;
            var regex = new Regex(@"\[\$(?<variable>\w*?)\]");

            while (regex.IsMatch(expression))
            {
                var match = regex.Match(expression);
                var variable = match.GetValue("variable");

                switch (variable)
                {
                    case "urlBase":
                        expression = match.Replace(expression, string.Empty);
                        break;
                    default:
                        expression = match.Replace(expression, string.Empty);
                        returnProperties.Add(variable, variable);
                        break;
                }

            }

            return returnProperties;
        }

        private object HandleMethodVariables(IBase baseObject, ExpressionType expressionType, string expression, params object[] parms)
        {
            var methodVariables = new Dictionary<string, string>();
            var restBaseObject = (RestEntityBase)baseObject;
            var rootObject = restBaseObject.JsonRootObject;
            var regex = new Regex(@"\[\$(?<variable>\w*?)\]");
            var isFirst = true;

            while (regex.IsMatch(expression))
            {
                var match = regex.Match(expression);
                var variable = match.GetValue("variable");

                switch (variable)
                {
                    case "urlBase":

                        if (expressionType == ExpressionType.Url)
                        {
                            if (isFirst)
                            {
                                expression = match.Replace(expression, "this.urlBase + \"");
                            }
                            else
                            {
                                expression = match.Replace(expression, "\" + this.urlBase + \"");
                            }
                        }

                        break;
                    default:

                        if (isFirst)
                        {
                            expression = match.Replace(expression, variable + " + \"");
                        }
                        else
                        {
                            expression = match.Replace(expression, "\" + " + variable + " + \"");
                        }

                        methodVariables.Add(variable, string.Format("variables[\"{0}\"]", variable));

                        break;
                }

                isFirst = false;
            }

            switch (expressionType)
            {
                case ExpressionType.Url:

                    var urlInitializer = "new Uri(" + expression;

                    foreach (var parm in parms)
                    {
                        urlInitializer += parm;
                    }

                    urlInitializer += "\");";
                    methodVariables.Add("uri", urlInitializer);

                    break;

                default:
                    DebugUtils.Break();
                    break;
            }

            return methodVariables;
        }
    }
}
