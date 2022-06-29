using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXModule : WiXEntity
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

		internal string Guid
		{
			get
			{
				return base.GetAttributeValue("Guid");
			}
			set
			{
				base.SetAttributeValue("Guid", value);
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

		public override object SupportedObject
		{
			get
			{
				return this;
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

		internal WiXModule(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}