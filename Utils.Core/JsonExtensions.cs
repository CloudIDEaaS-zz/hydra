using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Net.Http.Headers;

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
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };

            settings.Converters.Add(new KeyValuePairConverter());

            return JsonConvert.SerializeObject(obj, settings);
        }

        public static void WriteJson(this TextWriter writer, dynamic obj)
        {
            var serializer = new JsonSerializer();
            var builder = new StringBuilder();

            serializer.Converters.Add(new KeyValuePairConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.DateFormatHandling = DateFormatHandling.IsoDateFormat;

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
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };

            settings.Converters.Add(new KeyValuePairConverter());

            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static void WriteJsonCommand(this TextWriter writer, CommandPacket commandPacket)
        {
            writer.WriteJsonCommand(commandPacket, Environment.NewLine);
        }

        public static void WriteJsonCommand(this TextWriter writer, CommandPacket commandPacket, string lineTerminator)
        {
            var json = commandPacket.ToJsonText();

            writer.WriteJson(json);

            if (!lineTerminator.IsNullOrEmpty())
            {
                writer.WriteLine(lineTerminator);
                writer.Flush();
            }
        }

        public static CommandPacket ReadJsonCommand(this TextReader reader)
        {
            var json = string.Empty;
            var text = reader.ReadUntil(Environment.NewLine.Repeat(2), true);

            return ReadJson<CommandPacket>(text);
        }

        public static CommandPacket<T> ReadJsonCommand<T>(this TextReader reader)
        {
            var json = string.Empty;
            var text = reader.ReadUntil(Environment.NewLine.Repeat(2), true);

            return ReadJson<CommandPacket<T>>(text);
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

        public static object JsonSelect(this object obj, string tokenPath)
        {
            var jObject = JObject.FromObject(obj);
            var token = jObject.SelectToken(tokenPath);
            string json;

            if (token != null)
            {
                json = token.ToString();

                return ReadJson<object>(json);
            }
            else
            {
                return null;
            }
        }
    }
}
