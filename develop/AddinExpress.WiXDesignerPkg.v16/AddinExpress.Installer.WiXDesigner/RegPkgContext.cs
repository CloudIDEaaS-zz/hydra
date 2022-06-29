using Microsoft.VisualStudio.Shell;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace AddinExpress.Installer.WiXDesigner
{
	internal sealed class RegPkgContext : RegistrationAttribute.RegistrationContext, IDisposable
	{
		private Hive hive;

		private bool disposed;

		private Type componentType;

		private string componentPath = string.Empty;

		private string inprocServerPath = string.Empty;

		private string codeBase = string.Empty;

		private Microsoft.VisualStudio.Shell.RegistrationMethod registrationMethod;

		private TextWriter log;

		private string rootFolder = string.Empty;

		public override string CodeBase
		{
			get
			{
				if (this.codeBase == string.Empty && this.componentType != null)
				{
					Uri uri = new Uri(this.componentType.Assembly.CodeBase);
					this.codeBase = this.EscapePath(uri.LocalPath);
				}
				return this.codeBase;
			}
		}

		public override string ComponentPath
		{
			get
			{
				if (this.componentPath == string.Empty && this.componentType != null)
				{
					string codeBase = this.componentType.Assembly.CodeBase;
					codeBase = (new Uri(codeBase)).LocalPath;
					this.componentPath = this.EscapePath(Path.GetDirectoryName(codeBase));
				}
				return this.componentPath;
			}
		}

		public override Type ComponentType
		{
			get
			{
				return this.componentType;
			}
		}

		public override string InprocServerPath
		{
			get
			{
				if (this.inprocServerPath == string.Empty)
				{
					this.inprocServerPath = this.EscapePath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "mscoree.dll"));
				}
				return this.inprocServerPath;
			}
		}

		public override TextWriter Log
		{
			get
			{
				if (this.log == null)
				{
					this.log = new StringWriter(CultureInfo.CurrentUICulture);
				}
				return this.log;
			}
		}

		public override Microsoft.VisualStudio.Shell.RegistrationMethod RegistrationMethod
		{
			get
			{
				return this.registrationMethod;
			}
		}

		public override string RootFolder
		{
			get
			{
				return this.rootFolder;
			}
		}

		internal RegPkgContext(Type t, Hive hive, Microsoft.VisualStudio.Shell.RegistrationMethod registraterUsing)
		{
			this.hive = hive;
			this.registrationMethod = registraterUsing;
			this.componentType = t;
		}

		public override RegistrationAttribute.Key CreateKey(string name)
		{
			return this.hive.CreateKey(name);
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.disposed = true;
				if (this.log != null)
				{
					this.log.Dispose();
					this.log = null;
				}
			}
		}

		public override string EscapePath(string path)
		{
			return path;
		}

		public override void RemoveKey(string name)
		{
			this.hive.RemoveKey(name);
		}

		public override void RemoveKeyIfEmpty(string name)
		{
		}

		public override void RemoveValue(string name, string valuename)
		{
			this.hive.RemoveValue(name, valuename);
		}
	}
}