using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Utils
{
    public static class JsonExtensions
    {
        public static void WriteJson(this TextWriter writer, string text)
        {
            text = text.Replace("'", "\"").Replace("\r\n", "");

            writer.WriteLine(text);
        }

        public static string ToJsonText(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static void WriteJson(this TextWriter writer, dynamic obj)
        {
            var serializer = new JsonSerializer();
            var builder = new StringBuilder();

            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (var stringWriter = new StringWriter(builder))
            {
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    serializer.Serialize(jsonWriter, obj);
                }
            }

            writer.WriteLine(builder.ToString());
        }

        public static T ReadJson<T>(this TextReader reader)
        {
            var json = string.Empty;
            var text = reader.ReadToEnd();

            return ReadJson<T>(text);
        }

        public static T ReadJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static bool IsValidJson(string json)
        {
            json = json.Trim();

            try
            {
                if (json.StartsWith("{") && json.EndsWith("}"))
                {
                    JToken.Parse(json);
                }
                else if (json.StartsWith("[") && json.EndsWith("]"))
                {
                    JArray.Parse(json);
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static T ReadJson<T>(this TextReader reader, JsonSerializerSettings settings)
        {
            var json = string.Empty;
            var text = reader.ReadToEnd();

            return ReadJson<T>(text, settings);
        }

        public static T ReadJson<T>(string json, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static object ReadJson(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }
    }
}
