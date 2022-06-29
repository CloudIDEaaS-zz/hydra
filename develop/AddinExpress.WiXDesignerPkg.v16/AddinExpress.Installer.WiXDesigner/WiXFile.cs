using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXFile : WiXEntity
	{
		internal string Assembly
		{
			get
			{
				return base.GetAttributeValue("Assembly");
			}
			set
			{
				base.SetAttributeValue("Assembly", value);
				this.SetDirty();
			}
		}

		internal string AssemblyApplication
		{
			get
			{
				return base.GetAttributeValue("AssemblyApplication");
			}
			set
			{
				base.SetAttributeValue("AssemblyApplication", value);
				this.SetDirty();
			}
		}

		internal string AssemblyManifest
		{
			get
			{
				return base.GetAttributeValue("AssemblyManifest");
			}
			set
			{
				base.SetAttributeValue("AssemblyManifest", value);
				this.SetDirty();
			}
		}

		internal string BindPath
		{
			get
			{
				return base.GetAttributeValue("BindPath");
			}
			set
			{
				base.SetAttributeValue("BindPath", value);
				this.SetDirty();
			}
		}

		internal string Checksum
		{
			get
			{
				return base.GetAttributeValue("Checksum");
			}
			set
			{
				base.SetAttributeValue("Checksum", value);
				this.SetDirty();
			}
		}

		internal string CompanionFile
		{
			get
			{
				return base.GetAttributeValue("CompanionFile");
			}
			set
			{
				base.SetAttributeValue("CompanionFile", value);
				this.SetDirty();
			}
		}

		internal string Compressed
		{
			get
			{
				return base.GetAttributeValue("Compressed");
			}
			set
			{
				base.SetAttributeValue("Compressed", value);
				this.SetDirty();
			}
		}

		internal string DefaultLanguage
		{
			get
			{
				return base.GetAttributeValue("DefaultLanguage");
			}
			set
			{
				base.SetAttributeValue("DefaultLanguage", value);
				this.SetDirty();
			}
		}

		internal string DefaultSize
		{
			get
			{
				return base.GetAttributeValue("DefaultSize");
			}
			set
			{
				base.SetAttributeValue("DefaultSize", value);
				this.SetDirty();
			}
		}

		internal string DefaultVersion
		{
			get
			{
				return base.GetAttributeValue("DefaultVersion");
			}
			set
			{
				base.SetAttributeValue("DefaultVersion", value);
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

		internal string FontTitle
		{
			get
			{
				return base.GetAttributeValue("FontTitle");
			}
			set
			{
				base.SetAttributeValue("FontTitle", value);
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

		internal string KeyPath
		{
			get
			{
				return base.GetAttributeValue("KeyPath");
			}
			set
			{
				base.SetAttributeValue("KeyPath", value);
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

		internal string PatchAllowIgnoreOnError
		{
			get
			{
				return base.GetAttributeValue("PatchAllowIgnoreOnError");
			}
			set
			{
				base.SetAttributeValue("PatchAllowIgnoreOnError", value);
				this.SetDirty();
			}
		}

		internal string PatchGroup
		{
			get
			{
				return base.GetAttributeValue("PatchGroup");
			}
			set
			{
				base.SetAttributeValue("PatchGroup", value);
				this.SetDirty();
			}
		}

		internal string PatchIgnore
		{
			get
			{
				return base.GetAttributeValue("PatchIgnore");
			}
			set
			{
				base.SetAttributeValue("PatchIgnore", value);
				this.SetDirty();
			}
		}

		internal string PatchWholeFile
		{
			get
			{
				return base.GetAttributeValue("PatchWholeFile");
			}
			set
			{
				base.SetAttributeValue("PatchWholeFile", value);
				this.SetDirty();
			}
		}

		internal string ProcessorArchitecture
		{
			get
			{
				return base.GetAttributeValue("ProcessorArchitecture");
			}
			set
			{
				base.SetAttributeValue("ProcessorArchitecture", value);
				this.SetDirty();
			}
		}

		internal string ReadOnly
		{
			get
			{
				return base.GetAttributeValue("ReadOnly");
			}
			set
			{
				base.SetAttributeValue("ReadOnly", value);
				this.SetDirty();
			}
		}

		internal string SelfRegCost
		{
			get
			{
				return base.GetAttributeValue("SelfRegCost");
			}
			set
			{
				base.SetAttributeValue("SelfRegCost", value);
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

		internal string Source
		{
			get
			{
				return base.GetAttributeValue("Source");
			}
			set
			{
				base.SetAttributeValue("Source", value);
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

		internal string System
		{
			get
			{
				return base.GetAttributeValue("System");
			}
			set
			{
				base.SetAttributeValue("System", value);
				this.SetDirty();
			}
		}

		internal string TrueType
		{
			get
			{
				return base.GetAttributeValue("TrueType");
			}
			set
			{
				base.SetAttributeValue("TrueType", value);
				this.SetDirty();
			}
		}

		internal string Vital
		{
			get
			{
				return base.GetAttributeValue("Vital");
			}
			set
			{
				base.SetAttributeValue("Vital", value);
				this.SetDirty();
			}
		}

		internal WiXFile(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}