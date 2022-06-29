using System;
using System.Collections.Generic;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXProjectItem : WiXEntity
	{
		private string _wixSourceFilePath = string.Empty;

		private XmlDocument _wixSourceDocument;

		internal XmlDocument SourceDocument
		{
			get
			{
				return this._wixSourceDocument;
			}
		}

		internal string SourceFile
		{
			get
			{
				return this._wixSourceFilePath;
			}
		}

		internal WiXProjectItem(WiXProjectParser project) : base(project)
		{
			this._wixSourceDocument = new XmlDocument();
		}

		internal WiXProjectItem(WiXProjectParser owner, string filePath, XmlDocument doc) : this(owner)
		{
			this._wixSourceFilePath = filePath;
			this._wixSourceDocument = doc;
			this._name = this._wixSourceDocument.DocumentElement.Name;
			this._xmlNode = this._wixSourceDocument.DocumentElement;
			this.Parse(this, this, base.XmlNode.ChildNodes);
		}

		internal List<WiXEntity> GetWiXEntitiesByXmlTypeAndName(Type type, string name, List<WiXEntity> list)
		{
			for (int i = 0; i < this.ChildEntities.Count; i++)
			{
				if (this.ChildEntities[i].XmlNode.GetType() == type && this.ChildEntities[i].Name == name)
				{
					list.Add(this.ChildEntities[i]);
				}
			}
			return list;
		}

		private List<WiXEntity> GetWiXEntitiesByXmlTypeAndName(WiXEntityList childEntities, Type type, string name, List<WiXEntity> list)
		{
			for (int i = 0; i < childEntities.Count; i++)
			{
				if (childEntities[i].XmlNode.GetType() == type && childEntities[i].Name == name)
				{
					list.Add(childEntities[i]);
				}
				if (childEntities[i].HasChildEntities)
				{
					list = this.GetWiXEntitiesByXmlTypeAndName(childEntities[i].ChildEntities, type, name, list);
				}
			}
			return list;
		}

		internal WiXEntity GetWiXEntityById(string name, string id)
		{
			for (int i = 0; i < this.ChildEntities.Count; i++)
			{
				if (this.ChildEntities[i].Name == name && this.ChildEntities[i].GetAttributeValue("Id") == id)
				{
					return this.ChildEntities[i];
				}
				if (this.ChildEntities[i].HasChildEntities)
				{
					WiXEntity wiXEntityById = this.GetWiXEntityById(this.ChildEntities[i].ChildEntities, name, id);
					if (wiXEntityById != null)
					{
						return wiXEntityById;
					}
				}
			}
			return null;
		}

		private WiXEntity GetWiXEntityById(WiXEntityList list, string name, string id)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Name == name && list[i].GetAttributeValue("Id") == id)
				{
					return list[i];
				}
				if (list[i].HasChildEntities)
				{
					WiXEntity wiXEntityById = this.GetWiXEntityById(list[i].ChildEntities, name, id);
					if (wiXEntityById != null)
					{
						return wiXEntityById;
					}
				}
			}
			return null;
		}

		private bool IsAllUpper(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return false;
			}
			for (int i = 0; i < input.Length; i++)
			{
				if (char.IsLetterOrDigit(input[i]) && !char.IsUpper(input[i]))
				{
					return false;
				}
			}
			return true;
		}

		internal void Parse(IWiXEntity parent)
		{
			if (parent is WiXEntity)
			{
				parent.ChildEntities.Clear();
				this.Parse(this, parent, ((WiXEntity)parent).XmlNode.ChildNodes);
			}
		}

		private void Parse(IWiXEntity owner, IWiXEntity parent, XmlNodeList childNodes)
		{
			int i;
			WiXEntity wiXModule = null;
			if (parent.Name != "WixLocalization")
			{
				for (i = 0; i < childNodes.Count; i++)
				{
					string lower = childNodes[i].LocalName.ToLower();
					switch (lower)
					{
						case null:
						{
							goto Label0;
						}
						case "patchcreation":
						{
							if (base.Project.ProjectType == WiXProjectType.Unknown)
							{
								base.Project.ProjectType = WiXProjectType.PatchCreation;
								goto Label0;
							}
							else
							{
								goto Label0;
							}
						}
						case "bundle":
						{
							if (base.Project.ProjectType == WiXProjectType.Unknown)
							{
								base.Project.ProjectType = WiXProjectType.Bundle;
								goto Label0;
							}
							else
							{
								goto Label0;
							}
						}
						case "patch":
						{
							if (base.Project.ProjectType == WiXProjectType.Unknown)
							{
								base.Project.ProjectType = WiXProjectType.Patch;
								goto Label0;
							}
							else
							{
								goto Label0;
							}
						}
						case "module":
						{
							if (base.Project.ProjectType == WiXProjectType.Unknown)
							{
								base.Project.ProjectType = WiXProjectType.Module;
							}
							wiXModule = new WiXModule(base.Project, owner, parent, childNodes[i]);
							base.Project.Namespace = wiXModule.XmlNode.NamespaceURI;
							break;
						}
						case "product":
						{
							if (base.Project.ProjectType == WiXProjectType.Unknown)
							{
								base.Project.ProjectType = WiXProjectType.Product;
							}
							wiXModule = new WiXProduct(base.Project, owner, parent, childNodes[i]);
							base.Project.Namespace = wiXModule.XmlNode.NamespaceURI;
							break;
						}
						case "package":
						{
							wiXModule = new WiXPackage(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "upgrade":
						{
							wiXModule = new WiXUpgrade(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "fragment":
						{
							wiXModule = new WiXFragment(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "feature":
						{
							wiXModule = new WiXFeature(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "featureref":
						{
							wiXModule = new WiXFeatureRef(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "featuregroup":
						{
							wiXModule = new WiXFeatureGroup(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "featuregroupref":
						{
							wiXModule = new WiXFeatureGroupRef(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "customaction":
						{
							wiXModule = new WiXCustomAction(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "custom":
						{
							wiXModule = new WiXCustom(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "property":
						{
							wiXModule = new WiXProperty(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "setdirectory":
						{
							wiXModule = new WiXSetDirectory(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "setproperty":
						{
							wiXModule = new WiXSetProperty(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "createfolder":
						{
							wiXModule = new WiXCreateFolder(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "removefolder":
						{
							wiXModule = new WiXRemoveFolder(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "condition":
						{
							wiXModule = new WiXCondition(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "registrysearch":
						{
							wiXModule = new WiXRegistrySearch(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "registrysearchref":
						{
							wiXModule = new WiXRegistrySearchRef(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "componentsearch":
						{
							wiXModule = new WiXComponentSearch(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "filesearch":
						{
							wiXModule = new WiXFileSearch(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "filesearchref":
						{
							wiXModule = new WiXFileSearchRef(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "directorysearch":
						{
							wiXModule = new WiXDirectorySearch(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "directorysearchref":
						{
							wiXModule = new WiXDirectorySearchRef(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "directory":
						{
							wiXModule = new WiXDirectory(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "directoryref":
						{
							wiXModule = new WiXDirectoryRef(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "component":
						{
							wiXModule = new WiXComponent(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "componentref":
						{
							wiXModule = new WiXComponentRef(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "componentgroup":
						{
							wiXModule = new WiXComponentGroup(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "componentgroupref":
						{
							wiXModule = new WiXComponentGroupRef(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "file":
						{
							wiXModule = new WiXFile(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "shortcut":
						{
							wiXModule = new WiXShortcut(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "binary":
						{
							wiXModule = new WiXBinary(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "icon":
						{
							wiXModule = new WiXIcon(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "registry":
						{
							wiXModule = new WiXRegistry(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "registrykey":
						{
							wiXModule = new WiXRegistryKey(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "registryvalue":
						{
							wiXModule = new WiXRegistryValue(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "progid":
						{
							wiXModule = new WiXProgId(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "extension":
						{
							wiXModule = new WiXExtension(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "verb":
						{
							wiXModule = new WiXVerb(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "mime":
						{
							wiXModule = new WiXMIME(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "error":
						{
							wiXModule = new WiXError(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "merge":
						{
							wiXModule = new WiXMerge(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "mergeref":
						{
							wiXModule = new WiXMergeRef(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "dependency":
						{
							wiXModule = new WiXDependency(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "certificate":
						{
							wiXModule = new IISCertificate(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "certificateref":
						{
							wiXModule = new IISCertificateRef(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "httpheader":
						{
							wiXModule = new IISHttpHeader(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "mimemap":
						{
							wiXModule = new IISMimeMap(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "recycletime":
						{
							wiXModule = new IISRecycleTime(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "webaddress":
						{
							wiXModule = new IISWebAddress(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "webapplication":
						{
							wiXModule = new IISWebApplication(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "webapplicationextension":
						{
							wiXModule = new IISWebApplicationExtension(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "webapppool":
						{
							wiXModule = new IISWebAppPool(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "webdir":
						{
							wiXModule = new IISWebDir(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "webdirproperties":
						{
							wiXModule = new IISWebDirProperties(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "weberror":
						{
							wiXModule = new IISWebError(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "webfilter":
						{
							wiXModule = new IISWebFilter(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "weblog":
						{
							wiXModule = new IISWebLog(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "webproperty":
						{
							wiXModule = new IISWebProperty(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "webserviceextension":
						{
							wiXModule = new IISWebServiceExtension(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "website":
						{
							wiXModule = new IISWebSite(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "webvirtualdir":
						{
							wiXModule = new IISWebVirtualDir(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "adminuisequence":
						{
							wiXModule = new WiXAdminUISequence(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "control":
						{
							wiXModule = new WiXControl(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "dialog":
						{
							wiXModule = new WiXDialog(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "dialogref":
						{
							wiXModule = new WiXDialogRef(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "installuisequence":
						{
							wiXModule = new WiXInstallUISequence(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "installexecutesequence":
						{
							wiXModule = new WiXInstallExecuteSequence(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "publish":
						{
							wiXModule = new WiXPublish(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "radiobutton":
						{
							wiXModule = new WiXRadioButton(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "radiobuttongroup":
						{
							wiXModule = new WiXRadioButtonGroup(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "show":
						{
							wiXModule = new WiXShow(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "subscribe":
						{
							wiXModule = new WiXSubscribe(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "textstyle":
						{
							wiXModule = new WiXTextStyle(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "ui":
						{
							wiXModule = new WiXUI(base.Project, owner, parent, childNodes[i]);
							break;
						}
						case "uiref":
						{
							wiXModule = new WiXUIRef(base.Project, owner, parent, childNodes[i]);
							break;
						}
						default:
						{
							if (lower != "uitext")
							{
								goto Label0;
							}
							wiXModule = new WiXUIText(base.Project, owner, parent, childNodes[i]);
							break;
						}
					}
				Label1:
					if (!this.ChildEntities.Contains(wiXModule))
					{
						this.ChildEntities.Add(wiXModule);
					}
					if (wiXModule.IsSupported)
					{
						base.Project.SupportedEntities.Add(wiXModule);
					}
					if (childNodes[i].HasChildNodes)
					{
						this.Parse(owner, wiXModule, childNodes[i].ChildNodes);
					}
				}
			}
			else
			{
				WiXLocalization wiXLocalization = new WiXLocalization(base.Project, owner, null, (parent as WiXEntity).XmlNode);
				if (wiXLocalization.IsSupported)
				{
					base.Project.Languages.Add(wiXLocalization);
					string lCID = base.Project.ProjectManager.ProjectProperties.LCID;
					if (string.IsNullOrEmpty(lCID))
					{
						lCID = "0";
					}
					if (lCID == wiXLocalization.Id)
					{
						wiXLocalization.Parse();
						return;
					}
				}
			}
			return;
		Label0:
			wiXModule = new WiXEntity(base.Project, owner, parent, childNodes[i]);
			goto Label1;
		}
	}
}