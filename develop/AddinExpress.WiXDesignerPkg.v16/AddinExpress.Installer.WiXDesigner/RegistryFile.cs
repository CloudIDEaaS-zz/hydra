using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class RegistryFile
	{
		private Dictionary<string, Dictionary<string, RegistryValue>> _registryKeys;

		public Dictionary<string, Dictionary<string, RegistryValue>> RegistryKeys
		{
			get
			{
				return this._registryKeys;
			}
		}

		public RegistryFile(string fileName)
		{
			this._registryKeys = new Dictionary<string, Dictionary<string, RegistryValue>>();
			if (File.Exists(fileName))
			{
				string str = File.ReadAllText(fileName);
				if (Regex.IsMatch(str, "([ ]*(\r\n)*)REGEDIT4", RegexOptions.IgnoreCase | RegexOptions.Singleline))
				{
					return;
				}
				foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair in this.ParseFile(str))
				{
					Dictionary<string, RegistryValue> strs = new Dictionary<string, RegistryValue>();
					foreach (KeyValuePair<string, string> value in keyValuePair.Value)
					{
						try
						{
							strs.Add(value.Key, new RegistryValue(keyValuePair.Key, value.Key, value.Value));
						}
						catch
						{
						}
					}
					this._registryKeys.Add(keyValuePair.Key, strs);
				}
			}
		}

		private Dictionary<string, string> NormalizeKeys(string content)
		{
			Dictionary<string, string> strs = new Dictionary<string, string>();
			foreach (Match match in Regex.Matches(content, "^[\t ]*\\[.+\\][\r\n]+", RegexOptions.Multiline))
			{
				try
				{
					string str = RegistryFileUtils.RemoveLeadingNewLines(match.Value);
					if (str.EndsWith("="))
					{
						str = str.Substring(0, str.Length - 1);
					}
					str = RegistryFileUtils.RemoveSquareBrackets(str);
					str = (str != "@" ? RegistryFileUtils.RemoveLeadingChars(str, "\"") : "");
					int index = match.Index + match.Length;
					Match match1 = match.NextMatch();
					string str1 = content.Substring(index, (match1.Success ? match1.Index : content.Length) - index);
					str1 = RegistryFileUtils.RemoveLeadingNewLines(str1);
					if (!strs.ContainsKey(str))
					{
						strs.Add(str, str1);
					}
					else
					{
						string item = strs[str];
						StringBuilder stringBuilder = new StringBuilder(item);
						if (!item.EndsWith(Environment.NewLine))
						{
							stringBuilder.AppendLine();
						}
						stringBuilder.Append(str1);
						strs[str] = stringBuilder.ToString();
					}
				}
				catch
				{
				}
			}
			return strs;
		}

		private Dictionary<string, string> NormalizeValues(string content)
		{
			MatchCollection matchCollections = Regex.Matches(content, "^[\\t ]*(\".+\"|@)=(\"[^\"]*\"|[^\"]+)", RegexOptions.Multiline);
			Dictionary<string, string> strs = new Dictionary<string, string>();
			foreach (Match match in matchCollections)
			{
				try
				{
					string str = RegistryFileUtils.RemoveLeadingNewLines(match.Groups[1].Value);
					str = (str != "@" ? RegistryFileUtils.RemoveLeadingChars(str, "\"") : "");
					string str1 = RegistryFileUtils.RemoveLeadingNewLines(match.Groups[2].Value);
					if (!strs.ContainsKey(str))
					{
						strs.Add(str, str1);
					}
					else
					{
						string item = strs[str];
						StringBuilder stringBuilder = new StringBuilder(item);
						if (!item.EndsWith(Environment.NewLine))
						{
							stringBuilder.AppendLine();
						}
						stringBuilder.Append(str1);
						strs[str] = stringBuilder.ToString();
					}
				}
				catch
				{
				}
			}
			return strs;
		}

		private Dictionary<string, Dictionary<string, string>> ParseFile(string content)
		{
			Dictionary<string, Dictionary<string, string>> strs = new Dictionary<string, Dictionary<string, string>>();
			try
			{
				foreach (KeyValuePair<string, string> keyValuePair in this.NormalizeKeys(content))
				{
					Dictionary<string, string> strs1 = this.NormalizeValues(keyValuePair.Value);
					strs.Add(keyValuePair.Key, strs1);
				}
			}
			catch
			{
			}
			return strs;
		}
	}
}