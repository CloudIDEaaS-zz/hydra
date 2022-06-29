using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSSpecialFolder : VSBaseFolder
	{
		internal static string[] specialIDs;

		internal static string[] specialNames;

		internal override bool CanDelete
		{
			get
			{
				if (this.Property == SpecialFolderType.MergeRedirectFolder.ToString())
				{
					return false;
				}
				return true;
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
		public override string Name
		{
			get
			{
				if (string.IsNullOrEmpty(base.Name))
				{
					for (int i = 0; i < (int)VSSpecialFolder.specialIDs.Length; i++)
					{
						if (VSSpecialFolder.specialIDs[i] == this.Property)
						{
							return VSSpecialFolder.specialNames[i];
						}
					}
					if (!string.IsNullOrEmpty(this.Property) && this.Property.StartsWith("$(var.") && this._project.CustomVariables != null && this._project.CustomVariables.Count > 0)
					{
						WiXCustomVariable wiXCustomVariable = this._project.CustomVariables.FindVariable(this.Property);
						if (wiXCustomVariable != null)
						{
							for (int j = 0; j < (int)VSSpecialFolder.specialIDs.Length; j++)
							{
								if (VSSpecialFolder.specialIDs[j] == wiXCustomVariable.Value)
								{
									return VSSpecialFolder.specialNames[j];
								}
							}
						}
					}
				}
				return base.Name;
			}
		}

		[Browsable(true)]
		public override string Property
		{
			get
			{
				return base.Property;
			}
		}

		static VSSpecialFolder()
		{
			VSSpecialFolder.specialIDs = new string[] { "AdminToolsFolder", "AppDataFolder", "CommonAppDataFolder", "CommonFiles64Folder", "CommonFilesFolder", "DesktopFolder", "FavoritesFolder", "FontsFolder", "LocalAppDataFolder", "MyPicturesFolder", "NetHoodFolder", "PersonalFolder", "PrintHoodFolder", "ProgramFiles64Folder", "ProgramFilesFolder", "ProgramMenuFolder", "RecentFolder", "SendToFolder", "StartMenuFolder", "StartupFolder", "System16Folder", "System64Folder", "SystemFolder", "TempFolder", "TemplateFolder", "WindowsFolder", "GAC", "MergeRedirectFolder" };
			VSSpecialFolder.specialNames = new string[] { "Administrative Tools Folder", "User's Application Data Folder", "Common Application Data Folder", "Common Files (64-bit) Folder", "Common Files Folder", "User's Desktop", "User's Favorites Folder", "Fonts Folder", "User's Application Data Folder (Local)", "MyPictures Folder", "NetHood Folder", "User's Personal Data Folder", "PrintHood Folder", "Program Files (64-bit) Folder", "Program Files Folder", "User's Programs Menu", "Recent Folder", "User's Send To Menu", "User's Start Menu", "User's Startup Folder", "System (16-bit) Folder", "System (64-bit) Folder", "System Folder", "Temporary Folder", "User's Template Folder", "Windows Folder", "Global Assembly Cache Folder", "Module Retargetable Folder" };
		}

		public VSSpecialFolder(WiXProjectParser project, VSBaseFolder parent, WiXDirectory wixElement) : base(project, parent, wixElement)
		{
		}

		internal static bool CheckFor64BitFolder(VSBaseFolder folder)
		{
			if (folder.Property == SpecialFolderType.CommonFiles64Folder.ToString() || folder.Property == SpecialFolderType.ProgramFiles64Folder.ToString() || folder.Property == SpecialFolderType.System64Folder.ToString())
			{
				return true;
			}
			if (folder.Parent == null)
			{
				return false;
			}
			return VSSpecialFolder.CheckFor64BitFolder(folder.Parent);
		}

		internal static bool CheckForSpecialID(string id, WiXCustomVariables customVariables)
		{
			string[] strArrays = VSSpecialFolder.specialIDs;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				if (strArrays[i] == id)
				{
					return true;
				}
			}
			if (customVariables != null && customVariables.Count > 0 && !string.IsNullOrEmpty(id) && id.StartsWith("$(var.") && customVariables.FindVariable(id) != null)
			{
				return true;
			}
			return false;
		}

		internal static bool CheckForUserFolder(VSBaseFolder folder)
		{
			if (folder.Property == SpecialFolderType.AppDataFolder.ToString() || folder.Property == SpecialFolderType.DesktopFolder.ToString() || folder.Property == SpecialFolderType.FavoritesFolder.ToString() || folder.Property == SpecialFolderType.PersonalFolder.ToString() || folder.Property == SpecialFolderType.ProgramMenuFolder.ToString() || folder.Property == SpecialFolderType.SendToFolder.ToString() || folder.Property == SpecialFolderType.StartMenuFolder.ToString() || folder.Property == SpecialFolderType.StartupFolder.ToString() || folder.Property == SpecialFolderType.TemplateFolder.ToString() || folder.Property == SpecialFolderType.LocalAppDataFolder.ToString())
			{
				return true;
			}
			if (folder.Parent == null)
			{
				return false;
			}
			return VSSpecialFolder.CheckForUserFolder(folder.Parent);
		}

		internal static VSSpecialFolder Create(WiXProjectParser project, VSBaseFolder parent, WiXDirectory wixElement)
		{
			if (wixElement.Id == "GAC")
			{
				return new VSGACFolder(project, parent, wixElement);
			}
			return new VSSpecialFolder(project, parent, wixElement);
		}
	}
}