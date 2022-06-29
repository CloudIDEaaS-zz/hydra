using System;
using System.Text;
using System.Text.RegularExpressions;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class RegistryValue
	{
		private string _name = string.Empty;

		private string _value = string.Empty;

		private string _type = string.Empty;

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public string Type
		{
			get
			{
				return this._type;
			}
		}

		public string Value
		{
			get
			{
				return this._value;
			}
		}

		public RegistryValue(string regKeyName, string regValueName, string regValueData)
		{
			this._name = regValueName;
			this._value = regValueData;
			string str = this._value;
			this._type = this.GetRegEntryType(ref str);
			this._value = str;
		}

		private string GetRegEntryType(ref string text)
		{
			if (text.StartsWith("hex(a):"))
			{
				text = text.Substring(7);
				return "REG_RESOURCE_REQUIREMENTS_LIST";
			}
			if (text.StartsWith("hex(b):"))
			{
				text = text.Substring(7);
				return "REG_QWORD";
			}
			if (text.StartsWith("dword:"))
			{
				int num = Convert.ToInt32(text.Substring(6), 16);
				text = num.ToString();
				return "REG_DWORD";
			}
			if (text.StartsWith("hex(7):"))
			{
				text = RegistryFileUtils.RemoveContinueChar(text.Substring(7));
				text = this.GetStringFromHex(text.Split(new char[] { ',' }));
				return "REG_MULTI_SZ";
			}
			if (text.StartsWith("hex(6):"))
			{
				text = RegistryFileUtils.RemoveContinueChar(text.Substring(7));
				text = this.GetStringFromHex(text.Split(new char[] { ',' }));
				return "REG_LINK";
			}
			if (text.StartsWith("hex(2):"))
			{
				text = RegistryFileUtils.RemoveContinueChar(text.Substring(7));
				text = this.GetStringFromHex(text.Split(new char[] { ',' }));
				return "REG_EXPAND_SZ";
			}
			if (text.StartsWith("hex(0):"))
			{
				text = text.Substring(7);
				return "REG_NONE";
			}
			if (!text.StartsWith("hex:"))
			{
				text = Regex.Unescape(text);
				text = RegistryFileUtils.RemoveLeadingChars(text, "\"");
				return "REG_SZ";
			}
			text = RegistryFileUtils.RemoveContinueChar(text.Substring(4));
			if (text.EndsWith(","))
			{
				text = text.Substring(0, text.Length - 1);
			}
			return "REG_BINARY";
		}

		private string GetStringFromHex(string[] stringArray)
		{
			if ((int)stringArray.Length <= 1)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < (int)stringArray.Length - 2; i += 2)
			{
				string str = string.Concat(stringArray[i + 1], stringArray[i]);
				if (str != "0000")
				{
					char chr = Convert.ToChar(Convert.ToInt32(str, 16));
					stringBuilder.Append(chr);
				}
				else
				{
					stringBuilder.Append(Environment.NewLine);
				}
			}
			return stringBuilder.ToString();
		}
	}
}