using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class IISWebVirtualDir : WiXEntity
	{
		internal string Alias
		{
			get
			{
				return base.GetAttributeValue("Alias");
			}
			set
			{
				base.SetAttributeValue("Alias", value);
				this.SetDirty();
			}
		}

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

		public override object SupportedObject
		{
			get
			{
				return this;
			}
		}

		internal string WebApplication
		{
			get
			{
				return base.GetAttributeValue("WebApplication");
			}
			set
			{
				base.SetAttributeValue("WebApplication", value);
				this.SetDirty();
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

		internal IISWebVirtualDir(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}