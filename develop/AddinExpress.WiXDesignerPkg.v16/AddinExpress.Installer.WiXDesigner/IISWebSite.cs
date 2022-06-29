using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class IISWebSite : WiXEntity
	{
		internal string AutoStart
		{
			get
			{
				return base.GetAttributeValue("AutoStart");
			}
			set
			{
				base.SetAttributeValue("AutoStart", value);
				this.SetDirty();
			}
		}

		internal string ConfigureIfExists
		{
			get
			{
				return base.GetAttributeValue("ConfigureIfExists");
			}
			set
			{
				base.SetAttributeValue("ConfigureIfExists", value);
				this.SetDirty();
			}
		}

		internal string ConnectionTimeout
		{
			get
			{
				return base.GetAttributeValue("ConnectionTimeout");
			}
			set
			{
				base.SetAttributeValue("ConnectionTimeout", value);
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

		internal string Sequence
		{
			get
			{
				return base.GetAttributeValue("Sequence");
			}
			set
			{
				base.SetAttributeValue("Sequence", value);
				this.SetDirty();
			}
		}

		internal string SiteId
		{
			get
			{
				return base.GetAttributeValue("SiteId");
			}
			set
			{
				base.SetAttributeValue("SiteId", value);
				this.SetDirty();
			}
		}

		internal string StartOnInstall
		{
			get
			{
				return base.GetAttributeValue("StartOnInstall");
			}
			set
			{
				base.SetAttributeValue("StartOnInstall", value);
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

		internal string WebLog
		{
			get
			{
				return base.GetAttributeValue("WebLog");
			}
			set
			{
				base.SetAttributeValue("WebLog", value);
				this.SetDirty();
			}
		}

		internal IISWebSite(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}