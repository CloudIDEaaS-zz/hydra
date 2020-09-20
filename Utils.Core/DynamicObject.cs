using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public static class DynamicObjects
    {
        public static Dictionary<string, object> GetDynamicMemberNameValueDictionary(this object obj)
        {
            var dictionary = new Dictionary<string, object>();
            var jObject = (JObject)obj;

            foreach (var pair in jObject)
            {
                dictionary.Add(pair.Key, pair.Value);
            }

            return dictionary;
        }

        public static T GetDynamicMemberValue<T>(this object obj, string name)
        {
            return (T)DebugUtils.BreakReturnNull();
        }

        public static bool HasDynamicMember(this object obj, string name)
        {
            return DebugUtils.BreakReturn<bool>(false);
        }

        public static void SetDynamicMember(this object obj, string name, object value)
        {
            DebugUtils.Break();
        }
    }
}
