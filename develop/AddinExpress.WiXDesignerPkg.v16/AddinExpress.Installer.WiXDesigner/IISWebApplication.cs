using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class IISWebApplication : WiXEntity
	{
		internal string AllowSessions
		{
			get
			{
				return base.GetAttributeValue("AllowSessions");
			}
			set
			{
				base.SetAttributeValue("AllowSessions", value);
				this.SetDirty();
			}
		}

		internal string Buffer
		{
			get
			{
				return base.GetAttributeValue("Buffer");
			}
			set
			{
				base.SetAttributeValue("Buffer", value);
				this.SetDirty();
			}
		}

		internal string ClientDebugging
		{
			get
			{
				return base.GetAttributeValue("ClientDebugging");
			}
			set
			{
				base.SetAttributeValue("ClientDebugging", value);
				this.SetDirty();
			}
		}

		internal string DefaultScript
		{
			get
			{
				return base.GetAttributeValue("DefaultScript");
			}
			set
			{
				base.SetAttributeValue("DefaultScript", value);
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

		internal string Isolation
		{
			get
			{
				return base.GetAttributeValue("Isolation");
			}
			set
			{
				base.SetAttributeValue("Isolation", value);
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

		internal string ParentPaths
		{
			get
			{
				return base.GetAttributeValue("ParentPaths");
			}
			set
			{
				base.SetAttributeValue("ParentPaths", value);
				this.SetDirty();
			}
		}

		internal string ScriptTimeout
		{
			get
			{
				return base.GetAttributeValue("ScriptTimeout");
			}
			set
			{
				base.SetAttributeValue("ScriptTimeout", value);
				this.SetDirty();
			}
		}

		internal string ServerDebugging
		{
			get
			{
				return base.GetAttributeValue("ServerDebugging");
			}
			set
			{
				base.SetAttributeValue("ServerDebugging", value);
				this.SetDirty();
			}
		}

		internal string SessionTimeout
		{
			get
			{
				return base.GetAttributeValue("SessionTimeout");
			}
			set
			{
				base.SetAttributeValue("SessionTimeout", value);
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

		internal string WebAppPool
		{
			get
			{
				return base.GetAttributeValue("WebAppPool");
			}
			set
			{
				base.SetAttributeValue("WebAppPool", value);
				this.SetDirty();
			}
		}

		internal IISWebApplication(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}