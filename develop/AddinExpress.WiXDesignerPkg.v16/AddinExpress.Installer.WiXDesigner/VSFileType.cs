using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSFileType : VSComponentBase
	{
		private WiXProjectParser _project;

		private VSComponent _parentComponent;

		private VSProjectOutputVDProj _parentOutput;

		internal VsWiXProject.ReferenceDescriptor.FileType _parentFileType;

		private WiXProgId _wixElement;

		private VSFileTypeVerbs _verbs;

		private List<WiXExtension> _wixExtensions;

		internal override bool CanDelete
		{
			get
			{
				return true;
			}
		}

		internal override bool CanRename
		{
			get
			{
				return true;
			}
		}

		[Browsable(true)]
		[DefaultValue(null)]
		[Description("Specifies the executable file that is launched when an action is invoked on the selected file type")]
		[Editor(typeof(TargetPropertyEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(CommandPropertyConverter))]
		public object Command
		{
			get
			{
				if (this.ParentOutput != null)
				{
					return this.ParentOutput.KeyOutput.TargetName;
				}
				if (this.ParentComponent != null)
				{
					if (this.ParentComponent.Files.Count > 0)
					{
						if (this.ParentComponent.Files[0] is VSFile)
						{
							return (this.ParentComponent.Files[0] as VSFile).WiXElement.Id;
						}
					}
					else if (this.Verbs.Count > 0 && this.Verbs[0].WiXElements.Count > 0)
					{
						if (!string.IsNullOrEmpty(this.Verbs[0].WiXElements[0].TargetFile))
						{
							return this.Verbs[0].WiXElements[0].TargetFile;
						}
						if (!string.IsNullOrEmpty(this.Verbs[0].WiXElements[0].TargetProperty))
						{
							return this.Verbs[0].WiXElements[0].TargetProperty;
						}
					}
				}
				return string.Empty;
			}
			set
			{
				object obj;
				object obj1;
				if (value != null)
				{
					if (value is VSProjectOutputVDProj)
					{
						if (this.ParentOutput == value)
						{
							return;
						}
						if (this.ParentOutput != null)
						{
							List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes = new List<VsWiXProject.ReferenceDescriptor.FileType>();
							this.CombineVDProjFileTypes(ref fileTypes);
							VsWiXProject.ReferenceDescriptor referenceDescriptor = this.ParentOutput.ReferenceDescriptor;
							OutputGroup group = this.ParentOutput.Group;
							if (fileTypes.Count == 0)
							{
								obj1 = null;
							}
							else
							{
								obj1 = fileTypes;
							}
							referenceDescriptor.SetProjectOutputProperty(group, OutputGroupProperties.FileTypes, obj1);
							this._parentOutput = value as VSProjectOutputVDProj;
							fileTypes = new List<VsWiXProject.ReferenceDescriptor.FileType>();
							this.CombineVDProjFileTypes(ref fileTypes);
							fileTypes.Add(this._parentFileType);
							this.ParentOutput.ReferenceDescriptor.SetProjectOutputProperty(this.ParentOutput.Group, OutputGroupProperties.FileTypes, fileTypes);
						}
						else
						{
							this._parentFileType = new VsWiXProject.ReferenceDescriptor.FileType(this.Name, this.Description, this.Extensions, this.Icon, this.MIME);
							for (int i = 0; i < this.Verbs.Count; i++)
							{
								string name = this._verbs[i].Name;
								string arguments = this._verbs[i].Arguments;
								string verb = this._verbs[i].Verb;
								int sequence = this._verbs[i].Sequence;
								this._parentFileType.AddVerb(new VsWiXProject.ReferenceDescriptor.FileTypeVerb(name, arguments, verb, sequence.ToString()));
							}
							this._parentOutput = value as VSProjectOutputVDProj;
							List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes1 = new List<VsWiXProject.ReferenceDescriptor.FileType>();
							this.CombineVDProjFileTypes(ref fileTypes1);
							fileTypes1.Add(this._parentFileType);
							this.ParentOutput.ReferenceDescriptor.SetProjectOutputProperty(this.ParentOutput.Group, OutputGroupProperties.FileTypes, fileTypes1);
							this._wixElement.Delete();
							this._wixElement = null;
							this._wixExtensions.Clear();
							this._wixExtensions = null;
							this._parentComponent = null;
						}
					}
					if (value is VSFile)
					{
						if (this.ParentComponent == (value as VSFile).ParentComponent)
						{
							return;
						}
						if (this.ParentComponent != null)
						{
							this.MoveTo((value as VSFile).ParentComponent);
						}
						else
						{
							List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes2 = new List<VsWiXProject.ReferenceDescriptor.FileType>();
							this.CombineVDProjFileTypes(ref fileTypes2);
							VsWiXProject.ReferenceDescriptor referenceDescriptor1 = this.ParentOutput.ReferenceDescriptor;
							OutputGroup outputGroup = this.ParentOutput.Group;
							if (fileTypes2.Count == 0)
							{
								obj = null;
							}
							else
							{
								obj = fileTypes2;
							}
							referenceDescriptor1.SetProjectOutputProperty(outputGroup, OutputGroupProperties.FileTypes, obj);
							this._parentOutput = null;
							this._parentComponent = (value as VSFile).ParentComponent;
							XmlNode xmlNodes = Common.CreateXmlElementWithAttributes(this.ParentComponent.WiXElement.XmlNode.OwnerDocument, "ProgId", this.ParentComponent.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Description", "Icon", "Advertise" }, new string[] { this._parentFileType.Name, this._parentFileType.Description, this._parentFileType.Icon, "yes" }, "", false);
							this.ParentComponent.WiXElement.XmlNode.InsertAfter(xmlNodes, (value as VSFile).WiXElement.XmlNode);
							this._wixElement = new WiXProgId(this.Project, this.ParentComponent.WiXElement.Owner, this.ParentComponent.WiXElement, xmlNodes);
							this._wixExtensions = new List<WiXExtension>();
							List<string> strs = new List<string>(this._parentFileType.Extensions.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
							if (strs != null && strs.Count > 0)
							{
								foreach (string str in strs)
								{
									XmlNode xmlNodes1 = Common.CreateXmlElementWithAttributes(this.WiXElement.XmlNode.OwnerDocument, "Extension", this.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "ContentType" }, new string[] { str, this._parentFileType.MIME }, "", false);
									this.WiXElement.XmlNode.AppendChild(xmlNodes1);
									this.WiXElement.SetDirty();
									WiXExtension wiXExtension = new WiXExtension(this.Project, this.WiXElement.Owner, this.WiXElement, xmlNodes1);
									this.WiXExtensions.Add(wiXExtension);
									for (int j = 0; j < this._parentFileType.Verbs.Count; j++)
									{
										this.Verbs[j]._verb = null;
										if (this.Verbs[j]._wixElements == null)
										{
											this.Verbs[j]._wixElements = new List<WiXVerb>();
										}
										xmlNodes1 = Common.CreateXmlElementWithAttributes(this.WiXElement.XmlNode.OwnerDocument, "Verb", this.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Command", "Argument", "Sequence" }, new string[] { this._parentFileType.Verbs[j].Verb, this._parentFileType.Verbs[j].Name, this._parentFileType.Verbs[j].Arguments, this._parentFileType.Verbs[j].Sequence }, "", false);
										wiXExtension.XmlNode.AppendChild(xmlNodes1);
										wiXExtension.SetDirty();
										WiXVerb wiXVerb = new WiXVerb(this.Project, wiXExtension.Owner, wiXExtension, xmlNodes1);
										this.Verbs[j].WiXElements.Add(wiXVerb);
									}
								}
							}
						}
					}
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the description for the selected file type")]
		public string Description
		{
			get
			{
				if (this.ParentOutput != null)
				{
					return this._parentFileType.Description;
				}
				if (this.WiXElement == null)
				{
					return string.Empty;
				}
				return this.WiXElement.Description;
			}
			set
			{
				if (this.ParentOutput != null)
				{
					this._parentFileType.Description = value;
					List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes = new List<VsWiXProject.ReferenceDescriptor.FileType>();
					this.CombineVDProjFileTypes(ref fileTypes);
					fileTypes.Add(this._parentFileType);
					this.ParentOutput.ReferenceDescriptor.SetProjectOutputProperty(this.ParentOutput.Group, OutputGroupProperties.FileTypes, fileTypes);
				}
				if (this.ParentComponent != null)
				{
					this.WiXElement.Description = value;
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies one or more file extensions to be associated with the selected file type")]
		[MergableProperty(false)]
		public string Extensions
		{
			get
			{
				if (this.ParentOutput != null)
				{
					return this._parentFileType.Extensions;
				}
				if (this.ParentComponent == null)
				{
					return string.Empty;
				}
				string empty = string.Empty;
				List<string> strs = new List<string>();
				foreach (WiXExtension wiXExtension in this.WiXExtensions)
				{
					if (strs.Contains(wiXExtension.Id))
					{
						continue;
					}
					strs.Add(wiXExtension.Id);
				}
				if (strs.Count > 0)
				{
					foreach (string str in strs)
					{
						empty = (!string.IsNullOrEmpty(empty) ? string.Concat(empty, ";", str) : string.Concat(empty, str));
					}
				}
				return empty;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}
				value = value.Trim().Replace(" ", "");
				if (string.IsNullOrEmpty(value))
				{
					return;
				}
				if (this.ParentOutput != null)
				{
					this._parentFileType.Extensions = value;
					List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes = new List<VsWiXProject.ReferenceDescriptor.FileType>();
					this.CombineVDProjFileTypes(ref fileTypes);
					fileTypes.Add(this._parentFileType);
					this.ParentOutput.ReferenceDescriptor.SetProjectOutputProperty(this.ParentOutput.Group, OutputGroupProperties.FileTypes, fileTypes);
				}
				if (this.ParentComponent != null)
				{
					List<string> strs = new List<string>(value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
					if (strs != null && strs.Count > 0)
					{
						foreach (string str in strs)
						{
							WiXExtension wiXExtension = this.WiXExtensions.Find((WiXExtension e) => e.Id == str);
							if (wiXExtension != null)
							{
								continue;
							}
							XmlNode xmlNodes = Common.CreateXmlElementWithAttributes(this.WiXElement.XmlNode.OwnerDocument, "Extension", this.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "ContentType" }, new string[] { str, this.MIME }, "", false);
							this.WiXElement.XmlNode.AppendChild(xmlNodes);
							this.WiXElement.SetDirty();
							wiXExtension = new WiXExtension(this.Project, this.WiXElement.Owner, this.WiXElement, xmlNodes);
							this.WiXExtensions.Add(wiXExtension);
							for (int i = 0; i < this.Verbs.Count; i++)
							{
								XmlDocument ownerDocument = this.WiXElement.XmlNode.OwnerDocument;
								string namespaceURI = this.WiXElement.XmlNode.NamespaceURI;
								string[] strArrays = new string[] { "Id", "Command", "Argument", "Sequence" };
								string[] verb = new string[] { this.Verbs[i].Verb, this.Verbs[i].Name, this.Verbs[i].Arguments, null };
								int sequence = this.Verbs[i].Sequence;
								verb[3] = sequence.ToString();
								xmlNodes = Common.CreateXmlElementWithAttributes(ownerDocument, "Verb", namespaceURI, strArrays, verb, "", false);
								wiXExtension.XmlNode.AppendChild(xmlNodes);
								wiXExtension.SetDirty();
								WiXVerb wiXVerb = new WiXVerb(this.Project, wiXExtension.Owner, wiXExtension, xmlNodes);
								this.Verbs[i].WiXElements.Add(wiXVerb);
							}
						}
						for (int j = this.WiXExtensions.Count - 1; j >= 0; j--)
						{
							WiXExtension item = this.WiXExtensions[j];
							if (string.IsNullOrEmpty(strs.Find((string e) => e == item.Id)))
							{
								for (int k = 0; k < this.Verbs.Count; k++)
								{
									List<WiXVerb> wiXElements = this.Verbs[k].WiXElements;
									for (int l = wiXElements.Count - 1; l >= 0; l--)
									{
										if (item.ChildEntities.Contains(wiXElements[l]))
										{
											wiXElements[l].Delete();
											this.Verbs[k].WiXElements.Remove(wiXElements[l]);
										}
									}
								}
								item.Delete();
								this.WiXExtensions.Remove(item);
							}
						}
					}
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies an icon to be displayed for the selected file type")]
		[Editor(typeof(IconPropertyEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(IconPropertyConverter))]
		public string Icon
		{
			get
			{
				if (this.ParentOutput != null)
				{
					return this._parentFileType.Icon;
				}
				if (this.WiXElement == null)
				{
					return string.Empty;
				}
				return this.WiXElement.Icon;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					value = this.Project.AddIcon(value).Id;
				}
				if (string.IsNullOrEmpty(value))
				{
					value = null;
				}
				if (this.ParentOutput != null)
				{
					this._parentFileType.Icon = value;
					List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes = new List<VsWiXProject.ReferenceDescriptor.FileType>();
					this.CombineVDProjFileTypes(ref fileTypes);
					fileTypes.Add(this._parentFileType);
					this.ParentOutput.ReferenceDescriptor.SetProjectOutputProperty(this.ParentOutput.Group, OutputGroupProperties.FileTypes, fileTypes);
				}
				if (this.ParentComponent != null)
				{
					this.WiXElement.Icon = value;
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies one MIME type to be associated with the selected file type")]
		[MergableProperty(false)]
		public string MIME
		{
			get
			{
				if (this.ParentOutput != null)
				{
					return this._parentFileType.MIME;
				}
				string empty = string.Empty;
				if (this.ParentComponent != null)
				{
					foreach (WiXExtension wiXExtension in this.WiXExtensions)
					{
						if (string.IsNullOrEmpty(wiXExtension.ContentType))
						{
							continue;
						}
						empty = wiXExtension.ContentType;
						goto Label0;
					}
				Label0:
					string.IsNullOrEmpty(empty);
				}
				return empty;
			}
			set
			{
				if (this.ParentOutput != null)
				{
					this._parentFileType.MIME = value;
					List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes = new List<VsWiXProject.ReferenceDescriptor.FileType>();
					this.CombineVDProjFileTypes(ref fileTypes);
					fileTypes.Add(this._parentFileType);
					this.ParentOutput.ReferenceDescriptor.SetProjectOutputProperty(this.ParentOutput.Group, OutputGroupProperties.FileTypes, fileTypes);
				}
				if (this.ParentComponent != null)
				{
					foreach (WiXExtension wiXExtension in this.WiXExtensions)
					{
						wiXExtension.ContentType = value;
					}
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the name used in the File Types Editor to identify a selected file type")]
		[DisplayName("(Name)")]
		[MergableProperty(false)]
		[ReadOnly(false)]
		public override string Name
		{
			get
			{
				if (this.ParentOutput != null)
				{
					return this._parentFileType.Name;
				}
				if (this.WiXElement == null)
				{
					return string.Empty;
				}
				return this.WiXElement.Id;
			}
			set
			{
				if (this.ParentOutput != null)
				{
					this._parentFileType.Name = value;
					List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes = new List<VsWiXProject.ReferenceDescriptor.FileType>();
					this.CombineVDProjFileTypes(ref fileTypes);
					fileTypes.Add(this._parentFileType);
					this.ParentOutput.ReferenceDescriptor.SetProjectOutputProperty(this.ParentOutput.Group, OutputGroupProperties.FileTypes, fileTypes);
				}
				if (this.ParentComponent != null)
				{
					this.WiXElement.Id = value;
				}
				this.DoPropertyChanged();
			}
		}

		internal VSComponent ParentComponent
		{
			get
			{
				return this._parentComponent;
			}
		}

		internal VSProjectOutputVDProj ParentOutput
		{
			get
			{
				return this._parentOutput;
			}
		}

		internal WiXProjectParser Project
		{
			get
			{
				return this._project;
			}
		}

		[Browsable(false)]
		public VSFileTypeVerbs Verbs
		{
			get
			{
				return this._verbs;
			}
		}

		internal WiXProgId WiXElement
		{
			get
			{
				return this._wixElement;
			}
		}

		internal List<WiXExtension> WiXExtensions
		{
			get
			{
				return this._wixExtensions;
			}
		}

		protected VSFileType(WiXProjectParser project)
		{
			this._project = project;
			this._verbs = new VSFileTypeVerbs(project, this);
		}

		public VSFileType(WiXProjectParser project, VSComponent parent, WiXProgId wixElement) : this(project)
		{
			this._parentComponent = parent;
			this._wixElement = wixElement;
			this._wixExtensions = new List<WiXExtension>();
			List<WiXExtension> wiXExtensions = this._project.SupportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXExtension))
				{
					return false;
				}
				return (e as WiXExtension).Parent == this.WiXElement;
			}).ConvertAll<WiXExtension>((WiXEntity e) => e as WiXExtension);
			if (wiXExtensions == null || wiXExtensions.Count == 0)
			{
				wiXExtensions = this.WiXElement.ChildEntities.FindAll((WiXEntity e) => e is WiXExtension).ConvertAll<WiXExtension>((WiXEntity e) => e as WiXExtension);
			}
			if (wiXExtensions != null)
			{
				foreach (WiXExtension wiXExtension in wiXExtensions)
				{
					this._wixExtensions.Add(wiXExtension);
					if (!this._project.SupportedEntities.Contains(wiXExtension))
					{
						continue;
					}
					this._project.SupportedEntities.Remove(wiXExtension);
				}
			}
			this._verbs.Parse();
		}

		public VSFileType(WiXProjectParser project, VSProjectOutputVDProj parent, VsWiXProject.ReferenceDescriptor.FileType fileType) : this(project)
		{
			this._parentOutput = parent;
			this._parentFileType = fileType;
			this._verbs.Parse();
		}

		internal void CombineVDProjFileTypes(ref List<VsWiXProject.ReferenceDescriptor.FileType> list)
		{
			foreach (VSFileType fileType in this.Project.FileTypes)
			{
				if (fileType == this || fileType.ParentOutput != this.ParentOutput)
				{
					continue;
				}
				list.Add(fileType._parentFileType);
			}
		}

		public override void Delete()
		{
			if (this.ParentOutput != null)
			{
				List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes = new List<VsWiXProject.ReferenceDescriptor.FileType>();
				this.CombineVDProjFileTypes(ref fileTypes);
				if (fileTypes.Count == 0)
				{
					fileTypes = null;
				}
				this.ParentOutput.ReferenceDescriptor.SetProjectOutputProperty(this.ParentOutput.Group, OutputGroupProperties.FileTypes, fileTypes);
			}
			if (this.ParentComponent != null)
			{
				this.WiXElement.Delete();
			}
			this.Project.FileTypes.Remove(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._project = null;
				this._parentComponent = null;
				this._parentOutput = null;
				this._verbs.Clear();
				this._verbs = null;
				if (this._wixExtensions != null)
				{
					this._wixExtensions.Clear();
					this._wixExtensions = null;
				}
				this._wixElement = null;
			}
			base.Dispose(disposing);
		}

		protected override string GetClassName()
		{
			return "File Type Properties";
		}

		protected override string GetComponentName()
		{
			return this.Name;
		}

		private void MoveTo(VSComponent destinationComponent)
		{
			VSComponent parentComponent = this.ParentComponent;
			this.WiXElement.Parent.ChildEntities.Remove(this.WiXElement);
			this.WiXElement.XmlNode.ParentNode.RemoveChild(this.WiXElement.XmlNode);
			this.WiXElement.Parent.SetDirty();
			if (this.WiXElement.Owner != destinationComponent.WiXElement.Owner)
			{
				this.WiXElement.RebuildXmlNodes(destinationComponent.WiXElement.XmlNode.OwnerDocument, destinationComponent.WiXElement.Owner);
			}
			destinationComponent.WiXElement.ChildEntities.Add(this.WiXElement);
			destinationComponent.WiXElement.XmlNode.AppendChild(this.WiXElement.XmlNode);
			this.WiXElement.Parent = destinationComponent.WiXElement;
			this.WiXElement.Parent.SetDirty();
			this._parentComponent = destinationComponent;
		}
	}
}