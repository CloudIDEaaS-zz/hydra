namespace Microsoft.Build.Shared
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class QuotingUtilities
    {
        private static readonly char[] splitMarker = new char[1];

        internal static ArrayList SplitUnquoted(string input, params char[] separator)
        {
            int num;
            return SplitUnquoted(input, 0x7fffffff, false, false, out num, separator);
        }

        internal static ArrayList SplitUnquoted(string input, int maxSplits, bool keepEmptySplits, bool unquote, out int emptySplits, params char[] separator)
        {
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrow(maxSplits >= 2, "There is no point calling this method for less than two splits.");
            string str = new StringBuilder().Append(separator).ToString();
            Microsoft.Build.Shared.ErrorUtilities.VerifyThrow(str.IndexOf('"') == -1, "The double-quote character is not supported as a separator.");
            StringBuilder builder = new StringBuilder();
            builder.EnsureCapacity(input.Length);
            bool flag = false;
            int num = 0;
            int num2 = 1;
            for (int i = 0; (i < input.Length) && (num2 < maxSplits); i++)
            {
                char ch = input[i];
                if (ch != '\0')
                {
                    if (ch != '"')
                    {
                        if (ch != '\\')
                        {
                            goto Label_00CD;
                        }
                        builder.Append('\\');
                        num++;
                    }
                    else
                    {
                        builder.Append('"');
                        if ((num % 2) == 0)
                        {
                            if ((flag && (i < (input.Length - 1))) && (input[i + 1] == '"'))
                            {
                                builder.Append('"');
                                i++;
                            }
                            flag = !flag;
                        }
                        num = 0;
                    }
                }
                continue;
            Label_00CD:
                if (!flag && (((str.Length == 0) && char.IsWhiteSpace(input[i])) || (str.IndexOf(input[i]) != -1)))
                {
                    builder.Append('\0');
                    if (++num2 == maxSplits)
                    {
                        builder.Append(input, i + 1, input.Length - (i + 1));
                    }
                }
                else
                {
                    builder.Append(input[i]);
                }
                num = 0;
            }
            ArrayList list = new ArrayList();
            emptySplits = 0;
            foreach (string str2 in builder.ToString().Split(splitMarker, maxSplits))
            {
                string str3 = unquote ? Unquote(str2) : str2;
                if ((str3.Length > 0) || keepEmptySplits)
                {
                    list.Add(str3);
                }
                else
                {
                    emptySplits++;
                }
            }
            return list;
        }

        internal static string Unquote(string input)
        {
            int num;
            return Unquote(input, out num);
        }

        internal static string Unquote(string input, out int doubleQuotesRemoved)
        {
            StringBuilder builder = new StringBuilder();
            builder.EnsureCapacity(input.Length);
            bool flag = false;
            int repeatCount = 0;
            doubleQuotesRemoved = 0;
            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                if (ch != '"')
                {
                    if (ch != '\\')
                    {
                        goto Label_008F;
                    }
                    repeatCount++;
                }
                else
                {
                    builder.Append('\\', repeatCount / 2);
                    if ((repeatCount % 2) == 0)
                    {
                        if ((flag && (i < (input.Length - 1))) && (input[i + 1] == '"'))
                        {
                            builder.Append('"');
                            i++;
                        }
                        flag = !flag;
                        doubleQuotesRemoved++;
                    }
                    else
                    {
                        builder.Append('"');
                    }
                    repeatCount = 0;
                }
                continue;
            Label_008F:
                builder.Append('\\', repeatCount);
                builder.Append(input[i]);
                repeatCount = 0;
            }
            return builder.Append('\\', repeatCount).ToString();
        }
    }
}

