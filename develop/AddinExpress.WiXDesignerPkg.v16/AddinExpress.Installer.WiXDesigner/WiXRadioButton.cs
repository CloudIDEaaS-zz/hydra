using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXRadioButton : WiXEntity
	{
		internal string Bitmap
		{
			get
			{
				return base.GetAttributeValue("Bitmap");
			}
			set
			{
				base.SetAttributeValue("Bitmap", value);
				this.SetDirty();
			}
		}

		internal string Height
		{
			get
			{
				return base.GetAttributeValue("Height");
			}
			set
			{
				base.SetAttributeValue("Height", value);
				this.SetDirty();
			}
		}

		internal string Help
		{
			get
			{
				return base.GetAttributeValue("Help");
			}
			set
			{
				base.SetAttributeValue("Help", value);
				this.SetDirty();
			}
		}

		internal string Icon
		{
			get
			{
				return base.GetAttributeValue("Icon");
			}
			set
			{
				base.SetAttributeValue("Icon", value);
				this.SetDirty();
			}
		}

		public override object SupportedObject
		{
			get
			{
				return null;
			}
		}

		internal string Text
		{
			get
			{
				return base.GetAttributeValue("Text");
			}
			set
			{
				base.SetAttributeValue("Text", value);
				this.SetDirty();
			}
		}

		internal string ToolTip
		{
			get
			{
				return base.GetAttributeValue("ToolTip");
			}
			set
			{
				base.SetAttributeValue("ToolTip", value);
				this.SetDirty();
			}
		}

		internal string Value
		{
			get
			{
				return base.GetAttributeValue("Value");
			}
			set
			{
				base.SetAttributeValue("Value", value);
				this.SetDirty();
			}
		}

		internal string Width
		{
			get
			{
				return base.GetAttributeValue("Width");
			}
			set
			{
				base.SetAttributeValue("Width", value);
				this.SetDirty();
			}
		}

		internal string X
		{
			get
			{
				return base.GetAttributeValue("X");
			}
			set
			{
				base.SetAttributeValue("X", value);
				this.SetDirty();
			}
		}

		internal string Y
		{
			get
			{
				return base.GetAttributeValue("Y");
			}
			set
			{
				base.SetAttributeValue("Y", value);
				this.SetDirty();
			}
		}

		internal WiXRadioButton(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}