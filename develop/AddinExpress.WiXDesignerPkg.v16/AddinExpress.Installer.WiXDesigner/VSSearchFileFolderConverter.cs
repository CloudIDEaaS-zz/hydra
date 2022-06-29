using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSSearchFileFolderConverter : CollectionConverter
	{
		public VSSearchFileFolderConverter()
		{
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return true;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return true;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				return value;
			}
			return null;
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is string)
			{
				return value;
			}
			return null;
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			ArrayList arrayLists = new ArrayList();
			arrayLists.Add("[CommonFilesFolder]");
			arrayLists.Add("[CommonFiles64Folder]");
			arrayLists.Add("[FontsFolder]");
			arrayLists.Add("[ProgramFilesFolder]");
			arrayLists.Add("[ProgramFiles64Folder]");
			arrayLists.Add("[SystemFolder]");
			arrayLists.Add("[System64Folder]");
			arrayLists.Add("[WindowsFolder]");
			arrayLists.Add("[TARGETDIR]");
			return new TypeConverter.StandardValuesCollection(arrayLists);
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			if (context.Instance != null)
			{
				return true;
			}
			return false;
		}
	}
}