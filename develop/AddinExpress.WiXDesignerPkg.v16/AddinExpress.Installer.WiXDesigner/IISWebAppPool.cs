using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class IISWebAppPool : WiXEntity
	{
		internal string CpuAction
		{
			get
			{
				return base.GetAttributeValue("CpuAction");
			}
			set
			{
				base.SetAttributeValue("CpuAction", value);
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

		internal string Identity
		{
			get
			{
				return base.GetAttributeValue("Identity");
			}
			set
			{
				base.SetAttributeValue("Identity", value);
				this.SetDirty();
			}
		}

		internal string IdleTimeout
		{
			get
			{
				return base.GetAttributeValue("IdleTimeout");
			}
			set
			{
				base.SetAttributeValue("IdleTimeout", value);
				this.SetDirty();
			}
		}

		internal string ManagedPipelineMode
		{
			get
			{
				return base.GetAttributeValue("ManagedPipelineMode");
			}
			set
			{
				base.SetAttributeValue("ManagedPipelineMode", value);
				this.SetDirty();
			}
		}

		internal string ManagedRuntimeVersion
		{
			get
			{
				return base.GetAttributeValue("ManagedRuntimeVersion");
			}
			set
			{
				base.SetAttributeValue("ManagedRuntimeVersion", value);
				this.SetDirty();
			}
		}

		internal string MaxCpuUsage
		{
			get
			{
				return base.GetAttributeValue("MaxCpuUsage");
			}
			set
			{
				base.SetAttributeValue("MaxCpuUsage", value);
				this.SetDirty();
			}
		}

		internal string MaxWorkerProcesses
		{
			get
			{
				return base.GetAttributeValue("MaxWorkerProcesses");
			}
			set
			{
				base.SetAttributeValue("MaxWorkerProcesses", value);
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

		internal string PrivateMemory
		{
			get
			{
				return base.GetAttributeValue("PrivateMemory");
			}
			set
			{
				base.SetAttributeValue("PrivateMemory", value);
				this.SetDirty();
			}
		}

		internal string QueueLimit
		{
			get
			{
				return base.GetAttributeValue("QueueLimit");
			}
			set
			{
				base.SetAttributeValue("QueueLimit", value);
				this.SetDirty();
			}
		}

		internal string RecycleMinutes
		{
			get
			{
				return base.GetAttributeValue("RecycleMinutes");
			}
			set
			{
				base.SetAttributeValue("RecycleMinutes", value);
				this.SetDirty();
			}
		}

		internal string RecycleRequests
		{
			get
			{
				return base.GetAttributeValue("RecycleRequests");
			}
			set
			{
				base.SetAttributeValue("RecycleRequests", value);
				this.SetDirty();
			}
		}

		internal string RefreshCpu
		{
			get
			{
				return base.GetAttributeValue("RefreshCpu");
			}
			set
			{
				base.SetAttributeValue("RefreshCpu", value);
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

		internal string User
		{
			get
			{
				return base.GetAttributeValue("User");
			}
			set
			{
				base.SetAttributeValue("User", value);
				this.SetDirty();
			}
		}

		internal string VirtualMemory
		{
			get
			{
				return base.GetAttributeValue("VirtualMemory");
			}
			set
			{
				base.SetAttributeValue("VirtualMemory", value);
				this.SetDirty();
			}
		}

		internal IISWebAppPool(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : base(project, owner, parent, xmlNode)
		{
		}
	}
}