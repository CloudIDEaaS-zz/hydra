using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXEntity : IWiXEntity
	{
		private WiXProjectParser _project;

		private IWiXEntity _owner;

		private IWiXEntity _parent;

		protected string _name = string.Empty;

		protected System.Xml.XmlNode _xmlNode;

		private WiXEntityList _childEntities;

		private bool dirty;

		public virtual WiXEntityList ChildEntities
		{
			get
			{
				return this._childEntities;
			}
		}

		public virtual IWiXEntity FirstChild
		{
			get
			{
				if (this._childEntities.Count <= 0)
				{
					return null;
				}
				return this._childEntities[0];
			}
		}

		public virtual bool HasChildEntities
		{
			get
			{
				return this._childEntities.Count > 0;
			}
		}

		public virtual bool IsDirty
		{
			get
			{
				return this.dirty;
			}
			set
			{
				this.dirty = value;
			}
		}

		public virtual bool IsSupported
		{
			get
			{
				return this.SupportedObject != null;
			}
		}

		public virtual IWiXEntity LastChild
		{
			get
			{
				if (this._childEntities.Count <= 0)
				{
					return null;
				}
				return this._childEntities[this._childEntities.Count - 1];
			}
		}

		public virtual string Name
		{
			get
			{
				return this._name;
			}
		}

		public virtual IWiXEntity NextSibling
		{
			get
			{
				if (this._parent == null)
				{
					return null;
				}
				int num = this._parent.ChildEntities.IndexOf(this);
				if (num >= this._parent.ChildEntities.Count - 1)
				{
					return null;
				}
				return this._parent.ChildEntities[num + 1];
			}
		}

		public virtual IWiXEntity Owner
		{
			get
			{
				return this._owner;
			}
		}

		public virtual IWiXEntity Parent
		{
			get
			{
				return this._parent;
			}
			internal set
			{
				this._parent = value;
			}
		}

		public virtual IWiXEntity PreviousSibling
		{
			get
			{
				if (this._parent == null)
				{
					return null;
				}
				int num = this._parent.ChildEntities.IndexOf(this);
				if (num <= 0)
				{
					return null;
				}
				return this._parent.ChildEntities[num - 1];
			}
		}

		internal WiXProjectParser Project
		{
			get
			{
				return this._project;
			}
		}

		public virtual object SupportedObject
		{
			get
			{
				return null;
			}
		}

		internal System.Xml.XmlNode XmlNode
		{
			get
			{
				return this._xmlNode;
			}
			set
			{
				this._xmlNode = value;
			}
		}

		internal WiXEntity(WiXProjectParser project)
		{
			this._project = project;
			this._childEntities = new WiXEntityList(this);
		}

		internal WiXEntity(WiXProjectParser project, IWiXEntity owner, IWiXEntity parent, System.Xml.XmlNode xmlNode) : this(project)
		{
			this._owner = owner;
			this._parent = parent;
			this._name = xmlNode.LocalName;
			this._xmlNode = xmlNode;
			if (this._parent != null)
			{
				this._parent.ChildEntities.Add(this);
			}
		}

		internal virtual void Delete()
		{
			if (this.Parent != null)
			{
				this.Parent.ChildEntities.Remove(this);
			}
			if (this.XmlNode != null && this.XmlNode.ParentNode != null)
			{
				this.XmlNode.ParentNode.RemoveChild(this.XmlNode);
			}
			if (this.Parent != null)
			{
				this.Parent.SetDirty();
			}
		}

		internal string GetAttributeValue(string name)
		{
			if (this._xmlNode != null)
			{
				XmlAttribute itemOf = this._xmlNode.Attributes[name];
				if (itemOf != null)
				{
					return itemOf.Value;
				}
			}
			return null;
		}

		internal string GetAttributeValue(string name, string namespaceURI)
		{
			if (this._xmlNode != null)
			{
				XmlAttribute itemOf = this._xmlNode.Attributes[name, namespaceURI];
				if (itemOf != null)
				{
					return itemOf.Value;
				}
			}
			return null;
		}

		internal void RebuildXmlNodes(XmlDocument newDocument, IWiXEntity newOwner)
		{
			this._owner = newOwner;
			this._xmlNode = newDocument.ImportNode(this._xmlNode, false);
			this.RebuildXmlNodes(this, this.ChildEntities, newDocument, newOwner);
		}

		private void RebuildXmlNodes(WiXEntity parent, WiXEntityList childEntities, XmlDocument newDocument, IWiXEntity newOwner)
		{
			if (childEntities.Count == 0)
			{
				return;
			}
			foreach (WiXEntity childEntity in childEntities)
			{
				childEntity._owner = newOwner;
				childEntity._xmlNode = newDocument.ImportNode(childEntity._xmlNode, false);
				parent._xmlNode.AppendChild(childEntity._xmlNode);
				this.RebuildXmlNodes(childEntity, childEntity.ChildEntities, newDocument, newOwner);
			}
		}

		internal void SetAttributeValue(string name, string value)
		{
			if (this._xmlNode != null)
			{
				XmlAttribute itemOf = this._xmlNode.Attributes[name];
				if (itemOf != null)
				{
					if (!string.IsNullOrEmpty(value))
					{
						itemOf.Value = value;
						return;
					}
					this._xmlNode.Attributes.Remove(itemOf);
					return;
				}
				if (value != null)
				{
					itemOf = this._xmlNode.OwnerDocument.CreateAttribute(name);
					if (itemOf != null)
					{
						itemOf.Value = value;
						this._xmlNode.Attributes.Append(itemOf);
					}
				}
			}
		}

		internal void SetAttributeValue(string name, string value, bool allowEmptyValue)
		{
			if (this._xmlNode != null)
			{
				XmlAttribute itemOf = this._xmlNode.Attributes[name];
				if (itemOf != null)
				{
					if (allowEmptyValue)
					{
						if (value != null)
						{
							itemOf.Value = value;
							return;
						}
						this._xmlNode.Attributes.Remove(itemOf);
						return;
					}
					if (!string.IsNullOrEmpty(value))
					{
						itemOf.Value = value;
						return;
					}
					this._xmlNode.Attributes.Remove(itemOf);
					return;
				}
				if (value != null)
				{
					itemOf = this._xmlNode.OwnerDocument.CreateAttribute(name);
					if (itemOf != null)
					{
						itemOf.Value = value;
						this._xmlNode.Attributes.Append(itemOf);
					}
				}
			}
		}

		internal void SetAttributeValue(string name, string namespaceURI, string value)
		{
			if (this._xmlNode != null)
			{
				XmlAttribute itemOf = this._xmlNode.Attributes[name, namespaceURI];
				if (itemOf != null)
				{
					if (value != null)
					{
						itemOf.Value = value;
						return;
					}
					this._xmlNode.Attributes.Remove(itemOf);
					return;
				}
				if (value != null)
				{
					if (this._xmlNode.OwnerDocument.DocumentElement.LocalName.ToLower() == "wix")
					{
						bool flag = false;
						foreach (object attribute in this._xmlNode.OwnerDocument.DocumentElement.Attributes)
						{
							if (((XmlAttribute)attribute).Value != namespaceURI)
							{
								continue;
							}
							flag = true;
							goto Label0;
						}
					Label0:
						if (!flag)
						{
							XmlAttribute xmlAttribute = this._xmlNode.OwnerDocument.CreateAttribute("xmlns:adx");
							xmlAttribute.Value = namespaceURI;
							this._xmlNode.OwnerDocument.DocumentElement.Attributes.Append(xmlAttribute);
						}
					}
					itemOf = this._xmlNode.OwnerDocument.CreateAttribute(name, namespaceURI);
					if (itemOf != null)
					{
						itemOf.Value = value;
						this._xmlNode.Attributes.Append(itemOf);
					}
				}
			}
		}

		public virtual void SetDirty()
		{
			if (this._project.ViewManager != null)
			{
				if (this is WiXProjectItem)
				{
					this._project.ViewManager.OnXmlDocumentContentChanged((this as WiXProjectItem).SourceFile, (this as WiXProjectItem).SourceDocument);
				}
				else if (!(this.Owner is WiXProjectItem))
				{
					foreach (WiXProjectItem projectItem in this._project.ProjectItems)
					{
						this._project.ViewManager.OnXmlDocumentContentChanged(projectItem.SourceFile, projectItem.SourceDocument);
					}
				}
				else
				{
					this._project.ViewManager.OnXmlDocumentContentChanged((this.Owner as WiXProjectItem).SourceFile, (this.Owner as WiXProjectItem).SourceDocument);
				}
			}
			if (this._owner != null)
			{
				this._owner.IsDirty = true;
			}
		}
	}
}