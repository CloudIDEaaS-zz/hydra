using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    public abstract class ParseResultBase
    {
        public bool IsArg(string value, string arg, bool supportColon = true)
        {
            if (supportColon)
            {
                var index = value.IndexOf(':');

                if (index != -1)
                {
                    value = value.Substring(0, index);
                }
            }

            return (string.Compare(value, arg, StringComparison.OrdinalIgnoreCase) == 0);
        }
    }

    public class DefaultParseResult : ParseResultBase
    {
    }

    public static class CommandLineParser
    {
        public static T ParseArgs<T>(string[] arguments, Action<T, string> parseArg, Action<T, string, string> parseSwitch) where T : ParseResultBase, new()
        {
            ParseResultBase result = typeof(T).CreateInstance<T>();

            for (int i = 0; i < arguments.Length; i++)
            {
                string str = arguments[i];

                if (!string.IsNullOrEmpty(str))
                {
                    int colonIndex;
                    int equalsIndex;
                    int spaceIndex;
                    string colonArgument = null;
                    string equalsArgument = null;
                    string spaceArgument = null;

                    str = str.Trim();

                    if (!string.IsNullOrEmpty(str))
                    {
                        if ((str[0] == '/') || (str[0] == '-'))
                        {
                            str = str.Substring(1);

                            if ((str[0] == '-'))
                            {
                                str = str.Substring(1);
                            }
                        }
                        else
                        {
                            parseArg((T)result, str);
                            continue;
                        }

                        equalsIndex = str.IndexOf('=');
                        colonIndex = str.IndexOf(':');
                        spaceIndex = str.IndexOf(' ');

                        if (equalsIndex != -1)
                        {
                            equalsArgument = str.Right(str.Length - equalsIndex - 1);
                            str = str.Left(equalsIndex);

                            parseSwitch((T)result, str, equalsArgument);
                        }
                        else if (colonIndex != -1)
                        {
                            colonArgument = str.Right(str.Length - colonIndex - 1);
                            str = str.Left(colonIndex);

                            parseSwitch((T)result, str, colonArgument);
                        }
                        else if (spaceIndex != -1)
                        {
                            spaceArgument = str.Right(str.Length - spaceIndex - 1);
                            str = str.Left(spaceIndex);

                            parseSwitch((T)result, str, spaceArgument);
                        }
                        else
                        {
                            parseSwitch((T)result, str, null);
                        }
                    }
                }
            }

            return (T) result;
        }
    }
}
