using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSFileTypeVerb : VSComponentBase
	{
		private WiXProjectParser _project;

		private VSFileType _parent;

		internal List<WiXVerb> _wixElements;

		internal VsWiXProject.ReferenceDescriptor.FileTypeVerb _verb;

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies command-line arguments for the command invoked by the selected action")]
		[MergableProperty(false)]
		public string Arguments
		{
			get
			{
				if (this._wixElements == null)
				{
					return this._verb.Arguments;
				}
				if (this.WiXElements.Count <= 0)
				{
					return string.Empty;
				}
				return this.WiXElements[0].Argument;
			}
			set
			{
				if (this._wixElements == null)
				{
					this._verb.Arguments = value;
					List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes = new List<VsWiXProject.ReferenceDescriptor.FileType>();
					this.Parent.CombineVDProjFileTypes(ref fileTypes);
					fileTypes.Add(this.Parent._parentFileType);
					this.Parent.ParentOutput.ReferenceDescriptor.SetProjectOutputProperty(this.Parent.ParentOutput.Group, OutputGroupProperties.FileTypes, fileTypes);
				}
				else
				{
					foreach (WiXVerb wiXElement in this.WiXElements)
					{
						wiXElement.Argument = value;
					}
				}
			}
		}

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
		[DefaultValue("")]
		[Description("Specifies the name used in the File Types Editor to identify an action used to invoke a verb")]
		[DisplayName("(Name)")]
		[MergableProperty(false)]
		[ReadOnly(false)]
		public override string Name
		{
			get
			{
				if (this._wixElements == null)
				{
					return this._verb.Name;
				}
				if (this.WiXElements.Count <= 0)
				{
					return string.Empty;
				}
				return this.WiXElements[0].Command;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (this._wixElements == null)
					{
						this._verb.Name = value;
						List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes = new List<VsWiXProject.ReferenceDescriptor.FileType>();
						this.Parent.CombineVDProjFileTypes(ref fileTypes);
						fileTypes.Add(this.Parent._parentFileType);
						this.Parent.ParentOutput.ReferenceDescriptor.SetProjectOutputProperty(this.Parent.ParentOutput.Group, OutputGroupProperties.FileTypes, fileTypes);
					}
					else
					{
						foreach (WiXVerb wiXElement in this.WiXElements)
						{
							wiXElement.Command = value;
						}
					}
					this.DoPropertyChanged();
				}
			}
		}

		internal VSFileType Parent
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

		internal int Sequence
		{
			get
			{
				if (this._wixElements == null)
				{
					return Convert.ToInt32(this._verb.Sequence);
				}
				return Convert.ToInt32(this._wixElements[0].Sequence);
			}
			set
			{
				if (this._wixElements == null)
				{
					this._verb.Sequence = value.ToString();
					List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes = new List<VsWiXProject.ReferenceDescriptor.FileType>();
					this.Parent.CombineVDProjFileTypes(ref fileTypes);
					fileTypes.Add(this.Parent._parentFileType);
					this.Parent.ParentOutput.ReferenceDescriptor.SetProjectOutputProperty(this.Parent.ParentOutput.Group, OutputGroupProperties.FileTypes, fileTypes);
				}
				else
				{
					foreach (WiXVerb _wixElement in this._wixElements)
					{
						_wixElement.Sequence = value.ToString();
					}
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the verb used to invoke a selected action for a file type")]
		[MergableProperty(false)]
		public string Verb
		{
			get
			{
				if (this._wixElements == null)
				{
					return this._verb.Verb;
				}
				if (this.WiXElements.Count <= 0)
				{
					return string.Empty;
				}
				return this.WiXElements[0].Id;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					if (this._wixElements == null)
					{
						this._verb.Verb = value;
						List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes = new List<VsWiXProject.ReferenceDescriptor.FileType>();
						this.Parent.CombineVDProjFileTypes(ref fileTypes);
						fileTypes.Add(this.Parent._parentFileType);
						this.Parent.ParentOutput.ReferenceDescriptor.SetProjectOutputProperty(this.Parent.ParentOutput.Group, OutputGroupProperties.FileTypes, fileTypes);
					}
					else
					{
						foreach (WiXVerb wiXElement in this.WiXElements)
						{
							wiXElement.Id = value;
						}
					}
				}
			}
		}

		internal List<WiXVerb> WiXElements
		{
			get
			{
				return this._wixElements;
			}
		}

		protected VSFileTypeVerb()
		{
		}

		public VSFileTypeVerb(WiXProjectParser project, VSFileType parent) : this()
		{
			this._project = project;
			this._parent = parent;
			this._wixElements = new List<WiXVerb>();
		}

		public VSFileTypeVerb(WiXProjectParser project, VSFileType parent, VsWiXProject.ReferenceDescriptor.FileTypeVerb verb) : this()
		{
			this._project = project;
			this._parent = parent;
			this._verb = verb;
		}

		public override void Delete()
		{
			if (this._wixElements == null)
			{
				this.Parent._parentFileType.Verbs.Remove(this._verb);
				List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes = new List<VsWiXProject.ReferenceDescriptor.FileType>();
				this.Parent.CombineVDProjFileTypes(ref fileTypes);
				fileTypes.Add(this.Parent._parentFileType);
				this.Parent.ParentOutput.ReferenceDescriptor.SetProjectOutputProperty(this.Parent.ParentOutput.Group, OutputGroupProperties.FileTypes, fileTypes);
			}
			else
			{
				foreach (WiXVerb wiXElement in this.WiXElements)
				{
					wiXElement.Delete();
				}
			}
			this._parent.Verbs.Remove(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._project = null;
				this._parent = null;
				if (this._wixElements != null)
				{
					this._wixElements.Clear();
					this._wixElements = null;
				}
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
	}
}