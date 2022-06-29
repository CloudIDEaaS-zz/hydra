using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXRegistryKey : WiXEntity
	{
		internal string Action
		{
			get
			{
				return base.GetAttributeValue("Action");
			}
			set
			{
				base.SetAttributeValue("Action", value);
				this.SetDirty();
			}
		}

		internal string ForceCreateOnInstall
		{
			get
			{
				return base.GetAttributeValue("ForceCreateOnInstall");
			}
			set
			{
				base.SetAttributeValue("ForceCreateOnInstall", value);
				this.SetDirty();
			}
		}

		internal string ForceDeleteOnUninstall
		{
			get
			{
				return base.GetAttributeValue("ForceDeleteOnUninstall");
			}
			set
			{
				base.SetAttributeValue("ForceDeleteOnUninstall", value);
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

		internal string Key
		{
			get
			{
				return base.GetAttributeValue("Key");
			}
			set
			{
				base.SetAttributeValue("Key", value);
				this.SetDirty();
			}
		}

		internal string Root
		{
			get
			{
				return base.GetAttributeValue("Root");
			}
			set
			{
				base.SetAttributeValue("Root", value);
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

		internal WiXRegistryKey(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}