using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXDirectoryRef : WiXEntity
	{
		internal string DiskId
		{
			get
			{
				return base.GetAttributeValue("DiskId");
			}
			set
			{
				base.SetAttributeValue("DiskId", value);
				this.SetDirty();
			}
		}

		internal string FileSource
		{
			get
			{
				return base.GetAttributeValue("FileSource");
			}
			set
			{
				base.SetAttributeValue("FileSource", value);
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

		internal string Src
		{
			get
			{
				return base.GetAttributeValue("Src");
			}
			set
			{
				base.SetAttributeValue("Src", value);
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

		internal WiXDirectoryRef(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}