using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSRegistryValueBinary : VSRegistryValueBase
	{
		[Browsable(true)]
		[Description("Specifies the data stored in the selected registry value")]
		[TypeConverter(typeof(StringConverter))]
		public override object Value
		{
			get
			{
				if (base.WiXElement == null)
				{
					return string.Empty;
				}
				return base.WiXElement.Value;
			}
			set
			{
				if (value != null && value is string)
				{
					if (base.WiXElement != null)
					{
						string str = ((string)value).ToUpper().Trim();
						if (this.VerifyBinaryString(str))
						{
							base.WiXElement.Value = str;
							if (str == string.Empty)
							{
								XmlAttribute itemOf = base.WiXElement.XmlNode.Attributes["Value"];
								if (itemOf == null)
								{
									itemOf = base.WiXElement.XmlNode.OwnerDocument.CreateAttribute("Value");
									base.WiXElement.XmlNode.Attributes.Append(itemOf);
								}
								itemOf.Value = "";
							}
						}
					}
					this.DoPropertyChanged();
				}
			}
		}

		public VSRegistryValueBinary()
		{
		}

		public VSRegistryValueBinary(WiXProjectParser project, WiXRegistryValue wixElement, VSComponent parentComponent, VSRegistryValues collection) : base(project, wixElement, parentComponent, collection)
		{
		}

		protected override VSRegistryValueType GetValueType()
		{
			return VSRegistryValueType.vsdrvtBinary;
		}

		protected override string GetWiXValueType()
		{
			return "binary";
		}

		private bool VerifyBinaryString(string str)
		{
			if (str.Length % 2 != 0)
			{
				return false;
			}
			return Regex.IsMatch(str, "^[A-F0-9]+$");
		}
	}
}