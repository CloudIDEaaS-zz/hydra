using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXDirectory : WiXEntity
	{
		internal string ComponentGuidGenerationSeed
		{
			get
			{
				return base.GetAttributeValue("ComponentGuidGenerationSeed");
			}
			set
			{
				base.SetAttributeValue("ComponentGuidGenerationSeed", value);
				this.SetDirty();
			}
		}

		internal string DiskId
		{
			get
			{
				return base.GetAttributeValue("DiskId");
			}
			set
			{
				base.SetAttributeValue("DiskId", value);
				this.SetDirty();
			}
		}

		internal string FileSource
		{
			get
			{
				return base.GetAttributeValue("FileSource");
			}
			set
			{
				base.SetAttributeValue("FileSource", value);
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

		internal string LongName
		{
			get
			{
				return base.GetAttributeValue("LongName");
			}
			set
			{
				base.SetAttributeValue("LongName", value);
				this.SetDirty();
			}
		}

		internal string LongSource
		{
			get
			{
				return base.GetAttributeValue("LongSource");
			}
			set
			{
				base.SetAttributeValue("LongSource", value);
				this.SetDirty();
			}
		}

		internal new string Name
		{
			get
			{
				return base.GetAttributeValue("Name");
			}
			set
			{
				base.SetAttributeValue("Name", value);
				this.SetDirty();
			}
		}

		internal string ShortName
		{
			get
			{
				return base.GetAttributeValue("ShortName");
			}
			set
			{
				base.SetAttributeValue("ShortName", value);
				this.SetDirty();
			}
		}

		internal string ShortSourceName
		{
			get
			{
				return base.GetAttributeValue("ShortSourceName");
			}
			set
			{
				base.SetAttributeValue("ShortSourceName", value);
				this.SetDirty();
			}
		}

		internal string SourceName
		{
			get
			{
				return base.GetAttributeValue("SourceName");
			}
			set
			{
				base.SetAttributeValue("SourceName", value);
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

		internal WiXDirectory(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}

		internal bool InDirectoryRef()
		{
			for (IWiXEntity i = this.Parent; i != null; i = i.Parent)
			{
				if (i.IsSupported && i.SupportedObject is WiXDirectoryRef)
				{
					return true;
				}
			}
			return false;
		}
	}
}