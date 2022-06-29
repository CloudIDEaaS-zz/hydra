using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class IISHttpHeader : WiXEntity
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

		internal string Value
		{
			get
			{
				return base.GetAttributeValue("Value");
			}
			set
			{
				base.SetAttributeValue("Value", value);
				this.SetDirty();
			}
		}

		internal IISHttpHeader(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}