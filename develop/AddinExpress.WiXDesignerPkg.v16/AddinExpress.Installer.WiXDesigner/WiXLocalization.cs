using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXLocalization : WiXEntity
	{
		private bool parsed;

		private Dictionary<string, string> locStrings = new Dictionary<string, string>();

		internal string Id
		{
			get
			{
				return base.GetAttributeValue("Language");
			}
			set
			{
			}
		}

		public Dictionary<string, string> Strings
		{
			get
			{
				if (!this.parsed)
				{
					this.Parse();
				}
				return this.locStrings;
			}
		}

		public override object SupportedObject
		{
			get
			{
				if (base.XmlNode.HasChildNodes)
				{
					string locFileID = this.GetLocFileID();
					for (int i = 0; i < base.XmlNode.ChildNodes.Count; i++)
					{
						System.Xml.XmlNode itemOf = base.XmlNode.ChildNodes[i];
						if (itemOf.LocalName.ToLower() == "string" && itemOf.Attributes["Id"].Value == "LangID" && itemOf.FirstChild is XmlText && locFileID == (itemOf.FirstChild as XmlText).Value)
						{
							return this;
						}
					}
				}
				return null;
			}
		}

		internal WiXLocalization(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}

		public string GetLocFileID()
		{
			int num;
			if (!int.TryParse(this.Id, out num))
			{
				return string.Empty;
			}
			if (num > 1049)
			{
				if (num == 1055)
				{
					return "StdUI_TRK";
				}
				if (num == 2052)
				{
					return "StdUI_CHS";
				}
				if (num == 3082)
				{
					return "StdUI_ESN";
				}
			}
			else
			{
				switch (num)
				{
					case 1028:
					{
						return "StdUI_CHT";
					}
					case 1029:
					{
						return "StdUI_CSY";
					}
					case 1030:
					case 1032:
					case 1034:
					case 1035:
					case 1037:
					case 1038:
					case 1039:
					case 1043:
					case 1044:
					{
						break;
					}
					case 1031:
					{
						return "StdUI_DEU";
					}
					case 1033:
					{
						return "StdUI_ENU";
					}
					case 1036:
					{
						return "StdUI_FRA";
					}
					case 1040:
					{
						return "StdUI_ITA";
					}
					case 1041:
					{
						return "StdUI_JPN";
					}
					case 1042:
					{
						return "StdUI_KOR";
					}
					case 1045:
					{
						return "StdUI_PLK";
					}
					case 1046:
					{
						return "StdUI_PTB";
					}
					default:
					{
						if (num == 1049)
						{
							return "StdUI_RUS";
						}
						break;
					}
				}
			}
			if (ProjectUtilities.IsNeutralLCID(num))
			{
				return "StdUI_Neutral";
			}
			CultureInfo cultureInfo = ProjectUtilities.GetCultureInfo(num);
			return string.Concat("StdUI_", cultureInfo.ThreeLetterWindowsLanguageName);
		}

		public void Parse()
		{
			this.locStrings.Clear();
			try
			{
				for (int i = 0; i < base.XmlNode.ChildNodes.Count; i++)
				{
					System.Xml.XmlNode itemOf = base.XmlNode.ChildNodes[i];
					if (itemOf.LocalName.ToLower() == "string" && itemOf.FirstChild != null)
					{
						string value = itemOf.Attributes["Id"].Value;
						if (!string.IsNullOrEmpty(value))
						{
							this.locStrings[value] = itemOf.FirstChild.InnerText;
						}
					}
				}
			}
			finally
			{
				this.parsed = true;
			}
		}

		public void SetStringValue(string stringID, string text)
		{
			bool flag = false;
			for (int i = 0; i < base.XmlNode.ChildNodes.Count; i++)
			{
				System.Xml.XmlNode itemOf = base.XmlNode.ChildNodes[i];
				if (itemOf.LocalName.ToLower() == "string")
				{
					string value = itemOf.Attributes["Id"].Value;
					if (!string.IsNullOrEmpty(value) && value.Equals(stringID))
					{
						if (itemOf.FirstChild != null)
						{
							itemOf.RemoveChild(itemOf.FirstChild);
						}
						itemOf.AppendChild(itemOf.OwnerDocument.CreateCDataSection(text));
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				this.SetDirty();
				if (this.locStrings.ContainsKey(stringID))
				{
					this.locStrings[stringID] = text;
					return;
				}
				this.Parse();
			}
		}
	}
}