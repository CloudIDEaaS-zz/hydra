using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXCreateFolder : WiXEntity
	{
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

		public override object SupportedObject
		{
			get
			{
				return this;
			}
		}

		internal WiXCreateFolder(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}