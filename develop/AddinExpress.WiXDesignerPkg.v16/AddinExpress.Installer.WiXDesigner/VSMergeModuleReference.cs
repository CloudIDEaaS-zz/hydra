using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSMergeModuleReference : VSComponentBase
	{
		protected WiXProjectParser _project;

		private WiXDependency _wixElement;

		private string _modulePath = string.Empty;

		private bool _modulePathResolved = true;

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
				return false;
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
					return "Unknown Merge Module Reference";
				}
				return Path.GetFileName(this.SourcePath);
			}
			set
			{
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
				if (this._modulePathResolved && string.IsNullOrEmpty(this._modulePath) && this.WiXElement != null)
				{
					string str = (string.IsNullOrEmpty(this.WiXElement.RequiredId) ? "" : this.WiXElement.RequiredId);
					string str1 = (string.IsNullOrEmpty(this.WiXElement.RequiredLanguage) ? "" : this.WiXElement.RequiredLanguage);
					string str2 = (string.IsNullOrEmpty(this.WiXElement.RequiredVersion) ? "" : this.WiXElement.RequiredVersion);
					using (MsiHelper msiHelper = new MsiHelper())
					{
						this._modulePath = msiHelper.FindStandardMSM(str, str1, str2);
						if (string.IsNullOrEmpty(this._modulePath))
						{
							this._modulePathResolved = false;
						}
					}
				}
				return this._modulePath;
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

		internal WiXDependency WiXElement
		{
			get
			{
				return this._wixElement;
			}
		}

		protected VSMergeModuleReference()
		{
		}

		public VSMergeModuleReference(WiXProjectParser project, WiXDependency dependency) : this()
		{
			this._project = project;
			this._wixElement = dependency;
		}

		public VSMergeModuleReference(WiXProjectParser project, WiXDependency dependency, string path) : this(project, dependency)
		{
			this._modulePath = path;
		}

		public override void Delete()
		{
			if (this._project != null)
			{
				this._project.MergeModuleReferences.Remove(this);
			}
			this.WiXElement.Delete();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._project = null;
				this._wixElement = null;
			}
			base.Dispose(disposing);
		}
	}
}