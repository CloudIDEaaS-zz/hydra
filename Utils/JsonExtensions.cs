using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Serialization;
using System.Windows.Input;

namespace Utils
{
    public static class JsonExtensions
    {
        public static void ChangeTo(this JToken jToken, string newValue)
        {
            var owner = jToken.Parent.Parent;
            var property = (JProperty)jToken.Parent;

            property.Remove();
            owner.Add(new JProperty(property.Name, newValue));
        }

        public static void WriteJson(this TextWriter writer, string text)
        {
            //text = text.Replace("'", "\"").Replace("\r\n", "");

            writer.WriteLine(text);
        }

        public static string ToJsonText(this object obj, Formatting formatting = Formatting.None, NamingStrategy namingStrategy = null)
        {
            var serializer = new JsonSerializer();
            var builder = new StringBuilder();

            serializer.Converters.Add(new KeyValuePairConverter());
            serializer.Converters.Add(new StringEnumConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            serializer.Formatting = formatting;

            if (namingStrategy != null)
            {
                serializer.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = namingStrategy
                };
            }

            using (var stringWriter = new StringWriter(builder))
            {
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    serializer.Serialize(jsonWriter, obj);
                }
            }

            return builder.ToString();
        }

        public static void WriteJson(this TextWriter writer, dynamic obj, Formatting formatting = Formatting.None, NamingStrategy namingStrategy = null)
        {
            var serializer = new JsonSerializer();
            var builder = new StringBuilder();

            serializer.Converters.Add(new StringEnumConverter());
            serializer.Converters.Add(new KeyValuePairConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.DateFormatHandling = DateFormatHandling.IsoDateFormat;

            if (formatting != Formatting.None)
            {
                serializer.Formatting = formatting;
            }

            if (namingStrategy != null)
            {
                serializer.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = namingStrategy
                };
            }

            using (var stringWriter = new StringWriter(builder))
            {
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    serializer.Serialize(jsonWriter, obj);
                }
            }

            writer.WriteLine(builder.ToString());
        }

        public static T Convert<T>(object obj)
        {
            var json = obj.ToJsonText();

            return ReadJson<T>(json);
        }

        public static T ReadJson<T>(this TextReader reader, NamingStrategy namingStrategy = null)
        {
            var json = string.Empty;
            var text = reader.ReadToEnd();

            return ReadJson<T>(text, namingStrategy);
        }

        public static T ReadJson<T>(string json, NamingStrategy namingStrategy = null)
        {
            var settings = new JsonSerializerSettings();

            settings.Converters.Add(new KeyValuePairConverter());
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;

            if (namingStrategy != null)
            {
                settings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = namingStrategy
                };
            }

            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static string ToJsonDynamic(this object obj, bool prettyPrint = true)
        {
            var jsonObject = (DynamicJsonConverter.DynamicJsonObject)obj;

            return jsonObject.ToString();
        }

        public static void WriteJsonCommand(this TextWriter writer, CommandPacket commandPacket, Action<string> textWriteCallback = null)
        {
            if (commandPacket.Arguments == null)
            {
                commandPacket.Arguments = new KeyValuePair<string, object>[0];
            }

            writer.WriteJsonCommand(commandPacket, Environment.NewLine, textWriteCallback);
        }

        public static void WriteJsonCommand(this TextWriter writer, CommandPacket commandPacket, string lineTerminator, Action<string> textWriteCallback = null)
        {
            var json = commandPacket.ToJsonText();

            if (textWriteCallback != null)
            {
                textWriteCallback(json);
            }

            writer.WriteJson(json);

            if (!lineTerminator.IsNullOrEmpty())
            {
                writer.WriteLine(lineTerminator + lineTerminator);
            }

            writer.Flush();
        }

        public static CommandPacket ReadJsonCommand(this TextReader reader, Action<string> textReadCallback = null)
        {
            var jsonText = reader.ReadUntil(Environment.NewLine.Repeat(2), true);

            if (textReadCallback != null)
            {
                textReadCallback(jsonText);
            }

            return ReadJson<CommandPacket>(jsonText);
        }

        public static CommandPacket<T> ReadJsonCommand<T>(this TextReader reader, Action<string> textReadCallback = null)
        {
            var jsonText = reader.ReadUntil(Environment.NewLine.Repeat(2), true);

            if (textReadCallback != null)
            {
                textReadCallback(jsonText);
            }

            return ReadJson<CommandPacket<T>>(jsonText);
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
            catch (Exception ex)
            {
                return false;
            }
        }

        public static Exception GetJsonExceptions(string json)
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
                    return new FormatException("json does not start with a { or [");
                }

                return null;
            }
            catch (Exception ex)
            {
                return ex;
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

                if (IsValidJson(json))
                {
                    return ReadJson<object>(json);
                }
                else
                {
                    return token;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
