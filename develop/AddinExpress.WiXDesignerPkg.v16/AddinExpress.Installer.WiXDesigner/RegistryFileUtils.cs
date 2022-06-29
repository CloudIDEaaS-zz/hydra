using System;
using System.Text.RegularExpressions;

namespace AddinExpress.Installer.WiXDesigner
{
	internal static class RegistryFileUtils
	{
		internal static string RemoveContinueChar(string text)
		{
			return Regex.Replace(text, "\\\\\r\n[ ]*", string.Empty);
		}

		internal static string RemoveLeadingChars(string text, string chars)
		{
			string str = text.Trim();
			if (!str.StartsWith(chars) || !str.EndsWith(chars))
			{
				return str;
			}
			return str.Substring(1, str.Length - chars.Length * 2);
		}

		internal static string RemoveLeadingNewLines(string text)
		{
			string str = text;
			while (str.EndsWith("\r\n"))
			{
				str = str.Substring(0, str.Length - 2);
			}
			return str;
		}

		internal static string RemoveSquareBrackets(string text)
		{
			string str = text.Trim();
			if (!str.StartsWith("[") || !str.EndsWith("]"))
			{
				return str;
			}
			return str.Substring(1, str.Length - 2);
		}
	}
}