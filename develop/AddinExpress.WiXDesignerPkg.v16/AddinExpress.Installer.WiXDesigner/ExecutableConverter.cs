using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class ExecutableConverter : TypeConverter, IWin32Window
	{
		private TypeConverter.StandardValuesCollection standardValueCollection;

		public IntPtr Handle
		{
			get
			{
				return VsPackage.CurrentInstance.Handle;
			}
		}

		public ExecutableConverter()
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
			VSBaseFile vSBaseFile = value as VSBaseFile;
			if (vSBaseFile == null && context != null)
			{
				vSBaseFile = context.PropertyDescriptor.GetValue(context.Instance) as VSBaseFile;
			}
			if (vSBaseFile != null)
			{
				if (vSBaseFile is VSBinary)
				{
					return Path.GetFileName((vSBaseFile as VSBinary).SourcePath);
				}
				if (vSBaseFile is VSFile)
				{
					return Path.GetFileName((vSBaseFile as VSFile).SourcePath);
				}
			}
			return "(None)";
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (this.standardValueCollection != null)
			{
				TypeConverter.StandardValuesCollection standardValuesCollections = new TypeConverter.StandardValuesCollection(this.standardValueCollection);
				this.standardValueCollection = null;
				return standardValuesCollections;
			}
			VSBaseFile value = context.PropertyDescriptor.GetValue(context.Instance) as VSBaseFile;
			if (value is VSFile || value is VSBinary)
			{
				return new TypeConverter.StandardValuesCollection(new object[0]);
			}
			object obj = this.ShowDialog((VSDialogBase)context.Instance, null, string.Empty, null);
			if (obj == null)
			{
				this.standardValueCollection = new TypeConverter.StandardValuesCollection(new object[0]);
			}
			else
			{
				this.standardValueCollection = new TypeConverter.StandardValuesCollection(new object[] { VSBaseFile.Empty, obj });
			}
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

		private object ShowDialog(VSDialogBase vsDialog, VSBaseFolder parent, string fileName, object defaultValue)
		{
			object selectedFile;
			FormSelectItemInProject formSelectItemInProject = new FormSelectItemInProject();
			try
			{
				formSelectItemInProject.Initialize(parent, fileName, vsDialog.Project.FileSystem, vsDialog.Project.Binaries, new string[] { "Executable Files (*.exe)", "All Files (*.*)" });
				if (formSelectItemInProject.ShowDialog() == DialogResult.OK)
				{
					string empty = string.Empty;
					if (formSelectItemInProject.SelectedFile is VSFile)
					{
						empty = (formSelectItemInProject.SelectedFile as VSFile).SourcePath;
					}
					else if (formSelectItemInProject.SelectedFile is VSBinary)
					{
						empty = (formSelectItemInProject.SelectedFile as VSBinary).SourcePath;
					}
					if (string.IsNullOrEmpty(empty) || !empty.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
					{
						MessageBox.Show(this, "Not a valid file type for this property. Only .exe files are supported.", Common.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
					else
					{
						selectedFile = formSelectItemInProject.SelectedFile;
						return selectedFile;
					}
				}
				return defaultValue;
			}
			finally
			{
				formSelectItemInProject.Dispose();
			}
			return selectedFile;
		}
	}
}