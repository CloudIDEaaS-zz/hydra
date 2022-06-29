using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXPublish : WiXEntity
	{
		internal string Control
		{
			get
			{
				return base.GetAttributeValue("Control");
			}
			set
			{
				base.SetAttributeValue("Control", value);
				this.SetDirty();
			}
		}

		internal string Dialog
		{
			get
			{
				return base.GetAttributeValue("Dialog");
			}
			set
			{
				base.SetAttributeValue("Dialog", value);
				this.SetDirty();
			}
		}

		internal string Event
		{
			get
			{
				return base.GetAttributeValue("Event");
			}
			set
			{
				base.SetAttributeValue("Event", value);
				this.SetDirty();
			}
		}

		internal string Order
		{
			get
			{
				return base.GetAttributeValue("Order");
			}
			set
			{
				base.SetAttributeValue("Order", value);
				this.SetDirty();
			}
		}

		internal string Property
		{
			get
			{
				return base.GetAttributeValue("Property");
			}
			set
			{
				base.SetAttributeValue("Property", value);
				this.SetDirty();
			}
		}

		public override object SupportedObject
		{
			get
			{
				return null;
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

		internal WiXPublish(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}