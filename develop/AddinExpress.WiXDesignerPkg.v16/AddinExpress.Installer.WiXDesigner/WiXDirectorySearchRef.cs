using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXDirectorySearchRef : WiXEntity
	{
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

		internal new string Parent
		{
			get
			{
				return base.GetAttributeValue("Parent");
			}
			set
			{
				base.SetAttributeValue("Parent", value);
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

		internal WiXDirectorySearchRef(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}