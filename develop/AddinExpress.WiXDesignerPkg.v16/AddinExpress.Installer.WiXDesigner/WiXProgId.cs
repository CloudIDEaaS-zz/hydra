using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXProgId : WiXEntity
	{
		internal string Advertise
		{
			get
			{
				return base.GetAttributeValue("Advertise");
			}
			set
			{
				base.SetAttributeValue("Advertise", value);
				this.SetDirty();
			}
		}

		internal string Description
		{
			get
			{
				return base.GetAttributeValue("Description");
			}
			set
			{
				base.SetAttributeValue("Description", value);
				this.SetDirty();
			}
		}

		internal string Icon
		{
			get
			{
				return base.GetAttributeValue("Icon");
			}
			set
			{
				base.SetAttributeValue("Icon", value);
				this.SetDirty();
			}
		}

		internal string IconIndex
		{
			get
			{
				return base.GetAttributeValue("IconIndex");
			}
			set
			{
				base.SetAttributeValue("IconIndex", value);
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

		internal string NoOpen
		{
			get
			{
				return base.GetAttributeValue("NoOpen");
			}
			set
			{
				base.SetAttributeValue("NoOpen", value);
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

		internal WiXProgId(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}