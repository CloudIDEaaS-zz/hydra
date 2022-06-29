using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSSearches : List<VSSearchBase>
	{
		private WiXProjectParser _project;

		public VSSearches(WiXProjectParser project)
		{
			this._project = project;
		}

		public VSSearchComponent AddSearchComponent()
		{
			string i;
			int num = 1;
			for (i = string.Concat("COMPONENTEXISTS", num.ToString()); base.Find((VSSearchBase e) => e.Property == i) != null; i = string.Concat("COMPONENTEXISTS", num.ToString()))
			{
				num++;
			}
			WiXEntity wiXEntity = null;
			WiXEntity wiXEntity1 = null;
			XmlNode xmlNodes = null;
			XmlElement xmlElement = null;
			XmlElement xmlElement1 = null;
			this.FindParent(ref wiXEntity, ref wiXEntity1, ref xmlNodes);
			if (wiXEntity == null || wiXEntity1 == null)
			{
				throw new Exception("Cannot create Windows Installer Search.");
			}
			xmlElement = Common.CreateXmlElementWithAttributes(wiXEntity.XmlNode.OwnerDocument, "Property", wiXEntity.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { i }, "", false);
			XmlDocument ownerDocument = wiXEntity.XmlNode.OwnerDocument;
			string namespaceURI = wiXEntity.XmlNode.NamespaceURI;
			string[] strArrays = new string[] { "Id", "Guid" };
			string[] upper = new string[] { string.Concat("_", Common.GenerateId(this._project.ProjectType)), null };
			Guid empty = Guid.Empty;
			upper[1] = empty.ToString("B").ToUpper();
			xmlElement1 = Common.CreateXmlElementWithAttributes(ownerDocument, "ComponentSearch", namespaceURI, strArrays, upper, "", false);
			xmlElement.AppendChild(xmlElement1);
			if (xmlNodes == null)
			{
				wiXEntity.XmlNode.AppendChild(xmlElement);
			}
			else
			{
				wiXEntity.XmlNode.InsertAfter(xmlElement, xmlNodes);
			}
			WiXProperty wiXProperty = new WiXProperty(this._project, wiXEntity1, wiXEntity, xmlElement);
			WiXComponentSearch wiXComponentSearch = new WiXComponentSearch(this._project, wiXEntity1, wiXProperty, xmlElement1);
			VSSearchComponent vSSearchComponent = new VSSearchComponent(this._project, wiXComponentSearch, wiXProperty, this);
			wiXEntity.SetDirty();
			return vSSearchComponent;
		}

		public VSSearchFile AddSearchFile()
		{
			string i;
			int num = 1;
			for (i = string.Concat("FILEEXISTS", num.ToString()); base.Find((VSSearchBase e) => e.Property == i) != null; i = string.Concat("FILEEXISTS", num.ToString()))
			{
				num++;
			}
			WiXEntity wiXEntity = null;
			WiXEntity wiXEntity1 = null;
			XmlNode xmlNodes = null;
			XmlElement xmlElement = null;
			XmlElement xmlElement1 = null;
			XmlElement xmlElement2 = null;
			this.FindParent(ref wiXEntity, ref wiXEntity1, ref xmlNodes);
			if (wiXEntity == null || wiXEntity1 == null)
			{
				throw new Exception("Cannot create File Search.");
			}
			xmlElement = Common.CreateXmlElementWithAttributes(wiXEntity.XmlNode.OwnerDocument, "Property", wiXEntity.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { i }, "", false);
			xmlElement1 = Common.CreateXmlElementWithAttributes(wiXEntity.XmlNode.OwnerDocument, "DirectorySearch", wiXEntity.XmlNode.NamespaceURI, new string[] { "Id", "Depth", "Path" }, new string[] { string.Concat("_", Common.GenerateId(this._project.ProjectType)), "0", "[SystemFolder]" }, "", false);
			xmlElement2 = Common.CreateXmlElementWithAttributes(wiXEntity.XmlNode.OwnerDocument, "FileSearch", wiXEntity.XmlNode.NamespaceURI, new string[] { "Name" }, new string[] { "FileName.txt" }, "", false);
			xmlElement1.AppendChild(xmlElement2);
			xmlElement.AppendChild(xmlElement1);
			if (xmlNodes == null)
			{
				wiXEntity.XmlNode.AppendChild(xmlElement);
			}
			else
			{
				wiXEntity.XmlNode.InsertAfter(xmlElement, xmlNodes);
			}
			WiXProperty wiXProperty = new WiXProperty(this._project, wiXEntity1, wiXEntity, xmlElement);
			WiXDirectorySearch wiXDirectorySearch = new WiXDirectorySearch(this._project, wiXEntity1, wiXProperty, xmlElement1);
			WiXFileSearch wiXFileSearch = new WiXFileSearch(this._project, wiXEntity1, wiXDirectorySearch, xmlElement2);
			VSSearchFile vSSearchFile = new VSSearchFile(this._project, wiXFileSearch, wiXDirectorySearch, wiXProperty, this);
			wiXEntity.SetDirty();
			return vSSearchFile;
		}

		public VSSearchRegistry AddSearchRegistry()
		{
			return this.AddSearchRegistry("Software", "Name");
		}

		public VSSearchRegistry AddSearchRegistry(string regKey, string regName)
		{
			string i;
			int num = 1;
			for (i = string.Concat("REGISTRYVALUE", num.ToString()); base.Find((VSSearchBase e) => e.Property == i) != null; i = string.Concat("REGISTRYVALUE", num.ToString()))
			{
				num++;
			}
			WiXEntity wiXEntity = null;
			WiXEntity wiXEntity1 = null;
			XmlNode xmlNodes = null;
			XmlElement xmlElement = null;
			XmlElement xmlElement1 = null;
			this.FindParent(ref wiXEntity, ref wiXEntity1, ref xmlNodes);
			if (wiXEntity == null || wiXEntity1 == null)
			{
				throw new Exception("Cannot create Registry Search.");
			}
			xmlElement = Common.CreateXmlElementWithAttributes(wiXEntity.XmlNode.OwnerDocument, "Property", wiXEntity.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { i }, "", false);
			xmlElement1 = Common.CreateXmlElementWithAttributes(wiXEntity.XmlNode.OwnerDocument, "RegistrySearch", wiXEntity.XmlNode.NamespaceURI, new string[] { "Id", "Root", "Key", "Name", "Type" }, new string[] { string.Concat("_", Common.GenerateId(this._project.ProjectType)), "HKLM", regKey, regName, "raw" }, "", false);
			xmlElement.AppendChild(xmlElement1);
			if (xmlNodes == null)
			{
				wiXEntity.XmlNode.AppendChild(xmlElement);
			}
			else
			{
				wiXEntity.XmlNode.InsertAfter(xmlElement, xmlNodes);
			}
			WiXProperty wiXProperty = new WiXProperty(this._project, wiXEntity1, wiXEntity, xmlElement);
			WiXRegistrySearch wiXRegistrySearch = new WiXRegistrySearch(this._project, wiXEntity1, wiXProperty, xmlElement1);
			VSSearchRegistry vSSearchRegistry = new VSSearchRegistry(this._project, wiXRegistrySearch, wiXProperty, this);
			wiXEntity.SetDirty();
			return vSSearchRegistry;
		}

		private void FindParent(ref WiXEntity parent, ref WiXEntity owner, ref XmlNode insertAfter)
		{
			if (this._project.Searches.Count > 0)
			{
				parent = this._project.Searches[this._project.Searches.Count - 1].WiXProperty.Parent as WiXEntity;
				owner = this._project.Searches[this._project.Searches.Count - 1].WiXProperty.Owner as WiXEntity;
				insertAfter = this._project.Searches[this._project.Searches.Count - 1].WiXProperty.XmlNode;
				return;
			}
			if (this._project.LaunchConditions.Count > 0)
			{
				parent = this._project.LaunchConditions[this._project.LaunchConditions.Count - 1].WiXElement.Parent as WiXEntity;
				owner = this._project.LaunchConditions[this._project.LaunchConditions.Count - 1].WiXElement.Owner as WiXEntity;
				insertAfter = this._project.LaunchConditions[this._project.LaunchConditions.Count - 1].WiXElement.XmlNode;
				return;
			}
			if (this._project.ProjectType != WiXProjectType.Product)
			{
				parent = this._project.SupportedEntities.Find((WiXEntity e) => e is WiXFragment);
				if (parent != null)
				{
					owner = parent.Owner as WiXEntity;
				}
			}
			else
			{
				parent = this._project.SupportedEntities.Find((WiXEntity e) => e is WiXProduct);
				if (parent != null)
				{
					owner = parent.Owner as WiXEntity;
					WiXEntity wiXEntity = parent.ChildEntities.Find((WiXEntity e) => e.Name == "Package");
					if (wiXEntity != null)
					{
						if (parent.ChildEntities.Find((WiXEntity e) => e.Name == "Media") != null)
						{
							wiXEntity = parent.ChildEntities.Find((WiXEntity e) => e.Name == "Media");
						}
						if (parent.ChildEntities.Find((WiXEntity e) => e.Name == "MediaTemplate") != null)
						{
							wiXEntity = parent.ChildEntities.Find((WiXEntity e) => e.Name == "MediaTemplate");
						}
						insertAfter = wiXEntity.XmlNode;
						return;
					}
				}
			}
		}
	}
}