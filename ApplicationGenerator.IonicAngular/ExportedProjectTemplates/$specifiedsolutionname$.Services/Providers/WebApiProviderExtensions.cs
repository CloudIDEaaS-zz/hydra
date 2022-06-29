using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Utils;

namespace $safeprojectname$.Providers
{
    public interface IWebApiProvider
    {
        Dictionary<string, string> ConfigVariables { get; }
    }

    public static class WebApiProviderExtensions
    {
        public static Dictionary<string, string> GetVariables(this IWebApiProvider provider, string id)
        {
            var variables = new Dictionary<string, string>();
            var parts = id.Split("/");

            for (var x = 0; x < parts.Length; x++)
            {
                var part = parts[x];

                if (part.StartsWith("?"))
                {
                    var queryPart = part.RemoveStart("?");
                    var queryStringNameValues = HttpUtility.ParseQueryString(queryPart);

                    foreach (var name in queryStringNameValues.Keys.OfType<string>())
                    {
                        var value = queryStringNameValues.Get(name);

                        variables.Add(name, value);
                    }
                }
            }

            return provider.ConfigVariables.Concat(variables).ToDictionary();
        }
    }
}
