using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXTextStyle : WiXEntity
	{
		internal string Blue
		{
			get
			{
				return base.GetAttributeValue("Blue");
			}
			set
			{
				base.SetAttributeValue("Blue", value);
				this.SetDirty();
			}
		}

		internal string Bold
		{
			get
			{
				return base.GetAttributeValue("Bold");
			}
			set
			{
				base.SetAttributeValue("Bold", value);
				this.SetDirty();
			}
		}

		internal string FaceName
		{
			get
			{
				return base.GetAttributeValue("FaceName");
			}
			set
			{
				base.SetAttributeValue("FaceName", value);
				this.SetDirty();
			}
		}

		internal string Green
		{
			get
			{
				return base.GetAttributeValue("Green");
			}
			set
			{
				base.SetAttributeValue("Green", value);
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

		internal string Italic
		{
			get
			{
				return base.GetAttributeValue("Italic");
			}
			set
			{
				base.SetAttributeValue("Italic", value);
				this.SetDirty();
			}
		}

		internal string Red
		{
			get
			{
				return base.GetAttributeValue("Red");
			}
			set
			{
				base.SetAttributeValue("Red", value);
				this.SetDirty();
			}
		}

		internal string Size
		{
			get
			{
				return base.GetAttributeValue("Size");
			}
			set
			{
				base.SetAttributeValue("Size", value);
				this.SetDirty();
			}
		}

		internal string Strike
		{
			get
			{
				return base.GetAttributeValue("Strike");
			}
			set
			{
				base.SetAttributeValue("Strike", value);
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

		internal string Underline
		{
			get
			{
				return base.GetAttributeValue("Underline");
			}
			set
			{
				base.SetAttributeValue("Underline", value);
				this.SetDirty();
			}
		}

		internal WiXTextStyle(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}