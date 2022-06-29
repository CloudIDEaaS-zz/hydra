using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class IISWebError : WiXEntity
	{
		internal string ErrorCode
		{
			get
			{
				return base.GetAttributeValue("ErrorCode");
			}
			set
			{
				base.SetAttributeValue("ErrorCode", value);
				this.SetDirty();
			}
		}

		internal string File
		{
			get
			{
				return base.GetAttributeValue("File");
			}
			set
			{
				base.SetAttributeValue("File", value);
				this.SetDirty();
			}
		}

		internal string SubCode
		{
			get
			{
				return base.GetAttributeValue("SubCode");
			}
			set
			{
				base.SetAttributeValue("SubCode", value);
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

		internal string URL
		{
			get
			{
				return base.GetAttributeValue("URL");
			}
			set
			{
				base.SetAttributeValue("URL", value);
				this.SetDirty();
			}
		}

		internal IISWebError(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}