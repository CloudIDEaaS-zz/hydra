using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXProduct : WiXEntity
	{
		internal string Codepage
		{
			get
			{
				return base.GetAttributeValue("Codepage");
			}
			set
			{
				base.SetAttributeValue("Codepage", value);
				this.SetDirty();
			}
		}

		internal string Id
		{
			get
			{
				return base.GetAttributeValue("Id");
			}
			set
			{
				base.SetAttributeValue("Id", value);
				this.SetDirty();
			}
		}

		internal string Language
		{
			get
			{
				return base.GetAttributeValue("Language");
			}
			set
			{
				base.SetAttributeValue("Language", value);
				this.SetDirty();
			}
		}

		internal string Manufacturer
		{
			get
			{
				return base.GetAttributeValue("Manufacturer");
			}
			set
			{
				base.SetAttributeValue("Manufacturer", value);
				this.SetDirty();
			}
		}

		internal new string Name
		{
			get
			{
				return base.GetAttributeValue("Name");
			}
			set
			{
				base.SetAttributeValue("Name", value);
				this.SetDirty();
			}
		}

		public override object SupportedObject
		{
			get
			{
				return this;
			}
		}

		internal string UpgradeCode
		{
			get
			{
				return base.GetAttributeValue("UpgradeCode");
			}
			set
			{
				base.SetAttributeValue("UpgradeCode", value);
				this.SetDirty();
			}
		}

		internal string Version
		{
			get
			{
				return base.GetAttributeValue("Version");
			}
			set
			{
				base.SetAttributeValue("Version", value);
				this.SetDirty();
			}
		}

		internal WiXProduct(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}