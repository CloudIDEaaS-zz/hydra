using System;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSWebApplicationExtension : VSComponentBase
	{
		internal string CheckPath
		{
			get;
			set;
		}

		internal string Executable
		{
			get;
			set;
		}

		internal string Extension
		{
			get;
			set;
		}

		internal string Script
		{
			get;
			set;
		}

		internal string Verbs
		{
			get;
			set;
		}

		internal VSWebApplicationExtension(IISWebApplicationExtension wixElement)
		{
			this.CheckPath = wixElement.CheckPath;
			this.Executable = wixElement.Executable;
			this.Extension = wixElement.Extension;
			this.Script = wixElement.Script;
			this.Verbs = wixElement.Verbs;
		}

		internal VSWebApplicationExtension(string executable, string extension, string verbs)
		{
			this.CheckPath = string.Empty;
			this.Executable = executable;
			this.Extension = extension;
			this.Script = string.Empty;
			this.Verbs = verbs;
		}
	}
}