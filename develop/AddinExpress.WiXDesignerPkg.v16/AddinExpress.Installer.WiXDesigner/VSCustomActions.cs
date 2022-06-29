using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSCustomActions : List<VSCustomActionBase>
	{
		private WiXProjectParser _project;

		private WiXCustomAction _customActionCreateConfig;

		private WiXCustom _customCreateConfig;

		private WiXBinary _installUtil32;

		private WiXBinary _installUtil64;

		private WiXBinary _installUtilConfig;

		private WiXBinary _ADXDPCADLL32;

		private WiXBinary _ADXDPCADLL64;

		private WiXEntity _error1001;

		internal List<VSCustomActionBase> Commit
		{
			get
			{
				return base.FindAll((VSCustomActionBase e) => e.WiXCustomAction.Execute == "commit");
			}
		}

		internal List<VSCustomActionBase> Install
		{
			get
			{
				List<VSCustomActionBase> vSCustomActionBases = new List<VSCustomActionBase>();
				List<VSCustomActionBase> vSCustomActionBases1 = base.FindAll((VSCustomActionBase e) => e.WiXCustomAction.Execute == "deferred");
				if (vSCustomActionBases1 != null && vSCustomActionBases1.Count > 0)
				{
					vSCustomActionBases = vSCustomActionBases1.FindAll((VSCustomActionBase e) => {
						if (e.WiXCustom == null)
						{
							return false;
						}
						return e.WiXCustom.Text.Contains("NOT REMOVE~=\"ALL\"");
					});
					List<string> allComponentIDs = this._project.FileSystem.GetAllComponentIDs();
					if (allComponentIDs != null && allComponentIDs.Count > 0)
					{
						foreach (string allComponentID in allComponentIDs)
						{
							List<VSCustomActionBase> vSCustomActionBases2 = vSCustomActionBases1.FindAll((VSCustomActionBase e) => {
								if (e.WiXCustom == null)
								{
									return false;
								}
								return e.WiXCustom.Text.Contains(string.Concat("$", allComponentID, ">2"));
							});
							if (vSCustomActionBases2 == null || vSCustomActionBases2.Count <= 0)
							{
								continue;
							}
							foreach (VSCustomActionBase vSCustomActionBase in vSCustomActionBases2)
							{
								if (vSCustomActionBases.Contains(vSCustomActionBase))
								{
									continue;
								}
								vSCustomActionBases.Add(vSCustomActionBase);
							}
						}
					}
				}
				return vSCustomActionBases;
			}
		}

		internal List<VSCustomActionBase> Rollback
		{
			get
			{
				return base.FindAll((VSCustomActionBase e) => e.WiXCustomAction.Execute == "rollback");
			}
		}

		internal List<VSCustomActionBase> Uninstall
		{
			get
			{
				List<VSCustomActionBase> vSCustomActionBases = new List<VSCustomActionBase>();
				List<VSCustomActionBase> vSCustomActionBases1 = base.FindAll((VSCustomActionBase e) => e.WiXCustomAction.Execute == "deferred");
				if (vSCustomActionBases1 != null && vSCustomActionBases1.Count > 0)
				{
					vSCustomActionBases = vSCustomActionBases1.FindAll((VSCustomActionBase e) => {
						if (e.WiXCustom == null)
						{
							return false;
						}
						return e.WiXCustom.Text.Contains("REMOVE~=\"ALL\" AND ProductState <> 1");
					});
					List<string> allComponentIDs = this._project.FileSystem.GetAllComponentIDs();
					if (allComponentIDs != null && allComponentIDs.Count > 0)
					{
						foreach (string allComponentID in allComponentIDs)
						{
							List<VSCustomActionBase> vSCustomActionBases2 = vSCustomActionBases1.FindAll((VSCustomActionBase e) => {
								if (e.WiXCustom == null)
								{
									return false;
								}
								return e.WiXCustom.Text.Contains(string.Concat("$", allComponentID, "=2"));
							});
							if (vSCustomActionBases2 == null || vSCustomActionBases2.Count <= 0)
							{
								continue;
							}
							foreach (VSCustomActionBase vSCustomActionBase in vSCustomActionBases2)
							{
								if (vSCustomActionBases.Contains(vSCustomActionBase))
								{
									continue;
								}
								vSCustomActionBases.Add(vSCustomActionBase);
							}
						}
					}
				}
				return vSCustomActionBases;
			}
		}

		public VSCustomActions(WiXProjectParser project)
		{
			this._project = project;
		}

		internal VSCustomActionBase Add(int kind)
		{
			string empty = string.Empty;
			VSBaseFile vSBaseFile = this.ShowDialog(ref empty);
			if (vSBaseFile == null)
			{
				return null;
			}
			if (!(vSBaseFile is VSProjectOutputVDProj) || File.Exists((vSBaseFile as VSProjectOutputVDProj).KeyOutput.SourcePath))
			{
				return this.Add(kind, vSBaseFile, empty);
			}
			Common.ShowMessage(null, "The Custom Action cannot be added. You need to build the project before adding its Primary Output as the Custom Action.", Common.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return null;
		}

		internal List<VSCustomActionBase> Add()
		{
			List<VSCustomActionBase> vSCustomActionBases = new List<VSCustomActionBase>();
			string empty = string.Empty;
			VSBaseFile vSBaseFile = this.ShowDialog(ref empty);
			if (vSBaseFile != null)
			{
				if (vSBaseFile is VSProjectOutputVDProj && !File.Exists((vSBaseFile as VSProjectOutputVDProj).KeyOutput.SourcePath))
				{
					Common.ShowMessage(null, "The Custom Action cannot be added. You need to build the project before adding its Primary Output as the Custom Action.", Common.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return vSCustomActionBases;
				}
				VSCustomActionBase vSCustomActionBase = this.Add(0, vSBaseFile, empty);
				VSCustomActionBase vSCustomActionBase1 = this.Add(1, vSBaseFile, empty);
				VSCustomActionBase vSCustomActionBase2 = this.Add(2, vSBaseFile, empty);
				VSCustomActionBase vSCustomActionBase3 = this.Add(3, vSBaseFile, empty);
				vSCustomActionBases.Add(vSCustomActionBase);
				vSCustomActionBases.Add(vSCustomActionBase1);
				vSCustomActionBases.Add(vSCustomActionBase2);
				vSCustomActionBases.Add(vSCustomActionBase3);
			}
			return vSCustomActionBases;
		}

		private VSCustomActionBase Add(int kind, VSBaseFile file, string sourcePath)
		{
			this.FixCustomActions();
			this.SortCustomActions();
			string empty = string.Empty;
			string fileId = string.Empty;
			string id = string.Empty;
			string str = string.Empty;
			if (file is VSProjectOutputVDProj)
			{
				empty = (file as VSProjectOutputVDProj).Name;
				fileId = (file as VSProjectOutputVDProj).FileId;
				str = string.Concat("com", fileId);
			}
			else if (file is VSFile)
			{
				empty = (file as VSFile).TargetName;
				fileId = (file as VSFile).WiXElement.Id;
				str = (file as VSFile).ParentComponent.Id;
			}
			else if (file is VSBinary)
			{
				empty = (file as VSBinary).Name;
				id = (file as VSBinary).WiXElement.Id;
			}
			string empty1 = string.Empty;
			string str1 = string.Empty;
			if (kind == 0)
			{
				empty1 = "deferred";
				str1 = "Install";
			}
			else if (kind == 1)
			{
				empty1 = "commit";
				str1 = "Commit";
			}
			else if (kind == 2)
			{
				empty1 = "rollback";
				str1 = "Rollback";
			}
			else if (kind == 3)
			{
				empty1 = "deferred";
				str1 = "Uninstall";
			}
			WiXEntity mainEntity = this.GetMainEntity(this._project);
			if (mainEntity == null)
			{
				return null;
			}
			VSCustomActionBase vSCustomActionScript = null;
			if (!VSCustomActionBase.IsAssembly(sourcePath))
			{
				WiXCustomAction wiXCustomAction = null;
				WiXCustom wiXCustom = null;
				WiXEntity wiXEntity = Common.FindChild(mainEntity, "CustomAction", false) ?? Common.FindChild(mainEntity, "Package", false);
				if (wiXEntity != null)
				{
					XmlElement xmlElement = Common.CreateXmlElementWithAttributes(wiXEntity.XmlNode.OwnerDocument, "CustomAction", wiXEntity.XmlNode.NamespaceURI, new string[] { "Id", "Execute" }, new string[] { string.Concat("_", Common.GenerateId(this._project.ProjectType)), empty1 }, "", false);
					wiXCustomAction = new WiXCustomAction(this._project, wiXEntity.Owner, wiXEntity.Parent, xmlElement);
					if (this._project.ProjectManager.ProjectProperties.InstallAllUsers && (sourcePath.EndsWith(".exe") || sourcePath.EndsWith(".dll")))
					{
						wiXCustomAction.Impersonate = "no";
					}
					if (!string.IsNullOrEmpty(fileId))
					{
						wiXCustomAction.FileKey = fileId;
					}
					if (!string.IsNullOrEmpty(id))
					{
						wiXCustomAction.BinaryKey = id;
					}
					wiXEntity.XmlNode.ParentNode.InsertAfter(xmlElement, wiXEntity.XmlNode);
					wiXCustomAction.Parent.SetDirty();
				}
				if (wiXCustomAction == null)
				{
					return null;
				}
				WiXEntity wiXEntity1 = Common.FindChild(mainEntity, "InstallExecuteSequence", false);
				if (wiXEntity1 == null)
				{
					XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(mainEntity.XmlNode.OwnerDocument, "InstallExecuteSequence", mainEntity.XmlNode.NamespaceURI, new string[0], new string[0], "", false);
					mainEntity.XmlNode.AppendChild(xmlElement1);
					wiXEntity1 = new WiXEntity(this._project, mainEntity.Owner, mainEntity, xmlElement1);
					mainEntity.SetDirty();
				}
				if (wiXEntity1 != null)
				{
					XmlElement xmlElement2 = Common.CreateXmlElementWithAttributes(wiXEntity1.XmlNode.OwnerDocument, "Custom", wiXEntity1.XmlNode.NamespaceURI, new string[] { "Action" }, new string[] { wiXCustomAction.Id }, "", false);
					wiXCustom = new WiXCustom(this._project, wiXEntity1.Owner, wiXEntity1, xmlElement2);
					if (kind != 3)
					{
						List<VSCustomActionBase> vSCustomActionBases = new List<VSCustomActionBase>(this);
						vSCustomActionBases.RemoveAll((VSCustomActionBase e) => this.Uninstall.Contains(e));
						if (vSCustomActionBases.Count <= 0)
						{
							wiXCustom.After = "StartServices";
							wiXEntity1.XmlNode.AppendChild(xmlElement2);
						}
						else
						{
							wiXCustom.After = vSCustomActionBases[vSCustomActionBases.Count - 1].WiXCustom.Action;
							wiXEntity1.XmlNode.InsertAfter(xmlElement2, vSCustomActionBases[vSCustomActionBases.Count - 1].WiXCustom.XmlNode);
						}
						if (string.IsNullOrEmpty(str))
						{
							wiXCustom.Text = "NOT REMOVE~=\"ALL\"";
						}
						else
						{
							wiXCustom.Text = string.Concat("$", str, ">2");
						}
					}
					else
					{
						List<VSCustomActionBase> uninstall = this.Uninstall;
						if (uninstall.Count <= 0)
						{
							wiXCustom.After = "MsiUnpublishAssemblies";
							wiXEntity1.XmlNode.AppendChild(xmlElement2);
						}
						else
						{
							wiXCustom.After = uninstall[uninstall.Count - 1].WiXCustom.Action;
							wiXEntity1.XmlNode.InsertAfter(xmlElement2, uninstall[uninstall.Count - 1].WiXCustom.XmlNode);
						}
						if (string.IsNullOrEmpty(str))
						{
							wiXCustom.Text = "REMOVE~=\"ALL\" AND ProductState <> 1";
						}
						else
						{
							wiXCustom.Text = string.Concat("$", str, "=2");
						}
					}
					wiXEntity1.SetDirty();
				}
				if (wiXCustomAction != null && wiXCustom != null)
				{
					if (sourcePath.EndsWith(".vbs") || sourcePath.EndsWith(".js"))
					{
						vSCustomActionScript = new VSCustomActionScript(this._project, this, wiXCustomAction, null, wiXCustom, null, file)
						{
							Name = empty
						};
						XmlAttribute xmlAttribute = null;
						if (sourcePath.ToLower().EndsWith(".vbs"))
						{
							xmlAttribute = vSCustomActionScript.WiXCustomAction.XmlNode.OwnerDocument.CreateAttribute("VBScriptCall");
						}
						if (sourcePath.ToLower().EndsWith(".js"))
						{
							xmlAttribute = vSCustomActionScript.WiXCustomAction.XmlNode.OwnerDocument.CreateAttribute("JScriptCall");
						}
						if (xmlAttribute != null)
						{
							xmlAttribute.Value = "";
							vSCustomActionScript.WiXCustomAction.XmlNode.Attributes.Append(xmlAttribute);
						}
					}
					if (sourcePath.EndsWith(".dll"))
					{
						vSCustomActionScript = new VSCustomActionDLL(this._project, this, wiXCustomAction, null, wiXCustom, null, file)
						{
							Name = empty
						};
						wiXCustomAction.DllEntry = str1;
					}
					if (sourcePath.EndsWith(".exe") || sourcePath.EndsWith(".bat"))
					{
						vSCustomActionScript = new VSCustomActionEXE(this._project, this, wiXCustomAction, null, wiXCustom, null, file)
						{
							Name = empty
						};
						wiXCustomAction.ExeCommand = string.Concat("/", str1);
					}
				}
			}
			else if (this.EnsureManagedCAs())
			{
				WiXCustomAction wiXCustomAction1 = null;
				WiXCustomAction wiXCustomAction2 = null;
				WiXCustom text = null;
				WiXCustom action = null;
				WiXEntity wiXEntity2 = Common.FindChild(mainEntity, "CustomAction", false) ?? Common.FindChild(mainEntity, "Package", false);
				if (wiXEntity2 != null)
				{
					XmlElement xmlElement3 = Common.CreateXmlElementWithAttributes(wiXEntity2.XmlNode.OwnerDocument, "CustomAction", wiXEntity2.XmlNode.NamespaceURI, new string[] { "Id", "Execute", "BinaryKey", "DllEntry" }, new string[] { string.Concat("_", Common.GenerateId(this._project.ProjectType), ".", str1), empty1, "InstallUtil", "ManagedInstall" }, "", false);
					wiXCustomAction1 = new WiXCustomAction(this._project, wiXEntity2.Owner, wiXEntity2.Parent, xmlElement3);
					if (this._project.ProjectManager.ProjectProperties.InstallAllUsers)
					{
						wiXCustomAction1.Impersonate = "no";
					}
					wiXEntity2.XmlNode.ParentNode.InsertAfter(xmlElement3, wiXEntity2.XmlNode);
					wiXCustomAction1.Parent.SetDirty();
				}
				if (wiXCustomAction1 != null)
				{
					XmlDocument ownerDocument = wiXEntity2.XmlNode.OwnerDocument;
					string namespaceURI = wiXEntity2.XmlNode.NamespaceURI;
					string[] strArrays = new string[] { "Id", "Property", "Value" };
					string[] strArrays1 = new string[] { string.Concat(wiXCustomAction1.Id, ".SetProperty"), wiXCustomAction1.Id, null };
					strArrays1[2] = string.Concat(new string[] { "/installtype=notransaction /action=", str1.ToLower(), " /LogFile= \"[#", fileId, "]\" \"[VSDFxConfigFile]\"" });
					XmlElement xmlElement4 = Common.CreateXmlElementWithAttributes(ownerDocument, "CustomAction", namespaceURI, strArrays, strArrays1, "", false);
					wiXCustomAction2 = new WiXCustomAction(this._project, wiXEntity2.Owner, wiXEntity2.Parent, xmlElement4);
					wiXEntity2.XmlNode.ParentNode.InsertAfter(xmlElement4, wiXCustomAction1.XmlNode);
					wiXCustomAction2.Parent.SetDirty();
				}
				if (wiXCustomAction1 != null && wiXCustomAction2 != null)
				{
					WiXEntity wiXEntity3 = Common.FindChild(mainEntity, "InstallExecuteSequence", false);
					if (wiXEntity3 == null)
					{
						XmlElement xmlElement5 = Common.CreateXmlElementWithAttributes(mainEntity.XmlNode.OwnerDocument, "InstallExecuteSequence", mainEntity.XmlNode.NamespaceURI, new string[0], new string[0], "", false);
						mainEntity.XmlNode.AppendChild(xmlElement5);
						wiXEntity3 = new WiXEntity(this._project, mainEntity.Owner, mainEntity, xmlElement5);
						mainEntity.SetDirty();
					}
					if (wiXEntity3 != null)
					{
						XmlElement xmlElement6 = Common.CreateXmlElementWithAttributes(wiXEntity3.XmlNode.OwnerDocument, "Custom", wiXEntity3.XmlNode.NamespaceURI, new string[] { "Action", "After" }, new string[] { wiXCustomAction1.Id, wiXCustomAction2.Id }, "", false);
						text = new WiXCustom(this._project, wiXEntity3.Owner, wiXEntity3, xmlElement6);
						XmlElement xmlElement7 = Common.CreateXmlElementWithAttributes(wiXEntity3.XmlNode.OwnerDocument, "Custom", wiXEntity3.XmlNode.NamespaceURI, new string[] { "Action" }, new string[] { wiXCustomAction2.Id }, "", false);
						action = new WiXCustom(this._project, wiXEntity3.Owner, wiXEntity3, xmlElement7);
						if (kind != 3)
						{
							List<VSCustomActionBase> vSCustomActionBases1 = new List<VSCustomActionBase>(this);
							vSCustomActionBases1.RemoveAll((VSCustomActionBase e) => this.Uninstall.Contains(e));
							if (vSCustomActionBases1.Count <= 0)
							{
								action.After = "StartServices";
								wiXEntity3.XmlNode.AppendChild(xmlElement7);
								wiXEntity3.XmlNode.AppendChild(xmlElement6);
							}
							else
							{
								action.After = vSCustomActionBases1[vSCustomActionBases1.Count - 1].WiXCustom.Action;
								wiXEntity3.XmlNode.InsertAfter(xmlElement6, vSCustomActionBases1[vSCustomActionBases1.Count - 1].WiXCustom.XmlNode);
								wiXEntity3.XmlNode.InsertAfter(xmlElement7, vSCustomActionBases1[vSCustomActionBases1.Count - 1].WiXCustom.XmlNode);
							}
							if (string.IsNullOrEmpty(str))
							{
								action.Text = "NOT REMOVE~=\"ALL\"";
								text.Text = action.Text;
							}
							else
							{
								action.Text = string.Concat("$", str, ">2");
								text.Text = string.Concat("$", str, ">2");
							}
						}
						else
						{
							List<VSCustomActionBase> uninstall1 = this.Uninstall;
							if (uninstall1.Count <= 0)
							{
								action.After = "MsiUnpublishAssemblies";
								wiXEntity3.XmlNode.AppendChild(xmlElement7);
								wiXEntity3.XmlNode.AppendChild(xmlElement6);
							}
							else
							{
								action.After = uninstall1[uninstall1.Count - 1].WiXCustom.Action;
								wiXEntity3.XmlNode.InsertAfter(xmlElement6, uninstall1[uninstall1.Count - 1].WiXCustom.XmlNode);
								wiXEntity3.XmlNode.InsertAfter(xmlElement7, uninstall1[uninstall1.Count - 1].WiXCustom.XmlNode);
							}
							if (string.IsNullOrEmpty(str))
							{
								action.Text = "REMOVE~=\"ALL\" AND ProductState <> 1";
								text.Text = action.Text;
							}
							else
							{
								action.Text = string.Concat("$", str, "=2");
								text.Text = string.Concat("$", str, "=2");
							}
						}
						wiXEntity3.SetDirty();
					}
				}
				if (wiXCustomAction1 != null && wiXCustomAction2 != null && text != null && action != null)
				{
					if (sourcePath.EndsWith(".dll"))
					{
						vSCustomActionScript = new VSCustomActionDLL(this._project, this, wiXCustomAction1, wiXCustomAction2, text, action, file)
						{
							Name = empty,
							_isManaged = true
						};
					}
					if (sourcePath.EndsWith(".exe"))
					{
						vSCustomActionScript = new VSCustomActionEXE(this._project, this, wiXCustomAction1, wiXCustomAction2, text, action, file)
						{
							Name = empty,
							_isManaged = true
						};
					}
				}
			}
			return vSCustomActionScript;
		}

		public new void Clear()
		{
			base.Clear();
			this._customActionCreateConfig = null;
			this._customCreateConfig = null;
			this._installUtil32 = null;
			this._installUtil64 = null;
			this._installUtilConfig = null;
			this._ADXDPCADLL32 = null;
			this._ADXDPCADLL64 = null;
			this._error1001 = null;
		}

		private void CreateADXDPCA(ref WiXBinary dpca32, ref WiXBinary dpca64)
		{
			if (dpca32 == null)
			{
				VSBinary vSBinary = this._project.Binaries.Find((VSBinary e) => {
					if (e.WiXElement == null || !(e.WiXElement.Id == "ADXDPCADLL"))
					{
						return false;
					}
					if (string.IsNullOrEmpty(e.WiXElement.SourceFile))
					{
						return false;
					}
					return e.WiXElement.SourceFile.ToLower().EndsWith("adxdpca.dll");
				});
				if (vSBinary == null)
				{
					dpca32 = this._project.SupportedEntities.Find((WiXEntity e) => {
						if (!(e is WiXBinary) || !((e as WiXBinary).Id == "ADXDPCADLL"))
						{
							return false;
						}
						if (string.IsNullOrEmpty((e as WiXBinary).SourceFile))
						{
							return false;
						}
						return (e as WiXBinary).SourceFile.ToLower().EndsWith("adxdpca.dll");
					}) as WiXBinary;
				}
				else
				{
					dpca32 = vSBinary.WiXElement;
				}
			}
			if (dpca64 == null)
			{
				VSBinary vSBinary1 = this._project.Binaries.Find((VSBinary e) => {
					if (e.WiXElement == null || !(e.WiXElement.Id == "ADXDPCADLL"))
					{
						return false;
					}
					if (string.IsNullOrEmpty(e.WiXElement.SourceFile))
					{
						return false;
					}
					return e.WiXElement.SourceFile.ToLower().EndsWith("adxdpca64.dll");
				});
				if (vSBinary1 == null)
				{
					dpca64 = this._project.SupportedEntities.Find((WiXEntity e) => {
						if (!(e is WiXBinary) || !((e as WiXBinary).Id == "ADXDPCADLL"))
						{
							return false;
						}
						if (string.IsNullOrEmpty((e as WiXBinary).SourceFile))
						{
							return false;
						}
						return (e as WiXBinary).SourceFile.ToLower().EndsWith("adxdpca64.dll");
					}) as WiXBinary;
				}
				else
				{
					dpca64 = vSBinary1.WiXElement;
				}
			}
			if (dpca32 != null && dpca64 != null)
			{
				return;
			}
			if (dpca32 == null || dpca64 == null)
			{
				if (dpca32 != null)
				{
					if (!this._project.SupportedEntities.Remove(dpca32))
					{
						VSBinary vSBinary2 = this._project.Binaries.Find((VSBinary e) => {
							if (e.WiXElement == null || !(e.WiXElement.Id == "ADXDPCADLL"))
							{
								return false;
							}
							if (string.IsNullOrEmpty(e.WiXElement.SourceFile))
							{
								return false;
							}
							return e.WiXElement.SourceFile.ToLower().EndsWith("adxdpca.dll");
						});
						if (vSBinary2 != null)
						{
							this._project.Binaries.Remove(vSBinary2);
						}
					}
					dpca32.XmlNode.ParentNode.RemoveChild(dpca32.XmlNode);
					dpca32.Parent.SetDirty();
					dpca32 = null;
				}
				if (dpca64 != null)
				{
					if (!this._project.SupportedEntities.Remove(dpca64))
					{
						VSBinary vSBinary3 = this._project.Binaries.Find((VSBinary e) => {
							if (e.WiXElement == null || !(e.WiXElement.Id == "ADXDPCADLL"))
							{
								return false;
							}
							if (string.IsNullOrEmpty(e.WiXElement.SourceFile))
							{
								return false;
							}
							return e.WiXElement.SourceFile.ToLower().EndsWith("adxdpca64.dll");
						});
						if (vSBinary3 != null)
						{
							this._project.Binaries.Remove(vSBinary3);
						}
					}
					dpca64.XmlNode.ParentNode.RemoveChild(dpca64.XmlNode);
					dpca64.Parent.SetDirty();
					dpca64 = null;
				}
			}
			WiXEntity mainEntity = this.GetMainEntity(this._project);
			if (mainEntity != null)
			{
				string str = Path.Combine(Path.GetDirectoryName((mainEntity.Owner as WiXProjectItem).SourceFile), Path.Combine("Resources", "Binary"));
				if (!Directory.Exists(str))
				{
					Directory.CreateDirectory(str);
				}
				string str1 = Path.Combine(str, "adxdpca.dll");
				try
				{
					File.WriteAllBytes(str1, Resources.adxdpca);
				}
				catch
				{
				}
				string str2 = Path.Combine(str, "adxdpca64.dll");
				try
				{
					File.WriteAllBytes(str2, Resources.adxdpca64);
				}
				catch
				{
				}
				if (Common.FindChild(mainEntity, "Package", false) != null)
				{
					bool flag = true;
					WiXEntity wiXEntity = Common.FindChild(mainEntity, "Binary", false);
					if (wiXEntity != null)
					{
						WiXEntity previousSibling = wiXEntity.PreviousSibling as WiXEntity;
						if (previousSibling != null && previousSibling.XmlNode is XmlProcessingInstruction && !string.IsNullOrEmpty((previousSibling.XmlNode as XmlProcessingInstruction).Data) && (previousSibling.XmlNode as XmlProcessingInstruction).Data.ToLower().Contains("$(var.platform)=x86"))
						{
							wiXEntity = previousSibling;
							flag = false;
						}
					}
					else
					{
						wiXEntity = Common.FindChild(mainEntity, "Package", false);
					}
					if (wiXEntity != null)
					{
						XmlProcessingInstruction xmlProcessingInstruction = wiXEntity.XmlNode.OwnerDocument.CreateProcessingInstruction("if", "$(var.Platform)=x86 ");
						if (!flag)
						{
							wiXEntity.XmlNode.ParentNode.InsertBefore(xmlProcessingInstruction, wiXEntity.XmlNode);
						}
						else
						{
							wiXEntity.XmlNode.ParentNode.InsertAfter(xmlProcessingInstruction, wiXEntity.XmlNode);
						}
						wiXEntity = new WiXEntity(wiXEntity.Project, wiXEntity.Owner, wiXEntity.Parent, xmlProcessingInstruction);
						XmlElement xmlElement = Common.CreateXmlElementWithAttributes(mainEntity.XmlNode.OwnerDocument, "Binary", mainEntity.XmlNode.NamespaceURI, new string[] { "Id", "SourceFile" }, new string[] { "ADXDPCADLL", "Resources\\Binary\\adxdpca.dll" }, "", false);
						wiXEntity.XmlNode.ParentNode.InsertAfter(xmlElement, wiXEntity.XmlNode);
						dpca32 = new WiXBinary(this._project, wiXEntity.Owner, wiXEntity.Parent, xmlElement);
						wiXEntity = dpca32;
						xmlProcessingInstruction = wiXEntity.XmlNode.OwnerDocument.CreateProcessingInstruction("else", "");
						wiXEntity.XmlNode.ParentNode.InsertAfter(xmlProcessingInstruction, wiXEntity.XmlNode);
						wiXEntity = new WiXEntity(wiXEntity.Project, wiXEntity.Owner, wiXEntity.Parent, xmlProcessingInstruction);
						xmlElement = Common.CreateXmlElementWithAttributes(mainEntity.XmlNode.OwnerDocument, "Binary", mainEntity.XmlNode.NamespaceURI, new string[] { "Id", "SourceFile" }, new string[] { "ADXDPCADLL", "Resources\\Binary\\adxdpca64.dll" }, "", false);
						wiXEntity.XmlNode.ParentNode.InsertAfter(xmlElement, wiXEntity.XmlNode);
						dpca64 = new WiXBinary(this._project, wiXEntity.Owner, wiXEntity.Parent, xmlElement);
						wiXEntity = dpca64;
						xmlProcessingInstruction = wiXEntity.XmlNode.OwnerDocument.CreateProcessingInstruction("endif", "");
						wiXEntity.XmlNode.ParentNode.InsertAfter(xmlProcessingInstruction, wiXEntity.XmlNode);
						wiXEntity = new WiXEntity(wiXEntity.Project, wiXEntity.Owner, wiXEntity.Parent, xmlProcessingInstruction);
						wiXEntity.Parent.SetDirty();
					}
					this._project.ProjectManager.UpdateProjectTree();
				}
			}
		}

		private WiXCustomAction CreateCustomActionCreateConfig()
		{
			WiXCustomAction wiXCustomAction = this._project.SupportedEntities.Find((WiXEntity e) => {
				if (!(e is WiXCustomAction))
				{
					return false;
				}
				if (!((e as WiXCustomAction).Id == "CA_CreateConfig") || !((e as WiXCustomAction).BinaryKey == "ADXDPCADLL"))
				{
					return false;
				}
				return (e as WiXCustomAction).DllEntry == "GetConfig";
			}) as WiXCustomAction;
			if (wiXCustomAction == null)
			{
				WiXEntity mainEntity = this.GetMainEntity(this._project);
				if (mainEntity != null)
				{
					WiXEntity wiXEntity = Common.FindChild(mainEntity, "CustomAction", false) ?? Common.FindChild(mainEntity, "Package", false);
					if (wiXEntity != null)
					{
						XmlElement xmlElement = Common.CreateXmlElementWithAttributes(wiXEntity.XmlNode.OwnerDocument, "CustomAction", wiXEntity.XmlNode.NamespaceURI, new string[] { "Id", "BinaryKey", "DllEntry" }, new string[] { "CA_CreateConfig", "ADXDPCADLL", "GetConfig" }, "", false);
						wiXCustomAction = new WiXCustomAction(this._project, wiXEntity.Owner, wiXEntity.Parent, xmlElement);
						wiXEntity.XmlNode.ParentNode.InsertAfter(xmlElement, wiXEntity.XmlNode);
						wiXCustomAction.Parent.SetDirty();
					}
				}
			}
			return wiXCustomAction;
		}

		private WiXCustom CreateCustomCreateConfig()
		{
			WiXCustom wiXCustom = this._project.SupportedEntities.Find((WiXEntity e) => {
				if (!(e is WiXCustom))
				{
					return false;
				}
				return (e as WiXCustom).Action == "CA_CreateConfig";
			}) as WiXCustom;
			if (wiXCustom == null)
			{
				WiXEntity mainEntity = this.GetMainEntity(this._project);
				if (mainEntity != null)
				{
					WiXEntity wiXEntity = Common.FindChild(mainEntity, "InstallExecuteSequence", false);
					if (wiXEntity == null)
					{
						XmlElement xmlElement = Common.CreateXmlElementWithAttributes(mainEntity.XmlNode.OwnerDocument, "InstallExecuteSequence", mainEntity.XmlNode.NamespaceURI, new string[0], new string[0], "", false);
						mainEntity.XmlNode.AppendChild(xmlElement);
						wiXEntity = new WiXEntity(this._project, mainEntity.Owner, mainEntity, xmlElement);
						mainEntity.SetDirty();
					}
					if (wiXEntity != null)
					{
						XmlElement xmlElement1 = null;
						xmlElement1 = (!(mainEntity is WiXProduct) ? Common.CreateXmlElementWithAttributes(wiXEntity.XmlNode.OwnerDocument, "Custom", wiXEntity.XmlNode.NamespaceURI, new string[] { "Action", "Before" }, new string[] { "CA_CreateConfig", "CostInitialize" }, "", false) : Common.CreateXmlElementWithAttributes(wiXEntity.XmlNode.OwnerDocument, "Custom", wiXEntity.XmlNode.NamespaceURI, new string[] { "Action", "Sequence" }, new string[] { "CA_CreateConfig", "1" }, "", false));
						wiXCustom = new WiXCustom(this._project, wiXEntity.Owner, wiXEntity, xmlElement1);
						if (wiXEntity.XmlNode.ChildNodes.Count <= 0)
						{
							wiXEntity.XmlNode.AppendChild(xmlElement1);
						}
						else
						{
							wiXEntity.XmlNode.InsertBefore(xmlElement1, wiXEntity.XmlNode.ChildNodes[0]);
						}
						wiXEntity.Parent.SetDirty();
					}
				}
			}
			return wiXCustom;
		}

		private WiXEntity CreateError1001()
		{
			WiXError wiXError = this._project.SupportedEntities.Find((WiXEntity e) => {
				if (!(e is WiXError))
				{
					return false;
				}
				return (e as WiXError).Id == "1001";
			}) as WiXError;
			if (wiXError == null)
			{
				WiXEntity mainEntity = this.GetMainEntity(this._project);
				if (mainEntity != null)
				{
					WiXEntity wiXEntity = Common.FindChild(mainEntity, "InstallExecuteSequence", false);
					if (wiXEntity == null)
					{
						XmlElement xmlElement = Common.CreateXmlElementWithAttributes(mainEntity.XmlNode.OwnerDocument, "InstallExecuteSequence", mainEntity.XmlNode.NamespaceURI, new string[0], new string[0], "", false);
						mainEntity.XmlNode.AppendChild(xmlElement);
						wiXEntity = new WiXEntity(this._project, mainEntity.Owner, mainEntity, xmlElement);
						mainEntity.SetDirty();
					}
					if (wiXEntity != null)
					{
						XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(mainEntity.XmlNode.OwnerDocument, "UI", mainEntity.XmlNode.NamespaceURI, new string[0], new string[0], "", false);
						WiXEntity wiXEntity1 = new WiXEntity(this._project, mainEntity.Owner, mainEntity, xmlElement1);
						mainEntity.XmlNode.InsertAfter(xmlElement1, wiXEntity.XmlNode);
						mainEntity.SetDirty();
						XmlElement xmlElement2 = Common.CreateXmlElementWithAttributes(mainEntity.XmlNode.OwnerDocument, "Error", mainEntity.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { "1001" }, "", false);
						xmlElement1.AppendChild(xmlElement2);
						wiXError = new WiXError(this._project, wiXEntity1.Owner, wiXEntity1, xmlElement2)
						{
							Text = "Error [1]. [2]"
						};
						mainEntity.SetDirty();
					}
				}
			}
			return wiXError;
		}

		private void CreateFileFromTemplate(string filePath, string templateString)
		{
			using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
			{
				using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
				{
					streamWriter.Write(templateString);
				}
			}
		}

		private void CreateInstallUtil(ref WiXBinary util32, ref WiXBinary util64)
		{
			if (util32 == null)
			{
				VSBinary vSBinary = this._project.Binaries.Find((VSBinary e) => {
					if (e.WiXElement == null || !(e.WiXElement.Id == "InstallUtil"))
					{
						return false;
					}
					if (string.IsNullOrEmpty(e.WiXElement.SourceFile))
					{
						return false;
					}
					return e.WiXElement.SourceFile.EndsWith("InstallUtil");
				});
				if (vSBinary == null)
				{
					util32 = this._project.SupportedEntities.Find((WiXEntity e) => {
						if (!(e is WiXBinary) || !((e as WiXBinary).Id == "InstallUtil"))
						{
							return false;
						}
						if (string.IsNullOrEmpty((e as WiXBinary).SourceFile))
						{
							return false;
						}
						return (e as WiXBinary).SourceFile.EndsWith("InstallUtil");
					}) as WiXBinary;
				}
				else
				{
					util32 = vSBinary.WiXElement;
				}
			}
			if (util64 == null)
			{
				VSBinary vSBinary1 = this._project.Binaries.Find((VSBinary e) => {
					if (e.WiXElement == null || !(e.WiXElement.Id == "InstallUtil"))
					{
						return false;
					}
					if (string.IsNullOrEmpty(e.WiXElement.SourceFile))
					{
						return false;
					}
					return e.WiXElement.SourceFile.EndsWith("InstallUtil64");
				});
				if (vSBinary1 == null)
				{
					util64 = this._project.SupportedEntities.Find((WiXEntity e) => {
						if (!(e is WiXBinary) || !((e as WiXBinary).Id == "InstallUtil"))
						{
							return false;
						}
						if (string.IsNullOrEmpty((e as WiXBinary).SourceFile))
						{
							return false;
						}
						return (e as WiXBinary).SourceFile.EndsWith("InstallUtil64");
					}) as WiXBinary;
				}
				else
				{
					util64 = vSBinary1.WiXElement;
				}
			}
			if (util32 != null && util64 != null)
			{
				return;
			}
			if (util32 == null || util64 == null)
			{
				if (util32 != null)
				{
					if (!this._project.SupportedEntities.Remove(util32))
					{
						VSBinary vSBinary2 = this._project.Binaries.Find((VSBinary e) => {
							if (e.WiXElement == null || !(e.WiXElement.Id == "InstallUtil"))
							{
								return false;
							}
							if (string.IsNullOrEmpty(e.WiXElement.SourceFile))
							{
								return false;
							}
							return e.WiXElement.SourceFile.EndsWith("InstallUtil");
						});
						if (vSBinary2 != null)
						{
							this._project.Binaries.Remove(vSBinary2);
						}
					}
					util32.XmlNode.ParentNode.RemoveChild(util32.XmlNode);
					util32.Parent.SetDirty();
					util32 = null;
				}
				if (util64 != null)
				{
					if (!this._project.SupportedEntities.Remove(util64))
					{
						VSBinary vSBinary3 = this._project.Binaries.Find((VSBinary e) => {
							if (e.WiXElement == null || !(e.WiXElement.Id == "InstallUtil"))
							{
								return false;
							}
							if (string.IsNullOrEmpty(e.WiXElement.SourceFile))
							{
								return false;
							}
							return e.WiXElement.SourceFile.EndsWith("InstallUtil64");
						});
						if (vSBinary3 != null)
						{
							this._project.Binaries.Remove(vSBinary3);
						}
					}
					util64.XmlNode.ParentNode.RemoveChild(util64.XmlNode);
					util64.Parent.SetDirty();
					util64 = null;
				}
			}
			WiXEntity mainEntity = this.GetMainEntity(this._project);
			if (mainEntity != null)
			{
				string str = Path.Combine(Path.GetDirectoryName((mainEntity.Owner as WiXProjectItem).SourceFile), Path.Combine("Resources", "Binary"));
				if (!Directory.Exists(str))
				{
					Directory.CreateDirectory(str);
				}
				string rootFrameworkFolder = this.GetRootFrameworkFolder();
				string str1 = Path.Combine(rootFrameworkFolder, "Framework");
				string str2 = Path.Combine(rootFrameworkFolder, "Framework64");
				string str3 = "v4.0.30319";
				string str4 = Path.Combine(Path.Combine(str1, str3), "InstallUtilLib.dll");
				if (File.Exists(str4))
				{
					try
					{
						File.Copy(str4, Path.Combine(str, "InstallUtil"), true);
					}
					catch
					{
					}
				}
				string str5 = Path.Combine(Path.Combine(str2, str3), "InstallUtilLib.dll");
				if (File.Exists(str5))
				{
					try
					{
						File.Copy(str5, Path.Combine(str, "InstallUtil64"), true);
					}
					catch
					{
					}
				}
				WiXEntity wiXEntity = this._ADXDPCADLL32;
				WiXEntity wiXEntity1 = this._ADXDPCADLL64;
				if (wiXEntity == null || wiXEntity1 == null)
				{
					WiXEntity wiXEntity2 = Common.FindChild(mainEntity, "Package", false);
					if (wiXEntity2 != null)
					{
						XmlProcessingInstruction xmlProcessingInstruction = wiXEntity2.XmlNode.OwnerDocument.CreateProcessingInstruction("if", "$(var.Platform)=x86 ");
						wiXEntity2.XmlNode.ParentNode.InsertAfter(xmlProcessingInstruction, wiXEntity2.XmlNode);
						wiXEntity2 = new WiXEntity(wiXEntity2.Project, wiXEntity2.Owner, wiXEntity2.Parent, xmlProcessingInstruction);
						XmlElement xmlElement = Common.CreateXmlElementWithAttributes(mainEntity.XmlNode.OwnerDocument, "Binary", mainEntity.XmlNode.NamespaceURI, new string[] { "Id", "SourceFile" }, new string[] { "InstallUtil", "Resources\\Binary\\InstallUtil" }, "", false);
						wiXEntity2.XmlNode.ParentNode.InsertAfter(xmlElement, wiXEntity2.XmlNode);
						util32 = new WiXBinary(this._project, wiXEntity2.Owner, wiXEntity2.Parent, xmlElement);
						wiXEntity2 = util32;
						xmlProcessingInstruction = wiXEntity2.XmlNode.OwnerDocument.CreateProcessingInstruction("else", "");
						wiXEntity2.XmlNode.ParentNode.InsertAfter(xmlProcessingInstruction, wiXEntity2.XmlNode);
						wiXEntity2 = new WiXEntity(wiXEntity2.Project, wiXEntity2.Owner, wiXEntity2.Parent, xmlProcessingInstruction);
						xmlElement = Common.CreateXmlElementWithAttributes(mainEntity.XmlNode.OwnerDocument, "Binary", mainEntity.XmlNode.NamespaceURI, new string[] { "Id", "SourceFile" }, new string[] { "InstallUtil", "Resources\\Binary\\InstallUtil64" }, "", false);
						wiXEntity2.XmlNode.ParentNode.InsertAfter(xmlElement, wiXEntity2.XmlNode);
						util64 = new WiXBinary(this._project, wiXEntity2.Owner, wiXEntity2.Parent, xmlElement);
						wiXEntity2 = util64;
						xmlProcessingInstruction = wiXEntity2.XmlNode.OwnerDocument.CreateProcessingInstruction("endif", "");
						wiXEntity2.XmlNode.ParentNode.InsertAfter(xmlProcessingInstruction, wiXEntity2.XmlNode);
						wiXEntity2 = new WiXEntity(wiXEntity2.Project, wiXEntity2.Owner, wiXEntity2.Parent, xmlProcessingInstruction);
						wiXEntity2.Parent.SetDirty();
					}
				}
				else
				{
					XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(mainEntity.XmlNode.OwnerDocument, "Binary", mainEntity.XmlNode.NamespaceURI, new string[] { "Id", "SourceFile" }, new string[] { "InstallUtil", "Resources\\Binary\\InstallUtil" }, "", false);
					wiXEntity.XmlNode.ParentNode.InsertAfter(xmlElement1, wiXEntity.XmlNode);
					util32 = new WiXBinary(this._project, wiXEntity.Owner, wiXEntity.Parent, xmlElement1);
					util32.Parent.SetDirty();
					XmlElement xmlElement2 = Common.CreateXmlElementWithAttributes(mainEntity.XmlNode.OwnerDocument, "Binary", mainEntity.XmlNode.NamespaceURI, new string[] { "Id", "SourceFile" }, new string[] { "InstallUtil", "Resources\\Binary\\InstallUtil64" }, "", false);
					wiXEntity1.XmlNode.ParentNode.InsertAfter(xmlElement2, wiXEntity1.XmlNode);
					util64 = new WiXBinary(this._project, wiXEntity1.Owner, wiXEntity1.Parent, xmlElement2);
					util64.Parent.SetDirty();
				}
				this._project.ProjectManager.UpdateProjectTree();
			}
		}

		private WiXBinary CreateInstallUtilConfig()
		{
			WiXBinary wiXBinary = null;
			VSBinary vSBinary = this._project.Binaries.Find((VSBinary e) => {
				if (e.WiXElement == null)
				{
					return false;
				}
				return e.WiXElement.Id == "InstallUtilConfig";
			});
			wiXBinary = (vSBinary == null ? this._project.SupportedEntities.Find((WiXEntity e) => {
				if (!(e is WiXBinary))
				{
					return false;
				}
				return (e as WiXBinary).Id == "InstallUtilConfig";
			}) as WiXBinary : vSBinary.WiXElement);
			if (wiXBinary == null)
			{
				WiXEntity mainEntity = this.GetMainEntity(this._project);
				if (mainEntity != null)
				{
					string str = Path.Combine(Path.GetDirectoryName((mainEntity.Owner as WiXProjectItem).SourceFile), Path.Combine("Resources", "Binary"));
					if (!Directory.Exists(str))
					{
						Directory.CreateDirectory(str);
					}
					string str1 = Path.Combine(str, "InstallUtilConfig");
					this.CreateFileFromTemplate(str1, Resources.VSDNETCFG_v4.Replace("$supportedRuntimeVersion$", "v4.0"));
					WiXEntity wiXEntity = Common.FindChild(mainEntity, "Package", false);
					if (wiXEntity != null)
					{
						XmlElement xmlElement = Common.CreateXmlElementWithAttributes(mainEntity.XmlNode.OwnerDocument, "Binary", mainEntity.XmlNode.NamespaceURI, new string[] { "Id", "SourceFile" }, new string[] { "InstallUtilConfig", "Resources\\Binary\\InstallUtilConfig" }, "", false);
						wiXEntity.XmlNode.ParentNode.InsertAfter(xmlElement, wiXEntity.XmlNode);
						wiXBinary = new WiXBinary(this._project, mainEntity.Owner, mainEntity, xmlElement);
						wiXBinary.Parent.SetDirty();
					}
					this._project.ProjectManager.UpdateProjectTree();
				}
			}
			return wiXBinary;
		}

		internal bool EnsureManagedCAs()
		{
			if (this._customActionCreateConfig == null)
			{
				this._customActionCreateConfig = this.CreateCustomActionCreateConfig();
			}
			if (this._customCreateConfig == null)
			{
				this._customCreateConfig = this.CreateCustomCreateConfig();
			}
			if (this._ADXDPCADLL32 == null || this._ADXDPCADLL64 == null)
			{
				this.CreateADXDPCA(ref this._ADXDPCADLL32, ref this._ADXDPCADLL64);
			}
			if (this._installUtil32 == null || this._installUtil64 == null)
			{
				this.CreateInstallUtil(ref this._installUtil32, ref this._installUtil64);
			}
			if (this._installUtilConfig == null)
			{
				this._installUtilConfig = this.CreateInstallUtilConfig();
			}
			if (this._error1001 == null)
			{
				this._error1001 = this.CreateError1001();
			}
			if (this._customActionCreateConfig == null || this._customCreateConfig == null || this._installUtil32 == null && this._installUtil64 == null || this._ADXDPCADLL32 == null && this._ADXDPCADLL64 == null)
			{
				return false;
			}
			return this._installUtilConfig != null;
		}

		private void FixCustomActions()
		{
			List<VSCustomActionBase> install = this.Install;
			install.AddRange(this.Commit);
			install.AddRange(this.Rollback);
			if (install != null && install.Count > 0)
			{
				string action = "StartServices";
				for (int i = 0; i < install.Count; i++)
				{
					if (install[i].WiXCustomSetProperty != null)
					{
						if (string.IsNullOrEmpty(install[i].WiXCustomSetProperty.After))
						{
							install[i].WiXCustomSetProperty.After = action;
							install[i].WiXCustomSetProperty.Before = null;
							install[i].WiXCustomSetProperty.Sequence = null;
							action = install[i].WiXCustomSetProperty.Action;
						}
					}
					else if (install[i].WiXCustom != null && string.IsNullOrEmpty(install[i].WiXCustom.After))
					{
						install[i].WiXCustom.After = action;
						install[i].WiXCustom.Before = null;
						install[i].WiXCustom.Sequence = null;
						action = install[i].WiXCustom.Action;
					}
				}
			}
			install = this.Uninstall;
			if (install != null && install.Count > 0)
			{
				string str = "MsiUnpublishAssemblies";
				for (int j = 0; j < install.Count; j++)
				{
					if (install[j].WiXCustomSetProperty != null)
					{
						if (string.IsNullOrEmpty(install[j].WiXCustomSetProperty.After))
						{
							install[j].WiXCustomSetProperty.After = str;
							install[j].WiXCustomSetProperty.Before = null;
							install[j].WiXCustomSetProperty.Sequence = null;
							str = install[j].WiXCustomSetProperty.Action;
						}
					}
					else if (install[j].WiXCustom != null && string.IsNullOrEmpty(install[j].WiXCustom.After))
					{
						install[j].WiXCustom.After = str;
						install[j].WiXCustom.Before = null;
						install[j].WiXCustom.Sequence = null;
						str = install[j].WiXCustom.Action;
					}
				}
			}
		}

		private WiXEntity GetMainEntity(WiXProjectParser Project)
		{
			if (Project.ProjectType == WiXProjectType.Product)
			{
				return Project.SupportedEntities.Find((WiXEntity e) => e is WiXProduct);
			}
			if (Project.ProjectType != WiXProjectType.Module)
			{
				return null;
			}
			return Project.SupportedEntities.Find((WiXEntity e) => e is WiXModule);
		}

		private string GetRootFrameworkFolder()
		{
			return Path.GetFullPath(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "..\\"), "Microsoft.NET\\"));
		}

		private VSBaseFile ShowDialog(ref string sourcePath)
		{
			VSBaseFile selectedFile;
			FormSelectItemInProject formSelectItemInProject = new FormSelectItemInProject();
			try
			{
				formSelectItemInProject.Initialize(null, string.Empty, this._project.FileSystem, new string[] { "Executable and Script Files (*.exe;*.dll;*.vbs;*.js;*.bat)", "All Files (*.*)" });
				if (formSelectItemInProject.ShowDialog() == DialogResult.OK)
				{
					if (formSelectItemInProject.SelectedFile is VSProjectOutputVDProj)
					{
						if ((formSelectItemInProject.SelectedFile as VSProjectOutputVDProj).KeyOutput != null)
						{
							sourcePath = (formSelectItemInProject.SelectedFile as VSProjectOutputVDProj).KeyOutput.SourcePath.ToLower();
						}
					}
					else if (formSelectItemInProject.SelectedFile is VSFile)
					{
						sourcePath = (formSelectItemInProject.SelectedFile as VSFile).SourcePath;
					}
					else if (formSelectItemInProject.SelectedFile is VSBinary)
					{
						sourcePath = (formSelectItemInProject.SelectedFile as VSBinary).SourcePath;
					}
					if (string.IsNullOrEmpty(sourcePath) || !sourcePath.EndsWith(".exe") && !sourcePath.EndsWith(".dll") && !sourcePath.EndsWith(".vbs") && !sourcePath.EndsWith(".js") && !sourcePath.EndsWith(".bat"))
					{
						MessageBox.Show("Not a valid file type for a custom action.", Common.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					}
					else
					{
						selectedFile = formSelectItemInProject.SelectedFile;
						return selectedFile;
					}
				}
				return null;
			}
			finally
			{
				formSelectItemInProject.Dispose();
			}
			return selectedFile;
		}

		internal void SortCustomActions()
		{
			VSCustomActionBase item;
			VSCustomActionBase vSCustomActionBase;
			List<VSCustomActionBase> uninstall = this.Uninstall;
			List<VSCustomActionBase> install = this.Install;
			install.AddRange(this.Commit);
			install.AddRange(this.Rollback);
			if (uninstall.Count > 0)
			{
				VSCustomActionBase vSCustomActionBase1 = uninstall.Find((VSCustomActionBase e) => {
					if (e.WiXCustomSetProperty != null && e.WiXCustomSetProperty.After == "MsiUnpublishAssemblies")
					{
						return true;
					}
					if (e.WiXCustom == null)
					{
						return false;
					}
					return e.WiXCustom.After == "MsiUnpublishAssemblies";
				});
				if (vSCustomActionBase1 != null)
				{
					uninstall.Remove(vSCustomActionBase1);
					uninstall.Insert(0, vSCustomActionBase1);
					int num = 1;
					int num1 = 0;
					do
					{
					Label1:
						if (num >= uninstall.Count)
						{
							goto Label0;
						}
						item = uninstall[num];
						string empty = string.Empty;
						if (item.WiXCustomSetProperty != null)
						{
							empty = item.WiXCustomSetProperty.After;
						}
						else if (item.WiXCustom != null)
						{
							empty = item.WiXCustom.After;
						}
						if (!string.IsNullOrEmpty(empty))
						{
							VSCustomActionBase vSCustomActionBase2 = uninstall.Find((VSCustomActionBase e) => {
								if (e.WiXCustomSetProperty != null && e.WiXCustomSetProperty.Action == empty)
								{
									return true;
								}
								if (e.WiXCustom == null)
								{
									return false;
								}
								return e.WiXCustom.Action == empty;
							});
							if (vSCustomActionBase2 != null && vSCustomActionBase2 != item && uninstall.IndexOf(vSCustomActionBase2) + 1 != uninstall.IndexOf(item))
							{
								uninstall.Remove(item);
								if (uninstall.IndexOf(vSCustomActionBase2) + 1 <= uninstall.Count)
								{
									uninstall.Insert(uninstall.IndexOf(vSCustomActionBase2) + 1, item);
								}
								else
								{
									uninstall.Add(item);
								}
								num1++;
								continue;
							}
						}
						num++;
						num1 = 0;
						goto Label1;
					}
					while (num1 <= 1000);
					this._project.DoError(string.Concat("Error: Cannot find sequence for Custom Action with Id=", item.WiXCustom.Action));
				}
			}
		Label0:
			if (install.Count > 0)
			{
				VSCustomActionBase vSCustomActionBase3 = install.Find((VSCustomActionBase e) => {
					if (e.WiXCustomSetProperty != null && e.WiXCustomSetProperty.After == "StartServices")
					{
						return true;
					}
					if (e.WiXCustom == null)
					{
						return false;
					}
					return e.WiXCustom.After == "StartServices";
				});
				if (vSCustomActionBase3 != null)
				{
					install.Remove(vSCustomActionBase3);
					install.Insert(0, vSCustomActionBase3);
					int num2 = 1;
					int num3 = 0;
					do
					{
					Label3:
						if (num2 >= install.Count)
						{
							base.Clear();
							base.AddRange(install);
							base.AddRange(uninstall);
							return;
						}
						vSCustomActionBase = install[num2];
						string after = string.Empty;
						if (vSCustomActionBase.WiXCustomSetProperty != null)
						{
							after = vSCustomActionBase.WiXCustomSetProperty.After;
						}
						else if (vSCustomActionBase.WiXCustom != null)
						{
							after = vSCustomActionBase.WiXCustom.After;
						}
						if (!string.IsNullOrEmpty(after))
						{
							VSCustomActionBase vSCustomActionBase4 = install.Find((VSCustomActionBase e) => {
								if (e.WiXCustomSetProperty != null && e.WiXCustomSetProperty.Action == after)
								{
									return true;
								}
								if (e.WiXCustom == null)
								{
									return false;
								}
								return e.WiXCustom.Action == after;
							});
							if (vSCustomActionBase4 != null && vSCustomActionBase4 != vSCustomActionBase && install.IndexOf(vSCustomActionBase4) + 1 != install.IndexOf(vSCustomActionBase))
							{
								install.Remove(vSCustomActionBase);
								if (install.IndexOf(vSCustomActionBase4) + 1 <= install.Count)
								{
									install.Insert(install.IndexOf(vSCustomActionBase4) + 1, vSCustomActionBase);
								}
								else
								{
									install.Add(vSCustomActionBase);
								}
								num3++;
								continue;
							}
						}
						num2++;
						num3 = 0;
						goto Label3;
					}
					while (num3 <= 1000);
					this._project.DoError(string.Concat("Error: Cannot find sequence for Custom Action with Id=", vSCustomActionBase.WiXCustom.Action));
				}
			}
			base.Clear();
			base.AddRange(install);
			base.AddRange(uninstall);
		}
	}
}