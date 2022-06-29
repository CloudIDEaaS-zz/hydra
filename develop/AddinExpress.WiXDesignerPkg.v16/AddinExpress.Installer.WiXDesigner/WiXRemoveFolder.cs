using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXRemoveFolder : WiXEntity
	{
		internal string Directory
		{
			get
			{
				return base.GetAttributeValue("Directory");
			}
			set
			{
				base.SetAttributeValue("Directory", value);
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

		internal string On
		{
			get
			{
				return base.GetAttributeValue("On");
			}
			set
			{
				base.SetAttributeValue("On", value);
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
				return this;
			}
		}

		internal WiXRemoveFolder(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}