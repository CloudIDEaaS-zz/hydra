using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Assemblies;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSAssembly : VSFile
	{
		private AssemblyName _assemblyName;

		private List<string> _refAssemblies = new List<string>();

		private List<string> _files = new List<string>();

		internal override bool CanRename
		{
			get
			{
				return false;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Displays the Dependencies dialog box that contains a list of dependent files")]
		[Editor(typeof(DependenciesPropertyEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(DependenciesPropertyConverter))]
		public object Dependencies
		{
			get
			{
				return this._refAssemblies;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Indicates the name for an assembly that can be used in the run-time user interface to identify the assembly")]
		[ReadOnly(true)]
		public string DisplayName
		{
			get
			{
				return base.TargetName;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Displays the Files dialog box that contains a list of files in the assembly")]
		[Editor(typeof(FilesPropertyEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(FilesPropertyConverter))]
		public object Files
		{
			get
			{
				return this._files;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Indicates the hash algorithm used by the assembly manifest")]
		[ReadOnly(true)]
		public string HashAlgorithm
		{
			get
			{
				if (this._assemblyName == null)
				{
					return string.Empty;
				}
				return this._assemblyName.HashAlgorithm.ToString();
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Indicates the language for a localized assembly")]
		[ReadOnly(true)]
		public string Language
		{
			get
			{
				if (this._assemblyName == null)
				{
					return string.Empty;
				}
				return this._assemblyName.CultureInfo.ToString();
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Indicates the name for an assembly")]
		[DisplayName("(Name)")]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				if (string.IsNullOrEmpty(this.DisplayName))
				{
					return base.Name;
				}
				return Path.GetFileName(this.DisplayName);
			}
			set
			{
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Indicates the cryptographic public key for the assembly")]
		[ReadOnly(true)]
		public string PublicKey
		{
			get
			{
				if (this._assemblyName != null)
				{
					byte[] publicKey = this._assemblyName.GetPublicKey();
					if (publicKey != null)
					{
						return BitConverter.ToString(publicKey).Replace("-", "");
					}
				}
				return string.Empty;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Indicates the public part of the originator of the pey pair for the assembly")]
		[ReadOnly(true)]
		public string PublicKeyToken
		{
			get
			{
				if (this._assemblyName != null)
				{
					byte[] publicKeyToken = this._assemblyName.GetPublicKeyToken();
					if (publicKeyToken != null)
					{
						return BitConverter.ToString(publicKeyToken).Replace("-", "");
					}
				}
				return string.Empty;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the name for an assembly when it is installed on a target computer")]
		[ReadOnly(true)]
		public override string TargetName
		{
			get
			{
				return base.TargetName;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Indicates the version number of an assembly")]
		[ReadOnly(true)]
		public string Version
		{
			get
			{
				if (this._assemblyName == null)
				{
					return string.Empty;
				}
				return this._assemblyName.Version.ToString();
			}
		}

		public VSAssembly(WiXProjectParser project, VSComponent parent, WiXFile wixElement) : base(project, parent, wixElement)
		{
			if (File.Exists(this.SourcePath))
			{
				try
				{
					this._assemblyName = AssemblyName.GetAssemblyName(this.SourcePath);
				}
				catch
				{
				}
			}
		}

		public VSAssembly(VSBaseFolder folder, string filePath, VSComponent component) : base(folder, filePath, component)
		{
			if (File.Exists(this.SourcePath))
			{
				try
				{
					this._assemblyName = AssemblyName.GetAssemblyName(this.SourcePath);
				}
				catch
				{
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._assemblyName = null;
			}
			base.Dispose(disposing);
		}
	}
}