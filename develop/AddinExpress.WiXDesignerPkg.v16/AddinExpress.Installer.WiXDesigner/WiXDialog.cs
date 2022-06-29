using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXDialog : WiXEntity
	{
		internal string CustomPalette
		{
			get
			{
				return base.GetAttributeValue("CustomPalette");
			}
			set
			{
				base.SetAttributeValue("CustomPalette", value);
				this.SetDirty();
			}
		}

		internal string ErrorDialog
		{
			get
			{
				return base.GetAttributeValue("ErrorDialog");
			}
			set
			{
				base.SetAttributeValue("ErrorDialog", value);
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

		internal string Hidden
		{
			get
			{
				return base.GetAttributeValue("Hidden");
			}
			set
			{
				base.SetAttributeValue("Hidden", value);
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

		internal string KeepModeless
		{
			get
			{
				return base.GetAttributeValue("KeepModeless");
			}
			set
			{
				base.SetAttributeValue("KeepModeless", value);
				this.SetDirty();
			}
		}

		internal string LeftScroll
		{
			get
			{
				return base.GetAttributeValue("LeftScroll");
			}
			set
			{
				base.SetAttributeValue("LeftScroll", value);
				this.SetDirty();
			}
		}

		internal string Modeless
		{
			get
			{
				return base.GetAttributeValue("Modeless");
			}
			set
			{
				base.SetAttributeValue("Modeless", value);
				this.SetDirty();
			}
		}

		internal string NoMinimize
		{
			get
			{
				return base.GetAttributeValue("NoMinimize");
			}
			set
			{
				base.SetAttributeValue("NoMinimize", value);
				this.SetDirty();
			}
		}

		internal string RightAligned
		{
			get
			{
				return base.GetAttributeValue("RightAligned");
			}
			set
			{
				base.SetAttributeValue("RightAligned", value);
				this.SetDirty();
			}
		}

		internal string RightToLeft
		{
			get
			{
				return base.GetAttributeValue("RightToLeft");
			}
			set
			{
				base.SetAttributeValue("RightToLeft", value);
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

		internal string SystemModal
		{
			get
			{
				return base.GetAttributeValue("SystemModal");
			}
			set
			{
				base.SetAttributeValue("SystemModal", value);
				this.SetDirty();
			}
		}

		internal string Title
		{
			get
			{
				return base.GetAttributeValue("Title");
			}
			set
			{
				base.SetAttributeValue("Title", value);
				this.SetDirty();
			}
		}

		internal string TrackDiskSpace
		{
			get
			{
				return base.GetAttributeValue("TrackDiskSpace");
			}
			set
			{
				base.SetAttributeValue("TrackDiskSpace", value);
				this.SetDirty();
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

		internal WiXDialog(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}