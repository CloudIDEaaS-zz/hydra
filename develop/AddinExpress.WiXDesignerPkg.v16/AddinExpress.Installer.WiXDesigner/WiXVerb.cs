using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXVerb : WiXEntity
	{
		internal string Argument
		{
			get
			{
				return base.GetAttributeValue("Argument");
			}
			set
			{
				base.SetAttributeValue("Argument", value);
				this.SetDirty();
			}
		}

		internal string Command
		{
			get
			{
				return base.GetAttributeValue("Command");
			}
			set
			{
				base.SetAttributeValue("Command", value);
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

		internal string Target
		{
			get
			{
				return base.GetAttributeValue("Target");
			}
			set
			{
				base.SetAttributeValue("Target", value);
				this.SetDirty();
			}
		}

		internal string TargetFile
		{
			get
			{
				return base.GetAttributeValue("TargetFile");
			}
			set
			{
				base.SetAttributeValue("TargetFile", value);
				if (!string.IsNullOrEmpty(value))
				{
					base.SetAttributeValue("TargetProperty", null);
				}
				this.SetDirty();
			}
		}

		internal string TargetProperty
		{
			get
			{
				return base.GetAttributeValue("TargetProperty");
			}
			set
			{
				base.SetAttributeValue("TargetProperty", value);
				if (!string.IsNullOrEmpty(value))
				{
					base.SetAttributeValue("TargetFile", null);
				}
				this.SetDirty();
			}
		}

		internal WiXVerb(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}