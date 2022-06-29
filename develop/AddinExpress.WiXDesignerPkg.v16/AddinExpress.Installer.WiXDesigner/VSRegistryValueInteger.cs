using System;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSRegistryValueInteger : VSRegistryValueBase
	{
		[Browsable(true)]
		[Description("Specifies the data stored in the selected registry value")]
		[TypeConverter(typeof(Int32Converter))]
		public override object Value
		{
			get
			{
				if (base.WiXElement == null)
				{
					return 0;
				}
				return base.WiXElement.Value;
			}
			set
			{
				if (value != null && value is int)
				{
					if (base.WiXElement != null)
					{
						base.WiXElement.Value = Convert.ToString((int)value);
					}
					this.DoPropertyChanged();
				}
			}
		}

		public VSRegistryValueInteger()
		{
		}

		public VSRegistryValueInteger(WiXProjectParser project, WiXRegistryValue wixElement, VSComponent parentComponent, VSRegistryValues collection) : base(project, wixElement, parentComponent, collection)
		{
		}

		protected override VSRegistryValueType GetValueType()
		{
			return VSRegistryValueType.vsdrvtInteger;
		}

		protected override string GetWiXValueType()
		{
			return "integer";
		}
	}
}