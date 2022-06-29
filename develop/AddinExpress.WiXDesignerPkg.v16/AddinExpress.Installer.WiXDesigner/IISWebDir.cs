using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class IISWebDir : WiXEntity
	{
		internal string DirProperties
		{
			get
			{
				return base.GetAttributeValue("DirProperties");
			}
			set
			{
				base.SetAttributeValue("DirProperties", value);
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

		internal IISWebDir(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}