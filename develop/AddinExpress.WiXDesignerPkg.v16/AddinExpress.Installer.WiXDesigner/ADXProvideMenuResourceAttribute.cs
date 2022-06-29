using Microsoft.VisualStudio.Shell;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace AddinExpress.Installer.WiXDesigner
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
	[ComVisible(false)]
	public sealed class ADXProvideMenuResourceAttribute : RegistrationAttribute
	{
		private string _resourceID;

		private int _version;

		public string ResourceID
		{
			get
			{
				return this._resourceID;
			}
		}

		public int Version
		{
			get
			{
				return this._version;
			}
		}

		public ADXProvideMenuResourceAttribute(short resourceID, int version)
		{
			this._resourceID = resourceID.ToString();
			this._version = version;
		}

		public ADXProvideMenuResourceAttribute(string resourceID, int version)
		{
			if (string.IsNullOrEmpty(resourceID))
			{
				throw new ArgumentNullException("resourceID");
			}
			this._resourceID = resourceID;
			this._version = version;
		}

		public override void Register(RegistrationAttribute.RegistrationContext context)
		{
			context.Log.WriteLine(string.Format(Resources.Culture, Resources.Reg_NotifyMenuResource, new object[] { this.ResourceID, this.Version }));
			using (RegistrationAttribute.Key key = context.CreateKey("Menus"))
			{
				Guid gUID = context.ComponentType.GUID;
				key.SetValue(gUID.ToString("B").ToUpper(), string.Format(CultureInfo.InvariantCulture, ", {0}, {1}", new object[] { this.ResourceID, this.Version }));
			}
		}

		public override void Unregister(RegistrationAttribute.RegistrationContext context)
		{
			Guid gUID = context.ComponentType.GUID;
			context.RemoveValue("Menus", gUID.ToString("B").ToUpper());
		}
	}
}