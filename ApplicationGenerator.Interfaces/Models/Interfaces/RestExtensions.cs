using Microsoft.CSharp.RuntimeBinder;
using RestEntityProvider.Web.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace AbstraX.Models.Interfaces
{
    public static class RestExtensions
    {
        public static bool IsRef(this object obj)
        {
            return obj.HasDynamicMember("$ref");
        }

        public static object ResolveDefinition(this object jsonRootObject, object obj, out string name)
        {
            var definitionRef = (string)obj.GetDynamicMemberValue<object>("$ref");
            var pathParts = definitionRef.Split("/");
            var builder = new StringBuilder();
            var rootObject = (object)jsonRootObject;
            object refObject;
            string path;

            name = pathParts.Last();

            foreach (var part in pathParts)
            {
                if (part != "#")
                {
                    builder.AppendWithLeadingIfLength("..", part);
                }
            }

            path = builder.ToString();
            refObject = rootObject.JsonSelect(path);

            return refObject;
        }

        public static object DoReplacements(this object jsonObject, Dictionary<string, object> variables)
        {
            if (jsonObject is List<object>)
            {
                var list = (List<object>)jsonObject;

                for (var x = 0; x < list.Count; x++)
                {
                    var original = list[x];
                    object replaced;
                    
                    replaced = original.DoReplacements(variables);

                    list[x] = replaced;
                }
            }
            else
            {
                if (jsonObject is string)
                {
                    string value = jsonObject.DoVariableReplacements(variables);

                    return value;
                }
                else if (jsonObject is IList)
                {
                    var list = (IList)jsonObject;
                    var newList = list.Cast<object>().ToList();
                    var x = 0;

                    foreach (var item in newList)
                    {
                        object replaced;

                        replaced = item.DoReplacements(variables);

                        list[x] = replaced;
                        x++;
                    }
                }
                else
                {
                    foreach (var pair in jsonObject.GetDynamicMemberNameValueDictionary())
                    {
                        var member = pair.Key;
                        var value = pair.Value as string;

                        if (value != null)
                        {
                            value = jsonObject.DoVariableReplacements(variables, member, value);

                            if (value is string)
                            {
                                if (!variables.ContainsKey(member))
                                {
                                    variables.Add(member, value);
                                }
                            }
                        }
                        else
                        {
                            var obj = (object) pair.Value;

                            obj.DoReplacements(variables);
                        }
                    }
                }
            }

            return jsonObject;
        }

        private static string DoVariableReplacements(this object jsonObject, Dictionary<string, object> variables, string member = null, string value = null)
        {
            foreach (var variable in variables)
            {
                var pattern = @"\[\$" + variable.Key + @"\]";
                var regex = new Regex(pattern);

                if (jsonObject is string)
                {
                    value = (string)jsonObject;
                }

                if (regex.IsMatch(value.ToString()))
                {
                    value = value.RegexReplace(pattern, variable.Value.ToString());

                    if (member != null)
                    {
                        jsonObject.SetDynamicMember(member, value);
                    }
                }
            }

            return value;
        }

        public static object DoReplacements(this RestEntityBase entitiesBase, Dictionary<string, object> variables)
        {
            var jsonObject = (object) entitiesBase.JsonObject;

            jsonObject.DoReplacements(variables);

            return jsonObject;
        }
    }
}
