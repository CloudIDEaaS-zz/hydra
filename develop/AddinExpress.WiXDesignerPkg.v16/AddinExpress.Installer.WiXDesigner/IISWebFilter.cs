using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class IISWebFilter : WiXEntity
	{
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

		internal string Flags
		{
			get
			{
				return base.GetAttributeValue("Flags");
			}
			set
			{
				base.SetAttributeValue("Flags", value);
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

		internal string LoadOrder
		{
			get
			{
				return base.GetAttributeValue("LoadOrder");
			}
			set
			{
				base.SetAttributeValue("LoadOrder", value);
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

		internal string Path
		{
			get
			{
				return base.GetAttributeValue("Path");
			}
			set
			{
				base.SetAttributeValue("Path", value);
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

		internal string WebSite
		{
			get
			{
				return base.GetAttributeValue("WebSite");
			}
			set
			{
				base.SetAttributeValue("WebSite", value);
				this.SetDirty();
			}
		}

		internal IISWebFilter(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}