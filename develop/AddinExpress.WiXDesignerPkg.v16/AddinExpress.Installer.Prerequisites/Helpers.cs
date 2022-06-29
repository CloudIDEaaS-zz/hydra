using AddinExpress.Installer.Prerequisites.Manifests;
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AddinExpress.Installer.Prerequisites
{
	internal sealed class Helpers
	{
		public const string NeutralText = "neutral";

		public const string AllFilesFilter = "All Files(*.*)|*.*";

		internal static ArrayList BuiltProperties;

		private static bool g_CommandLineBuild;

		private static Helpers.LocStringCollection m_locstrings;

		private static bool DPIDetected;

		public static float DPI;

		public static bool IsCommandLineBuild
		{
			get
			{
				return Helpers.g_CommandLineBuild;
			}
			set
			{
				Helpers.g_CommandLineBuild = value;
			}
		}

		static Helpers()
		{
			Helpers.BuiltProperties = new ArrayList();
			Helpers.g_CommandLineBuild = false;
			Helpers.m_locstrings = new Helpers.LocStringCollection();
			Helpers.DPIDetected = false;
			Helpers.DPI = 96f;
		}

		public Helpers()
		{
		}

		public static string AbsolutePathFromRelative(string relativePath, string baseFolderForDerelativization)
		{
			int length = 0;
			if (relativePath == null)
			{
				throw new ArgumentNullException("relativePath");
			}
			if (baseFolderForDerelativization == null)
			{
				throw new ArgumentNullException("baseFolderForDerelativization");
			}
			if (Path.IsPathRooted(relativePath))
			{
				throw new ArgumentException("The path is not relative.", "relativePath");
			}
			if (!Path.IsPathRooted(baseFolderForDerelativization))
			{
				throw new ArgumentException("The base folder must be a rooted path.", "baseFolderForDerelativization");
			}
			StringBuilder stringBuilder = new StringBuilder(baseFolderForDerelativization);
			if (stringBuilder[stringBuilder.Length - 1] != Path.DirectorySeparatorChar)
			{
				stringBuilder.Append(Path.DirectorySeparatorChar);
			}
			for (int i = 0; i < relativePath.Length; i = length + 1)
			{
				length = relativePath.IndexOf(Path.DirectorySeparatorChar, i);
				if (length == -1)
				{
					length = relativePath.Length;
				}
				string str = relativePath.Substring(i, length - i);
				if (str == "..")
				{
					int num = stringBuilder.ToString().LastIndexOf(Path.DirectorySeparatorChar, stringBuilder.Length - 2);
					if (num == -1)
					{
						throw new ArgumentException("Too many '..' specified in relative path.");
					}
					stringBuilder.Remove(num, stringBuilder.Length - num);
				}
				else if (str != ".")
				{
					stringBuilder.Append(str);
				}
				if (length < relativePath.Length)
				{
					stringBuilder.Append(Path.DirectorySeparatorChar);
				}
			}
			if (stringBuilder.Length > 0 && !stringBuilder.ToString().EndsWith(Path.DirectorySeparatorChar.ToString()))
			{
				stringBuilder.Append(Path.DirectorySeparatorChar);
			}
			return stringBuilder.ToString();
		}

		public static LocString AddLocStringToProduct(string message, CultureInfo culture, BootstrapperProduct rootProduct)
		{
			return Helpers.AddLocStringToProduct(message, message, culture, rootProduct);
		}

		public static LocString AddLocStringToProduct(string requestedName, string message, CultureInfo culture, BootstrapperProduct rootProduct)
		{
			if (string.IsNullOrEmpty(requestedName) || string.IsNullOrEmpty(message))
			{
				throw new Exception("An internal error occurred. A null string was requested to be built, which is not allowed.");
			}
			string str = Helpers.RequestStringName(requestedName, culture);
			LocString localizedString = rootProduct.GetLocalizedString(culture, str);
			if (localizedString == null)
			{
				localizedString = new LocString(str, message, culture);
				rootProduct.AddLocalizedString(localizedString);
			}
			Helpers.m_locstrings.Add(localizedString);
			return localizedString;
		}

		public static void CopyFolderContents(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory, bool includeSubFolders)
		{
			int i;
			if (!targetDirectory.Exists)
			{
				targetDirectory.Create();
			}
			FileInfo[] files = sourceDirectory.GetFiles();
			for (i = 0; i < (int)files.Length; i++)
			{
				FileInfo fileInfo = files[i];
				try
				{
					fileInfo.CopyTo(Path.Combine(targetDirectory.FullName, fileInfo.Name), true);
				}
				catch (Exception exception)
				{
				}
			}
			DirectoryInfo[] directories = sourceDirectory.GetDirectories();
			for (i = 0; i < (int)directories.Length; i++)
			{
				DirectoryInfo directoryInfo = directories[i];
				if (directoryInfo.Name != targetDirectory.Name)
				{
					Helpers.CopyFolderContents(directoryInfo, new DirectoryInfo(Path.Combine(targetDirectory.FullName, directoryInfo.Name)), true);
				}
			}
		}

		public static void FillLanguageCombo(ComboBox languageCombo, CultureInfo selectCulture)
		{
			languageCombo.Items.Clear();
			languageCombo.Items.Add("neutral");
			languageCombo.Sorted = false;
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
			for (int i = 0; i < (int)cultures.Length; i++)
			{
				CultureInfo cultureInfo = cultures[i];
				if (cultureInfo.IsNeutralCulture)
				{
					languageCombo.Items.Add(cultureInfo);
				}
			}
			languageCombo.DisplayMember = "DisplayName";
			languageCombo.Sorted = false;
			if (selectCulture == CultureInfo.InvariantCulture)
			{
				languageCombo.Text = "neutral";
				return;
			}
			languageCombo.SelectedItem = selectCulture;
		}

		public static CultureInfo GetCulture(Control inputControl)
		{
			Control parent = inputControl;
			while (!(parent is UIPackage) && !(parent is UIInstallFile) && parent != null)
			{
				parent = parent.Parent;
			}
			if (parent == null)
			{
				throw new Exception("Helpers.GetCulture Failed to find a valid parent");
			}
			if (parent is UIPackage)
			{
				return CultureInfo.InvariantCulture;
			}
			return ((UIInstallFile)parent).Language;
		}

		public static ValueProperty GetMatchingProperty(string name, CultureInfo culture)
		{
			ValueProperty valueProperty;
			IEnumerator enumerator = null;
			IEnumerator enumerator1 = null;
			try
			{
				enumerator = Helpers.BuiltProperties.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Helpers.valueProperty current = (Helpers.valueProperty)enumerator.Current;
					if (!(current.prop.Name == name) || !current.culture.Equals(culture))
					{
						continue;
					}
					valueProperty = current.prop;
					return valueProperty;
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			try
			{
				enumerator1 = Helpers.BuiltProperties.GetEnumerator();
				while (enumerator1.MoveNext())
				{
					Helpers.valueProperty _valueProperty = (Helpers.valueProperty)enumerator1.Current;
					if (!(_valueProperty.prop.Name == name) || !_valueProperty.culture.Equals(CultureInfo.InvariantCulture))
					{
						continue;
					}
					valueProperty = _valueProperty.prop;
					return valueProperty;
				}
				throw new Exception(string.Concat(name, " property has not been built yet."));
			}
			finally
			{
				if (enumerator1 is IDisposable)
				{
					(enumerator1 as IDisposable).Dispose();
				}
			}
			return valueProperty;
		}

		public static Bitmap GetNormalizedImage(byte[] imageData, bool largeImages)
		{
			Bitmap bitmap = null;
			Helpers.Init();
			if (imageData != null)
			{
				int num = 0;
				int[] numArray = new int[] { 16, 20, 24, 28, 32, 40, 48, 56, 64, 72 };
				if (largeImages)
				{
					num = 4;
				}
				if (Helpers.DPI >= 216f)
				{
					num += 5;
				}
				else if (Helpers.DPI >= 192f)
				{
					num += 4;
				}
				else if (Helpers.DPI >= 168f)
				{
					num += 3;
				}
				else if (Helpers.DPI >= 144f)
				{
					num += 2;
				}
				else if (Helpers.DPI >= 120f)
				{
					num++;
				}
				using (MemoryStream memoryStream = new MemoryStream(imageData))
				{
					using (Image image = Image.FromStream(memoryStream))
					{
						bitmap = (!image.RawFormat.Equals(ImageFormat.Icon) ? image.Clone() as Bitmap : new Bitmap(image, numArray[num], numArray[num]));
					}
				}
			}
			return bitmap;
		}

		public static string GetProductName()
		{
			return "Designer for Visual Studio WiX Setup Projects";
		}

		public static void Init()
		{
			if (!Helpers.DPIDetected)
			{
				Helpers.DPIDetected = true;
				using (Form form = new Form())
				{
					using (Graphics graphic = form.CreateGraphics())
					{
						Helpers.DPI = graphic.DpiX;
					}
				}
			}
		}

		public static string RelativizePathIfPossible(string pathToRelativize, string basePath)
		{
			int i;
			if (!basePath.EndsWith("\\"))
			{
				basePath = string.Concat(basePath, "\\");
			}
			if (!pathToRelativize.EndsWith("\\"))
			{
				pathToRelativize = string.Concat(pathToRelativize, "\\");
			}
			string[] strArrays = basePath.Split(new char[] { '\\' });
			string[] strArrays1 = pathToRelativize.Split(new char[] { '\\' });
			int num = ((int)strArrays.Length < (int)strArrays1.Length ? (int)strArrays.Length : (int)strArrays1.Length);
			int num1 = -1;
			for (i = 0; i < num && strArrays[i] == strArrays1[i]; i++)
			{
				num1 = i;
			}
			if (num1 == -1)
			{
				return pathToRelativize;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (i = num1 + 1; i < (int)strArrays.Length; i++)
			{
				if (strArrays[i].Length > 0)
				{
					stringBuilder.Append("..\\");
				}
			}
			for (i = num1 + 1; i < (int)strArrays1.Length - 1; i++)
			{
				stringBuilder.Append(string.Concat(strArrays1[i], "\\"));
			}
			stringBuilder.Append(strArrays1[(int)strArrays1.Length - 1]);
			return stringBuilder.ToString();
		}

		private static string RequestStringName(string stringName, CultureInfo culture)
		{
			string matchingName = Helpers.m_locstrings.GetMatchingName(stringName, culture);
			if (string.IsNullOrEmpty(matchingName))
			{
				matchingName = Helpers.m_locstrings.GetUniqueName(stringName, culture);
			}
			return matchingName;
		}

		public static void ResetLocStrings()
		{
			Helpers.m_locstrings = null;
			Helpers.m_locstrings = new Helpers.LocStringCollection();
		}

		private class LocStringCollection
		{
			private ArrayList m_strings;

			public LocStringCollection()
			{
			}

			public void Add(LocString newLocstring)
			{
				if (this.IsUniqueName(newLocstring.Name, newLocstring.Culture))
				{
					this.m_strings.Add(newLocstring);
				}
			}

			public string GetMatchingName(string str, CultureInfo culture)
			{
				string name;
				IEnumerator enumerator = null;
				IEnumerator enumerator1 = null;
				try
				{
					enumerator = this.m_strings.GetEnumerator();
					while (enumerator.MoveNext())
					{
						LocString current = (LocString)enumerator.Current;
						if (!(current.Value == str) || !current.Culture.Equals(culture))
						{
							continue;
						}
						name = current.Name;
						return name;
					}
				}
				finally
				{
					if (enumerator is IDisposable)
					{
						(enumerator as IDisposable).Dispose();
					}
				}
				try
				{
					enumerator1 = this.m_strings.GetEnumerator();
					while (enumerator1.MoveNext())
					{
						LocString locString = (LocString)enumerator1.Current;
						if (!(locString.Value == str) || !locString.Culture.Equals(CultureInfo.InvariantCulture))
						{
							continue;
						}
						name = locString.Name;
						return name;
					}
					return null;
				}
				finally
				{
					if (enumerator1 is IDisposable)
					{
						(enumerator1 as IDisposable).Dispose();
					}
				}
				return name;
			}

			public string GetUniqueName(string str, CultureInfo culture)
			{
				int num = 24;
				int num1 = 0;
				string str1 = str.Replace(" ", "");
				str1 = str1.Substring(0, Math.Min(num, str1.Length));
				string str2 = str1;
				while (!this.IsUniqueName(str1, culture))
				{
					num1++;
					str1 = string.Concat(str2, "_", num1.ToString());
				}
				return str1;
			}

			private bool IsUniqueName(string name, CultureInfo culture)
			{
				bool flag;
				IEnumerator enumerator = null;
				try
				{
					enumerator = this.m_strings.GetEnumerator();
					while (enumerator.MoveNext())
					{
						LocString current = (LocString)enumerator.Current;
						if (!(current.Name == name) || !current.Culture.Equals(culture))
						{
							continue;
						}
						flag = false;
						return flag;
					}
					return true;
				}
				finally
				{
					if (enumerator is IDisposable)
					{
						(enumerator as IDisposable).Dispose();
					}
				}
				return flag;
			}
		}

		public class valueProperty
		{
			public CultureInfo culture;

			public ValueProperty prop;

			public valueProperty(ValueProperty valueProp, CultureInfo buildCulture)
			{
				this.culture = buildCulture;
				this.prop = valueProp;
			}
		}
	}
}