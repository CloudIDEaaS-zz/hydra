using Microsoft.VisualStudio.Shell;
using System;
using System.Globalization;
using System.IO;

namespace AddinExpress.Installer.WiXDesigner
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
	internal sealed class ADXProvideSolutionProps : RegistrationAttribute
	{
		private string _propName;

		public string PropName
		{
			get
			{
				return this._propName;
			}
		}

		public ADXProvideSolutionProps(string propName)
		{
			this._propName = propName;
		}

		public override void Register(RegistrationAttribute.RegistrationContext context)
		{
			TextWriter log = context.Log;
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			Guid gUID = context.ComponentType.GUID;
			log.WriteLine(string.Format(invariantCulture, "ProvideSolutionProps: ({0} = {1})", gUID.ToString("B").ToUpper(), this.PropName));
			RegistrationAttribute.Key key = null;
			try
			{
				key = context.CreateKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", "SolutionPersistence", this.PropName));
				string empty = string.Empty;
				gUID = context.ComponentType.GUID;
				key.SetValue(empty, gUID.ToString("B").ToUpper());
			}
			finally
			{
				if (key != null)
				{
					key.Close();
				}
			}
		}

		public override void Unregister(RegistrationAttribute.RegistrationContext context)
		{
			context.RemoveKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", "SolutionPersistence", this.PropName));
		}
	}
}