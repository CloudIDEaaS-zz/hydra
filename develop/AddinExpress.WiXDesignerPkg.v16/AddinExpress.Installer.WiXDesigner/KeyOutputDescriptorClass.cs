using System;
using System.ComponentModel;
using System.IO;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class KeyOutputDescriptorClass
	{
		private ProjectDescriptor.KeyOutputDescriptor _descriptor;

		[Description("Indicates the name for an assembly that can be used in the run-time user interface to identify the assembly")]
		public string DisplayName
		{
			get
			{
				return this._descriptor.DisplayName;
			}
		}

		[Description("Indicates the hash algorithm used by the assembly manifest")]
		public string HashAlgorithm
		{
			get
			{
				return this._descriptor.HashAlgorithm;
			}
		}

		[Description("Indicates the language for a localized assembly")]
		public string Language
		{
			get
			{
				return this._descriptor.Language;
			}
		}

		[Description("Indicates the name for an assembly")]
		[DisplayName("(Name)")]
		public string Name
		{
			get
			{
				return this._descriptor.Name;
			}
		}

		[Description("Indicates the cryptographic public key for the assembly")]
		public string PublicKey
		{
			get
			{
				return this._descriptor.PublicKey;
			}
		}

		[Description("Indicates the public part of the originator of the key pair for the assembly")]
		public string PublicKeyToken
		{
			get
			{
				return this._descriptor.PublicKeyToken;
			}
		}

		[Description("Indicates the path to a selected assembly on the development computer")]
		public string SourcePath
		{
			get
			{
				return this._descriptor.SourcePath;
			}
		}

		[Description("Indicates the name for an assembly when it is installed on a target computer")]
		public string TargetName
		{
			get
			{
				if (!string.IsNullOrEmpty(this._descriptor.TargetName))
				{
					return this._descriptor.TargetName;
				}
				return Path.GetFileName(this._descriptor.SourcePath);
			}
		}

		[Description("Indicates the version number of an assembly")]
		public string Version
		{
			get
			{
				return this._descriptor.Version;
			}
		}

		internal KeyOutputDescriptorClass(ProjectDescriptor.KeyOutputDescriptor descritor)
		{
			this._descriptor = descritor;
		}
	}
}