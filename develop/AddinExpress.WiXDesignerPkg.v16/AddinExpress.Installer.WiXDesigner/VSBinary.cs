using EnvDTE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSBinary : VSBaseFile
	{
		private WiXBinary _wixElement;

		internal override bool CanRename
		{
			get
			{
				return false;
			}
		}

		[Browsable(false)]
		[DisplayName("(Name)")]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
			}
		}

		[Browsable(true)]
		public override string SourcePath
		{
			get
			{
				if (this.WiXElement == null)
				{
					return string.Empty;
				}
				return this.WiXElement.SourceFile;
			}
			set
			{
				if (this.WiXElement != null)
				{
					this.WiXElement.SourceFile = value;
				}
			}
		}

		[Browsable(true)]
		[ReadOnly(true)]
		public override string TargetName
		{
			get
			{
				if (string.IsNullOrEmpty(this.WiXElement.Name))
				{
					return Path.GetFileName(this.SourcePath);
				}
				return this.WiXElement.Name;
			}
		}

		internal WiXBinary WiXElement
		{
			get
			{
				return this._wixElement;
			}
		}

		public VSBinary()
		{
		}

		public VSBinary(WiXProjectParser project, WiXBinary wixElement) : base(project, (VSComponent)null)
		{
			this._wixElement = wixElement;
		}

		public override void Delete()
		{
			if (base.ParentComponent != null)
			{
				base.ParentComponent.Files.Remove(this);
			}
			this.WiXElement.Delete();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._wixElement = null;
			}
			base.Dispose(disposing);
		}

		protected override string GetComponentName()
		{
			return this.TargetName;
		}

		internal string GetFullPath()
		{
			string empty = string.Empty;
			if (this.WiXElement != null)
			{
				empty = this.WiXElement.SourceFile;
				if (!Path.IsPathRooted(empty) && base.Project.ProjectManager.VsProject != null)
				{
					string directoryName = Path.GetDirectoryName(base.Project.ProjectManager.VsProject.FullName);
					empty = Path.GetFullPath(Path.Combine(CommonUtilities.AbsolutePathFromRelative(Path.GetDirectoryName(empty), directoryName), Path.GetFileName(empty)));
				}
			}
			return empty;
		}
	}
}