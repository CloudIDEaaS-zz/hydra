using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class BannerBitmapConverter : TypeConverter, IWin32Window
	{
		private TypeConverter.StandardValuesCollection standardValueCollection;

		public IntPtr Handle
		{
			get
			{
				return VsPackage.CurrentInstance.Handle;
			}
		}

		public BannerBitmapConverter()
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
					if ((vSBaseFile as VSBinary).WiXElement.Id == "DefBannerBitmap")
					{
						return "(Default)";
					}
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
			object obj = null;
			object empty = VSBaseFile.Empty;
			VSDialogBase instance = (VSDialogBase)context.Instance;
			List<VSBinary> binaries = instance.Project.Binaries;
			if (binaries != null && binaries.Count > 0)
			{
				empty = binaries.Find((VSBinary b) => b.WiXElement.Id == "DefBannerBitmap");
			}
			VSBaseFile value = context.PropertyDescriptor.GetValue(context.Instance) as VSBaseFile;
			if (value is VSBinary)
			{
				if ((value as VSBinary).WiXElement.Id != "DefBannerBitmap")
				{
					return new TypeConverter.StandardValuesCollection(new object[0]);
				}
				obj = this.ShowDialog(instance, null, string.Empty, null);
				if (obj == null)
				{
					return new TypeConverter.StandardValuesCollection(new object[0]);
				}
			}
			else if (value is VSFile)
			{
				return new TypeConverter.StandardValuesCollection(new object[0]);
			}
			if (obj == null)
			{
				this.standardValueCollection = new TypeConverter.StandardValuesCollection(new object[] { VSBaseFile.Empty, empty });
			}
			else
			{
				this.standardValueCollection = new TypeConverter.StandardValuesCollection(new object[] { VSBaseFile.Empty, empty, obj });
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
				formSelectItemInProject.Initialize(parent, fileName, vsDialog.Project.FileSystem, vsDialog.Project.Binaries, new string[] { "Image Files (*.bmp;*.jpg)", "All Files (*.*)" });
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
					if (string.IsNullOrEmpty(empty) || !empty.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) && !empty.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase))
					{
						MessageBox.Show(this, "Not a valid file type for an image.", Common.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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