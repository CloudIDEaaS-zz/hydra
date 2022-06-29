using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXDependency : WiXEntity
	{
		internal string RequiredId
		{
			get
			{
				return base.GetAttributeValue("RequiredId");
			}
			set
			{
				base.SetAttributeValue("RequiredId", value);
				this.SetDirty();
			}
		}

		internal string RequiredLanguage
		{
			get
			{
				return base.GetAttributeValue("RequiredLanguage");
			}
			set
			{
				base.SetAttributeValue("RequiredLanguage", value);
				this.SetDirty();
			}
		}

		internal string RequiredVersion
		{
			get
			{
				return base.GetAttributeValue("RequiredVersion");
			}
			set
			{
				base.SetAttributeValue("RequiredVersion", value);
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

		internal WiXDependency(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}