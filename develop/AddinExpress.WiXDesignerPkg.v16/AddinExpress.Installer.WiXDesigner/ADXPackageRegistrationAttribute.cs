using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AddinExpress.Installer.WiXDesigner
{
	[AttributeUsage(AttributeTargets.Class, Inherited=true)]
	[ComVisible(false)]
	public sealed class ADXPackageRegistrationAttribute : RegistrationAttribute
	{
		private RegistrationMethod registrationMethod;

		private string satellitePath;

		private bool useManagedResources;

		public RegistrationMethod RegisterUsing
		{
			get
			{
				return this.registrationMethod;
			}
			set
			{
				this.registrationMethod = value;
			}
		}

		public string SatellitePath
		{
			get
			{
				return this.satellitePath;
			}
			set
			{
				this.satellitePath = value;
			}
		}

		public bool UseManagedResourcesOnly
		{
			get
			{
				return this.useManagedResources;
			}
			set
			{
				this.useManagedResources = value;
			}
		}

		public ADXPackageRegistrationAttribute()
		{
		}

		public override void Register(RegistrationAttribute.RegistrationContext context)
		{
			string str;
			Type componentType = context.ComponentType;
			TextWriter log = context.Log;
			CultureInfo culture = Resources.Culture;
			string regNotifyPackage = Resources.Reg_NotifyPackage;
			object[] name = new object[] { componentType.Name, null };
			Guid gUID = componentType.GUID;
			name[1] = gUID.ToString("B").ToUpper();
			log.WriteLine(string.Format(culture, regNotifyPackage, name));
			RegistrationAttribute.Key key = null;
			try
			{
				key = context.CreateKey(this.RegKeyName(context));
				DescriptionAttribute item = TypeDescriptor.GetAttributes(componentType)[typeof(DescriptionAttribute)] as DescriptionAttribute;
				if (item == null || string.IsNullOrEmpty(item.Description))
				{
					key.SetValue(string.Empty, componentType.AssemblyQualifiedName);
				}
				else
				{
					key.SetValue(string.Empty, item.Description);
				}
				key.SetValue("InprocServer32", context.InprocServerPath);
				key.SetValue("Class", componentType.FullName);
				if (context.RegistrationMethod != RegistrationMethod.Default)
				{
					this.registrationMethod = context.RegistrationMethod;
				}
				switch (this.registrationMethod)
				{
					case RegistrationMethod.Default:
					case RegistrationMethod.Assembly:
					{
						key.SetValue("Assembly", componentType.Assembly.FullName);
						break;
					}
					case RegistrationMethod.CodeBase:
					{
						key.SetValue("CodeBase", context.CodeBase);
						break;
					}
				}
				RegistrationAttribute.Key key1 = null;
				if (!this.useManagedResources)
				{
					try
					{
						key1 = key.CreateSubkey("SatelliteDll");
						str = (this.SatellitePath == null ? context.ComponentPath : context.EscapePath(this.SatellitePath));
						key1.SetValue("Path", str);
						key1.SetValue("DllName", string.Format(CultureInfo.InvariantCulture, "{0}UI.dll", new object[] { Path.GetFileNameWithoutExtension(componentType.Assembly.ManifestModule.Name) }));
					}
					finally
					{
						if (key1 != null)
						{
							key1.Close();
						}
					}
				}
			}
			finally
			{
				if (key != null)
				{
					key.Close();
				}
			}
		}

		private string RegKeyName(RegistrationAttribute.RegistrationContext context)
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			object[] upper = new object[1];
			Guid gUID = context.ComponentType.GUID;
			upper[0] = gUID.ToString("B").ToUpper();
			return string.Format(invariantCulture, "Packages\\{0}", upper);
		}

		public override void Unregister(RegistrationAttribute.RegistrationContext context)
		{
			context.RemoveKey(this.RegKeyName(context));
		}
	}
}