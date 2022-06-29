using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXShow : WiXEntity
	{
		internal string After
		{
			get
			{
				return base.GetAttributeValue("After");
			}
			set
			{
				base.SetAttributeValue("After", value);
				this.SetDirty();
			}
		}

		internal string Before
		{
			get
			{
				return base.GetAttributeValue("Before");
			}
			set
			{
				base.SetAttributeValue("Before", value);
				this.SetDirty();
			}
		}

		internal string Dialog
		{
			get
			{
				return base.GetAttributeValue("Dialog");
			}
			set
			{
				base.SetAttributeValue("Dialog", value);
				this.SetDirty();
			}
		}

		internal string OnExit
		{
			get
			{
				return base.GetAttributeValue("OnExit");
			}
			set
			{
				base.SetAttributeValue("OnExit", value);
				this.SetDirty();
			}
		}

		internal string Overridable
		{
			get
			{
				return base.GetAttributeValue("Overridable");
			}
			set
			{
				base.SetAttributeValue("Overridable", value);
				this.SetDirty();
			}
		}

		internal string Sequence
		{
			get
			{
				return base.GetAttributeValue("Sequence");
			}
			set
			{
				base.SetAttributeValue("Sequence", value);
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

		internal WiXShow(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}