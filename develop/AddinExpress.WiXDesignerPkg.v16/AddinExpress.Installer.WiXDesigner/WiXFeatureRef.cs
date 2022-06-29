using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXFeatureRef : WiXEntity
	{
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

		internal string IgnoreParent
		{
			get
			{
				return base.GetAttributeValue("IgnoreParent");
			}
			set
			{
				base.SetAttributeValue("IgnoreParent", value);
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

		internal WiXFeatureRef(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}