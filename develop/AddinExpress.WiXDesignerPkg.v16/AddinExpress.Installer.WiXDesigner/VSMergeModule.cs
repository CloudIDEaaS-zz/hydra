using EnvDTE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSMergeModule : VSComponentBase
	{
		protected VSBaseFolder _parentFolder;

		protected WiXProjectParser _project;

		private WiXMerge _wixElement;

		private WiXMergeRef _wixMergeRef;

		private List<string> _files = new List<string>();

		private List<string> _dependencies = new List<string>();

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Stores the name of the author of a merge module")]
		[ReadOnly(true)]
		public string Author
		{
			get
			{
				string moduleSummaryInfo;
				if (string.IsNullOrEmpty(this.SourcePath))
				{
					return string.Empty;
				}
				using (MsiHelper msiHelper = new MsiHelper())
				{
					moduleSummaryInfo = msiHelper.GetModuleSummaryInfo(this.SourcePath, MsiSummaryInfoProperties.Author);
				}
				return moduleSummaryInfo;
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
		[Description("Stores a text string describing a merge module")]
		[ReadOnly(true)]
		public string Description
		{
			get
			{
				string moduleSummaryInfo;
				if (string.IsNullOrEmpty(this.SourcePath))
				{
					return string.Empty;
				}
				using (MsiHelper msiHelper = new MsiHelper())
				{
					moduleSummaryInfo = msiHelper.GetModuleSummaryInfo(this.SourcePath, MsiSummaryInfoProperties.Comments);
				}
				return moduleSummaryInfo;
			}
		}

		[Browsable(true)]
		[DefaultValue(null)]
		[Description("Displays the Files dialog box that contains a list of files in a merge module")]
		[Editor(typeof(FilesPropertyEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(FilesPropertyConverter))]
		public object Files
		{
			get
			{
				return this._files;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the numeric language ID or IDs for a merge module")]
		[ReadOnly(true)]
		public string LanguageIds
		{
			get
			{
				string moduleSummaryInfo;
				if (string.IsNullOrEmpty(this.SourcePath))
				{
					return string.Empty;
				}
				using (MsiHelper msiHelper = new MsiHelper())
				{
					moduleSummaryInfo = msiHelper.GetModuleSummaryInfo(this.SourcePath, MsiSummaryInfoProperties.Template);
				}
				return moduleSummaryInfo;
			}
		}

		[Browsable(true)]
		[DefaultValue(null)]
		[Description("Displays the Dependencies dialog box that contains a list of dependent files for a merge module")]
		[Editor(typeof(DependenciesPropertyEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(DependenciesPropertyConverter))]
		public object ModuleDependencies
		{
			get
			{
				return this._dependencies;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Stores a unique identifier for a merge module")]
		[ReadOnly(true)]
		public string ModuleSignature
		{
			get
			{
				string moduleSignatureInfo;
				if (string.IsNullOrEmpty(this.SourcePath))
				{
					return string.Empty;
				}
				using (MsiHelper msiHelper = new MsiHelper())
				{
					moduleSignatureInfo = msiHelper.GetModuleSignatureInfo(this.SourcePath, 1);
				}
				return moduleSignatureInfo;
			}
		}

		[Browsable(false)]
		[DisplayName("(Name)")]
		[MergableProperty(false)]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				if (string.IsNullOrEmpty(this.SourcePath))
				{
					return "Unknown Merge Module";
				}
				return Path.GetFileName(this.SourcePath);
			}
			set
			{
			}
		}

		[Browsable(true)]
		[DefaultValue(null)]
		[Description("")]
		[DisplayName("(Module Retargetable Folder)")]
		[Editor(typeof(FolderPropertyEditor), typeof(UITypeEditor))]
		[MergableProperty(false)]
		[TypeConverter(typeof(FolderPropertyConverter))]
		public VSBaseFolder ParentFolder
		{
			get
			{
				return this._parentFolder;
			}
			set
			{
				if (value != null)
				{
					this.MoveTo(value);
				}
			}
		}

		internal WiXProjectParser Project
		{
			get
			{
				return this._project;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Stores the path to a selected merge module on the development computer")]
		[ReadOnly(true)]
		public string SourcePath
		{
			get
			{
				if (this.WiXElement == null || string.IsNullOrEmpty(this.WiXElement.SourceFile))
				{
					return string.Empty;
				}
				string sourceFile = this.WiXElement.SourceFile;
				if (sourceFile.Contains("$(var."))
				{
					while (sourceFile.Contains("$(var."))
					{
						string str = sourceFile.Substring(sourceFile.IndexOf("$(var."));
						str = str.Substring(0, str.IndexOf(")") + 1);
						WiXCustomVariable wiXCustomVariable = this.Project.CustomVariables.FindVariable(str);
						if (wiXCustomVariable == null)
						{
							string referencedProjectVariable = this.Project.ProjectManager.GetReferencedProjectVariable(str);
							sourceFile = (string.IsNullOrEmpty(referencedProjectVariable) ? sourceFile.Replace(str, string.Empty) : sourceFile.Replace(str, referencedProjectVariable));
						}
						else
						{
							sourceFile = sourceFile.Replace(str, wiXCustomVariable.Value);
						}
					}
				}
				if (!Path.IsPathRooted(sourceFile) && this.Project.ProjectManager.VsProject != null)
				{
					string directoryName = Path.GetDirectoryName(this.Project.ProjectManager.VsProject.FullName);
					sourceFile = Path.GetFullPath(Path.Combine(CommonUtilities.AbsolutePathFromRelative(Path.GetDirectoryName(sourceFile), directoryName), Path.GetFileName(sourceFile)));
				}
				return sourceFile;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Stores additional information about a merge module")]
		[ReadOnly(true)]
		public string Subject
		{
			get
			{
				string moduleSummaryInfo;
				if (string.IsNullOrEmpty(this.SourcePath))
				{
					return string.Empty;
				}
				using (MsiHelper msiHelper = new MsiHelper())
				{
					moduleSummaryInfo = msiHelper.GetModuleSummaryInfo(this.SourcePath, MsiSummaryInfoProperties.Subject);
				}
				return moduleSummaryInfo;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Stores the title of a merge module")]
		[ReadOnly(true)]
		public string Title
		{
			get
			{
				string moduleSummaryInfo;
				if (string.IsNullOrEmpty(this.SourcePath))
				{
					return string.Empty;
				}
				using (MsiHelper msiHelper = new MsiHelper())
				{
					moduleSummaryInfo = msiHelper.GetModuleSummaryInfo(this.SourcePath, MsiSummaryInfoProperties.Title);
				}
				return moduleSummaryInfo;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Stores the version number of a merge module")]
		[ReadOnly(true)]
		public string Version
		{
			get
			{
				string moduleSignatureInfo;
				if (string.IsNullOrEmpty(this.SourcePath))
				{
					return string.Empty;
				}
				using (MsiHelper msiHelper = new MsiHelper())
				{
					moduleSignatureInfo = msiHelper.GetModuleSignatureInfo(this.SourcePath, 3);
				}
				return moduleSignatureInfo;
			}
		}

		internal WiXMerge WiXElement
		{
			get
			{
				return this._wixElement;
			}
		}

		internal WiXMergeRef WiXElementRef
		{
			get
			{
				return this._wixMergeRef;
			}
		}

		protected VSMergeModule()
		{
		}

		public VSMergeModule(WiXProjectParser project, WiXMerge merge, WiXMergeRef mergeRef, VSBaseFolder parent) : this()
		{
			this._project = project;
			this._parentFolder = parent;
			this._wixElement = merge;
			this._wixMergeRef = mergeRef;
		}

		public override void Delete()
		{
			if (this._project != null)
			{
				this._project.MergeModules.Remove(this);
			}
			if (this.WiXElementRef != null)
			{
				this.WiXElementRef.Delete();
			}
			if (this.WiXElement != null)
			{
				this.WiXElement.Delete();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._project = null;
				this._parentFolder = null;
				this._wixElement = null;
				this._wixMergeRef = null;
			}
			base.Dispose(disposing);
		}

		private void MoveTo(VSBaseFolder newParent)
		{
			if (newParent.WiXElement != null)
			{
				this.WiXElement.Parent.ChildEntities.Remove(this.WiXElement);
				this.WiXElement.XmlNode.ParentNode.RemoveChild(this.WiXElement.XmlNode);
				this.WiXElement.Parent.SetDirty();
				if (this.WiXElement.Owner != newParent.WiXElement.Owner)
				{
					this.WiXElement.RebuildXmlNodes(newParent.WiXElement.XmlNode.OwnerDocument, newParent.WiXElement.Owner);
				}
				newParent.WiXElement.ChildEntities.Add(this.WiXElement);
				newParent.WiXElement.XmlNode.AppendChild(this.WiXElement.XmlNode);
				this.WiXElement.Parent = newParent.WiXElement;
				this.WiXElement.Parent.SetDirty();
				this._parentFolder = newParent;
			}
		}
	}
}