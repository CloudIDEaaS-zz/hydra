using HandlebarsDotNet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Utils;

namespace ApplicationGenerator.Services
{
    public delegate void HandlebarsParserBlockHelper(TextWriter output, HelperOptions options, dynamic context, params object[] arguments);
    public delegate void HandlebarsParserHelper(TextWriter output, dynamic context, params object[] arguments);

    public interface IHandlebarsParser
    {
    }

    public class HandlebarsParser : IHandlebarsParser
    {
        public HandlebarsConfiguration Configuration { get; }
        private IHandlebars internalHandlebars;
        private Dictionary<string, object> globals;

        public HandlebarsParser()
        {
            this.internalHandlebars = Handlebars.Create();
            this.globals = new Dictionary<string, object>();

            this.RegisterHelper("variable", (writer, context, arguments) =>
            {
                var value = ((JValue)arguments[0]).ToObject<string>();

                writer.WriteSafeString(value.ToCamelCase().RemoveText(" "));
            });

            this.internalHandlebars.RegisterHelper("set", (writer, context, arguments) =>
            {
                var key = (string)arguments[0];
                var value = (object)arguments[1];

                if (globals.ContainsKey(key))
                {
                    globals.Remove(key);
                }

                globals.Add(key, value);
            });

            this.RegisterHelper("get", (writer, context, arguments) =>
            {
                var key = (string)arguments[0];

                writer.WriteSafeString(globals[key]);
            });

            this.RegisterHelper("clear", (writer, context, arguments) =>
            {
                globals.Clear();
            });

            this.RegisterHelper("ifEquals", (writer, options, context, arguments) =>
            {
                var arg1 = (string)arguments[0];
                var arg2 = (string)arguments[1];

                if (arg1 == arg2)
                {
                    options.Template(writer, null);
                }
                else
                {
                    options.Inverse(writer, null);
                }
            });

            this.RegisterHelper("lookup", (writer, options, context, arguments) =>
            {
                var lookupSource = arguments[0];

                if (lookupSource is string[])
                {
                    var lookupArray = (string[])arguments[0];
                    var lookupValue = (string)arguments[1];

                    if (lookupArray.Contains(lookupValue))
                    {
                        options.Template(writer, null);
                    }
                    else
                    {
                        options.Inverse(writer, null);
                    }
                }
                else if (lookupSource is Dictionary<string, string>)
                {
                    var lookupSet = (Dictionary<string, string>)arguments[0];
                    var lookupValue = (string)arguments[1];

                    if (lookupSet.ContainsKey(lookupValue))
                    {
                        options.Template(writer, null);
                    }
                    else
                    {
                        options.Inverse(writer, null);
                    }
                }
            });

            this.RegisterHelper("lookup", (writer, context, arguments) =>
            {
                var lookupSet = (Dictionary<string, string>)arguments[0];
                var lookupValue = (string)arguments[1];

                writer.WriteSafeString(lookupSet[lookupValue]);
            });
        }

        public Action<TextWriter, object> Compile(TextReader template)
        {
            return this.internalHandlebars.Compile(template);
        }

        public Func<object, string> Compile(string template)
        {
            return this.internalHandlebars.Compile(template);
        }

        public Func<object, string> CompileView(string templatePath)
        {
            return this.internalHandlebars.Compile(templatePath);
        }

        public void RegisterHelper(string helperName, HandlebarsParserHelper helperFunction)
        {
            this.internalHandlebars.RegisterHelper(helperName, (HandlebarsHelper)Delegate.CreateDelegate(typeof(HandlebarsHelper), helperFunction.Target, helperFunction.Method));
        }

        public void RegisterHelper(string helperName, HandlebarsParserBlockHelper helperFunction)
        {
            this.internalHandlebars.RegisterHelper(helperName, (HandlebarsBlockHelper) Delegate.CreateDelegate(typeof(HandlebarsBlockHelper), helperFunction.Target, helperFunction.Method));
        }

        public void RegisterTemplate(string templateName, Action<TextWriter, object> template)
        {
            this.internalHandlebars.RegisterTemplate(templateName, template);
        }

        public void RegisterTemplate(string templateName, string template)
        {
            this.internalHandlebars.RegisterTemplate(templateName, template);
        }
    }
}
