using EnvDTE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSFile : VSBaseFile
	{
		private WiXFile _wixElement;

		internal override bool CanRename
		{
			get
			{
				return true;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies a Windows Installer condition that must be satisfied (evaluate to true) in order for the selected item to be installed at installation time")]
		public string Condition
		{
			get
			{
				if (base.ParentComponent.WiXCondition == null)
				{
					return string.Empty;
				}
				return base.ParentComponent.WiXCondition.Condition;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.ParentComponent.DeleteWiXCondition();
					return;
				}
				if (base.ParentComponent.WiXCondition == null)
				{
					base.ParentComponent.CreateWiXCondition();
				}
				base.ParentComponent.WiXCondition.Condition = value;
			}
		}

		[Browsable(true)]
		[DefaultValue(null)]
		[Description("Specifies the folder where where the selected file will be installed on the target computer")]
		public override VSBaseFolder Folder
		{
			get
			{
				return base.Folder;
			}
			set
			{
				base.Folder = value;
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Specifies whether to install a file as a hidden file")]
		public bool Hidden
		{
			get
			{
				if (this.WiXElement.Hidden == null)
				{
					return false;
				}
				return this.WiXElement.Hidden == "yes";
			}
			set
			{
				string str;
				WiXFile wiXElement = this.WiXElement;
				if (value)
				{
					str = "yes";
				}
				else
				{
					str = null;
				}
				wiXElement.Hidden = str;
			}
		}

		[Browsable(false)]
		[DisplayName("(Name)")]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
			}
		}

		[Browsable(true)]
		[DefaultValue(VSPackageAs.vsdpaDefault)]
		[Description("Specifies whether to override the default packaging behavior for a selected file")]
		public VSPackageAs PackageAs
		{
			get
			{
				if (this.WiXElement.Compressed != null && this.WiXElement.Compressed == "no")
				{
					return VSPackageAs.vsdpaLoose;
				}
				return VSPackageAs.vsdpaDefault;
			}
			set
			{
				string str;
				WiXFile wiXElement = this.WiXElement;
				if (value == VSPackageAs.vsdpaLoose)
				{
					str = "no";
				}
				else
				{
					str = null;
				}
				wiXElement.Compressed = str;
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Specifies whether a file should be removed when the application is uninstalled")]
		public bool Permanent
		{
			get
			{
				if (base.ParentComponent == null || base.ParentComponent.WiXElement == null)
				{
					return false;
				}
				return base.ParentComponent.WiXElement.Permanent == "yes";
			}
			set
			{
				if (base.ParentComponent != null)
				{
					base.ParentComponent.WiXElement.Permanent = (value ? "yes" : "no");
				}
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Specifies whether to install a selected file as a read-only file")]
		public bool ReadOnly
		{
			get
			{
				if (this.WiXElement.ReadOnly == null)
				{
					return false;
				}
				return this.WiXElement.ReadOnly == "yes";
			}
			set
			{
				string str;
				WiXFile wiXElement = this.WiXElement;
				if (value)
				{
					str = "yes";
				}
				else
				{
					str = null;
				}
				wiXElement.ReadOnly = str;
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Specifies whether to mark a selected file as a shared legacy file that requires reference counting")]
		public bool SharedLegacyFile
		{
			get
			{
				if (base.ParentComponent == null || base.ParentComponent.WiXElement == null)
				{
					return false;
				}
				return base.ParentComponent.WiXElement.SharedDllRefCount == "yes";
			}
			set
			{
				if (base.ParentComponent != null)
				{
					base.ParentComponent.WiXElement.SharedDllRefCount = (value ? "yes" : "no");
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Displays the path to a selected file on the development computer")]
		[ReadOnly(true)]
		public override string SourcePath
		{
			get
			{
				if (string.IsNullOrEmpty(this.WiXElement.Source) && !string.IsNullOrEmpty(base.ParentComponent.Parent.WiXElement.FileSource) && !string.IsNullOrEmpty(this.WiXElement.Name))
				{
					return Path.Combine(base.ParentComponent.Parent.WiXElement.FileSource, this.WiXElement.Name);
				}
				string empty = string.Empty;
				if (Path.IsPathRooted(this.WiXElement.Source) && !string.IsNullOrEmpty(Path.GetFileName(this.WiXElement.Source)))
				{
					empty = this.WiXElement.Source;
					if (empty.Contains("$(var."))
					{
						while (empty.Contains("$(var."))
						{
							string str = empty.Substring(empty.IndexOf("$(var."));
							str = str.Substring(0, str.IndexOf(")") + 1);
							WiXCustomVariable wiXCustomVariable = base.Project.CustomVariables.FindVariable(str);
							if (wiXCustomVariable == null)
							{
								string referencedProjectVariable = base.Project.ProjectManager.GetReferencedProjectVariable(str);
								empty = (string.IsNullOrEmpty(referencedProjectVariable) ? empty.Replace(str, string.Empty) : empty.Replace(str, referencedProjectVariable));
							}
							else
							{
								empty = empty.Replace(str, wiXCustomVariable.Value);
							}
						}
					}
					return empty;
				}
				if (Path.IsPathRooted(this.WiXElement.Source) && string.IsNullOrEmpty(Path.GetFileName(this.WiXElement.Source)))
				{
					empty = Path.Combine(this.WiXElement.Source, this.WiXElement.Name);
					if (empty.Contains("$(var."))
					{
						while (empty.Contains("$(var."))
						{
							string str1 = empty.Substring(empty.IndexOf("$(var."));
							str1 = str1.Substring(0, str1.IndexOf(")") + 1);
							WiXCustomVariable wiXCustomVariable1 = base.Project.CustomVariables.FindVariable(str1);
							if (wiXCustomVariable1 == null)
							{
								string referencedProjectVariable1 = base.Project.ProjectManager.GetReferencedProjectVariable(str1);
								empty = (string.IsNullOrEmpty(referencedProjectVariable1) ? empty.Replace(str1, string.Empty) : empty.Replace(str1, referencedProjectVariable1));
							}
							else
							{
								empty = empty.Replace(str1, wiXCustomVariable1.Value);
							}
						}
					}
					return empty;
				}
				empty = this.WiXElement.Source;
				if (empty.Contains("$(var."))
				{
					while (empty.Contains("$(var."))
					{
						string str2 = empty.Substring(empty.IndexOf("$(var."));
						str2 = str2.Substring(0, str2.IndexOf(")") + 1);
						WiXCustomVariable wiXCustomVariable2 = base.Project.CustomVariables.FindVariable(str2);
						if (wiXCustomVariable2 == null)
						{
							string referencedProjectVariable2 = base.Project.ProjectManager.GetReferencedProjectVariable(str2);
							empty = (string.IsNullOrEmpty(referencedProjectVariable2) ? empty.Replace(str2, string.Empty) : empty.Replace(str2, referencedProjectVariable2));
						}
						else
						{
							empty = empty.Replace(str2, wiXCustomVariable2.Value);
						}
					}
				}
				if (string.IsNullOrEmpty(Path.GetFileName(empty)) && !string.IsNullOrEmpty(this.WiXElement.Name))
				{
					string name = this.WiXElement.Name;
					if (name.Contains("$(var."))
					{
						while (name.Contains("$(var."))
						{
							string str3 = name.Substring(name.IndexOf("$(var."));
							str3 = str3.Substring(0, str3.IndexOf(")") + 1);
							WiXCustomVariable wiXCustomVariable3 = base.Project.CustomVariables.FindVariable(str3);
							if (wiXCustomVariable3 == null)
							{
								string referencedProjectVariable3 = base.Project.ProjectManager.GetReferencedProjectVariable(str3);
								name = (string.IsNullOrEmpty(referencedProjectVariable3) ? name.Replace(str3, string.Empty) : name.Replace(str3, referencedProjectVariable3));
							}
							else
							{
								name = name.Replace(str3, wiXCustomVariable3.Value);
							}
						}
					}
					empty = Path.Combine(empty, name);
				}
				if (!Path.IsPathRooted(empty) && base.Project.ProjectManager.VsProject != null)
				{
					string directoryName = Path.GetDirectoryName(base.Project.ProjectManager.VsProject.FullName);
					empty = Path.GetFullPath(Path.Combine(CommonUtilities.AbsolutePathFromRelative(Path.GetDirectoryName(empty), directoryName), Path.GetFileName(empty)));
				}
				return empty;
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Specifies whether to install a selected file as a system file")]
		public bool System
		{
			get
			{
				if (this.WiXElement.System == null)
				{
					return false;
				}
				return this.WiXElement.System == "yes";
			}
			set
			{
				string str;
				WiXFile wiXElement = this.WiXElement;
				if (value)
				{
					str = "yes";
				}
				else
				{
					str = null;
				}
				wiXElement.System = str;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies a name for a file when it is installed on a target computer")]
		public override string TargetName
		{
			get
			{
				if (string.IsNullOrEmpty(this.WiXElement.Name))
				{
					return Path.GetFileName(this.SourcePath);
				}
				return this.WiXElement.Name;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.WiXElement.Name = value;
					this.DoPropertyChanged();
				}
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Description("Determines whether the installer will reevaluate the Condition property for the selected item when reinstalling on a trget computer")]
		public override bool Transitive
		{
			get
			{
				return base.Transitive;
			}
			set
			{
				base.Transitive = value;
			}
		}

		[Browsable(true)]
		[DefaultValue(true)]
		[Description("Specifies whether a selected file is vital for installation")]
		public bool Vital
		{
			get
			{
				if (this.WiXElement.Vital == null)
				{
					return true;
				}
				return this.WiXElement.Vital == "yes";
			}
			set
			{
				string str;
				WiXFile wiXElement = this.WiXElement;
				if (value)
				{
					str = null;
				}
				else
				{
					str = "no";
				}
				wiXElement.Vital = str;
			}
		}

		internal WiXFile WiXElement
		{
			get
			{
				return this._wixElement;
			}
		}

		public VSFile()
		{
		}

		public VSFile(WiXProjectParser project, VSComponent parent, WiXFile wixElement) : base(project, parent)
		{
			this._wixElement = wixElement;
		}

		public VSFile(VSBaseFolder folder, string filePath, VSComponent component) : base(folder._project, component)
		{
			if (base.ParentComponent == null)
			{
				base.ParentComponent = base.CreateComponent(folder);
			}
			if (base.ParentComponent == null)
			{
				throw new Exception(string.Concat("Cannot create WiX Component for ", filePath, " file."));
			}
			string empty = string.Empty;
			string str = filePath;
			if (base.Project.ProjectManager.VsProject != null)
			{
				empty = Path.GetDirectoryName(base.Project.ProjectManager.VsProject.FullName);
			}
			if (Path.IsPathRooted(filePath) && !string.IsNullOrEmpty(empty))
			{
				str = CommonUtilities.RelativizePathIfPossible(filePath, empty);
				if (!string.IsNullOrEmpty(str) && str.EndsWith("\\"))
				{
					str = str.Remove(str.Length - 1);
				}
			}
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(base.ParentComponent.WiXElement.XmlNode.OwnerDocument, "File", base.ParentComponent.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "DiskId", "Hidden", "ReadOnly", "TrueType", "System", "Vital", "Name", "Source" }, new string[] { string.Concat("_", Common.GenerateId(base.Project.ProjectType)), "1", "no", "no", "no", "no", "yes", Path.GetFileName(filePath), str }, "", false);
			base.ParentComponent.WiXElement.XmlNode.AppendChild(xmlElement);
			this._wixElement = new WiXFile(this._project, base.ParentComponent.WiXElement.Owner, base.ParentComponent.WiXElement, xmlElement);
			this._wixElement.Parent.SetDirty();
			base.ParentComponent.AdjustKeyPath();
			if (base.ParentComponent.Files.Count == 1 && (base.ParentComponent.WiXElement.KeyPath == null || base.ParentComponent.WiXElement.KeyPath == "no") && base.ParentComponent.WiXKeyPathElement == null)
			{
				this._wixElement.KeyPath = "yes";
			}
			if (base.ParentFolder != null && base.ParentFolder is VSGACFolder)
			{
				this.WiXElement.Assembly = ".net";
			}
		}

		public override void Delete()
		{
			if (base.ParentComponent != null)
			{
				base.ParentComponent.Files.Remove(this);
			}
			this.WiXElement.Delete();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._wixElement = null;
			}
			base.Dispose(disposing);
		}

		protected override string GetComponentName()
		{
			return this.TargetName;
		}

		internal override void MoveTo(VSBaseFolder destinationFolder)
		{
			if (base.ParentComponent.Parent != destinationFolder)
			{
				if (base.ParentComponent.Files.Count != 1)
				{
					VSComponent comPlusFlags = base.CreateComponent(destinationFolder);
					VSComponent parentComponent = base.ParentComponent;
					comPlusFlags.WiXElement.ComPlusFlags = parentComponent.WiXElement.ComPlusFlags;
					comPlusFlags.WiXElement.DisableRegistryReflection = parentComponent.WiXElement.DisableRegistryReflection;
					comPlusFlags.WiXElement.DiskId = parentComponent.WiXElement.DiskId;
					comPlusFlags.WiXElement.Feature = parentComponent.WiXElement.Feature;
					comPlusFlags.WiXElement.KeyPath = parentComponent.WiXElement.KeyPath;
					comPlusFlags.WiXElement.Location = parentComponent.WiXElement.Location;
					comPlusFlags.WiXElement.MultiInstance = parentComponent.WiXElement.MultiInstance;
					comPlusFlags.WiXElement.NeverOverwrite = parentComponent.WiXElement.NeverOverwrite;
					comPlusFlags.WiXElement.Permanent = parentComponent.WiXElement.Permanent;
					comPlusFlags.WiXElement.Shared = parentComponent.WiXElement.Shared;
					comPlusFlags.WiXElement.SharedDllRefCount = parentComponent.WiXElement.SharedDllRefCount;
					comPlusFlags.WiXElement.Transitive = parentComponent.WiXElement.Transitive;
					comPlusFlags.WiXElement.UninstallWhenSuperseded = parentComponent.WiXElement.UninstallWhenSuperseded;
					if (parentComponent.WiXCondition != null)
					{
						comPlusFlags.CreateWiXCondition();
						comPlusFlags.WiXCondition.Condition = parentComponent.WiXCondition.Condition;
					}
					if (parentComponent.WiXKeyPathElement != null)
					{
						comPlusFlags.CreateWiXKeyPathElement();
					}
					base.ParentComponent = comPlusFlags;
					this.WiXElement.Parent.ChildEntities.Remove(this.WiXElement);
					this.WiXElement.XmlNode.ParentNode.RemoveChild(this.WiXElement.XmlNode);
					this.WiXElement.Parent.SetDirty();
					if (this.WiXElement.Owner != comPlusFlags.WiXElement.Owner)
					{
						this.WiXElement.RebuildXmlNodes(comPlusFlags.WiXElement.XmlNode.OwnerDocument, comPlusFlags.WiXElement.Owner);
					}
					comPlusFlags.WiXElement.ChildEntities.Add(this.WiXElement);
					comPlusFlags.WiXElement.XmlNode.AppendChild(this.WiXElement.XmlNode);
					this.WiXElement.Parent = comPlusFlags.WiXElement;
					this.WiXElement.Parent.SetDirty();
				}
				else
				{
					base.ParentComponent.Parent = destinationFolder;
					base.ParentComponent.WiXElement.Parent.ChildEntities.Remove(base.ParentComponent.WiXElement);
					base.ParentComponent.WiXElement.XmlNode.ParentNode.RemoveChild(base.ParentComponent.WiXElement.XmlNode);
					base.ParentComponent.WiXElement.Parent.SetDirty();
					if (base.ParentComponent.WiXElement.Owner != destinationFolder.WiXElement.Owner)
					{
						base.ParentComponent.WiXElement.RebuildXmlNodes(destinationFolder.WiXElement.XmlNode.OwnerDocument, destinationFolder.WiXElement.Owner);
					}
					destinationFolder.WiXElement.ChildEntities.Add(base.ParentComponent.WiXElement);
					destinationFolder.WiXElement.XmlNode.AppendChild(base.ParentComponent.WiXElement.XmlNode);
					base.ParentComponent.WiXElement.Parent = destinationFolder.WiXElement;
					base.ParentComponent.WiXElement.Parent.SetDirty();
				}
				if (base.ParentFolder == null || !(base.ParentFolder is VSGACFolder))
				{
					this.WiXElement.Assembly = null;
					this.WiXElement.AssemblyApplication = null;
					this.WiXElement.AssemblyManifest = null;
				}
				else
				{
					this.WiXElement.Assembly = ".net";
				}
				base.ParentComponent.AdjustKeyPath();
			}
		}

		internal override void MoveTo(VSComponent destinationComponent)
		{
			VSComponent parentComponent = base.ParentComponent;
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
			base.ParentComponent = destinationComponent;
			if (parentComponent.Files.Count > 0)
			{
				parentComponent.AdjustKeyPath();
				return;
			}
			if (parentComponent.CanBeDeleted)
			{
				parentComponent.Delete();
			}
		}
	}
}