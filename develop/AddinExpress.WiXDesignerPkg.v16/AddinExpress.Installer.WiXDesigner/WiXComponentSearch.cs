using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXComponentSearch : WiXEntity
	{
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

		internal string Type
		{
			get
			{
				return base.GetAttributeValue("Type");
			}
			set
			{
				base.SetAttributeValue("Type", value);
				this.SetDirty();
			}
		}

		internal WiXComponentSearch(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}