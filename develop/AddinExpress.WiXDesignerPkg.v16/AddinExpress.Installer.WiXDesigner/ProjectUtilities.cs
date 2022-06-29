using EnvDTE;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class ProjectUtilities
	{
		private static System.IServiceProvider _serviceProvider;

		public const string LANG_NEUTRAL_DEFAULT_LCID = "0";

		public const string LANG_NEUTRAL_LCID_3072 = "3072";

		public const string LANG_NEUTRAL_MUI_LCID = "5120";

		public const string LANG_NEUTRAL_CUSTOM_LCID = "4096";

		public static IEnumerable<IVsProject> LoadedProjects
		{
			get
			{
				uint num = 0;
				Guid empty = Guid.Empty;
				IEnumHierarchies enumHierarchy = null;
				IVsHierarchy[] vsHierarchyArray = new IVsHierarchy[1];
				(ProjectUtilities._serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution).GetProjectEnum(1, ref empty, out enumHierarchy);
				enumHierarchy.Reset();
				while (enumHierarchy.Next(1, vsHierarchyArray, out num) == 0 && num == 1)
				{
					yield return (IVsProject)vsHierarchyArray[0];
				}
			}
		}

		public static System.IServiceProvider ServiceProvider
		{
			get
			{
				return ProjectUtilities._serviceProvider;
			}
			set
			{
				ProjectUtilities._serviceProvider = value;
			}
		}

		public ProjectUtilities()
		{
		}

		public static Dictionary<uint, string> AllItemsInProject(IVsProject project, bool checkIntermediateDir)
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}
			Dictionary<uint, string> nums = new Dictionary<uint, string>();
			string directoryName = Path.GetDirectoryName(ProjectUtilities.GetProjectFilePath(project));
			string str1 = Path.Combine(directoryName, "obj");
			ProjectUtilities.ChildrenOf(project as IVsHierarchy, -2).ForEach((uint id) => {
				string str = null;
				project.GetMkDocument(id, out str);
				if (str != null && str.Length > 0 && !Path.IsPathRooted(str))
				{
					str = CommonUtilities.AbsolutePathFromRelative(str, directoryName);
				}
				if (File.Exists(str) && CommonUtilities.IsFileExtensionSupported(str) && (!checkIntermediateDir || !str.StartsWith(str1, StringComparison.InvariantCultureIgnoreCase)))
				{
					nums.Add(id, str);
				}
			});
			return nums;
		}

		private static List<uint> ChildrenOf(IVsHierarchy hierarchy, uint rootID)
		{
			List<uint> nums = new List<uint>();
			for (uint i = ProjectUtilities.FirstChild(hierarchy, rootID); i != -1; i = ProjectUtilities.NextSibling(hierarchy, i))
			{
				nums.Add(i);
				nums.AddRange(ProjectUtilities.ChildrenOf(hierarchy, i));
			}
			return nums;
		}

		public static bool CreateTextBuffer(IVsProject vsProject, string filePath, uint id, out IVsUIHierarchy hierarchy, out uint itemid, out uint docCookie, out IVsTextLines textLines, out IVsWindowFrame docView)
		{
			object obj;
			object obj1;
			hierarchy = null;
			docCookie = 0;
			itemid = id;
			textLines = null;
			docView = null;
			IntPtr intPtr = new IntPtr(-1);
			Guid lOGVIEWIDTextView = VSConstants.LOGVIEWID_TextView;
			if (!string.IsNullOrEmpty(filePath))
			{
				intPtr = IntPtr.Zero;
				VsShellUtilities.IsDocumentOpen(ProjectUtilities._serviceProvider, filePath, lOGVIEWIDTextView, ref hierarchy, ref itemid, ref docView);
			}
			if (docView == null && vsProject.OpenItem(id, ref lOGVIEWIDTextView, intPtr, out docView) == 0 && (vsProject as IVsHierarchy).GetProperty(id, -2027, out obj) == 0)
			{
				hierarchy = (obj as EnvDTE.ProjectItem).Object as IVsUIHierarchy;
			}
			if (docView != null)
			{
				if (docView.GetProperty(-4004, out obj1) == 0)
				{
					textLines = obj1 as IVsTextLines;
				}
				if (docView.GetProperty(-4000, out obj1) == 0)
				{
					docCookie = (uint)obj1;
				}
			}
			return textLines != null;
		}

		private static UIHierarchyItem FindHierarchyItemByPath(UIHierarchyItems items, string[] paths, int index, Dictionary<UIHierarchyItems, bool> expandedItems)
		{
			UIHierarchyItem uIHierarchyItem;
			IEnumerator enumerator = items.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					UIHierarchyItem current = (UIHierarchyItem)enumerator.Current;
					if (current.Name != paths[index])
					{
						continue;
					}
					if (index != (int)paths.Length - 1)
					{
						int num = index + 1;
						index = num;
						uIHierarchyItem = ProjectUtilities.FindHierarchyItemByPath(current.UIHierarchyItems, paths, num, expandedItems);
						return uIHierarchyItem;
					}
					else
					{
						uIHierarchyItem = current;
						return uIHierarchyItem;
					}
				}
				expandedItems.Add(items, items.Expanded);
				return null;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return uIHierarchyItem;
		}

		private static uint FirstChild(IVsHierarchy hierarchy, uint rootID)
		{
			object obj = null;
			hierarchy.GetProperty(rootID, -1001, out obj);
			if (obj == null)
			{
				return (uint)-1;
			}
			return (uint)obj;
		}

		private static void GetAndSelectSolutionExplorerHierarchy(_DTE vs, out UIHierarchy hier, out UIHierarchyItem sol)
		{
			hier = null;
			sol = null;
			Window window = vs.Windows.Item("{3AE79031-E1BC-11D0-8F78-00A0C9110057}");
			if (window != null)
			{
				window.Activate();
				window.SetFocus();
				hier = window.Object as UIHierarchy;
				sol = hier.UIHierarchyItems.Item(1);
				if (sol == null)
				{
					throw new InvalidOperationException("No solution is opened.");
				}
				sol.Select(vsUISelectionType.vsUISelectionTypeSelect);
			}
		}

		public static CultureInfo GetCultureInfo(int lcid)
		{
			Dictionary<int, CultureInfo> nums = new Dictionary<int, CultureInfo>();
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
			for (int i = 0; i < (int)cultures.Length; i++)
			{
				CultureInfo cultureInfo = cultures[i];
				if (!nums.ContainsKey(cultureInfo.LCID))
				{
					nums.Add(cultureInfo.LCID, cultureInfo);
				}
			}
			if (!nums.ContainsKey(lcid))
			{
				return null;
			}
			return nums[lcid];
		}

		public static CultureInfo GetCultureInfo(string lcidText)
		{
			int num;
			if (!int.TryParse(lcidText, out num))
			{
				return null;
			}
			return ProjectUtilities.GetCultureInfo(num);
		}

		public static IVsPersistDocData GetDocumentInfo(string filePath)
		{
			uint num = 0;
			uint num1 = 0;
			IVsHierarchy vsHierarchy = null;
			IVsPersistDocData vsPersistDocDatum = null;
			VsShellUtilities.GetRDTDocumentInfo(ProjectUtilities._serviceProvider, filePath, ref vsHierarchy, ref num, ref vsPersistDocDatum, ref num1);
			return vsPersistDocDatum;
		}

		public static IVsHierarchy GetHierarchy(string projectPath)
		{
			if (string.IsNullOrEmpty(projectPath))
			{
				return null;
			}
			IVsHierarchy vsHierarchy = null;
			try
			{
				(ProjectUtilities._serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution).GetProjectOfUniqueName(projectPath, out vsHierarchy);
			}
			catch (Exception exception)
			{
			}
			return vsHierarchy;
		}

		public static IVsHierarchy GetHierarchyOfProject(EnvDTE.Project project)
		{
			if (project == null)
			{
				return null;
			}
			IVsHierarchy vsHierarchy = null;
			try
			{
				(ProjectUtilities._serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution).GetProjectOfUniqueName(project.FullName, out vsHierarchy);
			}
			catch (Exception exception)
			{
			}
			return vsHierarchy;
		}

		public static IVsHierarchy GetHierarchyOfProjectItem(EnvDTE.Project project, EnvDTE.ProjectItem item)
		{
			return null;
		}

		public static IVsProjectCfg2 GetProjectActiveConfiguration(IVsProject project)
		{
			IVsSolutionBuildManager service = ProjectUtilities._serviceProvider.GetService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager;
			if (service != null)
			{
				IVsProjectCfg2 vsProjectCfg2 = service.FindActiveProjectCfg(project as IVsHierarchy) as IVsProjectCfg2;
				if (vsProjectCfg2 != null)
				{
					return vsProjectCfg2;
				}
			}
			return null;
		}

		public static IVsProject GetProjectByFileName(string projectFile)
		{
			IVsProject vsProject;
			using (IEnumerator<IVsProject> enumerator = ProjectUtilities.LoadedProjects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IVsProject current = enumerator.Current;
					if (string.Compare(projectFile, ProjectUtilities.GetProjectFilePath(current), StringComparison.OrdinalIgnoreCase) != 0)
					{
						continue;
					}
					vsProject = current;
					return vsProject;
				}
				return null;
			}
			return vsProject;
		}

		public static IVsCfgProvider2 GetProjectConfigurationProvider(IVsProject project)
		{
			IVsProjectCfgProvider vsProjectCfgProvider;
			IVsSolutionBuildManager service = ProjectUtilities._serviceProvider.GetService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager;
			if (service != null)
			{
				IVsProjectCfg2 vsProjectCfg2 = service.FindActiveProjectCfg(project as IVsHierarchy) as IVsProjectCfg2;
				if (vsProjectCfg2 != null && ErrorHandler.Succeeded(vsProjectCfg2.get_ProjectCfgProvider(out vsProjectCfgProvider)) && vsProjectCfgProvider is IVsCfgProvider2)
				{
					return vsProjectCfgProvider as IVsCfgProvider2;
				}
			}
			return null;
		}

		public static string GetProjectFilePath(IVsProject project)
		{
			if (project == null)
			{
				return string.Empty;
			}
			string empty = string.Empty;
			if (project.GetMkDocument(-2, out empty) == 0)
			{
				return empty;
			}
			return string.Empty;
		}

		public static Guid GetProjectGuid(IVsHierarchy hierarchy)
		{
			Guid guid;
			hierarchy.GetGuidProperty(-2, -2059, out guid);
			return guid;
		}

		public static EnvDTE.ProjectItem GetProjectItemByPath(string path, ProjectItems projectItems)
		{
			EnvDTE.ProjectItem projectItemByPath = null;
			if (projectItems != null)
			{
				foreach (EnvDTE.ProjectItem projectItem in projectItems)
				{
					if (projectItem.SubProject != null)
					{
						continue;
					}
					string fileNames = null;
					try
					{
						fileNames = projectItem[0];
					}
					catch
					{
					}
					if (string.IsNullOrEmpty(fileNames) || !fileNames.Equals(path, StringComparison.OrdinalIgnoreCase))
					{
						projectItemByPath = ProjectUtilities.GetProjectItemByPath(path, projectItem.ProjectItems);
						if (projectItemByPath == null)
						{
							continue;
						}
						return projectItemByPath;
					}
					else
					{
						projectItemByPath = projectItem;
						return projectItemByPath;
					}
				}
			}
			return projectItemByPath;
		}

		public static string GetProjectItemFilePath(IVsProject project, uint itemID)
		{
			if (project == null)
			{
				return string.Empty;
			}
			string empty = string.Empty;
			if (project.GetMkDocument(itemID, out empty) == 0)
			{
				return empty;
			}
			return string.Empty;
		}

		private static IVsProject GetProjectOfItem(IVsHierarchy hierarchy, uint itemID)
		{
			return (IVsProject)hierarchy;
		}

		public static IList<IVsProject> GetProjectsOfCurrentSelections()
		{
			List<IVsProject> vsProjects = new List<IVsProject>();
			IVsMonitorSelection service = ProjectUtilities._serviceProvider.GetService(typeof(IVsMonitorSelection)) as IVsMonitorSelection;
			uint num = 0;
			IntPtr zero = IntPtr.Zero;
			IVsMultiItemSelect vsMultiItemSelect = null;
			IntPtr intPtr = IntPtr.Zero;
			service.GetCurrentSelection(out zero, out num, out vsMultiItemSelect, out intPtr);
			if (IntPtr.Zero != intPtr)
			{
				Marshal.Release(intPtr);
				intPtr = IntPtr.Zero;
			}
			if (num == -3)
			{
				uint num1 = 0;
				int num2 = 0;
				vsMultiItemSelect.GetSelectionInfo(out num1, out num2);
				VSITEMSELECTION[] vSITEMSELECTIONArray = new VSITEMSELECTION[num1];
				vsMultiItemSelect.GetSelectedItems(0, num1, vSITEMSELECTIONArray);
				VSITEMSELECTION[] vSITEMSELECTIONArray1 = vSITEMSELECTIONArray;
				for (int i = 0; i < (int)vSITEMSELECTIONArray1.Length; i++)
				{
					VSITEMSELECTION vSITEMSELECTION = vSITEMSELECTIONArray1[i];
					IVsProject projectOfItem = ProjectUtilities.GetProjectOfItem(vSITEMSELECTION.pHier, vSITEMSELECTION.itemid);
					if (!vsProjects.Contains(projectOfItem))
					{
						vsProjects.Add(projectOfItem);
					}
				}
			}
			else if (zero != IntPtr.Zero)
			{
				IVsHierarchy uniqueObjectForIUnknown = (IVsHierarchy)Marshal.GetUniqueObjectForIUnknown(zero);
				vsProjects.Add(ProjectUtilities.GetProjectOfItem(uniqueObjectForIUnknown, num));
			}
			return vsProjects;
		}

		public static IVsHierarchy GetSolutionWindowSelectedProject(out object propertiesObject, out IVsHierarchy parentProject, out bool multiSelection)
		{
			uint num;
			int num1;
			object obj;
			propertiesObject = null;
			parentProject = null;
			multiSelection = false;
			List<IVsProject> vsProjects = new List<IVsProject>();
			IVsMonitorSelection service = ProjectUtilities._serviceProvider.GetService(typeof(IVsMonitorSelection)) as IVsMonitorSelection;
			uint num2 = 0;
			IVsHierarchy uniqueObjectForIUnknown = null;
			IntPtr zero = IntPtr.Zero;
			IVsMultiItemSelect vsMultiItemSelect = null;
			IntPtr intPtr = IntPtr.Zero;
			service.GetCurrentSelection(out zero, out num2, out vsMultiItemSelect, out intPtr);
			if (IntPtr.Zero != intPtr)
			{
				Marshal.Release(intPtr);
				intPtr = IntPtr.Zero;
			}
			if (vsMultiItemSelect != null && vsMultiItemSelect.GetSelectionInfo(out num, out num1) == 0 && num1 > 0 && zero != IntPtr.Zero)
			{
				IVsHierarchy vsHierarchy = (IVsHierarchy)Marshal.GetUniqueObjectForIUnknown(zero);
				if (vsHierarchy != null && vsHierarchy.GetProperty(-2, -2018, out obj) == 0)
				{
					IVsBrowseObject vsBrowseObject = obj as IVsBrowseObject;
					if (vsBrowseObject != null)
					{
						uint num3 = 0;
						vsBrowseObject.GetProjectItem(out parentProject, out num3);
						if (parentProject != null)
						{
							multiSelection = true;
						}
					}
				}
			}
			if (num2 == -2 && zero != IntPtr.Zero)
			{
				uniqueObjectForIUnknown = (IVsHierarchy)Marshal.GetUniqueObjectForIUnknown(zero);
				uniqueObjectForIUnknown.GetProperty(-2, -2018, out propertiesObject);
			}
			return uniqueObjectForIUnknown;
		}

		public static IVsTextLines GetTextBufferFromRDT(string filePath, out IVsHierarchy hierarchy, out uint itemid, out uint docCookie)
		{
			hierarchy = null;
			itemid = 0;
			docCookie = 0;
			IntPtr zero = IntPtr.Zero;
			IVsTextLines uniqueObjectForIUnknown = null;
			IVsRunningDocumentTable service = null;
			try
			{
				service = ProjectUtilities._serviceProvider.GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
				if (service != null && service.FindAndLockDocument(1, filePath, out hierarchy, out itemid, out zero, out docCookie) == 0)
				{
					uniqueObjectForIUnknown = Marshal.GetUniqueObjectForIUnknown(zero) as IVsTextLines;
				}
			}
			finally
			{
				if (docCookie != 0 && service != null)
				{
					service.UnlockDocument(1, docCookie);
				}
			}
			return uniqueObjectForIUnknown;
		}

		public static string GetTextOfFileIfOpenInIde(string filePath)
		{
			string str;
			IVsRunningDocumentTable service = ProjectUtilities._serviceProvider.GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
			IVsHierarchy vsHierarchy = null;
			uint num = 0;
			IntPtr zero = IntPtr.Zero;
			uint num1 = 0;
			int num2 = service.FindAndLockDocument(1, filePath, out vsHierarchy, out num, out zero, out num1);
			try
			{
				if (num2 == 0)
				{
					IVsTextLines uniqueObjectForIUnknown = Marshal.GetUniqueObjectForIUnknown(zero) as IVsTextLines;
					if (uniqueObjectForIUnknown != null)
					{
						string str1 = null;
						int num3 = 0;
						int num4 = 0;
						uniqueObjectForIUnknown.GetLastLineIndex(out num3, out num4);
						uniqueObjectForIUnknown.GetLineText(0, 0, num3, num4, out str1);
						str = str1;
						return str;
					}
				}
				str = null;
			}
			finally
			{
				if (num1 != 0)
				{
					service.UnlockDocument(1, num1);
				}
			}
			return str;
		}

		public static string GetUniqueProjectNameFromFile(string projectFile)
		{
			IVsProject projectByFileName = ProjectUtilities.GetProjectByFileName(projectFile);
			if (projectByFileName == null)
			{
				return null;
			}
			return ProjectUtilities.GetUniqueUIName(projectByFileName);
		}

		public static string GetUniqueUIName(IVsProject project)
		{
			string str = null;
			(ProjectUtilities._serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution3).GetUniqueUINameOfProject((IVsHierarchy)project, out str);
			return str;
		}

		public static IVsSolution GetVsSolution()
		{
			return ProjectUtilities._serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
		}

		public static bool IsMSBuildProject(IVsProject project)
		{
			return ProjectCollection.GlobalProjectCollection.GetLoadedProjects(ProjectUtilities.GetProjectFilePath(project)).Count != 0;
		}

		public static bool IsNeutralLCID(string lcid)
		{
			if (string.IsNullOrEmpty(lcid))
			{
				return false;
			}
			if (lcid.Equals("0") || lcid.Equals("3072") || lcid.Equals("5120") || lcid.Equals("4096"))
			{
				return true;
			}
			return lcid.Equals("127");
		}

		public static bool IsNeutralLCID(int lcid)
		{
			if (lcid == 3072 || lcid == 5120 || lcid == 4096 || lcid == 0)
			{
				return true;
			}
			return lcid == 127;
		}

		private static UIHierarchyItem LocateProjectInUICollection(UIHierarchyItems items, EnvDTE.Project project, Dictionary<UIHierarchyItems, bool> expandedItems)
		{
			UIHierarchyItem uIHierarchyItem;
			if (items == null)
			{
				return null;
			}
			IEnumerator enumerator = items.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					UIHierarchyItem current = (UIHierarchyItem)enumerator.Current;
					if (!(current.Object is EnvDTE.Project) || !(current.Object as EnvDTE.Project).FullName.Equals(project.FullName, StringComparison.InvariantCultureIgnoreCase))
					{
						UIHierarchyItem uIHierarchyItem1 = ProjectUtilities.LocateProjectInUICollection(current.UIHierarchyItems, project, expandedItems);
						if (uIHierarchyItem1 == null)
						{
							continue;
						}
						uIHierarchyItem = uIHierarchyItem1;
						return uIHierarchyItem;
					}
					else
					{
						current.Select(vsUISelectionType.vsUISelectionTypeSelect);
						uIHierarchyItem = current;
						return uIHierarchyItem;
					}
				}
				expandedItems.Add(items, items.Expanded);
				return null;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return uIHierarchyItem;
		}

		private static uint NextSibling(IVsHierarchy hierarchy, uint firstID)
		{
			object obj = null;
			hierarchy.GetProperty(firstID, -1002, out obj);
			if (obj == null)
			{
				return (uint)-1;
			}
			return (uint)obj;
		}

		public static string NormalizeProjectOutputName(string outputName)
		{
			string empty = string.Empty;
			string str = outputName;
			for (int i = 0; i < str.Length; i++)
			{
				char chr = str[i];
				empty = (chr == '\u005F' || chr == '.' || char.IsLetterOrDigit(chr) ? string.Concat(empty, chr.ToString()) : string.Concat(empty, "_"));
			}
			if (!empty.Equals(outputName))
			{
				outputName = empty;
			}
			return outputName;
		}

		public static bool OpenFileInternal(string filePath, IVsHierarchy hierarchy, uint itemid, out uint docCookie, out bool exists)
		{
			IVsUIHierarchy vsUIHierarchy = null;
			object obj;
			docCookie = 0;
			exists = false;
			uint num = itemid;
			IVsWindowFrame vsWindowFrame = null;
			Guid lOGVIEWIDTextView = VSConstants.LOGVIEWID_TextView;
			if (VsShellUtilities.IsDocumentOpen(ProjectUtilities._serviceProvider, filePath, lOGVIEWIDTextView, ref vsUIHierarchy, ref num, ref vsWindowFrame))
			{
				exists = true;
				if (vsWindowFrame.GetProperty(-4000, out obj) == 0)
				{
					docCookie = (uint)obj;
				}
			}
			if (docCookie == 0)
			{
				IntPtr zero = IntPtr.Zero;
				IVsTextLines objectForIUnknown = null;
				IVsRunningDocumentTable service = null;
				try
				{
					ILocalRegistry localRegistry = (ILocalRegistry)ProjectUtilities._serviceProvider.GetService(typeof(SLocalRegistry));
					service = ProjectUtilities._serviceProvider.GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
					if (localRegistry != null && service != null)
					{
						Guid gUID = typeof(IVsTextLines).GUID;
						if (localRegistry.CreateInstance(typeof(VsTextBufferClass).GUID, null, ref gUID, 1, out zero) == 0)
						{
							objectForIUnknown = Marshal.GetObjectForIUnknown(zero) as IVsTextLines;
							((IObjectWithSite)objectForIUnknown).SetSite((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)ProjectUtilities._serviceProvider.GetService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider)));
							if (service.RegisterAndLockDocument(2, filePath, hierarchy, itemid, zero, out docCookie) == 0)
							{
								(objectForIUnknown as IVsPersistDocData).LoadDocData(filePath);
								service.NotifyDocumentChanged(docCookie, 65536);
							}
						}
					}
				}
				catch (Exception exception)
				{
					if (docCookie > 0 && service != null)
					{
						service.UnlockDocument(2, docCookie);
						docCookie = 0;
					}
					if (zero != IntPtr.Zero)
					{
						Marshal.Release(zero);
					}
				}
			}
			return docCookie != 0;
		}

		private static void RestoreExpandedItems(Dictionary<UIHierarchyItems, bool> expandedItems)
		{
			foreach (KeyValuePair<UIHierarchyItems, bool> expandedItem in expandedItems)
			{
				expandedItem.Key.Expanded = expandedItem.Value;
			}
		}

		public static UIHierarchyItem SelectItem(string path)
		{
			UIHierarchyItem item = null;
			_DTE service = ProjectUtilities._serviceProvider.GetService(typeof(DTE)) as _DTE;
			if (service != null)
			{
				UIHierarchy uIHierarchy = null;
				UIHierarchyItem uIHierarchyItem = null;
				try
				{
					ProjectUtilities.GetAndSelectSolutionExplorerHierarchy(service, out uIHierarchy, out uIHierarchyItem);
					if (uIHierarchyItem != null && uIHierarchy != null)
					{
						string str = Path.Combine(uIHierarchyItem.Name, path);
						item = uIHierarchy.GetItem(str);
					}
				}
				catch (ArgumentException argumentException)
				{
					if (uIHierarchyItem != null)
					{
						Dictionary<UIHierarchyItems, bool> uIHierarchyItems = new Dictionary<UIHierarchyItems, bool>();
						item = ProjectUtilities.FindHierarchyItemByPath(uIHierarchyItem.UIHierarchyItems, path.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }), 0, uIHierarchyItems);
						ProjectUtilities.RestoreExpandedItems(uIHierarchyItems);
					}
				}
				if (item != null)
				{
					item.UIHierarchyItems.Expanded = true;
					item.Select(vsUISelectionType.vsUISelectionTypeSelect);
				}
			}
			return item;
		}

		public static UIHierarchyItem SelectProject(EnvDTE.Project project)
		{
			UIHierarchy uIHierarchy;
			UIHierarchyItem uIHierarchyItem;
			if (project == null)
			{
				return null;
			}
			UIHierarchyItem uIHierarchyItem1 = null;
			ProjectUtilities.GetAndSelectSolutionExplorerHierarchy(project.DTE, out uIHierarchy, out uIHierarchyItem);
			if (uIHierarchy != null && uIHierarchyItem != null)
			{
				Dictionary<UIHierarchyItems, bool> uIHierarchyItems = new Dictionary<UIHierarchyItems, bool>();
				uIHierarchyItem1 = ProjectUtilities.LocateProjectInUICollection(uIHierarchyItem.UIHierarchyItems, project, uIHierarchyItems);
				ProjectUtilities.RestoreExpandedItems(uIHierarchyItems);
			}
			return uIHierarchyItem1;
		}

		public static bool SelectStartupProject()
		{
			object obj;
			ErrorHandler.Succeeded(((IVsMonitorSelection)ProjectUtilities._serviceProvider.GetService(typeof(SVsShellMonitorSelection))).GetCurrentElementValue(3, out obj));
			return true;
		}
	}
}