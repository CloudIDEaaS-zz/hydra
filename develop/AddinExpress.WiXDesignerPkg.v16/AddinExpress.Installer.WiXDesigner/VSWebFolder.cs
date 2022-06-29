using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSWebFolder : VSBaseFolder
	{
		private IISWebVirtualDir _wixWebVirtualDir;

		private IISWebApplication _wixWebApplication;

		private IISWebDir _wixWebDir;

		private IISWebDirProperties _wixWebDirProperties;

		[Browsable(false)]
		[DefaultValue(false)]
		[Description("Sets the Internet Information Services Directory Browsing property for the selected folder")]
		public bool AllowDirectoryBrowsing
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Sets the Internet Information Services Read property for the selected folder")]
		public bool AllowReadAccess
		{
			get
			{
				if (this._wixWebDirProperties == null)
				{
					return false;
				}
				return this._wixWebDirProperties.Read == "yes";
			}
			set
			{
				if (this._wixWebDirProperties != null)
				{
					this._wixWebDirProperties.Read = (value ? "yes" : "no");
				}
			}
		}

		[Browsable(false)]
		[DefaultValue(false)]
		[Description("Sets the Internet Information Services Script Source Access property for the selected folder")]
		public bool AllowScriptSourceAccess
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Sets the Internet Information Services Write property for the selected folder")]
		public bool AllowWriteAccess
		{
			get
			{
				if (this._wixWebDirProperties == null)
				{
					return false;
				}
				return this._wixWebDirProperties.Write == "yes";
			}
			set
			{
				if (this._wixWebDirProperties != null)
				{
					this._wixWebDirProperties.Write = (value ? "yes" : "no");
				}
			}
		}

		[Browsable(false)]
		[DefaultValue(false)]
		public override bool AlwaysCreate
		{
			get
			{
				return base.AlwaysCreate;
			}
			set
			{
				base.AlwaysCreate = value;
			}
		}

		[Browsable(false)]
		public override string DefaultLocation
		{
			get
			{
				return base.DefaultLocation;
			}
			set
			{
				base.DefaultLocation = value;
			}
		}

		[Browsable(true)]
		[DefaultValue(DirectoryExecutePermissions.vsdepScriptsOnly)]
		[Description("Sets the Internet Information Services Execute Permissions property for the selected folder")]
		public DirectoryExecutePermissions ExecutePermissions
		{
			get
			{
				if (this._wixWebDirProperties != null)
				{
					if (this._wixWebDirProperties.Script == "no" && this._wixWebDirProperties.Execute == "no")
					{
						return DirectoryExecutePermissions.vsdepNone;
					}
					if (this._wixWebDirProperties.Script == "yes" && this._wixWebDirProperties.Execute == "yes")
					{
						return DirectoryExecutePermissions.vsdepScriptsAndExecutables;
					}
				}
				return DirectoryExecutePermissions.vsdepScriptsOnly;
			}
			set
			{
				if (this._wixWebDirProperties != null)
				{
					switch (value)
					{
						case DirectoryExecutePermissions.vsdepNone:
						{
							this._wixWebDirProperties.Script = "no";
							this._wixWebDirProperties.Execute = "no";
							return;
						}
						case DirectoryExecutePermissions.vsdepScriptsOnly:
						{
							this._wixWebDirProperties.Script = "yes";
							this._wixWebDirProperties.Execute = "no";
							return;
						}
						case DirectoryExecutePermissions.vsdepScriptsAndExecutables:
						{
							this._wixWebDirProperties.Script = "yes";
							this._wixWebDirProperties.Execute = "yes";
							break;
						}
						default:
						{
							return;
						}
					}
				}
			}
		}

		[Browsable(true)]
		[DefaultValue(true)]
		[Description("Sets the Internet Information Services Index This Resource property for the selected folder")]
		public bool Index
		{
			get
			{
				if (this._wixWebDirProperties == null)
				{
					return false;
				}
				return this._wixWebDirProperties.Index == "yes";
			}
			set
			{
				if (this._wixWebDirProperties != null)
				{
					this._wixWebDirProperties.Index = (value ? "yes" : "no");
				}
			}
		}

		[Browsable(true)]
		[DefaultValue(true)]
		[Description("Sets the Internet Information Services Log Visits property for the selected folder")]
		public bool LogVisits
		{
			get
			{
				if (this._wixWebDirProperties == null)
				{
					return false;
				}
				return this._wixWebDirProperties.LogVisits == "yes";
			}
			set
			{
				if (this._wixWebDirProperties != null)
				{
					this._wixWebDirProperties.LogVisits = (value ? "yes" : "no");
				}
			}
		}

		[Browsable(true)]
		[DisplayName("(Name)")]
		[ReadOnly(false)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (this.WiXWebDir != null)
					{
						this.WiXWebDir.Path = value;
					}
					if (this.WiXWebVirtualDir != null)
					{
						this.WiXWebVirtualDir.Alias = value;
					}
					if (this.WiXWebApplication != null)
					{
						this.WiXWebApplication.Name = value;
					}
					base.Name = value;
				}
			}
		}

		internal IISWebApplication WiXWebApplication
		{
			get
			{
				return this._wixWebApplication;
			}
			set
			{
				this._wixWebApplication = value;
			}
		}

		internal IISWebDir WiXWebDir
		{
			get
			{
				return this._wixWebDir;
			}
			set
			{
				this._wixWebDir = value;
			}
		}

		internal IISWebDirProperties WiXWebDirProperties
		{
			get
			{
				return this._wixWebDirProperties;
			}
			set
			{
				this._wixWebDirProperties = value;
			}
		}

		internal IISWebVirtualDir WiXWebVirtualDir
		{
			get
			{
				return this._wixWebVirtualDir;
			}
			set
			{
				this._wixWebVirtualDir = value;
			}
		}

		public VSWebFolder(WiXProjectParser project, VSBaseFolder parent, WiXDirectory wixElement) : base(project, parent, wixElement)
		{
			VSWebFolder vSWebFolder = this;
			this._wixWebVirtualDir = project.SupportedEntities.Find((WiXEntity e) => {
				if (!(e is IISWebVirtualDir))
				{
					return false;
				}
				return (e as IISWebVirtualDir).Directory == wixElement.Id;
			}) as IISWebVirtualDir;
			if (this._wixWebVirtualDir == null)
			{
				List<WiXComponent> wiXComponents = wixElement.ChildEntities.FindAll((WiXEntity e) => e is WiXComponent).ConvertAll<WiXComponent>((WiXEntity e) => e as WiXComponent);
				if (wiXComponents != null)
				{
					foreach (WiXComponent wiXComponent in wiXComponents)
					{
						this._wixWebDir = Common.FindChild(wiXComponent, "WebDir", false) as IISWebDir;
						if (this._wixWebDir == null)
						{
							continue;
						}
						goto Label0;
					}
				}
			}
		Label0:
			if (this._wixWebVirtualDir != null)
			{
				project.SupportedEntities.Remove(this._wixWebVirtualDir);
				if (!string.IsNullOrEmpty(this._wixWebVirtualDir.DirProperties))
				{
					this._wixWebDirProperties = project.SupportedEntities.Find((WiXEntity e) => {
						if (!(e is IISWebDirProperties))
						{
							return false;
						}
						return (e as IISWebDirProperties).Id == vSWebFolder._wixWebVirtualDir.DirProperties;
					}) as IISWebDirProperties;
				}
				else
				{
					this._wixWebDirProperties = Common.FindChild(this._wixWebVirtualDir, "WebDirProperties", false) as IISWebDirProperties;
				}
				if (!string.IsNullOrEmpty(this._wixWebVirtualDir.WebApplication))
				{
					this._wixWebApplication = project.SupportedEntities.Find((WiXEntity e) => {
						if (!(e is IISWebApplication))
						{
							return false;
						}
						return (e as IISWebApplication).Id == vSWebFolder._wixWebVirtualDir.WebApplication;
					}) as IISWebApplication;
				}
				else
				{
					this._wixWebApplication = Common.FindChild(this._wixWebVirtualDir, "WebApplication", false) as IISWebApplication;
				}
			}
			if (this._wixWebDir != null)
			{
				project.SupportedEntities.Remove(this._wixWebDir);
				if (string.IsNullOrEmpty(this._wixWebDir.DirProperties))
				{
					this._wixWebDirProperties = Common.FindChild(this._wixWebDir, "WebDirProperties", false) as IISWebDirProperties;
					return;
				}
				this._wixWebDirProperties = project.SupportedEntities.Find((WiXEntity e) => {
					if (!(e is IISWebDirProperties))
					{
						return false;
					}
					return (e as IISWebDirProperties).Id == vSWebFolder._wixWebDir.DirProperties;
				}) as IISWebDirProperties;
			}
		}

		public override void Delete()
		{
			if (this._wixWebVirtualDir != null)
			{
				this._wixWebVirtualDir.Delete();
				this._wixWebVirtualDir = null;
			}
			base.Delete();
		}
	}
}