using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSFolders : List<VSBaseFolder>
	{
		private WiXProjectParser _project;

		private VSBaseFolder _owner;

		internal VSFolders(WiXProjectParser project, VSBaseFolder owner)
		{
			this._project = project;
			this._owner = owner;
		}

		internal VSCustomFolder AddCustomFolder()
		{
			WiXDirectory wiXDirectory;
			string i;
			string j;
			if (this._owner != null)
			{
				return null;
			}
			int num = 1;
			for (i = string.Concat("Custom Folder #", num.ToString()); base.Find((VSBaseFolder e) => e.Name == i) != null; i = string.Concat("Custom Folder #", num.ToString()))
			{
				num++;
			}
			num = 1;
			for (j = string.Concat("NEWPROPERTY", num.ToString()); base.Find((VSBaseFolder e) => e.Property == j) != null; j = string.Concat("NEWPROPERTY", num.ToString()))
			{
				num++;
			}
			VSBaseFolder item = base[base.Count - 1];
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(item.WiXElement.XmlNode.OwnerDocument, "Directory", item.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Name" }, new string[] { j, i }, "", false);
			if (base.Count != 1 || !(base[0].Property != SpecialFolderType.MergeRedirectFolder.ToString()))
			{
				item.WiXElement.XmlNode.ParentNode.AppendChild(xmlElement);
				wiXDirectory = new WiXDirectory(this._project, item.WiXElement.Owner, item.WiXElement.Parent, xmlElement);
			}
			else
			{
				item.WiXElement.XmlNode.AppendChild(xmlElement);
				wiXDirectory = new WiXDirectory(this._project, item.WiXElement.Owner, item.WiXElement, xmlElement);
			}
			wiXDirectory.Parent.SetDirty();
			VSCustomFolder vSCustomFolder = new VSCustomFolder(this._project, this._owner, wiXDirectory)
			{
				Name = i,
				Property = j
			};
			WiXEntity wiXEntity = null;
			WiXEntity wiXEntity1 = Common.FindParent(item.WiXElement, "Fragment");
			if (wiXEntity1 == null)
			{
				wiXEntity1 = Common.FindParent(item.WiXElement, "Product") ?? Common.FindParent(item.WiXElement, "Module");
				if (wiXEntity1 != null)
				{
					wiXEntity = wiXEntity1.ChildEntities.Find((WiXEntity e) => e.Name == "Package");
				}
			}
			xmlElement = Common.CreateXmlElementWithAttributes(item.WiXElement.XmlNode.OwnerDocument, "SetDirectory", item.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Value" }, new string[] { j, "[TARGETDIR]" }, "", false);
			if (wiXEntity != null)
			{
				wiXEntity1.XmlNode.InsertAfter(xmlElement, wiXEntity.XmlNode);
			}
			else
			{
				wiXEntity1.XmlNode.InsertBefore(xmlElement, wiXEntity1.XmlNode.FirstChild);
			}
			vSCustomFolder.WiXSetDirectory = new WiXSetDirectory(this._project, item.WiXElement.Owner, wiXEntity1, xmlElement);
			wiXEntity1.SetDirty();
			base.Add(vSCustomFolder);
			return vSCustomFolder;
		}

		internal VSFolder AddFolder()
		{
			string i;
			if (this._owner == null)
			{
				return null;
			}
			int num = 1;
			for (i = string.Concat("New Folder #", num.ToString()); base.Find((VSBaseFolder e) => e.Name == i) != null; i = string.Concat("New Folder #", num.ToString()))
			{
				num++;
			}
			string str = string.Concat("dir_", Common.GenerateId(this._project.ProjectType));
			WiXDirectory wiXDirectory = null;
			if (this._owner == null)
			{
				VSBaseFolder item = base[base.Count - 1];
				XmlElement xmlElement = Common.CreateXmlElementWithAttributes(item.WiXElement.XmlNode.OwnerDocument, "Directory", item.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Name" }, new string[] { str, i }, "", false);
				item.WiXElement.XmlNode.ParentNode.AppendChild(xmlElement);
				wiXDirectory = new WiXDirectory(this._project, item.WiXElement.Owner, item.WiXElement.Parent, xmlElement);
			}
			else
			{
				XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(this._owner.WiXElement.XmlNode.OwnerDocument, "Directory", this._owner.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Name" }, new string[] { str, i }, "", false);
				this._owner.WiXElement.XmlNode.AppendChild(xmlElement1);
				wiXDirectory = new WiXDirectory(this._project, this._owner.WiXElement.Owner, this._owner.WiXElement, xmlElement1);
			}
			if (wiXDirectory == null)
			{
				return null;
			}
			wiXDirectory.Parent.SetDirty();
			VSFolder vSFolder = new VSFolder(this._project, this._owner, wiXDirectory)
			{
				Name = i,
				Property = str
			};
			base.Add(vSFolder);
			if (VSSpecialFolder.CheckForUserFolder(vSFolder))
			{
				VSComponent directoryComponent = vSFolder.GetDirectoryComponent() ?? vSFolder.CreateDirectoryComponent();
				XmlElement xmlElement2 = Common.CreateXmlElementWithAttributes(directoryComponent.WiXElement.XmlNode.OwnerDocument, "RemoveFolder", directoryComponent.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "On", "Directory" }, new string[] { string.Concat("id_", Common.GenerateId(this._project.ProjectType)), "uninstall", vSFolder.Property }, "", false);
				directoryComponent.WiXElement.XmlNode.AppendChild(xmlElement2);
				vSFolder.WiXRemoveFolder = new WiXRemoveFolder(this._project, directoryComponent.WiXElement.Owner, directoryComponent.WiXElement, xmlElement2);
			}
			return vSFolder;
		}

		internal VSSpecialFolder AddSpecialFolder(SpecialFolderType folderType)
		{
			WiXDirectory wiXDirectory;
			if (this._owner != null)
			{
				return null;
			}
			VSBaseFolder vSBaseFolder = base.Find((VSBaseFolder e) => e.Property == folderType.ToString());
			if (vSBaseFolder != null)
			{
				if (vSBaseFolder is VSGACFolder)
				{
					return vSBaseFolder as VSGACFolder;
				}
				if (!(vSBaseFolder is VSSpecialFolder))
				{
					return null;
				}
				return vSBaseFolder as VSSpecialFolder;
			}
			VSBaseFolder item = base[base.Count - 1];
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(item.WiXElement.XmlNode.OwnerDocument, "Directory", item.WiXElement.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { folderType.ToString() }, "", false);
			if (base.Count != 1 || !(base[0].Property != SpecialFolderType.MergeRedirectFolder.ToString()))
			{
				item.WiXElement.XmlNode.ParentNode.AppendChild(xmlElement);
				wiXDirectory = new WiXDirectory(this._project, item.WiXElement.Owner, item.WiXElement.Parent, xmlElement);
			}
			else
			{
				item.WiXElement.XmlNode.AppendChild(xmlElement);
				wiXDirectory = new WiXDirectory(this._project, item.WiXElement.Owner, item.WiXElement, xmlElement);
			}
			wiXDirectory.Parent.SetDirty();
			VSSpecialFolder vSSpecialFolder = VSSpecialFolder.Create(this._project, this._owner, wiXDirectory);
			base.Add(vSSpecialFolder);
			return vSSpecialFolder;
		}

		internal VSWebCustomFolder AddWebCustomFolder()
		{
			string i;
			string j;
			if (this._owner != null)
			{
				return null;
			}
			int num = 1;
			for (i = string.Concat("Web Custom Folder #", num.ToString()); base.Find((VSBaseFolder e) => e.Name == i) != null; i = string.Concat("Web Custom Folder #", num.ToString()))
			{
				num++;
			}
			num = 1;
			for (j = string.Concat("NEWWEBPROPERTY", num.ToString()); base.Find((VSBaseFolder e) => e.Property == j) != null; j = string.Concat("NEWWEBPROPERTY", num.ToString()))
			{
				num++;
			}
			VSBaseFolder item = base[base.Count - 1];
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(item.WiXElement.XmlNode.OwnerDocument, "Directory", item.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Name" }, new string[] { j, i }, "", false);
			WiXDirectory wiXDirectory = null;
			if (base.Count != 1 || !(base[0].Property != SpecialFolderType.MergeRedirectFolder.ToString()))
			{
				item.WiXElement.XmlNode.ParentNode.AppendChild(xmlElement);
				wiXDirectory = new WiXDirectory(this._project, item.WiXElement.Owner, item.WiXElement.Parent, xmlElement);
			}
			else
			{
				item.WiXElement.XmlNode.AppendChild(xmlElement);
				wiXDirectory = new WiXDirectory(this._project, item.WiXElement.Owner, item.WiXElement, xmlElement);
			}
			if (wiXDirectory == null)
			{
				return null;
			}
			wiXDirectory.Parent.SetDirty();
			VSWebCustomFolder vSWebCustomFolder = new VSWebCustomFolder(this._project, this._owner, wiXDirectory)
			{
				Name = i,
				Property = j
			};
			base.Add(vSWebCustomFolder);
			VSComponent vSComponent = vSWebCustomFolder.CreateDirectoryComponent();
			XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(vSComponent.WiXElement.XmlNode.OwnerDocument, "CreateFolder", vSComponent.WiXElement.XmlNode.NamespaceURI, new string[] { "Directory" }, new string[] { vSWebCustomFolder.Property }, "", false);
			vSComponent.WiXElement.XmlNode.AppendChild(xmlElement1);
			vSWebCustomFolder.WiXCreateFolder = new WiXCreateFolder(this._project, vSComponent.WiXElement.Owner, vSComponent.WiXElement, xmlElement1);
			XmlElement xmlElement2 = Common.CreateXmlElementWithAttributes(vSComponent.WiXElement.XmlNode.OwnerDocument, "RemoveFolder", vSComponent.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "On", "Directory" }, new string[] { string.Concat("id_", Common.GenerateId(this._project.ProjectType)), "uninstall", vSWebCustomFolder.Property }, "", false);
			vSComponent.WiXElement.XmlNode.AppendChild(xmlElement2);
			vSWebCustomFolder.WiXRemoveFolder = new WiXRemoveFolder(this._project, vSComponent.WiXElement.Owner, vSComponent.WiXElement, xmlElement2);
			WiXEntity owner = item.WiXElement.Owner as WiXEntity;
			if (owner != null)
			{
				string empty = string.Empty;
				foreach (XmlAttribute attribute in owner.XmlNode.Attributes)
				{
					if (attribute.Value != IISSchema.TargetNamespace)
					{
						continue;
					}
					empty = attribute.LocalName;
					goto Label0;
				}
			Label0:
				string id = this._project.WebSetupParameters.WebSites[0].Id;
				string str = this._project.WebSetupParameters.AppPools[0].Id;
				XmlElement xmlElement3 = Common.CreateXmlElementWithAttributes(vSComponent.WiXElement.XmlNode.OwnerDocument, "WebVirtualDir", IISSchema.TargetNamespace, new string[] { "Id", "Alias", "WebSite", "Directory" }, new string[] { string.Concat("webdir_", j), i, id, j }, empty, false);
				vSComponent.WiXElement.XmlNode.AppendChild(xmlElement3);
				vSWebCustomFolder.WiXWebVirtualDir = new IISWebVirtualDir(this._project, vSComponent.WiXElement.Owner, vSComponent.WiXElement, xmlElement3);
				XmlElement xmlElement4 = Common.CreateXmlElementWithAttributes(vSWebCustomFolder.WiXWebVirtualDir.XmlNode.OwnerDocument, "WebDirProperties", IISSchema.TargetNamespace, new string[] { "Id", "Read", "Write", "Script", "Execute", "LogVisits", "Index", "DefaultDocuments" }, new string[] { string.Concat("webprop_", j), "yes", "no", "yes", "no", "yes", "yes", "default.aspx" }, empty, false);
				vSWebCustomFolder.WiXWebVirtualDir.XmlNode.AppendChild(xmlElement4);
				vSWebCustomFolder.WiXWebDirProperties = new IISWebDirProperties(this._project, vSWebCustomFolder.WiXWebVirtualDir.Owner, vSWebCustomFolder.WiXWebVirtualDir, xmlElement4);
				XmlElement xmlElement5 = Common.CreateXmlElementWithAttributes(vSWebCustomFolder.WiXWebVirtualDir.XmlNode.OwnerDocument, "WebApplication", IISSchema.TargetNamespace, new string[] { "Id", "Name", "Isolation", "WebAppPool" }, new string[] { string.Concat("webapp_", j), i, "medium", str }, empty, false);
				vSWebCustomFolder.WiXWebVirtualDir.XmlNode.AppendChild(xmlElement5);
				vSWebCustomFolder.WiXWebApplication = new IISWebApplication(this._project, vSWebCustomFolder.WiXWebVirtualDir.Owner, vSWebCustomFolder.WiXWebVirtualDir, xmlElement5);
			}
			VSWebFolder vSWebFolder = vSWebCustomFolder.Folders.AddWebFolder();
			vSWebFolder.Name = "bin";
			vSWebFolder.AllowReadAccess = false;
			wiXDirectory.SetDirty();
			return vSWebCustomFolder;
		}

		internal VSWebFolder AddWebFolder()
		{
			string i;
			if (this._owner == null)
			{
				return null;
			}
			int num = 1;
			for (i = string.Concat("New Web Folder #", num.ToString()); base.Find((VSBaseFolder e) => e.Name == i) != null; i = string.Concat("New Web Folder #", num.ToString()))
			{
				num++;
			}
			string str = string.Concat("dir_", Common.GenerateId(this._project.ProjectType));
			WiXDirectory wiXDirectory = null;
			if (this._owner == null)
			{
				VSBaseFolder item = base[base.Count - 1];
				XmlElement xmlElement = Common.CreateXmlElementWithAttributes(item.WiXElement.XmlNode.OwnerDocument, "Directory", item.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Name" }, new string[] { str, i }, "", false);
				item.WiXElement.XmlNode.ParentNode.AppendChild(xmlElement);
				wiXDirectory = new WiXDirectory(this._project, item.WiXElement.Owner, item.WiXElement.Parent, xmlElement);
			}
			else
			{
				XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(this._owner.WiXElement.XmlNode.OwnerDocument, "Directory", this._owner.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Name" }, new string[] { str, i }, "", false);
				this._owner.WiXElement.XmlNode.AppendChild(xmlElement1);
				wiXDirectory = new WiXDirectory(this._project, this._owner.WiXElement.Owner, this._owner.WiXElement, xmlElement1);
			}
			if (wiXDirectory == null)
			{
				return null;
			}
			wiXDirectory.Parent.SetDirty();
			VSWebFolder vSWebFolder = new VSWebFolder(this._project, this._owner, wiXDirectory)
			{
				Name = i,
				Property = str
			};
			base.Add(vSWebFolder);
			VSComponent vSComponent = vSWebFolder.CreateDirectoryComponent();
			XmlElement xmlElement2 = Common.CreateXmlElementWithAttributes(vSComponent.WiXElement.XmlNode.OwnerDocument, "CreateFolder", vSComponent.WiXElement.XmlNode.NamespaceURI, new string[] { "Directory" }, new string[] { vSWebFolder.Property }, "", false);
			vSComponent.WiXElement.XmlNode.AppendChild(xmlElement2);
			vSWebFolder.WiXCreateFolder = new WiXCreateFolder(this._project, vSComponent.WiXElement.Owner, vSComponent.WiXElement, xmlElement2);
			XmlElement xmlElement3 = Common.CreateXmlElementWithAttributes(vSComponent.WiXElement.XmlNode.OwnerDocument, "RemoveFolder", vSComponent.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "On", "Directory" }, new string[] { string.Concat("id_", Common.GenerateId(this._project.ProjectType)), "uninstall", vSWebFolder.Property }, "", false);
			vSComponent.WiXElement.XmlNode.AppendChild(xmlElement3);
			vSWebFolder.WiXRemoveFolder = new WiXRemoveFolder(this._project, vSComponent.WiXElement.Owner, vSComponent.WiXElement, xmlElement3);
			if (this._owner is VSWebFolder)
			{
				string empty = string.Empty;
				if ((this._owner as VSWebFolder).WiXWebDir != null)
				{
					empty = (this._owner as VSWebFolder).WiXWebDir.XmlNode.Prefix;
				}
				if ((this._owner as VSWebFolder).WiXWebVirtualDir != null)
				{
					empty = (this._owner as VSWebFolder).WiXWebVirtualDir.XmlNode.Prefix;
				}
				string id = this._project.WebSetupParameters.WebSites[0].Id;
				XmlElement xmlElement4 = Common.CreateXmlElementWithAttributes(vSComponent.WiXElement.XmlNode.OwnerDocument, "WebDir", IISSchema.TargetNamespace, new string[] { "Id", "Path", "WebSite" }, new string[] { string.Concat("webdir_", str), i, id }, empty, false);
				vSComponent.WiXElement.XmlNode.AppendChild(xmlElement4);
				vSWebFolder.WiXWebDir = new IISWebDir(this._project, vSComponent.WiXElement.Owner, vSComponent.WiXElement, xmlElement4);
				XmlElement xmlElement5 = Common.CreateXmlElementWithAttributes(vSWebFolder.WiXWebDir.XmlNode.OwnerDocument, "WebDirProperties", IISSchema.TargetNamespace, new string[] { "Id", "Read", "Write", "Script", "Execute", "LogVisits", "Index" }, new string[] { string.Concat("webprop_", str), "yes", "no", "yes", "no", "yes", "yes" }, empty, false);
				vSWebFolder.WiXWebDir.XmlNode.AppendChild(xmlElement5);
				vSWebFolder.WiXWebDirProperties = new IISWebDirProperties(this._project, vSWebFolder.WiXWebDir.Owner, vSWebFolder.WiXWebDir, xmlElement5);
			}
			wiXDirectory.SetDirty();
			return vSWebFolder;
		}

		internal List<string> GetAllComponentIDs()
		{
			return this.GetAllComponentIDs(this);
		}

		private List<string> GetAllComponentIDs(VSFolders folders)
		{
			List<string> strs = new List<string>();
			foreach (VSBaseFolder folder in folders)
			{
				foreach (VSComponent component in folder.Components)
				{
					if (strs.Contains(component.WiXElement.Id))
					{
						continue;
					}
					strs.Add(component.WiXElement.Id);
				}
				foreach (VSProjectOutputUnknown projectOutput in folder.ProjectOutputs)
				{
					if (!(projectOutput is VSProjectOutputVDProj) || string.IsNullOrEmpty((projectOutput as VSProjectOutputVDProj).FileId) || strs.Contains(string.Concat("com", (projectOutput as VSProjectOutputVDProj).FileId)))
					{
						continue;
					}
					strs.Add(string.Concat("com", (projectOutput as VSProjectOutputVDProj).FileId));
				}
				if (folder.Folders.Count <= 0)
				{
					continue;
				}
				strs.AddRange(this.GetAllComponentIDs(folder.Folders));
			}
			return strs;
		}

		internal VSComponent GetComponentById(string id)
		{
			return this.GetComponentById(this, id);
		}

		private VSComponent GetComponentById(VSFolders folders, string id)
		{
			VSComponent vSComponent;
			List<VSBaseFolder>.Enumerator enumerator = folders.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					VSBaseFolder current = enumerator.Current;
					List<VSComponent>.Enumerator enumerator1 = current.Components.GetEnumerator();
					try
					{
						while (enumerator1.MoveNext())
						{
							VSComponent current1 = enumerator1.Current;
							if (current1.Id != id)
							{
								continue;
							}
							vSComponent = current1;
							return vSComponent;
						}
					}
					finally
					{
						((IDisposable)enumerator1).Dispose();
					}
					if (current.Folders.Count <= 0)
					{
						continue;
					}
					VSComponent componentById = this.GetComponentById(current.Folders, id);
					if (componentById == null)
					{
						continue;
					}
					vSComponent = componentById;
					return vSComponent;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return vSComponent;
		}

		internal VSBaseFile GetFileById(string id)
		{
			return this.GetFileById(this, id);
		}

		private VSBaseFile GetFileById(VSFolders folders, string id)
		{
			VSBaseFile vSBaseFile;
			List<VSBaseFolder>.Enumerator enumerator = folders.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					VSBaseFolder current = enumerator.Current;
					List<VSBaseFile>.Enumerator enumerator1 = current.Files.GetEnumerator();
					try
					{
						while (enumerator1.MoveNext())
						{
							VSBaseFile current1 = enumerator1.Current;
							if (!(current1 is VSFile) || !((current1 as VSFile).WiXElement.Id == id))
							{
								if (!(current1 is VSProjectOutputVDProj) || !((current1 as VSProjectOutputVDProj).FileId == id))
								{
									continue;
								}
								vSBaseFile = current1;
								return vSBaseFile;
							}
							else
							{
								vSBaseFile = current1;
								return vSBaseFile;
							}
						}
					}
					finally
					{
						((IDisposable)enumerator1).Dispose();
					}
					if (current.Folders.Count <= 0)
					{
						continue;
					}
					VSBaseFile fileById = this.GetFileById(current.Folders, id);
					if (fileById == null)
					{
						continue;
					}
					vSBaseFile = fileById;
					return vSBaseFile;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return vSBaseFile;
		}

		internal VSBaseFolder GetFolderById(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return null;
			}
			return this.GetFolderById(this, id);
		}

		private VSBaseFolder GetFolderById(VSFolders folders, string id)
		{
			VSBaseFolder vSBaseFolder;
			List<VSBaseFolder>.Enumerator enumerator = folders.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					VSBaseFolder current = enumerator.Current;
					if (current.Property != id)
					{
						if (current.Folders.Count <= 0)
						{
							continue;
						}
						VSBaseFolder folderById = this.GetFolderById(current.Folders, id);
						if (folderById == null)
						{
							continue;
						}
						vSBaseFolder = folderById;
						return vSBaseFolder;
					}
					else
					{
						vSBaseFolder = current;
						return vSBaseFolder;
					}
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return vSBaseFolder;
		}

		internal VSProjectOutputUnknown GetProjectOutputById(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return null;
			}
			return this.GetProjectOutputById(this, id);
		}

		private VSProjectOutputUnknown GetProjectOutputById(VSFolders folders, string id)
		{
			VSProjectOutputUnknown vSProjectOutputUnknown;
			List<VSBaseFolder>.Enumerator enumerator = folders.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					VSBaseFolder current = enumerator.Current;
					List<VSProjectOutputUnknown>.Enumerator enumerator1 = current.ProjectOutputs.GetEnumerator();
					try
					{
						while (enumerator1.MoveNext())
						{
							VSProjectOutputUnknown current1 = enumerator1.Current;
							if (current1.WiXElement.Id != id)
							{
								continue;
							}
							vSProjectOutputUnknown = current1;
							return vSProjectOutputUnknown;
						}
					}
					finally
					{
						((IDisposable)enumerator1).Dispose();
					}
					if (current.Folders.Count <= 0)
					{
						continue;
					}
					VSProjectOutputUnknown projectOutputById = this.GetProjectOutputById(current.Folders, id);
					if (projectOutputById == null)
					{
						continue;
					}
					vSProjectOutputUnknown = projectOutputById;
					return vSProjectOutputUnknown;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return vSProjectOutputUnknown;
		}
	}
}