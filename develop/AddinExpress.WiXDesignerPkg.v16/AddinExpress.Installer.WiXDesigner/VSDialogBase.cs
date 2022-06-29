using AddinExpress.Installer.WiXDesigner.WiXEngine.UserInterface;
using EnvDTE;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	[ToolboxItem(false)]
	internal class VSDialogBase : VSComponentBase
	{
		private WiXProjectParser _project;

		private AddinExpress.Installer.WiXDesigner.WiXDialog _wixElement;

		private WiXProperty _wixPrevArgsPropElement;

		private WiXProperty _wixNextArgsPropElement;

		private WiXControl _wixNextButton;

		private WiXControl _wixPrevButton;

		private WiXEntity _wixShowElement;

		private VSUserInterface _collection;

		private AddinExpress.Installer.WiXDesigner.DialogType dialogType = AddinExpress.Installer.WiXDesigner.DialogType.Custom;

		private AddinExpress.Installer.WiXDesigner.DialogScope dialogScope;

		private AddinExpress.Installer.WiXDesigner.DialogStage dialogStage;

		private string sequence = string.Empty;

		private string prevArgsPropId;

		private string nextArgsPropId;

		private int order = -1;

		internal readonly static ResourceManager Resources;

		internal readonly static ResourceManager ResourcesMultiLang;

		protected WiXControl _wixBannerBitmap;

		internal AddinExpress.Installer.WiXDesigner.DialogScope DialogScope
		{
			get
			{
				return this.dialogScope;
			}
		}

		internal AddinExpress.Installer.WiXDesigner.DialogStage DialogStage
		{
			get
			{
				return this.dialogStage;
			}
		}

		internal AddinExpress.Installer.WiXDesigner.DialogType DialogType
		{
			get
			{
				return this.dialogType;
			}
		}

		internal string DisplayName
		{
			get
			{
				return this.GetDisplayName();
			}
		}

		internal bool IsChained
		{
			get
			{
				if (string.IsNullOrEmpty(this.nextArgsPropId))
				{
					return false;
				}
				return !string.IsNullOrEmpty(this.prevArgsPropId);
			}
		}

		[Browsable(false)]
		[Description("Specifies the name used in the User Interface Editor to identify a selected dialog")]
		[DisplayName("(Name)")]
		[ReadOnly(false)]
		public override string Name
		{
			get
			{
				string dialogName = this.GetDialogName();
				if (string.IsNullOrEmpty(dialogName))
				{
					dialogName = this._wixElement.VSName;
					if (string.IsNullOrEmpty(dialogName))
					{
						dialogName = this._wixElement.GetAttributeValue("Id");
						if (string.IsNullOrEmpty(dialogName))
						{
							dialogName = "Unnamed Dialog";
						}
					}
				}
				return dialogName;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this._project.ProjectManager.AddWiXExtensionReference("VDWExtension", false);
					if (this._wixElement != null)
					{
						this._wixElement.VSName = value;
						this.DoPropertyChanged();
					}
				}
			}
		}

		internal string NextArgsPropId
		{
			get
			{
				return this.nextArgsPropId;
			}
			set
			{
				this.nextArgsPropId = value;
			}
		}

		internal int Order
		{
			get
			{
				return this.order;
			}
			set
			{
				this.order = value;
			}
		}

		internal string PrevArgsPropId
		{
			get
			{
				return this.prevArgsPropId;
			}
			set
			{
				this.prevArgsPropId = value;
			}
		}

		internal WiXProjectParser Project
		{
			get
			{
				return this._project;
			}
		}

		internal string Sequence
		{
			get
			{
				return this.sequence;
			}
			set
			{
				this.sequence = value;
			}
		}

		internal AddinExpress.Installer.WiXDesigner.WiXDialog WiXDialog
		{
			get
			{
				return this._wixElement;
			}
		}

		internal WiXProperty WiXNextArgsProperty
		{
			get
			{
				return this._wixNextArgsPropElement;
			}
		}

		internal WiXControl WiXNextButton
		{
			get
			{
				return this._wixNextButton;
			}
		}

		internal WiXProperty WiXPrevArgsProperty
		{
			get
			{
				return this._wixPrevArgsPropElement;
			}
		}

		internal WiXControl WiXPrevButton
		{
			get
			{
				return this._wixPrevButton;
			}
		}

		internal WiXEntity WiXShowElement
		{
			get
			{
				return this._wixShowElement;
			}
			set
			{
				this._wixShowElement = value;
			}
		}

		static VSDialogBase()
		{
			VSDialogBase.Resources = new ResourceManager("AddinExpress.Installer.WiXDesigner.WiXEngine.UserInterface.UIResources", typeof(UIResources).Assembly);
			VSDialogBase.ResourcesMultiLang = new ResourceManager("AddinExpress.Installer.WiXDesigner.WiXEngine.UserInterface.UIMultiLangResources", typeof(UIResources).Assembly);
		}

		protected VSDialogBase(WiXProjectParser project)
		{
			this._project = project;
			this.InitializeDialog();
		}

		protected VSDialogBase(WiXProjectParser project, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.WiXDialog wixElement, AddinExpress.Installer.WiXDesigner.DialogType dialogType, AddinExpress.Installer.WiXDesigner.DialogStage dialogStage, AddinExpress.Installer.WiXDesigner.DialogScope dialogScope)
		{
			this._project = project;
			this._wixElement = wixElement;
			this._collection = collection;
			this.dialogType = dialogType;
			this.dialogStage = dialogStage;
			this.dialogScope = dialogScope;
			this.InitializeDialog();
		}

		internal static VSDialogBase AddDialog(AddinExpress.Installer.WiXDesigner.DialogType dialogType, WiXEntity rootSetupEntity, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.DialogScope scope, AddinExpress.Installer.WiXDesigner.DialogStage stage, string ns, string language)
		{
			List<WiXEntity>.Enumerator enumerator;
			int item;
			int num;
			int item1;
			int num1;
			int item2;
			int num2;
			int item3;
			int num3;
			string attributeValue = null;
			VSDialogBase uI = null;
			VSDialogBase vSDialogBase = null;
			int num4 = 0;
			while (num4 < collection.Count)
			{
				if (collection[num4].DialogScope != scope)
				{
					num4++;
				}
				else
				{
					vSDialogBase = collection[num4];
					break;
				}
			}
			if (vSDialogBase != null)
			{
				while (num4 < collection.Count - 1)
				{
					VSDialogBase vSDialogBase1 = collection[num4 + 1];
					if (vSDialogBase1.DialogStage != stage || vSDialogBase1.DialogScope != scope)
					{
						if (vSDialogBase.DialogStage == stage)
						{
							break;
						}
						vSDialogBase = null;
						break;
					}
					else
					{
						int num5 = num4 + 1;
						num4 = num5;
						vSDialogBase = collection[num5];
					}
				}
			}
			if (vSDialogBase != null && vSDialogBase.dialogType == AddinExpress.Installer.WiXDesigner.DialogType.Finished)
			{
				vSDialogBase = null;
				if (num4 > 0 && collection[num4 - 1].DialogStage == AddinExpress.Installer.WiXDesigner.DialogStage.End && collection[num4 - 1].DialogScope == scope)
				{
					int num6 = num4 - 1;
					num4 = num6;
					vSDialogBase = collection[num6];
				}
			}
			switch (dialogType)
			{
				case AddinExpress.Installer.WiXDesigner.DialogType.ConfirmInstallation:
				{
					string str = "ConfirmDlg";
					string str1 = "ConfirmInstallForm";
					if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
					{
						str = string.Concat("Admin", str);
						str1 = string.Concat("Admin", str1);
					}
					Dictionary<string, string> strs = new Dictionary<string, string>()
					{
						{ "{BannerBitmapPlaceholder}", "Text=\"DefBannerBitmap\" " }
					};
					AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog = VSDialogBase.CreateWiXDialog(collection.Project, scope, str, str, str1, str1, ns, language, strs);
					if (wiXDialog == null)
					{
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					uI = VSDialogBase.AddDialogToUI(null, wiXDialog, str1, collection, scope, stage, vSDialogBase);
					VSDialogBase.RegisterUIReference(str, rootSetupEntity);
					if (uI != null)
					{
						if (vSDialogBase == null)
						{
							if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
							{
								collection.Add(uI);
							}
							else
							{
								collection.Insert(0, uI);
							}
						}
						else if (num4 != collection.Count - 1)
						{
							collection.Insert(num4 + 1, uI);
						}
						else
						{
							collection.Add(uI);
						}
					}
					return uI;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.RadioButtons2:
				case AddinExpress.Installer.WiXDesigner.DialogType.RadioButtons3:
				case AddinExpress.Installer.WiXDesigner.DialogType.RadioButtons4:
				{
					string str2 = "2";
					string str3 = "Custom2Buttons";
					string str4 = "Custom2ButtonDlg";
					if (dialogType == AddinExpress.Installer.WiXDesigner.DialogType.RadioButtons3)
					{
						str2 = "3";
						str3 = "Custom3Buttons";
						str4 = "Custom3ButtonDlg";
					}
					else if (dialogType == AddinExpress.Installer.WiXDesigner.DialogType.RadioButtons4)
					{
						str2 = "4";
						str3 = "Custom4Buttons";
						str4 = "Custom4ButtonDlg";
					}
					Dictionary<string, string> strs1 = new Dictionary<string, string>()
					{
						{ "{BUTTON_PROP_ID}", string.Concat("BUTTON", str2) },
						{ "{BUTTON_DEF_VALUE}", "1" },
						{ "{BUTTON1_VALUE}", "1" },
						{ "{BUTTON2_VALUE}", "2" },
						{ "{BUTTON3_VALUE}", "3" },
						{ "{BUTTON4_VALUE}", "4" },
						{ "{BannerBitmapPlaceholder}", "Text=\"DefBannerBitmap\" " }
					};
					AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog1 = VSDialogBase.CreateWiXDialog(collection.Project, scope, str4, str4, str3, str3, ns, language, strs1);
					if (wiXDialog1 == null)
					{
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					uI = VSDialogBase.AddDialogToUI(null, wiXDialog1, str3, collection, scope, stage, vSDialogBase);
					VSDialogBase.RegisterUIReference(str4, rootSetupEntity);
					if (uI != null)
					{
						if (vSDialogBase == null)
						{
							if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
							{
								collection.Add(uI);
							}
							else
							{
								collection.Insert(0, uI);
							}
						}
						else if (num4 != collection.Count - 1)
						{
							collection.Insert(num4 + 1, uI);
						}
						else
						{
							collection.Add(uI);
						}
					}
					return uI;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.CheckBoxesA:
				case AddinExpress.Installer.WiXDesigner.DialogType.CheckBoxesB:
				case AddinExpress.Installer.WiXDesigner.DialogType.CheckBoxesC:
				{
					string str5 = "A";
					string str6 = "CustomCheckA";
					string str7 = "CustomCheck1Dlg";
					List<int> freeCheckboxSequences = VSDialogBase.GetFreeCheckboxSequences(collection.Project);
					if (freeCheckboxSequences.Count < 4)
					{
						item = 696;
						num = 697;
						item1 = 698;
						num1 = 699;
					}
					else
					{
						item = freeCheckboxSequences[0];
						num = freeCheckboxSequences[1];
						item1 = freeCheckboxSequences[2];
						num1 = freeCheckboxSequences[3];
					}
					if (dialogType == AddinExpress.Installer.WiXDesigner.DialogType.CheckBoxesB)
					{
						str5 = "B";
						str6 = "CustomCheckB";
						str7 = "CustomCheck2Dlg";
					}
					else if (dialogType == AddinExpress.Installer.WiXDesigner.DialogType.CheckBoxesC)
					{
						str5 = "C";
						str6 = "CustomCheckC";
						str7 = "CustomCheck3Dlg";
					}
					Dictionary<string, string> strs2 = new Dictionary<string, string>()
					{
						{ string.Concat("{CHECKBOX", str5, "4_PROP}"), string.Concat("CHECKBOX", str5, "4") },
						{ string.Concat("{CHECKBOX", str5, "3_PROP}"), string.Concat("CHECKBOX", str5, "3") },
						{ string.Concat("{CHECKBOX", str5, "2_PROP}"), string.Concat("CHECKBOX", str5, "2") },
						{ string.Concat("{CHECKBOX", str5, "1_PROP}"), string.Concat("CHECKBOX", str5, "1") },
						{ string.Concat("{CHECKBOX", str5, "1_HIDDEN=\"\"}"), string.Empty },
						{ string.Concat("{CHECKBOX", str5, "2_HIDDEN=\"\"}"), string.Empty },
						{ string.Concat("{CHECKBOX", str5, "3_HIDDEN=\"\"}"), string.Empty },
						{ string.Concat("{CHECKBOX", str5, "4_HIDDEN=\"\"}"), string.Empty },
						{ "{BannerBitmapPlaceholder}", "Text=\"DefBannerBitmap\" " }
					};
					AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog2 = VSDialogBase.CreateWiXDialog(collection.Project, scope, str7, str7, str6, str6, ns, language, strs2);
					if (wiXDialog2 == null)
					{
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					WiXEntity wiXEntity = null;
					WiXEntity parent = wiXDialog2.Parent.Parent as WiXEntity;
					wiXEntity = (scope != AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall ? VSDialogBase.GetUISequence(AddinExpress.Installer.WiXDesigner.DialogScope.UserInstall, parent, null, collection.Project) : VSDialogBase.GetUISequence(AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall, parent, null, collection.Project));
					WiXEntity executeSequence = VSDialogBase.GetExecuteSequence(parent, null, collection.Project);
					VSDialogBase.CreateCustomEntity(string.Concat("CustomCheck", str5, "_SetProperty_CHECKBOX1"), wiXEntity, item.ToString());
					VSDialogBase.CreateCustomEntity(string.Concat("CustomCheck", str5, "_SetProperty_CHECKBOX2"), wiXEntity, num.ToString());
					VSDialogBase.CreateCustomEntity(string.Concat("CustomCheck", str5, "_SetProperty_CHECKBOX3"), wiXEntity, item1.ToString());
					VSDialogBase.CreateCustomEntity(string.Concat("CustomCheck", str5, "_SetProperty_CHECKBOX4"), wiXEntity, num1.ToString());
					VSDialogBase.CreateCustomEntity(string.Concat("CustomCheck", str5, "_SetProperty_CHECKBOX1"), executeSequence, item.ToString());
					VSDialogBase.CreateCustomEntity(string.Concat("CustomCheck", str5, "_SetProperty_CHECKBOX2"), executeSequence, num.ToString());
					VSDialogBase.CreateCustomEntity(string.Concat("CustomCheck", str5, "_SetProperty_CHECKBOX3"), executeSequence, item1.ToString());
					VSDialogBase.CreateCustomEntity(string.Concat("CustomCheck", str5, "_SetProperty_CHECKBOX4"), executeSequence, num1.ToString());
					VSDialogBase.CreateCheckboxCustomAction(string.Concat("CustomCheck", str5, "_SetProperty_CHECKBOX1"), (WiXEntity)wiXDialog2.Parent, string.Concat("CHECKBOX", str5, "1"), string.Empty, "firstSequence");
					VSDialogBase.CreateCheckboxCustomAction(string.Concat("CustomCheck", str5, "_SetProperty_CHECKBOX2"), (WiXEntity)wiXDialog2.Parent, string.Concat("CHECKBOX", str5, "2"), string.Empty, "firstSequence");
					VSDialogBase.CreateCheckboxCustomAction(string.Concat("CustomCheck", str5, "_SetProperty_CHECKBOX3"), (WiXEntity)wiXDialog2.Parent, string.Concat("CHECKBOX", str5, "3"), string.Empty, "firstSequence");
					VSDialogBase.CreateCheckboxCustomAction(string.Concat("CustomCheck", str5, "_SetProperty_CHECKBOX4"), (WiXEntity)wiXDialog2.Parent, string.Concat("CHECKBOX", str5, "4"), string.Empty, "firstSequence");
					uI = VSDialogBase.AddDialogToUI(null, wiXDialog2, str6, collection, scope, stage, vSDialogBase);
					VSDialogBase.RegisterUIReference(str7, rootSetupEntity);
					if (uI != null)
					{
						if (vSDialogBase == null)
						{
							if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
							{
								collection.Add(uI);
							}
							else
							{
								collection.Insert(0, uI);
							}
						}
						else if (num4 != collection.Count - 1)
						{
							collection.Insert(num4 + 1, uI);
						}
						else
						{
							collection.Add(uI);
						}
					}
					return uI;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.CustomerInformation:
				{
					string str8 = "CustomerInfoDlg";
					string str9 = "CustomerInfoForm";
					Dictionary<string, string> strs3 = new Dictionary<string, string>()
					{
						{ "{PIDTemplateValuePlaceholder}", "&lt;###-%%%%%%%&gt;" },
						{ "{CIF_SS_ValuePlaceholder}", "0" },
						{ "{CIF_SO_ValuePlaceholder}", "1" },
						{ "{CIF_SO_HidePlaceholder}", "Hidden=\"yes\" " },
						{ "{CIF_SS_HidePlaceholder}", "Hidden=\"yes\" " },
						{ "{BannerBitmapPlaceholder}", "Text=\"DefBannerBitmap\" " }
					};
					AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog3 = VSDialogBase.CreateWiXDialog(collection.Project, scope, str8, str8, str9, str9, ns, language, strs3);
					if (wiXDialog3 == null)
					{
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					uI = VSDialogBase.AddDialogToUI(null, wiXDialog3, str9, collection, scope, stage, vSDialogBase);
					VSDialogBase.RegisterUIReference(str8, rootSetupEntity);
					if (uI != null)
					{
						if (vSDialogBase == null)
						{
							if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
							{
								collection.Add(uI);
							}
							else
							{
								collection.Insert(0, uI);
							}
						}
						else if (num4 != collection.Count - 1)
						{
							collection.Insert(num4 + 1, uI);
						}
						else
						{
							collection.Add(uI);
						}
					}
					return uI;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.TextBoxesA:
				case AddinExpress.Installer.WiXDesigner.DialogType.TextBoxesB:
				case AddinExpress.Installer.WiXDesigner.DialogType.TextBoxesC:
				{
					string str10 = "A";
					string str11 = "CustomTextA";
					string str12 = "CustomText1Dlg";
					List<int> nums = VSDialogBase.GetFreeCheckboxSequences(collection.Project);
					if (nums.Count < 4)
					{
						item2 = 692;
						num2 = 693;
						item3 = 694;
						num3 = 695;
					}
					else
					{
						item2 = nums[0];
						num2 = nums[1];
						item3 = nums[2];
						num3 = nums[3];
					}
					if (dialogType == AddinExpress.Installer.WiXDesigner.DialogType.TextBoxesB)
					{
						str10 = "B";
						str11 = "CustomTextB";
						str12 = "CustomText2Dlg";
					}
					else if (dialogType == AddinExpress.Installer.WiXDesigner.DialogType.TextBoxesC)
					{
						str10 = "C";
						str11 = "CustomTextC";
						str12 = "CustomText3Dlg";
					}
					Dictionary<string, string> strs4 = new Dictionary<string, string>()
					{
						{ string.Concat("{EDIT", str10, "4_PROP}"), string.Concat("EDIT", str10, "4") },
						{ string.Concat("{EDIT", str10, "3_PROP}"), string.Concat("EDIT", str10, "3") },
						{ string.Concat("{EDIT", str10, "2_PROP}"), string.Concat("EDIT", str10, "2") },
						{ string.Concat("{EDIT", str10, "1_PROP}"), string.Concat("EDIT", str10, "1") },
						{ string.Concat("{EDIT", str10, "1_HIDDEN}"), string.Empty },
						{ string.Concat("{EDIT", str10, "2_HIDDEN}"), string.Empty },
						{ string.Concat("{EDIT", str10, "3_HIDDEN}"), string.Empty },
						{ string.Concat("{EDIT", str10, "4_HIDDEN}"), string.Empty },
						{ "{BannerBitmapPlaceholder}", "Text=\"DefBannerBitmap\" " }
					};
					AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog4 = VSDialogBase.CreateWiXDialog(collection.Project, scope, str12, str12, str11, str11, ns, language, strs4);
					if (wiXDialog4 == null)
					{
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					WiXEntity wiXEntity1 = null;
					WiXEntity parent1 = wiXDialog4.Parent.Parent as WiXEntity;
					wiXEntity1 = (scope != AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall ? VSDialogBase.GetUISequence(AddinExpress.Installer.WiXDesigner.DialogScope.UserInstall, parent1, null, collection.Project) : VSDialogBase.GetUISequence(AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall, parent1, null, collection.Project));
					WiXEntity executeSequence1 = VSDialogBase.GetExecuteSequence(parent1, null, collection.Project);
					VSDialogBase.CreateCustomEntity(string.Concat("CustomText", str10, "_SetProperty_EDIT1"), wiXEntity1, item2.ToString());
					VSDialogBase.CreateCustomEntity(string.Concat("CustomText", str10, "_SetProperty_EDIT2"), wiXEntity1, num2.ToString());
					VSDialogBase.CreateCustomEntity(string.Concat("CustomText", str10, "_SetProperty_EDIT3"), wiXEntity1, item3.ToString());
					VSDialogBase.CreateCustomEntity(string.Concat("CustomText", str10, "_SetProperty_EDIT4"), wiXEntity1, num3.ToString());
					VSDialogBase.CreateCustomEntity(string.Concat("CustomText", str10, "_SetProperty_EDIT1"), executeSequence1, item2.ToString());
					VSDialogBase.CreateCustomEntity(string.Concat("CustomText", str10, "_SetProperty_EDIT2"), executeSequence1, num2.ToString());
					VSDialogBase.CreateCustomEntity(string.Concat("CustomText", str10, "_SetProperty_EDIT3"), executeSequence1, item3.ToString());
					VSDialogBase.CreateCustomEntity(string.Concat("CustomText", str10, "_SetProperty_EDIT4"), executeSequence1, num3.ToString());
					VSDialogBase.CreateCheckboxCustomAction(string.Concat("CustomText", str10, "_SetProperty_EDIT1"), (WiXEntity)wiXDialog4.Parent, string.Concat("EDIT", str10, "1"), string.Empty, "firstSequence");
					VSDialogBase.CreateCheckboxCustomAction(string.Concat("CustomText", str10, "_SetProperty_EDIT2"), (WiXEntity)wiXDialog4.Parent, string.Concat("EDIT", str10, "2"), string.Empty, "firstSequence");
					VSDialogBase.CreateCheckboxCustomAction(string.Concat("CustomText", str10, "_SetProperty_EDIT3"), (WiXEntity)wiXDialog4.Parent, string.Concat("EDIT", str10, "3"), string.Empty, "firstSequence");
					VSDialogBase.CreateCheckboxCustomAction(string.Concat("CustomText", str10, "_SetProperty_EDIT4"), (WiXEntity)wiXDialog4.Parent, string.Concat("EDIT", str10, "4"), string.Empty, "firstSequence");
					uI = VSDialogBase.AddDialogToUI(null, wiXDialog4, str11, collection, scope, stage, vSDialogBase);
					VSDialogBase.RegisterUIReference(str12, rootSetupEntity);
					if (uI != null)
					{
						if (vSDialogBase == null)
						{
							if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
							{
								collection.Add(uI);
							}
							else
							{
								collection.Insert(0, uI);
							}
						}
						else if (num4 != collection.Count - 1)
						{
							collection.Insert(num4 + 1, uI);
						}
						else
						{
							collection.Add(uI);
						}
					}
					return uI;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.Finished:
				{
					Dictionary<string, string> strs5 = new Dictionary<string, string>()
					{
						{ "{BannerBitmapPlaceholder}", "Text=\"DefBannerBitmap\" " }
					};
					AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog5 = VSDialogBase.CreateWiXDialog(collection.Project, scope, "FinishedDlg", "AdminFinishedDlg", "FinishedForm", "AdminFinishedForm", ns, language, strs5);
					if (wiXDialog5 == null)
					{
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					WiXEntity parent2 = wiXDialog5.Parent.Parent as WiXEntity;
					bool flag = false;
					bool flag1 = false;
					foreach (WiXEntity wiXEntity2 in collection.Project.SupportedEntities.FindAll((WiXEntity e) => {
						if (!(e is WiXShow))
						{
							return false;
						}
						attributeValue = e.GetAttributeValue("Dialog");
						if (attributeValue == "FinishedForm")
						{
							return true;
						}
						return attributeValue == "AdminFinishedForm";
					}))
					{
						WiXEntity parent3 = wiXEntity2.Parent as WiXEntity;
						attributeValue = wiXEntity2.GetAttributeValue("Dialog");
						if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
						{
							if (attributeValue.StartsWith("Admin"))
							{
								collection.Project.SupportedEntities.Remove(wiXEntity2);
								wiXEntity2.Delete();
								if (parent3 == null)
								{
									continue;
								}
								parent3.SetDirty();
							}
							else if (!(wiXEntity2.Parent is WiXAdminUISequence))
							{
								flag = true;
							}
							else
							{
								collection.Project.SupportedEntities.Remove(wiXEntity2);
								wiXEntity2.Delete();
								if (parent3.HasChildEntities)
								{
									parent3.SetDirty();
								}
								else
								{
									collection.Project.SupportedEntities.Remove(parent3);
									parent3.Delete();
									parent2.SetDirty();
								}
							}
						}
						else if (!attributeValue.StartsWith("Admin"))
						{
							collection.Project.SupportedEntities.Remove(wiXEntity2);
							wiXEntity2.Delete();
							if (parent3 == null)
							{
								continue;
							}
							parent3.SetDirty();
						}
						else if (!(wiXEntity2.Parent is WiXInstallUISequence))
						{
							flag1 = true;
						}
						else
						{
							collection.Project.SupportedEntities.Remove(wiXEntity2);
							wiXEntity2.Delete();
							if (parent3.HasChildEntities)
							{
								parent3.SetDirty();
							}
							else
							{
								collection.Project.SupportedEntities.Remove(parent3);
								parent3.Delete();
								parent2.SetDirty();
							}
						}
					}
					if (scope != AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
					{
						VSDialogBase.RegisterUIReference("FinishedDlg", rootSetupEntity);
					}
					else
					{
						VSDialogBase.RegisterUIReference("AdminFinishedDlg", rootSetupEntity);
					}
					uI = VSDialogBase.CreateVSDialogFromWixDialog(wiXDialog5, scope, stage, collection);
					if (uI == null)
					{
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					WiXEntity uISequence = VSDialogBase.GetUISequence(scope, parent2, null, collection.Project);
					WiXShow wiXShow = VSDialogBase.CreateShowEntity(wiXDialog5.Id, uISequence, string.Empty, string.Empty, string.Empty, "success", string.Empty);
					uI.WiXShowElement = wiXShow;
					if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.UserInstall)
					{
						if (!flag1)
						{
							uISequence = VSDialogBase.GetUISequence(AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall, parent2, null, collection.Project);
							VSDialogBase.CreateShowEntity(wiXDialog5.Id, uISequence, string.Empty, string.Empty, string.Empty, "success", string.Empty);
						}
					}
					else if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall && !flag)
					{
						uISequence = VSDialogBase.GetUISequence(AddinExpress.Installer.WiXDesigner.DialogScope.UserInstall, parent2, null, collection.Project);
						VSDialogBase.CreateShowEntity(wiXDialog5.Id, uISequence, string.Empty, string.Empty, string.Empty, "success", string.Empty);
					}
					int num7 = 0;
					VSDialogBase.GetStandardActionStage("FinishedForm", string.Empty, true, ref num7);
					uI.Order = num7;
					if (uI != null)
					{
						if (vSDialogBase == null)
						{
							if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
							{
								collection.Add(uI);
							}
							else
							{
								collection.Insert(0, uI);
							}
						}
						else if (num4 != collection.Count - 1)
						{
							collection.Insert(num4 + 1, uI);
						}
						else
						{
							collection.Add(uI);
						}
					}
					return uI;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.InstallationFolder:
				{
					string str13 = "FolderDlg";
					string str14 = "FolderForm";
					if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
					{
						str13 = string.Concat("Admin", str13);
						str14 = string.Concat("Admin", str14);
					}
					Dictionary<string, string> strs6 = new Dictionary<string, string>()
					{
						{ "{FolderFormAllUsersVisiblePlaceholder}", "1" },
						{ "{BannerBitmapPlaceholder}", "Text=\"DefBannerBitmap\" " }
					};
					AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog6 = VSDialogBase.CreateWiXDialog(collection.Project, scope, str13, str13, str14, str14, ns, language, strs6);
					if (wiXDialog6 == null)
					{
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					uI = VSDialogBase.AddDialogToUI(null, wiXDialog6, str14, collection, scope, stage, vSDialogBase);
					VSDialogBase.RegisterUIReference(str13, rootSetupEntity);
					if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
					{
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					WiXEntity wiXEntity3 = collection.Project.SupportedEntities.Find((WiXEntity b) => {
						if (!(b is WiXCustomAction))
						{
							return false;
						}
						return b.GetAttributeValue("Id") == "VSDCA_AllUsers";
					});
					if (wiXEntity3 != null)
					{
						collection.Project.SupportedEntities.Remove(wiXEntity3);
						wiXEntity3.Delete();
					}
					List<WiXEntity> wiXEntities = collection.Project.SupportedEntities.FindAll((WiXEntity b) => {
						if (!(b is WiXCustom))
						{
							return false;
						}
						return b.GetAttributeValue("Action") == "VSDCA_AllUsers";
					});
					if (wiXEntities.Count <= 0)
					{
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					enumerator = wiXEntities.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							WiXEntity current = enumerator.Current;
							collection.Project.SupportedEntities.Remove(current);
							current.Delete();
						}
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
					break;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.LicenseAgreement:
				{
					string str15 = "LicenseDlg";
					string str16 = "EulaForm";
					if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
					{
						str15 = string.Concat("Admin", str15);
						str16 = string.Concat("Admin", str16);
					}
					Dictionary<string, string> strs7 = new Dictionary<string, string>()
					{
						{ "{SunkenControlPlaceholder}", "Sunken=\"yes\" " },
						{ "{SourceLicenseReadmeFilePlaceholder=\"\"}", string.Empty },
						{ "{BannerBitmapPlaceholder}", "Text=\"DefBannerBitmap\" " }
					};
					AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog7 = VSDialogBase.CreateWiXDialog(collection.Project, scope, str15, str15, str16, str16, ns, language, strs7);
					if (wiXDialog7 == null)
					{
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					uI = VSDialogBase.AddDialogToUI(null, wiXDialog7, str16, collection, scope, stage, vSDialogBase);
					VSDialogBase.RegisterUIReference(str15, rootSetupEntity);
					if (uI != null)
					{
						if (vSDialogBase == null)
						{
							if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
							{
								collection.Add(uI);
							}
							else
							{
								collection.Insert(0, uI);
							}
						}
						else if (num4 != collection.Count - 1)
						{
							collection.Insert(num4 + 1, uI);
						}
						else
						{
							collection.Add(uI);
						}
					}
					return uI;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.Progress:
				{
					string str17 = "ProgressDlg";
					string str18 = "ProgressForm";
					if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
					{
						str17 = string.Concat("Admin", str17);
						str18 = string.Concat("Admin", str18);
					}
					Dictionary<string, string> strs8 = new Dictionary<string, string>()
					{
						{ "{ShowProgress}", string.Empty },
						{ "{ShowProgress=\"\"}", string.Empty },
						{ "{BannerBitmapPlaceholder}", "Text=\"DefBannerBitmap\" " }
					};
					AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog8 = VSDialogBase.CreateWiXDialog(collection.Project, scope, str17, str17, str18, str18, ns, language, strs8);
					if (wiXDialog8 == null)
					{
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					uI = VSDialogBase.AddDialogToUI(null, wiXDialog8, str18, collection, scope, stage, vSDialogBase);
					VSDialogBase.RegisterUIReference(str17, rootSetupEntity);
					if (uI != null)
					{
						if (vSDialogBase == null)
						{
							if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
							{
								collection.Add(uI);
							}
							else
							{
								collection.Insert(0, uI);
							}
						}
						else if (num4 != collection.Count - 1)
						{
							collection.Insert(num4 + 1, uI);
						}
						else
						{
							collection.Add(uI);
						}
					}
					return uI;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.ReadMe:
				{
					string str19 = "ReadmeDlg";
					string str20 = "ReadmeForm";
					if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
					{
						str19 = string.Concat("Admin", str19);
						str20 = string.Concat("Admin", str20);
					}
					Dictionary<string, string> strs9 = new Dictionary<string, string>()
					{
						{ "{SunkenControlPlaceholder}", "Sunken=\"yes\" " },
						{ "{SourceLicenseReadmeFilePlaceholder=\"\"}", string.Empty },
						{ "{BannerBitmapPlaceholder}", "Text=\"DefBannerBitmap\" " }
					};
					AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog9 = VSDialogBase.CreateWiXDialog(collection.Project, scope, str19, str19, str20, str20, ns, language, strs9);
					if (wiXDialog9 == null)
					{
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					uI = VSDialogBase.AddDialogToUI(null, wiXDialog9, str20, collection, scope, stage, vSDialogBase);
					VSDialogBase.RegisterUIReference(str19, rootSetupEntity);
					if (uI != null)
					{
						if (vSDialogBase == null)
						{
							if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
							{
								collection.Add(uI);
							}
							else
							{
								collection.Insert(0, uI);
							}
						}
						else if (num4 != collection.Count - 1)
						{
							collection.Insert(num4 + 1, uI);
						}
						else
						{
							collection.Add(uI);
						}
					}
					return uI;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.RegisterUser:
				{
					string str21 = "RegisterDlg";
					string str22 = "RegisterUserExeForm";
					Dictionary<string, string> strs10 = new Dictionary<string, string>()
					{
						{ "{BinaryIDPlaceholder}", string.Empty },
						{ "{BannerBitmapPlaceholder}", "Text=\"DefBannerBitmap\" " }
					};
					AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog10 = VSDialogBase.CreateWiXDialog(collection.Project, scope, str21, str21, str22, str22, ns, language, strs10);
					if (wiXDialog10 == null)
					{
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					uI = VSDialogBase.AddDialogToUI(null, wiXDialog10, str22, collection, scope, stage, vSDialogBase);
					VSDialogBase.RegisterUIReference(str21, rootSetupEntity);
					if (uI != null)
					{
						if (vSDialogBase == null)
						{
							if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
							{
								collection.Add(uI);
							}
							else
							{
								collection.Insert(0, uI);
							}
						}
						else if (num4 != collection.Count - 1)
						{
							collection.Insert(num4 + 1, uI);
						}
						else
						{
							collection.Add(uI);
						}
					}
					return uI;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.Splash:
				{
					string str23 = "SplashDlg";
					string str24 = "SplashForm";
					if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
					{
						str23 = string.Concat("Admin", str23);
						str24 = string.Concat("Admin", str24);
					}
					Dictionary<string, string> strs11 = new Dictionary<string, string>()
					{
						{ "{SunkenControlPlaceholder}", "Sunken=\"yes\" " }
					};
					AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog11 = VSDialogBase.CreateWiXDialog(collection.Project, scope, str23, str23, str24, str24, ns, language, strs11);
					if (wiXDialog11 == null)
					{
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					uI = VSDialogBase.AddDialogToUI(null, wiXDialog11, str24, collection, scope, stage, vSDialogBase);
					VSDialogBase.RegisterUIReference(str23, rootSetupEntity);
					if (uI != null)
					{
						if (vSDialogBase == null)
						{
							if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
							{
								collection.Add(uI);
							}
							else
							{
								collection.Insert(0, uI);
							}
						}
						else if (num4 != collection.Count - 1)
						{
							collection.Insert(num4 + 1, uI);
						}
						else
						{
							collection.Add(uI);
						}
					}
					return uI;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.Welcome:
				{
					string str25 = "WelcomeDlg";
					string str26 = "WelcomeForm";
					if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
					{
						str25 = string.Concat("Admin", str25);
						str26 = string.Concat("Admin", str26);
					}
					Dictionary<string, string> strs12 = new Dictionary<string, string>()
					{
						{ "{BannerBitmapPlaceholder}", "Text=\"DefBannerBitmap\" " }
					};
					AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog12 = VSDialogBase.CreateWiXDialog(collection.Project, scope, str25, str25, str26, str26, ns, language, strs12);
					if (wiXDialog12 == null)
					{
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					uI = VSDialogBase.AddDialogToUI(null, wiXDialog12, str26, collection, scope, stage, vSDialogBase);
					VSDialogBase.RegisterUIReference(str25, rootSetupEntity);
					if (uI != null)
					{
						if (vSDialogBase == null)
						{
							if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
							{
								collection.Add(uI);
							}
							else
							{
								collection.Insert(0, uI);
							}
						}
						else if (num4 != collection.Count - 1)
						{
							collection.Insert(num4 + 1, uI);
						}
						else
						{
							collection.Add(uI);
						}
					}
					return uI;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.Empty:
				{
					if (uI != null)
					{
						if (vSDialogBase == null)
						{
							if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
							{
								collection.Add(uI);
							}
							else
							{
								collection.Insert(0, uI);
							}
						}
						else if (num4 != collection.Count - 1)
						{
							collection.Insert(num4 + 1, uI);
						}
						else
						{
							collection.Add(uI);
						}
					}
					return uI;
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.InstallationAddress:
				{
					string str27 = "WebFolderDlg";
					string str28 = "WebFolderForm";
					if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
					{
						str27 = string.Concat("Admin", str27);
						str28 = string.Concat("Admin", str28);
					}
					Dictionary<string, string> strs13 = new Dictionary<string, string>()
					{
						{ "{BannerBitmapPlaceholder}", "Text=\"DefBannerBitmap\" " }
					};
					AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog13 = VSDialogBase.CreateWiXDialog(collection.Project, scope, str27, str27, str28, str28, ns, language, strs13);
					if (wiXDialog13 == null)
					{
						if (uI != null)
						{
							if (vSDialogBase == null)
							{
								if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
								{
									collection.Add(uI);
								}
								else
								{
									collection.Insert(0, uI);
								}
							}
							else if (num4 != collection.Count - 1)
							{
								collection.Insert(num4 + 1, uI);
							}
							else
							{
								collection.Add(uI);
							}
						}
						return uI;
					}
					uI = VSDialogBase.AddDialogToUI(null, wiXDialog13, str28, collection, scope, stage, vSDialogBase);
					VSDialogBase.RegisterUIReference(str27, rootSetupEntity);
					if (uI != null)
					{
						if (vSDialogBase == null)
						{
							if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
							{
								collection.Add(uI);
							}
							else
							{
								collection.Insert(0, uI);
							}
						}
						else if (num4 != collection.Count - 1)
						{
							collection.Insert(num4 + 1, uI);
						}
						else
						{
							collection.Add(uI);
						}
					}
					return uI;
				}
				default:
				{
					if (uI != null)
					{
						if (vSDialogBase == null)
						{
							if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall || collection.Count == 0)
							{
								collection.Add(uI);
							}
							else
							{
								collection.Insert(0, uI);
							}
						}
						else if (num4 != collection.Count - 1)
						{
							collection.Insert(num4 + 1, uI);
						}
						else
						{
							collection.Add(uI);
						}
					}
					return uI;
				}
			}
		}

		internal static VSDialogBase AddDialogToUI(VSDialogBase vsDialog, AddinExpress.Installer.WiXDesigner.WiXDialog wixDialog, string dialogID, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.DialogScope scope, AddinExpress.Installer.WiXDesigner.DialogStage stage, VSDialogBase lastdialog)
		{
			WiXEntity wiXEntity = null;
			VSDialogBase vSDialogBase = vsDialog;
			WiXEntity parent = wixDialog.Parent.Parent as WiXEntity;
			foreach (WiXEntity wiXEntity1 in collection.Project.SupportedEntities.FindAll((WiXEntity e) => {
				if (!(e is WiXShow))
				{
					return false;
				}
				return e.GetAttributeValue("Dialog") == dialogID;
			}))
			{
				WiXEntity parent1 = wiXEntity1.Parent as WiXEntity;
				collection.Project.SupportedEntities.Remove(wiXEntity1);
				wiXEntity1.Delete();
				if (parent1 == null)
				{
					continue;
				}
				parent1.SetDirty();
			}
			if (vSDialogBase == null)
			{
				vSDialogBase = VSDialogBase.CreateVSDialogFromWixDialog(wixDialog, scope, stage, collection);
			}
			if (vSDialogBase != null)
			{
				int num = 0;
				VSDialogBase.GetStandardActionStage(dialogID, (stage == AddinExpress.Installer.WiXDesigner.DialogStage.End ? "end" : string.Empty), true, ref num);
				vSDialogBase.Order = num;
				if (lastdialog == null || lastdialog != null && !lastdialog.IsChained)
				{
					wiXEntity = (scope != AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall ? VSDialogBase.GetUISequence(AddinExpress.Installer.WiXDesigner.DialogScope.UserInstall, parent, null, collection.Project) : VSDialogBase.GetUISequence(AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall, parent, null, collection.Project));
					string empty = "Installed=\"\" AND NOT RESUME";
					if (vSDialogBase.DialogType == AddinExpress.Installer.WiXDesigner.DialogType.Progress)
					{
						empty = string.Empty;
					}
					if (lastdialog != null)
					{
						vSDialogBase.WiXShowElement = VSDialogBase.CreateShowEntity(vSDialogBase.WiXDialog.Id, wiXEntity, string.Empty, lastdialog.WiXDialog.Id, string.Empty, string.Empty, empty);
					}
					else
					{
						vSDialogBase.WiXShowElement = VSDialogBase.CreateShowEntity(vSDialogBase.WiXDialog.Id, wiXEntity, num.ToString(), string.Empty, string.Empty, string.Empty, empty);
					}
				}
				else
				{
					lastdialog.SetNextDialogId(vSDialogBase.WiXDialog.Id);
					vSDialogBase.SetPrevDialogId(lastdialog.WiXDialog.Id);
				}
			}
			return vSDialogBase;
		}

		internal static void AddFinishDialogs(WiXEntity targetUIDialogs, WiXEntity oppositeUIDialogs, WiXEntity rootEntity, WiXEntity beforeEntity, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.DialogScope scope, string language, bool multiLangSupport)
		{
			List<AddinExpress.Installer.WiXDesigner.WiXDialog> wiXDialogs;
			WiXEntity parent;
			string empty = string.Empty;
			string str = "FatalErrorForm";
			string str1 = "UserExitForm";
			string str2 = "ResumeForm";
			string str3 = "MaintenanceForm";
			string str4 = "FinishDialogs";
			if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
			{
				empty = "Admin";
				str = string.Concat(empty, "FatalErrorForm");
				str1 = string.Concat(empty, "UserExitForm");
				str2 = string.Concat(empty, "ResumeForm");
				str3 = string.Concat(empty, "MaintenanceForm");
				str4 = "AdminFinishDialogs";
			}
			WiXEntity wiXEntity = VSDialogBase.AddXmlUIBlock(VSDialogBase.GetXmlContent(str4, language, rootEntity.XmlNode.NamespaceURI, multiLangSupport).Replace("{BannerBitmapPlaceholder}", "Text=\"DefBannerBitmap\" "), rootEntity, null, null, collection.Project, out wiXDialogs);
			if (wiXEntity != null)
			{
				collection.Project.SupportedEntities.Add(wiXEntity);
				foreach (WiXEntity wiXEntity1 in collection.Project.SupportedEntities.FindAll((WiXEntity e) => {
					if (!(e is WiXShow))
					{
						return false;
					}
					string attributeValue = e.GetAttributeValue("Dialog");
					if (attributeValue == str || attributeValue == str1 || attributeValue == str2)
					{
						return true;
					}
					return attributeValue == str3;
				}))
				{
					parent = wiXEntity1.Parent as WiXEntity;
					collection.Project.SupportedEntities.Remove(wiXEntity1);
					wiXEntity1.Delete();
					if (parent == null)
					{
						continue;
					}
					parent.SetDirty();
				}
				WiXEntity uISequence = null;
				WiXEntity uISequence1 = null;
				if (scope != AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
				{
					uISequence = VSDialogBase.GetUISequence(AddinExpress.Installer.WiXDesigner.DialogScope.UserInstall, rootEntity, beforeEntity, collection.Project);
					uISequence1 = VSDialogBase.GetUISequence(AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall, rootEntity, beforeEntity, collection.Project);
				}
				else
				{
					uISequence = VSDialogBase.GetUISequence(AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall, rootEntity, beforeEntity, collection.Project);
					uISequence1 = VSDialogBase.GetUISequence(AddinExpress.Installer.WiXDesigner.DialogScope.UserInstall, rootEntity, beforeEntity, collection.Project);
				}
				foreach (WiXEntity wiXEntity2 in uISequence.ChildEntities.FindAll((WiXEntity e) => {
					if (!(e is WiXShow))
					{
						return false;
					}
					string attributeValue = e.GetAttributeValue("Dialog");
					if (attributeValue.Contains("FatalErrorForm") || attributeValue.Contains("UserExitForm") || attributeValue.Contains("ResumeForm"))
					{
						return true;
					}
					return attributeValue.Contains("MaintenanceForm");
				}))
				{
					parent = wiXEntity2.Parent as WiXEntity;
					collection.Project.SupportedEntities.Remove(wiXEntity2);
					wiXEntity2.Delete();
					if (parent == null)
					{
						continue;
					}
					parent.SetDirty();
				}
				if (oppositeUIDialogs == null)
				{
					foreach (WiXEntity wiXEntity3 in uISequence1.ChildEntities.FindAll((WiXEntity e) => {
						if (!(e is WiXShow))
						{
							return false;
						}
						string attributeValue = e.GetAttributeValue("Dialog");
						if (attributeValue.Contains("FatalErrorForm") || attributeValue.Contains("UserExitForm") || attributeValue.Contains("ResumeForm"))
						{
							return true;
						}
						return attributeValue.Contains("MaintenanceForm");
					}))
					{
						parent = wiXEntity3.Parent as WiXEntity;
						collection.Project.SupportedEntities.Remove(wiXEntity3);
						wiXEntity3.Delete();
						if (parent == null)
						{
							continue;
						}
						parent.SetDirty();
					}
				}
				foreach (AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog in wiXDialogs)
				{
					collection.Project.SupportedEntities.Add(wiXDialog);
					XmlElement xmlElement = null;
					if (wiXDialog.Id == str)
					{
						xmlElement = Common.CreateXmlElementWithAttributes(uISequence.XmlNode.OwnerDocument, "Show", uISequence.XmlNode.NamespaceURI, new string[] { "Dialog", "OnExit" }, new string[] { str, "error" }, "", false);
						xmlElement.AppendChild(uISequence.XmlNode.OwnerDocument.CreateCDataSection("NOT HideFatalErrorForm"));
						uISequence.XmlNode.AppendChild(xmlElement);
					}
					else if (wiXDialog.Id == str1)
					{
						xmlElement = Common.CreateXmlElementWithAttributes(uISequence.XmlNode.OwnerDocument, "Show", uISequence.XmlNode.NamespaceURI, new string[] { "Dialog", "OnExit" }, new string[] { str1, "cancel" }, "", false);
						uISequence.XmlNode.AppendChild(xmlElement);
					}
					else if (wiXDialog.Id == str2)
					{
						xmlElement = Common.CreateXmlElementWithAttributes(uISequence.XmlNode.OwnerDocument, "Show", uISequence.XmlNode.NamespaceURI, new string[] { "Dialog", "Sequence" }, new string[] { str2, "998" }, "", false);
						xmlElement.AppendChild(uISequence.XmlNode.OwnerDocument.CreateCDataSection("Installed=\"\" AND RESUME"));
						uISequence.XmlNode.AppendChild(xmlElement);
					}
					else if (wiXDialog.Id == str3)
					{
						xmlElement = Common.CreateXmlElementWithAttributes(uISequence.XmlNode.OwnerDocument, "Show", uISequence.XmlNode.NamespaceURI, new string[] { "Dialog", "Sequence" }, new string[] { str3, "999" }, "", false);
						xmlElement.AppendChild(uISequence.XmlNode.OwnerDocument.CreateCDataSection("Installed<>\"\""));
						uISequence.XmlNode.AppendChild(xmlElement);
					}
					if (xmlElement == null)
					{
						continue;
					}
					WiXShow wiXShow = new WiXShow(collection.Project, uISequence.Owner, uISequence, xmlElement);
					collection.Project.SupportedEntities.Add(wiXShow);
					if (oppositeUIDialogs != null)
					{
						continue;
					}
					xmlElement = (XmlElement)xmlElement.CloneNode(true);
					uISequence1.XmlNode.AppendChild(xmlElement);
					wiXShow = new WiXShow(collection.Project, uISequence1.Owner, uISequence1, xmlElement);
					collection.Project.SupportedEntities.Add(wiXShow);
				}
			}
		}

		internal static List<AddinExpress.Installer.WiXDesigner.WiXDialog> AddInnerXmlDialogs(WiXEntity parent, string xmlText, WiXProjectParser project)
		{
			List<AddinExpress.Installer.WiXDesigner.WiXDialog> wiXDialogs = new List<AddinExpress.Installer.WiXDesigner.WiXDialog>();
			if (parent != null)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(xmlText);
				if (xmlDocument.DocumentElement != null && xmlDocument.DocumentElement.FirstChild != null)
				{
					foreach (XmlNode childNode in xmlDocument.DocumentElement.FirstChild.ChildNodes)
					{
						XmlNode xmlNodes = parent.XmlNode.OwnerDocument.ImportNode(childNode, true);
						if (xmlNodes == null)
						{
							continue;
						}
						parent.XmlNode.AppendChild(xmlNodes);
						if (xmlNodes.Name != "Dialog")
						{
							continue;
						}
						AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog = new AddinExpress.Installer.WiXDesigner.WiXDialog(project, parent.Owner, parent, xmlNodes);
						if (parent.Owner is WiXProjectItem)
						{
							((WiXProjectItem)parent.Owner).Parse(wiXDialog);
						}
						wiXDialogs.Add(wiXDialog);
					}
				}
			}
			return wiXDialogs;
		}

		public static WiXEntity AddStandardUILocProjectItem(WiXProjectParser project, string ns, string itemName, CultureInfo language, bool languageSupported)
		{
			WiXProjectItem wiXProjectItem = null;
			if (language != null)
			{
				string str = Path.Combine(Path.GetDirectoryName(project.ProjectManager.VsProject.FullName), itemName);
				foreach (WiXProjectItem projectItem in project.ProjectItems)
				{
					if (string.IsNullOrEmpty(projectItem.SourceFile) || !projectItem.SourceFile.Equals(str, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					wiXProjectItem = projectItem;
					goto Label0;
				}
			Label0:
				if (wiXProjectItem == null)
				{
					wiXProjectItem = VSDialogBase.CreateNewStandardUILocFile(str, ns, project, itemName, language, languageSupported);
				}
			}
			return wiXProjectItem;
		}

		internal static WiXEntity AddXmlUIBlock(string xmlText, WiXEntity parent, WiXEntity beforeNode, WiXEntity afterNode, WiXProjectParser project)
		{
			List<AddinExpress.Installer.WiXDesigner.WiXDialog> wiXDialogs;
			return VSDialogBase.AddXmlUIBlock(xmlText, parent, beforeNode, afterNode, project, out wiXDialogs);
		}

		internal static WiXEntity AddXmlUIBlock(string xmlText, WiXEntity parent, WiXEntity beforeNode, WiXEntity afterNode, WiXProjectParser project, out List<AddinExpress.Installer.WiXDesigner.WiXDialog> innerDialogs)
		{
			innerDialogs = null;
			WiXEntity wiXUI = null;
			if (parent != null)
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(xmlText);
				if (xmlDocument.DocumentElement != null && xmlDocument.DocumentElement.FirstChild != null)
				{
					XmlNode xmlNodes = parent.XmlNode.OwnerDocument.ImportNode(xmlDocument.DocumentElement.FirstChild, true);
					if (xmlNodes != null)
					{
						if (beforeNode != null)
						{
							parent.XmlNode.InsertBefore(xmlNodes, beforeNode.XmlNode);
						}
						else if (afterNode == null)
						{
							parent.XmlNode.AppendChild(xmlNodes);
						}
						else
						{
							parent.XmlNode.InsertAfter(xmlNodes, afterNode.XmlNode);
						}
						if (!(parent is WiXProjectItem))
						{
							wiXUI = new WiXUI(project, parent.Owner, parent, xmlNodes);
							if (parent.Owner is WiXProjectItem)
							{
								((WiXProjectItem)parent.Owner).Parse(wiXUI);
							}
						}
						else
						{
							wiXUI = new WiXUI(project, parent, parent, xmlNodes);
							((WiXProjectItem)parent).Parse(wiXUI);
						}
						parent.SetDirty();
						foreach (WiXEntity childEntity in wiXUI.ChildEntities)
						{
							if (!(childEntity is AddinExpress.Installer.WiXDesigner.WiXDialog))
							{
								continue;
							}
							if (innerDialogs == null)
							{
								innerDialogs = new List<AddinExpress.Installer.WiXDesigner.WiXDialog>();
							}
							innerDialogs.Add((AddinExpress.Installer.WiXDesigner.WiXDialog)childEntity);
						}
					}
				}
			}
			return wiXUI;
		}

		private static WiXCustomAction CreateAllUsersCustomAction(WiXEntity rootEntity, WiXEntity beforeEntity)
		{
			WiXCustomAction wiXCustomAction = null;
			if (rootEntity.Project.SupportedEntities.Find((WiXEntity e) => {
				if (!(e is WiXCustomAction))
				{
					return false;
				}
				return e.GetAttributeValue("Id") == "VSDCA_FolderForm_AllUsers";
			}) == null)
			{
				XmlElement xmlElement = Common.CreateXmlElementWithAttributes(rootEntity.XmlNode.OwnerDocument, "CustomAction", rootEntity.XmlNode.NamespaceURI, new string[] { "Id", "Property", "Value" }, new string[] { "VSDCA_FolderForm_AllUsers", "FolderForm_AllUsers", "ALL" }, "", false);
				if (beforeEntity == null)
				{
					rootEntity.XmlNode.AppendChild(xmlElement);
				}
				else
				{
					rootEntity.XmlNode.InsertBefore(xmlElement, beforeEntity.XmlNode);
				}
				wiXCustomAction = new WiXCustomAction(rootEntity.Project, rootEntity.Owner, rootEntity, xmlElement);
				rootEntity.Project.SupportedEntities.Add(wiXCustomAction);
				rootEntity.SetDirty();
			}
			if (rootEntity.Project.SupportedEntities.Find((WiXEntity e) => {
				if (!(e is WiXCustom))
				{
					return false;
				}
				return e.GetAttributeValue("Action") == "VSDCA_FolderForm_AllUsers";
			}) == null)
			{
				WiXEntity wiXInstallUISequence = rootEntity.ChildEntities.Find((WiXEntity e) => e is WiXInstallUISequence);
				if (wiXInstallUISequence == null)
				{
					XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(rootEntity.XmlNode.OwnerDocument, "InstallUISequence", rootEntity.XmlNode.NamespaceURI, new string[0], new string[0], "", false);
					if (beforeEntity == null)
					{
						rootEntity.XmlNode.AppendChild(xmlElement1);
					}
					else
					{
						rootEntity.XmlNode.InsertBefore(xmlElement1, rootEntity.XmlNode);
					}
					wiXInstallUISequence = new WiXInstallUISequence(rootEntity.Project, rootEntity.Owner, rootEntity, xmlElement1);
					rootEntity.Project.SupportedEntities.Add(wiXInstallUISequence);
					rootEntity.SetDirty();
				}
				if (wiXInstallUISequence != null)
				{
					XmlElement xmlElement2 = Common.CreateXmlElementWithAttributes(wiXInstallUISequence.XmlNode.OwnerDocument, "Custom", wiXInstallUISequence.XmlNode.NamespaceURI, new string[] { "Action", "Sequence" }, new string[] { "VSDCA_FolderForm_AllUsers", "997" }, "", false);
					xmlElement2.AppendChild(wiXInstallUISequence.XmlNode.OwnerDocument.CreateCDataSection("Installed=\"\" AND NOT RESUME AND ALLUSERS=1"));
					wiXInstallUISequence.XmlNode.AppendChild(xmlElement2);
					WiXCustom wiXCustom = new WiXCustom(rootEntity.Project, rootEntity.Owner, wiXInstallUISequence, xmlElement2);
					rootEntity.Project.SupportedEntities.Add(wiXCustom);
					wiXInstallUISequence.SetDirty();
				}
			}
			return wiXCustomAction;
		}

		private static WiXCustomAction CreateCheckboxCustomAction(string actionID, WiXEntity beforeEntity, string property, string value, string execute)
		{
			WiXCustomAction wiXCustomAction = null;
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(beforeEntity.XmlNode.OwnerDocument, "CustomAction", beforeEntity.XmlNode.NamespaceURI, new string[] { "Id", "Property", "Value", "Execute" }, new string[] { actionID, property, value, execute }, "", true);
			if (xmlElement != null)
			{
				((WiXEntity)beforeEntity.Parent).XmlNode.InsertBefore(xmlElement, beforeEntity.XmlNode);
				wiXCustomAction = new WiXCustomAction(beforeEntity.Project, beforeEntity.Owner, beforeEntity.Parent, xmlElement);
				beforeEntity.Project.SupportedEntities.Add(wiXCustomAction);
				beforeEntity.Parent.SetDirty();
			}
			return wiXCustomAction;
		}

		private static WiXCustom CreateCustomEntity(string actionID, WiXEntity uiSequence, string sequence)
		{
			WiXCustom wiXCustom = null;
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(uiSequence.XmlNode.OwnerDocument, "Custom", uiSequence.XmlNode.NamespaceURI, new string[] { "Action", "Sequence" }, new string[] { actionID, sequence }, "", false);
			if (xmlElement != null)
			{
				uiSequence.XmlNode.AppendChild(xmlElement);
				wiXCustom = new WiXCustom(uiSequence.Project, uiSequence.Owner, uiSequence, xmlElement);
				uiSequence.Project.SupportedEntities.Add(wiXCustom);
				uiSequence.SetDirty();
			}
			return wiXCustom;
		}

		internal static bool CreateDialog(WiXProjectParser project, List<WiXEntity> sequenceList, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.WiXDialog wixElement, AddinExpress.Installer.WiXDesigner.DialogScope scope, AddinExpress.Installer.WiXDesigner.DialogStage stage, int order, List<AddinExpress.Installer.WiXDesigner.WiXDialog> dialogList, List<WiXProperty> propertyList)
		{
			string str;
			string str1;
			if (!string.IsNullOrEmpty(wixElement.GetAttributeValue("Id")))
			{
				VSDialogBase showElement = VSDialogBase.CreateVSDialogFromWixDialog(wixElement, scope, stage, collection);
				if (showElement != null)
				{
					collection.Add(showElement);
					showElement._wixShowElement = VSDialogBase.GetShowElement(wixElement, sequenceList);
					showElement.order = order;
					if (showElement.WiXNextArgsProperty != null)
					{
						string attributeValue = showElement.WiXNextArgsProperty.GetAttributeValue("Value");
						if (!string.IsNullOrEmpty(attributeValue))
						{
							AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog = dialogList.Find((AddinExpress.Installer.WiXDesigner.WiXDialog d) => d.GetAttributeValue("Id") == attributeValue);
							if (wiXDialog != null)
							{
								VSDialogBase.CreateDialog(project, sequenceList, collection, wiXDialog, scope, stage, order, dialogList, propertyList);
							}
						}
					}
					return true;
				}
				showElement = new VSCustomDialog(project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.Welcome, stage, scope)
				{
					dialogScope = scope,
					dialogType = AddinExpress.Installer.WiXDesigner.DialogType.Custom,
					dialogStage = stage,
					order = order,
					_wixShowElement = VSDialogBase.GetShowElement(wixElement, sequenceList)
				};
				WiXControl nextButton = VSDialogBase.GetNextButton(wixElement.ChildEntities);
				WiXControl prevButton = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
				if (nextButton != null && prevButton != null)
				{
					showElement._wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(nextButton, propertyList, out str);
					showElement.nextArgsPropId = str;
					showElement._wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(prevButton, propertyList, out str1);
					showElement.prevArgsPropId = str1;
				}
				if (showElement.IsChained || showElement.WiXShowElement != null)
				{
					collection.Add(showElement);
				}
			}
			return false;
		}

		internal static List<VSDialogBase> CreateDialog(List<AddinExpress.Installer.WiXDesigner.DialogType> dialogTypes, VSUserInterface collection, AddinExpress.Installer.WiXDesigner.DialogScope scope, AddinExpress.Installer.WiXDesigner.DialogStage stage)
		{
			string str = "product";
			List<VSDialogBase> vSDialogBases = new List<VSDialogBase>();
			bool isMultiLangSupported = collection.Project.ProjectManager.IsMultiLangSupported;
			if (collection.Project.ProjectType == WiXProjectType.Product)
			{
				WiXEntity wiXEntity = collection.Project.SupportedEntities.Find((WiXEntity e) => e.Name.Equals(str, StringComparison.OrdinalIgnoreCase));
				if (wiXEntity != null)
				{
					string currentLCID = collection.Project.CurrentLCID;
					string str1 = Path.Combine(Path.GetDirectoryName(collection.Project.ProjectManager.VsProject.FullName), "Resources");
					if (string.IsNullOrEmpty(currentLCID))
					{
						currentLCID = "0";
					}
					WiXEntity wiXEntity1 = collection.Project.SupportedEntities.Find((WiXEntity e) => {
						if (!(e is WiXUI))
						{
							return false;
						}
						return e.GetAttributeValue("Id") == "Base";
					});
					if (wiXEntity1 == null)
					{
						wiXEntity1 = VSDialogBase.AddXmlUIBlock(VSDialogBase.GetXmlContent("Base", currentLCID, wiXEntity.XmlNode.NamespaceURI, isMultiLangSupported), wiXEntity, null, null, collection.Project);
						if (wiXEntity1 != null)
						{
							collection.Project.SupportedEntities.Add(wiXEntity1);
						}
					}
					if (wiXEntity1 != null)
					{
						if (collection.Project.SupportedEntities.Find((WiXEntity e) => {
							if (!(e is WiXCustomAction))
							{
								return false;
							}
							return e.GetAttributeValue("Id") == "ERRCA_UIANDADVERTISED";
						}) == null)
						{
							XmlElement xmlElement = Common.CreateXmlElementWithAttributes(wiXEntity1.XmlNode.OwnerDocument, "CustomAction", wiXEntity1.XmlNode.NamespaceURI, new string[] { "Id", "Error" }, new string[] { "ERRCA_UIANDADVERTISED", "[VSDUIANDADVERTISED]" }, "", false);
							wiXEntity.XmlNode.InsertAfter(xmlElement, wiXEntity1.XmlNode);
							WiXCustomAction wiXCustomAction = new WiXCustomAction(collection.Project, wiXEntity.Owner, wiXEntity, xmlElement);
							collection.Project.SupportedEntities.Add(wiXCustomAction);
							wiXEntity.SetDirty();
						}
						if (collection.Project.SupportedEntities.Find((WiXEntity e) => {
							if (!(e is WiXCustom))
							{
								return false;
							}
							return e.GetAttributeValue("Action") == "ERRCA_UIANDADVERTISED";
						}) == null)
						{
							WiXEntity wiXInstallUISequence = wiXEntity.ChildEntities.Find((WiXEntity e) => e is WiXInstallUISequence);
							if (wiXInstallUISequence == null)
							{
								XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(wiXEntity1.XmlNode.OwnerDocument, "InstallUISequence", wiXEntity1.XmlNode.NamespaceURI, new string[0], new string[0], "", false);
								wiXEntity.XmlNode.InsertBefore(xmlElement1, wiXEntity1.XmlNode);
								wiXInstallUISequence = new WiXInstallUISequence(collection.Project, wiXEntity.Owner, wiXEntity, xmlElement1);
								collection.Project.SupportedEntities.Add(wiXInstallUISequence);
								wiXEntity.SetDirty();
							}
							if (wiXInstallUISequence != null)
							{
								XmlElement xmlElement2 = Common.CreateXmlElementWithAttributes(wiXInstallUISequence.XmlNode.OwnerDocument, "Custom", wiXInstallUISequence.XmlNode.NamespaceURI, new string[] { "Action", "Sequence" }, new string[] { "ERRCA_UIANDADVERTISED", "5" }, "", false);
								xmlElement2.AppendChild(wiXInstallUISequence.XmlNode.OwnerDocument.CreateCDataSection("ProductState=1"));
								wiXInstallUISequence.XmlNode.AppendChild(xmlElement2);
								WiXCustom wiXCustom = new WiXCustom(collection.Project, wiXEntity.Owner, wiXInstallUISequence, xmlElement2);
								collection.Project.SupportedEntities.Add(wiXCustom);
								wiXInstallUISequence.SetDirty();
							}
						}
					}
					WiXEntity wiXEntity2 = collection.Project.SupportedEntities.Find((WiXEntity e) => {
						if (!(e is WiXUI))
						{
							return false;
						}
						return e.GetAttributeValue("Id") == "BasicDialogs";
					});
					if (wiXEntity2 == null)
					{
						string str2 = VSDialogBase.GetXmlContent("BasicDialogs", currentLCID, wiXEntity.XmlNode.NamespaceURI, isMultiLangSupported).Replace("$NewFldrBtn$", "Resources\\WiXNewFolderBtn.ico").Replace("$UpFldrBtn$", "Resources\\WiXUpFolderBtn.ico");
						if (!Directory.Exists(str1))
						{
							Directory.CreateDirectory(str1);
						}
						VSDialogBase.SaveBinaryFile(Path.Combine(str1, "WiXUpFolderBtn.ico"), UIResources.WiXUpFolderBtn);
						VSDialogBase.SaveBinaryFile(Path.Combine(str1, "WiXNewFolderBtn.ico"), UIResources.WiXNewFolderBtn);
						wiXEntity2 = VSDialogBase.AddXmlUIBlock(str2, wiXEntity, null, null, collection.Project);
						if (wiXEntity2 != null)
						{
							collection.Project.SupportedEntities.Add(wiXEntity2);
						}
					}
					WiXEntity wiXEntity3 = collection.Project.SupportedEntities.Find((WiXEntity e) => {
						if (!(e is WiXUI))
						{
							return false;
						}
						return e.GetAttributeValue("Id") == "UserInterface";
					});
					if (wiXEntity3 == null)
					{
						string str3 = VSDialogBase.GetXmlContent("UserInterface", currentLCID, wiXEntity.XmlNode.NamespaceURI, isMultiLangSupported).Replace("$DefBannerBitmap$", "Resources\\WiXDefBannerBitmap.bmp");
						if (!Directory.Exists(str1))
						{
							Directory.CreateDirectory(str1);
						}
						VSDialogBase.SaveBinaryFile(Path.Combine(str1, "WiXDefBannerBitmap.bmp"), UIResources.WiXDefBannerBitmap);
						wiXEntity3 = VSDialogBase.AddXmlUIBlock(str3, wiXEntity, null, null, collection.Project);
						if (wiXEntity3 != null)
						{
							collection.Project.SupportedEntities.Add(wiXEntity3);
						}
					}
					VSDialogBase.CreateAllUsersCustomAction(wiXEntity, wiXEntity1);
					VSDialogBase.CreateDIRCACustomAction(wiXEntity, wiXEntity1);
					WiXEntity wiXEntity4 = collection.Project.SupportedEntities.Find((WiXEntity e) => {
						if (!(e is WiXUI))
						{
							return false;
						}
						string attributeValue = e.GetAttributeValue("Id");
						if (attributeValue == "FinishDialogs")
						{
							return true;
						}
						return attributeValue == "FinishedDlg";
					});
					WiXEntity wiXEntity5 = collection.Project.SupportedEntities.Find((WiXEntity e) => {
						if (!(e is WiXUI))
						{
							return false;
						}
						string attributeValue = e.GetAttributeValue("Id");
						if (attributeValue == "AdminFinishDialogs")
						{
							return true;
						}
						return attributeValue == "AdminFinishedDlg";
					});
					if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.UserInstall)
					{
						if (wiXEntity4 == null)
						{
							VSDialogBase.AddFinishDialogs(wiXEntity4, wiXEntity5, wiXEntity, wiXEntity1, collection, scope, currentLCID, isMultiLangSupported);
						}
					}
					else if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall && wiXEntity5 == null)
					{
						VSDialogBase.AddFinishDialogs(wiXEntity5, wiXEntity4, wiXEntity, wiXEntity1, collection, scope, currentLCID, isMultiLangSupported);
					}
					bool flag = true;
					int num = dialogTypes.IndexOf(AddinExpress.Installer.WiXDesigner.DialogType.Finished);
					if (num >= 0 && num < dialogTypes.Count - 1)
					{
						dialogTypes.RemoveAt(num);
						dialogTypes.Add(AddinExpress.Installer.WiXDesigner.DialogType.Finished);
					}
					foreach (AddinExpress.Installer.WiXDesigner.DialogType dialogType in dialogTypes)
					{
						VSDialogBase vSDialogBase = VSDialogBase.AddDialog(dialogType, wiXEntity, collection, scope, stage, wiXEntity.XmlNode.NamespaceURI, currentLCID);
						if (vSDialogBase != null)
						{
							vSDialogBases.Add(vSDialogBase);
						}
						if (dialogType != AddinExpress.Installer.WiXDesigner.DialogType.Finished)
						{
							continue;
						}
						flag = false;
					}
					if (flag)
					{
						VSDialogBase vSDialogBase1 = VSDialogBase.FindDialog(AddinExpress.Installer.WiXDesigner.DialogType.Finished, scope, AddinExpress.Installer.WiXDesigner.DialogStage.End, collection);
						if (vSDialogBase1 == null)
						{
							vSDialogBase1 = VSDialogBase.AddDialog(AddinExpress.Installer.WiXDesigner.DialogType.Finished, wiXEntity, collection, scope, AddinExpress.Installer.WiXDesigner.DialogStage.End, wiXEntity.XmlNode.NamespaceURI, currentLCID);
							if (vSDialogBase1 != null)
							{
								vSDialogBases.Add(vSDialogBase1);
							}
						}
					}
				}
			}
			return vSDialogBases;
		}

		private static void CreateDIRCACustomAction(WiXEntity rootEntity, WiXEntity beforeEntity)
		{
			WiXCustomAction wiXCustomAction = null;
			if (rootEntity.Project.SupportedEntities.Find((WiXEntity e) => {
				if (!(e is WiXCustomAction))
				{
					return false;
				}
				return e.GetAttributeValue("Id") == "DIRCA_TARGETDIR";
			}) == null)
			{
				XmlElement xmlElement = Common.CreateXmlElementWithAttributes(rootEntity.XmlNode.OwnerDocument, "CustomAction", rootEntity.XmlNode.NamespaceURI, new string[] { "Id", "Property", "Value", "Execute" }, new string[] { "DIRCA_TARGETDIR", "TARGETDIR", "[ProgramFilesFolder][Manufacturer]\\[ProductName]", "firstSequence" }, "", false);
				if (beforeEntity == null)
				{
					rootEntity.XmlNode.AppendChild(xmlElement);
				}
				else
				{
					rootEntity.XmlNode.InsertBefore(xmlElement, beforeEntity.XmlNode);
				}
				wiXCustomAction = new WiXCustomAction(rootEntity.Project, rootEntity.Owner, rootEntity, xmlElement);
				rootEntity.Project.SupportedEntities.Add(wiXCustomAction);
				rootEntity.SetDirty();
			}
			bool flag = false;
			bool flag1 = false;
			bool flag2 = false;
			bool flag3 = false;
			foreach (WiXEntity wiXEntity in rootEntity.Project.SupportedEntities.FindAll((WiXEntity e) => {
				if (e is WiXInstallUISequence || e is WiXAdminUISequence)
				{
					return true;
				}
				return e is WiXInstallExecuteSequence;
			}))
			{
				if (wiXEntity.ChildEntities.Find((WiXEntity e) => {
					if (!(e is WiXCustom))
					{
						return false;
					}
					return e.GetAttributeValue("Action") == "DIRCA_TARGETDIR";
				}) != null)
				{
					if (wiXEntity is WiXInstallUISequence)
					{
						flag1 = true;
					}
					else if (wiXEntity is WiXAdminUISequence)
					{
						flag2 = true;
					}
					else if (wiXEntity is WiXInstallExecuteSequence)
					{
						flag3 = true;
					}
				}
				flag = flag1 & flag2 & flag3;
				if (!flag)
				{
					continue;
				}
				return;
			}
			if (!flag)
			{
				if (!flag1)
				{
					WiXEntity uISequence = VSDialogBase.GetUISequence(AddinExpress.Installer.WiXDesigner.DialogScope.UserInstall, rootEntity, beforeEntity, rootEntity.Project);
					if (uISequence != null)
					{
						XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(uISequence.XmlNode.OwnerDocument, "Custom", uISequence.XmlNode.NamespaceURI, new string[] { "Action", "Before" }, new string[] { "DIRCA_TARGETDIR", "CostInitialize" }, "", false);
						xmlElement1.AppendChild(uISequence.XmlNode.OwnerDocument.CreateCDataSection("TARGETDIR = \"\""));
						uISequence.XmlNode.AppendChild(xmlElement1);
						WiXCustom wiXCustom = new WiXCustom(rootEntity.Project, rootEntity.Owner, uISequence, xmlElement1);
						rootEntity.Project.SupportedEntities.Add(wiXCustom);
						uISequence.SetDirty();
					}
				}
				if (!flag2)
				{
					WiXEntity uISequence1 = VSDialogBase.GetUISequence(AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall, rootEntity, beforeEntity, rootEntity.Project);
					if (uISequence1 != null)
					{
						XmlElement xmlElement2 = Common.CreateXmlElementWithAttributes(uISequence1.XmlNode.OwnerDocument, "Custom", uISequence1.XmlNode.NamespaceURI, new string[] { "Action", "Before" }, new string[] { "DIRCA_TARGETDIR", "CostInitialize" }, "", false);
						xmlElement2.AppendChild(uISequence1.XmlNode.OwnerDocument.CreateCDataSection("TARGETDIR = \"\""));
						uISequence1.XmlNode.AppendChild(xmlElement2);
						WiXCustom wiXCustom1 = new WiXCustom(rootEntity.Project, rootEntity.Owner, uISequence1, xmlElement2);
						rootEntity.Project.SupportedEntities.Add(wiXCustom1);
						uISequence1.SetDirty();
					}
				}
				if (!flag3)
				{
					WiXEntity executeSequence = VSDialogBase.GetExecuteSequence(rootEntity, beforeEntity, rootEntity.Project);
					if (executeSequence != null)
					{
						XmlElement xmlElement3 = Common.CreateXmlElementWithAttributes(executeSequence.XmlNode.OwnerDocument, "Custom", executeSequence.XmlNode.NamespaceURI, new string[] { "Action", "Before" }, new string[] { "DIRCA_TARGETDIR", "CostInitialize" }, "", false);
						xmlElement3.AppendChild(executeSequence.XmlNode.OwnerDocument.CreateCDataSection("TARGETDIR = \"\""));
						executeSequence.XmlNode.AppendChild(xmlElement3);
						WiXCustom wiXCustom2 = new WiXCustom(rootEntity.Project, rootEntity.Owner, executeSequence, xmlElement3);
						rootEntity.Project.SupportedEntities.Add(wiXCustom2);
						executeSequence.SetDirty();
					}
				}
			}
		}

		internal static WiXProjectItem CreateNewStandardUIFile(string filePath, string ns, WiXProjectParser project)
		{
			XmlDocument xmlDocument;
			WiXProjectItem wiXProjectItem = null;
			project.ProjectManager.OpenWiXFile(filePath, -1, out xmlDocument);
			if (xmlDocument != null)
			{
				wiXProjectItem = project.AddWiXSourceFile(filePath, xmlDocument);
			}
			if (wiXProjectItem == null)
			{
				string str = UIResources.fragment.Replace("$namespace$", ns);
				str = str.Replace("$fragmentid$", Path.GetFileNameWithoutExtension(filePath));
				VSDialogBase.SaveTextFile(filePath, str, true);
				if (project.ProjectManager.VsProject.ProjectItems.AddFromFile(filePath) != null)
				{
					project.ProjectManager.OpenWiXFile(filePath, -1, out xmlDocument);
					if (xmlDocument != null)
					{
						wiXProjectItem = project.AddWiXSourceFile(filePath, xmlDocument);
					}
				}
			}
			return wiXProjectItem;
		}

		private static WiXProjectItem CreateNewStandardUILocFile(string filePath, string ns, WiXProjectParser project, string itemName, CultureInfo culture, bool languageSupported)
		{
			XmlDocument xmlDocument;
			string xmlContent;
			WiXProjectItem wiXProjectItem = null;
			project.ProjectManager.OpenWiXFile(filePath, -1, out xmlDocument);
			if (xmlDocument != null)
			{
				wiXProjectItem = project.AddWiXSourceFile(filePath, xmlDocument);
			}
			if (wiXProjectItem == null)
			{
				string str = (culture.Equals(CultureInfo.InvariantCulture) ? "0" : culture.LCID.ToString());
				string str1 = (culture.Equals(CultureInfo.InvariantCulture) ? "neutral" : culture.Name);
				if (languageSupported || ProjectUtilities.IsNeutralLCID(str))
				{
					xmlContent = (!ProjectUtilities.IsNeutralLCID(str) ? VSDialogBase.GetXmlContent(string.Concat("Strings_", str), str, ns, true) : VSDialogBase.GetXmlContent("Strings_0", str, ns, true));
				}
				else
				{
					xmlContent = VSDialogBase.GetXmlContent("Strings_neutral", str, ns, true);
					string str2 = (culture.Equals(CultureInfo.InvariantCulture) ? "StdUI_Neutral" : string.Concat("StdUI_", culture.ThreeLetterWindowsLanguageName));
					xmlContent = xmlContent.Replace("$Culture$", str1);
					xmlContent = xmlContent.Replace("$Language$", str);
					xmlContent = xmlContent.Replace("$ID$", str);
					xmlContent = xmlContent.Replace("$LangID$", str2);
					int aNSICodePage = culture.TextInfo.ANSICodePage;
					xmlContent = xmlContent.Replace("$Codepage$", aNSICodePage.ToString());
					xmlContent = xmlContent.Replace("$xmlns$", project.ProjectManager.WixLocUINamespaceUri);
				}
				VSDialogBase.SaveTextFile(filePath, xmlContent, true);
				if (project.ProjectManager.VsProject.ProjectItems.AddFromFile(filePath) != null)
				{
					project.ProjectManager.OpenWiXFile(filePath, -1, out xmlDocument);
					if (xmlDocument != null)
					{
						wiXProjectItem = project.AddWiXSourceFile(filePath, xmlDocument);
					}
				}
			}
			return wiXProjectItem;
		}

		private static WiXProperty CreatePropertyEntity(string propertyID, string value, WiXEntity parent, WiXEntity before)
		{
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(parent.XmlNode.OwnerDocument, "Property", parent.XmlNode.NamespaceURI, new string[] { "Id", "Value" }, new string[] { propertyID, value }, "", false);
			if (before == null)
			{
				parent.XmlNode.AppendChild(xmlElement);
			}
			else
			{
				parent.XmlNode.InsertBefore(xmlElement, before.XmlNode);
			}
			WiXProperty wiXProperty = new WiXProperty(parent.Project, parent.Owner, parent, xmlElement);
			parent.Project.SupportedEntities.Add(wiXProperty);
			parent.SetDirty();
			return wiXProperty;
		}

		private static WiXCustomAction CreateRegisterUserCustomAction(WiXEntity beforeEntity)
		{
			WiXCustomAction wiXCustomAction = null;
			XmlElement xmlElement = Common.CreateXmlElementWithAttributes(beforeEntity.XmlNode.OwnerDocument, "CustomAction", beforeEntity.XmlNode.NamespaceURI, new string[] { "Id", "BinaryKey", "ExeCommand", "Return" }, new string[] { "RegisterUserExeForm_Action", string.Empty, string.Empty, "ignore" }, "", true);
			if (xmlElement != null)
			{
				((WiXEntity)beforeEntity.Parent).XmlNode.InsertBefore(xmlElement, beforeEntity.XmlNode);
				wiXCustomAction = new WiXCustomAction(beforeEntity.Project, beforeEntity.Owner, beforeEntity.Parent, xmlElement);
				beforeEntity.Project.SupportedEntities.Add(wiXCustomAction);
				beforeEntity.Parent.SetDirty();
			}
			return wiXCustomAction;
		}

		private static WiXShow CreateShowEntity(string dialogID, WiXEntity uiSequence, string sequence, string after, string before, string onExit, string innerText)
		{
			WiXShow wiXShow = null;
			XmlElement xmlElement = null;
			if (!string.IsNullOrEmpty(sequence))
			{
				xmlElement = Common.CreateXmlElementWithAttributes(uiSequence.XmlNode.OwnerDocument, "Show", uiSequence.XmlNode.NamespaceURI, new string[] { "Dialog", "Sequence" }, new string[] { dialogID, sequence }, "", false);
			}
			else if (!string.IsNullOrEmpty(after))
			{
				xmlElement = Common.CreateXmlElementWithAttributes(uiSequence.XmlNode.OwnerDocument, "Show", uiSequence.XmlNode.NamespaceURI, new string[] { "Dialog", "After" }, new string[] { dialogID, after }, "", false);
			}
			else if (!string.IsNullOrEmpty(before))
			{
				xmlElement = Common.CreateXmlElementWithAttributes(uiSequence.XmlNode.OwnerDocument, "Show", uiSequence.XmlNode.NamespaceURI, new string[] { "Dialog", "Before" }, new string[] { dialogID, before }, "", false);
			}
			else if (!string.IsNullOrEmpty(onExit))
			{
				xmlElement = Common.CreateXmlElementWithAttributes(uiSequence.XmlNode.OwnerDocument, "Show", uiSequence.XmlNode.NamespaceURI, new string[] { "Dialog", "OnExit" }, new string[] { dialogID, onExit }, "", false);
			}
			if (xmlElement != null)
			{
				XmlCDataSection xmlCDataSection = null;
				if (!string.IsNullOrEmpty(innerText))
				{
					xmlCDataSection = uiSequence.XmlNode.OwnerDocument.CreateCDataSection(innerText);
					xmlElement.AppendChild(xmlCDataSection);
				}
				uiSequence.XmlNode.AppendChild(xmlElement);
				wiXShow = new WiXShow(uiSequence.Project, uiSequence.Owner, uiSequence, xmlElement);
				if (xmlCDataSection != null)
				{
					WiXCondition wiXCondition = new WiXCondition(uiSequence.Project, uiSequence.Owner, wiXShow, xmlCDataSection);
				}
				uiSequence.Project.SupportedEntities.Add(wiXShow);
				uiSequence.SetDirty();
			}
			return wiXShow;
		}

		internal static VSDialogBase CreateVSDialogFromWixDialog(AddinExpress.Installer.WiXDesigner.WiXDialog wixElement, AddinExpress.Installer.WiXDesigner.DialogScope scope, AddinExpress.Installer.WiXDesigner.DialogStage stage, VSUserInterface collection)
		{
			string str1;
			string str2;
			string str3;
			string str4;
			string str5;
			string str6;
			string str7;
			string str8;
			string str9;
			string str10;
			string str11;
			string str12;
			string str13;
			string str14;
			string str15;
			string str16;
			string str17;
			string str18;
			string str19;
			string str20;
			string str21;
			string str22;
			string str23;
			string str24;
			string str25;
			string str26;
			string str27;
			string str28;
			string str29;
			string str30;
			string str31;
			string str32;
			string str33;
			string str34;
			string str35;
			string str36;
			string str37;
			string str38;
			string str39;
			string str40;
			VSDialogBase vSCheckboxesDialog = null;
			string str41 = wixElement.GetAttributeValue("Id");
			if (str41 != null)
			{
				switch (str41)
				{
					case "CustomCheckA":
					{
						List<WiXEntity> wiXEntities = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BodyText" || attributeValue == "BannerBmp")
							{
								return true;
							}
							return attributeValue == "BannerText";
						});
						if (wiXEntities != null && wiXEntities.Count == 3)
						{
							WiXControl nextButton = VSDialogBase.GetNextButton(wixElement.ChildEntities);
							WiXControl prevButton = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
							if (nextButton != null && prevButton != null)
							{
								vSCheckboxesDialog = new VSCheckboxesDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.CheckBoxesA, stage, scope)
								{
									_wixNextButton = nextButton,
									_wixPrevButton = prevButton,
									_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(nextButton, collection.Project, out str1),
									nextArgsPropId = str1,
									_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(prevButton, collection.Project, out str2),
									prevArgsPropId = str2
								};
							}
						}
						break;
					}
					case "CustomCheckB":
					{
						List<WiXEntity> wiXEntities1 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BodyText" || attributeValue == "BannerBmp")
							{
								return true;
							}
							return attributeValue == "BannerText";
						});
						if (wiXEntities1 != null && wiXEntities1.Count == 3)
						{
							WiXControl wiXControl = VSDialogBase.GetNextButton(wixElement.ChildEntities);
							WiXControl prevButton1 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
							if (wiXControl != null && prevButton1 != null)
							{
								vSCheckboxesDialog = new VSCheckboxesDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.CheckBoxesB, stage, scope)
								{
									_wixNextButton = wiXControl,
									_wixPrevButton = prevButton1,
									_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(wiXControl, collection.Project, out str3),
									nextArgsPropId = str3,
									_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(prevButton1, collection.Project, out str4),
									prevArgsPropId = str4
								};
							}
						}
						break;
					}
					case "CustomCheckC":
					{
						List<WiXEntity> wiXEntities2 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BodyText" || attributeValue == "BannerBmp")
							{
								return true;
							}
							return attributeValue == "BannerText";
						});
						if (wiXEntities2 != null && wiXEntities2.Count == 3)
						{
							WiXControl nextButton1 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
							WiXControl wiXControl1 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
							if (nextButton1 != null && wiXControl1 != null)
							{
								vSCheckboxesDialog = new VSCheckboxesDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.CheckBoxesC, stage, scope)
								{
									_wixNextButton = nextButton1,
									_wixPrevButton = wiXControl1,
									_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(nextButton1, collection.Project, out str5),
									nextArgsPropId = str5,
									_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(wiXControl1, collection.Project, out str6),
									prevArgsPropId = str6
								};
							}
						}
						break;
					}
					case "AdminConfirmInstallForm":
					case "ConfirmInstallForm":
					{
						List<WiXEntity> wiXEntities3 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							return attributeValue == "BannerBmp";
						});
						if (wiXEntities3 != null && wiXEntities3.Count == 1)
						{
							WiXControl nextButton2 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
							WiXControl prevButton2 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
							if (nextButton2 != null && prevButton2 != null)
							{
								vSCheckboxesDialog = new VSConfirmDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.ConfirmInstallation, stage, scope)
								{
									_wixNextButton = nextButton2,
									_wixPrevButton = prevButton2,
									_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(nextButton2, collection.Project, out str7),
									nextArgsPropId = str7,
									_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(prevButton2, collection.Project, out str8),
									prevArgsPropId = str8
								};
							}
						}
						break;
					}
					case "CustomerInfoForm":
					{
						List<WiXEntity> wiXEntities4 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BannerBmp" || attributeValue == "SerialEdit" || attributeValue == "OrganizationEdit")
							{
								return true;
							}
							return attributeValue == "NameEdit";
						});
						if (wiXEntities4 != null && wiXEntities4.Count == 4)
						{
							wiXEntities4 = wixElement.Parent.ChildEntities.FindAll((WiXEntity e) => {
								string attributeValue = e.GetAttributeValue("Id");
								if (e.Name != "Property")
								{
									return false;
								}
								if (attributeValue == "PIDTemplate" || attributeValue == "CustomerInfoForm_ShowSerial")
								{
									return true;
								}
								return attributeValue == "CustomerInfoForm_ShowOrg";
							});
							if (wiXEntities4 != null && wiXEntities4.Count == 3)
							{
								WiXControl wiXControl2 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
								WiXControl prevButton3 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
								if (wiXControl2 != null && prevButton3 != null)
								{
									vSCheckboxesDialog = new VSCustomerInfoDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.CustomerInformation, stage, scope)
									{
										_wixNextButton = wiXControl2,
										_wixPrevButton = prevButton3,
										_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(wiXControl2, collection.Project, out str9),
										nextArgsPropId = str9,
										_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(prevButton3, collection.Project, out str10),
										prevArgsPropId = str10
									};
								}
							}
						}
						break;
					}
					case "AdminFinishedForm":
					{
						List<WiXEntity> wiXEntities5 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BannerBmp")
							{
								return true;
							}
							return attributeValue == "BodyTextInstall";
						});
						if (wiXEntities5 != null && wiXEntities5.Count == 2)
						{
							vSCheckboxesDialog = new VSFinishDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.Finished, stage, scope);
						}
						break;
					}
					case "FinishedForm":
					{
						List<WiXEntity> wiXEntities6 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BannerBmp")
							{
								return true;
							}
							return attributeValue == "UpdateText";
						});
						if (wiXEntities6 != null && wiXEntities6.Count == 2)
						{
							vSCheckboxesDialog = new VSFinishDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.Finished, stage, scope);
						}
						break;
					}
					case "AdminFolderForm":
					{
						List<WiXEntity> wiXEntities7 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BannerBmp")
							{
								return true;
							}
							return attributeValue == "FolderEdit";
						});
						if (wiXEntities7 != null && wiXEntities7.Count == 2)
						{
							WiXControl nextButton3 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
							WiXControl wiXControl3 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
							if (nextButton3 != null && wiXControl3 != null)
							{
								vSCheckboxesDialog = new VSInstallFolderDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.InstallationFolder, stage, scope)
								{
									_wixNextButton = nextButton3,
									_wixPrevButton = wiXControl3,
									_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(nextButton3, collection.Project, out str11),
									nextArgsPropId = str11,
									_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(wiXControl3, collection.Project, out str12),
									prevArgsPropId = str12
								};
							}
						}
						break;
					}
					case "FolderForm":
					{
						List<WiXEntity> wiXEntities8 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BannerBmp")
							{
								return true;
							}
							return attributeValue == "AllUsersText";
						});
						if (wiXEntities8 != null && wiXEntities8.Count == 2)
						{
							wiXEntities8 = wixElement.Parent.ChildEntities.FindAll((WiXEntity e) => {
								string attributeValue = e.GetAttributeValue("Id");
								string str = e.GetAttributeValue("Property");
								if (!(e.Name == "Property") && !(e.Name == "RadioButtonGroup"))
								{
									return false;
								}
								if (attributeValue == "FolderForm_AllUsers" || attributeValue == "FolderForm_AllUsersVisible")
								{
									return true;
								}
								return str == "FolderForm_AllUsers";
							});
							if (wiXEntities8 != null && wiXEntities8.Count == 3)
							{
								WiXControl nextButton4 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
								WiXControl prevButton4 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
								if (nextButton4 != null && prevButton4 != null)
								{
									vSCheckboxesDialog = new VSInstallFolderDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.InstallationFolder, stage, scope)
									{
										_wixNextButton = nextButton4,
										_wixPrevButton = prevButton4,
										_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(nextButton4, collection.Project, out str13),
										nextArgsPropId = str13,
										_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(prevButton4, collection.Project, out str14),
										prevArgsPropId = str14
									};
								}
							}
						}
						break;
					}
					case "AdminEulaForm":
					{
						List<WiXEntity> wiXEntities9 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BannerBmp" || attributeValue == "LicenseText")
							{
								return true;
							}
							return attributeValue == "AgreeRadioGroup";
						});
						if (wiXEntities9 != null && wiXEntities9.Count == 3)
						{
							wiXEntities9 = wixElement.Parent.ChildEntities.FindAll((WiXEntity e) => {
								string attributeValue = e.GetAttributeValue("Id");
								string str = e.GetAttributeValue("Property");
								if (!(e.Name == "Property") && !(e.Name == "RadioButtonGroup"))
								{
									return false;
								}
								if (attributeValue == "AdminEulaForm_Property")
								{
									return true;
								}
								return str == "AdminEulaForm_Property";
							});
							if (wiXEntities9 != null && wiXEntities9.Count == 2)
							{
								WiXControl wiXControl4 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
								WiXControl prevButton5 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
								if (wiXControl4 != null && prevButton5 != null)
								{
									vSCheckboxesDialog = new VSEulaDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.LicenseAgreement, stage, scope)
									{
										_wixNextButton = wiXControl4,
										_wixPrevButton = prevButton5,
										_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(wiXControl4, collection.Project, out str15),
										nextArgsPropId = str15,
										_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(prevButton5, collection.Project, out str16),
										prevArgsPropId = str16
									};
								}
							}
						}
						break;
					}
					case "EulaForm":
					{
						List<WiXEntity> wiXEntities10 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BannerBmp" || attributeValue == "LicenseText")
							{
								return true;
							}
							return attributeValue == "AgreeRadioGroup";
						});
						if (wiXEntities10 != null && wiXEntities10.Count == 3)
						{
							wiXEntities10 = wixElement.Parent.ChildEntities.FindAll((WiXEntity e) => {
								string attributeValue = e.GetAttributeValue("Id");
								string str = e.GetAttributeValue("Property");
								if (!(e.Name == "Property") && !(e.Name == "RadioButtonGroup"))
								{
									return false;
								}
								if (attributeValue == "EulaForm_Property")
								{
									return true;
								}
								return str == "EulaForm_Property";
							});
							if (wiXEntities10 != null && wiXEntities10.Count == 2)
							{
								WiXControl nextButton5 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
								WiXControl wiXControl5 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
								if (nextButton5 != null && wiXControl5 != null)
								{
									vSCheckboxesDialog = new VSEulaDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.LicenseAgreement, stage, scope)
									{
										_wixNextButton = nextButton5,
										_wixPrevButton = wiXControl5,
										_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(nextButton5, collection.Project, out str17),
										nextArgsPropId = str17,
										_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(wiXControl5, collection.Project, out str18),
										prevArgsPropId = str18
									};
								}
							}
						}
						break;
					}
					case "AdminProgressForm":
					case "ProgressForm":
					{
						List<WiXEntity> wiXEntities11 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BannerBmp")
							{
								return true;
							}
							return attributeValue == "ProgressBar";
						});
						if (wiXEntities11 != null && wiXEntities11.Count == 2)
						{
							vSCheckboxesDialog = new VSProgressDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.Progress, stage, scope);
						}
						break;
					}
					case "Custom2Buttons":
					{
						List<WiXEntity> wiXEntities12 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BannerBmp" || attributeValue == "BodyText")
							{
								return true;
							}
							return attributeValue == "BannerText";
						});
						if (wiXEntities12 != null && wiXEntities12.Count == 3)
						{
							wiXEntities12 = wixElement.Parent.ChildEntities.FindAll((WiXEntity e) => {
								e.GetAttributeValue("Property");
								if (e.Name != "RadioButtonGroup")
								{
									return false;
								}
								return e.ChildEntities.Count == 2;
							});
							if (wiXEntities12 != null && wiXEntities12.Count == 1)
							{
								WiXControl nextButton6 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
								WiXControl prevButton6 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
								if (nextButton6 != null && prevButton6 != null)
								{
									vSCheckboxesDialog = new VSRadioButtonsDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.RadioButtons2, stage, scope)
									{
										_wixNextButton = nextButton6,
										_wixPrevButton = prevButton6,
										_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(nextButton6, collection.Project, out str19),
										nextArgsPropId = str19,
										_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(prevButton6, collection.Project, out str20),
										prevArgsPropId = str20
									};
								}
							}
						}
						break;
					}
					case "Custom3Buttons":
					{
						List<WiXEntity> wiXEntities13 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BannerBmp" || attributeValue == "BodyText")
							{
								return true;
							}
							return attributeValue == "BannerText";
						});
						if (wiXEntities13 != null && wiXEntities13.Count == 3)
						{
							wiXEntities13 = wixElement.Parent.ChildEntities.FindAll((WiXEntity e) => {
								e.GetAttributeValue("Property");
								if (e.Name != "RadioButtonGroup")
								{
									return false;
								}
								return e.ChildEntities.Count == 3;
							});
							if (wiXEntities13 != null && wiXEntities13.Count == 1)
							{
								WiXControl wiXControl6 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
								WiXControl prevButton7 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
								if (wiXControl6 != null && prevButton7 != null)
								{
									vSCheckboxesDialog = new VSRadioButtonsDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.RadioButtons3, stage, scope)
									{
										_wixNextButton = wiXControl6,
										_wixPrevButton = prevButton7,
										_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(wiXControl6, collection.Project, out str21),
										nextArgsPropId = str21,
										_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(prevButton7, collection.Project, out str22),
										prevArgsPropId = str22
									};
								}
							}
						}
						break;
					}
					case "Custom4Buttons":
					{
						List<WiXEntity> wiXEntities14 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BannerBmp" || attributeValue == "BodyText")
							{
								return true;
							}
							return attributeValue == "BannerText";
						});
						if (wiXEntities14 != null && wiXEntities14.Count == 3)
						{
							wiXEntities14 = wixElement.Parent.ChildEntities.FindAll((WiXEntity e) => {
								e.GetAttributeValue("Property");
								if (e.Name != "RadioButtonGroup")
								{
									return false;
								}
								return e.ChildEntities.Count == 4;
							});
							if (wiXEntities14 != null && wiXEntities14.Count == 1)
							{
								WiXControl nextButton7 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
								WiXControl wiXControl7 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
								if (nextButton7 != null && wiXControl7 != null)
								{
									vSCheckboxesDialog = new VSRadioButtonsDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.RadioButtons4, stage, scope)
									{
										_wixNextButton = nextButton7,
										_wixPrevButton = wiXControl7,
										_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(nextButton7, collection.Project, out str23),
										nextArgsPropId = str23,
										_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(wiXControl7, collection.Project, out str24),
										prevArgsPropId = str24
									};
								}
							}
						}
						break;
					}
					case "AdminReadmeForm":
					case "ReadmeForm":
					{
						List<WiXEntity> wiXEntities15 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BannerBmp" || attributeValue == "BannerText")
							{
								return true;
							}
							return attributeValue == "ReadmeText";
						});
						if (wiXEntities15 != null && wiXEntities15.Count == 3)
						{
							WiXControl nextButton8 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
							WiXControl prevButton8 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
							if (nextButton8 != null && prevButton8 != null)
							{
								vSCheckboxesDialog = new VSReadmeDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.ReadMe, stage, scope)
								{
									_wixNextButton = nextButton8,
									_wixPrevButton = prevButton8,
									_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(nextButton8, collection.Project, out str25),
									nextArgsPropId = str25,
									_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(prevButton8, collection.Project, out str26),
									prevArgsPropId = str26
								};
							}
						}
						break;
					}
					case "RegisterUserExeForm":
					{
						List<WiXEntity> wiXEntities16 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BannerBmp" || attributeValue == "BannerText")
							{
								return true;
							}
							return attributeValue == "RegisterButton";
						});
						if (wiXEntities16 != null && wiXEntities16.Count == 3)
						{
							WiXControl wiXControl8 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
							WiXControl prevButton9 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
							if (wiXControl8 != null && prevButton9 != null)
							{
								vSCheckboxesDialog = new VSRegisterUserDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.RegisterUser, stage, scope)
								{
									_wixNextButton = wiXControl8,
									_wixPrevButton = prevButton9,
									_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(wiXControl8, collection.Project, out str27),
									nextArgsPropId = str27,
									_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(prevButton9, collection.Project, out str28),
									prevArgsPropId = str28
								};
							}
						}
						break;
					}
					case "AdminSplashForm":
					case "SplashForm":
					{
						List<WiXEntity> wiXEntities17 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							return attributeValue == "SplashBmp";
						});
						if (wiXEntities17 != null && wiXEntities17.Count == 1)
						{
							WiXControl nextButton9 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
							WiXControl wiXControl9 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
							if (nextButton9 != null && wiXControl9 != null)
							{
								vSCheckboxesDialog = new VSSplashDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.Splash, stage, scope)
								{
									_wixNextButton = nextButton9,
									_wixPrevButton = wiXControl9,
									_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(nextButton9, collection.Project, out str29),
									nextArgsPropId = str29,
									_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(wiXControl9, collection.Project, out str30),
									prevArgsPropId = str30
								};
							}
						}
						break;
					}
					case "CustomTextA":
					{
						List<WiXEntity> wiXEntities18 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BodyText" || attributeValue == "BannerBmp")
							{
								return true;
							}
							return attributeValue == "BannerText";
						});
						if (wiXEntities18 != null && wiXEntities18.Count == 3)
						{
							WiXControl nextButton10 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
							WiXControl prevButton10 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
							if (nextButton10 != null && prevButton10 != null)
							{
								vSCheckboxesDialog = new VSTextboxesDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.TextBoxesA, stage, scope)
								{
									_wixNextButton = nextButton10,
									_wixPrevButton = prevButton10,
									_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(nextButton10, collection.Project, out str31),
									nextArgsPropId = str31,
									_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(prevButton10, collection.Project, out str32),
									prevArgsPropId = str32
								};
							}
						}
						break;
					}
					case "CustomTextB":
					{
						List<WiXEntity> wiXEntities19 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BodyText" || attributeValue == "BannerBmp")
							{
								return true;
							}
							return attributeValue == "BannerText";
						});
						if (wiXEntities19 != null && wiXEntities19.Count == 3)
						{
							WiXControl wiXControl10 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
							WiXControl prevButton11 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
							if (wiXControl10 != null && prevButton11 != null)
							{
								vSCheckboxesDialog = new VSTextboxesDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.TextBoxesB, stage, scope)
								{
									_wixNextButton = wiXControl10,
									_wixPrevButton = prevButton11,
									_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(wiXControl10, collection.Project, out str33),
									nextArgsPropId = str33,
									_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(prevButton11, collection.Project, out str34),
									prevArgsPropId = str34
								};
							}
						}
						break;
					}
					case "CustomTextC":
					{
						List<WiXEntity> wiXEntities20 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BodyText" || attributeValue == "BannerBmp")
							{
								return true;
							}
							return attributeValue == "BannerText";
						});
						if (wiXEntities20 != null && wiXEntities20.Count == 3)
						{
							WiXControl nextButton11 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
							WiXControl wiXControl11 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
							if (nextButton11 != null && wiXControl11 != null)
							{
								vSCheckboxesDialog = new VSTextboxesDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.TextBoxesC, stage, scope)
								{
									_wixNextButton = nextButton11,
									_wixPrevButton = wiXControl11,
									_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(nextButton11, collection.Project, out str35),
									nextArgsPropId = str35,
									_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(wiXControl11, collection.Project, out str36),
									prevArgsPropId = str36
								};
							}
						}
						break;
					}
					case "AdminWelcomeForm":
					case "WelcomeForm":
					{
						List<WiXEntity> wiXEntities21 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "WelcomeText" || attributeValue == "BannerBmp")
							{
								return true;
							}
							return attributeValue == "CopyrightWarningText";
						});
						if (wiXEntities21 != null && wiXEntities21.Count == 3)
						{
							WiXControl nextButton12 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
							WiXControl prevButton12 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
							if (nextButton12 != null && prevButton12 != null)
							{
								vSCheckboxesDialog = new VSWelcomeDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.Welcome, stage, scope)
								{
									dialogScope = scope,
									dialogType = AddinExpress.Installer.WiXDesigner.DialogType.Welcome,
									dialogStage = stage,
									_wixNextButton = nextButton12,
									_wixPrevButton = prevButton12,
									_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(nextButton12, collection.Project, out str37),
									nextArgsPropId = str37,
									_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(prevButton12, collection.Project, out str38),
									prevArgsPropId = str38
								};
							}
						}
						break;
					}
					case "AdminWebFolderForm":
					case "WebFolderForm":
					{
						List<WiXEntity> wiXEntities22 = wixElement.ChildEntities.FindAll((WiXEntity e) => {
							string attributeValue = e.GetAttributeValue("Id");
							if (e.Name != "Control")
							{
								return false;
							}
							if (attributeValue == "BannerBmp" || attributeValue == "SiteCombo" || attributeValue == "AppPoolsCombo")
							{
								return true;
							}
							return attributeValue == "VDirEdit";
						});
						if (wiXEntities22 != null && wiXEntities22.Count == 4)
						{
							WiXControl wiXControl12 = VSDialogBase.GetNextButton(wixElement.ChildEntities);
							WiXControl prevButton13 = VSDialogBase.GetPrevButton(wixElement.ChildEntities);
							if (wiXControl12 != null && prevButton13 != null)
							{
								vSCheckboxesDialog = new VSWebFolderDialog(collection.Project, collection, wixElement, AddinExpress.Installer.WiXDesigner.DialogType.InstallationAddress, stage, scope)
								{
									_wixNextButton = wiXControl12,
									_wixPrevButton = prevButton13,
									_wixNextArgsPropElement = VSDialogBase.GetPropertyByButton(wiXControl12, collection.Project, out str39),
									nextArgsPropId = str39,
									_wixPrevArgsPropElement = VSDialogBase.GetPropertyByButton(prevButton13, collection.Project, out str40),
									prevArgsPropId = str40
								};
							}
						}
						break;
					}
				}
			}
			return vSCheckboxesDialog;
		}

		internal static AddinExpress.Installer.WiXDesigner.WiXDialog CreateWiXDialog(WiXProjectParser project, AddinExpress.Installer.WiXDesigner.DialogScope scope, string userUIDlgID, string adminUIDlgID, string userDlgID, string adminDlgID, string ns, string language, Dictionary<string, string> replacements)
		{
			List<AddinExpress.Installer.WiXDesigner.WiXDialog> wiXDialogs;
			IWiXEntity owner;
			string str;
			WiXEntity wiXEntity = null;
			AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog = null;
			bool isMultiLangSupported = project.ProjectManager.IsMultiLangSupported;
			if (scope != AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
			{
				str = userUIDlgID;
				wiXEntity = project.SupportedEntities.Find((WiXEntity e) => {
					if (!(e is WiXUI))
					{
						return false;
					}
					return e.GetAttributeValue("Id") == userUIDlgID;
				});
			}
			else
			{
				str = adminUIDlgID;
				wiXEntity = project.SupportedEntities.Find((WiXEntity e) => {
					if (!(e is WiXUI))
					{
						return false;
					}
					return e.GetAttributeValue("Id") == adminUIDlgID;
				});
			}
			if (wiXEntity == null)
			{
				WiXEntity standardUIProjectItem = VSDialogBase.GetStandardUIProjectItem(project, ns);
				if (standardUIProjectItem != null)
				{
					string xmlContent = VSDialogBase.GetXmlContent(str, language, ns, isMultiLangSupported);
					if (!string.IsNullOrEmpty(xmlContent))
					{
						foreach (KeyValuePair<string, string> replacement in replacements)
						{
							xmlContent = xmlContent.Replace(replacement.Key, replacement.Value);
						}
						WiXEntity wiXFragment = null;
						if (standardUIProjectItem.HasChildEntities)
						{
							wiXFragment = project.SupportedEntities.Find((WiXEntity e) => {
								if (!(e is WiXFragment))
								{
									return false;
								}
								return e.GetAttributeValue("Id") == str;
							});
						}
						if (wiXFragment == null)
						{
							XmlElement xmlElement = Common.CreateXmlElementWithAttributes(standardUIProjectItem.XmlNode.OwnerDocument, "Fragment", standardUIProjectItem.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { str }, "", false);
							standardUIProjectItem.XmlNode.AppendChild(xmlElement);
							WiXProjectParser wiXProjectParser = project;
							if (standardUIProjectItem.Owner == null)
							{
								owner = standardUIProjectItem;
							}
							else
							{
								owner = standardUIProjectItem.Owner;
							}
							wiXFragment = new WiXFragment(wiXProjectParser, owner, standardUIProjectItem, xmlElement);
							project.SupportedEntities.Add(wiXFragment);
							wiXFragment.Parent.SetDirty();
						}
						wiXEntity = VSDialogBase.AddXmlUIBlock(xmlContent, wiXFragment, null, null, project, out wiXDialogs);
						if (wiXEntity != null)
						{
							project.SupportedEntities.Add(wiXEntity);
							foreach (AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog1 in wiXDialogs)
							{
								project.SupportedEntities.Add(wiXDialog1);
							}
						}
					}
				}
			}
			if (wiXEntity != null)
			{
				string str1 = (scope != AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall ? userDlgID : adminDlgID);
				wiXDialog = wiXEntity.ChildEntities.Find((WiXEntity d) => {
					if (!(d is AddinExpress.Installer.WiXDesigner.WiXDialog))
					{
						return false;
					}
					return d.GetAttributeValue("Id") == str1;
				}) as AddinExpress.Installer.WiXDesigner.WiXDialog;
				if (wiXDialog == null)
				{
					string xmlContent1 = VSDialogBase.GetXmlContent(str, language, ns, isMultiLangSupported);
					if (!string.IsNullOrEmpty(xmlContent1))
					{
						foreach (KeyValuePair<string, string> keyValuePair in replacements)
						{
							xmlContent1 = xmlContent1.Replace(keyValuePair.Key, keyValuePair.Value);
						}
						List<AddinExpress.Installer.WiXDesigner.WiXDialog> wiXDialogs1 = VSDialogBase.AddInnerXmlDialogs(wiXEntity, xmlContent1, project);
						if (wiXDialogs1.Count > 0)
						{
							wiXDialog = wiXEntity.ChildEntities.Find((WiXEntity d) => {
								if (!(d is AddinExpress.Installer.WiXDesigner.WiXDialog))
								{
									return false;
								}
								return d.GetAttributeValue("Id") == str1;
							}) as AddinExpress.Installer.WiXDesigner.WiXDialog;
							foreach (AddinExpress.Installer.WiXDesigner.WiXDialog wiXDialog2 in wiXDialogs1)
							{
								project.SupportedEntities.Add(wiXDialog2);
							}
						}
					}
				}
			}
			return wiXDialog;
		}

		public override void Delete()
		{
			this.DeleteFromUI();
			if (this.WiXDialog != null)
			{
				string str = this.WiXDialog.GetAttributeValue("Id");
				WiXEntity parent = this.WiXDialog.Parent as WiXEntity;
				this.Project.SupportedEntities.Remove(this.WiXDialog);
				this.WiXDialog.Delete();
				List<WiXEntity> wiXEntities = this.Project.SupportedEntities.FindAll((WiXEntity e) => {
					if (!(e is WiXDialogRef))
					{
						return false;
					}
					return e.GetAttributeValue("Id") == str;
				});
				if (wiXEntities.Count > 0)
				{
					foreach (WiXEntity wiXEntity in wiXEntities)
					{
						this.Project.SupportedEntities.Remove(wiXEntity);
						wiXEntity.Delete();
					}
				}
				if (parent != null && !parent.HasChildEntities)
				{
					string str1 = parent.GetAttributeValue("Id");
					wiXEntities = this.Project.SupportedEntities.FindAll((WiXEntity e) => {
						if (!(e is WiXUIRef))
						{
							return false;
						}
						return e.GetAttributeValue("Id") == str1;
					});
					if (wiXEntities.Count > 0)
					{
						foreach (WiXEntity wiXEntity1 in wiXEntities)
						{
							this.Project.SupportedEntities.Remove(wiXEntity1);
							wiXEntity1.Delete();
						}
					}
					this.Project.SupportedEntities.Remove(parent);
					parent.Delete();
				}
			}
			this._collection.IndexOf(this);
			this._collection.Remove(this);
			int num = 0;
			foreach (VSDialogBase vSDialogBase in this._collection)
			{
				if (vSDialogBase.DialogType == AddinExpress.Installer.WiXDesigner.DialogType.Custom)
				{
					continue;
				}
				num++;
			}
			if (num == 0)
			{
				WiXEntity wiXEntity2 = this._collection.Project.SupportedEntities.Find((WiXEntity e) => {
					if (!(e is WiXUI))
					{
						return false;
					}
					return e.GetAttributeValue("Id") == "Base";
				});
				if (wiXEntity2 != null)
				{
					WiXEntity parent1 = wiXEntity2.Parent as WiXEntity;
					this._collection.Project.SupportedEntities.Remove(wiXEntity2);
					wiXEntity2.Delete();
					if (parent1 != null)
					{
						parent1.SetDirty();
					}
				}
				WiXEntity wiXEntity3 = this._collection.Project.SupportedEntities.Find((WiXEntity e) => {
					if (!(e is WiXCustomAction))
					{
						return false;
					}
					return e.GetAttributeValue("Id") == "ERRCA_UIANDADVERTISED";
				});
				if (wiXEntity3 != null)
				{
					WiXEntity parent2 = wiXEntity3.Parent as WiXEntity;
					this._collection.Project.SupportedEntities.Remove(wiXEntity3);
					wiXEntity3.Delete();
					if (parent2 != null)
					{
						parent2.SetDirty();
					}
				}
				WiXEntity wiXEntity4 = this._collection.Project.SupportedEntities.Find((WiXEntity e) => {
					if (!(e is WiXCustom))
					{
						return false;
					}
					return e.GetAttributeValue("Action") == "ERRCA_UIANDADVERTISED";
				});
				if (wiXEntity4 != null)
				{
					WiXEntity parent3 = wiXEntity4.Parent as WiXEntity;
					this._collection.Project.SupportedEntities.Remove(wiXEntity4);
					wiXEntity4.Delete();
					if (parent3 != null)
					{
						parent3.SetDirty();
					}
				}
				WiXEntity wiXEntity5 = this._collection.Project.SupportedEntities.Find((WiXEntity e) => {
					if (!(e is WiXUI))
					{
						return false;
					}
					return e.GetAttributeValue("Id") == "BasicDialogs";
				});
				if (wiXEntity5 != null)
				{
					WiXEntity parent4 = wiXEntity5.Parent as WiXEntity;
					this._collection.Project.SupportedEntities.Remove(wiXEntity5);
					wiXEntity5.Delete();
					if (parent4 != null)
					{
						parent4.SetDirty();
					}
				}
				WiXEntity wiXEntity6 = this._collection.Project.SupportedEntities.Find((WiXEntity e) => {
					if (!(e is WiXUI))
					{
						return false;
					}
					return e.GetAttributeValue("Id") == "UserInterface";
				});
				if (wiXEntity6 != null)
				{
					WiXEntity parent5 = wiXEntity6.Parent as WiXEntity;
					this._collection.Project.SupportedEntities.Remove(wiXEntity6);
					wiXEntity6.Delete();
					if (parent5 != null)
					{
						parent5.SetDirty();
					}
				}
				WiXEntity wiXEntity7 = this._collection.Project.SupportedEntities.Find((WiXEntity e) => {
					if (!(e is WiXUI))
					{
						return false;
					}
					return e.GetAttributeValue("Id") == "FinishDialogs";
				});
				if (wiXEntity7 != null)
				{
					WiXEntity parent6 = wiXEntity7.Parent as WiXEntity;
					this._collection.Project.SupportedEntities.Remove(wiXEntity7);
					wiXEntity7.Delete();
					if (parent6 != null)
					{
						parent6.SetDirty();
					}
				}
				WiXEntity wiXEntity8 = this._collection.Project.SupportedEntities.Find((WiXEntity e) => {
					if (!(e is WiXUI))
					{
						return false;
					}
					return e.GetAttributeValue("Id") == "FinishedDlg";
				});
				if (wiXEntity8 != null)
				{
					WiXEntity parent7 = wiXEntity8.Parent as WiXEntity;
					this._collection.Project.SupportedEntities.Remove(wiXEntity8);
					wiXEntity8.Delete();
					if (parent7 != null)
					{
						parent7.SetDirty();
					}
				}
				WiXEntity wiXEntity9 = this._collection.Project.SupportedEntities.Find((WiXEntity e) => {
					if (!(e is WiXUIRef))
					{
						return false;
					}
					return e.GetAttributeValue("Id") == "FinishedDlg";
				});
				if (wiXEntity9 != null)
				{
					WiXEntity parent8 = wiXEntity9.Parent as WiXEntity;
					this._collection.Project.SupportedEntities.Remove(wiXEntity9);
					wiXEntity9.Delete();
					if (parent8 != null)
					{
						parent8.SetDirty();
					}
				}
				wiXEntity8 = this._collection.Project.SupportedEntities.Find((WiXEntity e) => {
					if (!(e is WiXUI))
					{
						return false;
					}
					return e.GetAttributeValue("Id") == "AdminFinishedDlg";
				});
				if (wiXEntity8 != null)
				{
					WiXEntity parent9 = wiXEntity8.Parent as WiXEntity;
					this._collection.Project.SupportedEntities.Remove(wiXEntity8);
					wiXEntity8.Delete();
					if (parent9 != null)
					{
						parent9.SetDirty();
					}
				}
				wiXEntity9 = this._collection.Project.SupportedEntities.Find((WiXEntity e) => {
					if (!(e is WiXUIRef))
					{
						return false;
					}
					return e.GetAttributeValue("Id") == "AdminFinishedDlg";
				});
				if (wiXEntity9 != null)
				{
					WiXEntity parent10 = wiXEntity9.Parent as WiXEntity;
					this._collection.Project.SupportedEntities.Remove(wiXEntity9);
					wiXEntity9.Delete();
					if (parent10 != null)
					{
						parent10.SetDirty();
					}
				}
				WiXEntity wiXEntity10 = this._collection.Project.SupportedEntities.Find((WiXEntity e) => {
					if (!(e is WiXUI))
					{
						return false;
					}
					return e.GetAttributeValue("Id") == "AdminFinishDialogs";
				});
				if (wiXEntity10 != null)
				{
					WiXEntity parent11 = wiXEntity10.Parent as WiXEntity;
					this._collection.Project.SupportedEntities.Remove(wiXEntity10);
					wiXEntity10.Delete();
					if (parent11 != null)
					{
						parent11.SetDirty();
					}
				}
				WiXEntity wiXEntity11 = this._collection.Project.SupportedEntities.Find((WiXEntity e) => {
					if (!(e is WiXCustomAction))
					{
						return false;
					}
					return e.GetAttributeValue("Id") == "VSDCA_FolderForm_AllUsers";
				});
				if (wiXEntity11 != null)
				{
					WiXEntity parent12 = wiXEntity11.Parent as WiXEntity;
					this._collection.Project.SupportedEntities.Remove(wiXEntity11);
					wiXEntity11.Delete();
					if (parent12 != null)
					{
						parent12.SetDirty();
					}
				}
				WiXEntity wiXEntity12 = this._collection.Project.SupportedEntities.Find((WiXEntity e) => {
					if (!(e is WiXCustom))
					{
						return false;
					}
					return e.GetAttributeValue("Action") == "VSDCA_FolderForm_AllUsers";
				});
				if (wiXEntity12 != null)
				{
					WiXEntity parent13 = wiXEntity12.Parent as WiXEntity;
					this._collection.Project.SupportedEntities.Remove(wiXEntity12);
					wiXEntity12.Delete();
					if (parent13 != null)
					{
						parent13.SetDirty();
					}
				}
				foreach (WiXEntity wiXEntity13 in this._collection.Project.SupportedEntities.FindAll((WiXEntity e) => {
					if (!(e is WiXShow))
					{
						return false;
					}
					string attributeValue = e.GetAttributeValue("Dialog");
					if (attributeValue == "UserExitForm" || attributeValue == "FatalErrorForm" || attributeValue == "MaintenanceForm" || attributeValue == "ResumeForm" || attributeValue == "AdminUserExitForm" || attributeValue == "AdminFatalErrorForm" || attributeValue == "AdminMaintenanceForm")
					{
						return true;
					}
					return attributeValue == "AdminResumeForm";
				}))
				{
					WiXEntity parent14 = wiXEntity13.Parent as WiXEntity;
					this._collection.Project.SupportedEntities.Remove(wiXEntity13);
					wiXEntity13.Delete();
					if (parent14 == null)
					{
						continue;
					}
					parent14.SetDirty();
				}
			}
		}

		internal bool DeleteFromUI()
		{
			if (this.WiXDialog == null)
			{
				return false;
			}
			WiXEntity parent = this.WiXDialog.Parent as WiXEntity;
			if (parent == null)
			{
				return false;
			}
			if (!(parent.Parent is WiXEntity))
			{
				return false;
			}
			this.SortDialogStage();
			string attributeValue = this.WiXDialog.GetAttributeValue("Id");
			VSDialogBase vSDialogBase = null;
			VSDialogBase vSDialogBase1 = null;
			string empty = string.Empty;
			string str = string.Empty;
			string empty1 = string.Empty;
			string attributeValue1 = string.Empty;
			string str1 = string.Empty;
			string innerText = string.Empty;
			if (this.WiXShowElement != null)
			{
				empty = this.WiXShowElement.GetAttributeValue("Dialog");
				str = this.WiXShowElement.GetAttributeValue("Sequence");
				empty1 = this.WiXShowElement.GetAttributeValue("After");
				attributeValue1 = this.WiXShowElement.GetAttributeValue("Before");
				str1 = this.WiXShowElement.GetAttributeValue("OnExit");
				if (this.WiXShowElement.FirstChild != null)
				{
					XmlNode xmlNode = ((WiXEntity)this.WiXShowElement.FirstChild).XmlNode;
					if (xmlNode != null)
					{
						innerText = xmlNode.InnerText;
					}
				}
			}
			if (this.IsChained)
			{
				if (this.WiXPrevArgsProperty != null)
				{
					string attributeValue2 = this.WiXPrevArgsProperty.GetAttributeValue("Value");
					if (!string.IsNullOrEmpty(attributeValue2))
					{
						vSDialogBase = this._collection.Find((VSDialogBase d) => d.WiXDialog.GetAttributeValue("Id") == attributeValue2);
					}
					WiXEntity wiXEntity = this.WiXPrevArgsProperty.Parent as WiXEntity;
					this.Project.SupportedEntities.Remove(this.WiXPrevArgsProperty);
					this.WiXPrevArgsProperty.Delete();
					this._wixPrevArgsPropElement = null;
					if (wiXEntity != null)
					{
						wiXEntity.SetDirty();
					}
				}
				if (this.WiXNextArgsProperty != null)
				{
					string str2 = this.WiXNextArgsProperty.GetAttributeValue("Value");
					if (!string.IsNullOrEmpty(str2))
					{
						vSDialogBase1 = this._collection.Find((VSDialogBase d) => d.WiXDialog.GetAttributeValue("Id") == str2);
					}
					WiXEntity parent1 = this.WiXNextArgsProperty.Parent as WiXEntity;
					this.Project.SupportedEntities.Remove(this.WiXNextArgsProperty);
					this.WiXNextArgsProperty.Delete();
					this._wixNextArgsPropElement = null;
					if (parent1 != null)
					{
						parent1.SetDirty();
					}
				}
				if (vSDialogBase != null && vSDialogBase.IsChained)
				{
					if (vSDialogBase.WiXNextArgsProperty == null || !(vSDialogBase.WiXNextArgsProperty.GetAttributeValue("Value") == attributeValue))
					{
						vSDialogBase = null;
					}
					else if (vSDialogBase1 == null)
					{
						vSDialogBase.SetNextDialogId(null);
					}
					else
					{
						vSDialogBase.SetNextDialogId(vSDialogBase1.WiXDialog.GetAttributeValue("Id"));
					}
				}
				if (vSDialogBase1 != null && vSDialogBase1.IsChained)
				{
					if (vSDialogBase1.WiXPrevArgsProperty == null || !(vSDialogBase1.WiXPrevArgsProperty.GetAttributeValue("Value") == attributeValue))
					{
						vSDialogBase1 = null;
					}
					else if (vSDialogBase == null)
					{
						vSDialogBase1.SetPrevDialogId(null);
					}
					else
					{
						vSDialogBase1.SetPrevDialogId(vSDialogBase.WiXDialog.GetAttributeValue("Id"));
					}
				}
			}
			if (this.WiXShowElement != null)
			{
				if (vSDialogBase1 == null)
				{
					empty = string.Empty;
					WiXEntity wiXEntity1 = this.WiXShowElement.Parent as WiXEntity;
					this.Project.SupportedEntities.Remove(this.WiXShowElement);
					this.WiXShowElement.Delete();
					this.WiXShowElement = null;
					if (wiXEntity1 != null)
					{
						wiXEntity1.SetDirty();
					}
				}
				else
				{
					WiXEntity parent2 = this.WiXShowElement.Parent as WiXEntity;
					this.Project.SupportedEntities.Remove(this.WiXShowElement);
					this.WiXShowElement.Delete();
					this.WiXShowElement = null;
					if (parent2 != null)
					{
						parent2.SetDirty();
					}
					WiXEntity wiXEntity2 = vSDialogBase1.WiXDialog.Parent as WiXEntity;
					if (wiXEntity2 != null)
					{
						WiXEntity parent3 = wiXEntity2.Parent as WiXEntity;
						if (parent3 != null)
						{
							empty = vSDialogBase1.WiXDialog.Id;
							WiXEntity uISequence = VSDialogBase.GetUISequence(this.dialogScope, parent3, null, this.Project);
							vSDialogBase1.WiXShowElement = VSDialogBase.CreateShowEntity(empty, uISequence, str, empty1, attributeValue1, str1, innerText);
						}
					}
				}
				if (string.IsNullOrEmpty(str1))
				{
					if (string.IsNullOrEmpty(empty))
					{
						VSDialogBase vSDialogBase2 = this._collection.Find((VSDialogBase e) => {
							if (e.WiXShowElement == null)
							{
								return false;
							}
							return e.WiXShowElement.GetAttributeValue("After") == attributeValue;
						});
						if (vSDialogBase2 == null)
						{
							VSDialogBase vSDialogBase3 = this._collection.Find((VSDialogBase e) => {
								if (e.WiXShowElement == null)
								{
									return false;
								}
								return e.WiXShowElement.GetAttributeValue("Before") == attributeValue;
							});
							if (vSDialogBase3 != null)
							{
								empty = vSDialogBase3.WiXDialog.GetAttributeValue("Id");
								WiXEntity wiXEntity3 = vSDialogBase3.WiXShowElement.Parent as WiXEntity;
								this.Project.SupportedEntities.Remove(vSDialogBase3.WiXShowElement);
								vSDialogBase3.WiXShowElement.Delete();
								vSDialogBase3.WiXShowElement = VSDialogBase.CreateShowEntity(empty, wiXEntity3, str, empty1, attributeValue1, string.Empty, innerText);
							}
						}
						else
						{
							empty = vSDialogBase2.WiXDialog.GetAttributeValue("Id");
							WiXEntity parent4 = vSDialogBase2.WiXShowElement.Parent as WiXEntity;
							this.Project.SupportedEntities.Remove(vSDialogBase2.WiXShowElement);
							vSDialogBase2.WiXShowElement.Delete();
							vSDialogBase2.WiXShowElement = VSDialogBase.CreateShowEntity(empty, parent4, str, empty1, attributeValue1, string.Empty, innerText);
						}
					}
					if (!string.IsNullOrEmpty(empty))
					{
						List<VSDialogBase> vSDialogBases = this._collection.FindAll((VSDialogBase e) => {
							if (e.WiXShowElement == null)
							{
								return false;
							}
							if (e.WiXShowElement.GetAttributeValue("After") == attributeValue)
							{
								return true;
							}
							return e.WiXShowElement.GetAttributeValue("Before") == attributeValue;
						});
						if (vSDialogBases.Count > 0)
						{
							foreach (VSDialogBase vSDialogBase4 in vSDialogBases)
							{
								if (string.IsNullOrEmpty(vSDialogBase4.WiXShowElement.GetAttributeValue("After")))
								{
									if (string.IsNullOrEmpty(vSDialogBase4.WiXShowElement.GetAttributeValue("Before")))
									{
										continue;
									}
									vSDialogBase4.WiXShowElement.SetAttributeValue("Before", empty);
									vSDialogBase4.WiXShowElement.SetDirty();
								}
								else
								{
									vSDialogBase4.WiXShowElement.SetAttributeValue("After", empty);
									vSDialogBase4.WiXShowElement.SetDirty();
								}
							}
						}
					}
				}
			}
			return true;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		internal static VSDialogBase FindDialog(AddinExpress.Installer.WiXDesigner.DialogType dialogType, AddinExpress.Installer.WiXDesigner.DialogScope scope, AddinExpress.Installer.WiXDesigner.DialogStage stage, VSUserInterface collection)
		{
			VSDialogBase vSDialogBase;
			List<VSDialogBase>.Enumerator enumerator = collection.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					VSDialogBase current = enumerator.Current;
					if (current.DialogType != dialogType || current.DialogScope != scope || current.DialogStage != stage)
					{
						continue;
					}
					vSDialogBase = current;
					return vSDialogBase;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return vSDialogBase;
		}

		internal VSBaseFile GetBannerBitmapValue()
		{
			VSBaseFile fileById;
			if (this._wixBannerBitmap != null)
			{
				string attributeValue = this._wixBannerBitmap.GetAttributeValue("Text");
				if (!string.IsNullOrEmpty(attributeValue))
				{
					List<VSBinary> binaries = this._project.Binaries;
					if (binaries != null && binaries.Count > 0)
					{
						fileById = binaries.Find((VSBinary b) => b.WiXElement.Id == attributeValue);
						if (fileById != null)
						{
							return fileById;
						}
					}
					fileById = this._project.FileSystem.GetFileById(attributeValue);
					if (fileById != null)
					{
						return fileById;
					}
				}
			}
			return VSBaseFile.Empty;
		}

		protected override string GetClassName()
		{
			return "User Interface Dialog Properties";
		}

		protected override string GetComponentName()
		{
			return this.Name;
		}

		public static string GetCultureByLCID(string language)
		{
			int num;
			string str;
			if (!string.IsNullOrEmpty(language) && int.TryParse(language, out num))
			{
				try
				{
					str = (!ProjectUtilities.IsNeutralLCID(num) ? ProjectUtilities.GetCultureInfo(num).Name : "neutral");
				}
				catch (Exception exception)
				{
					return string.Empty;
				}
				return str;
			}
			return string.Empty;
		}

		public static CultureInfo GetCultureInfoByLCID(string language)
		{
			int num;
			CultureInfo cultureInfo = null;
			if (!string.IsNullOrEmpty(language) && int.TryParse(language, out num))
			{
				try
				{
					cultureInfo = (!ProjectUtilities.IsNeutralLCID(num) ? ProjectUtilities.GetCultureInfo(num) : CultureInfo.InvariantCulture);
				}
				catch (Exception exception)
				{
				}
			}
			return cultureInfo;
		}

		private string GetDialogName()
		{
			string attributeValue = this._wixElement.GetAttributeValue("Id");
			if (attributeValue != null)
			{
				if (attributeValue == "CustomCheckA")
				{
					return "Checkboxes  (A)";
				}
				if (attributeValue == "CustomCheckB")
				{
					return "Checkboxes  (B)";
				}
				if (attributeValue == "CustomCheckC")
				{
					return "Checkboxes  (C)";
				}
				if (attributeValue == "AdminConfirmInstallForm" || attributeValue == "ConfirmInstallForm")
				{
					return "Confirm Installation";
				}
				if (attributeValue == "CustomerInfoForm")
				{
					return "Customer Information";
				}
				if (attributeValue == "AdminFinishedForm" || attributeValue == "FinishedForm")
				{
					return "Finished";
				}
				if (attributeValue == "AdminFolderForm")
				{
					return "Installation Folder";
				}
				if (attributeValue == "FolderForm")
				{
					return "Installation Folder";
				}
				if (attributeValue == "AdminEulaForm" || attributeValue == "EulaForm")
				{
					return "License Agreement";
				}
				if (attributeValue == "AdminProgressForm" || attributeValue == "ProgressForm")
				{
					return "Progress";
				}
				if (attributeValue == "Custom2Buttons")
				{
					return "RadioButtons  (2 buttons)";
				}
				if (attributeValue == "Custom3Buttons")
				{
					return "RadioButtons  (3 buttons)";
				}
				if (attributeValue == "Custom4Buttons")
				{
					return "RadioButtons  (4 buttons)";
				}
				if (attributeValue == "AdminReadmeForm" || attributeValue == "ReadmeForm")
				{
					return "Read Me";
				}
				if (attributeValue == "RegisterUserExeForm")
				{
					return "Register User";
				}
				if (attributeValue == "AdminSplashForm" || attributeValue == "SplashForm")
				{
					return "Splash";
				}
				if (attributeValue == "CustomTextA")
				{
					return "Textboxes (A)";
				}
				if (attributeValue == "CustomTextB")
				{
					return "Textboxes (B)";
				}
				if (attributeValue == "CustomTextC")
				{
					return "Textboxes (C)";
				}
				if (attributeValue == "AdminWelcomeForm" || attributeValue == "WelcomeForm")
				{
					return "Welcome";
				}
				if (attributeValue == "AdminWebFolderForm" || attributeValue == "WebFolderForm")
				{
					return "Installation Address";
				}
			}
			return null;
		}

		private string GetDisplayName()
		{
			switch (this.dialogType)
			{
				case AddinExpress.Installer.WiXDesigner.DialogType.ConfirmInstallation:
				{
					return "Confirm Installation";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.RadioButtons2:
				{
					return "RadioButtons (2 buttons)";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.RadioButtons3:
				{
					return "RadioButtons (3 buttons)";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.RadioButtons4:
				{
					return "RadioButtons (4 buttons)";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.CheckBoxesA:
				{
					return "Checkboxes (A)";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.CheckBoxesB:
				{
					return "Checkboxes (B)";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.CheckBoxesC:
				{
					return "Checkboxes (C)";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.CustomerInformation:
				{
					return "Customer Information";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.TextBoxesA:
				{
					return "Textboxes (A)";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.TextBoxesB:
				{
					return "Textboxes (B)";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.TextBoxesC:
				{
					return "Textboxes (C)";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.Finished:
				{
					return "Finished";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.InstallationFolder:
				{
					return "Installation Folder";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.LicenseAgreement:
				{
					return "License Agreement";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.Progress:
				{
					return "Progress";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.ReadMe:
				{
					return "Read Me";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.RegisterUser:
				{
					return "Register User";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.Splash:
				{
					return "Splash";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.Welcome:
				{
					return "Welcome";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.Empty:
				{
					return "Empty Dialog";
				}
				case AddinExpress.Installer.WiXDesigner.DialogType.InstallationAddress:
				{
					return "Installation Address";
				}
			}
			return string.Empty;
		}

		private static WiXEntity GetExecuteSequence(WiXEntity parent, WiXEntity before, WiXProjectParser project)
		{
			WiXEntity wiXInstallExecuteSequence = null;
			wiXInstallExecuteSequence = parent.ChildEntities.Find((WiXEntity e) => e is WiXInstallExecuteSequence);
			if (wiXInstallExecuteSequence == null)
			{
				XmlElement xmlElement = Common.CreateXmlElementWithAttributes(parent.XmlNode.OwnerDocument, "InstallExecuteSequence", parent.XmlNode.NamespaceURI, new string[0], new string[0], "", false);
				if (before == null)
				{
					parent.XmlNode.AppendChild(xmlElement);
				}
				else
				{
					parent.XmlNode.InsertBefore(xmlElement, before.XmlNode);
				}
				wiXInstallExecuteSequence = new WiXInstallExecuteSequence(project, parent.Owner, parent, xmlElement);
				project.SupportedEntities.Add(wiXInstallExecuteSequence);
				parent.SetDirty();
			}
			return wiXInstallExecuteSequence;
		}

		private string GetFileNameById(string id)
		{
			return null;
		}

		private VSDialogBase GetFirstChainedDialog(VSDialogBase beginDialog)
		{
			VSDialogBase i;
			VSDialogBase vSDialogBase = null;
			for (i = beginDialog; i.WiXPrevArgsProperty != null && i.WiXShowElement == null; i = vSDialogBase)
			{
				string attributeValue = i.WiXPrevArgsProperty.GetAttributeValue("Value");
				vSDialogBase = this._collection.Find((VSDialogBase e) => e.WiXDialog.GetAttributeValue("Id") == attributeValue);
				if (vSDialogBase == null)
				{
					break;
				}
			}
			return i;
		}

		private static List<int> GetFreeCheckboxSequences(WiXProjectParser project)
		{
			string attributeValue = null;
			List<int> nums = new List<int>();
			for (int i = 701; i <= 799; i++)
			{
				nums.Add(i);
			}
			project.SupportedEntities.FindAll((WiXEntity e) => {
				int num;
				if (e is WiXCustom)
				{
					attributeValue = e.GetAttributeValue("Sequence");
					if (!string.IsNullOrEmpty(attributeValue) && int.TryParse(attributeValue, out num) && num > 700 && num < 800)
					{
						nums.Remove(num);
					}
				}
				return false;
			});
			return nums;
		}

		internal static AddinExpress.Installer.WiXDesigner.DialogStage GetInstallStage(WiXEntity wixEntity, List<WiXEntity> items, ref int order)
		{
			int num;
			WiXEntity wiXEntity;
			AddinExpress.Installer.WiXDesigner.DialogStage dialogStage;
			AddinExpress.Installer.WiXDesigner.DialogStage dialogStage1 = AddinExpress.Installer.WiXDesigner.DialogStage.Undefined;
			string attributeValue = wixEntity.GetAttributeValue("Sequence");
			if (!string.IsNullOrEmpty(attributeValue) && int.TryParse(attributeValue, out num))
			{
				order = num;
				if (num < 1280)
				{
					dialogStage = AddinExpress.Installer.WiXDesigner.DialogStage.Start;
				}
				else
				{
					dialogStage = (num > 1300 ? AddinExpress.Installer.WiXDesigner.DialogStage.End : AddinExpress.Installer.WiXDesigner.DialogStage.Progress);
				}
				dialogStage1 = dialogStage;
			}
			if (dialogStage1 == AddinExpress.Installer.WiXDesigner.DialogStage.Undefined)
			{
				string str = "before";
				string attributeValue1 = wixEntity.GetAttributeValue("Before");
				if (string.IsNullOrEmpty(attributeValue1))
				{
					str = "after";
					attributeValue1 = wixEntity.GetAttributeValue("After");
				}
				if (!string.IsNullOrEmpty(attributeValue1))
				{
					dialogStage1 = (!VSDialogBase.IsExistsInSequence(attributeValue1, items, out wiXEntity) ? VSDialogBase.GetStandardActionStage(attributeValue1, str, false, ref order) : VSDialogBase.GetInstallStage(wiXEntity, items, ref order));
				}
			}
			if (dialogStage1 == AddinExpress.Installer.WiXDesigner.DialogStage.Undefined && !string.IsNullOrEmpty(wixEntity.GetAttributeValue("OnExit")))
			{
				order = -1;
				dialogStage1 = AddinExpress.Installer.WiXDesigner.DialogStage.End;
			}
			return dialogStage1;
		}

		private VSDialogBase GetLastChainedDialog(VSDialogBase beginDialog)
		{
			VSDialogBase i;
			VSDialogBase vSDialogBase = null;
			for (i = beginDialog; i.WiXNextArgsProperty != null; i = vSDialogBase)
			{
				string attributeValue = i.WiXNextArgsProperty.GetAttributeValue("Value");
				vSDialogBase = this._collection.Find((VSDialogBase e) => e.WiXDialog.GetAttributeValue("Id") == attributeValue);
				if (vSDialogBase == null)
				{
					break;
				}
			}
			return i;
		}

		internal static WiXControl GetNextButton(WiXEntityList children)
		{
			return (WiXControl)children.Find((WiXEntity e) => {
				if (!(e is WiXControl) || !(e.GetAttributeValue("Type") == "PushButton"))
				{
					return false;
				}
				WiXEntity wiXEntity = e.ChildEntities.Find((WiXEntity c) => {
					if (c.Name != "Publish")
					{
						return false;
					}
					return c.GetAttributeValue("Event") == "NewDialog";
				});
				WiXEntity wiXEntity1 = e.ChildEntities.Find((WiXEntity c) => {
					if (c.Name != "Publish")
					{
						return false;
					}
					return c.GetAttributeValue("Event") == "EndDialog";
				});
				if (wiXEntity == null)
				{
					return false;
				}
				return wiXEntity1 != null;
			});
		}

		internal static WiXControl GetPrevButton(WiXEntityList children)
		{
			return (WiXControl)children.Find((WiXEntity e) => {
				if (!(e is WiXControl) || !(e.GetAttributeValue("Type") == "PushButton"))
				{
					return false;
				}
				WiXEntity wiXEntity = e.ChildEntities.Find((WiXEntity c) => {
					if (c.Name != "Publish")
					{
						return false;
					}
					return c.GetAttributeValue("Event") == "NewDialog";
				});
				WiXEntity wiXEntity1 = e.ChildEntities.Find((WiXEntity c) => {
					if (c.Name != "Publish")
					{
						return false;
					}
					return c.GetAttributeValue("Event") == "EndDialog";
				});
				if (wiXEntity == null)
				{
					return false;
				}
				return wiXEntity1 == null;
			});
		}

		private static WiXProperty GetPropertyByButton(WiXControl wixControl, List<WiXProperty> propertyList, out string propId)
		{
			propId = null;
			WiXProperty wiXProperty = null;
			WiXEntity wiXEntity = wixControl.ChildEntities.Find((WiXEntity c) => {
				if (c.Name != "Publish")
				{
					return false;
				}
				return c.GetAttributeValue("Event") == "NewDialog";
			});
			if (wiXEntity != null)
			{
				string attributeValue = wiXEntity.GetAttributeValue("Value");
				if (!string.IsNullOrEmpty(attributeValue))
				{
					attributeValue = attributeValue.Replace("[", string.Empty);
					attributeValue = attributeValue.Replace("]", string.Empty);
					propId = attributeValue;
					WiXProperty wiXProperty1 = propertyList.Find((WiXProperty p) => p.GetAttributeValue("Id") == attributeValue);
					if (wiXProperty1 != null)
					{
						wiXProperty = wiXProperty1;
					}
				}
			}
			return wiXProperty;
		}

		private static WiXProperty GetPropertyByButton(WiXControl wixControl, WiXProjectParser project, out string propId)
		{
			propId = null;
			WiXProperty wiXProperty = null;
			WiXEntity wiXEntity = wixControl.ChildEntities.Find((WiXEntity c) => {
				if (c.Name != "Publish")
				{
					return false;
				}
				return c.GetAttributeValue("Event") == "NewDialog";
			});
			if (wiXEntity != null)
			{
				string attributeValue = wiXEntity.GetAttributeValue("Value");
				if (!string.IsNullOrEmpty(attributeValue))
				{
					attributeValue = attributeValue.Replace("[", string.Empty);
					attributeValue = attributeValue.Replace("]", string.Empty);
					propId = attributeValue;
					WiXProperty wiXProperty1 = project.SupportedEntities.Find((WiXEntity p) => {
						if (!(p is WiXProperty))
						{
							return false;
						}
						return p.GetAttributeValue("Id") == attributeValue;
					}) as WiXProperty;
					if (wiXProperty1 != null)
					{
						wiXProperty = wiXProperty1;
					}
				}
			}
			return wiXProperty;
		}

		private static WiXEntity GetShowElement(WiXEntity dialog, List<WiXEntity> sequenceList)
		{
			string attributeValue = dialog.GetAttributeValue("Id");
			return sequenceList.Find((WiXEntity e) => {
				if (!(e is WiXShow))
				{
					return false;
				}
				return e.GetAttributeValue("Dialog") == attributeValue;
			});
		}

		internal static AddinExpress.Installer.WiXDesigner.DialogStage GetStandardActionStage(string actionName, string direction, bool includeDefaultUI, ref int order)
		{
			if (includeDefaultUI)
			{
				if (actionName != null)
				{
					if (actionName == "LaunchConditions")
					{
						order = 100;
						return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
					}
					if (actionName == "PrepareDlg")
					{
						order = 140;
						return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
					}
					if (actionName == "AppSearch")
					{
						order = 400;
						return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
					}
					if (actionName == "CCPSearch")
					{
						order = 500;
						return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
					}
					if (actionName == "RMCCPSearch")
					{
						order = 600;
						return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
					}
					if (actionName == "CostInitialize")
					{
						order = 800;
						return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
					}
					if (actionName == "FileCost")
					{
						order = 900;
						return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
					}
					if (actionName == "CostFinalize")
					{
						order = 1000;
						return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
					}
					if (actionName == "WelcomeDlg")
					{
						order = 1230;
						return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
					}
					if (actionName == "ResumeDlg")
					{
						order = 1240;
						return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
					}
					if (actionName == "MaintenanceWelcomeDlg")
					{
						order = 1250;
						return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
					}
					if (actionName == "CustomCheckA" || actionName == "CustomCheckB" || actionName == "CustomCheckC" || actionName == "AdminConfirmInstallForm" || actionName == "ConfirmInstallForm" || actionName == "CustomerInfoForm" || actionName == "AdminFolderForm" || actionName == "FolderForm" || actionName == "AdminEulaForm" || actionName == "EulaForm" || actionName == "Custom2Buttons" || actionName == "Custom3Buttons" || actionName == "Custom4Buttons" || actionName == "AdminReadmeForm" || actionName == "ReadmeForm" || actionName == "RegisterUserExeForm" || actionName == "AdminSplashForm" || actionName == "SplashForm" || actionName == "CustomTextA" || actionName == "CustomTextB" || actionName == "CustomTextC" || actionName == "AdminWelcomeForm" || actionName == "WelcomeForm" || actionName == "AdminWebFolderForm" || actionName == "WebFolderForm")
					{
						if (direction == "end")
						{
							order = 1301;
							return AddinExpress.Installer.WiXDesigner.DialogStage.End;
						}
						order = 1001;
						return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
					}
					if (actionName == "SetODBCFolders")
					{
						order = 1100;
						return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
					}
					if (actionName == "ProgressDlg")
					{
						order = 1280;
						return AddinExpress.Installer.WiXDesigner.DialogStage.Progress;
					}
					if (actionName == "ExecuteAction")
					{
						if (direction == "after")
						{
							order = 1301;
							return AddinExpress.Installer.WiXDesigner.DialogStage.End;
						}
						if (direction == "before")
						{
							order = 1279;
							return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
						}
						order = 1300;
						return AddinExpress.Installer.WiXDesigner.DialogStage.Progress;
					}
					if (actionName == "AdminProgressForm" || actionName == "ProgressForm")
					{
						order = 1299;
						return AddinExpress.Installer.WiXDesigner.DialogStage.Progress;
					}
					if (actionName == "FatalErrorDlg")
					{
						order = -3;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "UserExitDlg")
					{
						order = -2;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "ExitDlg")
					{
						order = -1;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "AdminFinishedForm")
					{
						order = 6610;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "FinishedForm")
					{
						order = 6610;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "UserExitForm")
					{
						order = -2;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "FatalErrorForm")
					{
						order = -3;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "InstallValidate")
					{
						order = 1400;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "InstallInitialize")
					{
						order = 1500;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "AllocateRegistrySpace")
					{
						order = 1550;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "ProcessComponents")
					{
						order = 1600;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "UnpublishComponents")
					{
						order = 1700;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "UnpublishFeatures")
					{
						order = 1800;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "StopServices")
					{
						order = 1900;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "DeleteServices")
					{
						order = 2000;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "UnregisterComPlus")
					{
						order = 2100;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "SelfUnregModules")
					{
						order = 2200;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "UnregisterTypeLibraries")
					{
						order = 2300;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "RemoveODBC")
					{
						order = 2400;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "UnregisterFonts")
					{
						order = 2500;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "RemoveRegistryValues")
					{
						order = 2600;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "UnregisterClassInfo")
					{
						order = 2700;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "UnregisterExtensionInfo")
					{
						order = 2800;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "UnregisterProgIdInfo")
					{
						order = 2900;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "UnregisterMIMEInfo")
					{
						order = 3000;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "RemoveIniValues")
					{
						order = 3100;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "RemoveShortcuts")
					{
						order = 3200;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "RemoveEnvironmentStrings")
					{
						order = 3300;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "RemoveDuplicateFiles")
					{
						order = 3400;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "RemoveFiles")
					{
						order = 3500;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "RemoveFolders")
					{
						order = 3600;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "CreateFolders")
					{
						order = 3700;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "MoveFiles")
					{
						order = 3800;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "InstallAdminPackage")
					{
						order = 3900;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "InstallFiles")
					{
						order = 4000;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "PatchFiles")
					{
						order = 4090;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "DuplicateFiles")
					{
						order = 4210;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "BindImage")
					{
						order = 4300;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "CreateShortcuts")
					{
						order = 4500;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "RegisterClassInfo")
					{
						order = 4600;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "RegisterExtensionInfo")
					{
						order = 4700;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "RegisterProgIdInfo")
					{
						order = 4800;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "RegisterMIMEInfo")
					{
						order = 4900;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "WriteRegistryValues")
					{
						order = 5000;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "WriteIniValues")
					{
						order = 5100;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "WriteEnvironmentStrings")
					{
						order = 5200;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "RegisterFonts")
					{
						order = 5300;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "InstallODBC")
					{
						order = 5400;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "RegisterTypeLibraries")
					{
						order = 5500;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "SelfRegModules")
					{
						order = 5600;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "RegisterComPlus")
					{
						order = 5700;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "InstallServices")
					{
						order = 5800;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "StartServices")
					{
						order = 5900;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "RegisterUser")
					{
						order = 6000;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "RegisterProduct")
					{
						order = 6100;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "PublishComponents")
					{
						order = 6200;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "PublishFeatures")
					{
						order = 6300;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "PublishProduct")
					{
						order = 6400;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (actionName == "InstallFinalize")
					{
						order = 6600;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
				}
			}
			else if (actionName != null)
			{
				if (actionName == "LaunchConditions")
				{
					order = 100;
					return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
				}
				if (actionName == "PrepareDlg")
				{
					order = 140;
					return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
				}
				if (actionName == "AppSearch")
				{
					order = 400;
					return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
				}
				if (actionName == "CCPSearch")
				{
					order = 500;
					return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
				}
				if (actionName == "RMCCPSearch")
				{
					order = 600;
					return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
				}
				if (actionName == "CostInitialize")
				{
					order = 800;
					return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
				}
				if (actionName == "FileCost")
				{
					order = 900;
					return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
				}
				if (actionName == "CostFinalize")
				{
					order = 1000;
					return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
				}
				if (actionName == "WelcomeDlg")
				{
					order = 1230;
					return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
				}
				if (actionName == "ResumeDlg")
				{
					order = 1240;
					return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
				}
				if (actionName == "MaintenanceWelcomeDlg")
				{
					order = 1250;
					return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
				}
				if (actionName == "SetODBCFolders")
				{
					order = 1100;
					return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
				}
				if (actionName == "ProgressDlg")
				{
					order = 1280;
					return AddinExpress.Installer.WiXDesigner.DialogStage.Progress;
				}
				if (actionName == "ExecuteAction")
				{
					if (direction == "after")
					{
						order = 1301;
						return AddinExpress.Installer.WiXDesigner.DialogStage.End;
					}
					if (direction == "before")
					{
						order = 1279;
						return AddinExpress.Installer.WiXDesigner.DialogStage.Start;
					}
					order = 1300;
					return AddinExpress.Installer.WiXDesigner.DialogStage.Progress;
				}
				if (actionName == "FatalErrorDlg")
				{
					order = -3;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "UserExitDlg")
				{
					order = -2;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "ExitDlg")
				{
					order = -1;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "InstallValidate")
				{
					order = 1400;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "InstallInitialize")
				{
					order = 1500;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "AllocateRegistrySpace")
				{
					order = 1550;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "ProcessComponents")
				{
					order = 1600;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "UnpublishComponents")
				{
					order = 1700;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "UnpublishFeatures")
				{
					order = 1800;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "StopServices")
				{
					order = 1900;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "DeleteServices")
				{
					order = 2000;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "UnregisterComPlus")
				{
					order = 2100;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "SelfUnregModules")
				{
					order = 2200;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "UnregisterTypeLibraries")
				{
					order = 2300;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "RemoveODBC")
				{
					order = 2400;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "UnregisterFonts")
				{
					order = 2500;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "RemoveRegistryValues")
				{
					order = 2600;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "UnregisterClassInfo")
				{
					order = 2700;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "UnregisterExtensionInfo")
				{
					order = 2800;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "UnregisterProgIdInfo")
				{
					order = 2900;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "UnregisterMIMEInfo")
				{
					order = 3000;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "RemoveIniValues")
				{
					order = 3100;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "RemoveShortcuts")
				{
					order = 3200;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "RemoveEnvironmentStrings")
				{
					order = 3300;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "RemoveDuplicateFiles")
				{
					order = 3400;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "RemoveFiles")
				{
					order = 3500;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "RemoveFolders")
				{
					order = 3600;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "CreateFolders")
				{
					order = 3700;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "MoveFiles")
				{
					order = 3800;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "InstallAdminPackage")
				{
					order = 3900;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "InstallFiles")
				{
					order = 4000;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "PatchFiles")
				{
					order = 4090;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "DuplicateFiles")
				{
					order = 4210;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "BindImage")
				{
					order = 4300;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "CreateShortcuts")
				{
					order = 4500;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "RegisterClassInfo")
				{
					order = 4600;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "RegisterExtensionInfo")
				{
					order = 4700;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "RegisterProgIdInfo")
				{
					order = 4800;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "RegisterMIMEInfo")
				{
					order = 4900;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "WriteRegistryValues")
				{
					order = 5000;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "WriteIniValues")
				{
					order = 5100;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "WriteEnvironmentStrings")
				{
					order = 5200;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "RegisterFonts")
				{
					order = 5300;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "InstallODBC")
				{
					order = 5400;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "RegisterTypeLibraries")
				{
					order = 5500;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "SelfRegModules")
				{
					order = 5600;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "RegisterComPlus")
				{
					order = 5700;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "InstallServices")
				{
					order = 5800;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "StartServices")
				{
					order = 5900;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "RegisterUser")
				{
					order = 6000;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "RegisterProduct")
				{
					order = 6100;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "PublishComponents")
				{
					order = 6200;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "PublishFeatures")
				{
					order = 6300;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "PublishProduct")
				{
					order = 6400;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
				if (actionName == "InstallFinalize")
				{
					order = 6600;
					return AddinExpress.Installer.WiXDesigner.DialogStage.End;
				}
			}
			return AddinExpress.Installer.WiXDesigner.DialogStage.Undefined;
		}

		internal static WiXEntity GetStandardUIProjectItem(WiXProjectParser project, string ns)
		{
			WiXProjectItem wiXProjectItem = null;
			string str = Path.Combine(Path.GetDirectoryName(project.ProjectManager.VsProject.FullName), "StandardUI.wxs");
			foreach (WiXProjectItem projectItem in project.ProjectItems)
			{
				if (string.IsNullOrEmpty(projectItem.SourceFile) || !projectItem.SourceFile.Equals(str, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				wiXProjectItem = projectItem ?? VSDialogBase.CreateNewStandardUIFile(str, ns, project);
				return wiXProjectItem;
			}
			if (wiXProjectItem == null)
			{
				wiXProjectItem = VSDialogBase.CreateNewStandardUIFile(str, ns, project);
			}
			return wiXProjectItem;
		}

		protected string GetTextValue(WiXEntity elem, string attrName)
		{
			if (elem != null)
			{
				string attributeValue = elem.GetAttributeValue(attrName);
				if (!string.IsNullOrEmpty(attributeValue))
				{
					attributeValue = attributeValue.Trim();
					if (attributeValue.StartsWith("{\\"))
					{
						int num = attributeValue.IndexOf("}");
						if (num != -1)
						{
							attributeValue = attributeValue.Substring(num + 1);
						}
					}
					if (this.Project.ProjectManager != null && this.Project.ProjectManager.IsMultiLangSupported && attributeValue.StartsWith("!(loc."))
					{
						attributeValue = attributeValue.Substring(attributeValue.IndexOf(".") + 1);
						int num1 = attributeValue.IndexOf(")");
						if (num1 != -1)
						{
							attributeValue = attributeValue.Substring(0, num1);
						}
						WiXLocalization currentLocalization = this.Project.CurrentLocalization;
						if (currentLocalization != null && currentLocalization.Strings.ContainsKey(attributeValue))
						{
							attributeValue = currentLocalization.Strings[attributeValue];
							return attributeValue;
						}
						currentLocalization = this.Project.NeutralLocalization;
						if (currentLocalization != null && currentLocalization.Strings.ContainsKey(attributeValue))
						{
							attributeValue = currentLocalization.Strings[attributeValue];
						}
					}
					return attributeValue;
				}
			}
			return string.Empty;
		}

		private static WiXEntity GetUISequence(AddinExpress.Installer.WiXDesigner.DialogScope scope, WiXEntity parent, WiXEntity before, WiXProjectParser project)
		{
			WiXEntity wiXInstallUISequence = null;
			if (scope == AddinExpress.Installer.WiXDesigner.DialogScope.UserInstall)
			{
				wiXInstallUISequence = parent.ChildEntities.Find((WiXEntity e) => e is WiXInstallUISequence);
				if (wiXInstallUISequence == null)
				{
					XmlElement xmlElement = Common.CreateXmlElementWithAttributes(parent.XmlNode.OwnerDocument, "InstallUISequence", parent.XmlNode.NamespaceURI, new string[0], new string[0], "", false);
					if (before == null)
					{
						parent.XmlNode.AppendChild(xmlElement);
					}
					else
					{
						parent.XmlNode.InsertBefore(xmlElement, before.XmlNode);
					}
					wiXInstallUISequence = new WiXInstallUISequence(project, parent.Owner, parent, xmlElement);
					project.SupportedEntities.Add(wiXInstallUISequence);
					parent.SetDirty();
				}
			}
			else if (scope != AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
			{
			}
			else
			{
				wiXInstallUISequence = parent.ChildEntities.Find((WiXEntity e) => e is WiXAdminUISequence);
				if (wiXInstallUISequence == null)
				{
					XmlElement xmlElement1 = Common.CreateXmlElementWithAttributes(parent.XmlNode.OwnerDocument, "AdminUISequence", parent.XmlNode.NamespaceURI, new string[0], new string[0], "", false);
					if (before == null)
					{
						parent.XmlNode.AppendChild(xmlElement1);
					}
					else
					{
						parent.XmlNode.InsertBefore(xmlElement1, before.XmlNode);
					}
					wiXInstallUISequence = new WiXAdminUISequence(project, parent.Owner, parent, xmlElement1);
					project.SupportedEntities.Add(wiXInstallUISequence);
					parent.SetDirty();
				}
			}
			return wiXInstallUISequence;
		}

		internal static string GetXmlContent(string resourceId, string language, string ns, bool multiLangSupport)
		{
			string empty = string.Empty;
			try
			{
				if (!multiLangSupport)
				{
					string str = resourceId;
					str = (!ProjectUtilities.IsNeutralLCID(language) ? string.Concat(str, language) : string.Concat(str, "0"));
					empty = VSDialogBase.Resources.GetString(str);
				}
				else
				{
					empty = VSDialogBase.ResourcesMultiLang.GetString(resourceId);
				}
			}
			catch (Exception exception)
			{
			}
			if (!multiLangSupport && string.IsNullOrEmpty(empty) && !ProjectUtilities.IsNeutralLCID(language))
			{
				try
				{
					empty = VSDialogBase.Resources.GetString(string.Concat(resourceId, "0"));
				}
				catch (Exception exception1)
				{
				}
			}
			if (!string.IsNullOrEmpty(empty))
			{
				empty = empty.Replace("$namespace$", ns);
			}
			return empty;
		}

		protected virtual void InitializeDialog()
		{
			if (this.WiXDialog != null)
			{
				this._wixBannerBitmap = this.WiXDialog.ChildEntities.Find((WiXEntity c) => {
					if (!(c is WiXControl))
					{
						return false;
					}
					return c.GetAttributeValue("Id") == "BannerBmp";
				}) as WiXControl;
			}
		}

		internal bool InsertToUI(VSDialogBase prevDialog, VSDialogBase nextDialog)
		{
			string attributeValue = this.WiXDialog.GetAttributeValue("Id");
			string empty = string.Empty;
			string str = string.Empty;
			string empty1 = string.Empty;
			string str1 = string.Empty;
			string empty2 = string.Empty;
			string innerText = string.Empty;
			string attributeValue1 = string.Empty;
			string attributeValue2 = string.Empty;
			string str2 = string.Empty;
			string innerText1 = string.Empty;
			WiXEntity parent = null;
			WiXEntity wiXEntity = this.WiXDialog.Parent as WiXEntity;
			if (wiXEntity == null)
			{
				return false;
			}
			parent = wiXEntity.Parent as WiXEntity;
			if (parent == null)
			{
				return false;
			}
			if (prevDialog != null)
			{
				empty = prevDialog.WiXDialog.GetAttributeValue("Id");
				if (prevDialog.WiXShowElement != null)
				{
					prevDialog.WiXShowElement.GetAttributeValue("Sequence");
					prevDialog.WiXShowElement.GetAttributeValue("After");
					prevDialog.WiXShowElement.GetAttributeValue("Before");
					if (prevDialog.WiXShowElement.FirstChild != null)
					{
						XmlNode xmlNode = ((WiXEntity)prevDialog.WiXShowElement.FirstChild).XmlNode;
						if (xmlNode != null)
						{
							innerText = xmlNode.InnerText;
						}
					}
				}
			}
			if (nextDialog != null)
			{
				str = nextDialog.WiXDialog.GetAttributeValue("Id");
				if (nextDialog.WiXShowElement != null)
				{
					attributeValue1 = nextDialog.WiXShowElement.GetAttributeValue("Sequence");
					attributeValue2 = nextDialog.WiXShowElement.GetAttributeValue("After");
					str2 = nextDialog.WiXShowElement.GetAttributeValue("Before");
					if (nextDialog.WiXShowElement.FirstChild != null)
					{
						XmlNode xmlNodes = ((WiXEntity)nextDialog.WiXShowElement.FirstChild).XmlNode;
						if (xmlNodes != null)
						{
							innerText1 = xmlNodes.InnerText;
						}
					}
				}
			}
			if (!this.IsChained)
			{
				if (prevDialog != null && !prevDialog.IsChained)
				{
					if (prevDialog.WiXShowElement == null)
					{
						return false;
					}
					WiXEntity uISequence = VSDialogBase.GetUISequence(this.dialogScope, parent, null, this.Project);
					WiXShow wiXShow = VSDialogBase.CreateShowEntity(attributeValue, uISequence, string.Empty, empty, string.Empty, string.Empty, innerText);
					List<VSDialogBase> vSDialogBases = this._collection.FindAll((VSDialogBase e) => {
						if (e.WiXShowElement == null)
						{
							return false;
						}
						return e.WiXShowElement.GetAttributeValue("After") == empty;
					});
					if (vSDialogBases.Count > 0)
					{
						foreach (VSDialogBase vSDialogBase in vSDialogBases)
						{
							vSDialogBase.WiXShowElement.SetAttributeValue("After", attributeValue);
							vSDialogBase.WiXShowElement.SetDirty();
						}
					}
					this.WiXShowElement = wiXShow;
					return true;
				}
				if (nextDialog != null && !nextDialog.IsChained)
				{
					if (nextDialog.WiXShowElement == null)
					{
						return false;
					}
					WiXEntity uISequence1 = VSDialogBase.GetUISequence(this.dialogScope, parent, null, this.Project);
					WiXShow wiXShow1 = VSDialogBase.CreateShowEntity(attributeValue, uISequence1, attributeValue1, attributeValue2, str2, string.Empty, innerText1);
					nextDialog.WiXShowElement.SetAttributeValue("After", attributeValue);
					nextDialog.WiXShowElement.SetAttributeValue("Before", null);
					nextDialog.WiXShowElement.SetAttributeValue("Sequence", null);
					List<VSDialogBase> vSDialogBases1 = this._collection.FindAll((VSDialogBase e) => {
						if (e.WiXShowElement == null)
						{
							return false;
						}
						return e.WiXShowElement.GetAttributeValue("Before") == str;
					});
					if (vSDialogBases1.Count > 0)
					{
						foreach (VSDialogBase vSDialogBase1 in vSDialogBases1)
						{
							vSDialogBase1.WiXShowElement.SetAttributeValue("Before", attributeValue);
							vSDialogBase1.WiXShowElement.SetDirty();
						}
					}
					this.WiXShowElement = wiXShow1;
					return true;
				}
				if (prevDialog != null)
				{
					VSDialogBase firstChainedDialog = this.GetFirstChainedDialog(prevDialog);
					if (firstChainedDialog.WiXShowElement == null)
					{
						return false;
					}
					prevDialog.SetNextDialogId(null);
					if (prevDialog != firstChainedDialog)
					{
						prevDialog = firstChainedDialog;
						empty = prevDialog.WiXDialog.GetAttributeValue("Id");
						prevDialog.WiXShowElement.GetAttributeValue("Sequence");
						prevDialog.WiXShowElement.GetAttributeValue("After");
						prevDialog.WiXShowElement.GetAttributeValue("Before");
						if (prevDialog.WiXShowElement.FirstChild != null)
						{
							XmlNode xmlNode1 = ((WiXEntity)prevDialog.WiXShowElement.FirstChild).XmlNode;
							if (xmlNode1 != null)
							{
								innerText = xmlNode1.InnerText;
							}
						}
					}
					WiXEntity wiXEntity1 = VSDialogBase.GetUISequence(this.dialogScope, parent, null, this.Project);
					WiXShow wiXShow2 = VSDialogBase.CreateShowEntity(attributeValue, wiXEntity1, string.Empty, empty, string.Empty, string.Empty, innerText);
					List<VSDialogBase> vSDialogBases2 = this._collection.FindAll((VSDialogBase e) => {
						if (e.WiXShowElement == null)
						{
							return false;
						}
						return e.WiXShowElement.GetAttributeValue("After") == empty;
					});
					if (vSDialogBases2.Count > 0)
					{
						foreach (VSDialogBase vSDialogBase2 in vSDialogBases2)
						{
							vSDialogBase2.WiXShowElement.SetAttributeValue("After", attributeValue);
							vSDialogBase2.WiXShowElement.SetDirty();
						}
					}
					this.WiXShowElement = wiXShow2;
					if (nextDialog != null && nextDialog.IsChained)
					{
						nextDialog.SetPrevDialogId(null);
						wiXShow2 = VSDialogBase.CreateShowEntity(str, wiXEntity1, string.Empty, attributeValue, string.Empty, string.Empty, innerText);
						foreach (VSDialogBase vSDialogBase3 in vSDialogBases2)
						{
							vSDialogBase3.WiXShowElement.SetAttributeValue("After", str);
							vSDialogBase3.WiXShowElement.SetDirty();
						}
						nextDialog.WiXShowElement = wiXShow2;
					}
					return true;
				}
				if (nextDialog != null)
				{
					if (nextDialog.WiXShowElement == null)
					{
						return false;
					}
					WiXEntity uISequence2 = VSDialogBase.GetUISequence(this.dialogScope, parent, null, this.Project);
					WiXShow wiXShow3 = VSDialogBase.CreateShowEntity(attributeValue, uISequence2, attributeValue1, attributeValue2, str2, string.Empty, innerText1);
					nextDialog.WiXShowElement.SetAttributeValue("After", attributeValue);
					nextDialog.WiXShowElement.SetAttributeValue("Before", null);
					nextDialog.WiXShowElement.SetAttributeValue("Sequence", null);
					this.WiXShowElement = wiXShow3;
					return true;
				}
			}
			else
			{
				if (prevDialog != null && prevDialog.IsChained)
				{
					if (prevDialog.WiXNextArgsProperty != null)
					{
						string attributeValue3 = prevDialog.WiXNextArgsProperty.GetAttributeValue("Value");
						if (!string.IsNullOrEmpty(attributeValue3))
						{
							this.SetNextDialogId(attributeValue3);
							nextDialog = this._collection.Find((VSDialogBase d) => d.WiXDialog.GetAttributeValue("Id") == attributeValue3);
							if (nextDialog != null)
							{
								nextDialog.SetPrevDialogId(attributeValue);
							}
						}
					}
					prevDialog.SetNextDialogId(attributeValue);
					this.SetPrevDialogId(empty);
					return true;
				}
				if (nextDialog != null && nextDialog.IsChained)
				{
					if (nextDialog.WiXShowElement == null)
					{
						return false;
					}
					WiXEntity parent1 = nextDialog.WiXShowElement.Parent as WiXEntity;
					this.Project.SupportedEntities.Remove(nextDialog.WiXShowElement);
					nextDialog.WiXShowElement.Delete();
					nextDialog.WiXShowElement = null;
					if (parent1 != null)
					{
						parent1.SetDirty();
					}
					nextDialog.SetPrevDialogId(attributeValue);
					WiXEntity wiXEntity2 = VSDialogBase.GetUISequence(this.dialogScope, parent, null, this.Project);
					this.WiXShowElement = VSDialogBase.CreateShowEntity(attributeValue, wiXEntity2, attributeValue1, attributeValue2, str2, string.Empty, innerText1);
					this.SetNextDialogId(str);
					List<VSDialogBase> vSDialogBases3 = this._collection.FindAll((VSDialogBase e) => {
						if (e.WiXShowElement == null)
						{
							return false;
						}
						if (e.WiXShowElement.GetAttributeValue("After") == str)
						{
							return true;
						}
						return e.WiXShowElement.GetAttributeValue("Before") == str;
					});
					if (vSDialogBases3.Count > 0)
					{
						foreach (VSDialogBase vSDialogBase4 in vSDialogBases3)
						{
							if (string.IsNullOrEmpty(vSDialogBase4.WiXShowElement.GetAttributeValue("After")))
							{
								if (string.IsNullOrEmpty(vSDialogBase4.WiXShowElement.GetAttributeValue("Before")))
								{
									continue;
								}
								vSDialogBase4.WiXShowElement.SetAttributeValue("Before", attributeValue);
								vSDialogBase4.WiXShowElement.SetDirty();
							}
							else
							{
								vSDialogBase4.WiXShowElement.SetAttributeValue("After", attributeValue);
								vSDialogBase4.WiXShowElement.SetDirty();
							}
						}
					}
					return true;
				}
				if (prevDialog != null)
				{
					if (prevDialog.WiXShowElement == null)
					{
						return false;
					}
					WiXShow wiXShow4 = null;
					WiXEntity uISequence3 = VSDialogBase.GetUISequence(this.dialogScope, parent, null, this.Project);
					List<VSDialogBase> vSDialogBases4 = this._collection.FindAll((VSDialogBase e) => {
						if (e.WiXShowElement == null)
						{
							return false;
						}
						return e.WiXShowElement.GetAttributeValue("After") == empty;
					});
					if (vSDialogBases4.Count > 0)
					{
						foreach (VSDialogBase vSDialogBase5 in vSDialogBases4)
						{
							vSDialogBase5.WiXShowElement.SetAttributeValue("After", attributeValue);
							vSDialogBase5.WiXShowElement.SetDirty();
						}
					}
					wiXShow4 = VSDialogBase.CreateShowEntity(this.WiXDialog.Id, uISequence3, string.Empty, empty, string.Empty, string.Empty, innerText);
					this.WiXShowElement = wiXShow4;
					return true;
				}
				if (nextDialog != null)
				{
					if (nextDialog.WiXShowElement == null)
					{
						return false;
					}
					WiXShow wiXShow5 = null;
					WiXEntity wiXEntity3 = VSDialogBase.GetUISequence(this.dialogScope, parent, null, this.Project);
					wiXShow5 = VSDialogBase.CreateShowEntity(this.WiXDialog.Id, wiXEntity3, attributeValue1, attributeValue2, str2, string.Empty, innerText1);
					nextDialog.WiXShowElement.SetAttributeValue("After", attributeValue);
					nextDialog.WiXShowElement.SetAttributeValue("Before", null);
					nextDialog.WiXShowElement.SetAttributeValue("Sequence", null);
					nextDialog.WiXShowElement.SetDirty();
					this.WiXShowElement = wiXShow5;
					return true;
				}
			}
			return false;
		}

		internal static bool IsBasicDialog(AddinExpress.Installer.WiXDesigner.WiXDialog dialog)
		{
			string attributeValue = dialog.GetAttributeValue("Id");
			if (!(attributeValue == "Cancel") && !(attributeValue == "ConfirmRemoveDialog") && !(attributeValue == "DiskCost") && !(attributeValue == "FilesInUse") && !(attributeValue == "SelectFolderDialog") && !(attributeValue == "ErrorDialog") && !(attributeValue == "UserExitForm") && !(attributeValue == "FatalErrorForm") && !(attributeValue == "MaintenanceForm") && !(attributeValue == "ResumeForm") && !(attributeValue == "AdminUserExitForm") && !(attributeValue == "AdminFatalErrorForm") && !(attributeValue == "AdminMaintenanceForm") && !(attributeValue == "AdminResumeForm"))
			{
				return false;
			}
			return true;
		}

		private static bool IsExistsInSequence(string dialogId, List<WiXEntity> sequenceItems, out WiXEntity elem)
		{
			bool flag;
			elem = null;
			List<WiXEntity>.Enumerator enumerator = sequenceItems.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					WiXEntity current = enumerator.Current;
					if (!(current is WiXShow) && !(current is WiXCustom) || !(dialogId == current.GetAttributeValue("Dialog")))
					{
						continue;
					}
					elem = current;
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		public static bool IsLCIDSupported(string language)
		{
			// 
			// Current member / type: System.Boolean AddinExpress.Installer.WiXDesigner.VSDialogBase::IsLCIDSupported(System.String)
			// File path: C:\Program Files (x86)\Add-in Express\Designer for Visual Studio WiX Setup Projects\Bin\VS2019\AddinExpress.WiXDesignerPkg.v16.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Boolean IsLCIDSupported(System.String)
			// 
			// Object reference not set to an instance of an object.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\DTree\BaseDominatorTreeBuilder.cs:line 112
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\DTree\BaseDominatorTreeBuilder.cs:line 26
			//    at ..BuildTree( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\DTree\FastDominatorTreeBuilder.cs:line 25
			//    at ..(ILogicalConstruct ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\DominatorTreeDependentStep.cs:line 18
			//    at ..(ILogicalConstruct ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\Loops\LoopBuilder.cs:line 68
			//    at ..(ILogicalConstruct ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\Loops\LoopBuilder.cs:line 59
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\LogicalFlowBuilderStep.cs:line 128
			//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\LogicFlow\LogicalFlowBuilderStep.cs:line 51
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		private void ReconnectAfter(VSDialogBase dialog, ref VSDialogBase refDialog)
		{
			bool flag = false;
			string attributeValue = refDialog.WiXDialog.GetAttributeValue("Id");
			if (refDialog.IsChained && dialog.IsChained)
			{
				string str = dialog.WiXDialog.GetAttributeValue("Id");
				if (this._collection.FindAll((VSDialogBase e) => {
					if (e.WiXShowElement == null)
					{
						return false;
					}
					return e.WiXShowElement.GetAttributeValue("Before") == str;
				}).Count == 0)
				{
					VSDialogBase lastChainedDialog = this.GetLastChainedDialog(refDialog);
					if (lastChainedDialog != null)
					{
						lastChainedDialog.SetNextDialogId(str);
						dialog.SetPrevDialogId(lastChainedDialog.WiXDialog.GetAttributeValue("Id"));
						WiXEntity parent = dialog.WiXShowElement.Parent as WiXEntity;
						this.Project.SupportedEntities.Remove(dialog.WiXShowElement);
						dialog.WiXShowElement.Delete();
						dialog.WiXShowElement = null;
						if (parent != null)
						{
							parent.SetDirty();
						}
						List<VSDialogBase> vSDialogBases = this._collection.FindAll((VSDialogBase e) => {
							if (e.WiXShowElement == null)
							{
								return false;
							}
							return e.WiXShowElement.GetAttributeValue("After") == str;
						});
						if (vSDialogBases.Count > 0)
						{
							foreach (VSDialogBase vSDialogBase in vSDialogBases)
							{
								vSDialogBase.WiXShowElement.SetAttributeValue("After", attributeValue);
								vSDialogBase.WiXShowElement.SetDirty();
							}
						}
						flag = true;
					}
				}
			}
			if (!flag)
			{
				dialog.WiXShowElement.SetAttributeValue("After", attributeValue);
				dialog.WiXShowElement.SetAttributeValue("Sequence", null);
				dialog.WiXShowElement.SetAttributeValue("Before", null);
				dialog.WiXShowElement.SetDirty();
			}
			refDialog = dialog;
		}

		private void ReconnectBefore(VSDialogBase dialog, ref VSDialogBase refDialog)
		{
			bool flag = false;
			string attributeValue = refDialog.WiXDialog.GetAttributeValue("Id");
			if (refDialog.IsChained && dialog.IsChained)
			{
				string str = dialog.WiXDialog.GetAttributeValue("Id");
				if (this._collection.FindAll((VSDialogBase e) => {
					if (e.WiXShowElement == null)
					{
						return false;
					}
					return e.WiXShowElement.GetAttributeValue("After") == str;
				}).Count == 0)
				{
					VSDialogBase firstChainedDialog = this.GetFirstChainedDialog(refDialog);
					if (firstChainedDialog != null)
					{
						WiXEntity parent = dialog.WiXShowElement.Parent as WiXEntity;
						this.Project.SupportedEntities.Remove(dialog.WiXShowElement);
						dialog.WiXShowElement.Delete();
						dialog.WiXShowElement = firstChainedDialog.WiXShowElement;
						dialog.WiXShowElement.SetAttributeValue("Dialog", dialog.WiXDialog.GetAttributeValue("Id"));
						if (parent != null)
						{
							parent.SetDirty();
						}
						VSDialogBase lastChainedDialog = this.GetLastChainedDialog(dialog);
						lastChainedDialog.SetNextDialogId(firstChainedDialog.WiXDialog.GetAttributeValue("Id"));
						firstChainedDialog.WiXShowElement = null;
						firstChainedDialog.SetPrevDialogId(lastChainedDialog.WiXDialog.GetAttributeValue("Id"));
						flag = true;
					}
				}
			}
			if (!flag)
			{
				dialog.WiXShowElement.SetAttributeValue("Before", attributeValue);
				dialog.WiXShowElement.SetAttributeValue("Sequence", null);
				dialog.WiXShowElement.SetAttributeValue("After", null);
				dialog.WiXShowElement.SetDirty();
			}
			refDialog = dialog;
		}

		internal static void RegisterUIReference(string id, WiXEntity rootSetupEntity)
		{
			if (rootSetupEntity.Project.SupportedEntities.Find((WiXEntity e) => {
				if (!(e is WiXUIRef))
				{
					return false;
				}
				return e.GetAttributeValue("Id") == id;
			}) == null)
			{
				XmlElement xmlElement = Common.CreateXmlElementWithAttributes(rootSetupEntity.XmlNode.OwnerDocument, "UIRef", rootSetupEntity.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { id }, "", false);
				WiXEntity wiXEntity = rootSetupEntity.ChildEntities.Find((WiXEntity e) => e is WiXUI);
				if (wiXEntity == null)
				{
					rootSetupEntity.XmlNode.AppendChild(xmlElement);
				}
				else
				{
					rootSetupEntity.XmlNode.InsertBefore(xmlElement, wiXEntity.XmlNode);
				}
				WiXEntity wiXUIRef = new WiXUIRef(rootSetupEntity.Project, rootSetupEntity.Owner, rootSetupEntity, xmlElement);
				rootSetupEntity.Project.SupportedEntities.Add(wiXUIRef);
				rootSetupEntity.SetDirty();
			}
		}

		private static void SaveBinaryFile(string filePath, byte[] data)
		{
			using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
			{
				fileStream.Write(data, 0, (int)data.Length);
				fileStream.Flush();
			}
		}

		private static void SaveTextFile(string filePath, string text, bool forceUTF8)
		{
			using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
			{
				using (StreamWriter streamWriter = new StreamWriter(fileStream, (forceUTF8 ? Encoding.UTF8 : Encoding.Default)))
				{
					streamWriter.Write(text);
					streamWriter.Flush();
				}
			}
		}

		internal bool SetBannerBitmapValue(VSBaseFile value)
		{
			if (this._wixBannerBitmap != null)
			{
				string attributeValue = this._wixBannerBitmap.GetAttributeValue("Text");
				if (value == null || value == VSBaseFile.Empty)
				{
					if (!string.IsNullOrEmpty(attributeValue))
					{
						this._wixBannerBitmap.SetAttributeValue("Text", null);
						this._wixBannerBitmap.SetAttributeValue("Type", "Text");
						this._wixBannerBitmap.SetDirty();
						return true;
					}
				}
				else if (value is VSBinary)
				{
					string id = (value as VSBinary).WiXElement.Id;
					if (id != attributeValue)
					{
						this._wixBannerBitmap.SetAttributeValue("Text", id);
						this._wixBannerBitmap.SetAttributeValue("Type", "Bitmap");
						this._wixBannerBitmap.SetDirty();
						return true;
					}
				}
				else if (value is VSFile)
				{
					VSBinary vSBinary = null;
					string empty = string.Empty;
					string str = Common.GenerateBinaryId(this.Project.ProjectType);
					string str1 = (value as VSFile).SourcePath;
					if (this.Project.ProjectManager.VsProject != null)
					{
						empty = Path.GetDirectoryName(this.Project.ProjectManager.VsProject.FullName);
					}
					if (!string.IsNullOrEmpty(empty))
					{
						vSBinary = this.Project.Binaries.Find((VSBinary b) => {
							string sourcePath = b.SourcePath;
							if (string.IsNullOrEmpty(sourcePath))
							{
								return false;
							}
							if (!Path.IsPathRooted(sourcePath))
							{
								sourcePath = Path.GetFullPath(Path.Combine(empty, sourcePath));
							}
							return sourcePath.Equals(str1, StringComparison.OrdinalIgnoreCase);
						});
						if (vSBinary != null)
						{
							str = vSBinary.WiXElement.Id;
						}
					}
					if (str != attributeValue)
					{
						if (vSBinary == null)
						{
							string str2 = (value as VSFile).SourcePath;
							if (Path.IsPathRooted(str2) && !string.IsNullOrEmpty(empty))
							{
								str2 = Path.Combine(CommonUtilities.RelativizePathIfPossible(Path.GetDirectoryName(str2), empty), Path.GetFileName(str2));
							}
							WiXEntity wiXEntity = this.Project.SupportedEntities.Find((WiXEntity b) => {
								if (!(b is WiXUI))
								{
									return false;
								}
								return b.GetAttributeValue("Id") == "UserInterface";
							}) ?? this._wixElement.Parent as WiXEntity;
							XmlElement xmlElement = Common.CreateXmlElementWithAttributes(wiXEntity.XmlNode.OwnerDocument, "Binary", wiXEntity.XmlNode.NamespaceURI, new string[] { "Id", "SourceFile" }, new string[] { str, str2 }, "", false);
							if (!wiXEntity.HasChildEntities)
							{
								wiXEntity.XmlNode.AppendChild(xmlElement);
							}
							else
							{
								wiXEntity.XmlNode.InsertBefore(xmlElement, wiXEntity.XmlNode.FirstChild);
							}
							this.Project.Binaries.Add(new VSBinary(this.Project, new WiXBinary(this.Project, wiXEntity.Owner, wiXEntity, xmlElement)));
							wiXEntity.SetDirty();
						}
						this._wixBannerBitmap.SetAttributeValue("Text", str);
						this._wixBannerBitmap.SetAttributeValue("Type", "Bitmap");
						this._wixBannerBitmap.SetDirty();
						return true;
					}
				}
			}
			return false;
		}

		internal void SetNextDialogId(string dialogId)
		{
			if (!string.IsNullOrEmpty(dialogId))
			{
				if (this.WiXNextArgsProperty != null)
				{
					this.WiXNextArgsProperty.SetAttributeValue("Value", dialogId);
					this.WiXNextArgsProperty.SetDirty();
					return;
				}
				if (!string.IsNullOrEmpty(this.nextArgsPropId))
				{
					XmlElement xmlElement = Common.CreateXmlElementWithAttributes(this.WiXDialog.XmlNode.OwnerDocument, "Property", this.WiXDialog.XmlNode.NamespaceURI, new string[] { "Id", "Value" }, new string[] { this.nextArgsPropId, dialogId }, "", false);
					WiXUI parent = this.WiXDialog.Parent as WiXUI;
					if (parent != null && parent.Parent != null)
					{
						(parent.Parent as WiXEntity).XmlNode.InsertBefore(xmlElement, parent.XmlNode);
						this._wixNextArgsPropElement = new WiXProperty(this.Project, this.WiXDialog.Owner, parent.Parent, xmlElement);
						this.Project.SupportedEntities.Add(this._wixNextArgsPropElement);
						(parent.Parent as WiXEntity).SetDirty();
					}
				}
			}
			else if (this.WiXNextArgsProperty != null)
			{
				WiXEntity wiXEntity = this.WiXNextArgsProperty.Parent as WiXEntity;
				this.Project.SupportedEntities.Remove(this.WiXNextArgsProperty);
				this.WiXNextArgsProperty.Delete();
				this._wixNextArgsPropElement = null;
				if (wiXEntity != null)
				{
					wiXEntity.SetDirty();
					return;
				}
			}
		}

		internal void SetPrevDialogId(string dialogId)
		{
			if (!string.IsNullOrEmpty(dialogId))
			{
				if (this.WiXPrevArgsProperty != null)
				{
					this.WiXPrevArgsProperty.SetAttributeValue("Value", dialogId);
					this.WiXPrevArgsProperty.SetDirty();
					return;
				}
				if (!string.IsNullOrEmpty(this.prevArgsPropId))
				{
					XmlElement xmlElement = Common.CreateXmlElementWithAttributes(this.WiXDialog.XmlNode.OwnerDocument, "Property", this.WiXDialog.XmlNode.NamespaceURI, new string[] { "Id", "Value" }, new string[] { this.prevArgsPropId, dialogId }, "", false);
					WiXUI parent = this.WiXDialog.Parent as WiXUI;
					if (parent != null && parent.Parent != null)
					{
						(parent.Parent as WiXEntity).XmlNode.InsertBefore(xmlElement, parent.XmlNode);
						this._wixPrevArgsPropElement = new WiXProperty(this.Project, this.WiXDialog.Owner, parent.Parent, xmlElement);
						this.Project.SupportedEntities.Add(this._wixPrevArgsPropElement);
						(parent.Parent as WiXEntity).SetDirty();
					}
				}
			}
			else if (this.WiXPrevArgsProperty != null)
			{
				WiXEntity wiXEntity = this.WiXPrevArgsProperty.Parent as WiXEntity;
				this.Project.SupportedEntities.Remove(this.WiXPrevArgsProperty);
				this.WiXPrevArgsProperty.Delete();
				this._wixPrevArgsPropElement = null;
				if (wiXEntity != null)
				{
					wiXEntity.SetDirty();
					return;
				}
			}
		}

		protected bool SetTextValue(WiXEntity elem, string attrName, string text)
		{
			if (elem != null)
			{
				string attributeValue = elem.GetAttributeValue(attrName) ?? string.Empty;
				string str = attributeValue.Trim();
				string empty = string.Empty;
				if (str.StartsWith("{\\"))
				{
					int num = str.IndexOf("}");
					if (num != -1)
					{
						empty = str.Substring(0, num + 1);
					}
				}
				str = (string.IsNullOrEmpty(empty) ? text : string.Concat(empty, text));
				if (attributeValue != str)
				{
					bool flag = true;
					if ((this.Project.ProjectManager == null ? false : this.Project.ProjectManager.IsMultiLangSupported))
					{
						string attributeValue1 = elem.GetAttributeValue(attrName);
						if (!string.IsNullOrEmpty(attributeValue1))
						{
							attributeValue1 = attributeValue1.Trim();
							if (attributeValue1.StartsWith("{\\"))
							{
								int num1 = attributeValue1.IndexOf("}");
								if (num1 != -1)
								{
									attributeValue1 = attributeValue1.Substring(num1 + 1);
								}
							}
							if (attributeValue1.StartsWith("!(loc."))
							{
								flag = false;
								attributeValue1 = attributeValue1.Substring(attributeValue1.IndexOf(".") + 1);
								int num2 = attributeValue1.IndexOf(")");
								if (num2 != -1)
								{
									attributeValue1 = attributeValue1.Substring(0, num2);
								}
								if (!string.IsNullOrEmpty(attributeValue1))
								{
									WiXLocalization currentLocalization = this.Project.CurrentLocalization;
									if (currentLocalization == null)
									{
										currentLocalization = this.Project.NeutralLocalization;
										if (currentLocalization != null)
										{
											currentLocalization.SetStringValue(attributeValue1, text);
										}
									}
									else
									{
										currentLocalization.SetStringValue(attributeValue1, text);
									}
								}
							}
						}
					}
					if (flag)
					{
						elem.SetAttributeValue(attrName, str);
						elem.SetDirty();
					}
					return true;
				}
			}
			return false;
		}

		protected bool SetTextValue(WiXEntity elem, string attrName, string text, bool allowEmptyValue)
		{
			if (elem != null)
			{
				string attributeValue = elem.GetAttributeValue(attrName) ?? string.Empty;
				string str = attributeValue.Trim();
				string empty = string.Empty;
				if (str.StartsWith("{\\"))
				{
					int num = str.IndexOf("}");
					if (num != -1)
					{
						empty = str.Substring(0, num + 1);
					}
				}
				str = (string.IsNullOrEmpty(empty) ? text : string.Concat(empty, text));
				if (attributeValue != str)
				{
					bool flag = false;
					if ((this.Project.ProjectManager == null ? false : this.Project.ProjectManager.IsMultiLangSupported))
					{
						string attributeValue1 = elem.GetAttributeValue(attrName);
						if (!string.IsNullOrEmpty(attributeValue1))
						{
							attributeValue1 = attributeValue1.Trim();
							if (attributeValue1.StartsWith("{\\"))
							{
								int num1 = attributeValue1.IndexOf("}");
								if (num1 != -1)
								{
									attributeValue1 = attributeValue1.Substring(num1 + 1);
								}
							}
							if (attributeValue1.StartsWith("!(loc."))
							{
								flag = true;
								attributeValue1 = attributeValue1.Substring(attributeValue1.IndexOf(".") + 1);
								int num2 = attributeValue1.IndexOf(")");
								if (num2 != -1)
								{
									attributeValue1 = attributeValue1.Substring(0, num2);
								}
							}
							if (!string.IsNullOrEmpty(attributeValue1))
							{
								WiXLocalization currentLocalization = this.Project.CurrentLocalization;
								if (currentLocalization == null)
								{
									currentLocalization = this.Project.NeutralLocalization;
									if (currentLocalization != null)
									{
										currentLocalization.SetStringValue(attributeValue1, text);
									}
								}
								else
								{
									currentLocalization.SetStringValue(attributeValue1, text);
								}
							}
						}
					}
					if (!flag)
					{
						elem.SetAttributeValue(attrName, str, allowEmptyValue);
						elem.SetDirty();
					}
					return true;
				}
			}
			return false;
		}

		private void SortDialogStage()
		{
			string attributeValue;
			string str;
			string attributeValue1;
			List<VSDialogBase> userInstall = null;
			List<int> nums = new List<int>();
			if (this.DialogScope == AddinExpress.Installer.WiXDesigner.DialogScope.AdministrativeInstall)
			{
				userInstall = this.Project.UserInterface.UserInstall;
			}
			else if (this.DialogScope == AddinExpress.Installer.WiXDesigner.DialogScope.UserInstall)
			{
				userInstall = this.Project.UserInterface.UserInstall;
			}
			if (userInstall.Count == 0)
			{
				return;
			}
			if (userInstall != null && userInstall.Count > 0)
			{
				int num = 0;
				for (int i = 0; i < userInstall.Count; i++)
				{
					VSDialogBase item = userInstall[i];
					if (item.WiXShowElement != null && item.DialogScope == this.DialogScope && item.DialogStage == this.DialogStage)
					{
						attributeValue = item.WiXShowElement.GetAttributeValue("Sequence");
						str = item.WiXShowElement.GetAttributeValue("After");
						attributeValue1 = item.WiXShowElement.GetAttributeValue("Before");
						if (!string.IsNullOrEmpty(attributeValue))
						{
							nums.Add(i);
						}
						else if (!string.IsNullOrEmpty(str))
						{
							if (VSDialogBase.GetStandardActionStage(str, string.Empty, false, ref num) != AddinExpress.Installer.WiXDesigner.DialogStage.Undefined)
							{
								nums.Add(i);
							}
						}
						else if (!string.IsNullOrEmpty(attributeValue1) && VSDialogBase.GetStandardActionStage(attributeValue1, string.Empty, false, ref num) != AddinExpress.Installer.WiXDesigner.DialogStage.Undefined)
						{
							nums.Add(i);
						}
					}
				}
			}
			int num1 = 0;
			WiXEntity wiXShowElement = null;
			for (int j = 0; j < userInstall.Count; j++)
			{
				VSDialogBase vSDialogBase = userInstall[j];
				if (vSDialogBase.DialogScope == this.DialogScope && vSDialogBase.DialogStage == this.DialogStage)
				{
					attributeValue = null;
					str = null;
					attributeValue1 = null;
					string innerText = null;
					if (vSDialogBase.WiXShowElement != null && string.IsNullOrEmpty(vSDialogBase.WiXShowElement.GetAttributeValue("OnExit")))
					{
						bool flag = false;
						if (nums.Count == 0)
						{
							if (wiXShowElement == null)
							{
								flag = true;
								wiXShowElement = userInstall[0].WiXShowElement;
							}
						}
						else if (j <= nums[num1])
						{
							if (wiXShowElement == null)
							{
								flag = true;
								wiXShowElement = userInstall[nums[num1]].WiXShowElement;
							}
						}
						else if (num1 < nums.Count - 1)
						{
							flag = true;
							num1++;
							wiXShowElement = userInstall[nums[num1]].WiXShowElement;
						}
						if (vSDialogBase.WiXShowElement != wiXShowElement)
						{
							WiXEntity parent = vSDialogBase.WiXShowElement.Parent as WiXEntity;
							string str1 = wiXShowElement.GetAttributeValue("Dialog");
							attributeValue = wiXShowElement.GetAttributeValue("Sequence");
							str = wiXShowElement.GetAttributeValue("After");
							attributeValue1 = wiXShowElement.GetAttributeValue("Before");
							if (vSDialogBase.WiXShowElement.FirstChild != null)
							{
								XmlNode xmlNode = ((WiXEntity)vSDialogBase.WiXShowElement.FirstChild).XmlNode;
								if (xmlNode != null)
								{
									innerText = xmlNode.InnerText;
								}
							}
							this.Project.SupportedEntities.Remove(vSDialogBase.WiXShowElement);
							vSDialogBase.WiXShowElement.Delete();
							wiXShowElement = (!flag ? VSDialogBase.CreateShowEntity(vSDialogBase.WiXDialog.Id, parent, string.Empty, str1, string.Empty, string.Empty, innerText) : VSDialogBase.CreateShowEntity(vSDialogBase.WiXDialog.Id, parent, attributeValue, str, attributeValue1, string.Empty, innerText));
							vSDialogBase.WiXShowElement = wiXShowElement;
						}
					}
				}
			}
		}
	}
}