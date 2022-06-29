using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSCustomActionBase : VSComponentBase
	{
		private WiXProjectParser _project;

		private VSCustomActions _collection;

		private AddinExpress.Installer.WiXDesigner.WiXCustomAction _wixCustomAction;

		private AddinExpress.Installer.WiXDesigner.WiXCustomAction _wixCustomActionSetProperty;

		private AddinExpress.Installer.WiXDesigner.WiXCustom _wixCustom;

		private AddinExpress.Installer.WiXDesigner.WiXCustom _wixCustomSetProperty;

		private VSBaseFile _vsFile;

		internal bool _isManaged;

		[Browsable(false)]
		[DefaultValue("")]
		[Description("Specifies command-line arguments for the selected custom action")]
		public virtual string Arguments
		{
			get
			{
				if (this.InstallerClass)
				{
					return string.Empty;
				}
				if (this.WiXCustomAction == null)
				{
					return string.Empty;
				}
				return this.WiXCustomAction.ExeCommand;
			}
			set
			{
				if (this.InstallerClass)
				{
					return;
				}
				if (this.WiXCustomAction != null)
				{
					this.WiXCustomAction.ExeCommand = value;
				}
			}
		}

		internal override bool CanRename
		{
			get
			{
				return true;
			}
		}

		internal VSCustomActions Collection
		{
			get
			{
				return this._collection;
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies a Windows Installer condition that must be satisfied (evaluate to true) in order for the selected custom action to be run at installation time")]
		public string Condition
		{
			get
			{
				if (this.WiXCustom == null || string.IsNullOrEmpty(this.WiXCustom.Text))
				{
					return string.Empty;
				}
				string str = this.WiXCustom.Text.Replace("REMOVE~=\"ALL\" AND ProductState <> 1", "").Replace("NOT REMOVE~=\"ALL\"", "").Trim();
				if (str.ToUpper().StartsWith("AND "))
				{
					str = str.Remove(0, 4);
				}
				if (str.ToUpper().EndsWith(" AND"))
				{
					str = str.Remove(str.Length - 4);
				}
				List<string> allComponentIDs = this._project.FileSystem.GetAllComponentIDs();
				if (allComponentIDs != null && allComponentIDs.Count > 0)
				{
					foreach (string allComponentID in allComponentIDs)
					{
						if (str.Contains(string.Concat("$", allComponentID, ">2")))
						{
							str = str.Replace(string.Concat("$", allComponentID, ">2"), string.Empty).Trim();
							if (str.ToUpper().StartsWith("AND "))
							{
								str = str.Remove(0, 4);
							}
							if (str.ToUpper().EndsWith(" AND"))
							{
								str = str.Remove(str.Length - 4);
							}
						}
						if (!str.Contains(string.Concat("$", allComponentID, "=2")))
						{
							continue;
						}
						str = str.Replace(string.Concat("$", allComponentID, "=2"), string.Empty).Trim();
						if (str.ToUpper().StartsWith("AND "))
						{
							str = str.Remove(0, 4);
						}
						if (!str.ToUpper().EndsWith(" AND"))
						{
							continue;
						}
						str = str.Remove(str.Length - 4);
					}
				}
				return str;
			}
			set
			{
				if (this.WiXCustom != null)
				{
					string str = (string.IsNullOrEmpty(this.WiXCustom.Text) ? string.Empty : this.WiXCustom.Text);
					string condition = this.Condition;
					if (!string.IsNullOrEmpty(condition) && str.Contains(condition))
					{
						str = str.Replace(condition, string.Empty).Trim();
						if (str.ToUpper().StartsWith("AND "))
						{
							str = str.Remove(0, 4);
						}
						if (str.ToUpper().EndsWith(" AND"))
						{
							str = str.Remove(str.Length - 4);
						}
					}
					if (!string.IsNullOrEmpty(value))
					{
						this.WiXCustom.Text = string.Concat(str, " AND ", value);
					}
					else
					{
						this.WiXCustom.Text = str;
					}
					if (this.WiXCustomSetProperty != null)
					{
						this.WiXCustomSetProperty.Text = this.WiXCustom.Text;
					}
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies custom data to be passed to an installer")]
		public string CustomActionData
		{
			get
			{
				if (this.InstallerClass)
				{
					if (this.WiXCustomActionSetProperty != null && !string.IsNullOrEmpty(this.WiXCustomActionSetProperty.Value))
					{
						List<string> strs = new List<string>(this.WiXCustomActionSetProperty.Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
						strs.RemoveAll((string e) => !e.StartsWith("/"));
						strs.RemoveAll((string e) => e.ToLower().StartsWith("/installtype"));
						strs.RemoveAll((string e) => e.ToLower().StartsWith("/action"));
						strs.RemoveAll((string e) => e.ToLower().StartsWith("/logfile"));
						return string.Join(" ", strs);
					}
				}
				else if (this.WiXCustomActionSetProperty != null)
				{
					return this.WiXCustomActionSetProperty.Value;
				}
				return string.Empty;
			}
			set
			{
				if (!this.InstallerClass)
				{
					if (string.IsNullOrEmpty(value))
					{
						this.DeleteCustomActionSetProperty();
						return;
					}
					if (this.WiXCustomActionSetProperty == null)
					{
						this.CreateCustomActionSetProperty();
					}
					this.WiXCustomActionSetProperty.Value = value;
				}
				else if (this.WiXCustomActionSetProperty != null)
				{
					if (!value.Contains("//") && !value.Contains("="))
					{
						MessageBox.Show("CustomActionData for InstallerClass actions must be in format '/name1=value1 /name2=value2'", Common.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
						return;
					}
					string empty = string.Empty;
					if (this.Collection.Install.Contains(this))
					{
						empty = "install";
					}
					else if (this.Collection.Commit.Contains(this))
					{
						empty = "commit";
					}
					else if (this.Collection.Rollback.Contains(this))
					{
						empty = "rollback";
					}
					else if (this.Collection.Uninstall.Contains(this))
					{
						empty = "uninstall";
					}
					if (string.IsNullOrEmpty(empty) && !string.IsNullOrEmpty(this.WiXCustomActionSetProperty.Value))
					{
						List<string> strs = new List<string>(this.WiXCustomActionSetProperty.Value.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
						strs.RemoveAll((string e) => !e.StartsWith("/"));
						strs.RemoveAll((string e) => e.ToLower().StartsWith("/installtype"));
						strs.RemoveAll((string e) => e.ToLower().StartsWith("/logfile"));
						foreach (string str in strs)
						{
							if (!str.ToLower().StartsWith("/action"))
							{
								continue;
							}
							empty = str.Remove(0, "/action".Length);
							if (empty.StartsWith("="))
							{
								empty = empty.Remove(0, 1);
							}
							empty = empty.Trim();
							goto Label0;
						}
					}
				Label0:
					string id = string.Empty;
					if (this.File is VSFile)
					{
						id = (this.File as VSFile).WiXElement.Id;
					}
					if (this.File is VSBinary)
					{
						id = (this.File as VSBinary).WiXElement.Id;
					}
					if (this.File is VSProjectOutputVDProj)
					{
						id = (this.File as VSProjectOutputVDProj).FileId;
					}
					string str1 = "/installtype=notransaction /action={0} /LogFile= {1}\"[#{2}]\" \"[VSDFxConfigFile]\"";
					this.WiXCustomActionSetProperty.Value = string.Format(str1, empty, (string.IsNullOrEmpty(value) ? string.Empty : string.Concat(value, " ")), id);
					return;
				}
			}
		}

		[Browsable(false)]
		[DefaultValue("")]
		[Description("Specifies an entry point within a dll custom action")]
		public virtual string EntryPoint
		{
			get
			{
				if (this.InstallerClass)
				{
					return string.Empty;
				}
				if (this.WiXCustomAction == null)
				{
					return string.Empty;
				}
				return this.WiXCustomAction.DllEntry;
			}
			set
			{
				if (this.InstallerClass)
				{
					return;
				}
				if (this.WiXCustomAction != null)
				{
					this.WiXCustomAction.DllEntry = value;
				}
			}
		}

		internal VSBaseFile File
		{
			get
			{
				return this._vsFile;
			}
		}

		[Browsable(false)]
		[DefaultValue(false)]
		[Description("Specifies whether a custom action is a .NET ProjectInstaller class")]
		public virtual bool InstallerClass
		{
			get
			{
				return this._isManaged;
			}
			set
			{
				if (!value)
				{
					string customActionData = this.CustomActionData;
					if (!string.IsNullOrEmpty(customActionData))
					{
						this.WiXCustomActionSetProperty.Value = customActionData;
					}
					else
					{
						this.DeleteCustomActionSetProperty();
					}
					this.WiXCustomAction.FileKey = string.Empty;
					this.WiXCustomAction.BinaryKey = string.Empty;
					this.WiXCustomAction.DllEntry = string.Empty;
					if (this.File is VSFile)
					{
						this.WiXCustomAction.FileKey = (this.File as VSFile).WiXElement.Id;
					}
					if (this.File is VSProjectOutputVDProj)
					{
						this.WiXCustomAction.FileKey = (this.File as VSProjectOutputVDProj).FileId;
					}
					if (this.File is VSBinary)
					{
						this.WiXCustomAction.BinaryKey = (this.File as VSBinary).WiXElement.Id;
					}
					this._isManaged = value;
				}
				else
				{
					if (!System.IO.File.Exists(this.SourcePath))
					{
						return;
					}
					if (!VSCustomActionBase.IsAssembly(this.SourcePath))
					{
						return;
					}
					if (this.Collection.EnsureManagedCAs())
					{
						string str = this.CustomActionData;
						if (this.WiXCustomActionSetProperty == null)
						{
							this.CreateCustomActionSetProperty();
						}
						this.WiXCustomAction.ExeCommand = string.Empty;
						this.WiXCustomAction.FileKey = string.Empty;
						this.WiXCustomAction.BinaryKey = "InstallUtil";
						this.WiXCustomAction.DllEntry = "ManagedInstall";
						this.WiXCustomActionSetProperty.Property = this.WiXCustomAction.Id;
						string empty = string.Empty;
						if (this.Collection.Install.Contains(this))
						{
							empty = "install";
						}
						else if (this.Collection.Commit.Contains(this))
						{
							empty = "commit";
						}
						else if (this.Collection.Rollback.Contains(this))
						{
							empty = "rollback";
						}
						else if (this.Collection.Uninstall.Contains(this))
						{
							empty = "uninstall";
						}
						string id = string.Empty;
						if (this.File is VSFile)
						{
							id = (this.File as VSFile).WiXElement.Id;
						}
						if (this.File is VSBinary)
						{
							id = (this.File as VSBinary).WiXElement.Id;
						}
						if (this.File is VSProjectOutputVDProj)
						{
							id = (this.File as VSProjectOutputVDProj).FileId;
						}
						string str1 = "/installtype=notransaction /action={0} /LogFile= {1}\"[#{2}]\" \"[VSDFxConfigFile]\"";
						this.WiXCustomActionSetProperty.Value = string.Format(str1, empty, (string.IsNullOrEmpty(str) ? string.Empty : string.Concat(str, " ")), id);
						this._isManaged = value;
						return;
					}
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Specifies the name used in the Custom Actions Editor to identify a selected custom action")]
		[DisplayName("(Name)")]
		[ReadOnly(false)]
		public override string Name
		{
			get
			{
				if (string.IsNullOrEmpty(this.WiXCustomAction.VSName))
				{
					return "Unnamed Custom Action";
				}
				return this.WiXCustomAction.VSName;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this._project.ProjectManager.AddWiXExtensionReference("VDWExtension", false);
					if (this.WiXCustomAction != null)
					{
						this.WiXCustomAction.VSName = value;
					}
					if (this.WiXCustomActionSetProperty != null)
					{
						this.WiXCustomActionSetProperty.VSName = value;
					}
					this.DoPropertyChanged();
				}
			}
		}

		[Browsable(true)]
		[DefaultValue("")]
		[Description("Indicates the path to the custom action source file on the development computer")]
		[ReadOnly(true)]
		public string SourcePath
		{
			get
			{
				if (this.File != null)
				{
					if (this.File is VSFile)
					{
						return (this.File as VSFile).SourcePath;
					}
					if (this.File is VSBinary)
					{
						return (this.File as VSBinary).SourcePath;
					}
					if (this.File is VSProjectOutputVDProj && (this.File as VSProjectOutputVDProj).KeyOutput != null)
					{
						return (this.File as VSProjectOutputVDProj).KeyOutput.SourcePath;
					}
				}
				return string.Empty;
			}
		}

		internal AddinExpress.Installer.WiXDesigner.WiXCustom WiXCustom
		{
			get
			{
				return this._wixCustom;
			}
		}

		internal AddinExpress.Installer.WiXDesigner.WiXCustomAction WiXCustomAction
		{
			get
			{
				return this._wixCustomAction;
			}
		}

		internal AddinExpress.Installer.WiXDesigner.WiXCustomAction WiXCustomActionSetProperty
		{
			get
			{
				return this._wixCustomActionSetProperty;
			}
		}

		internal AddinExpress.Installer.WiXDesigner.WiXCustom WiXCustomSetProperty
		{
			get
			{
				return this._wixCustomSetProperty;
			}
		}

		public VSCustomActionBase()
		{
		}

		public VSCustomActionBase(WiXProjectParser project, VSCustomActions collection, AddinExpress.Installer.WiXDesigner.WiXCustomAction wixElement, AddinExpress.Installer.WiXDesigner.WiXCustomAction wixElementSetProperty, AddinExpress.Installer.WiXDesigner.WiXCustom wixCustom, AddinExpress.Installer.WiXDesigner.WiXCustom wixCustomSetProperty, VSBaseFile vsFile) : this()
		{
			this._project = project;
			this._wixCustomAction = wixElement;
			this._wixCustomActionSetProperty = wixElementSetProperty;
			this._wixCustom = wixCustom;
			this._wixCustomSetProperty = wixCustomSetProperty;
			this._vsFile = vsFile;
			this._collection = collection;
			this._collection.Add(this);
		}

		private void CreateCustomActionSetProperty()
		{
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(this.WiXCustomAction.XmlNode.OwnerDocument, "CustomAction", this.WiXCustomAction.XmlNode.NamespaceURI, new string[] { "Id", "Property" }, new string[] { string.Concat(this.WiXCustomAction.Id, ".SetProperty"), this.WiXCustomAction.Id }, "", false);
			this._wixCustomActionSetProperty = new AddinExpress.Installer.WiXDesigner.WiXCustomAction(this._project, this.WiXCustomAction.Owner, this.WiXCustomAction.Parent, xmlElement);
			(this.WiXCustomAction.Parent as WiXEntity).XmlNode.InsertAfter(xmlElement, this.WiXCustomAction.XmlNode);
			this.WiXCustomAction.Parent.SetDirty();
			XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(this.WiXCustomAction.XmlNode.OwnerDocument, "Custom", this.WiXCustomAction.XmlNode.NamespaceURI, new string[] { "Action", "After" }, new string[] { this.WiXCustomActionSetProperty.Id, this.WiXCustom.After }, "", false);
			this._wixCustomSetProperty = new AddinExpress.Installer.WiXDesigner.WiXCustom(this._project, this.WiXCustom.Owner, this.WiXCustom.Parent, xmlElement1);
			this.WiXCustomSetProperty.Text = this.WiXCustom.Text;
			(this.WiXCustomSetProperty.Parent as WiXEntity).XmlNode.InsertBefore(xmlElement1, this.WiXCustom.XmlNode);
			this.WiXCustomSetProperty.Parent.SetDirty();
			this.WiXCustom.After = this.WiXCustomSetProperty.Action;
		}

		public override void Delete()
		{
			string after;
			string str;
			string after1;
			string str1;
			if (this.WiXCustomAction != null)
			{
				this.WiXCustomAction.Delete();
			}
			if (this.WiXCustomActionSetProperty != null)
			{
				this.WiXCustomActionSetProperty.Delete();
			}
			List<VSCustomActionBase> uninstall = this.Collection.Uninstall;
			if (!uninstall.Contains(this))
			{
				uninstall = this.Collection.Install;
				uninstall.AddRange(this.Collection.Commit);
				uninstall.AddRange(this.Collection.Rollback);
				if (uninstall.Contains(this) && uninstall.Count > uninstall.IndexOf(this) + 1)
				{
					VSCustomActionBase item = uninstall[uninstall.IndexOf(this) + 1];
					if (item.WiXCustomSetProperty != null)
					{
						AddinExpress.Installer.WiXDesigner.WiXCustom wiXCustomSetProperty = item.WiXCustomSetProperty;
						if (this.WiXCustomSetProperty != null)
						{
							str = this.WiXCustomSetProperty.After;
						}
						else
						{
							str = (this.WiXCustom != null ? this.WiXCustom.After : string.Empty);
						}
						wiXCustomSetProperty.After = str;
					}
					else if (item.WiXCustom != null)
					{
						AddinExpress.Installer.WiXDesigner.WiXCustom wiXCustom = item.WiXCustom;
						if (this.WiXCustomSetProperty != null)
						{
							after = this.WiXCustomSetProperty.After;
						}
						else
						{
							after = (this.WiXCustom != null ? this.WiXCustom.After : string.Empty);
						}
						wiXCustom.After = after;
					}
				}
			}
			else if (uninstall.Count > uninstall.IndexOf(this) + 1)
			{
				VSCustomActionBase vSCustomActionBase = uninstall[uninstall.IndexOf(this) + 1];
				if (vSCustomActionBase.WiXCustomSetProperty != null)
				{
					AddinExpress.Installer.WiXDesigner.WiXCustom wiXCustomSetProperty1 = vSCustomActionBase.WiXCustomSetProperty;
					if (this.WiXCustomSetProperty != null)
					{
						str1 = this.WiXCustomSetProperty.After;
					}
					else
					{
						str1 = (this.WiXCustom != null ? this.WiXCustom.After : string.Empty);
					}
					wiXCustomSetProperty1.After = str1;
				}
				else if (vSCustomActionBase.WiXCustom != null)
				{
					AddinExpress.Installer.WiXDesigner.WiXCustom wiXCustom1 = vSCustomActionBase.WiXCustom;
					if (this.WiXCustomSetProperty != null)
					{
						after1 = this.WiXCustomSetProperty.After;
					}
					else
					{
						after1 = (this.WiXCustom != null ? this.WiXCustom.After : string.Empty);
					}
					wiXCustom1.After = after1;
				}
			}
			if (this.WiXCustom != null)
			{
				this.WiXCustom.Delete();
			}
			if (this.WiXCustomSetProperty != null)
			{
				this.WiXCustomSetProperty.Delete();
			}
			this.Collection.Remove(this);
		}

		private void DeleteCustomActionSetProperty()
		{
			if (this.WiXCustomActionSetProperty != null)
			{
				if (this.WiXCustomSetProperty != null)
				{
					this.WiXCustom.After = this.WiXCustomSetProperty.After;
					this.WiXCustomSetProperty.Delete();
					this._wixCustomSetProperty = null;
				}
				this.WiXCustomActionSetProperty.Delete();
				this._wixCustomActionSetProperty = null;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._project = null;
				this._wixCustomAction = null;
				this._wixCustomActionSetProperty = null;
				this._wixCustom = null;
				this._wixCustomSetProperty = null;
				this._vsFile = null;
				this._collection = null;
			}
			base.Dispose(disposing);
		}

		protected override string GetClassName()
		{
			return "Custom Action Properties";
		}

		protected override string GetComponentName()
		{
			return this.Name;
		}

		internal static bool IsAssembly(string filePath)
		{
			bool flag;
			try
			{
				AssemblyName.GetAssemblyName(filePath);
				flag = true;
			}
			catch
			{
				return false;
			}
			return flag;
		}
	}
}