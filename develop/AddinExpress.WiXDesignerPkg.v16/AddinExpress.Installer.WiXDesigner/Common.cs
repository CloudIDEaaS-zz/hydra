using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class Common
	{
		internal static string ProductName;

		static Common()
		{
			Common.ProductName = "Designer for Visual Studio WiX Setup Projects";
		}

		public Common()
		{
		}

		internal static XmlElement CreateXmlElementWithAttributes(XmlDocument doc, string qualifiedName, string namespaceURI, string[] attributeNames, string[] attributeValues, string prefix = "", bool allowEmptyValues = false)
		{
			XmlElement xmlElement = doc.CreateElement(qualifiedName, namespaceURI);
			xmlElement.Prefix = prefix;
			if (attributeNames != null && attributeValues != null && (int)attributeNames.Length == (int)attributeValues.Length)
			{
				for (int i = 0; i < (int)attributeNames.Length; i++)
				{
					if (!string.IsNullOrEmpty(attributeValues[i]) || allowEmptyValues)
					{
						XmlAttribute xmlAttribute = doc.CreateAttribute(attributeNames[i]);
						xmlAttribute.Value = attributeValues[i];
						xmlElement.Attributes.Append(xmlAttribute);
					}
				}
			}
			return xmlElement;
		}

		internal static bool FileExists(string path, WiXProjectItem parent)
		{
			string empty = string.Empty;
			return Common.FileExists(path, parent, out empty);
		}

		internal static bool FileExists(string path, WiXProjectItem parent, out string realPath)
		{
			realPath = path;
			if (File.Exists(path))
			{
				return true;
			}
			if (parent != null)
			{
				realPath = Path.GetDirectoryName(parent.SourceFile);
				realPath = Path.Combine(realPath, path);
				if (File.Exists(realPath))
				{
					return true;
				}
			}
			realPath = string.Empty;
			return false;
		}

		internal static WiXEntity FindChild(WiXEntity parent, string childName, bool recursive)
		{
			if (parent != null)
			{
				if (recursive)
				{
					return Common.FindChild(parent.ChildEntities, childName);
				}
				for (int i = 0; i < parent.ChildEntities.Count; i++)
				{
					if (parent.ChildEntities[i].Name.ToLower() == childName.ToLower())
					{
						return parent.ChildEntities[i];
					}
				}
			}
			return null;
		}

		private static WiXEntity FindChild(WiXEntityList list, string childName)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Name.ToLower() == childName.ToLower())
				{
					return list[i];
				}
				if (list[i].ChildEntities.Count > 0)
				{
					WiXEntity wiXEntity = Common.FindChild(list[i].ChildEntities, childName);
					if (wiXEntity != null)
					{
						return wiXEntity;
					}
				}
			}
			return null;
		}

		internal static WiXEntity FindParent(WiXEntity entity, string parentName)
		{
			for (WiXEntity i = entity.Parent as WiXEntity; i != null; i = i.Parent as WiXEntity)
			{
				if (i.Name == parentName)
				{
					return i;
				}
			}
			return null;
		}

		internal static string GenerateBinaryId(WiXProjectType projectType)
		{
			return string.Concat("_", Common.GenerateId(projectType));
		}

		internal static string GenerateGuid()
		{
			return Guid.NewGuid().ToString("D").ToUpper();
		}

		internal static string GenerateIconId()
		{
			return string.Concat("_", Common.GenerateGuid().Replace("-", "").ToUpper().Substring(0, 12));
		}

		internal static string GenerateId(WiXProjectType projectType)
		{
			if (projectType == WiXProjectType.Module)
			{
				return Common.GenerateShortId();
			}
			return Common.GenerateGuid().Replace("-", "_").ToUpper();
		}

		internal static string GenerateShortId()
		{
			return Common.GenerateGuid().Substring(0, 13).Replace("-", "_").ToUpper();
		}

		internal static string MakeRelativePath(string from, string to)
		{
			Uri uri = new Uri(from);
			Uri uri1 = new Uri(to);
			return Uri.UnescapeDataString(uri.MakeRelativeUri(uri1).ToString()).Replace('/', Path.DirectorySeparatorChar);
		}

		internal static DialogResult ShowMessage(Control parent, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			while (parent != null && parent.RightToLeft == RightToLeft.Inherit)
			{
				parent = parent.Parent;
			}
			MessageBoxOptions messageBoxOption = (MessageBoxOptions)0;
			if (parent != null && parent.RightToLeft == RightToLeft.Yes)
			{
				messageBoxOption = MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
			}
			return MessageBox.Show(text, caption, buttons, icon, MessageBoxDefaultButton.Button1, messageBoxOption);
		}
	}
}