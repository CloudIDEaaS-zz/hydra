using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXComponent : WiXEntity
	{
		internal string ComPlusFlags
		{
			get
			{
				return base.GetAttributeValue("ComPlusFlags");
			}
			set
			{
				base.SetAttributeValue("ComPlusFlags", value);
				this.SetDirty();
			}
		}

		internal string Directory
		{
			get
			{
				return base.GetAttributeValue("Directory");
			}
			set
			{
				base.SetAttributeValue("Directory", value);
				this.SetDirty();
			}
		}

		internal string DisableRegistryReflection
		{
			get
			{
				return base.GetAttributeValue("DisableRegistryReflection");
			}
			set
			{
				base.SetAttributeValue("DisableRegistryReflection", value);
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

		internal string Feature
		{
			get
			{
				return base.GetAttributeValue("Feature");
			}
			set
			{
				base.SetAttributeValue("Feature", value);
				this.SetDirty();
			}
		}

		internal string Guid
		{
			get
			{
				return base.GetAttributeValue("Guid");
			}
			set
			{
				base.SetAttributeValue("Guid", value);
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

		internal string Location
		{
			get
			{
				return base.GetAttributeValue("Location");
			}
			set
			{
				base.SetAttributeValue("Location", value);
				this.SetDirty();
			}
		}

		internal string MultiInstance
		{
			get
			{
				return base.GetAttributeValue("MultiInstance");
			}
			set
			{
				base.SetAttributeValue("MultiInstance", value);
				this.SetDirty();
			}
		}

		internal string NeverOverwrite
		{
			get
			{
				return base.GetAttributeValue("NeverOverwrite");
			}
			set
			{
				base.SetAttributeValue("NeverOverwrite", value);
				this.SetDirty();
			}
		}

		internal string Permanent
		{
			get
			{
				return base.GetAttributeValue("Permanent");
			}
			set
			{
				base.SetAttributeValue("Permanent", value);
				this.SetDirty();
			}
		}

		internal string Shared
		{
			get
			{
				return base.GetAttributeValue("Shared");
			}
			set
			{
				base.SetAttributeValue("Shared", value);
				this.SetDirty();
			}
		}

		internal string SharedDllRefCount
		{
			get
			{
				return base.GetAttributeValue("SharedDllRefCount");
			}
			set
			{
				base.SetAttributeValue("SharedDllRefCount", value);
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

		internal string Transitive
		{
			get
			{
				return base.GetAttributeValue("Transitive");
			}
			set
			{
				base.SetAttributeValue("Transitive", value);
				this.SetDirty();
			}
		}

		internal string UninstallWhenSuperseded
		{
			get
			{
				return base.GetAttributeValue("UninstallWhenSuperseded");
			}
			set
			{
				base.SetAttributeValue("UninstallWhenSuperseded", value);
				this.SetDirty();
			}
		}

		internal string Win64
		{
			get
			{
				return base.GetAttributeValue("Win64");
			}
			set
			{
				base.SetAttributeValue("Win64", value);
				this.SetDirty();
			}
		}

		internal WiXComponent(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}