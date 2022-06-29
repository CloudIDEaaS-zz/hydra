using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXDialogRef : WiXEntity
	{
		public override object SupportedObject
		{
			get
			{
				return this;
			}
		}

		internal string VSName
		{
			get
			{
				return base.GetAttributeValue("VSName", "http://schemas.add-in-express.com/wixdesigner");
			}
			set
			{
				base.SetAttributeValue("VSName", "http://schemas.add-in-express.com/wixdesigner", value);
				this.SetDirty();
			}
		}

		internal WiXDialogRef(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}