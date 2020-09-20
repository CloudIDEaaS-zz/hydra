using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Utils
{
    public static class JsonExtensions
    {
        public static void WriteJson(this TextWriter writer, string text)
        {
            text = text.Replace("'", "\"").Replace(Environment.NewLine, "");

            writer.WriteLine(text);
            writer.WriteLine(Environment.NewLine);

            writer.Flush();
        }

        public static void WriteJson(this Stream stream, string text)
        {
            var writer = new StreamWriter(stream);
            text = text.Replace("'", "\"").Replace(Environment.NewLine, "");

            writer.WriteLine(text);
            writer.WriteLine(Environment.NewLine);

            writer.Flush();
        }

        public static string ToJson(this object obj, bool prettyPrint = true)
        {
            var serializer = new JavaScriptSerializer();
            var builder = new StringBuilder();

            using (var stringWriter = new StringWriter(builder))
            {
                serializer.Serialize(obj, builder);
            }

            if (prettyPrint)
            {
                var formatter = new JsonFormatter(builder.ToString());

                return formatter.Format();
            }

            return builder.ToString();
        }

        public static string ToJsonDynamic(this object obj, bool prettyPrint = true)
        {
            var jsonObject = (DynamicJsonConverter.DynamicJsonObject)obj;

            return jsonObject.ToString();
        }

        public static void WriteJsonCommand(this TextWriter writer, CommandPacket commandPacket)
        {
            writer.WriteJsonCommand(commandPacket, Environment.NewLine);
        }

        public static void WriteJsonCommand(this TextWriter writer, CommandPacket commandPacket, string lineTerminator)
        {
            var serializer = new JavaScriptSerializer();
            var builder = new StringBuilder();

            using (var stringWriter = new StringWriter(builder))
            {
                serializer.Serialize(commandPacket, builder);
            }

            writer.WriteLine(builder.ToString());

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

            return ReadJson(text);
        }

        public static void WriteJson(this TextWriter writer, dynamic obj)
        {
            var serializer = new JavaScriptSerializer();
            var builder = new StringBuilder();

            using (var stringWriter = new StringWriter(builder))
            {
                serializer.Serialize(obj, builder);
            }

            writer.WriteLine(builder.ToString());

            writer.Flush();
        }

        public static CommandPacket ReadJson(this TextReader reader)
        {
            var json = string.Empty;
            var text = reader.ReadToEnd();

            return ReadJson(text);
        }

        public static T ReadJson<T>(this TextReader reader)
        {
            var json = string.Empty;
            var text = reader.ReadToEnd();

            return ReadJson<T>(text);
        }

        public static CommandPacket ReadJson(string json)
        {
            var serializer = new JavaScriptSerializer();

            serializer.RegisterConverters(new List<JavaScriptConverter> { new KeyValuePairConverter() });

            return serializer.Deserialize<CommandPacket>(json);
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

        public static T ReadJson<T>(string json)
        {
            var serializer = new JavaScriptSerializer();

            if (typeof(T).Name == "Object")
            {
                serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
            }
            else
            {
                serializer.RegisterConverters(new List<JavaScriptConverter> { new KeyValuePairConverter() });
            }

            return serializer.Deserialize<T>(json);
        }
    }
}
