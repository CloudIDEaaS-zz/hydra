using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXPackage : WiXEntity
	{
		internal string AdminImage
		{
			get
			{
				return base.GetAttributeValue("AdminImage");
			}
			set
			{
				base.SetAttributeValue("AdminImage", value);
				this.SetDirty();
			}
		}

		internal string Comments
		{
			get
			{
				return base.GetAttributeValue("Comments");
			}
			set
			{
				base.SetAttributeValue("Comments", value);
				this.SetDirty();
			}
		}

		internal string Compressed
		{
			get
			{
				return base.GetAttributeValue("Compressed");
			}
			set
			{
				base.SetAttributeValue("Compressed", value);
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

		internal string InstallerVersion
		{
			get
			{
				return base.GetAttributeValue("InstallerVersion");
			}
			set
			{
				base.SetAttributeValue("InstallerVersion", value);
				this.SetDirty();
			}
		}

		internal string InstallPrivileges
		{
			get
			{
				return base.GetAttributeValue("InstallPrivileges");
			}
			set
			{
				base.SetAttributeValue("InstallPrivileges", value);
				this.SetDirty();
			}
		}

		internal string InstallScope
		{
			get
			{
				return base.GetAttributeValue("InstallScope");
			}
			set
			{
				base.SetAttributeValue("InstallScope", value);
				this.SetDirty();
			}
		}

		internal string Keywords
		{
			get
			{
				return base.GetAttributeValue("Keywords");
			}
			set
			{
				base.SetAttributeValue("Keywords", value);
				this.SetDirty();
			}
		}

		internal string Languages
		{
			get
			{
				return base.GetAttributeValue("Languages");
			}
			set
			{
				base.SetAttributeValue("Languages", value);
				this.SetDirty();
			}
		}

		internal string Manufacturer
		{
			get
			{
				return base.GetAttributeValue("Manufacturer");
			}
			set
			{
				base.SetAttributeValue("Manufacturer", value);
				this.SetDirty();
			}
		}

		internal string Platform
		{
			get
			{
				return base.GetAttributeValue("Platform");
			}
			set
			{
				base.SetAttributeValue("Platform", value);
				this.SetDirty();
			}
		}

		internal string Platforms
		{
			get
			{
				return base.GetAttributeValue("Platforms");
			}
			set
			{
				base.SetAttributeValue("Platforms", value);
				this.SetDirty();
			}
		}

		internal string ReadOnly
		{
			get
			{
				return base.GetAttributeValue("ReadOnly");
			}
			set
			{
				base.SetAttributeValue("ReadOnly", value);
				this.SetDirty();
			}
		}

		internal string ShortNames
		{
			get
			{
				return base.GetAttributeValue("ShortNames");
			}
			set
			{
				base.SetAttributeValue("ShortNames", value);
				this.SetDirty();
			}
		}

		internal string SummaryCodepage
		{
			get
			{
				return base.GetAttributeValue("SummaryCodepage");
			}
			set
			{
				base.SetAttributeValue("SummaryCodepage", value);
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

		internal WiXPackage(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}