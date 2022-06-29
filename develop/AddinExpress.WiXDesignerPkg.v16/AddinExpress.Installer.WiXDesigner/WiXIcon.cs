using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXIcon : WiXEntity
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

		internal string SourceFile
		{
			get
			{
				return base.GetAttributeValue("SourceFile");
			}
			set
			{
				base.SetAttributeValue("SourceFile", value);
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

		internal WiXIcon(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}