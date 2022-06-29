using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSRegistryKeys : List<VSRegistryKey>
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

		public VSRegistryKeys(WiXProjectParser project, VSRegistryKey parent)
		{
			this._project = project;
			this._parent = parent;
		}

		internal VSRegistryKey AddRegistryKey()
		{
			string str;
			string i;
			int num = 1;
			for (i = string.Concat("New Key #", num.ToString()); base.Find((VSRegistryKey e) => e.Name == i) != null; i = string.Concat("New Key #", num.ToString()))
			{
				num++;
			}
			WiXEntity wiXEntity = null;
			WiXEntity wiXEntity1 = null;
			XmlNode xmlNodes = null;
			this.FindParent(ref wiXEntity, ref wiXEntity1, ref xmlNodes);
			if (wiXEntity == null || wiXEntity1 == null)
			{
				throw new Exception("Cannot create registry key.");
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
			string empty = string.Empty;
			if (this.ParentKey == null)
			{
				if (this == this.Project.Registry.HKCR)
				{
					empty = "HKCR";
				}
				else if (this == this.Project.Registry.HKCU)
				{
					empty = "HKCU";
				}
				else if (this == this.Project.Registry.HKLM)
				{
					empty = "HKLM";
				}
				else if (this == this.Project.Registry.HKU)
				{
					empty = "HKU";
				}
				else if (this == this.Project.Registry.HKMU)
				{
					empty = "HKMU";
				}
				str = i;
			}
			else
			{
				empty = this.ParentKey.WiXElement.Root;
				str = string.Concat(this.ParentKey.WiXElement.Key, "\\", i);
			}
			XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(wiXEntity.XmlNode.OwnerDocument, "RegistryKey", wiXEntity.XmlNode.NamespaceURI, new string[] { "Root", "Key", "ForceCreateOnInstall", "ForceDeleteOnUninstall" }, new string[] { empty, str, "no", "no" }, "", false);
			xmlElement.AppendChild(xmlElement1);
			WiXRegistryKey wiXRegistryKey = new WiXRegistryKey(this.Project, wiXComponent.Owner, wiXComponent, xmlElement1);
			wiXRegistryKey.Parent.SetDirty();
			return new VSRegistryKey(this.Project, wiXRegistryKey, vSComponent, this);
		}

		private void FindParent(ref WiXEntity parent, ref WiXEntity owner, ref XmlNode insertAfter)
		{
			VSRegistryKey item = null;
			if (base.Count > 0)
			{
				item = base[base.Count - 1];
				parent = item.ParentComponent.WiXElement.Parent as WiXEntity;
				owner = item.ParentComponent.WiXElement.Owner as WiXEntity;
				insertAfter = item.ParentComponent.WiXElement.XmlNode;
				return;
			}
			if (this.ParentKey != null)
			{
				item = this.ParentKey;
				parent = item.ParentComponent.WiXElement.Parent as WiXEntity;
				owner = item.ParentComponent.WiXElement.Owner as WiXEntity;
				insertAfter = item.ParentComponent.WiXElement.XmlNode;
				return;
			}
			if (this.Project.Registry.HKCR.Count > 0)
			{
				item = this.Project.Registry.HKCR[this.Project.Registry.HKCR.Count - 1];
			}
			else if (this.Project.Registry.HKCU.Count > 0)
			{
				item = this.Project.Registry.HKCU[this.Project.Registry.HKCU.Count - 1];
			}
			else if (this.Project.Registry.HKLM.Count > 0)
			{
				item = this.Project.Registry.HKLM[this.Project.Registry.HKLM.Count - 1];
			}
			else if (this.Project.Registry.HKU.Count > 0)
			{
				item = this.Project.Registry.HKU[this.Project.Registry.HKU.Count - 1];
			}
			else if (this.Project.Registry.HKMU.Count > 0)
			{
				item = this.Project.Registry.HKMU[this.Project.Registry.HKMU.Count - 1];
			}
			if (item != null)
			{
				parent = item.ParentComponent.WiXElement.Parent as WiXEntity;
				owner = item.ParentComponent.WiXElement.Owner as WiXEntity;
				insertAfter = item.ParentComponent.WiXElement.XmlNode;
				return;
			}
			if (this.Project.ProjectType == WiXProjectType.Product)
			{
				VSComponentGroup vSComponentGroup = this.Project.ComponentGroups.Find((VSComponentGroup e) => e.WiXElement.Id == "RegistryGroup");
				if (vSComponentGroup == null)
				{
					WiXProduct wiXProduct = this.Project.SupportedEntities.Find((WiXEntity e) => e is WiXProduct) as WiXProduct;
					if (wiXProduct == null)
					{
						return;
					}
					VSBaseFolder folderById = this.Project.FileSystem.GetFolderById("TARGETDIR");
					if (folderById == null)
					{
						return;
					}
					VSFeature vSFeature = null;
					if (this.Project.Features.Count > 0)
					{
						vSFeature = this.Project.Features[0];
					}
					if (vSFeature == null)
					{
						return;
					}
					XmlElement xmlElement = Common.CreateXmlElementWithAttributes(wiXProduct.XmlNode.OwnerDocument, "ComponentGroup", wiXProduct.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { "RegistryGroup" }, "", false);
					folderById.WiXElement.XmlNode.ParentNode.InsertAfter(xmlElement, folderById.WiXElement.XmlNode);
					WiXComponentGroup wiXComponentGroup = new WiXComponentGroup(this.Project, folderById.WiXElement.Owner, folderById.WiXElement.Parent, xmlElement);
					wiXComponentGroup.Parent.SetDirty();
					vSComponentGroup = new VSComponentGroup(folderById, wiXComponentGroup);
					XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(wiXProduct.XmlNode.OwnerDocument, "ComponentGroupRef", wiXProduct.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { "RegistryGroup" }, "", false);
					vSFeature.WiXElement.XmlNode.AppendChild(xmlElement1);
					WiXComponentGroupRef wiXComponentGroupRef = new WiXComponentGroupRef(this.Project, vSFeature.WiXElement.Owner, vSFeature.WiXElement, xmlElement1);
					wiXComponentGroupRef.Parent.SetDirty();
					vSComponentGroup.ComponentGroupRefs.Add(wiXComponentGroupRef);
					this.Project.ComponentGroups.Add(vSComponentGroup);
				}
				if (vSComponentGroup != null)
				{
					parent = vSComponentGroup.WiXElement;
					owner = vSComponentGroup.WiXElement.Owner as WiXEntity;
					insertAfter = null;
					return;
				}
			}
			else if (this.Project.ProjectType == WiXProjectType.Module)
			{
				VSBaseFolder vSBaseFolder = this.Project.FileSystem.GetFolderById("TARGETDIR");
				if (vSBaseFolder != null)
				{
					parent = vSBaseFolder.WiXElement;
					owner = vSBaseFolder.WiXElement.Owner as WiXEntity;
					insertAfter = null;
					return;
				}
				WiXModule wiXModule = this.Project.SupportedEntities.Find((WiXEntity e) => e is WiXModule) as WiXModule;
				if (wiXModule != null)
				{
					WiXEntity wiXEntity = Common.FindChild(wiXModule, "Directory", false);
					if (wiXEntity != null)
					{
						parent = wiXEntity;
						owner = wiXEntity.Owner as WiXEntity;
						insertAfter = null;
					}
				}
			}
		}

		internal VSRegistryKey FindRegistryKey(string fullPath)
		{
			return this.FindRegistryKey(this, fullPath);
		}

		private VSRegistryKey FindRegistryKey(VSRegistryKeys keys, string fullPath)
		{
			VSRegistryKey vSRegistryKey;
			List<VSRegistryKey>.Enumerator enumerator = keys.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					VSRegistryKey current = enumerator.Current;
					if (current.FullPath != fullPath)
					{
						if (current.Keys.Count <= 0)
						{
							continue;
						}
						VSRegistryKey vSRegistryKey1 = this.FindRegistryKey(current.Keys, fullPath);
						if (vSRegistryKey1 == null)
						{
							continue;
						}
						vSRegistryKey = vSRegistryKey1;
						return vSRegistryKey;
					}
					else
					{
						vSRegistryKey = current;
						return vSRegistryKey;
					}
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return vSRegistryKey;
		}
	}
}