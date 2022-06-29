using Microsoft.VisualStudio.Shell;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace AddinExpress.Installer.WiXDesigner
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
	[ComVisible(false)]
	public sealed class ADXProvideAutoLoadAttribute : RegistrationAttribute
	{
		private Guid loadGuid = Guid.Empty;

		public Guid LoadGuid
		{
			get
			{
				return this.loadGuid;
			}
		}

		private string RegKeyName
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "AutoLoadPackages\\{0}", new object[] { this.loadGuid.ToString("B") });
			}
		}

		public ADXProvideAutoLoadAttribute(string cmdUiContextGuid)
		{
			this.loadGuid = new Guid(cmdUiContextGuid);
		}

		public override void Register(RegistrationAttribute.RegistrationContext context)
		{
			context.Log.WriteLine(string.Format(Resources.Culture, Resources.Reg_NotifyAutoLoad, new object[] { this.loadGuid.ToString("B").ToUpper() }));
			using (RegistrationAttribute.Key key = context.CreateKey(this.RegKeyName))
			{
				Guid gUID = context.ComponentType.GUID;
				key.SetValue(gUID.ToString("B").ToUpper(), 0);
			}
		}

		public override void Unregister(RegistrationAttribute.RegistrationContext context)
		{
			string regKeyName = this.RegKeyName;
			Guid gUID = context.ComponentType.GUID;
			context.RemoveValue(regKeyName, gUID.ToString("B").ToUpper());
		}
	}
}