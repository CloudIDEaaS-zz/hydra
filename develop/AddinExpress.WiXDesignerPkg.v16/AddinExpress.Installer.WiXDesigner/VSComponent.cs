using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSComponent : VSComponentBase
	{
		private WiXComponent _wixElement;

		private AddinExpress.Installer.WiXDesigner.WiXCondition _wixCondition;

		private WiXRegistryValue _wixKeyPathElement;

		private VSBaseFolder _parent;

		private VSFiles _files;

		private List<WiXComponentRef> _componentRefs;

		internal bool CanBeDeleted
		{
			get
			{
				if (this.WiXElement.ChildEntities.Count == 0)
				{
					return true;
				}
				if (this.WiXElement.ChildEntities.Count == 1)
				{
					if (this.WiXCondition != null)
					{
						return true;
					}
					if (this.WiXKeyPathElement != null)
					{
						return true;
					}
				}
				if (this.WiXElement.ChildEntities.Count == 2 && this.WiXCondition != null && this.WiXKeyPathElement != null)
				{
					return true;
				}
				return false;
			}
		}

		internal List<WiXComponentRef> ComponentRefs
		{
			get
			{
				return this._componentRefs;
			}
		}

		[Browsable(false)]
		public VSFiles Files
		{
			get
			{
				return this._files;
			}
		}

		internal string Id
		{
			get
			{
				if (this._wixElement == null)
				{
					return string.Empty;
				}
				return this._wixElement.Id;
			}
			set
			{
				if (this._wixElement != null)
				{
					this._wixElement.Id = value;
				}
				if (this._componentRefs != null)
				{
					foreach (WiXComponentRef _componentRef in this._componentRefs)
					{
						_componentRef.Id = value;
					}
				}
			}
		}

		[Browsable(false)]
		public VSBaseFolder Parent
		{
			get
			{
				return this._parent;
			}
			internal set
			{
				if (this._parent != null)
				{
					this._parent.Components.Remove(this);
				}
				this._parent = value;
				if (this._parent != null)
				{
					this._parent.Components.Add(this);
				}
			}
		}

		internal AddinExpress.Installer.WiXDesigner.WiXCondition WiXCondition
		{
			get
			{
				return this._wixCondition;
			}
			set
			{
				this._wixCondition = value;
			}
		}

		internal WiXComponent WiXElement
		{
			get
			{
				return this._wixElement;
			}
		}

		internal WiXRegistryValue WiXKeyPathElement
		{
			get
			{
				return this._wixKeyPathElement;
			}
			set
			{
				this._wixKeyPathElement = value;
			}
		}

		public VSComponent()
		{
			this._componentRefs = new List<WiXComponentRef>();
		}

		public VSComponent(VSBaseFolder parent, WiXComponent wixElement) : this()
		{
			this._files = new VSFiles(parent._project, parent);
			this._parent = parent;
			this._wixElement = wixElement;
		}

		internal void AdjustDirectoryKeyPath()
		{
			if (!VSSpecialFolder.CheckForUserFolder(this._parent))
			{
				this.DeleteWiXKeyPathElement();
				if (this._parent.WiXRemoveFolder != null && this._parent.WiXCreateFolder == null)
				{
					this._parent.WiXRemoveFolder.Delete();
					this._parent.WiXRemoveFolder = null;
				}
			}
			else
			{
				this.CreateWiXKeyPathElement();
				if (this._parent.WiXRemoveFolder == null)
				{
					XmlElement xmlElement = Common.CreateXmlElementWithAttributes(this.WiXElement.XmlNode.OwnerDocument, "RemoveFolder", this.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "On", "Directory" }, new string[] { string.Concat("id_", Common.GenerateId(this._parent._project.ProjectType)), "uninstall", this._parent.Property }, "", false);
					this.WiXElement.XmlNode.AppendChild(xmlElement);
					this._parent.WiXRemoveFolder = new WiXRemoveFolder(this._parent._project, this.WiXElement.Owner, this.WiXElement, xmlElement);
				}
			}
			this.WiXElement.SetDirty();
		}

		internal void AdjustKeyPath()
		{
			if (!VSSpecialFolder.CheckForUserFolder(this._parent))
			{
				this.DeleteWiXKeyPathElement();
				this.WiXElement.KeyPath = null;
				int num = 0;
				int num1 = 0;
				bool flag = false;
				foreach (VSBaseFile _file in this._files)
				{
					if (!(_file is VSFile))
					{
						continue;
					}
					num++;
					if ((_file as VSFile).WiXElement.KeyPath != "yes")
					{
						continue;
					}
					num1++;
					if (flag)
					{
						continue;
					}
					flag = true;
				}
				if (!flag && this.WiXElement.ChildEntities.Count > 0)
				{
					WiXRegistryKey wiXRegistryKey = this.WiXElement.ChildEntities.Find((WiXEntity x) => x is WiXRegistryKey) as WiXRegistryKey;
					if (wiXRegistryKey != null && wiXRegistryKey.ChildEntities.Count > 0)
					{
						WiXRegistryValue wiXRegistryValue = wiXRegistryKey.ChildEntities.Find((WiXEntity x) => x is WiXRegistryValue) as WiXRegistryValue;
						if (wiXRegistryValue != null)
						{
							flag = (string.IsNullOrEmpty(wiXRegistryValue.KeyPath) ? false : wiXRegistryValue.KeyPath == "yes");
						}
					}
				}
				if (!flag)
				{
					if (num <= 0)
					{
						this.WiXElement.KeyPath = "yes";
					}
					else
					{
						foreach (VSBaseFile vSBaseFile in this._files)
						{
							if (!(vSBaseFile is VSFile))
							{
								continue;
							}
							(vSBaseFile as VSFile).WiXElement.KeyPath = "yes";
							this.WiXElement.SetDirty();
							return;
						}
					}
				}
				else if (num1 > 1)
				{
					for (int i = this._files.Count - 1; i >= 0; i--)
					{
						if (this._files[i] is VSFile && (this._files[i] as VSFile).WiXElement.KeyPath == "yes")
						{
							(this._files[i] as VSFile).WiXElement.KeyPath = null;
							int num2 = num1 - 1;
							num1 = num2;
							if (num2 == 1)
							{
								break;
							}
						}
					}
				}
			}
			else
			{
				this.WiXElement.KeyPath = null;
				this.CreateWiXKeyPathElement();
				foreach (VSBaseFile _file1 in this._files)
				{
					if (!(_file1 is VSFile))
					{
						continue;
					}
					(_file1 as VSFile).WiXElement.KeyPath = null;
				}
			}
			this.WiXElement.SetDirty();
		}

		internal void CreateWiXCondition()
		{
			if (this._wixCondition == null)
			{
				XmlNode xmlNodes = Common.CreateXmlElementWithAttributes(this.WiXElement.XmlNode.OwnerDocument, "Condition", this.WiXElement.XmlNode.NamespaceURI, new string[0], new string[0], "", false);
				this.WiXElement.XmlNode.AppendChild(xmlNodes);
				this._wixCondition = new AddinExpress.Installer.WiXDesigner.WiXCondition(this.WiXElement.Project, this.WiXElement.Owner, this.WiXElement, xmlNodes);
				this.WiXElement.SetDirty();
			}
		}

		internal void CreateWiXKeyPathElement()
		{
			if (this._wixKeyPathElement == null)
			{
				XmlDocument ownerDocument = this.WiXElement.XmlNode.OwnerDocument;
				string namespaceURI = this.WiXElement.XmlNode.NamespaceURI;
				string[] strArrays = new string[] { "Root", "Key", "Name", "Type", "Value", "KeyPath" };
				string[] id = new string[] { "HKCU", "Software\\[Manufacturer]\\[ProductName]\\Installer", this._wixElement.Id, "string", null, null };
				id[4] = (this._parent != null ? string.Concat(this._parent.Name, " directory") : this._wixElement.Id);
				id[5] = "yes";
				XmlElement xmlElement = Common.CreateXmlElementWithAttributes(ownerDocument, "RegistryValue", namespaceURI, strArrays, id, "", false);
				this.WiXElement.XmlNode.AppendChild(xmlElement);
				this._wixKeyPathElement = new WiXRegistryValue(this.WiXElement.Project, this.WiXElement.Owner, this.WiXElement, xmlElement);
				this.WiXElement.SetDirty();
			}
		}

		public override void Delete()
		{
			if (this._parent != null && this._parent.Components.Contains(this))
			{
				this._parent.Components.Remove(this);
			}
			if (this._files != null)
			{
				while (this._files.Count > 0)
				{
					this._files[0].Delete();
				}
				this._files.Clear();
			}
			if (this.WiXElement != null)
			{
				this.WiXElement.Delete();
			}
			if (this._componentRefs != null)
			{
				foreach (WiXComponentRef _componentRef in this._componentRefs)
				{
					_componentRef.Delete();
				}
				this._componentRefs.Clear();
			}
		}

		internal void DeleteWiXCondition()
		{
			if (this._wixCondition != null)
			{
				this._wixCondition.Delete();
				this._wixCondition = null;
			}
		}

		internal void DeleteWiXKeyPathElement()
		{
			if (this._wixKeyPathElement != null)
			{
				this._wixKeyPathElement.Delete();
				this._wixKeyPathElement = null;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._files.Clear();
				this._files = null;
				this._componentRefs.Clear();
				this._componentRefs = null;
				this._parent = null;
				this._wixElement = null;
				this._wixCondition = null;
				this._wixKeyPathElement = null;
			}
			base.Dispose(disposing);
		}

		internal void MoveTo(VSBaseFolder destinationFolder)
		{
			bool flag;
			if (this.Parent != destinationFolder)
			{
				flag = (destinationFolder.Components.Count != 0 ? false : destinationFolder.Files.Count == 0);
				this.Parent = destinationFolder;
				this.WiXElement.Parent.ChildEntities.Remove(this.WiXElement);
				this.WiXElement.XmlNode.ParentNode.RemoveChild(this.WiXElement.XmlNode);
				this.WiXElement.Parent.SetDirty();
				if (this.WiXElement.Owner != destinationFolder.WiXElement.Owner)
				{
					this.WiXElement.RebuildXmlNodes(destinationFolder.WiXElement.XmlNode.OwnerDocument, destinationFolder.WiXElement.Owner);
				}
				if (!flag)
				{
					destinationFolder.WiXElement.ChildEntities.Add(this.WiXElement);
					destinationFolder.WiXElement.XmlNode.AppendChild(this.WiXElement.XmlNode);
				}
				else
				{
					destinationFolder.WiXElement.ChildEntities.Insert(0, this.WiXElement);
					if (!destinationFolder.WiXElement.XmlNode.HasChildNodes)
					{
						destinationFolder.WiXElement.XmlNode.AppendChild(this.WiXElement.XmlNode);
					}
					else
					{
						destinationFolder.WiXElement.XmlNode.InsertBefore(this.WiXElement.XmlNode, destinationFolder.WiXElement.XmlNode.ChildNodes[0]);
					}
				}
				if (!string.IsNullOrEmpty(this.WiXElement.Directory))
				{
					this.WiXElement.Directory = string.Empty;
				}
				this.WiXElement.Parent = destinationFolder.WiXElement;
				this.WiXElement.Parent.SetDirty();
				this.AdjustKeyPath();
				foreach (VSBaseFile file in this.Files)
				{
					if (file is VSShortcut)
					{
						(file as VSShortcut).AdjustDirectories();
					}
					if (!(file is VSAssembly))
					{
						continue;
					}
					if (!(destinationFolder is VSGACFolder))
					{
						(file as VSAssembly).WiXElement.Assembly = null;
						(file as VSAssembly).WiXElement.AssemblyApplication = null;
						(file as VSAssembly).WiXElement.AssemblyManifest = null;
					}
					else
					{
						(file as VSAssembly).WiXElement.Assembly = ".net";
					}
				}
				if (destinationFolder._project.Features.Count > 0)
				{
					bool flag1 = false;
					foreach (VSFeature feature in destinationFolder._project.Features)
					{
						if (feature.WiXElement.ChildEntities.Find((WiXEntity e) => {
							if (!(e is WiXComponentRef))
							{
								return false;
							}
							return (e as WiXComponentRef).Id == this.Id;
						}) == null)
						{
							continue;
						}
						flag1 = true;
						goto Label0;
					}
				Label0:
					if (!flag1)
					{
						WiXFeature wiXElement = destinationFolder._project.Features[0].WiXElement;
						XmlNode xmlNodes = Common.CreateXmlElementWithAttributes(wiXElement.XmlNode.OwnerDocument, "ComponentRef", wiXElement.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { this.Id }, "", false);
						wiXElement.XmlNode.AppendChild(xmlNodes);
						WiXComponentRef wiXComponentRef = new WiXComponentRef(wiXElement.Project, wiXElement.Owner, wiXElement, xmlNodes);
						wiXElement.SetDirty();
						this._componentRefs.Add(wiXComponentRef);
					}
				}
			}
		}
	}
}