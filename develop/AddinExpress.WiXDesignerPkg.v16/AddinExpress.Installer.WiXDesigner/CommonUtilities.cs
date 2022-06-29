using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AddinExpress.Installer.WiXDesigner
{
	internal static class CommonUtilities
	{
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
				throw new ArgumentException(Resources.PathNotRelative, "relativePath");
			}
			if (!Path.IsPathRooted(baseFolderForDerelativization))
			{
				throw new ArgumentException(Resources.BaseFolderMustBeRooted, "baseFolderForDerelativization");
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
						return stringBuilder.ToString();
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
			return stringBuilder.ToString();
		}

		private static bool CanRelativize(string absolutePath, string basePath)
		{
			if (absolutePath == null)
			{
				throw new ArgumentNullException("pathToRelativize");
			}
			if (basePath == null)
			{
				throw new ArgumentNullException("basePath");
			}
			if (!Path.IsPathRooted(absolutePath) || !Path.IsPathRooted(basePath))
			{
				throw new ArgumentException(Resources.BothMustBeRooted);
			}
			return string.Compare(Path.GetPathRoot(absolutePath), Path.GetPathRoot(basePath), StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static string Concatenate(IEnumerable<string> inputs, string separator)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string input in inputs)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(separator);
				}
				stringBuilder.Append(input);
			}
			return stringBuilder.ToString();
		}

		public static int CountInstances(string text, char toFind)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			int num = 0;
			string str = text;
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] == toFind)
				{
					num++;
				}
			}
			return num;
		}

		public static string Duplicate(string text, int count)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			StringBuilder stringBuilder = new StringBuilder(text.Length * count);
			for (int i = 0; i < count; i++)
			{
				stringBuilder.Append(text);
			}
			return stringBuilder.ToString();
		}

		public static string EncodeProgramFilesVar(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return path;
			}
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			if (!path.StartsWith(folderPath, StringComparison.OrdinalIgnoreCase))
			{
				return path;
			}
			return string.Concat("$(ProgramFiles)", path.Substring(folderPath.Length));
		}

		public static string EscapeChar(string text, char toEscape)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			char[] chrArray = new char[] { toEscape, '\\' };
			for (int i = text.IndexOfAny(chrArray, num); i >= 0; i = text.IndexOfAny(chrArray, num))
			{
				stringBuilder.Append(text.Substring(num, i - num));
				stringBuilder.Append("\\");
				stringBuilder.Append(text[i]);
				num = i + 1;
			}
			stringBuilder.Append(text.Substring(num));
			return stringBuilder.ToString();
		}

		public static string FindCommonSubstring(string first, string second, bool ignoreCase)
		{
			int i;
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			if (second == null)
			{
				throw new ArgumentNullException("second");
			}
			for (i = 0; i < first.Length && i < second.Length; i++)
			{
				if (ignoreCase)
				{
					if (char.ToLowerInvariant(first[i]) != char.ToLowerInvariant(second[i]))
					{
						break;
					}
				}
				else if (first[i] != second[i])
				{
					break;
				}
			}
			return first.Substring(0, i);
		}

		public static string GetRelativeDirectory(string absDirPath, string relDirTo)
		{
			int i;
			if (!absDirPath.EndsWith("\\"))
			{
				absDirPath = string.Concat(absDirPath, "\\");
			}
			if (!relDirTo.EndsWith("\\"))
			{
				relDirTo = string.Concat(relDirTo, "\\");
			}
			string[] strArrays = absDirPath.Split(new char[] { '\\' });
			string[] strArrays1 = relDirTo.Split(new char[] { '\\' });
			int num = ((int)strArrays.Length < (int)strArrays1.Length ? (int)strArrays.Length : (int)strArrays1.Length);
			int num1 = -1;
			for (i = 0; i < num && strArrays[i] == strArrays1[i]; i++)
			{
				num1 = i;
			}
			if (num1 == -1)
			{
				return relDirTo;
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

		public static bool IsFileExtensionSupported(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				return false;
			}
			string extension = Path.GetExtension(fileName);
			if (extension.Equals(".wxs", StringComparison.InvariantCultureIgnoreCase) || extension.Equals(".wxi", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			return extension.Equals(".wxl", StringComparison.InvariantCultureIgnoreCase);
		}

		public static bool IsXMLRefFile(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				return false;
			}
			string extension = Path.GetExtension(fileName);
			string str = Path.GetFileName(Path.GetDirectoryName(fileName));
			if (!extension.Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
			{
				return false;
			}
			return str.Equals("XSLT", StringComparison.InvariantCultureIgnoreCase);
		}

		public static bool OrderedCollectionsAreEqual<T>(IList<T> first, IList<T> second)
		{
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			if (second == null)
			{
				throw new ArgumentNullException("second");
			}
			if (first.Count != second.Count)
			{
				return false;
			}
			for (int i = 0; i < first.Count; i++)
			{
				if (second.IndexOf(first[i]) != i)
				{
					return false;
				}
			}
			return true;
		}

		public static IList<string> ParseEscaped(string text, char separator)
		{
			List<string> strs = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			char[] chrArray = new char[] { separator, '\\' };
			int num = 0;
			for (int i = text.IndexOfAny(chrArray, num); i >= 0; i = text.IndexOfAny(chrArray, num))
			{
				stringBuilder.Append(text.Substring(num, i - num));
				if (text[i] == separator)
				{
					strs.Add(stringBuilder.ToString());
					stringBuilder.Length = 0;
				}
				else if (i + 1 < text.Length)
				{
					if (text[i + 1] == separator)
					{
						stringBuilder.Append(separator);
						i++;
					}
					else if (text[i + 1] == '\\')
					{
						stringBuilder.Append('\\');
						i++;
					}
				}
				num = i + 1;
			}
			if (num < text.Length)
			{
				stringBuilder.Append(text.Substring(num));
			}
			if (stringBuilder.Length > 0)
			{
				strs.Add(stringBuilder.ToString());
			}
			return strs;
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

		public static bool UnorderedCollectionsAreEqual<T>(ICollection<T> first, ICollection<T> second)
		{
			bool flag;
			if (first == null)
			{
				throw new ArgumentNullException("first");
			}
			if (second == null)
			{
				throw new ArgumentNullException("second");
			}
			if (first.Count != second.Count)
			{
				return false;
			}
			using (IEnumerator<T> enumerator = first.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (second.Contains(enumerator.Current))
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			return flag;
		}
	}
}