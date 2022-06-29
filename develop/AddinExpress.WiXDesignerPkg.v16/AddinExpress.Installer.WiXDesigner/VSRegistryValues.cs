using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSRegistryValues : List<VSRegistryValueBase>
	{
		private WiXProjectParser _project;

		private VSRegistryKey _parent;

		internal VSRegistryKey ParentKey
		{
			get
			{
				return this._parent;
			}
		}

		internal WiXProjectParser Project
		{
			get
			{
				return this._project;
			}
		}

		public VSRegistryValues(WiXProjectParser project, VSRegistryKey parent)
		{
			this._project = project;
			this._parent = parent;
		}

		internal VSRegistryValueBase Add(string wixType)
		{
			string i;
			int num = 1;
			for (i = string.Concat("New Value #", num.ToString()); base.Find((VSRegistryValueBase e) => e.Name == i) != null; i = string.Concat("New Value #", num.ToString()))
			{
				num++;
			}
			WiXEntity wiXEntity = null;
			WiXEntity wiXEntity1 = null;
			XmlNode xmlNodes = null;
			this.FindParent(ref wiXEntity, ref wiXEntity1, ref xmlNodes);
			if (wiXEntity == null || wiXEntity1 == null)
			{
				throw new Exception("Cannot create registry value.");
			}
			XmlDocument ownerDocument = wiXEntity.XmlNode.OwnerDocument;
			string namespaceURI = wiXEntity.XmlNode.NamespaceURI;
			string[] strArrays = new string[] { "Id", "Guid", "Transitive", "Directory" };
			string[] strArrays1 = new string[] { string.Concat("_", Common.GenerateId(this.Project.ProjectType)), Common.GenerateGuid(), "no", null };
			strArrays1[3] = (this.Project.ProjectType == WiXProjectType.Product ? "TARGETDIR" : string.Empty);
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(ownerDocument, "Component", namespaceURI, strArrays, strArrays1, "", false);
			if (xmlNodes == null)
			{
				wiXEntity.XmlNode.AppendChild(xmlElement);
			}
			else
			{
				wiXEntity.XmlNode.InsertAfter(xmlElement, xmlNodes);
			}
			WiXComponent wiXComponent = new WiXComponent(this.Project, wiXEntity.Owner, wiXEntity, xmlElement);
			VSBaseFolder folderById = this.Project.FileSystem.GetFolderById("TARGETDIR") ?? this.Project.FileSystem.GetFolderById("MergeRedirectFolder") ?? this.Project.FileSystem[0];
			VSComponent vSComponent = new VSComponent(folderById, wiXComponent);
			folderById.Components.Add(vSComponent);
			wiXComponent.Parent.SetDirty();
			XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(wiXEntity.XmlNode.OwnerDocument, "RegistryValue", wiXEntity.XmlNode.NamespaceURI, new string[] { "Root", "Key", "Type", "Name" }, new string[] { this.ParentKey.WiXElement.Root, this.ParentKey.WiXElement.Key, wixType, i }, "", false);
			if (wixType != "integer")
			{
				XmlAttribute itemOf = xmlElement1.Attributes["Value"];
				if (itemOf == null)
				{
					itemOf = xmlElement1.OwnerDocument.CreateAttribute("Value");
					xmlElement1.Attributes.Append(itemOf);
				}
				itemOf.Value = "";
			}
			else
			{
				XmlAttribute xmlAttribute = xmlElement1.Attributes["Value"];
				if (xmlAttribute == null)
				{
					xmlAttribute = xmlElement1.OwnerDocument.CreateAttribute("Value");
					xmlElement1.Attributes.Append(xmlAttribute);
				}
				xmlAttribute.Value = "0";
			}
			xmlElement.AppendChild(xmlElement1);
			WiXRegistryValue wiXRegistryValue = new WiXRegistryValue(this.Project, wiXComponent.Owner, wiXComponent, xmlElement1);
			wiXRegistryValue.Parent.SetDirty();
			VSRegistryValueBase vSRegistryValueString = null;
			if (wixType != null)
			{
				if (wixType == "string")
				{
					vSRegistryValueString = new VSRegistryValueString(this.Project, wiXRegistryValue, vSComponent, this);
				}
				else if (wixType == "expandable")
				{
					vSRegistryValueString = new VSRegistryValueEnvString(this.Project, wiXRegistryValue, vSComponent, this);
				}
				else if (wixType == "binary")
				{
					vSRegistryValueString = new VSRegistryValueBinary(this.Project, wiXRegistryValue, vSComponent, this);
				}
				else if (wixType == "integer")
				{
					vSRegistryValueString = new VSRegistryValueInteger(this.Project, wiXRegistryValue, vSComponent, this);
				}
			}
			return vSRegistryValueString;
		}

		private void FindParent(ref WiXEntity parent, ref WiXEntity owner, ref XmlNode insertAfter)
		{
			if (this.ParentKey != null && this.ParentKey.ParentComponent != null)
			{
				parent = this.ParentKey.ParentComponent.WiXElement.Parent as WiXEntity;
				owner = this.ParentKey.ParentComponent.WiXElement.Owner as WiXEntity;
				if (base.Count > 0 && base[base.Count - 1].ParentComponent != null)
				{
					insertAfter = base[base.Count - 1].ParentComponent.WiXElement.XmlNode;
				}
				if (insertAfter == null)
				{
					insertAfter = this.ParentKey.ParentComponent.WiXElement.XmlNode;
				}
			}
		}
	}
}