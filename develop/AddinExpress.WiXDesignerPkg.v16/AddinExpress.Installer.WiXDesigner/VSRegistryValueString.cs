using System;
using System.ComponentModel;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSRegistryValueString : VSRegistryValueBase
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
						base.WiXElement.Value = (string)value;
						if ((string)value == string.Empty)
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
					this.DoPropertyChanged();
				}
			}
		}

		public VSRegistryValueString()
		{
		}

		public VSRegistryValueString(WiXProjectParser project, WiXRegistryValue wixElement, VSComponent parentComponent, VSRegistryValues collection) : base(project, wixElement, parentComponent, collection)
		{
		}

		protected override VSRegistryValueType GetValueType()
		{
			return VSRegistryValueType.vsdrvtString;
		}

		protected override string GetWiXValueType()
		{
			return "string";
		}
	}
}