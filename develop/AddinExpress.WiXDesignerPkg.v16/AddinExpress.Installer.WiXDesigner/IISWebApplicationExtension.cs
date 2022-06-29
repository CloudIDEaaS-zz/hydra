using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class IISWebApplicationExtension : WiXEntity
	{
		internal string CheckPath
		{
			get
			{
				return base.GetAttributeValue("CheckPath");
			}
			set
			{
				base.SetAttributeValue("CheckPath", value);
				this.SetDirty();
			}
		}

		internal string Executable
		{
			get
			{
				return base.GetAttributeValue("Executable");
			}
			set
			{
				base.SetAttributeValue("Executable", value);
				this.SetDirty();
			}
		}

		internal string Extension
		{
			get
			{
				return base.GetAttributeValue("Extension");
			}
			set
			{
				base.SetAttributeValue("Extension", value);
				this.SetDirty();
			}
		}

		internal string Script
		{
			get
			{
				return base.GetAttributeValue("Script");
			}
			set
			{
				base.SetAttributeValue("Script", value);
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

		internal string Verbs
		{
			get
			{
				return base.GetAttributeValue("Verbs");
			}
			set
			{
				base.SetAttributeValue("Verbs", value);
				this.SetDirty();
			}
		}

		internal IISWebApplicationExtension(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}