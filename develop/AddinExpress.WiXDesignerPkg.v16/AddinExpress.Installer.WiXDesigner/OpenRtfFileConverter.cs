using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class OpenRtfFileConverter : TypeConverter, IWin32Window
	{
		private TypeConverter.StandardValuesCollection standardValueCollection;

		public IntPtr Handle
		{
			get
			{
				return VsPackage.CurrentInstance.Handle;
			}
		}

		public OpenRtfFileConverter()
		{
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type sourceType)
		{
			return true;
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType != typeof(string))
			{
				return base.ConvertTo(context, culture, value, destinationType);
			}
			string str = (string)value;
			if (string.IsNullOrEmpty(str))
			{
				return "(None)";
			}
			return str;
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (this.standardValueCollection != null)
			{
				TypeConverter.StandardValuesCollection standardValuesCollections = new TypeConverter.StandardValuesCollection(this.standardValueCollection);
				this.standardValueCollection = null;
				return standardValuesCollections;
			}
			string value = (string)context.PropertyDescriptor.GetValue(context.Instance);
			if (!string.IsNullOrEmpty(value))
			{
				return new TypeConverter.StandardValuesCollection(new object[0]);
			}
			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				DefaultExt = "rtf",
				Filter = "Rich Text Format Files|*.rtf|All Files|*.*",
				Multiselect = false
			};
			if (context.Instance is VSEulaDialog)
			{
				openFileDialog.Title = "Select License File";
			}
			else if (context.Instance is VSReadmeDialog)
			{
				openFileDialog.Title = "Select Readme File";
			}
			if (!string.IsNullOrEmpty(value))
			{
				try
				{
					openFileDialog.InitialDirectory = Path.GetDirectoryName(value);
					openFileDialog.FileName = Path.GetFileName(value);
				}
				catch (Exception exception)
				{
				}
			}
			if (openFileDialog.ShowDialog(this) != DialogResult.OK)
			{
				return new TypeConverter.StandardValuesCollection(new object[0]);
			}
			if (!openFileDialog.FileName.EndsWith(".rtf", StringComparison.OrdinalIgnoreCase))
			{
				MessageBox.Show(this, "Not a valid file type for this property. Only .rtf files are supported.", Common.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return new TypeConverter.StandardValuesCollection(new object[0]);
			}
			this.standardValueCollection = new TypeConverter.StandardValuesCollection(new object[] { string.Empty, openFileDialog.FileName });
			return this.standardValueCollection;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}