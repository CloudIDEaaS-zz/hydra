using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXFeature : WiXEntity
	{
		internal string Absent
		{
			get
			{
				return base.GetAttributeValue("Absent");
			}
			set
			{
				base.SetAttributeValue("Absent", value);
				this.SetDirty();
			}
		}

		internal string AllowAdvertise
		{
			get
			{
				return base.GetAttributeValue("AllowAdvertise");
			}
			set
			{
				base.SetAttributeValue("AllowAdvertise", value);
				this.SetDirty();
			}
		}

		internal string ConfigurableDirectory
		{
			get
			{
				return base.GetAttributeValue("ConfigurableDirectory");
			}
			set
			{
				base.SetAttributeValue("ConfigurableDirectory", value);
				this.SetDirty();
			}
		}

		internal string Description
		{
			get
			{
				return base.GetAttributeValue("Description");
			}
			set
			{
				base.SetAttributeValue("Description", value);
				this.SetDirty();
			}
		}

		internal string Display
		{
			get
			{
				return base.GetAttributeValue("Display");
			}
			set
			{
				base.SetAttributeValue("Display", value);
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

		internal string InstallDefault
		{
			get
			{
				return base.GetAttributeValue("InstallDefault");
			}
			set
			{
				base.SetAttributeValue("InstallDefault", value);
				this.SetDirty();
			}
		}

		internal string Level
		{
			get
			{
				return base.GetAttributeValue("Level");
			}
			set
			{
				base.SetAttributeValue("Level", value);
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

		internal string TypicalDefault
		{
			get
			{
				return base.GetAttributeValue("TypicalDefault");
			}
			set
			{
				base.SetAttributeValue("TypicalDefault", value);
				this.SetDirty();
			}
		}

		internal WiXFeature(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}